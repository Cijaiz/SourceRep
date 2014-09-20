using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.UI.Filters;
using C2C.UI.ViewModels;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// This controller handles actions related to the Site settings.
    /// </summary>
    [Authorize]
    public class SiteSettingController : BaseController
    {
        [CheckPermission(ApplicationPermission.ManageSiteSetting)]
        public ActionResult Index()
        {
            ViewData["sitesettingResponse"] = (PageNotificationViewModel)TempData["sitesettingResponse"];

            SiteSetting sitesetting = null;
            sitesetting = SiteSettingManager.Get();

            return View(sitesetting);
        }
       
        /// <summary>
        /// Get Method for Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [CheckPermission(ApplicationPermission.ManageSiteSetting)]
        public ActionResult Edit()
        {
            SiteSetting sitesetting = null;
            sitesetting = SiteSettingManager.Get();
            
            return View(sitesetting);
        }

        //Post method for Edit
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManageSiteSetting)]
        public ActionResult Edit(SiteSetting sitesetting)
        {
            ProcessResponse<SiteSetting> response = null;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (ModelState.IsValid)
            {
                sitesetting.UpdatedBy = User.UserId;
                response = SiteSettingManager.Update(sitesetting);
            }

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["sitesettingResponse"] = responseMessage;

            return RedirectToAction("Index");
        }
    }
}
