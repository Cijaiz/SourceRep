using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using TFH = Microsoft.Practices.TransientFaultHandling;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Linq;
using C2C.Core.Logger;
using C2C.Core.Helper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Octane.NotificationUtility
{
    /// <summary>
    /// Helper methods for Azure.
    /// </summary>
    public static class AzureHelper
    {
        public const string NotificationEngineStorage = "NotificationEngineStorage";
        public const string InstanceBlobContainerName = "InstanceBlobContainerName";
        public const string InstanceBlobName = "InstanceBlobName";
        public const string LeaseBlobName = "leaseblob";
        public const string LeaseBlobText = "leaseblobtext";
        public const string Seperator = ",";
        public static string AzureConnectionString { private get; set; }       

        private static DateTime? lifeTime = null;
        /// <summary>
        /// Get the retry policy for the transient faults handling..
        /// </summary>
        public static TFH.RetryPolicy<StorageTransientErrorDetectionStrategy> StorageRetryPolicy
        {
            get
            {
                //Define Strategy on Retry behaviour.
                var retryStrategy = new TFH.Incremental(5, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
                var retryPolicy = new TFH.RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

                //Receive Notifications about Retries..
                retryPolicy.Retrying += (sender, args) =>
                    {
                        var message = string.Format("Retry - Count:{0}, Delay:{1}, Exception{2}",
                            args.CurrentRetryCount, args.Delay, args.LastException);
                    };

                return retryPolicy;
            }
        }

        /// <summary>
        ///Downloads message from Queue
        /// <param name="queueName">Queue Name</param>
        /// <param name="messageCount">Message Count</param>
        /// <returns>Downloaded Messages</returns>
        /// </summary>
        public static IEnumerable<CloudQueueMessage> DownloadMessagesFromQueue(string queueName, int messageCount)
        {
            Guard.IsNotBlank(queueName, "queueName");

            IEnumerable<CloudQueueMessage> downloadedMessages = null;
            Microsoft.WindowsAzure.Storage.CloudStorageAccount myStorage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(AzureConnectionString);
            CloudQueueClient queueClient = myStorage.CreateCloudQueueClient();

            AzureHelper.StorageRetryPolicy.ExecuteAction(()
                =>
            {
                CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
                downloadedMessages = queue.GetMessages(messageCount, TimeSpan.FromSeconds(30));
            });
            return downloadedMessages;
        }

        /// <summary>
        //CreateQueueIfNotExists
        /// <param name="queueName">Queue Name</param>
        /// </summary>
        public static void CreateQueueIfNotExists(string queueName)
        {
            Guard.IsNotBlank(queueName, "queueName");

            CloudStorageAccount myStorage = CloudStorageAccount.Parse(AzureConnectionString);
            CloudQueueClient queueClient = myStorage.CreateCloudQueueClient();

            AzureHelper.StorageRetryPolicy.ExecuteAction(()
                =>
            {
                CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
                queue.CreateIfNotExists();
            });


            #region Async Operation for Queue Creation
            //CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
            //AzureHelper.StorageRetryPolicy.ExecuteAction(
            //    beginAsync =>
            //    {
            //        queue.BeginCreateIfNotExist(beginAsync, null);
            //    },
            //    endAsync =>
            //    {
            //        return queue.EndCreateIfNotExist(endAsync);
            //    },
            //    asyncResult =>
            //    {
            //        if (asyncResult)
            //        {
            //            Logger.Info(string.Format("Queue Name {0} is created.", queueName.ToLower()));
            //        }
            //        else
            //        {
            //            Logger.Info(string.Format("Queue Name {0} is not created.", queueName.ToLower()));
            //        }
            //    },
            //    exception =>
            //    {
            //        var message = string.Format("Create Queue Name {0} is failed", exception.ToString());
            //        Logger.Error(string.Format("Queue Name {0} is not created.", queueName.ToLower()), exception);
            //    }); 
            #endregion
        }

        /// <summary>
        ///Upload message into Queue
        /// <param name="queueName">Queue Name</param>
        /// <param name="messageCount">Message Count</param>
        /// </summary>
        public static void UploadMessagesIntoQueue(string queueName, CloudQueueMessage queueMessage)
        {
            Guard.IsNotBlank(queueName, "queueName");
            Guard.IsNotNull(queueMessage, "queueMessage");

            CloudStorageAccount myStorage = CloudStorageAccount.Parse(AzureConnectionString);
            CloudQueueClient queueClient = myStorage.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
            AzureHelper.StorageRetryPolicy.ExecuteAction(()
                =>
                    {
                        queue.AddMessage(queueMessage);
                    });

            ////Asynchronous operation for uploading queue messages..
            //AzureHelper.StorageRetryPolicy.ExecuteAction(
            //    beginAsync =>
            //    {
            //        queue.BeginAddMessage(queueMessage, beginAsync, null);
            //    },
            //    endAsync =>
            //    {
            //        queue.EndAddMessage(endAsync);
            //    },
            //    () =>
            //    {
            //        Logger.Info(string.Format("Message : {0} successfully uploaded in Queue: {1}.", queueMessage.Id, queueName));
            //    },
            //    exception =>
            //    {
            //        var message = string.Format("Message : {0} Upload Failed in Queue {1}.", queueMessage.Id, queueName);
            //        Logger.Error(message, exception);
            //    });
        }

        /// <summary>
        ///Get Queue Messages Count
        /// <param name="queueName">Queue Name</param>
        /// <returns>Queue Messages Count</returns>
        /// </summary>
        public static int GetQueueMessagesCount(string queueName)
        {
            int messageCount = 0;

            Guard.IsNotBlank(queueName, "queueName");

            CloudStorageAccount myStorage = CloudStorageAccount.Parse(AzureConnectionString);
            CloudQueueClient queueClient = myStorage.CreateCloudQueueClient();

            AzureHelper.StorageRetryPolicy.ExecuteAction(()
                =>
            {
                CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
                queue.FetchAttributes();
                messageCount = Convert.ToInt32(queue.ApproximateMessageCount);
            });
            return messageCount;
        }

        /// <summary>
        ///Delete Queue Message
        /// <param name="queueName">Queue Name</param>
        /// <param name="messageToBeDeleted">Message To Be Deleted</param>
        /// </summary>
        public static void DeleteQueueMessage(string queueName, CloudQueueMessage messageToBeDeleted)
        {
            Guard.IsNotBlank(queueName, "queueName");
            Guard.IsNotNull(messageToBeDeleted, "messageToBeDeleted");

            CloudStorageAccount myStorage = CloudStorageAccount.Parse(AzureConnectionString);
            CloudQueueClient queueClient = myStorage.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference(queueName.ToLower());
            AzureHelper.StorageRetryPolicy.ExecuteAction(()
                =>
            {
                queue.DeleteMessage(messageToBeDeleted.Id, messageToBeDeleted.PopReceipt);
            });

            //Asynchronous operation for deleting queue messages..
            //AzureHelper.StorageRetryPolicy.ExecuteAction(
            //    beginAsync =>
            //    {
            //        queue.BeginDeleteMessage(messageToBeDeleted, beginAsync, null);
            //    },
            //    endAsync =>
            //    {
            //        queue.EndDeleteMessage(endAsync);
            //    },
            //    () =>
            //    {
            //        Logger.Info(string.Format("Message : {0} successfully deleted in Queue: {1}.", messageToBeDeleted.Id, queueName));
            //    },
            //    exception =>
            //    {
            //        var message = string.Format("Message : {0} delete Failed in Queue {1}.", messageToBeDeleted.Id, queueName);
            //        Logger.Error(message, exception);
            //    });
        }

        /// <summary>
        /// Downloads contents from blob as a stream
        /// </summary>
        /// <param name="blobUrl">Blog Url</param>
        /// <returns>Returns stream</returns>
        public static Stream DownloadContentsFromBlob(string blobUrl)
        {
            //Null check
            Guard.IsNotBlank(blobUrl, "blob template Url");

            Stream contents = new MemoryStream();
            // Get reference to the blob
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUrl));
            AzureHelper.StorageRetryPolicy.ExecuteAction(() =>
                {
                    blob.DownloadToStream(contents);
                });

            contents.Position = 0;
            return contents;
        }

        /// <summary>
        ///  Checks on the blob to determine the current owner & his lifetime.
        ///  If the lifetime is expired for the current owner, the calling instance updates itself as the owner and sets lifetime for it.
        ///  Otherwise,it goes for a sleep.
        ///  Lifetime is for the next 2 mins.
        /// </summary>
        /// <param name="containerName">Container Name</param>
        public static bool IsInstanceOwner()
        {
            bool isOwner = false;
            try
            {

                string leaseId = String.Empty;
                CloudStorageAccount storage = CloudStorageAccount.Parse(AzureConnectionString);
                CloudBlobClient blobClient = storage.CreateCloudBlobClient();
                CloudBlockBlob blockBlob = null;
                CloudBlockBlob tempBlob = null;
                CloudBlobContainer container = null;

                AzureHelper.StorageRetryPolicy.ExecuteAction(()
                    =>
                {

                    container = blobClient.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue(InstanceBlobContainerName));
                    container.CreateIfNotExists();
                    blockBlob = container.GetBlockBlobReference(RoleEnvironment.GetConfigurationSettingValue(InstanceBlobName));
                    tempBlob = container.GetBlockBlobReference(LeaseBlobName);

                    //TODO:Test
                    byte[] bytes = new byte[LeaseBlobText.Length * sizeof(char)];
                   System.Buffer.BlockCopy(LeaseBlobText.ToCharArray(), 0, bytes, 0, bytes.Length);
                   MemoryStream stream = new MemoryStream(bytes);
                   tempBlob.UploadFromStream(stream);

                });

                if (Exists(blockBlob))
                {
                    //Checks if the Lifetime of the current instance is not null & has not expired
                    if (lifeTime != null && DateTime.Compare(lifeTime.Value, DateTime.UtcNow) > 0)
                    {
                        isOwner = true;
                    }
                    else
                    {
                        //Read the Contents of the blob
                        DateTime dateTime;
                        MemoryStream stream = new MemoryStream();
                        blockBlob.DownloadToStream(stream);
                        string downloadedText = new StreamReader(stream).ReadToEnd();
                        if (!string.IsNullOrEmpty(downloadedText))
                        {
                            #region Process DownloadedText

                            string[] seperator = new string[] { Seperator };
                            string[] details = downloadedText.Trim().Split(seperator, StringSplitOptions.None);

                            Logger.Debug(string.Format("Downloaded Text {0} ,{1} from blob ", details[0], details[1]));

                            //if (DateTime.Compare(Convert.ToDateTime(details[1]), DateTime.UtcNow) < 0)

                            if (DateTime.TryParse(details[1], out dateTime))
                            {
                                if (DateTime.Compare(Convert.ToDateTime(details[1]), DateTime.UtcNow) < 0)
                                {
                                    Logger.Debug(string.Format("{0} is earlier than {1} ", details[1], DateTime.UtcNow));
                                }

                                if (!string.IsNullOrEmpty(details[0]) && !string.IsNullOrEmpty(details[1]))
                                {
                                    if (details[0] == RoleEnvironment.CurrentRoleInstance.Id || DateTime.Compare(Convert.ToDateTime(details[1]), DateTime.UtcNow) < 0)
                                    {
                                        isOwner = true;
                                    }
                                }
                            }
                            else
                            {
                                Logger.Debug(string.Format("Unable to Convert {0} to Datetime ", details[1]));
                            }
                            #endregion
                        }
                        else
                            Logger.Debug(string.Format("DownloadedText is null for {0}", blockBlob.Name));
                    }

                    if (isOwner)
                    {
                        Logger.Debug(string.Format("Role Instance Id:{0} ,has become the owner", RoleEnvironment.CurrentRoleInstance.Id));

                        //Update the DateTime to 2 mins
                        lifeTime = DateTime.UtcNow.AddMinutes(2);

                        //Overwrite the contents of blockblob
                        if (lifeTime.HasValue)
                        {
                            //blockBlob.UploadText(RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString());
                            //TODO:Test
                            byte[] bytes = new byte[(RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString()).Length * sizeof(char)];
                            System.Buffer.BlockCopy((RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString()).ToCharArray(), 0, bytes, 0, bytes.Length);
                            MemoryStream stream = new MemoryStream(bytes);
                            tempBlob.UploadFromStream(stream);
                            Logger.Debug(string.Format("Role Instance Id:{0} has sucessfully uploaded blob  with new time {1}", RoleEnvironment.CurrentRoleInstance.Id, lifeTime.Value.ToString()));
                        }
                        else
                            Logger.Debug(string.Format("Lifetime value is null for {0} ", RoleEnvironment.CurrentRoleInstance.Id));

                    }
                    else
                        Logger.Debug(string.Format("Role Instance Id:{0} has gone for a sleep,as it not the owner ", RoleEnvironment.CurrentRoleInstance.Id));
                }
                // Occurs for the first time :Create a new blob ,set the lifetime for which the instance has to acquire a lease on tempblob
                else
                {
                    //Obtain the lease on the "Lease Blob"
                    leaseId = AcquireLeaseOnContainer(tempBlob, storage);
                    if (leaseId != null)
                    {
                        lifeTime = DateTime.UtcNow.AddMinutes(2);
                        if (lifeTime.HasValue)
                        
                        {
                            //blockBlob.UploadText(RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString());
                            //TODO:Test
                            byte[] bytes = new byte[(RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString()).Length * sizeof(char)];
                            System.Buffer.BlockCopy((RoleEnvironment.CurrentRoleInstance.Id + "," + lifeTime.Value.ToString()).ToCharArray(), 0, bytes, 0, bytes.Length);
                            MemoryStream stream = new MemoryStream(bytes);
                            tempBlob.UploadFromStream(stream);
                        }

                        else
                        {
                            Logger.Debug(string.Format("Lifetime value is null for {0} ", RoleEnvironment.CurrentRoleInstance.Id));
                        }
                        //Release the lease on "Lease Blob"
                        ReleaseLeaseOnContainer(tempBlob, storage, leaseId);
                        isOwner = true;

                        Logger.Debug(string.Format("Role Instance Id:{0}has created the blob for the first time & acquired lease forTime :{1} ", RoleEnvironment.CurrentRoleInstance.Id, lifeTime.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(String.Format("Exception at Azure IsinstanceOwner Message: \n{0}\n\nStackTrace: \n{1}", ex.Message, ex.StackTrace));

            }
            return isOwner;
        }

        /// <summary>
        /// Checks if a particular blob exsists
        /// </summary>
        /// <param name="blob">BlockBlob reference</param>
        /// <returns></returns>
        private static bool Exists(CloudBlockBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Acquires a lease on the blob 
        /// </summary>
        /// <param name="blockBlob">BlockBlob reference</param>
        /// <param name="storage">Storage account of the blob</param>
        /// <returns></returns>
        private static string AcquireLeaseOnContainer(CloudBlockBlob blockBlob, CloudStorageAccount storage)
        {
            string leaseId = null;
            try
            {
                blockBlob.AcquireLease(null /* infinite lease */, leaseId);
                //HttpWebRequest request = BlobRequest.Lease(blockBlob.Uri, 60, LeaseAction.Acquire, null);
                //storage.Credentials.SignRequest(request);

                //using (var response = request.GetResponse())
                //{
                //    HttpWebResponse resp = (HttpWebResponse)response;
                //    leaseId = response.Headers["x-ms-lease-id"];
                //}
            }

            catch (Exception e)
            {
                Logger.Debug(string.Format("Role Instance Id:{0}has failed to acquired lease.Exception :{1}", RoleEnvironment.CurrentRoleInstance.Id, e.Message));

            }

            return leaseId;

        }

        /// <summary>
        /// Releases a lease on the blob
        /// </summary>
        /// <param name="blockBlob">BlockBlob reference</param>
        /// <param name="storage">Storage account of the blob</param>
        /// <param name="leaseId">LeaseId of the Blob</param>
        public static void ReleaseLeaseOnContainer(CloudBlockBlob blockBlob, CloudStorageAccount storage, string leaseId)
        {
            //HttpWebRequest request = BlobRequest.Lease(blockBlob.Uri, 60, LeaseAction.Release, leaseId);
            //storage.Credentials.SignRequest(request);
            //request.GetResponse().Close();
        }       
    }
}
