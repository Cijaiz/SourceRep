using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace MailingService
{
    /// <summary>
    /// Helper methods for Azure.
    /// </summary>
    public static class AzureHelper
    {
        public static string AzureConnectionString { private get; set; }
        public static string AzureStorageConnectionString = "MailAttachmentStorageAccount";

       

        /// <summary>
        /// Downloads contents from blob as a stream
        /// </summary>
        /// <param name="blobUrl">Blog Url</param>
        /// <returns>Returns stream</returns>
        public static Stream DownloadStreamFromBlob(string blobUrl, out string attachedFileName, out string contentType)
        {
            attachedFileName = "";
            contentType = MediaTypeNames.Text.Plain;

            //Construct URI from blob Url string
            Uri uri = new Uri(blobUrl);

            //Split the url to get the container name from the blob url.
            var segments = uri.Segments
           .Select(s => s.EndsWith("/") ? s.Substring(0, s.Length - 1) : s)
           .ToArray();
            String containerName = segments[1];
            Stream contents = null;

            //Set connection string 
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(AzureStorageConnectionString));
            if (storageAccount != null)
            {
                contents = new MemoryStream();

                CloudBlobClient client = storageAccount.CreateCloudBlobClient();
                //Create container reference
                CloudBlobContainer container = client.GetContainerReference(containerName);
                container.CreateIfNotExists();

                // Get reference to the blob          
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobUrl);

                IRetryPolicy exponentialRetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);
                client.RetryPolicy = exponentialRetryPolicy;
                
                //Download the blob content as stream
                blockBlob.DownloadToStream(contents);
      
                contents.Position = 0;

                //Assign metadata values
                if (blockBlob.Metadata["FileName"] != null)
                    attachedFileName = blockBlob.Metadata["FileName"];
                if (blockBlob.Properties.ContentType != null)
                    contentType = blockBlob.Properties.ContentType;
            }
            return contents;
        }
    }
}