using C2C.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NotificationHub
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
               name: "DefaultApiWithAction",
               routeTemplate: "api/{controller}/{action}");

            config.MessageHandlers.Add(new AuthenticationMessageHandler());
        }
    }
}
