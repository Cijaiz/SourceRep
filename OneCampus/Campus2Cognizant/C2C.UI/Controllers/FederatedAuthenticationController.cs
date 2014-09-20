using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using C2C.Core.Security;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace C2C.UI.Controllers
{
    public class FederatedAuthenticationController : BaseController
    {
        public ActionResult STSLogin(SiteSetting siteSetting)
        {
            Guard.IsNotNull(siteSetting, "Site Settings");

            string ReturnUrl = "";
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "/";
            }
            ReturnUrl = "/FederatedAuthentication/ProcessToken?ReturnUrl=" + ReturnUrl;

            SignInRequestMessage signingRequest = new SignInRequestMessage(new Uri(siteSetting.StsLoginUrl), siteSetting.Realm, siteSetting.ReturnUrlBase + ReturnUrl);
            return new RedirectResult(signingRequest.RequestUrl);
        }

        public ActionResult CTSLogin(SiteSetting siteSetting)
        {
            // Redirect to sign-in URL, append ReturnUrl as well...
            Guard.IsNotNull(siteSetting, "Site Settings");

            return new RedirectResult(siteSetting.CtsLoginUrl);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Authenticate(string ReturnUrl = "")
        {
            string fromAcs = Request.Form.Get(WSFederationConstants.Parameters.Result);
            if (Request.Form.Get(WSFederationConstants.Parameters.Result) != null)
            {
                var settings = SiteSettingManager.Get();

                // Parse sign-in response
                SignInResponseMessage message =
                    WSFederationMessage.CreateFromFormPost(System.Web.HttpContext.Current.Request) as
                    SignInResponseMessage;

                XmlTextReader xmlReader = new XmlTextReader(
                    new StringReader(message.Result));
                XDocument xDoc = XDocument.Load(xmlReader);
                XNamespace xNs = "http://schemas.xmlsoap.org/ws/2005/02/trust";
                var rst = xDoc.Descendants(xNs + "RequestedSecurityToken").FirstOrDefault();
                if (rst == null)
                {
                    throw new ApplicationException("No RequestedSecurityToken element was found in the returned XML token. Ensure an unencrypted SAML 2.0 token is issued.");
                }
                var rstDesc = rst.Descendants().FirstOrDefault();
                if (rstDesc == null)
                {
                    throw new ApplicationException("No valid RequestedSecurityToken element was found in the returned XML token. Ensure an unencrypted SAML 2.0 token is issued.");
                }

                var config = new SecurityTokenHandlerConfiguration();
                config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(settings.AudienceUrl));
                config.CertificateValidator = X509CertificateValidator.None;
                config.IssuerNameRegistry = new AccessControlServiceIssuerRegistry(
                    settings.StsIssuerUrl, settings.X509CertificateThumbprint);

                config.DetectReplayedTokens = true;
                config.TokenReplayCacheExpirationPeriod = TimeSpan.FromMilliseconds(500);
                //config.MaxClockSkew = TimeSpan.FromSeconds(30);
                //config.SaveBootstrapTokens = true;

                var securityTokenHandlers = new SecurityTokenHandlerCollection(config);
                securityTokenHandlers.Add(new Saml11SecurityTokenHandler());
                securityTokenHandlers.Add(new Saml2SecurityTokenHandler());
                securityTokenHandlers.Add(new EncryptedSecurityTokenHandler());

                var token = securityTokenHandlers.ReadToken(rstDesc.CreateReader());

                ClaimsIdentityCollection claims = securityTokenHandlers.ValidateToken(token);
                IPrincipal principal = new ClaimsPrincipal(claims);

                // Map claims to local users
                string roleClaimValue = "";
                string usernameClaimValue = "";
                string emailClaimValue = "";
                foreach (var claimsIdentity in claims)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && settings.TranslateClaimsToRoles)
                        {
                            roleClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && settings.TranslateClaimsToUserProperties)
                        {
                            emailClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                        {
                            usernameClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && string.IsNullOrEmpty(usernameClaimValue))
                        {
                            usernameClaimValue = claim.Value;
                        }
                    }
                }
                if (string.IsNullOrEmpty(usernameClaimValue))
                {
                    throw new SecurityException("Could not determine username from input claims. Ensure a \"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\" or \"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\" claim is issued by the STS.");
                }

                //Search and find the user..
                var userContextResponse = MembershipManager.GetUserContext(usernameClaimValue);

                #region Create New User
                //Create a new User..
                if (userContextResponse == null)
                {
                    ProcessResponse<User> userResponse = null;
                    C2C.BusinessEntities.C2CEntities.User newUser = new User()
                    {
                        UserName = usernameClaimValue,
                        Password = DefaultValue.DEFAULT_PASSWORD,
                        UpdatedOn = DateTime.UtcNow,
                    };

                    userResponse = UserManager.Create(newUser);

                    if (userResponse.Status == ResponseStatus.Success)
                    {
                        userContextResponse = MembershipManager.BuildUserContext(userResponse.Object, userResponse.Object.Profile);
                    }
                    else
                    {
                        ModelState.AddModelError("", userResponse.Message);
                        return RedirectToAction("Index", "Error");
                    }
                }
                #endregion

                //Existing User.
                if (userContextResponse.Status == BusinessEntities.ResponseStatus.Success)
                {
                    Guard.IsNotNull(userContextResponse.Object, "UserContext");
                    FormsAuthenticationProvider.SetAuthCookie(userContextResponse.Object.UserName, userContextResponse.Object, false);

                    #region Notifing UserLog Information
                    // TODO : Need to chk the performance if need implement TPL
                    // Fetch IP address.
                    string strHostName = Request.UserHostAddress.ToString();
                    Task.Factory.StartNew(() =>
                    {
                        var iPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString();

                        var browser = Request.Browser;
                        UserLog log = new UserLog()
                        {
                            Browser = browser.Browser,
                            BrowserVersion = browser.Version,
                            IsMobileDevice = browser.IsMobileDevice,
                            LoggedOn = DateTime.UtcNow,
                            LastActivityOn = DateTime.UtcNow,
                            IPAddress = iPAddress,
                            UserId = userContextResponse.Object.UserId
                        };

                        C2C.UI.Publisher.NotifyPublisher.NotifyUserLog(log);
                    }, TaskCreationOptions.LongRunning);
                    #endregion

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", userContextResponse.Message);
                }
            }
            return RedirectToAction("Index", "Error"); ;
        }
    }

    class AccessControlServiceIssuerRegistry : IssuerNameRegistry
    {
        private string stsIssuerUrl = "";
        private string x509CertificateThumbprint = "";

        public AccessControlServiceIssuerRegistry(string stsIssuerUrl, string x509CertificateThumbprint)
        {
            this.stsIssuerUrl = stsIssuerUrl;
            this.x509CertificateThumbprint = x509CertificateThumbprint;
        }

        public override string GetIssuerName(SecurityToken securityToken)
        {
            X509SecurityToken token = securityToken as X509SecurityToken;
            if (token == null)
            {
                throw new SecurityTokenException("Token is not a X509 Security Token.");
            }

            var cert = token.Certificate;

            //Commented due to certificate comparision failed....
            //if (cert.Thumbprint.Equals(this.x509CertificateThumbprint, StringComparison.OrdinalIgnoreCase))
            //{
            //    return this.stsIssuerUrl;
            //}

            if (cert.Thumbprint.CompareTo(this.x509CertificateThumbprint) == 0)
            {
                return this.stsIssuerUrl;
            }

            throw new SecurityTokenException("Token not issued by access control service. Ensure thumbprint and STS issuer URK are configured correctly.");
        }
    }
}
