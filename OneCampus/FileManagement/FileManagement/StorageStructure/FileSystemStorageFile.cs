using FileManagement.StorageSkeleton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManagement.StorageStructure
{
    public class FileSystemStorageFile : IStorageFile
    {
        private readonly string path;
        private readonly FileInfo fileInfo;

        public FileSystemStorageFile(string filePath, FileInfo fileInformation)
        {
            path = filePath;
            fileInfo = fileInformation;
        }

        #region IStorageFile Implementation
        public string GetName()
        {
            return fileInfo.Name;
        }

        public string GetPath()
        {
            return path;
        }

        public DateTime GetLastUpdated()
        {
            return fileInfo.LastWriteTime;
        }

        public string GetFileType()
        {
            return fileInfo.Extension;
        }

        public long GetSize()
        {
            return fileInfo.Length;
        }

        public System.IO.Stream OpenRead()
        {
            return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        }

        public System.IO.Stream OpenWrite()
        {
            return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
        } 
        #endregion
    }
}
