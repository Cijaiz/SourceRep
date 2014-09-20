using C2C.Core.Constants.C2CWeb;
using C2C.Core.Security.Structure;
using System;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Filters
{
    public class CheckPermission : AuthorizeAttribute
    {
        private readonly ApplicationPermission _appPermission;

        public CheckPermission(ApplicationPermission appPermission)
        {
            _appPermission = appPermission;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool hasPermission = true;
            CustomPrincipal val = (CustomPrincipal)HttpContext.Current.User;
            if (!val.HasPermission(_appPermission))
            {
                hasPermission = false;
                throw new HttpException(401, "Not Authorized to Perform this Action");
            }
            
            return hasPermission;
        }
    }
}