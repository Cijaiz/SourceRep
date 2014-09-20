using C2C.Core.Logger;
using C2C.Core.Security;
using C2C.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using C2C.Core.Extensions;

namespace C2C.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            FormsAuthenticationProvider.SetCurrentPrinciple();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = string.Empty;
            var currentAction = string.Empty;

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;
                    case 401:
                        action = "UnAuthorizedAccess";
                        break;
                    // others if any
                    default:
                        action = "Index";
                        break;
                }
            }

            // log the error.
            Logger.Error(ex.ToFormatedString());

            httpContext.ClearError();

            // If not an AJAX request redirect to error page.
            bool isAjaxCall = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
            if (isAjaxCall)
            {
                if (ex == new UnauthorizedAccessException())
                {
                    httpContext.Response.StatusCode = 401;
                }
                else
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
                }
            }
            else
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = action;
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            // If we're an ajax request, and doing a 302, then we actually need to do a 401		
            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}