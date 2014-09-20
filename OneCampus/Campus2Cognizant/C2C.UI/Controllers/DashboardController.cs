using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Extensions;
using C2C.Core.Helper;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    public class DashboardController : BaseController
    {
        private string _notificationUrl = string.Format("{0}/{1}", CommonHelper.GetConfigSetting(C2C.Core.Constants.C2CWeb.Key.HUB_URL), "ActivityLog");
        
        //
        // GET: /Dashboard/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ActiveUserStat()
        {
            OnlineUserStat stat = new OnlineUserStat();

            WebClient webClient = new WebClient();
            var text = webClient.SafeWebClientProcessing(string.Format("{0}/{1}", _notificationUrl, "ActiveUserStat"), CommonHelper.GetHeader(Consumer.Hub));
            var data = SerializationHelper.JsonDeserialize<ProcessResponse<OnlineUserStat>>(text);
            stat = data.Object;
            return PartialView("_ActiveUserStat", stat);
        }

        [HttpGet]
        public ActionResult UserBrowserStat()
        {
            List<BrowserStat> stat = new List<BrowserStat>();

            WebClient webClient = new WebClient();
            var text = webClient.SafeWebClientProcessing(string.Format("{0}/{1}", _notificationUrl, "UserBrowserStat"), CommonHelper.GetHeader(Consumer.Hub));
            var data = SerializationHelper.JsonDeserialize<ProcessResponse<List<BrowserStat>>>(text);
            stat = data.Object;
            return PartialView("_UserBrowserStat", stat);
        }
    }
}
