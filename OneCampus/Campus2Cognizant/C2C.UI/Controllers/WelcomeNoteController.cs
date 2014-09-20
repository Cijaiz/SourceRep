#region References
using System.Web;
using System.Web.Mvc;
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using System;
using C2C.UI.Filters;
using C2C.UI.ViewModels;
using System.Collections.Generic;
using C2C.Core.Helper;
#endregion

namespace C2C.UI.Controllers
{
    [Authorize]
    public class WelcomeNoteController : BaseController
    {
        /// <summary>
        /// Lists all the WelcomeNotes along with pagination.
        /// </summary>
        /// <returns>WelcomeNote list page</returns>
        public ActionResult Index()
        {
            WelcomeNoteListViewModel noteList = new WelcomeNoteListViewModel();
            Pager pager = new Pager();

            List<WelcomeNote> notes = WelcomeNoteManager.Get(pager);
            noteList.WelcomeNotes = notes;
            noteList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View(noteList);
        }

        public ActionResult Tile()
        {
            return PartialView();
        }

        /// <summary>
        /// Create WelcomeNote
        /// </summary>
        /// <returns>Page to key in WelcomeNote details</returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Edit WelcomeNote for the id.
        /// </summary>
        /// <param name="id">WelcomeNoteId</param>
        /// <returns>Page to display WelcomeNote details of teh id.</returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            WelcomeNote welcomeNote = null;
            if (ModelState.IsValid)
            {
                welcomeNote = WelcomeNoteManager.Get(id);
            }

            return View(welcomeNote);
        }

        /// <summary>
        /// Changes the status of WelcomeNote to deleted
        /// </summary>
        /// <param name="id">WelcomeNoteId</param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        public ActionResult Delete(int id)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            ProcessResponse response = WelcomeNoteManager.Delete(id);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Display the fetails of a particulaer welcomenote
        /// </summary>
        /// <param name="id">WelcomeNote Id</param>
        /// <returns>Welcome note details page</returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        public ActionResult Details(int id)
        {
            WelcomeNote welcomeNote = null;
            if (ModelState.IsValid)
            {
                welcomeNote = WelcomeNoteManager.Get(id);
            }

            return View(welcomeNote);
        }

        /// <summary>
        /// HttpPost to save the modified details of WlecomeNote
        /// </summary>
        /// <param name="welcomeNote">WelcomeNote entity</param>
        /// <returns>Page to key in modifications.</returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(WelcomeNote welcomeNote)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (ModelState.IsValid)
            {
                welcomeNote.Status = Status.Active;
                var response = WelcomeNoteManager.Update(welcomeNote);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
                TempData["Response"] = responseMessage;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Post action to save WelcomeNote details
        /// </summary>
        /// <param name="welcomeNote">Entity to be saved in DB.</param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageWelcomeNote)]
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(WelcomeNote welcomeNote)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (ModelState.IsValid)
            {
                welcomeNote.Status = Status.Active;
                var response = WelcomeNoteManager.Create(welcomeNote);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
                TempData["Response"] = responseMessage;

                return RedirectToAction("Index");
            }

            return View();
        }

        /// <summary>
        /// WelcomeNote for the  pop up
        /// </summary>
        /// <param name="sourceID"></param>
        /// <returns>String to be displayed in popup</returns>
        public string GetWelcomeNoteToDisplay(int sourceID)
        {
            //TODO: Add admin check
            string welcomeNote = string.Empty;
            if (sourceID == 1 && User.LastLogon == null)
            {
                HttpCookie userInfo = HttpContext.Request.Cookies["UserInfo"];
                if (userInfo == null)
                {
                    welcomeNote = WelcomeNoteManager.GetWelcomeNoteToDisplay(sourceID);
                    HttpUtilityHelper.SetCookie("UserInfo", "UserID", User.UserId.ToString(),60);
                }
            }
            else if (sourceID == 2)
            {
                welcomeNote = WelcomeNoteManager.GetWelcomeNoteToDisplay(sourceID);
            }
            return welcomeNote;
        }
    }
}
