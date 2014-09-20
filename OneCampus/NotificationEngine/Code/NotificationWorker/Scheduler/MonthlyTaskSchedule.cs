
namespace Octane.NotificationScheduleWorker.Scheduler
{
    /// <summary>
    /// Properties for Monthly Tasks
    /// </summary>
    public class MonthlyTaskSchedule : TaskSchedule
    {
        public int DayOfMonth { get; set; }
    }
}
