#region References
using C2C.BusinessEntities;
using C2C.Core.Helper;
using System.Collections.Generic;
using BE = C2C.BusinessEntities.C2CEntities;
using DAL = C2C.DataAccessLogic;
#endregion 

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the Share and Uses ShareWorker to do Data manipulation.
    /// </summary>
    public static class ShareManager
    {
        /// <summary>
        /// Gets count of the shared users.
        /// </summary>
        /// <param name="contentTypeId">Content type id.</param>
        /// <param name="contentId">Id of the content to be shared.</param>
        /// <returns>Shared user count.</returns>
        public static int GetSharedUsersCount(short contentTypeId, int contentId)
        {
            Guard.IsNotZero(contentTypeId, "contentTypeId");
            Guard.IsNotZero(contentId, "contentId");

            return DAL.ShareWorker.GetInstance().GetSharedUsersCount(contentTypeId, contentId);
        }

        /// <summary>
        /// Gets the shared user list.
        /// </summary>
        /// <param name="contentTypeId">Content type id.</param>
        /// <param name="contentId">Id of the content to be shared.</param>
        /// <returns>List of Shared user.</returns>
        public static List<BE.UserProfile> GetSharedUsers(short contentTypeId, int contentId,Pager pager)
        {
            Guard.IsNotZero(contentTypeId, "contentTypeId");
            Guard.IsNotZero(contentId, "contentId");

            return DAL.ShareWorker.GetInstance().GetSharedUsers(contentTypeId, contentId,pager);
        }

        /// <summary>
        /// Saves the Shared content details.
        /// </summary>
        /// <param name="contentTypeId">Content type id.</param>
        /// <param name="contentId">Id of the content to be shared.</param>
        /// <param name="userId">Id of the user who is going to share this content.</param>
        /// <param name="sharedUserId">List of users to whom the content to be shared.</param>
        /// <returns>Shared users count.</returns>
        public static int Share(short contentTypeId, int contentId, int userId, List<int> sharedUserId)
        {
            Guard.IsNotZero(contentTypeId, "contentTypeId");
            Guard.IsNotZero(contentId, "contentId");
            Guard.IsNotZero(userId, "UserId");
            Guard.IsNotNull(sharedUserId, "SharedUserId");

            return DAL.ShareWorker.GetInstance().Share(contentTypeId, contentId, userId, sharedUserId);
        }

        /// <summary>
        /// Gets the list of user to share the content.
        /// </summary>
        /// <param name="groupList">Logged in user group list.</param>
        /// <param name="userId">Logged in user id.</param>
        /// <param name="pager">Pager object.</param>
        /// <returns></returns>
        public static List<BE.UserProfile> GetUserList(List<BE.UserGroup> groupList,int userId,Pager pager)
        {
            Guard.IsNotNull(groupList, "Group List");
            Guard.IsNotZero(userId, "UserId");

            return DAL.ShareWorker.GetInstance().GetUserList(groupList,userId,pager);
        }
    }
}
