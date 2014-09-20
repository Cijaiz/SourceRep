using System;
using System.Collections.Generic;
using System.IO;

namespace FileManagement.StorageSkeleton
{
    public interface IFileStorageProvider
    {
        /// <summary>
        /// List all the files at the given folder location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        /// <returns>List of IStorageFiles retrived from the given traverse location.</returns>
        IEnumerable<IStorageFile> ListFiles(string path);

        /// <summary>
        /// List all the Folders at the given folder loction.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        /// <returns>List of IStorageFolder retrived from the given traverse location.</returns>
        IEnumerable<IStorageFolder> ListFolders(string path);

        /// <summary>
        /// Tries to create Folder at a particular location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        /// <returns>Whether the folder is created or not.</returns>
        bool TryCreateFolder(string path);

        /// <summary>
        /// Creates the Folder at a particular location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        void CreateFolder(string path);

        /// <summary>
        /// Deletes the Folder from the given location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        void DeleteFolder(string path);

        /// <summary>
        /// Renames the Folder in the Given location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        void RenameFolder(string path);

        /// <summary>
        /// Deletes the File from the given location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Renames the File from the given location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        void RenameFile(string path);

        /// <summary>
        /// Creates a File in the specified location.
        /// </summary>
        /// <param name="path">The path of the traverse location.</param>
        /// <returns></returns>
        IStorageFile CreateFile(string path);

        /// <summary>
        /// Retrieves a file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file within the storage provider.</param>
        /// <returns>The IStorage file.</returns>
        /// <exception cref="ArgumentException">If the file is not found.</exception>
        IStorageFile GetFile(string path);

        /// <summary>
        /// Tries to save the stream into the given location.
        /// </summary>
        /// <param name="path">Path location to upload the Stream.</param>
        /// <param name="inputStream">Stream with file contents.</param>
        /// <param name="uploadedPath">Uploaded path and file name. {This may change if the file already exists. A new name is automatically provided by the Component.}</param>
        /// <returns>Is Successfully uploaded or not.</returns>
        bool TrySaveStream(string path, Stream inputStream, ref string uploadedPath);

        /// <summary>
        /// Saves the stream to the given location.
        /// </summary>
        /// <param name="path">Path location to upload the Stream.</param>
        /// <param name="inputStream">Stream with file contents.</param>
        /// <param name="uploadedPath">Uploaded path and file name. {This may change if the file already exists. A new name is automatically provided by the Component.}</param>
        void SaveStream(string path, Stream inputStream, ref string uploadedPath);

        /// <summary>
        /// Combines and Returns path1 and path2.
        /// </summary>
        /// <param name="path1">First path to combine.</param>
        /// <param name="path2">Second path to combine.</param>
        /// <returns>Combined path</returns>
        string CombinePath(string path1, string path2);

        /// <summary>
        /// Returns the publicly mapped URL for the provided relative path.
        /// </summary>
        /// <param name="relativePath">The relative path for which the public URL is to be retunred.</param>
        /// <returns>Public URL mapped to the Given relative path.</returns>
        string GetPublicURL(string relativePath);
    }
}
