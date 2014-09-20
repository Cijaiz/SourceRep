
namespace Octane.NotificationScheduleWorker.Scheduler
{
    /// <summary>
    ///Properties for Scheduled Tasks
    /// </summary>
    public abstract class TaskSchedule
    {
        public string TaskId { get; set; }
        public string AtMin { get; set; }
        public string AtHour { get; set; }
        public string EventCode { get; set; }
        public string Description { get; set; }
        public string TaskType { get; set; }
    }
}
