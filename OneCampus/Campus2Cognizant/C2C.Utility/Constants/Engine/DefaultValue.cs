namespace C2C.Core.Constants.Engine
{
    public class DefaultValue
    {

        #region Notification Controller's Action Names
        public const string TOADDRESS_ACTION = "DataPublisher/GetToAddress";
        public const string WEEKLY_TASK_SCHEDULER_ACTION = "DataPublisher/GetWeeklyTaskScheduleMetadata";
        public const string ERROR_EMAIL_TOADDRESS_ACTION = "DataPublisher/GetToAddressForErrorEmails";
        #endregion

        #region Hub : NotificationCorner Controller & Action

        public const string NOTIFICATIONFEED_CONTROLLER = "NotificationCorner";
        public const string NOTIFICATIONFEED_INSERTFEED_ACTION = "InsertFeed";
        public const string GETFEEDCOUNT_ACTIONNAME = "GetFeedCount";
        public const string GETFEEDS_ACTIONNAME = "GetFeeds";
        public const string MARKASREAD_ACTIONNAME = "MarkAsRead";

        #endregion

        #region Hub : User Controller and Action
        public const string NOTIFICATIONFEED_USER_CONTROLLER = "User";
        public const string NOTIFICATIONFEED_ADDMEMBERSTOGROUP_ACTION = "AddToGroup";
        public const string NOTIFICATIONFEED_REMOVEMEMBERSFROMGROUP_ACTION = "RemoveFromGroup";
        public const string NOTIFICATIONFEED_USERPROFILESYNC_ACTION = "UpdateProfile";
        #endregion

        #region Default Values
        public const string CHIRE_IMPORT_ACTION = "CHireImport/CandidateImport";
        public const string ONBOARDING_INTEGRATION = "ONBOARDING";
        public const string TOKEN_HEADER = "Authorization";
        #endregion

        #region OnBoarding Integration
        public const string ONBOARDING_INTEGRATION_ACTION = "IntegrateOnBoarding";
        public const string GET_USER_EMAILADDRESS = "GetUserEmailAddress";
        #endregion

        #region Notification Engine Related.
        public const string HIGH_PRIORITY_QUEUE_NAME = "highpriorityqueue";
        public const string LOW_PRIORITY_QUEUE_NAME = "lowpriorityqueue";
        public const string NOTIFICATION_ENGINE_STORAGE = "NotificationEngineStorage";

        public const string TASKS_THRESHOLD_LIMIT = "TasksThresholdLimit";
        public const string MESSAGES_FOR_TASK_ALLOCATION = "MessagesForTaskAllocation";

        public const int MAXIMUM_MESSAGE_RETRY_COUNT = 2;

        public const string CHIRE_LOG_TABLE = "CHireImportLogs";
        public const string TABLE_STORAGE_NAME = "taskschedulerdata";
        public const string DYNAMIC_DLL_HISTORY = "DynamicLoadHistory";

        //Dynamic Load
        public const string NOTIFICATION_RELAY_CONTAINER = "dynamicloadrelay";
        public const string NOTIFICATION_RELAY_BLOB = "NotificationRelay.dll";
        public const string CURRENT_RELAY_FOLDER = "CurrentRelay";
        public const string OLD_RELAY_FOLDER = "OldRelay";
        public const string NEW_RELAY_FOLDER = "NewRelay";
        #endregion
    }
}
