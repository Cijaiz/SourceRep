namespace C2C.Core.Helper
{
    #region Reference

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.WindowsAzure;
    using C2C.Core.Security;
    using C2C.Core.Constants.Engine;

    #endregion

    /// <summary>
    /// Common helper
    /// </summary>
    public class CommonHelper
    {
        /// <summary>
        /// This function is used to get the configuration value.
        /// </summary>
        /// <example>
        /// string value = CommonHelper.GetConfigSetting("key");
        /// </example>
        /// <remarks>
        ///  It will check for the key in cloud configuration if its not available then it will app/web config file.
        /// </remarks>
        /// <exception cref="">Key is not found in the config file.</exception>
        /// <param name="key">key name</param>
        /// <returns>Config value as string</returns>
        public static string GetConfigSetting(string key)
        {
            return CloudConfigurationManager.GetSetting(key);
        }

        public static Dictionary<string, string> GetHeader(Consumer consumer)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            AuthenticationTokenBuilder authenticationBuilder = new AuthenticationTokenBuilder();

            switch (consumer)
            {
                case Consumer.Hub:
                    authenticationBuilder.ACSRealmSettingKey = Key.HUB_REALM;
                    break;
                case Consumer.MailingService:
                    authenticationBuilder.ACSRealmSettingKey = Key.MAILINGSERVICE_REALM;
                    break;
                case Consumer.Orchard:
                    authenticationBuilder.ACSRealmSettingKey = Key.ORCHARD_REALM;
                    break;
                default:
                    break;
            }

            authenticationBuilder.ACSNameSpaceSettingKey = Key.ACS_NAMESPACE;
            authenticationBuilder.ACSUserIdSettingKey = Key.SERVICEIDENTITY_USERNAME;
            authenticationBuilder.ACSPasswordSettingKey = Key.SERVICEIDENTITY_PASSWORD;
            string token = string.Format("WRAP access_token=\"{0}\"", authenticationBuilder.BuildACSToken());
            header.Add(DefaultValue.TOKEN_HEADER, token);

            return header;
        }
    }
}
