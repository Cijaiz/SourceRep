using C2C.Core.Logger;
using Microsoft.WindowsAzure.ServiceRuntime;
using Octane.NotificationScheduleWorker.SchedulerManager;
using Octane.NotificationUtility;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Octane.NotificationScheduleWorker.Scheduler
{
    /// <summary>
    ///HourlyTaskSchedule
    /// </summary>
    public class HourlyTaskSchedule : TaskSchedule
    {
        Timer recurrenceTimer;
        public Int16 Recurrence { get; set; }
        public static IList<Timer> createdTimers = new List<Timer>();

        /// <summary>
        /// Schedules the hourly task
        /// </summary>
        public void Schedule()
        {
            if (Recurrence != 0)
            {
                //new timer object
                recurrenceTimer = new Timer();

                recurrenceTimer.Interval = TimeSpan.FromMinutes(Recurrence).TotalMilliseconds;
                recurrenceTimer.Elapsed += new ElapsedEventHandler(recurrenceTimer_Elapsed);

                //add to the timer lists to clear the timer later.
                createdTimers.Add(recurrenceTimer);

                //Start the timer.
                recurrenceTimer.Start();
            }
            else
            {
                //Log..
            }
        }

        /// <summary>
        /// This method handles the timer elapsed event.
        /// </summary>
        /// <param name="sender">source</param>
        /// <param name="e">EventArgs</param>
        void recurrenceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("Hourly Message :" + TaskId + ": " + EventCode + ": " + Recurrence);
            if (AzureHelper.IsInstanceOwner())
            {
                ScheduleManager.concurrentQueue.Enqueue(new HourlyTaskSchedule
                                                                     {
                                                                         TaskId = TaskId,
                                                                         EventCode = EventCode,
                                                                         Description = Description,
                                                                         TaskType = Octane.NotificationUtility.TaskType.Hourly.ToString()
                                                                     });
                Logger.Debug(string.Format("Role Instance {0} has pushed task {1} hourly to queue", RoleEnvironment.CurrentRoleInstance.Id, TaskId));
            }
        }
    }
}
