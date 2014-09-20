using FileManagement.StorageSkeleton;
using FileManagement.Utility;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace FileManagement.StorageStructure
{
    public class AzureSystemStorageFolder : IStorageFolder
    {
        private CloudBlobDirectory blob;
        private readonly string rootPath;

        public AzureSystemStorageFolder(CloudBlobDirectory cloudBlobDictionary, string cloudRootPath)
        {
            blob = cloudBlobDictionary;
            rootPath = cloudRootPath;
        }

        #region IStorage Folder Implementation
        public string GetName()
        {
            var path = GetPath();
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        public string GetPath()
        {
            using (new HttpContextWeaver())
            {
                return blob.Uri.ToString().Substring(rootPath.Length).Trim('/');
            }
        }

        public long GetSize()
        {
            using (new HttpContextWeaver())
            {
                return GetDirectorySize(blob);
            }
        }

        public DateTime GetLastUpdated()
        {
            return DateTime.MinValue;
        } 
        #endregion

        /// <summary>
        /// Returns the directory size of the directory blob.
        /// </summary>
        /// <param name="directoryBlob">The directory blob to return the size.</param>
        /// <returns>Size of the direcotyr.</returns>
        private static long GetDirectorySize(CloudBlobDirectory directoryBlob)
        {
            long size = 0;

            foreach (var blobItem in directoryBlob.ListBlobs())
            {
                if (blobItem is CloudPageBlob)
                    size += ((CloudPageBlob)blobItem).Properties.Length;

                if (blobItem is CloudBlobDirectory)
                    size += GetDirectorySize((CloudBlobDirectory)blobItem);
            }

            return size;
        }
    }
}
