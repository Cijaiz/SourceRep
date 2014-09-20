using FileManagement.StorageSkeleton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManagement.StorageStructure
{
    public class FileSystemStorageFolder : IStorageFolder
    {
        private readonly string path;
        private readonly DirectoryInfo directoryInfo;

        public FileSystemStorageFolder(string filePath, DirectoryInfo directoryInformation)
        {
            path = filePath;
            directoryInfo = directoryInformation;
        }

        #region IStorageFolder Implementation
        public string GetName()
        {
            return directoryInfo.Name;
        }

        public string GetPath()
        {
            return path;
        }

        public DateTime GetLastUpdated()
        {
            return directoryInfo.LastWriteTime;
        }

        public long GetSize()
        {
            return GetDirectorySize(directoryInfo); ;
        } 
        #endregion

        /// <summary>
        /// Returns whether the file is hidden or not.
        /// </summary>
        /// <param name="di">The file info to be validated.</param>
        /// <returns>True when its hidden.</returns>
        private static bool IsHidden(FileSystemInfo di)
        {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }

        /// <summary>
        /// Returns the directory size of the directory blob.
        /// </summary>
        /// <param name="directoryBlob">The directory blob to return the size.</param>
        /// <returns>Size of the direcotyr.</returns>
        private static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            long size = 0;

            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!IsHidden(fileInfo))
                {
                    size += fileInfo.Length;
                }
            }
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dInfo in directoryInfos)
            {
                if (!IsHidden(dInfo))
                {
                    size += GetDirectorySize(dInfo);
                }
            }

            return size;
        }
    }
}
