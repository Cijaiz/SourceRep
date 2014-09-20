
namespace FileManagement.StorageSkeleton
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public interface IStorageFile
    {
        /// <summary>
        /// Returns the File name of the file.
        /// </summary>
        /// <returns>The file name of the file is returned.</returns>
        string GetName();

        /// <summary>
        /// REturns the File path of the file.
        /// </summary>
        /// <returns>The file path of the file is returned.</returns>
        string GetPath();

        /// <summary>
        /// Returns the last updated time of the File.
        /// </summary>
        /// <returns>The last updated time ofthe file.</returns>
        DateTime GetLastUpdated();

        /// <summary>
        /// Returns th eFile tyupe of the file.
        /// </summary>
        /// <returns>The file type of the file.</returns>
        string GetFileType();

        /// <summary>
        /// Returns the Size of the File.
        /// </summary>
        /// <returns>The SIze of the file is returned.</returns>
        long GetSize();

        /// <summary>
        /// Opens the File ins Read only mode.
        /// </summary>
        /// <returns>Memory stream of the file contents.</returns>
        Stream OpenRead();

        /// <summary>
        /// Opens the File ins Read write mode.
        /// </summary>
        /// <returns>Memory stream of the file Contents.</returns>
        Stream OpenWrite();
    }
}
