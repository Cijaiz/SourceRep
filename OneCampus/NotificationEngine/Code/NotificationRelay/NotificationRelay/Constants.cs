using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotificationRelay
{  
    /// <summary>
    /// Notification Engine Constants
    /// </summary>
    public static class RelayConstants
    {
        #region Universal EventCodes defined
        public const string BLOG_POST = "B01";
        public const string BLOG_POST1 = "R01";
        public const string BLOG_COMMENT = "B02";

        public const string POLL_PUBLISH = "P01";  
        public const string POLL_REMAINDER = "P02";

        public const string COMMENT_APPROVAL = "C01";

        public const string FORGOT_PASSWORD = "F01";

        public const string WELCOME_EMAIL = "W02";
        public const string WEEKLY_EMAIL = "W01";

        public const string ERROR_EMAIL = "E01";

        public const string QUIZ_PUBLISH = "Q01";

        public const string CONTEST_REGISTER = "CR";
        public const string CONTEST_PUBLISHED = "CP";

        public const string GROUP_ADD_USER = "GAU";
        public const string GROUP_REMOVE_USER = "GRU";

        public const string USER_PROFILE_SYNC = "UPS";

        public const string SHARE_CONTENT = "S01";
        #endregion
        
        #region NOTIFICATION CORNER
        public const string NOTIFICATIONFEED = "NFEED";

        public const string NOTIFICATIONFEED_CONTROLLER = "NotificationCorner";
        public const string NOTIFICATIONFEED_INSERTFEED_ACTION = "InsertFeed";
        public const string NOTIFICATIONFEED_USER_CONTROLLER = "User";
        public const string NOTIFICATIONFEED_ADDMEMBERSTOGROUP_ACTION = "AddToGroup";
        public const string NOTIFICATIONFEED_REMOVEMEMBERSFROMGROUP_ACTION = "RemoveFromGroup";
        public const string NOTIFICATIONFEED_USERPROFILESYNC_ACTION = "UpdateProfile";
        public const string GETFEEDCOUNT_ACTIONNAME ="GetFeedCount" ; 
        public const string GETFEEDS_ACTIONNAME ="GetFeeds";
        public const string MARKASREAD_ACTIONNAME  ="MarkAsRead";

        public const string CONTENT_TYPE_SHARE = "share";
        #endregion

        public const string ONBOARDING_INTEGRATION = "ONBOARDING";

        #region Notification Controller's Action Names

        public const string BLOG_POST_ACTION = "GetBlogPostMetaData"; 
        public const string BLOG_POST_COMMENT_ACTION = "GetBlogPostCommentsData";

        public const string POLL_PUBLISH_ACTION = "GetPollPublishData";
        public const string POLL_REMINDER_ACTION = "GetPollRemainderData";

        public const string COMMENT_APPROVAL_ACTION = "GetCommentApprovalMetaData";
        public const string TOADDRESS_ACTION = "GetToAddress";

        public const string WEEKLY_TASK_SCHEDULER_ACTION = "GetWeeklyTaskScheduleMetadata";

        public const string QUIZ_PUBLISH_ACTION = "GetQuizPublishData"; 
        #endregion

        #region Notification Engine Related.
      
        public const string NOTIFICATION_ENGINE_STORAGE = "NotificationEngineStorage";

        public const string TASKS_THRESHOLD_LIMIT = "TasksThresholdLimit";
        public const string MESSAGES_FOR_TASK_ALLOCATION = "MessagesForTaskAllocation";

        public const int MAXIMUM_MESSAGE_RETRY_COUNT = 2;

        public const string ERROR_EMAIL_TOADDRESS_ACTION = "GetToAddressForErrorEmails";
        public const string FORGOT_PASSWORD_ACTION = "GetForgotPasswordData";
        public const string WELCOME_EMAIL_ACTION = "GetWelcomeEmailData";

        public const string WEEKLY_EMAIL_ACTION = "GetWeeklyTaskScheduleMetadata";
        public const string WEEKLY_EMAIL_CONTROLLER = "DataPublisher";

        public const string USER_IMPORT_CONTAINER = "userimport";
        public const string CONTEST_PUBLISH_ACTION = "GetContestData";
        public const string CONTEST_REGISTER_ACTION = "GetContestRegisterData"; 
        #endregion

        #region Import Data from CHire
        public const string IMPORT_CHIREDATA = "CH01";
        public const string IMPORT_USERFROM_BLOB = "FU01";
        public const string CHIRE_IMPORT_ACTION = "Import"; 
        #endregion

        #region OnBoarding Integration
        public const string ONBOARDING_INTEGRATION_CONTROLLER = "OnBoarding";
        public const string ONBOARDING_INTEGRATION_ACTION = "IntegrateOnBoarding";
        public const string GET_USER_EMAILADDRESS = "GetUserEmailAddress";
        #endregion

        #region Mailing Service Configuration Constants

        public const string MAILING_SERVICE_CONFIG_SMTP_HOST = "Smtp.Host";
        public const string MAILING_SERVICE_CONFIG_SMTP_PORT = "Smtp.Port";
        public const string MAILING_SERVICE_CONFIG_SMTP_USERNAME = "Smtp.UserName";
        public const string MAILING_SERVICE_CONFIG_SMTP_PASSWORD = "Smtp.Password";
        public const string MAILING_SERVICE_CONFIG_SMTP_ENABLESSL = "Smtp.EnableSsl";

        #endregion

        #region Acs Authentication  Constants       
        public const string ACS_NAMESPACE = "AcsNamespace";       
        public const string MAILINGSERVICE_REALM = "MailingServiceRelyingParty";
        public const string ORCHARD_REALM = "OrchardRelyingParty";
        public const string HUB_REALM = "HubRelyingParty";
        public const string SERVICEIDENTITY_USERNAME = "ServiceIdentityUserName";
        public const string SERVICEIDENTITY_PASSWORD = "ServiceIdentityPassword";

        #endregion

        public enum Consumer
        {
            Orchard,
            Hub,
            MailingService
        }

    }
}









