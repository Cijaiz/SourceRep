#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;

#endregion

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the members of the Group and uses UserGroupWorker to do Data manipulation.
    /// </summary>
    public static class UserGroupManager
    {
        /// <summary>
        /// Gets the list of groups for the user.
        /// </summary>
        /// <param name="userId">User id for which group list to be given.</param>
        /// <returns>List of Groups.</returns>
        public static List<Group> GetGroups(int userId, Pager pager)
        {
            Guard.IsNotZero(userId, "UserId");
            return DAL.UserGroupWorker.GetInstance().GetUserGroups(userId, pager);
        }

        /// <summary>
        /// Gets the list of groups for the user.
        /// </summary>
        /// <param name="userId">User id for which group list to be given.</param>
        /// <returns>List of Groups.</returns>
        public static List<UserGroup> GetUserGroups(int userId, Pager pager)
        {
            Guard.IsNotZero(userId, "UserId");
            return DAL.UserGroupWorker.GetInstance().Get(userId);
        }

        /// <summary>
        /// Adds member for the particular group.
        /// </summary>
        /// <param name="groupId">Group id for which member to be added.</param>
        /// <param name="memberId">Member id to be added.</param>
        /// <param name="userId">Id of the user who is going to add the member.</param>
        /// <returns>Process Response.</returns>
        public static ProcessResponse Add(int groupId, int memberId, int userId)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotZero(memberId, "memberId");
            Guard.IsNotZero(userId, "userId");

            return DAL.UserGroupWorker.GetInstance().Add(groupId, memberId, userId);
        }

        /// <summary>
        /// Deletes the particular member from the Group.
        /// </summary>
        /// <param name="groupId">Group id from which the member is to deleted.</param>
        /// <param name="memberId">Member id to be deleted.</param>
        /// <param name="userId">Id of the user who is going to delete the member.</param>
        /// <returns>Process response.</returns>
        public static ProcessResponse Delete(int groupId, int memberId, int userId)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotZero(memberId, "memberId");
            Guard.IsNotZero(userId, "userId");

            return DAL.UserGroupWorker.GetInstance().Delete(groupId, memberId, userId);
        }

        /// <summary>
        /// Gets the members count for the particular group and with status if given.
        /// </summary>
        /// <param name="groupId">Group id for which the members count to be given.</param>
        /// <param name="status">Status of the members in that group.</param>
        /// <returns>Members count for the group.</returns>
        public static int GetMembersCount(int groupId, int userId, Status? status = null)
        {
            Guard.IsNotZero(groupId, "groupId");
            return DAL.UserGroupWorker.GetInstance().GetMembersCount(groupId, userId, status);
        }

        /// <summary>
        /// Adds list of members for the particular group.
        /// </summary>
        /// <param name="groupId">Group id for which members to be added.</param>
        /// <param name="memberId">Member id's to be added.</param>
        /// <param name="userId">Id of the user who is going to add the members</param>
        /// <returns></returns>
        public static ProcessResponse AddMembers(int groupId, List<int> memberId, int userId)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotNull(memberId, "memberId");
            Guard.IsNotZero(userId, "userId");

            return DAL.UserGroupWorker.GetInstance().AddMembers(groupId, memberId, userId);
        }

        /// <summary>
        /// Gets the non members for the particular group and with search text if given.
        /// </summary>
        /// <param name="groupId">Group id for which the non members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <param name="searchText">Search text for which the member name contains to be given.</param>
        /// <returns>List of members those who are not part of the particular group.</returns>
        public static List<UserProfile> GetNonMembers(int groupId, Pager pager, string searchText)
        {
            Guard.IsNotZero(groupId, "groupId");
            return DAL.UserGroupWorker.GetInstance().GetNonMembers(groupId, pager, searchText);
        }

        /// <summary>
        /// Updates the member for the particular group and with status.
        /// </summary>
        /// <param name="groupId">Group id for which the member to be updated.</param>
        /// <param name="memberId">Member id which is to be updated.</param>
        /// <param name="userId">Id of the user who is going to update.</param>
        /// <param name="status">Status of the member to be updated.</param>
        /// <returns>Process response.</returns>
        public static ProcessResponse Update(int groupId, int memberId, int userId, Status status)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotZero(memberId, "memberId");
            Guard.IsNotZero(userId, "userId");
            Guard.IsNotNull(status, "Status");

            return DAL.UserGroupWorker.GetInstance().Update(groupId, memberId, userId, status);
        }

        /// <summary>
        /// Gets the members count for the list of groups and with status if given.
        /// </summary>
        /// <param name="groupList">List of groups which belongs to logged in user.</param>
        /// <param name="userId">User id for which the members count to be given.</param>
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>Members Count.</returns>
        public static int GroupMembersCount(List<UserGroup> groupList, int userId, Status? status = null)
        {
            Guard.IsNotNull(groupList, "group list");
            Guard.IsNotZero(userId, "UserID");

            return DAL.UserGroupWorker.GetInstance().GroupMembersCount(groupList, userId, status);
        }

        /// <summary>
        /// Gets the non members count for the particular group and with status if given.
        /// </summary>
        /// <param name="groupId">Group id for which the non members count to be given.</param>
        /// <param name="status">Status of the non members in that group.</param>
        /// <returns>Non members count for the group.</returns>
        public static int GetNonMembersCount(int groupId, string searchText = null, Status? status = null)
        {
            Guard.IsNotZero(groupId, "groupId");
            return DAL.UserGroupWorker.GetInstance().GetNonMembersCount(groupId, status, searchText);
        }

        /// <summary>
        /// Gets the non members for the particular group and with search text if given.
        /// </summary>
        /// <param name="groupId">Group id for which the members to be given.</param>
        /// <param name="userId">User id for which the members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>        
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>List of members those who are part of the group.</returns>
        public static List<UserProfile> GetMembers(int groupId, int userId, Pager pager, Status? status = null)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotZero(userId, "UserID");

            return DAL.UserGroupWorker.GetInstance().GetMembers(groupId, userId, pager, status);
        }

        /// <summary>
        /// Updates the members for the particular group and with status.
        /// </summary>
        /// <param name="groupId">Group id for which the members to be updated.</param>
        /// <param name="memberId">List of member id's which is to be updated.</param>
        /// <param name="userId">Id of the user who is going to update.</param>
        /// <param name="status">Status of the member to be updated.</param>
        /// <returns>Process response.</returns>
        public static ProcessResponse UpdateMembers(int groupId, List<int> memberId, int userId, Status status)
        {
            Guard.IsNotZero(groupId, "groupId");
            Guard.IsNotNull(memberId, "memberId");
            Guard.IsNotZero(userId, "userId");
            Guard.IsNotNull(status, "Status");

            return DAL.UserGroupWorker.GetInstance().UpdateMembers(groupId, memberId, userId, status);
        }

        /// <summary>
        /// Gets the members for the particular list of groups of the logged in user.
        /// </summary>
        /// <param name="groupList">List of groups which belongs to logged in user.</param>
        /// <param name="userId">User id for which the members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>List of members those who are part of the group.</returns>
        public static List<UserProfile> GetGroupMembers(List<UserGroup> groupList, int userId, Pager pager, Status? status = null)
        {
            Guard.IsNotNull(groupList, "group list");
            Guard.IsNotZero(userId, "UserID");

            return DAL.UserGroupWorker.GetInstance().GetGroupMembers(groupList, userId, pager, status);
        }
    }
}
