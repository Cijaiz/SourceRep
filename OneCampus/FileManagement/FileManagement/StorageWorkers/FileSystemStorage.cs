using FileManagement.StorageSkeleton;
using FileManagement.StorageStructure;
using FileManagement.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace FileManagement.StorageWorkers
{
    /// <summary>
    /// Worker for File Systems to do File management operations.
    /// </summary>
    public class FileSystemStorage : IFileStorageProvider
    {
        private readonly static string defaultStoreFolder = "Media";
        private readonly string storagePath = string.Empty;
        private readonly string publicPath = string.Empty;

        public static FileSystemStorage Instance(string storagePath = null)
        {
            return new FileSystemStorage(!string.IsNullOrEmpty(storagePath) ? storagePath : defaultStoreFolder);
        }

        public FileSystemStorage(string storePath)
        {
            Guard.IsNotBlank(storePath, "storePath");

            storagePath = HostingEnvironment.IsHosted
                            ? HostingEnvironment.MapPath("~/" + defaultStoreFolder + "/") ?? ""
                            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultStoreFolder);

            string appPath = "";
            if (HostingEnvironment.IsHosted)
            {
                //appPath = HostingEnvironment.ApplicationVirtualPath;
                Uri originalUri = System.Web.HttpContext.Current.Request.Url;
                string urlScheme = originalUri.Scheme;
                string urlHost = originalUri.Host;
                int urlPort = originalUri.Port;

                appPath = string.Format("{0}://{1}{2}/", urlScheme, urlHost, (urlPort > 0 ? ":" + urlPort : string.Empty));
            }

            if (!appPath.EndsWith("/"))
                appPath = appPath + '/';
            //if (!appPath.StartsWith("/"))
            //    appPath = '/' + appPath;

            publicPath = string.Concat(appPath, defaultStoreFolder, "/");

            //Create Default Directories..
            TryCreateFolder(storagePath);
        }

        #region IFileStorageProvider Implementation
        public string GetPublicURL(string relativePath)
        {
            return MapPublic(relativePath);
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists)
            {
                if (!TryCreateFolder(path))
                {
                    throw new ArgumentException(T("Unable to create Directory {0}.", path).ToString());
                }
            }

            return directoryInfo
                .GetFiles()
                .Where(fi => !IsHidden(fi))
                .Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(MapPublic(Path.Combine((path), fi.Name)), fi))
                .ToList();
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateFolder(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
                if (directoryInfo.Exists)
                    return true;

                CreateFolder(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void CreateFolder(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
            if (directoryInfo.Exists)
            {
                throw new ArgumentException(T("Directory {0} already exists", path).ToString());
            }

            Directory.CreateDirectory(directoryInfo.FullName);
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

        public IStorageFile GetFile(string path)
        {
            FileInfo fileInfo = new FileInfo(MapStorage(path));
            if (!fileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} does not exist", path).ToString());
            }

            return new FileSystemStorageFile(MapPublic(path), fileInfo);
        }

        /// <summary>
        /// Creates a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <exception cref="ArgumentException">If the file already exists.</exception>
        /// <returns>The created file.</returns>
        public IStorageFile CreateFile(string path)
        {
            FileInfo fileInfo = new FileInfo(MapStorage(path));
            if (fileInfo.Exists)
            {
                //throw new ArgumentException(T("File {0} already exists", fileInfo.Name).ToString());
                string fileName = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName + extension;

                string pathWithoutFileName = path.Substring(0, path.LastIndexOf('/'));
                path = pathWithoutFileName + "/" + uniqueFileName;
                fileInfo = new FileInfo(MapStorage(path));
            }

            // ensure the directory exists
            var dirName = Path.GetDirectoryName(fileInfo.FullName);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllBytes(fileInfo.FullName, new byte[0]);

            return new FileSystemStorageFile(path, fileInfo);
        }

        /// <summary>
        /// Tries to save a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <returns>True if success; False otherwise.</returns>
        public bool TrySaveStream(string path, System.IO.Stream inputStream, ref string uploadedPath)
        {
            try
            {
                SaveStream(path, inputStream, ref uploadedPath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <exception cref="ArgumentException">If the stream can't be saved due to access permissions.</exception>
        public void SaveStream(string path, System.IO.Stream inputStream, ref string uploadedPath)
        {
            // Create the file.
            // The CreateFile method will map the still relative path
            var file = CreateFile(path);
            uploadedPath = file.GetPath();

            var outputStream = file.OpenWrite();
            var buffer = new byte[8192];
            for (; ; )
            {

                var length = inputStream.Read(buffer, 0, buffer.Length);
                if (length <= 0)
                    break;
                outputStream.Write(buffer, 0, length);
            }
            outputStream.Dispose();
        }

        /// <summary>
        /// Combines to paths.
        /// </summary>
        /// <param name="path1">The parent path.</param>
        /// <param name="path2">The child path.</param>
        /// <returns>The combined path.</returns>
        public string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }
        #endregion

        #region Private Method Operations.
        /// <summary>
        /// Maps the Storage path with the given path.
        /// </summary>
        /// <param name="path">The relative path for which mapping should be done.</param>
        /// <returns>The Mapped path.</returns>
        private string MapStorage(string path)
        {
            string mappedStoragePath = string.IsNullOrEmpty(path) ? storagePath : Path.Combine(storagePath, path);
            return mappedStoragePath;
        }

        /// <summary>
        /// Maps the public path with the given relative path.
        /// </summary>
        /// <param name="path">The relative path for which mapping should be done.</param>
        /// <returns>The Mapped Public path.</returns>
        private string MapPublic(string path)
        {
            return string.IsNullOrEmpty(path) ? publicPath : Path.Combine(publicPath, path).Replace(Path.DirectorySeparatorChar, '/');
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
        /// Returns whether the File is Hidden or not.
        /// </summary>
        /// <param name="di">File Information.</param>
        /// <returns>True when hidden.</returns>
        private static bool IsHidden(FileSystemInfo di)
        {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }
        #endregion
    }
}
