using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using WebApiAuthorization.App_Start;

namespace WebApiAuthorization
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            config.Routes.MapHttpRoute(
               name: "DefaultApiWithAction",
               routeTemplate: "api/{controller}/{action}");

            //config.MessageHandlers.Add(new AuthenticationMessageHandler());
        }
    }
}
