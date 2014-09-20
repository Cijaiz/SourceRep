using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace C2C.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "FederatedDefault",
                url: "FederatedAuthentication/ProcessToken/{ReturnUrl}",
                defaults: new
                {
                    controller = "FederatedAuthentication",
                    action = "Authenticate",
                    ReturnUrl = UrlParameter.Optional
                },
                namespaces: new string[] { "C2C.UI.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "C2C.UI.Controllers" }
            );
        }
    }
}