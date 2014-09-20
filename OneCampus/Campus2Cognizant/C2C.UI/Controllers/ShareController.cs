#region References
using System;
using System.Collections.Generic;
using System.Linq;
using C2C.BusinessEntities;
using C2C.BusinessLogic;
using System.Web.Mvc;
using C2C.UI.ViewModels;
using BE = C2C.BusinessEntities.C2CEntities;
using C2C.Core.Helper;
using C2C.Core.Constants.C2CWeb;
using C2C.BusinessEntities.C2CEntities;
using C2C.UI.Filters;
using C2C.BusinessEntities.NotificationEntities;
using System.Net;
using C2C.Core.Constants.Engine;
using C2C.UI.Publisher;
#endregion

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides CRUD operations for Share Entity Library.
    /// </summary>
    [Authorize]
    public class ShareController : BaseController
    {
        /// <summary>
        /// Displays the list of users to share as a partial view.
        /// </summary>
        /// <param name="page">Pager object</param>
        /// <returns>List of Users.</returns>
        public ActionResult UserList(int page)
        {
            int userId = User.UserId;
            Pager pager = new Pager();
            pager.PageNo = page;
            pager.PageSize = 15;

            var groupList = UserGroupManager.GetUserGroups(userId, pager);
            List<UserProfile> userList = ShareManager.GetUserList(groupList, userId, pager);

            return View("_UserList", userList);
        }

        /// <summary>
        /// Displays the shared users page for the content. 
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId.</param>
        /// <param name="contentId">Id of the shared content.</param>
        /// <returns>Shared Users view.</returns>
        public ActionResult SharedUsers(short contentTypeId, int contentId)
        {
            Pager pager = new Pager();
            pager.PageSize = 15;

            int userCount = ShareManager.GetSharedUsersCount(contentTypeId, contentId);
            int sharedUserPages = (int)Math.Ceiling((double)userCount / (double)pager.PageSize);

            List<BE.UserProfile> sharedUsers = new List<UserProfile>();            
            ShareViewModel userList = new ShareViewModel()
                                            {
                                                ContentId = contentId,
                                                ContentTypeId = contentTypeId,

                                                UsersList = sharedUsers,
                                                UserCount = sharedUserPages
                                            };
            return View(userList);
        }

        /// <summary>
        /// Gets the list of shared users for the particular content. 
        /// </summary>
        /// <param name="page">Pager object.</param>
        /// <param name="contentTypeId">ContentTypeId.</param>
        /// <param name="contentId">Id of the shared content.</param>
        /// <returns>List of shared users.</returns>
        public ActionResult SharedUsersList(int page, short contentTypeId, int contentId)
        {
            Pager pager = new Pager();
            pager.PageNo = page;
            pager.PageSize = 15;

            var sharedUsersList = ShareManager.GetSharedUsers(contentTypeId, contentId, pager);
            if (sharedUsersList != null && sharedUsersList.Count() > 0)
            {
                foreach (var item in sharedUsersList)
                {
                    if (string.IsNullOrEmpty(item.ProfilePhoto))
                        item.ProfilePhoto = C2C.Core.Constants.C2CWeb.DefaultValue.PROFILE_DEFAULT_IMAGE_URL;
                    else
                        item.ProfilePhoto = StorageHelper.GetMediaFilePath(item.ProfilePhoto, StorageHelper.SHARE_SIZE);
                }
            }

            return View("_sharedUsersList", sharedUsersList);
        }

        /// <summary>
        /// Displays the share content page.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId.</param>
        /// <param name="contentId">Id of the content to be shared.</param>
        /// <param name="contentTitle">Title of the content to be shared.</param>
        /// <returns>Share content view.</returns>
        public ActionResult ShareContent(short contentTypeId, int contentId, string contentTitle)
        {
            int userId = User.UserId;
            Pager pager = new Pager();
            pager.PageSize = 15;
            
            var groupList = UserGroupManager.GetUserGroups(userId, pager);
            int usersCount = UserGroupManager.GroupMembersCount(groupList, userId);

            List<UserProfile> userList = new List<UserProfile>();
            int totalSharePages = (int)Math.Ceiling((double)usersCount / (double)pager.PageSize);

            ShareViewModel share = new ShareViewModel()
            {
                ContentTypeId = contentTypeId,
                ContentId = contentId,

                ContentTitle = contentTitle,
                UsersList = userList,
                UserCount = totalSharePages
            };

            return View("ShareContent", share);
        }

        /// <summary>
        /// Submits the shared content details.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId.</param>
        /// <param name="contentId">Id of the shared content.</param>
        /// <param name="sharedTo">Users to whom the content shared.</param>
        /// <param name="description">Description for the content shared.</param>
        /// <returns>Json object.</returns>
        [CheckPermission(ApplicationPermission.ShareContent)]
        public ActionResult SubmitContent(short contentTypeId, int contentId, string sharedTo, string description)
        {
            int userId = User.UserId;
            List<int> sharedUserId = new List<int>();

            if (string.IsNullOrEmpty(description)) 
            {
                return Json(new { Status = false, Message = "Must specify a comment to proceed.!!" }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(sharedTo))
            {
                //shared to the user came as a ',' separated string. Removing that ',' separation here and filled to string array.
                string[] userList = sharedTo.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();

                //User list contains string array of shared to users. That is separated here and converted as a int and filled to SharedUserId list of int.
                if (userList != null && userList.Count() > 0)
                {
                    foreach (var item in userList)
                    {
                        sharedUserId.Add(Convert.ToInt32(item));
                    }
                }

                int count = ShareManager.Share(contentTypeId, contentId, userId, sharedUserId);

                string contentTitle = string.Empty;

                switch (contentTypeId)
                {
                    case (short)Module.Blog:
                        contentTitle = BlogManager.Get(contentId).Title;
                        break;

                    default:
                        break;

                }

                NotificationContent notificationContent = new NotificationContent()
                {
                    //URL = _orchardServices.ContentManager.GetItemMetadata(share.ContentItem).DisplayRouteValues.ToString(),
                    URL = GetUrl(contentId, "Blog", "Article"),
                    Id = contentId,
                    ContentTitle = WebUtility.HtmlEncode(contentTitle),
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddYears(5),
                    Description = WebUtility.HtmlEncode(User.FirstName + " has shared " + contentTitle),
                    SharedBy = User.UserId,
                    Users = sharedUserId,
                };

                PublisherEvents publisherEvents = new PublisherEvents()
                {
                    EventId = contentId.ToString(),
                    EventCode = EventCodes.SHARE_CONTENT,
                    NotificationContent = SerializationHelper.JsonSerialize<NotificationContent>(notificationContent)

                };

                C2C.UI.Publisher.NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);

                //returns the Json result to Ajax script to show the status.
                return Json(new { Status = true, Message = "Your content shared successfully.!!", SharedUsersCount = count }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Message = "Select at least one user to proceed.!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetUrl(int itemId, string controller, string action)
        {
            string domainUrl = string.Empty;
            string itemUrl = string.Empty;
            Uri uri = Request.Url;
            domainUrl = String.Format("{0}{1}{2}", (Request.IsSecureConnection) ? "https://" : "http://",
                                              uri.Host, (uri.IsDefaultPort) ? "" : String.Format(":{0}", uri.Port));
            itemUrl = string.Format("{0}/{1}/{2}/{3}/", domainUrl,
                controller, action, itemId.ToString());
            return itemUrl;
        }
    }
}