using FileManagement.StorageSkeleton;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManagement.StorageStructure
{
    /// <summary>
    /// Azure System storage file which contains information like file name, type, size etc.
    /// </summary>
    public class AzureSystemStorageFile : IStorageFile
    {
        private CloudBlockBlob blob;
        private readonly string rootPath;

        public AzureSystemStorageFile(CloudBlockBlob cloudblob, string cloudRootPath)
        {
            blob = cloudblob;
            rootPath = cloudRootPath;
        }

        #region IStorage File Implementation
        public string GetPath()
        {
            return blob.Uri.ToString().Substring(rootPath.Length).Trim('/');
        }

        public string GetName()
        {
            return Path.GetFileName(GetPath());
        }

        public long GetSize()
        {
            return blob.Properties.Length;
        }

        public DateTime GetLastUpdated()
        {
            return blob.Properties.LastModified.Value.UtcDateTime;
        }

        public string GetFileType()
        {
            return Path.GetExtension(GetPath());
        }

        public Stream OpenRead()
        {
            return blob.OpenRead();
        }

        public Stream OpenWrite()
        {
            return blob.OpenWrite();
        } 
        #endregion
    }
}
