using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Net;

namespace C2C.Core.Helper.AzureHelper
{
    public sealed class Queue
    {
        private CloudQueueClient cloudQueueClient = null;
        private CloudStorageAccount cloudStorageAccount = null;

        private Queue(string storageConnectionString)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            IRetryPolicy exponentialRetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);
            cloudQueueClient.RetryPolicy = exponentialRetryPolicy;
        }

        public static Queue GetInstance(string storageConnectionString)
        {
            return new Queue(storageConnectionString);
        }

        public bool Create(string queueName)
        {
            try
            {
                CloudQueue queue = cloudQueueClient.GetQueueReference(queueName);
                queue.CreateIfNotExists();
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

        public void Push(string queueName, CloudQueueMessage message)
        {
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            if (cloudQueue.Exists())
            {
                try
                {
                    CloudQueue queue = cloudQueueClient.GetQueueReference(queueName);
                    queue.AddMessage(message);
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Push(string queueName, bool createQueueIfNotExists, CloudQueueMessage message)
        {
            CloudQueue queue = cloudQueueClient.GetQueueReference(queueName);
            if (!queue.Exists())
            {
                queue.CreateIfNotExists();
            }

            queue.AddMessage(message);

        }
    }
}
