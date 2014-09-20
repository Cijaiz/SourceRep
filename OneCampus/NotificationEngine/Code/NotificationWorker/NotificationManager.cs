using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.Core.Logger;
using Microsoft.WindowsAzure;
using Octane.NotificationEngineInterfaces;
using Octane.NotificationScheduleWorker.SchedulerManager;
using Octane.NotificationUtility;
using Octane.NotificationWorker.Engine;
using System.Threading.Tasks;

namespace Octane.NotificationWorker
{
    /// <summary>
    /// This Class is the Main Manager for the Notification engine which will be Invoked from the Worker Entry point manages the Execution of the Notifcication Engine.
    /// </summary>
    public class NotificationManager
    {
        private static byte TasksThresholdLimit = 1; //Default is only one task being allocated.
        private static byte MessagesForTaskAllocation = 10; //Default is only 10 messages being processed.

        private static string HighPriorityQueueName = DefaultValue.HIGH_PRIORITY_QUEUE_NAME;
        private static string LowPriorityQueueName = DefaultValue.LOW_PRIORITY_QUEUE_NAME;

        /// <summary>
        /// Constructor for the Notification Manger.
        /// </summary>
        public NotificationManager() { }

        /// <summary>
        /// Incase we would want to access the instance methods this method is used.
        /// </summary>
        /// <returns>new Instance of Notification Manager</returns>
        public static NotificationManager ManagerInstance()
        {
            return new NotificationManager();
        }

        /// <summary>
        /// Notification engine's storage connection string.
        /// </summary>
        public static string StorageConnectionString { get; private set; }

        /// <summary>
        /// Starts the warmup tasks for the Notification Engine.
        /// </summary>
        public static void Start(string azureConnectionString)
        {
            Guard.IsNotBlank(azureConnectionString, "azureConnectionString");

            //Set the Azure Connection String to the Helpers.
            AzureHelper.AzureConnectionString = azureConnectionString;
            StorageConnectionString = azureConnectionString;

            //Create Default Queues..
            AzureHelper.CreateQueueIfNotExists(HighPriorityQueueName);
            AzureHelper.CreateQueueIfNotExists(LowPriorityQueueName);

            //Set the Thresholdlimit from Role Configurtion.
            byte defaultTaskThresholdLimit = TasksThresholdLimit;
            byte defaultMessagesForTaskAllocation = MessagesForTaskAllocation;

            TasksThresholdLimit = byte.TryParse(CloudConfigurationManager.GetSetting(DefaultValue.TASKS_THRESHOLD_LIMIT), out defaultTaskThresholdLimit) ? defaultTaskThresholdLimit : TasksThresholdLimit;
            MessagesForTaskAllocation = byte.TryParse(CloudConfigurationManager.GetSetting(DefaultValue.MESSAGES_FOR_TASK_ALLOCATION), out defaultMessagesForTaskAllocation) ? defaultMessagesForTaskAllocation : MessagesForTaskAllocation;

            INotificationEngine engine = new EmailNotificationEngine();
            engine.StartEngine(CloudConfigurationManager.GetSetting("EmailTemplateConfigurationPath"),
                CloudConfigurationManager.GetSetting("DataPublisherUrl"),
                CloudConfigurationManager.GetSetting("NotificationFeederUrl"),
                CloudConfigurationManager.GetSetting("EmailingServiceUrl"),
                CloudConfigurationManager.GetSetting("FromAddressId"));
        }

        /// <summary>
        /// Runs the Notification Engine to Start processing the messages from the queues.
        /// This is the decision maker for messages to be downloaded or processed. Here the Priority 1 queue messages are processed first 
        /// then the low priority messages are processed.
        /// </summary>
        public static void RunEngine()
        {
            //Process messages from high Priority Queue first then go to Low priority ones.
            NotificationManager.ManagerInstance().DynamicTaskCreation();
        }

        /// <summary>
        /// Here new tasks are created for allocation of the processing based on threshold.
        /// </summary>
        /// <param name="numberOfTasksToCreate">total number of tasks to be created.</param>
        private void DynamicTaskCreation()
        {
            //Check the message count from the low priority queue and allocate the tasks based on the queue count..
            int currentLowPriorityMessageCount = AzureHelper.GetQueueMessagesCount(LowPriorityQueueName);
            int currenthighPriorityMessageCount = AzureHelper.GetQueueMessagesCount(HighPriorityQueueName);

            //Tasks are created based on the message count taken dynamically considering one task being created for a set of 10 messages approx. not more 
            // than 4 total tasks currently.
            int numberOfTasksToCreate = 1;
            if (currentLowPriorityMessageCount >= MessagesForTaskAllocation)
                numberOfTasksToCreate = currentLowPriorityMessageCount / MessagesForTaskAllocation;

            Logger.Debug("Tasks Threshold Limit is : " + TasksThresholdLimit.ToString());
            Logger.Debug(string.Format("{0} Tasks Created to process {1} low priority Messages & {2} high priority messages",
                                                                                            numberOfTasksToCreate.ToString(),
                                                                                            currentLowPriorityMessageCount.ToString(),
                                                                                            currenthighPriorityMessageCount.ToString()
                                                                                            ));

            //Create the tasks for faster processing..
            Task[] tasksArray = null;
            tasksArray = new Task[numberOfTasksToCreate <= TasksThresholdLimit ? numberOfTasksToCreate : TasksThresholdLimit];

            for (int i = 0; i < tasksArray.Length; i++)
            {
                tasksArray[i] = Task.Factory.StartNew((obj) =>
                {
                    MessageDispatcher dispatcher = new MessageDispatcher(ScheduleManager.NotificationRelay);

                    //Start Listening the queue..
                    QueueListener listener = (QueueListener)obj;
                    listener.StartListeningLowPriorityQueue(dispatcher.ProcessMessage, dispatcher.ProcessErrorMessage);
                }
                , new QueueListener(HighPriorityQueueName, 1, LowPriorityQueueName, 1)
                , TaskCreationOptions.AttachedToParent & (TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning));
            }

            //Wait For all tasks to complete before proceeding to next....
            Task.WaitAll(tasksArray);
        }
    }
}
