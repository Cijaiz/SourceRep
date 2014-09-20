#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.UI.Filters;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
#endregion

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides CRUD operations for Group Entity Library.
    /// </summary>
    [Authorize]
    public class GroupController : BaseController
    {
        /// <summary>
        /// Displays the index page as list of groups.
        /// </summary>
        /// <returns>An Index page</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Index()
        {
            Pager pager = new Pager();
            GroupListViewModel groupList = new GroupListViewModel();
            List<GroupViewModel> groups = new List<GroupViewModel>();

            int count = GroupManager.GetCount();
            int totalPages = (int)Math.Ceiling((double)count / (double)pager.PageSize);

            groupList.PageCount = totalPages;
            groupList.GroupList = groups;
            groupList.SearchText = string.Empty;
            groupList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View(groupList);
        }      

        /// <summary>
        /// Displays the create page using group model.
        /// </summary>
        /// <returns>Create Page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Displays the group model for the id given.
        /// </summary>
        /// <param name="id">Id of the group.</param>
        /// <returns>Edit Page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Edit(int id)
        {
            var group = GroupManager.Get(id);
            return View(group);
        }

        /// <summary>
        /// Displays the Group details using group model.
        /// </summary>
        /// <param name="id">Id of the group.</param>
        /// <returns>View Page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult View(int id)
        {
            var group = GroupManager.Get(id);
            return View(group);
        }

        /// <summary>
        /// Updates the group model.
        /// </summary>
        /// <param name="group">Group model in which the group to be edited.</param>
        /// <returns>Index page.</returns>
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                PageNotificationViewModel responseMessage = new PageNotificationViewModel();
                group.UpdatedBy = User.UserId;

                // Getting the response as update success or failure.
                var response = GroupManager.Update(group);

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
        /// Creates the group model.
        /// </summary>
        /// <param name="group">Group model in which the group to be created.</param>
        /// <returns>Index Page.</returns>
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                PageNotificationViewModel responseMessage = new PageNotificationViewModel();
                var currentUser = User.UserId;

                var response = GroupManager.Create(group, currentUser);

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
        /// Search and gives the list of group which contains the search text in group name.
        /// </summary>
        /// <param name="searchText">Search text for which group list to be shown.</param>
        /// <returns>Index page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Search(string searchText)
        {            
            Pager pager = new Pager();
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            GroupListViewModel groupList = new GroupListViewModel();
            List<GroupViewModel> groups = new List<GroupViewModel>();

            if (string.IsNullOrEmpty(searchText))
                responseMessage.AddWarning(Message.TYPE_TEXT_TO_SEARCH);

            int count = GroupManager.GetCount(searchText);
            int totalPages = (int)Math.Ceiling((double)count / (double)pager.PageSize);

            if (totalPages <= 0)
            {
                responseMessage.AddWarning(Message.GROUP_NOT_FOUNT_FOR_SEARCH);
                TempData["Response"] = responseMessage;
                return RedirectToAction("Index");
            }
         
            groupList.PageCount = totalPages;
            groupList.GroupList = groups;

            groupList.SearchText = searchText;            
            groupList.ResponseMessage = responseMessage;

            return View("Index", groupList);
        }

        /// <summary>
        /// Gets the list of groups.
        /// </summary>
        /// <param name="page">Pager Object.</param>
        /// <param name="searchText">Search text for which group name contains.</param>
        /// <returns>List of groups.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult GroupList(int page, string searchText)
        {
            Pager pager = new Pager() { PageNo = page };
            GroupListViewModel groupList = new GroupListViewModel();
            List<Group> groups = null;

            if (string.IsNullOrEmpty(searchText))
                groups = GroupManager.GetList(pager);
            else
                groups = GroupManager.Search(searchText, pager);

            groups.ForEach(p => groupList.GroupList.Add(
                        new GroupViewModel()
                        {
                            Id = p.Id,
                            Title = p.Title,
                            IsCollege = p.IsCollege
                        }));

            return View("_GroupList", groupList);
        }
    }
}