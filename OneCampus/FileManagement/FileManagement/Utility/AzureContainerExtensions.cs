using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;

namespace FileManagement.Utility
{
    /// <summary>
    /// Extension Class for Azure specific Extensions.
    /// </summary>
    public static class AzureContainerExtensions
    {
        /// <summary>
        /// Extension method for identifying whether the blob exists or not.
        /// </summary>
        /// <param name="container">Container on which the operation is to happen.</param>
        /// <param name="path">The path on which the operation is to happen.</param>
        /// <returns>Whether the provided blob exists or not.</returns>
        public static bool BlobExists(this CloudBlobContainer container, string path)
        {
            if (String.IsNullOrEmpty(path) || path.Trim() == String.Empty)
                throw new ArgumentException("Path can't be empty");

            bool isBlobExists = false;
            try
            {
                using (new HttpContextWeaver())
                {
                    var blob = container.GetBlobReferenceFromServer(path);
                    isBlobExists = blob.Exists();
                    blob.FetchAttributes();
                }
                return isBlobExists;
            }
            catch
            {
                return isBlobExists;
            }
        }

        /// <summary>
        /// Ensure that a blob exsits.
        /// </summary>
        /// <param name="container">Container on which the operation is to happen.</param>
        /// <param name="path">The path on which the operation is to happen.</param>
        public static void EnsureBlobExists(this CloudBlobContainer container, string path)
        {
            if (!BlobExists(container, path))
                throw new ArgumentException("File " + path + "does not exists");
        }

        /// <summary>
        /// Checks whether the Directory exists or not.
        /// </summary>
        /// <param name="container">Container on which the operation is to happen.</param>
        /// <param name="path">The path on which the operation is to happen.</param>
        /// <returns>True when the Search Directory exists..</returns>
        public static bool DirectoryExists(this CloudBlobContainer container, string path)
        {
            Guard.IsNotBlank(path, "Path");
            return container.GetDirectoryReference(path).ListBlobs().Count() > 0;
        }

        /// <summary>
        /// Ensures that the directory does not exists.
        /// </summary>
        /// <param name="container">Container on which the operation is to happen.</param>
        /// <param name="path">The path on which the operation is to happen.</param>
        public static void EnsureDirectoryDoesNotExist(this CloudBlobContainer container, string path)
        {
            if (DirectoryExists(container, path))
            {
                throw new ArgumentException("Directory " + path + " already exists");
            }
        }
    }
}
