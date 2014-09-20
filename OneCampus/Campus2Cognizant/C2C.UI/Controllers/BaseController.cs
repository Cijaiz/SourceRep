using C2C.Core.Security.Structure;
using C2C.UI.Handler;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    [CustomHandleError]
    public class BaseController : Controller
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
    }
}
