#region References
using System.Web.Mvc;
#endregion

namespace C2C.UI.Controllers
{
    [Authorize]
    public class FutureCallingController : BaseController
    {
       /// <summary>
       /// Directs to Future Is Calling article page.
       /// </summary>
       /// <returns>View for Future is Calling static page</returns>
        public ActionResult Article()
        {
            return View();
        }

    }
}
