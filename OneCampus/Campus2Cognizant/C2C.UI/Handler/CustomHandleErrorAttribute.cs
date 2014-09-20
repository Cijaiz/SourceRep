using C2C.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using C2C.Core.Extensions;

namespace C2C.UI.Handler
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            HttpException httpError = new HttpException(null, filterContext.Exception);
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            bool isAjaxRequest = string.Equals("XMLHttpRequest", filterContext.HttpContext.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
            
            // if the request is AJAX return JSON else view.
            if (!isAjaxRequest && !filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
                if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    return;
                }

                if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
                {
                    return;
                }

                if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
                {
                    return;
                }

                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };

                filterContext.HttpContext.Response.StatusCode = 500;
            }
            else
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };

                if (filterContext.Exception is HttpException)
                {
                    filterContext.HttpContext.Response.StatusCode = httpError.GetHttpCode();
                }
            }

            // log the error.
            Logger.Error(filterContext.Exception.ToFormatedString());

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}