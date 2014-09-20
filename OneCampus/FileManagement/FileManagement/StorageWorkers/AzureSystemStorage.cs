using FileManagement.StorageSkeleton;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace FileManagement.StorageWorkers
{
    public class AzureSystemStorage : AzureSystemStorageBase, IFileStorageProvider
    {
        private readonly static string defaultStoreFolder = "orchardmediastore";

        private readonly string storagePath = string.Empty;
        private readonly string publicPath = string.Empty;

        public static AzureSystemStorage Instance(string storagePath = null)
        {
            return new AzureSystemStorage(!string.IsNullOrEmpty(storagePath) ? storagePath : defaultStoreFolder);
        }

        public AzureSystemStorage(string storePath)
            : this(storePath, "", CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString")))
        { }

        public AzureSystemStorage(string storePath, string cloudRoot, CloudStorageAccount storageAccount)
            : base(storePath, cloudRoot, false, storageAccount) { }

        /// <summary>
        /// Tries to save the stream into the given location.
        /// </summary>
        /// <param name="path">Path location to upload the Stream.</param>
        /// <param name="inputStream">Stream with file contents.</param>
        /// <param name="uploadedPath">Uploaded path and file name. {This may change if the file already exists. A new name is automatically provided by the Component.}</param>
        /// <returns>Is Successfully uploaded or not.</returns>
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
        /// Saves the stream to the given location.
        /// </summary>
        /// <param name="path">Path location to upload the Stream.</param>
        /// <param name="inputStream">Stream with file contents.</param>
        /// <param name="uploadedPath">Uploaded path and file name. {This may change if the file already exists. A new name is automatically provided by the Component.}</param>
        public void SaveStream(string path, System.IO.Stream inputStream, ref string uploadedPath)
        {
            // Create the file.
            // The CreateFile method will map the still relative path
            var file = CreateFile(path);
            uploadedPath = file.GetPath();

            using (var outputStream = file.OpenWrite())
            {
                var buffer = new byte[8192];
                for (; ; )
                {
                    var length = inputStream.Read(buffer, 0, buffer.Length);
                    if (length <= 0)
                        break;
                    outputStream.Write(buffer, 0, length);
                }
            }
        }
    }
}
