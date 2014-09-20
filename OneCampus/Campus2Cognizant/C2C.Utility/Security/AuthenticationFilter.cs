using C2C.Core.Security.Constants;
using C2C.Core.Security.Structure;
using System.Web.Mvc;

namespace C2C.Core.Security
{
    public class AuthenticationFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrEmpty(Authenticator.ACSNameSpaceSettingKey)) Authenticator.ACSNameSpaceSettingKey = Key.ACS_NAMESPACE;
            if (string.IsNullOrEmpty(Authenticator.ACSRealmSettingKey)) Authenticator.ACSRealmSettingKey = Key.ACS_REALM;
            if (string.IsNullOrEmpty(Authenticator.ACSTokenKeySettingKey)) Authenticator.ACSTokenKeySettingKey = Key.ACS_TOKEN_KEY;

            Authenticator.ValidateSWTTokenRequest();
        }
    }
}
