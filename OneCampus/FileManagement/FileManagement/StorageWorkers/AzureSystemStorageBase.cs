using FileManagement.StorageSkeleton;
using FileManagement.StorageStructure;
using FileManagement.Utility;
using Microsoft.Win32;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FileManagement.StorageWorkers
{
    /// <summary>
    /// Worker base for Azure File Systems to do File management operations.
    /// </summary>
    public class AzureSystemStorageBase
    {
        public const string FolderEntry = "$$$OneCampusDirectoryReference$$$.$$$";
        private readonly CloudStorageAccount storageAccount;
        private readonly string root;
        private readonly string absoluteRoot;

        public string ContainerName { get; protected set; }
        public CloudBlobClient BlobClient { get; private set; }
        public CloudBlobContainer Container { get; private set; }

        public AzureSystemStorageBase(string containerName, string cloudRoot, bool isPrivate, CloudStorageAccount cloudStorageAccount)
        {
            storageAccount = cloudStorageAccount;
            ContainerName = containerName;
            root = String.IsNullOrEmpty(cloudRoot) ? "" : root + "/";
            absoluteRoot = Combine(Combine(storageAccount.BlobEndpoint.AbsoluteUri, containerName), root);

            using (new HttpContextWeaver())
            {
                BlobClient = storageAccount.CreateCloudBlobClient();

                // Get and create the container if it does not exist
                // The container is named with DNS naming restrictions (i.e. all lower case)
                Container = BlobClient.GetContainerReference(ContainerName);

                Container.CreateIfNotExists();

                Container.SetPermissions(isPrivate
                                             ? new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off }
                                             : new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
            };
        }

        #region IFileStorageProvider Implementation
        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            Guard.IsNotBlank(path, "File Path");

            path = path ?? String.Empty;
            EnsurePathIsRelative(path);
            string prefix = Combine(Combine(Container.Name, root), path);

            if (!prefix.EndsWith("/"))
                prefix += "/";

            using (new HttpContextWeaver())
            {
                return BlobClient
                    .ListBlobs(prefix)
                    .OfType<CloudBlockBlob>()
                    .Where(blobItem => !blobItem.Uri.AbsoluteUri.EndsWith(FolderEntry))
                    .Select(blobItem => new AzureSystemStorageFile(blobItem, absoluteRoot))
                    .ToArray();
            }
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            path = path ?? String.Empty;
            EnsurePathIsRelative(path);
            using (new HttpContextWeaver())
            {
                // return root folders
                if (String.Concat(root, path) == String.Empty)
                {
                    return Container.ListBlobs()
                        .OfType<CloudBlobDirectory>()
                        .Select<CloudBlobDirectory, IStorageFolder>(d => new AzureSystemStorageFolder(d, absoluteRoot))
                        .ToList();
                }

                if (!Container.DirectoryExists(String.Concat(root, path)))
                {
                    try
                    {
                        CreateFolder(path);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(string.Format("The folder could not be created at path: {0}. {1}",
                                                                  path, ex));
                    }
                }

                return Container.GetDirectoryReference(String.Concat(root, path))
                    .ListBlobs()
                    .OfType<CloudBlobDirectory>()
                    .Select<CloudBlobDirectory, IStorageFolder>(d => new AzureSystemStorageFolder(d, absoluteRoot))
                    .ToList();
            }
        }

        public bool TryCreateFolder(string path)
        {
            try
            {
                if (!Container.DirectoryExists(String.Concat(root, path)))
                {
                    CreateFolder(path);
                    return true;
                }

                // return false to be consistent with FileSystemProvider's implementation
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void CreateFolder(string path)
        {
            EnsurePathIsRelative(path);
            using (new HttpContextWeaver())
            {
                Container.EnsureDirectoryDoesNotExist(String.Concat(root, path));

                // Creating a virtually hidden file to make the directory an existing concept
                CreateFile(Combine(path, FolderEntry));

                int lastIndex;
                while ((lastIndex = path.LastIndexOf('/')) > 0)
                {
                    path = path.Substring(0, lastIndex);
                    if (!Container.DirectoryExists(String.Concat(root, path)))
                    {
                        CreateFile(Combine(path, FolderEntry));
                    }
                }
            }
        }

        public void DeleteFolder(string path)
        {
            throw new NotImplementedException();
        }

        public void RenameFolder(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public void RenameFile(string path)
        {
            throw new NotImplementedException();
        }

        public IStorageFile CreateFile(string path)
        {
            EnsurePathIsRelative(path);

            if (Container.BlobExists(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName + extension;

                string pathWithoutFileName = path.Substring(0, path.LastIndexOf('/').Equals(-1) ? 0 : path.LastIndexOf('/'));
                path = pathWithoutFileName + "/" + uniqueFileName;
            }

            // create all folder entries in the hierarchy
            int lastIndex;
            var localPath = path;
            while ((lastIndex = localPath.LastIndexOf('/')) > 0)
            {
                localPath = localPath.Substring(0, lastIndex);
                var folder = Container.GetBlockBlobReference(String.Concat(root, Combine(localPath, FolderEntry)));
                folder.OpenWrite().Dispose();
            }

            var blob = Container.GetBlockBlobReference(String.Concat(root, path));
            var contentType = GetContentType(path);
            if (!String.IsNullOrWhiteSpace(contentType))
            {
                blob.Properties.ContentType = contentType;
            }

            using (var fileStream = new MemoryStream(new byte[0]))
            {
                blob.UploadFromStream(fileStream);
            }
            return new AzureSystemStorageFile(blob, absoluteRoot);
        }

        public IStorageFile GetFile(string path)
        {
            EnsurePathIsRelative(path);

            using (new HttpContextWeaver())
            {
                Container.EnsureBlobExists(String.Concat(root, path));
                return new AzureSystemStorageFile(Container.GetBlockBlobReference(String.Concat(root, path)), absoluteRoot);
            }
        }
        #endregion

        /// <summary>
        /// Returns the mime-type of the specified file path, looking into IIS configuration and the Registry
        /// </summary>
        private string GetContentType(string path)
        {
            string extension = Path.GetExtension(path);
            if (String.IsNullOrWhiteSpace(extension))
            {
                return "application/unknown";
            }

            try
            {
                string applicationHost = System.Environment.ExpandEnvironmentVariables(@"%windir%\system32\inetsrv\config\applicationHost.config");
                string webConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/").FilePath;

                // search for custom mime types in web.config and applicationhost.config
                foreach (var configFile in new[] { webConfig, applicationHost })
                {
                    if (File.Exists(configFile))
                    {
                        var xdoc = XDocument.Load(configFile);
                        var mimeMap = xdoc.XPathSelectElements("//staticContent/mimeMap[@fileExtension='" + extension + "']").FirstOrDefault();
                        if (mimeMap != null)
                        {
                            var mimeType = mimeMap.Attribute("mimeType");
                            if (mimeType != null)
                            {
                                return mimeType.Value;
                            }
                        }
                    }
                }

                // search into the registry
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(extension.ToLower());
                if (regKey != null)
                {
                    var contentType = regKey.GetValue("Content Type");
                    if (contentType != null)
                    {
                        return contentType.ToString();
                    }
                }
            }
            catch
            {
                // if an exception occured return application/unknown
                return "application/unknown";
            }

            return "application/unknown";
        }

        /// <summary>
        /// Formats the string with the given params.
        /// </summary>
        /// <param name="target">Target string to be formatted.</param>
        /// <param name="args">Args to be fomatted.</param>
        /// <returns>The Formatted string.</returns>
        private string T(string target, params object[] args)
        {
            return string.Format(target, args);
        }

        /// <summary>
        /// Ensures whether the path is relative.
        /// </summary>
        /// <param name="path">relative path of the file.</param>
        private static void EnsurePathIsRelative(string path)
        {
            if (path.StartsWith("/") || path.StartsWith("http://") || path.StartsWith("https://"))
                throw new ArgumentException("Path must be relative");
        }

        /// <summary>
        /// Combines both the paths.
        /// </summary>
        /// <param name="path1">First path to combine.</param>
        /// <param name="path2">Second path to be combined.</param>
        /// <returns>Combined path the be returned.</returns>
        public string CombinePath(string path1, string path2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rturns the public URL for the relative path.
        /// </summary>
        /// <param name="relativePath">The path for which the public URL to be rturne.d</param>
        /// <returns>Public URL for the given path.</returns>
        public string GetPublicURL(string relativePath)
        {
            EnsurePathIsRelative(relativePath);

            using (new HttpContextWeaver())
            {
                Container.EnsureBlobExists(String.Concat(root, relativePath));
                return Container.GetBlockBlobReference(String.Concat(root, relativePath)).Uri.ToString();
            }
        }

        /// <summary>
        /// Combines the path..
        /// </summary>
        /// <param name="path1">First path to combine.</param>
        /// <param name="path2">Second path to combine.</param>
        /// <returns>Combined path.</returns>
        public string Combine(string path1, string path2)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException("path1");
            }

            if (path2 == null)
            {
                throw new ArgumentNullException("path2");
            }

            if (String.IsNullOrEmpty(path2))
            {
                return path1;
            }

            if (String.IsNullOrEmpty(path1))
            {
                return path2;
            }

            if (path2.StartsWith("http://") || path2.StartsWith("https://"))
            {
                return path2;
            }

            var ch = path1[path1.Length - 1];

            if (ch != '/')
            {
                return (path1.TrimEnd('/') + '/' + path2.TrimStart('/'));
            }

            return (path1 + path2);
        }
    }
}
