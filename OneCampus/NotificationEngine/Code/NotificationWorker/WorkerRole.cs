using C2C.Core.Constants.Engine;
using C2C.Core.Logger;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Octane.NotificationScheduleWorker.Scheduler;
using Octane.NotificationScheduleWorker.SchedulerManager;
using Octane.NotificationUtility;
using Octane.NotificationWorker.Engine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Octane.NotificationWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        #region Class Variables

        private CancellationTokenSource concurrentQueueListenerTokenSource = new CancellationTokenSource();

        //this variable is used to track if the new task configuration xml is loaded successfully or not.
        public static bool isConfigurationLoaded = false;

        private static string lowPriorityQueueName = DefaultValue.LOW_PRIORITY_QUEUE_NAME;
        private static string highPriorityQueueName = DefaultValue.HIGH_PRIORITY_QUEUE_NAME;

        #endregion

        /// <summary>
        ///Overide the Run method
        /// </summary>
        public override void Run()
        {
            //Create separate tasks for Engine and Scheduler
            Parallel.Invoke(
                                () => { NotificationEngine(); },
                                () => { Scheduler(); }
                            );
        }

        private void Scheduler()
        {
            // This is a sample worker implementation. Replace with your logic.
            Logger.Debug("Octane.NotificationScheduleWorker entry point called", null);

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            //Infinite loop
            while (true)
            {
                ThreadSleep();

                //Loaddll
                ScheduleManager.LoadNotificationRelayDll();

                #region Performance Statistics
                watch.Start();
                Logger.Debug(string.Format("Thread Waken.at Hour : {0}, Minute : {1}, Second : {2}, Millisecond : {3}..", DateTime.UtcNow.Hour.ToString(),
                                                                                                                        DateTime.UtcNow.Minute.ToString(),
                                                                                                                        DateTime.UtcNow.Second.ToString(),
                                                                                                                        DateTime.UtcNow.Millisecond.ToString()));
                #endregion

                //Reset variables
                ScheduleManager.RefreshVariables();

                //Check for midnight
                if (IsMidnightHourAndMinute())
                {
                    Logger.Debug("Mid Night hour");

                    //Refresh the XML task configuration file
                    ScheduleManager.RefreshScheduleConfiguration();

                    CreatingTimers();

                    //Loaddll
                    ScheduleManager.LoadNotificationRelayDll();
                }

                //Process scheduling Job.
                ScheduleManager.ProcessTasks();

                //Check if there is any problem in loading updated configuration file.
                if (!isConfigurationLoaded)
                    Logger.Error("Error in loading configuration file. Pursing with old data");

                #region Performance Statistics
                watch.Stop();

                Logger.Debug("Total number of Hourly Timers created : " + HourlyTaskSchedule.createdTimers.Count.ToString());
                Logger.Debug("Timer Info : Starting here.... ");

                foreach (var timer in HourlyTaskSchedule.createdTimers)
                    Logger.Debug("Info for Timer Elapsing interval :" + timer.Interval.ToString());

                Logger.Debug("Timer Info : Ending here.... ");

                Logger.Debug("Time taken for scheduling : " + watch.Elapsed.Seconds + " Seconds and " + watch.Elapsed.Milliseconds.ToString() + " Milli seconds");
                watch.Reset();
                #endregion

                //sleep thread by default for 5000 Milli seconds. {This is done in case there is no work to be done.. sleep the thread for timer calculations.}
                Thread.Sleep(5000);
            }
        }

        private static void NotificationEngine()
        {
            while (true)
            {
                Stopwatch watchTimer = new Stopwatch();
                watchTimer.Start();

                string highPriorityQueueMessages = AzureHelper.GetQueueMessagesCount(highPriorityQueueName).ToString();
                string lowPriorityQueueMessages = AzureHelper.GetQueueMessagesCount(lowPriorityQueueName).ToString();

                Logger.Debug(string.Format("\n Total messages available in the High priority queue : {0} \n Total messages available in the low priority queue : {1}",
                                    highPriorityQueueMessages, lowPriorityQueueMessages));

                NotificationManager.RunEngine();

                Logger.Debug(string.Format("Total Time For processing highPriority messages is {0} \n and lowpriority messages in {1} is : {2} seconds",
                                                                                                                            highPriorityQueueMessages,
                                                                                                                            lowPriorityQueueMessages,
                                                                                                                            watchTimer.Elapsed.Seconds.ToString()));
                watchTimer.Stop();
                watchTimer.Reset();

                Logger.Debug("Main thread sleeping for 50000 milliseconds..");
                Thread.Sleep(12000);
            }
        }

        /// <summary>
        ///Method gets executed the first time worker role is executed.
        /// </summary>
        public override bool OnStart()
        {
            RoleEnvironment.Changed += new EventHandler<RoleEnvironmentChangedEventArgs>(RoleEnvironment_Changed);

            Logger.Debug(string.Format("Starting Processor WorkerRole.. {0}", RoleEnvironment.CurrentRoleInstance.Id));

            // Set the maximum number of concurrent connections 
            //Connection optimisation for http and webclient call.
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.CheckCertificateRevocationList = true;
            //ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = 12;

            //Start the Engine and setup all the Warmup tasks..
            NotificationManager.Start(RoleEnvironment.GetConfigurationSettingValue("NotificationEngineStorage"));

            #region Task Scheduler Configuration

            ScheduleManager.ScheduleTaskXmlUrl = RoleEnvironment.GetConfigurationSettingValue("ScheduleTaskXmlUrl");
            ScheduleManager.SchedulerQueueName = DefaultValue.LOW_PRIORITY_QUEUE_NAME;

            AzureHelper.AzureConnectionString = RoleEnvironment.GetConfigurationSettingValue("NotificationEngineStorage");

            //Create required queue in storage account
            AzureHelper.CreateQueueIfNotExists(ScheduleManager.SchedulerQueueName);

            //Create required tables in storage account       
            C2C.Core.Helper.AzureHelper.Table tableHelper = C2C.Core.Helper.AzureHelper.Table.GetInstance(NotificationManager.StorageConnectionString);
            tableHelper.Create(DefaultValue.TABLE_STORAGE_NAME);
            tableHelper.Create(DefaultValue.DYNAMIC_DLL_HISTORY);

            //Get the latest data in task schedule configuration file
            ScheduleManager.RefreshScheduleConfiguration();

            //Start listening the Internal Queue for executing the requests...
            ScheduleManager.InitializeConcurrentQueueListener(concurrentQueueListenerTokenSource);

            #endregion

            #region CHire Configuration
            CHireManager.SftpServer = RoleEnvironment.GetConfigurationSettingValue("SftpServer");
            CHireManager.SftpUserName = RoleEnvironment.GetConfigurationSettingValue("SftpUserName");
            CHireManager.SftpPassword = RoleEnvironment.GetConfigurationSettingValue("SftpPassword");
            EmailNotificationEngine.chireFilePath = CloudConfigurationManager.GetSetting("CHireFilePath");

            //create table  
            tableHelper.Create(DefaultValue.CHIRE_LOG_TABLE);
            #endregion

            Logger.Debug("Role Started at " + DateTime.UtcNow.ToString());

            //Needs to be done whenever the role is restarted.
            CreatingTimers();

            //DYnamic load dll.
            ScheduleManager.LoadNotificationRelayDll();

            return base.OnStart();
        }

        void RoleEnvironment_Changed(object sender, RoleEnvironmentChangedEventArgs e)
        {
            Logger.Debug(string.Format("Re--Starting Processor WorkerRole.. {0} at {1}", RoleEnvironment.CurrentRoleInstance.Id, DateTime.UtcNow.ToShortDateString()));

            //Start the Engine and setup all the Warmup tasks..
            NotificationManager.Start(RoleEnvironment.GetConfigurationSettingValue("NotificationEngineStorage"));
        }

        /// <summary>
        /// Method for Role's OnStop event
        /// </summary>
        public override void OnStop()
        {
            concurrentQueueListenerTokenSource.Cancel();
            base.OnStop();
        }

        #region Private Methods
        /// <summary>
        ///  This method checks if it is midnight 00.00
        /// </summary>
        /// <returns></returns>
        private bool IsMidnightHourAndMinute()
        {
            return (DateTime.UtcNow.Hour.Equals(0) && DateTime.UtcNow.Minute.Equals(0) && DateTime.UtcNow.Second.Equals(0));
        }

        /// <summary>
        /// Determines the Thread's sleep time for next minute
        /// </summary>
        private void ThreadSleep()
        {
            while (true)
            {
                //Check whether the next minute is born.. and break the loop and come out..
                if (DateTime.UtcNow.Second.Equals(0))
                    break;
                else //Sleep for 999 Milli second for next check.
                    Thread.Sleep(999);
            }
        }
        /// <summary>
        /// Creates Timers 
        /// </summary>
        private void CreatingTimers()
        {
            Logger.Debug("Timers are being created For Role Instance  " + RoleEnvironment.CurrentRoleInstance.Id);

            //Terminate previous Timers and release system resources...
            HourlyTaskSchedule.createdTimers.ToList()
                                            .ForEach
                                            (
                                                timerObject =>
                                                {
                                                    timerObject.Stop();
                                                    timerObject.Dispose();
                                                }
                                            );

            //Clear all the items in the timers list.
            HourlyTaskSchedule.createdTimers.Clear();

            //Restart schedulers
            ScheduleManager.RefreshHourlySchedule();
        }

        #endregion
    }
}
