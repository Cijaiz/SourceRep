using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Extensions;
using C2C.Core.Helper;
using C2C.Core.Helper.AzureHelper;
using C2C.Core.Logger;
using DynamicLoadRelay;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Octane.NotificationScheduleWorker.Scheduler;
using Octane.NotificationUtility;
using Octane.NotificationWorker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Octane.NotificationScheduleWorker.SchedulerManager
{
    /// <summary>
    /// Manages the Task Scheduling process.
    /// </summary>
    public static class ScheduleManager
    {
        #region private properties
        private static string CurrentDay { get; set; }
        private static string CurrentMin { get; set; }
        private static string CurrentHour { get; set; }
        private static string CurrentMonth { get; set; }
        private static string CurrentDayOfWeek { get; set; }
        private static XDocument taskConfiguration { get; set; }

        private static string NewRelayFolder = string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.NEW_RELAY_FOLDER);
        private static string OldRelayFolder = string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.OLD_RELAY_FOLDER);
        private static string CurrentRelayFolder = string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.CURRENT_RELAY_FOLDER);
        private static string NewRelayDll = string.Format(@"{0}\{1}", NewRelayFolder, DefaultValue.NOTIFICATION_RELAY_BLOB);
        private static string OldRelayDll = string.Format(@"{0}\{1}", OldRelayFolder, DefaultValue.NOTIFICATION_RELAY_BLOB);
        private static string storageConnectionString = CommonHelper.GetConfigSetting(DefaultValue.NOTIFICATION_ENGINE_STORAGE);
        public static INotificationRelay NotificationRelay { get; private set; }
        #endregion

        #region Public properties
        public static string SchedulerQueueName { get; set; }
        public static string ScheduleTaskXmlUrl { get; set; }
        #endregion

        #region Public variables
        public static ConcurrentQueue<TaskSchedule> concurrentQueue = new ConcurrentQueue<TaskSchedule>();
        #endregion

        #region Private Methods
        /// <summary>
        /// Downloads the XML Configuration from blob and loads it in XDocument Object
        /// </summary>
        private static void LoadTaskConfiguration()
        {
            Stream contents = null;
            try
            {
                contents = AzureHelper.DownloadContentsFromBlob(ScheduleTaskXmlUrl);
                if (contents != null)
                {
                    contents.Position = 0;
                    taskConfiguration = XDocument.Load(contents);
                    WorkerRole.isConfigurationLoaded = true;
                }
                else
                {
                    Logger.Debug("Task Configuration file is empty");
                }
            }
            catch (XmlException xmlException)
            {
                HandleException(xmlException);
            }
            catch (ArgumentException argException)
            {
                HandleException(argException);
            }
            catch (PathTooLongException pathException)
            {
                HandleException(pathException);
            }
            catch (IOException ioException)
            {
                HandleException(ioException);
            }
            catch (NotSupportedException notSupported)
            {
                HandleException(notSupported);
            }
            catch (StorageException storageClientException)
            {
                HandleException(storageClientException);
            }
            catch (Exception generalException)
            {
                HandleException(generalException);
            }
        }

        /// <summary>
        /// This method handles all the exception types and logs it
        /// </summary>
        /// <param name="generalException">Exception </param>
        private static void HandleException(Exception generalException)
        {
            if (generalException is XmlException)
            {
                //There is a load or parse error in the XML
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error(string.Format("XmlException in loading XML. Pursing with old configuration data \n Error Message: {0}", generalException.ToFormatedString()));
                    Logger.Error(string.Format("Old configuration data is : {0}" , taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error(string.Format("XmlException in loading XML_Method Name_LoadTaskConfiguration \n Error Message: {0}", generalException.ToFormatedString()));
                    throw generalException;
                }
            }
            else if (generalException is ArgumentException)
            {
                //FileName is null
                //filename is a zero-length string, contains only white space, or contains one or more invalid characters 
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error(string.Format("ArgumentException in loading XML. Pursing with old configuration data \n Error Message: {0}", generalException.ToFormatedString()));
                    Logger.Error(string.Format("Old configuration data is : {0}" ,taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error(string.Format("ArgumentException in loading XML_Method Name_LoadTaskConfiguration \n Error Message: {0}", generalException.ToFormatedString()));
                    throw generalException;
                }
            }
            else if (generalException is PathTooLongException)
            {
                //The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, 
                //paths must be less than 248 characters, and file names must be less than 260 characters.
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error("PathTooLongException in loading XML. Pursing with old configuration data");
                    Logger.Error(string.Format("Old configuration data is :{0} " , taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error("PathTooLongException in loading XML_Method Name_LoadTaskConfiguration");
                    throw generalException;
                }
            }
            else if (generalException is IOException)
            {
                ////The file specified in filename was not found.
                //An I/O error occurred while opening the file.

                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error("IOException in loading XML. Pursing with old configuration data");
                    Logger.Error(string.Format("Old configuration data is : {0}" , taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error("IOException in loading XML_Method Name_LoadTaskConfiguration");
                    throw generalException;
                }
            }
            else if (generalException is NotSupportedException)
            {
                //filename is in an invalid format.               
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error("NotSupportedException in loading XML. Pursing with old configuration data");
                    Logger.Error(string.Format("Old configuration data is :{0} " , taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error("NotSupportedException in loading XML_Method Name_LoadTaskConfiguration");
                    throw generalException;
                }
            }
            else if (generalException is StorageException)
            {
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error("StorageClientException in loading XML. Pursing with old configuration data");
                    Logger.Error(string.Format("Old configuration data is : {0}" ,taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error("StorageClientException in loading XML_Method Name_LoadTaskConfiguration");
                    throw generalException;
                }
            }
            else
            {
                if (taskConfiguration != null)
                {
                    WorkerRole.isConfigurationLoaded = false;
                    Logger.Error("General Exception in loading XML. Pursing with old configuration data");
                    Logger.Error(string.Format("Old configuration data is :{0} " , taskConfiguration.ToString()));
                }
                else
                {
                    Logger.Error("General Exception in loading XML_Method Name_LoadTaskConfiguration");
                    throw generalException;
                }
            }
        }
        /// <summary>
        /// Create Directories on the Specified Path
        /// </summary>
        /// <param name="paths">Path for the directory to create</param>
        /// <returns>Success/Failure of Creation</returns>
        private static ProcessResponse CreateDirectories(List<string> paths)
        {
            try
            {
                foreach (var path in paths)
                {
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                }
                return new ProcessResponse() { Status = ResponseStatus.Success, Message = "Sucessfully Created Folders" };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                return new ProcessResponse() { Status = ResponseStatus.Failed, Message = string.Format("Folders Creation for Dynamic Load Failed.Exception:{0}", ex.Message) };
            }
        }
        /// <summary>
        /// Downloads Notication Relay Dll from Blob Container
        /// </summary>
        /// <returns></returns>
        private static ProcessResponse DownloadNotificationRelayDll()
        {
            try
            {
                //acesss the blob (container name ,blob name are constants ) & download               
                Guard.IsNotNull(storageConnectionString, "storageConnectionString");

                //Clean the newRelayDLL Folder.
                DirectoryInfo directory = new DirectoryInfo(NewRelayFolder);
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();

                //Place the downloadeddll on the newRelayDLL Folder
                Blob.GetInstance(storageConnectionString).DownloadFileFromBlob(DefaultValue.NOTIFICATION_RELAY_CONTAINER, DefaultValue.NOTIFICATION_RELAY_BLOB, NewRelayDll);

                return new ProcessResponse() { Status = ResponseStatus.Success, Message = "Sucessfully downloaded dll " };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                return new ProcessResponse() { Status = ResponseStatus.Failed, Message = string.Format("Download of Notification Relay DLL Failed.Exception:{0}", ex.Message) };
            }

        }
        /// <summary>
        /// Creates Folders (OldRelay,NewRelay,CurrentRelay) in application current path
        /// </summary>
        /// <returns>Success/Failure of Creation</returns>
        private static ProcessResponse CreateFoldersForDynamicLoad()
        {
            //Base Directory Creation OnStart

            List<string> foldersToCreate = new List<string>();
            foldersToCreate.Add(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.OLD_RELAY_FOLDER));
            foldersToCreate.Add(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.NEW_RELAY_FOLDER));
            foldersToCreate.Add(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DefaultValue.CURRENT_RELAY_FOLDER));
            return CreateDirectories(foldersToCreate);
        }

        private static ProcessResponse ProcessNotifcationRelayDll()
        {
            StringBuilder responseText = new StringBuilder();
            string currentRelayDll = string.Format(@"{0}\{1}", CurrentRelayFolder, DefaultValue.NOTIFICATION_RELAY_BLOB);

            try
            {
                //Delete old contents if any in the older relay folder
                DirectoryInfo directory = new DirectoryInfo(OldRelayFolder);
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();

                //Process only if u have a new dll downloaded
                if (Directory.GetFiles(NewRelayFolder).Length > 0)
                {
                    //Move the exsisting dll from working folder to oldfolderpath
                    if (Directory.GetFiles(CurrentRelayFolder).Length > 0)
                    {
                        FileInfo currentfile = new FileInfo(currentRelayDll);
                        currentfile.MoveTo(OldRelayFolder + "\\" + currentfile.Name);
                        responseText.AppendLine("Moved Exsisting  Dll to OldRelay Folder.");
                    }

                    //Move the downloaded dll to working folder path 
                    FileInfo newFile = new FileInfo(NewRelayDll);
                    newFile.MoveTo(CurrentRelayFolder + "\\" + newFile.Name);
                    responseText.AppendLine("Moved NewRelayDLL to CurrentRelayFolder.");

                    //Load the dynamic dll() 
                    ProcessResponse resDynamicLoad = DynamicLoadDll(currentRelayDll);
                    responseText.AppendLine(resDynamicLoad.Message);

                    //if(onsucess) of Loading Dynamic DLL Sucess
                    if (resDynamicLoad.Status == ResponseStatus.Success)
                    {
                        // delete dll in old folder
                        if (Directory.GetFiles(OldRelayFolder).Length > 0)
                        {
                            File.Delete(OldRelayDll);
                            responseText.AppendLine("Deleted the DLL exsisting in OldRelayFolder.");
                        }
                        responseText.AppendLine("ProcessNotifcationRelayDll Succeeded.");
                    }
                    //Loading Dynamic DLL Failed
                    else
                    {
                        //Move the old dll back to working folder 
                        if (Directory.GetFiles(OldRelayFolder).Length > 0)
                        {
                            responseText.AppendLine("RollBack:Moving the OldDLL back to CurrentRelayFolder.");
                            FileInfo oldRelayfile = new FileInfo(OldRelayDll);
                            oldRelayfile.MoveTo(CurrentRelayFolder + "\\" + oldRelayfile.Name);

                            //Delete the new dll from downloaded folder
                            File.Delete(NewRelayDll);
                            responseText.AppendLine("RollBack:Deleted the NewDll downloaded from NewRelayFolder.");
                        }

                        return new ProcessResponse() { Status = ResponseStatus.Failed, Message = responseText.ToString() };
                    }
                }

                return new ProcessResponse() { Status = ResponseStatus.Success, Message = responseText.ToString() };
            }

            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                return new ProcessResponse() { Status = ResponseStatus.Failed, Message = string.Format("{0}\nProcessNotifcationRelayDll  Failed.Exception:{0}", responseText.ToString(), ex.Message) };
            }
        }

        private static ProcessResponse DynamicLoadDll(string filePath)
        {
            try
            {
               if(Path.GetExtension(filePath).Equals(".dll",StringComparison.InvariantCultureIgnoreCase))
               {
                byte[] buffer;
                //Read the file into byte array
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    
                    buffer = new byte[(int)fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                }

                Assembly assembly = Assembly.Load(buffer);
                var type = assembly.GetType("NotificationRelay.EngineData");

                NotificationRelay = Activator.CreateInstance(type) as INotificationRelay;
                if (NotificationRelay != null)
                {
                    return new ProcessResponse() { Status = ResponseStatus.Success, Message = "Dyamic Loading of DLL Suceeded" };
                }
                else
                {
                    return new ProcessResponse() { Status = ResponseStatus.Failed, Message = "Dynamic Loading of DLL failed.Could Not type cast assembly" };
                }
               }
                else
                {
                    return new ProcessResponse() { Status = ResponseStatus.Failed, Message = "Dynamic Loading of DLL failed.File Extension is not correct" };
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                return new ProcessResponse() { Status = ResponseStatus.Failed, Message = string.Format("Dynamic Loading of DLL failed.Exception:{0}", ex.Message) };
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the ScheduleManager's Configurations.
        /// </summary>
        public static void RefreshScheduleConfiguration()
        {
            LoadTaskConfiguration();
        }


        /// <summary>
        /// Resets private variables based on current date and time.
        /// </summary>
        public static void RefreshVariables()
        {
            CurrentDay = DateTime.UtcNow.Day.ToString();
            CurrentHour = DateTime.UtcNow.Hour.ToString();
            CurrentMin = DateTime.UtcNow.Minute.ToString();
            CurrentMonth = DateTime.UtcNow.ToString("MMMM");
            CurrentDayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ProcessTasks()
        {
            TasksSeperator();
        }

        /// <summary>
        /// Seperating Tasks based on TaskType
        /// </summary>
        public static void TasksSeperator()
        {
            //Create the tasks for faster processing..
            Task[] tasksArray = null;
            tasksArray = new Task[2];

            //Create seperate task for Monthly Schedule
            tasksArray[0] = Task.Factory.StartNew(() =>
            {
                //Start the manager for each task created.
                PickTask(1);
            }
            , TaskCreationOptions.AttachedToParent & (TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning));

            //Create seperate task for Weekly Schedule
            tasksArray[1] = Task.Factory.StartNew(() =>
            {
                //Start the manager for each task created.
                PickTask(2);
            }
            , TaskCreationOptions.AttachedToParent & (TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning));

            //Wait For all tasks to complete before proceeding to next....
            Task.WaitAll(tasksArray);
            tasksArray = null;
        }

        /// <summary>
        /// Picks the respective tasks from XML Document Object based on the input.
        /// </summary>
        /// <param name="scheduleType">Schedule Type. 0 -> Monthly, 1 -> Weekly.</param>
        public static void PickTask(Byte scheduleType)
        {
            try
            {
                switch (scheduleType)
                {
                    //Monthly Schedule
                    case 1:
                        IList<MonthlyTaskSchedule> orderedMonthlyTasks = RetrieveTaskListForCurrentMonth();
                        if (orderedMonthlyTasks != null)
                        {
                            foreach (var task in orderedMonthlyTasks)
                            {
                                if (concurrentQueue.Where(queueItem => queueItem.TaskId == task.TaskId).Count() == 0)
                                {
                                    if (AzureHelper.IsInstanceOwner())
                                    {
                                        concurrentQueue.Enqueue(task);
                                        Logger.Debug(string.Format("Role Instance {0} has pushed task {1} monthly to queue", RoleEnvironment.CurrentRoleInstance.Id, task.TaskId));
                                        Logger.Debug(string.Format("Successfully added Monthly Message to concurrent Q:{0},{1}", task.TaskId ,task.EventCode));
                                    }
                                }
                            }
                        }
                        break;

                    //Weekly Schedule
                    case 2:
                        IList<WeeklyTaskSchedule> orderedWeeklyTasks = RetrieveTaskListForCurrentDay();
                        if (orderedWeeklyTasks != null)
                        {
                            foreach (var task in orderedWeeklyTasks)
                            {
                                if (concurrentQueue.Where(queueItem => queueItem.TaskId == task.TaskId).Count() == 0)
                                {
                                    if (AzureHelper.IsInstanceOwner())
                                    {
                                        concurrentQueue.Enqueue(task);
                                        Logger.Debug(string.Format("Role Instance {0} has pushed task {1} weekly to queue", RoleEnvironment.CurrentRoleInstance.Id, task.TaskId));
                                        Logger.Debug("Successfully added Weekly Message to concurrent Q :" + task.TaskId + ": " + task.EventCode);
                                    }

                                }
                            }
                        }
                        break;
                    //default case
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(String.Format("Exception at Azure PickTask Message: \n{0}\n\nStackTrace: \n{1}", ex.Message, ex.StackTrace));

            }
        }

        /// <summary>
        /// Fetch Tasks for Current day from XML configuration
        /// </summary>
        /// <returns>List of objects</returns>
        private static IList<WeeklyTaskSchedule> RetrieveTaskListForCurrentDay()
        {
            //Gets all tasks for the current day.
            var currentDayTaskList = taskConfiguration.Descendants(CurrentDayOfWeek).Descendants(Constants.TASK);

            //Loops until there is atleast 1 task, for current day.
            if (currentDayTaskList.Count() > 0)
            {
                //Query the currentDayTaskList based on the current Hr/Min and pushes the matched items to the concurrent queue.
                var dayTasks = (from currentTask in currentDayTaskList
                                .Where(task => task.Attribute(Constants.AT_HOUR).Value == CurrentHour
                                  && task.Attribute(Constants.AT_MIN).Value == CurrentMin)

                                select new WeeklyTaskSchedule()
                                {
                                    TaskId = currentTask.Attribute(Constants.TASK_ID).Value,
                                    EventCode = currentTask.Attribute(Constants.EVENT_CODE).Value,
                                    Description = currentTask.Attribute(Constants.DESCRIPTION).Value,
                                    TaskType = TaskType.Weekly.ToString()
                                })
                                                               .ToList();

                var orderedTasks = dayTasks
                                        .OrderBy(currentWeekTask => Convert.ToInt16(currentWeekTask.AtHour)) //Orders the lists based on Hour.
                                        .ThenBy(currentWeekTask => Convert.ToInt16(currentWeekTask.AtMin)) //Orders based on Min value.
                                        .ToList();
                return orderedTasks;
            }
            return null;
        }

        /// <summary>
        /// Fetches tasks that matches for current time
        /// </summary>
        /// <returns></returns>
        private static IList<MonthlyTaskSchedule> RetrieveTaskListForCurrentMonth()
        {
            var currentMonthTaskList = taskConfiguration.Descendants(CurrentMonth).Descendants(Constants.TASK);
            if (currentMonthTaskList.Count() > 0)
            {

                var monthlyTasks = (from currentMonthTask in currentMonthTaskList
                                       .Where(task => task.Attribute(Constants.DAY_OF_MONTH).Value == CurrentDay
                                            && task.Attribute(Constants.AT_HOUR).Value == CurrentHour
                                            && task.Attribute(Constants.AT_MIN).Value == CurrentMin)

                                    select new MonthlyTaskSchedule()
                                    {
                                        TaskId = currentMonthTask.Attribute(Constants.TASK_ID).Value,
                                        EventCode = currentMonthTask.Attribute(Constants.EVENT_CODE).Value,
                                        Description = currentMonthTask.Attribute(Constants.DESCRIPTION).Value,
                                        TaskType = TaskType.Monthly.ToString()
                                    })
                                                                    .ToList();

                var orderedTasks = monthlyTasks
                                          .OrderBy(taskschedule => Convert.ToInt16(taskschedule.AtHour))
                                          .ThenBy(taskschedule => Convert.ToInt16(taskschedule.AtMin))
                                          .ToList();
                return orderedTasks;
            }
            return null;
        }

        /// <summary>
        /// Scheduler to handle hourly task
        /// </summary>
        public static void RefreshHourlySchedule()
        {
            //Reads the hourly tasks from XML configuration file.
            var hourlyTasksList = taskConfiguration.Descendants(Constants.HOURLY_SCHEDULE).Descendants(Constants.TASK);

            //Loops until there is atleast 1 task, for the hourly schedule.
            if (hourlyTasksList.Count() > 0)
            {
                (from hourlyTask in hourlyTasksList
                 where ((hourlyTask.Attribute(Constants.RECURRENCE) != null) || (!string.IsNullOrEmpty(hourlyTask.Attribute(Constants.RECURRENCE).Value)))
                 select new HourlyTaskSchedule()
                 {
                     TaskType = TaskType.Hourly.ToString(),
                     TaskId = hourlyTask.Attribute(Constants.TASK_ID).Value,
                     EventCode = hourlyTask.Attribute(Constants.EVENT_CODE).Value,
                     Description = hourlyTask.Attribute(Constants.DESCRIPTION).Value,
                     Recurrence = Int16.Parse(hourlyTask.Attribute(Constants.RECURRENCE).Value)
                 })
                                .ToList()
                                .ForEach(p => p.Schedule()); //Schedule each task
            }
        }

        /// <summary>
        ///Methods for ConcurrentQueueListener
        /// </summary>
        public static void InitializeConcurrentQueueListener(CancellationTokenSource tokenSource)
        {
            CancellationToken token = tokenSource.Token;

            //Create the tasks for faster processing..
            Task[] tasksArray = null;
            tasksArray = new Task[1];

            for (byte taskCount = 0; taskCount < tasksArray.Length; taskCount++)
            {
                tasksArray[taskCount] = Task.Factory.StartNew((obj) =>
                {
                    CancellationToken ct = token;

                    ConcurrentQueueListener listener = (ConcurrentQueueListener)obj;

                    // Were we already canceled?
                    ct.ThrowIfCancellationRequested();

                    //Start the Queue Listener 
                    listener.ListenConcurrentQueue(ct);
                }
                , new ConcurrentQueueListener()
                , token
                , TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
                , TaskScheduler.Current);
            }

            //No Wait Required let the Main thread continue work..
        }
        /// <summary>
        ///Dynamically  loads the Notification Relay dll
        /// </summary>
        public static void LoadNotificationRelayDll()
        {
            List<ProcessResponse> finalResponse = new List<ProcessResponse>();
            try
            {
                //Creating Folders namely OldRelay,NewRelay,CurrentRelay in Application Current Directory
                ProcessResponse resCreateFolder = CreateFoldersForDynamicLoad();

                //Add the response of Creating folders to final response list
                finalResponse.Add(resCreateFolder);

                //On Sucess of Creating Folders
                if (resCreateFolder.Status == ResponseStatus.Success)
                {
                    //Download the dll from blob and place it in new relay dll
                    ProcessResponse resDownloadNotification = DownloadNotificationRelayDll();

                    //Add the response Downloading Dll to final response list
                    finalResponse.Add(resDownloadNotification);

                    //On Sucess of Downloading Dll
                    if (resDownloadNotification.Status == ResponseStatus.Success)
                    {
                        //Process the Downloaded DLL for dynamic load
                        ProcessResponse resProcessNotification = ProcessNotifcationRelayDll();

                        //Add the response Processing Notification Relay Dll to final response list
                        finalResponse.Add(resProcessNotification);

                        if (resProcessNotification.Status == ResponseStatus.Success)
                        {
                            finalResponse.Add(new ProcessResponse { Status = ResponseStatus.Success, Message = "LoadNotificationRelayDll Succeeded." });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                finalResponse.Add(new ProcessResponse() { Status = ResponseStatus.Failed, Message = string.Format("LoadNotificationRelayDll failed.Exception:{0}", ex.Message) });

            }

            //Log the DynamicLoadHistory Table 
            ProcessResponse logResponse = UpdateDynamicLoadHistory(finalResponse, ResponseStatus.Success);
            //Notify Users via mail 
            NotifyUsers(logResponse);

        }
        /// <summary>
        /// Updates the Dynamic Load History Table on the processing of Notification Relay Dll
        /// </summary>
        /// <param name="processResponseList">List containing the process response</param>
        /// <param name="status">Success/Failure of Dynamic Load</param>
        /// <returns></returns>
        private static ProcessResponse UpdateDynamicLoadHistory(List<ProcessResponse> processResponseList, ResponseStatus status)
        {
            StringBuilder summaryMessage = new StringBuilder();
            try
            {
                summaryMessage.Append("<p>Dynamic DLL Load Details</p>\n");
                summaryMessage.Append("<TABLE>\n");
                foreach (var item in processResponseList)
                {
                    summaryMessage.Append("<TR>\n");
                    summaryMessage.Append("<TD>" + item.Status + "</TD>\n<TD>" + item.Message + "</TD>\n");
                    summaryMessage.Append("</TR>\n");
                }

                summaryMessage.Append("</TABLE>");

                C2C.Core.Helper.AzureHelper.Table tableHelper = C2C.Core.Helper.AzureHelper.Table.GetInstance(storageConnectionString);
                tableHelper.InsertRow<DynamicLoadHistoryEntity>(new DynamicLoadHistoryEntity()
                {
                    PartitionKey = DateTime.UtcNow.ToString("dd.mm.yyyy"),
                    RowKey = DateTime.UtcNow.TimeOfDay.ToString(),
                    Status = status.ToString(),
                    Message = summaryMessage.ToString()

                }, DefaultValue.DYNAMIC_DLL_HISTORY);

                summaryMessage.AppendLine("<p>DynamicLoadHistory Table insert succeeded</p>");
                return new ProcessResponse() { Status = ResponseStatus.Success, Message = summaryMessage.ToString() };

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                summaryMessage.AppendLine("<p>DynamicLoadHistory Table insert failed</p>");
                return new ProcessResponse() { Status = ResponseStatus.Failed, Message = summaryMessage.ToString() };
            }

        }
        /// <summary>
        /// Pushes a Message to the Highpriorty Queue of Notification Engine.
        /// Notifcation triggers the mail ,by reading the Queue
        /// </summary>
        /// <param name="response">Response Status on the Success /Failure of Notification</param>
        private static void NotifyUsers(ProcessResponse response)
        {
            NotificationContent notificationContent = new NotificationContent()
            {
                Description = string.Format("Status: {0} \n Message: {1}", response.Status, response.Message.ToString()),
                Subject = string.Format("Dynamic Dll load. Status:{0}", response.Status)
            };

            PublisherEvents publisherEvents = new PublisherEvents()
            {
                EventCode = EventCodes.DYNAMIC_DLL_LOAD,
                NotificationContent = SerializationHelper.JsonSerialize<NotificationContent>(notificationContent)
            };
            string json = SerializationHelper.JsonSerialize(publisherEvents);
            CloudQueueMessage message = new CloudQueueMessage(json);
            Queue.GetInstance(storageConnectionString).Push(DefaultValue.HIGH_PRIORITY_QUEUE_NAME, message);
        }
        #endregion
    }
}
