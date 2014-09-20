using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Helper;
using C2C.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using C2C.Core.Extensions;
using C2C.Core.Constants.C2CWeb;

namespace C2C.UI.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Title = "Welcome to Campus To Cognizant";
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult STSLogin()
        {
            //Retrive the site setting to decide whether the Federation is enabled in site..
            SiteSetting siteSetting = SiteSettingManager.Get();
            if (siteSetting != null && siteSetting.IsFederationEnabled)
            {
                //Redirect only when federation is enabled..
                return RedirectToAction("STSLogin", "FederatedAuthentication", siteSetting);
            }

            ViewBag.ReturnUrl = string.Empty;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // If Authenticated user then redirect to default page
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Retrive the site setting to decide whether the Federation is enabled in site..
                SiteSetting siteSetting = SiteSettingManager.Get();
                if (siteSetting != null && siteSetting.IsFederationEnabled)
                {
                    //Redirect only when federation is enabled..
                    return RedirectToAction("CTSLogin", "FederatedAuthentication", siteSetting);
                }

                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var response = MembershipManager.IsAuthenticated(model);
                if (response.Status == ResponseStatus.Success)
                {
                    #region Notifing UserLog Information

                    // Fetch IP address.
                    string strHostName = Request.UserHostAddress;

                    Task.Factory.StartNew(() =>
                    {
                        // Fetch IP address.
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
                            UserId = response.Object.UserId
                        };

                        C2C.UI.Publisher.NotifyPublisher.NotifyUserLog(log);
                    }, TaskCreationOptions.LongRunning);
                    #endregion

                    FormsAuthenticationProvider.SetAuthCookie(model.UserName, response.Object, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", response.Message);
                }
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthenticationProvider.SignOut();
            WebClient wb = new WebClient();
            wb.SafeWebClientProcessing(DefaultValue.CDSSO_FRAMEWORK_LOGOFF_URL);
            return RedirectToAction("Index", "Account");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                ProcessResponse changePasswordSucceeded = null;
                try
                {
                    changePasswordSucceeded = MembershipManager.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded.Status = ResponseStatus.Failed;
                }

                if (changePasswordSucceeded.Status == ResponseStatus.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public void CacheSiteSettings()
        {
            SiteSetting siteSetting = SiteSettingManager.Get();
        }
    }
}
