using C2C.Core.Security.Constants;
using C2C.Core.Security.Structure;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace C2C.Core.Security
{
    public class CustomServiceAuthorizationManager  : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            bool isAuthorized = false;

            if (string.IsNullOrEmpty(Authenticator.ACSNameSpaceSettingKey)) Authenticator.ACSNameSpaceSettingKey = Key.ACS_NAMESPACE;
            if (string.IsNullOrEmpty(Authenticator.ACSTokenKeySettingKey)) Authenticator.ACSTokenKeySettingKey = Key.ACS_TOKEN_KEY;
            if (string.IsNullOrEmpty(Authenticator.ACSRealmSettingKey)) Authenticator.ACSRealmSettingKey = Key.ACS_REALM;

            try
            {
                string authorizationheader = WebOperationContext.Current.IncomingRequest.Headers[System.Net.HttpRequestHeader.Authorization];
                Authenticator.ValidateSWTTokenRequest(authorizationheader);
                isAuthorized = true;
            }
            catch
            {
                isAuthorized = false;
            }

            return isAuthorized;
        }

        ///Note: Need to add this service behavior in the web.config file 
        ///<serviceBehaviors>
        /// <behavior name="AuthorizationBehavior">
        ///     <serviceAuthorization serviceAuthorizationManagerType="AuthenticationGateway.CustomServiceAuthorizationManager, AuthenticationGateway" />
        /// </behavior>
        /// </serviceBehaviors>
    }
}
