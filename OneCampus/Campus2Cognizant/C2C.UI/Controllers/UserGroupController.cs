#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.UI.Filters;
using C2C.UI.Publisher;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
#endregion

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides CRUD operations for UserGroup Entity Library.
    /// </summary>
    [Authorize]
    public class UserGroupController : BaseController
    {
        /// <summary>
        /// Displays the add member page with list of members to be added for the group.
        /// </summary>
        /// <param name="groupId">Group id for which members to added.</param>
        /// <returns>Add members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult AddMembers(int groupId)
        {
            List<GroupMemberViewModel> members = new List<GroupMemberViewModel>();
            Pager pager = new Pager();
            GroupMemberListViewModel groupMemberList = new GroupMemberListViewModel();

            int membersCount = UserGroupManager.GetNonMembersCount(groupId);
            
            groupMemberList.GroupId = groupId;
            groupMemberList.UserPageCount = (int)Math.Ceiling((double)membersCount / (double)pager.PageSize); 
            groupMemberList.GroupMembers = members;

            groupMemberList.SearchText = string.Empty;
            groupMemberList.SelectedUserIds = new List<int>();
            groupMemberList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View("AddMembers", groupMemberList);
        }

        /// <summary>
        /// Adds the member to the group id given.
        /// </summary>
        /// <param name="groupId">Group id for which member to be added.</param>
        /// <param name="memberId">Member id which is to be added to the group.</param>
        /// <returns>Add members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult AddMember(int groupId, int memberId)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            var currentUser = User.UserId;

            var response = UserGroupManager.Add(groupId, memberId, currentUser);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(Message.MEMBER_ADDED);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            List<int> userIds = new List<int>();
            userIds.Add(memberId);
            NotificationSyncGroupMember(groupId, userIds, SyncStatus.Add);

            return RedirectToAction("AddMembers", new { groupId = groupId });
        }

        /// <summary>
        /// Search and displays the list of user to add for the group.
        /// </summary>
        /// <param name="searchText">Search text for which user to be listed.</param>
        /// <param name="groupId">Group id for which users to be added.</param>
        /// <returns>Add members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult Search(string searchText, int groupId)
        {
            List<GroupMemberViewModel> members = new List<GroupMemberViewModel>();
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            Pager pager = new Pager();
            GroupMemberListViewModel groupMemberList = new GroupMemberListViewModel();

            if (string.IsNullOrEmpty(searchText))
                responseMessage.AddError(Message.TYPE_TEXT_TO_SEARCH);

            int membersCount = UserGroupManager.GetNonMembersCount(groupId,searchText);

            groupMemberList.GroupId = groupId;
            groupMemberList.UserPageCount = (int)Math.Ceiling((double)membersCount / (double)pager.PageSize);
            groupMemberList.GroupMembers = members;

            groupMemberList.SearchText = searchText;
            groupMemberList.SelectedUserIds = new List<int>();
            groupMemberList.ResponseMessage = responseMessage;

            return View("AddMembers", groupMemberList);         
        }

        /// <summary>
        /// Deletes the particular member from the group.
        /// </summary>
        /// <param name="groupId">Group id for which member to be deleted.</param>
        /// <param name="memberId">Member id which is to be deleted.</param>
        /// <returns>Manage members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult DeleteMember(int groupId, int memberId)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            var currentUser = User.UserId;

            var response = UserGroupManager.Delete(groupId, memberId, currentUser);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            List<int> userIds = new List<int>();
            userIds.Add(memberId);

            NotificationSyncGroupMember(groupId, userIds, SyncStatus.Remove);

            return RedirectToAction("ManageMembers", new { groupId = groupId });
        }
        
        /// <summary>
        /// Gets the list of users non group members.
        /// </summary>
        /// <param name="groupId">Group id in which the users is mapped as a member.</param>
        /// <param name="page">Pager object.</param>
        /// <returns>List of members.</returns>
        public ActionResult GetNonGroupMembers(int groupId, int page,string searchText)
        {
            Pager pager = new Pager() { PageNo = page };
            GroupMemberListViewModel groupMemberList = new GroupMemberListViewModel();

            var members = UserGroupManager.GetNonMembers(groupId, pager, searchText);

            members.ForEach(p => groupMemberList.GroupMembers.Add(
                        new GroupMemberViewModel()
                        {
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            UserId = p.Id,

                            UserImage = p.ProfilePhoto,
                            IsChecked = false,
                            GroupId = groupId,
                        }));

            groupMemberList.GroupId = groupId;
            groupMemberList.SelectedUserIds = new List<int>();

            return View("_NonGroupMembers", groupMemberList);
        }

        /// <summary>
        /// Displays the manage member page with list of members in the group.
        /// </summary>
        /// <param name="groupId">Group id for which the member to be managed.</param>
        /// <param name="status">Status of the users in the group.</param>
        /// <returns>Manage members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult ManageMembers(int groupId, Status? status = null)
        {
            Pager pager = new Pager();
            int userId = User.UserId;
            GroupMemberListViewModel groupMemberList = new GroupMemberListViewModel();
            List<GroupMemberViewModel> groupMembers = new List<GroupMemberViewModel>();            

            int membersCount = UserGroupManager.GetMembersCount(groupId, userId, status);

            groupMemberList.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            groupMemberList.StatusList.Add(new SelectListItem { Text = "All", Selected = true, });

            groupMemberList.GroupId = groupId;
            groupMemberList.GroupMembers = groupMembers;
            groupMemberList.UserPageCount = (int)Math.Ceiling((double)membersCount / (double)pager.PageSize);

            if (status.HasValue) 
            {
                groupMemberList.memberStatus = status.ToString();
            }
           
            groupMemberList.SelectedUserIds = new List<int>();
            groupMemberList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View("ManageMembers", groupMemberList);
        }

        /// <summary>
        /// Updates the member status for the particular group.
        /// </summary>
        /// <param name="groupId">Group id for which member status to be updated.</param>
        /// <param name="memberId">Member id to be updated.</param>
        /// <param name="status">Status of the user to be updated.</param>
        /// <returns>Manage members page.</returns>
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult ManageMember(int groupId, int memberId, Status status)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            var currentUser = User.UserId;

            var response = UserGroupManager.Update(groupId, memberId, currentUser, status);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(Message.MEMBER_STATUS_CHANGED);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;


            return RedirectToAction("ManageMembers", new { groupId = groupId });
        }

        /// <summary>
        /// Adds the list of members to the group.
        /// </summary>
        /// <param name="groupMembers">Members to be added.</param>
        /// <param name="groupId">Group id for which member to be added.</param>
        /// <returns>Index page.</returns>
        [HttpPost, ActionName("AddMembers")]
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult AddMembers(GroupMemberListViewModel groupMembers, int groupId)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            var currentUser = User.UserId;

            if (groupMembers.SelectedUserIds != null && groupMembers.SelectedUserIds.Count() > 0)
            {
                var response = UserGroupManager.AddMembers(groupId, groupMembers.SelectedUserIds, currentUser);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(Message.MEMBERS_ADDED);
                    List<int> userIds = new List<int>();
                    userIds = groupMembers.SelectedUserIds;
                    NotificationSyncGroupMember(groupId, userIds, SyncStatus.Add);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
            }
            else
            {
                responseMessage.AddError(Message.SELECT_USER_TO_PROCEED);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("AddMembers", new { groupId = groupId });
        }

        /// <summary>
        /// Updates the list members status in that group.
        /// </summary>
        /// <param name="groupMembers">Members to be updated.</param>
        /// <param name="groupId">Group id for which member to be updated.</param>
        /// <returns>Index page.</returns>
        [HttpPost, ActionName("ManageMembers")]
        [CheckPermission(ApplicationPermission.ManageGroups)]
        public ActionResult ManageMembers(GroupMemberListViewModel groupMembers, int groupId)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            Status status = groupMembers.Status;
            var currentUser = User.UserId;

            if (groupMembers.SelectedUserIds != null && groupMembers.SelectedUserIds.Count() > 0)
            {
                var response = UserGroupManager.UpdateMembers(groupId, groupMembers.SelectedUserIds, currentUser, status);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(Message.MEMBERS_STATUS_UPDATED);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
            }
            else
            {
                responseMessage.AddError(Message.SELECT_USER_TO_PROCEED);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("ManageMembers", new { groupId = groupId });
        }

        /// <summary>
        /// Gets the list of users group members.
        /// </summary>
        /// <param name="groupId">Group id in which the users is mapped as a member.</param>
        /// <param name="page">Pager</param>
        /// <param name="status">Status of the users.</param>
        /// <returns>List of members.</returns>
        public ActionResult GetUserGroupMembers(int groupId, int page, string memberStatus)
        {
            Pager pager = new Pager() { PageNo = page };
            int userId = User.UserId;
            Status status = Status.Active;

            GroupMemberListViewModel groupMemberList = new GroupMemberListViewModel();
            if (!string.IsNullOrEmpty(memberStatus))             
                 status = (Status)Enum.Parse(typeof(Status), memberStatus, true);

            var members = UserGroupManager.GetMembers(groupId, userId, pager, status);
            members.ForEach(p => groupMemberList.GroupMembers.Add(
                        new GroupMemberViewModel()
                        {
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            UserId = p.Id,

                            UserImage = string.IsNullOrEmpty(p.ProfilePhoto) ? C2C.Core.Constants.C2CWeb.DefaultValue.PROFILE_DEFAULT_IMAGE_URL
                                                                             : StorageHelper.GetMediaFilePath(p.ProfilePhoto, StorageHelper.PROFILESUMMARY_SIZE),
                            IsChecked = false,
                            GroupId = groupId,
                            Status = p.Status
                        }));

            groupMemberList.GroupId = groupId;
            groupMemberList.SelectedUserIds = new List<int>();

            return View("_UserGroupMembers", groupMemberList);
        }

        private void NotificationSyncGroupMember(int groupId, List<int> userIds, SyncStatus status)
        {
            string eventCode = string.Empty;
            switch (status)
            {
                case SyncStatus.Add:
                    eventCode = EventCodes.GROUP_ADD_USER;
                    break;
                case SyncStatus.Remove:
                    eventCode = EventCodes.GROUP_REMOVE_USER;
                    break;
            }

            foreach (var Id in userIds)
            {
                PublisherEvents publisherEvents = new PublisherEvents()
                {
                    EventId = "10",
                    EventCode = eventCode,
                    NotificationContent = SerializationHelper.JsonSerialize<GroupMember>(new GroupMember() { UserId = Id, GroupId = groupId })
                };
                NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);
            }

            //Adding code for sending messages to Queue

        }
    }
}