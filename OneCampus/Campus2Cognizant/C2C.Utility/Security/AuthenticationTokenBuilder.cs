namespace C2C.Core.Security
{
    #region Reference
    using C2C.Core.Helper;
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    #endregion

    /// <summary>
    /// To Build ACS token
    /// </summary>
    public class AuthenticationTokenBuilder
    {
        public string ACSNameSpaceSettingKey { get; set; }
        public string ACSRealmSettingKey { get; set; }
        public string ACSUserIdSettingKey { get; set; }
        public string ACSPasswordSettingKey { get; set; }

        /// <summary>
        /// Generates ACS token based on the configuration.
        /// </summary>
        /// <returns>ACS token.</returns>
        public string BuildACSToken()
        {
            if (string.IsNullOrEmpty(ACSNameSpaceSettingKey) || string.IsNullOrEmpty(ACSRealmSettingKey) ||
                string.IsNullOrEmpty(ACSUserIdSettingKey) || string.IsNullOrEmpty(ACSPasswordSettingKey))
            {
                throw new ApplicationException(Constants.Message.INVALID_SETTINGS_KEY_VALUE);
            }

            string acsNameSpace = CommonHelper.GetConfigSetting(ACSNameSpaceSettingKey);
            string acsRealm = CommonHelper.GetConfigSetting(ACSRealmSettingKey);
            string acsUserId = CommonHelper.GetConfigSetting(ACSUserIdSettingKey);
            string acsPwd = CommonHelper.GetConfigSetting(ACSPasswordSettingKey);

            return BuildACSToken(acsNameSpace, acsRealm, acsUserId, acsPwd);
        }

        /// <summary>
        /// Generated ACS token based on the value provided.
        /// </summary>
        /// <param name="acsNameSpace">ACS name space</param>
        /// <param name="acsRealm">ACS Realm</param>
        /// <param name="userId">ACS service user id</param>
        /// <param name="password">ACS service password</param>
        /// <returns>ACS token.</returns>
        private string BuildACSToken(string acsNameSpace, string acsRealm, string userId, string password)
        {
            Guard.IsNotBlank(acsNameSpace, "acsNameSpace");
            Guard.IsNotBlank(acsRealm, "acsRealm");
            Guard.IsNotBlank(userId, "userId");
            Guard.IsNotBlank(password, "password");

            // request a token from ACS
            WebClient client = new WebClient();
            client.BaseAddress = string.Format("https://{0}.{1}", acsNameSpace, Constants.DefaultValue.ACS_HOSTNAME);

            NameValueCollection values = new NameValueCollection();
            values.Add("wrap_name", userId);
            values.Add("wrap_password", password);
            values.Add("wrap_scope", acsRealm);

            byte[] responseBytes = client.UploadValues("WRAPv0.9/", "POST", values);

            string response = Encoding.UTF8.GetString(responseBytes);

            return HttpUtility.UrlDecode(
                response
                .Split('&')
                .Single(value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))
                .Split('=')[1]);
        }
    }
}
