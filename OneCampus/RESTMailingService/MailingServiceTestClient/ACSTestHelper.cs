using C2C.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailingServiceTestClient
{
    public static class ACSTestHelper
    {
        private static AuthenticationTokenBuilder TokenBuilder { get; set; }

        internal static string GetACSToken()
        {
            if (TokenBuilder == null)
            {
                TokenBuilder = new AuthenticationTokenBuilder();
                TokenBuilder.ACSNameSpaceSettingKey = "AcsNamespace";
                TokenBuilder.ACSPasswordSettingKey = "AcsPassword";
                TokenBuilder.ACSRealmSettingKey = "AcsRealm";
                TokenBuilder.ACSUserIdSettingKey = "AcsUserName";
            }

            return TokenBuilder.BuildACSToken();
        }
    }
}
