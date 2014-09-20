using C2C.Core.Constants.Engine;
using C2C.Core.Logger;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Octane.NotificationUtility;
using System;
using System.Linq;

namespace Octane.NotificationWorker
{
    /// <summary>
    /// Methods for Downloading and Processing Messages from Queue
    /// </summary>
    public class QueueListener
    {
        private readonly string highPriorityqueueNameToDownload;
        private readonly int highPriorityMessagesToDownload;

        private readonly string lowPriorityqueueNameToDownload;
        private readonly int lowPriorityMessagesToDownload;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="highPriorityQueueName">High Priority Queue name from where the message needs to be downloaded.</param>
        /// <param name="highPrioritymessageCount">High Priority Message count to be downloaded..</param>
        /// <param name="lowPriorityQueueName">Low priority Queue name from where the message needs to be downloaded.</param>
        /// <param name="lowPrioritymessageCount">Low Priority Message count to be downloaded..</param>
        public QueueListener(string highPriorityQueueName, int highPrioritymessageCount, string lowPriorityQueueName, int lowPrioritymessageCount)
        {
            highPriorityqueueNameToDownload = highPriorityQueueName;
            highPriorityMessagesToDownload = highPrioritymessageCount;

            lowPriorityqueueNameToDownload = lowPriorityQueueName;
            lowPriorityMessagesToDownload = lowPrioritymessageCount;
        }

        /// <summary>
        /// Here the messages are downloaded and processed by listening high priority queue..
        /// </summary>
        /// <param name="messageProcessor">Message dispatcher which carries the message for processing.</param>
        private void StartListeningHighPriorityQueue(Func<CloudQueueMessage, bool> messageProcessor, Action<CloudQueueMessage> errorMessageProcessor)
        {
            while (true)
            {
                Logger.Debug("<<<<listening high priority queue>>>>");

                //process message logic goes here...
                CloudQueueMessage downloadedMessage = AzureHelper.DownloadMessagesFromQueue(highPriorityqueueNameToDownload, highPriorityMessagesToDownload).FirstOrDefault();

                //start listening the queue and break when there is no message...
                if (downloadedMessage == null)
                    break;

                //Process the downloaded message..
                if (downloadedMessage.DequeueCount.Equals(1))
                {
                    if (messageProcessor(downloadedMessage))
                        //Safe delete message on completion..
                        AzureHelper.DeleteQueueMessage(highPriorityqueueNameToDownload, downloadedMessage);
                }
                else if (downloadedMessage.DequeueCount >= DefaultValue.MAXIMUM_MESSAGE_RETRY_COUNT)
                {
                    //Handle Error Messages..
                    errorMessageProcessor(downloadedMessage);
                    //Safe delete message on completion..
                    AzureHelper.DeleteQueueMessage(highPriorityqueueNameToDownload, downloadedMessage);
                }
            }
        }

        /// <summary>
        /// Here the messages are downloaded and processed by listening low priority queue..
        /// </summary>
        /// <param name="messageProcessor">Message dispatcher which carries the message for processing.</param>
        public void StartListeningLowPriorityQueue(Func<CloudQueueMessage, bool> messageProcessor, Action<CloudQueueMessage> errorMessageProcessor)
        {
            while (true)
            {
                Logger.Debug("<<<<listening low priority queue>>>>");

                //Process HighPriority Queues for Immediate Deliveries based on the messages count...
                if (AzureHelper.GetQueueMessagesCount(highPriorityqueueNameToDownload) > 0)
                    StartListeningHighPriorityQueue(messageProcessor, errorMessageProcessor);

                //process message logic goes here...
                CloudQueueMessage downloadedMessage = AzureHelper.DownloadMessagesFromQueue(lowPriorityqueueNameToDownload, lowPriorityMessagesToDownload).FirstOrDefault();

                //start listening the queue and break when there is no message...
                if (downloadedMessage == null)
                    break;

                //Process the downloaded message..
                if (downloadedMessage.DequeueCount.Equals(1))
                {
                    if (messageProcessor(downloadedMessage))
                        //Safe delete message on completion..
                        AzureHelper.DeleteQueueMessage(lowPriorityqueueNameToDownload, downloadedMessage);
                }
                else if (downloadedMessage.DequeueCount >= DefaultValue.MAXIMUM_MESSAGE_RETRY_COUNT)
                {
                    errorMessageProcessor(downloadedMessage);
                    //Safe delete message on completion..
                    AzureHelper.DeleteQueueMessage(lowPriorityqueueNameToDownload, downloadedMessage);
                }
            }
        }
    }
}
