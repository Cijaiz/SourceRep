using C2C.Core.Security.Constants;
using C2C.Core.Security.Structure;
using System;
using System.Web;

namespace C2C.Core.Security
{
    public class AuthenticationModule : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Authenticator.ACSNameSpaceSettingKey)) Authenticator.ACSNameSpaceSettingKey = Key.ACS_NAMESPACE;
            if (string.IsNullOrEmpty(Authenticator.ACSRealmSettingKey)) Authenticator.ACSRealmSettingKey = Key.ACS_REALM;
            if (string.IsNullOrEmpty(Authenticator.ACSTokenKeySettingKey)) Authenticator.ACSTokenKeySettingKey = Key.ACS_TOKEN_KEY;

            Authenticator.ValidateSWTTokenRequest();
        }
    }
}
