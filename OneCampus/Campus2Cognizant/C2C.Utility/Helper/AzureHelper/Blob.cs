using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace C2C.Core.Helper.AzureHelper
{
    public class Blob
    {
        private CloudBlobClient cloudblobClient = null;
        private CloudStorageAccount cloudStorageAccount = null;

        private Blob(string storageConnectionString)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            cloudblobClient = cloudStorageAccount.CreateCloudBlobClient();
            IRetryPolicy exponentialRetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);
            cloudblobClient.RetryPolicy = exponentialRetryPolicy;
        }

        public static Blob GetInstance(string storageConnectionString)
        {
            return new Blob(storageConnectionString);
        }
        /// <summary>
        /// Creates a Blob Container
        /// </summary>
        /// <param name="blobContainerName">Blob Container Name</param>
        /// <param name="isPublic">bool value denoting whether the container is public </param>
        /// <returns></returns>
        public bool CreateBlobContainer(string blobContainerName, bool isPublic)
        {
            try
            {
                CloudBlobContainer container = cloudblobClient.GetContainerReference(blobContainerName);
                container.CreateIfNotExists();
                if (isPublic)
                {
                    container.SetPermissions(
                      new BlobContainerPermissions
                      {
                          PublicAccess = BlobContainerPublicAccessType.Blob
                      });
                }
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {
                    return false;
                }
                throw;
            }
        }
        /// <summary>
        /// Downloads a blob to the specified filepath
        /// </summary>
        /// <param name="containerName">Name of container of the Blob</param>
        /// <param name="blobName">Name of the blob to download</param>
        /// <param name="filePath">Path where the blob will be downloaded</param>
        /// <returns></returns>
        public bool DownloadFileFromBlob(string containerName, string blobName, string filePath)
        {
            FileStream stream = null;
            CloudBlobContainer container = cloudblobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            using (stream = System.IO.File.OpenWrite(filePath))
            {
                blockBlob.DownloadToStream(stream);
            }
            return true;
        }
    }
}
