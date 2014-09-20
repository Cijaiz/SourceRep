using C2C.Core.Logger;
using C2C.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using C2C.Core.Constants.C2CWeb;
using C2C.UI.ViewModels;
using C2C.Core.Helper;


namespace C2C.UI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.Facebook = DefaultValue.FACE_BOOK;
            homeViewModel.Linkedin = DefaultValue.LINKED_IN;
            homeViewModel.Twitter = DefaultValue.TWITTER;
            homeViewModel.Youtube = DefaultValue.YOU_TUBE;
            return View(homeViewModel);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult BrowserSupport(string returnUrl)
        {
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["BrowserSupport"];

            //Check if cookie exists and if so, add returnUrl in cookies.
            if (cookie == null)
            {
                cookie = new HttpCookie("BrowserSupport");
                cookie["returnUrl"] = returnUrl;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            return View();
        }
    }
}
