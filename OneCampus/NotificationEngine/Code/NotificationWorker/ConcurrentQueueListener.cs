using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Logger;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Octane.NotificationScheduleWorker.Scheduler;
using Octane.NotificationScheduleWorker.SchedulerManager;
using Octane.NotificationUtility;
using Octane.NotificationWorker;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Octane.NotificationScheduleWorker
{
    /// <summary>
    /// Methods for Processing messages in Concurrent Queue.
    /// </summary>
   
    public class ConcurrentQueueListener
    {

        private C2C.Core.Helper.AzureHelper.Table tableHelper = C2C.Core.Helper.AzureHelper.Table.GetInstance(NotificationManager.StorageConnectionString);
        /// <summary>
        /// Method to listen for items in the Concurrent Queue.
        /// </summary>
        public void ListenConcurrentQueue(CancellationToken ct)
        {
            TaskSchedule task;
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                if (ScheduleManager.concurrentQueue.Count.Equals(0))
                {
                    //Sleep for 5 seconds..
                    Thread.Sleep(5000);
                    continue;
                }

                // Returns the object from the queue without removing it.            
                if (ScheduleManager.concurrentQueue.TryPeek(out task))
                {
                    //TableHelper.CreateTask<TaskSchedulerDetails>(new TaskSchedulerDetails()
                    //{
                    //    TaskId = task.TaskId,
                    //    TaskDescription = task.Description,
                    //    TaskStatus = TaskStatus.Started.ToString(),
                    //    ErrorDescription = "",
                    //    PartitionKey = task.TaskType,
                    //    RowKey = DateTime.UtcNow.Ticks.ToString(),
                    //    Timestamp = DateTime.UtcNow
                    //});

                    
                    tableHelper.InsertRow<TaskSchedulerDetails>(new TaskSchedulerDetails()
                     {
                        TaskId = task.TaskId,
                        TaskDescription = task.Description,
                        TaskStatus = TaskStatus.Started.ToString(),
                        ErrorDescription = "",
                        PartitionKey = task.TaskType,
                        RowKey = DateTime.UtcNow.Ticks.ToString(),
                        Timestamp = DateTime.UtcNow
                     }, DefaultValue.TABLE_STORAGE_NAME);



                   
                    if (ProcessMessageInConcurrentQueue(task))
                    {
                        // Remove and returns the object from the queue.   
                        if (ScheduleManager.concurrentQueue.TryDequeue(out task))
                        {
                            Logger.Debug(string.Format("Successfully dequeued message. Event code: {0}, Task Id: {1}, Task Type: {2} ",
                                                                                                                                        task.EventCode,
                                                                                                                                     task.TaskId,
                                                                                                                                        task.TaskType));
                            //TableHelper.CreateTask<TaskSchedulerDetails>(new TaskSchedulerDetails()
                            //{
                                
                            //    TaskId = task.TaskId,
                            //    TaskDescription = task.Description,
                            //    TaskStatus = TaskStatus.Completed.ToString(),
                            //    ErrorDescription = "",
                            //    PartitionKey = task.TaskType,
                            //    RowKey = DateTime.UtcNow.Ticks.ToString(),
                            //    Timestamp = DateTime.UtcNow
                            //});

                            tableHelper.InsertRow<TaskSchedulerDetails>(new TaskSchedulerDetails()
                             {
                                
                                TaskId = task.TaskId,
                                TaskDescription = task.Description,
                                TaskStatus = TaskStatus.Completed.ToString(),
                                ErrorDescription = "",
                                PartitionKey = task.TaskType,
                                RowKey = DateTime.UtcNow.Ticks.ToString(),
                                Timestamp = DateTime.UtcNow
                             }, DefaultValue.TABLE_STORAGE_NAME);

                        }
                        else
                        {
                            Logger.Debug(string.Format("Problem in dequeuing message from concurrent queue. Task id: {0}, Event code: {1}",
                                                                                                                                         task.TaskId,
                                                                                                                                         task.EventCode));
                            //TableHelper.CreateTask<TaskSchedulerDetails>(new TaskSchedulerDetails()
                            //{
                            //    TaskId = task.TaskId,
                            //    TaskDescription = task.Description,
                            //    TaskStatus = TaskStatus.Failure.ToString(),
                            //    ErrorDescription = "Problem in processing messages and inserting to the storage queue",
                            //    PartitionKey = task.TaskType,
                            //    RowKey = DateTime.UtcNow.Ticks.ToString(),
                            //    Timestamp = DateTime.UtcNow
                            //});


                            tableHelper.InsertRow<TaskSchedulerDetails>(new TaskSchedulerDetails()
                            {
                                TaskId = task.TaskId,
                                TaskDescription = task.Description,
                                TaskStatus = TaskStatus.Failure.ToString(),
                                ErrorDescription = "Problem in processing messages and inserting to the storage queue",
                                PartitionKey = task.TaskType,
                                RowKey = DateTime.UtcNow.Ticks.ToString(),
                                Timestamp = DateTime.UtcNow
                            }, DefaultValue.TABLE_STORAGE_NAME);
                        }
                    }
                    else
                    {
                        Logger.Debug("Problem in processing messages and inserting to the storage queue.");
                    }
                }
            }
        }
        
   
        /// <summary>
        /// Processes the message in concurrent queue and pushed into Azure Storage Queue.
        /// </summary>
        /// <param name="eventCode">Event code</param>
        private bool ProcessMessageInConcurrentQueue(TaskSchedule task)
        {
            bool _status = false;
            string taskCompletionDate = Regex.Replace(DateTime.UtcNow.ToString(), @"[\ /?#]", "");
            try
            {
                //Serialise the string to Json object
                string newMessage = JsonHelper.Serialize<PublisherEvents>
                                                    (
                                                        new PublisherEvents
                                                        {
                                                            EventCode = task.EventCode,
                                                            TaskId = task.TaskId,
                                                            EventId = Constants.EVENT_ID
                                                        }
                                                    );

                AzureHelper.UploadMessagesIntoQueue(ScheduleManager.SchedulerQueueName, new CloudQueueMessage(newMessage));
                _status = true;
            }
            catch (Exception generalException)
            {
                _status = false;
                Logger.Error(string.Format("Error in Processing message_Method_Name_ProcessMessageInConcurrentQueue,{0}", generalException));

                //TableHelper.CreateTask<TaskSchedulerDetails>(new TaskSchedulerDetails()
                //{
                //    TaskId = task.TaskId,
                //    TaskDescription = task.Description,
                //    TaskStatus = TaskStatus.Failure.ToString(),
                //    ErrorDescription = "Error in Processing Message In Concurrent Queue",
                //    PartitionKey = task.TaskType,
                //    RowKey = DateTime.UtcNow.Ticks.ToString(),
                //    Timestamp = DateTime.UtcNow
                //});

                tableHelper.InsertRow<TaskSchedulerDetails>(new TaskSchedulerDetails()
                {
                    TaskId = task.TaskId,
                    TaskDescription = task.Description,
                    TaskStatus = TaskStatus.Failure.ToString(),
                    ErrorDescription = "Error in Processing Message In Concurrent Queue",
                    PartitionKey = task.TaskType,
                    RowKey = DateTime.UtcNow.Ticks.ToString(),
                    Timestamp = DateTime.UtcNow
                }, DefaultValue.TABLE_STORAGE_NAME);

            }
            return _status;
        }
    }
}
