using System.Collections.Generic;
namespace C2C.Core.Constants.C2CWeb
{
    public class DefaultValue
    {
        public const int MAX_LOGIN_RETRY_ATTEMPT = 5;
        public const int PAGE_SIZE = 10;
        public const string DEFAULT_PASSWORD = "password-1";

        public const string PROFILE_DEFAULT_IMAGE_URL = "/Content/Themes/base/images/defaultuser.gif";
        public const int DEFAULT_USER_ID = 1;
        public const string GENERAL_ROLE = "General user";


        public List<int> ROLE_IDS_FOR_EMAIL = new List<int>() { 1, 2 };
      
        public const string YOU_TUBE = "https://www.youtube.com/user/cognizant";
        public const string FACE_BOOK = "https://facebook.com/Cognizant";
        public const string TWITTER = "https://twitter.com/Cognizant";
        public const string LINKED_IN = "https://www.linkedin.com/company/1680";

        public const string NEW_POLL_CREATED = "New poll has been created";

        public const int SITESETTINGS_EXPIRYMINUTES = 15;

        public const string CDSSO_FRAMEWORK_LOGOFF_URL="https://cassso.cognizant.com/OneCampus/logout.aspx";
    }
}
