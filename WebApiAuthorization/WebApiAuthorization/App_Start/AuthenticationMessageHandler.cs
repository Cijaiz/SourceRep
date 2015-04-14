using C2C.Core.Security.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiAuthorization.App_Start
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        //Note .net 4.0
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if (string.IsNullOrEmpty(Authenticator.ACSNameSpaceSettingKey)) Authenticator.ACSNameSpaceSettingKey = Key.ACS_NAMESPACE;
            //if (string.IsNullOrEmpty(Authenticator.ACSRealmSettingKey)) Authenticator.ACSRealmSettingKey = Key.ACS_REALM;
            //if (string.IsNullOrEmpty(Authenticator.ACSTokenKeySettingKey)) Authenticator.ACSTokenKeySettingKey = Key.ACS_TOKEN_KEY;

            Exception authenticationException = null;
            bool isAuthenticated = Authenticator.ValidateSWTTokenRequest(out authenticationException);
            if (!isAuthenticated)
            {
                HttpResponseMessage responseMessage = null;
                if (authenticationException is UnauthorizedAccessException)
                {
                    responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Access Control Service Authentication Check Failed.."),
                        ReasonPhrase = "Please check the Service Credentials.. Unable to authenticate the SWAT token.."
                    };
                }
                else if (authenticationException is ApplicationException)
                {
                    responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Application Error Occured.. Please check the Reason for more details.."
                                                    + authenticationException.Message + authenticationException.StackTrace),
                        ReasonPhrase = (authenticationException.Message).Replace('/', '#'),
                    };
                }
                else if (authenticationException is Exception)
                {
                    responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("General Application Error Occured.. Please check the Reason for more details.."
                                                    + authenticationException.Message + authenticationException.StackTrace),
                        ReasonPhrase = (authenticationException.Message).Replace('/', '#'),
                    };
                }

                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetResult(responseMessage);
                return task.Task;
            }
            else
                // Call the inner handler.
                return base.SendAsync(request, cancellationToken);
        }

        //Note .net4.5 and above.
        //protected internal async override Task<HttpResponseMessage> SendAsync(
        //    HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    ACSAuthenticationHelper.ACSHostNameSettingKey = Constants.ACS_HOSTNAME;
        //    ACSAuthenticationHelper.ACSNameSpaceSettingKey = Constants.ACS_NAMESPACE;
        //    ACSAuthenticationHelper.ACSTokenKeySettingKey = Constants.ACS_TOKEN_KEY;
        //    ACSAuthenticationHelper.ACSRealmSettingKey = Constants.ACS_REALM;

        //    ACSAuthenticationHelper.ValidateSWTTokenRequest();

        //    // Call the inner handler.
        //    var response = await base.SendAsync(request, cancellationToken);
        //    return response;
        //}
    }
}
