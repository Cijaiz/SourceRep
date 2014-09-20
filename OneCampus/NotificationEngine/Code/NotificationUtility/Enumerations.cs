
namespace Octane.NotificationUtility
{
    /// <summary>
    /// Enum for Task type
    /// </summary>
    public enum TaskType
    {
        Weekly = 0,
        Monthly,
        Hourly
    }

    public enum TaskStatus
    {
        Started,
        Failure,
        Completed
    }
}
