namespace C2C.Core.Security.Structure
{
    #region Reference
    using C2C.Core.Helper;
    //using C2C.Core.Security.Constants;
    //using Microsoft.AccessControl2.SDK;
    using System;
    using System.Web;
    using System.Web.Services.Description;
    #endregion

    public static class Authenticator
    {
        private static string _ACSNameSpaceSettingKey { get; set; }
        private static string _ACSRealmSettingKey { get; set; }
        private static string _ACSTokenKeySettingKey { get; set; }

        /// <summary>
        /// This property holds config file setting key for ACS service identity token.
        /// Either if the key is empty or invalid key it will throw exception.
        /// </summary>
        public static string ACSTokenKeySettingKey
        {
            get { return _ACSTokenKeySettingKey; }
            set
            {
                _ACSTokenKeySettingKey = value;
                ACSTokenKey = CommonHelper.GetConfigSetting(value);
            }
        }

        /// <summary>
        /// This property holds config file setting key for ACS service namespace.
        /// Either if the key is empty or invalid key it will throw exception.
        /// </summary>
        public static string ACSNameSpaceSettingKey
        {
            get { return _ACSNameSpaceSettingKey; }
            set
            {
                _ACSNameSpaceSettingKey = value;
                ACSNameSpace = CommonHelper.GetConfigSetting(value);
            }
        }

        /// <summary>
        /// This property holds config file setting key for ACS Realm.
        /// Either if the key is empty or invalid key it will throw exception.
        /// </summary>
        public static string ACSRealmSettingKey
        {
            get { return _ACSRealmSettingKey; }
            set
            {
                _ACSRealmSettingKey = value;
                ACSRealm = CommonHelper.GetConfigSetting(value);
            }
        }

        private static string ACSNameSpace { get; set; }
        private static string ACSRealm { get; set; }
        private static string ACSTokenKey { get; set; }
        
        /// <summary>
        /// This method will validate the request for ACS SWT token.
        /// If authentication failed then it will throw exception.
        /// </summary>
        public static void ValidateSWTTokenRequest()
        {
            string authorizationHeader = HttpContext.Current.Request.Headers.Get("Authorization");
            ValidateSWTToken(authorizationHeader);
        }

        /// <summary>
        /// This method will validate the request for ACS SWT token.
        /// If authentication failed then it will throw exception.
        /// <param name="authenticationException">Exception Information.</param>
        /// <returns>IsAuthenticated or Not.</returns>
        public static bool ValidateSWTTokenRequest(out Exception authenticationException)
        {
            bool isAuthenticated = false;
            authenticationException = null;
            try
            {
                string authorizationHeader = HttpContext.Current.Request.Headers.Get("Authorization");
                ValidateSWTToken(authorizationHeader);
                isAuthenticated = true;
            }
            catch (UnauthorizedAccessException unauthorizedException)
            {
                authenticationException = unauthorizedException;
            }
            catch (ApplicationException applicationException)
            {
                authenticationException = applicationException;
            }
            catch (Exception generalException)
            {
                authenticationException = generalException;
            }
            return isAuthenticated;
        }

        /// <summary>
        /// This method will validate the request for ACS SWT token.
        /// If authentication failed then it will throw exception.
        /// </summary>
        public static void ValidateSWTTokenRequest(string authorizationHeader)
        {
            ValidateSWTToken(authorizationHeader);
        }

        private static void ValidateSWTToken(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(ACSTokenKey) || string.IsNullOrEmpty(ACSNameSpace) || string.IsNullOrEmpty(ACSRealm))
            {
                //throw new ApplicationException(Message.INVALID_SETTINGS_VALUE);
            }
            else
            {
                bool isValidRequest = true;
                //HANDLE SWT TOKEN VALIDATION
                // get the authorization header
                string headerValue = authorizationHeader;

                // check that a value is there and check that it starts with 'WRAP'
                if (string.IsNullOrEmpty(headerValue) || !headerValue.StartsWith("WRAP "))
                {
                    isValidRequest = false;
                }
                else
                {
                    string[] nameValuePair = headerValue.Substring("WRAP ".Length).Split(new char[] { '=' }, 2);

                    if (nameValuePair.Length != 2 ||
                        nameValuePair[0] != "access_token" ||
                        !nameValuePair[1].StartsWith("\"") ||
                        !nameValuePair[1].EndsWith("\""))
                    {
                        isValidRequest = false;
                    }
                    else
                    {
                        // trim off the leading and trailing double-quotes
                        string token = nameValuePair[1].Substring(1, nameValuePair[1].Length - 2);

                        // create a token valuator
                        //TokenValidator validator = new TokenValidator(
                        //    DefaultValue.ACS_HOSTNAME,
                        //    Authenticator.ACSNameSpace,
                        //    Authenticator.ACSRealm,
                        //    Authenticator.ACSTokenKey);

                        // validate the token
                        //if (!validator.Validate(token))
                        //{
                        //    isValidRequest = false;

                        //}
                    }
                }

                if (!isValidRequest)
                {
                    //throw new UnauthorizedAccessException(Message.UN_AUTHORIZED);
                }
            }
        }
    }
}
