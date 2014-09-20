
namespace FileManagement.StorageSkeleton
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IStorageFolder
    {
        /// <summary>
        /// REturns the path of the Folder.
        /// </summary>
        /// <returns>The file path of the Folder is returned.</returns>
        string GetPath();

        /// <summary>
        /// Returns the File name of the Folder.
        /// </summary>
        /// <returns>The file name of the Folder is returned.</returns>
        string GetName();

        /// <summary>
        /// Returns the Size of the Folder.
        /// </summary>
        /// <returns>The SIze of the Folder is returned.</returns>
        long GetSize();

        /// <summary>
        /// Returns the last updated time of the Folder.
        /// </summary>
        /// <returns>The last updated time ofthe Folder.</returns>
        DateTime GetLastUpdated();
    }
}
