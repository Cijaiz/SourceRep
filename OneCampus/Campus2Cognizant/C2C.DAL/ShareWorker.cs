#region References
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
#endregion

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on Share Entity Libraries.
    /// </summary>
    public class ShareWorker
    {
        /// <summary>
        /// Creates a new instance for ShareWorker class
        /// </summary>
        /// <returns>Returns the new instance</returns>        
        public static ShareWorker GetInstance()
        {
            return new ShareWorker();
        }

        /// <summary>
        /// Gets Shared users count for the particular content.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId</param>
        /// <param name="contentId">Content Id for which shared user count to be given.</param>
        /// <returns>Shared Users Count.</returns>
        public int GetSharedUsersCount(short contentTypeId, int contentId)
        {
            int count = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                count = dbContext.ContentShares.Where(p => p.ContentTypeId == contentTypeId && p.ContentId == contentId).Count();
            }

            return count;
        }

        /// <summary>
        /// Saves the shared users details of the shared content.
        /// </summary>
        /// <param name="sharedTo">List of users to whom the content is shared.</param>
        /// <param name="contentId">Id of the content shared.</param>
        /// <returns>Process Response.</returns>
        public ProcessResponse ShareUsers(List<int> sharedTo, int contentId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                foreach (var item in sharedTo)
                {
                    var sharedUser = new C2C.DataStore.ContentSharedUser()
                    {
                        UserId = item,
                        ContentShareId = contentId
                    };

                    dbContext.ContentSharedUsers.Add(sharedUser);
                }

                if (dbContext.SaveChanges() > 0)
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Success,
                        Message = Message.CREATED
                    };
                }
                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.FAILED
                    };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the list of users shared this content.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId</param>
        /// <param name="contentId">Content Id for which shared user to be given.</param>
        /// <returns>List of shared users.</returns>
        public List<BE.UserProfile> GetSharedUsers(short contentTypeId, int contentId,Pager pager)
        {
            List<BE.UserProfile> users = new List<BE.UserProfile>();
            int count = pager.PageSize;
            int skip = pager.SkipCount;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                users = (from user in dbContext.ContentShares
                         where user.ContentTypeId == contentTypeId && user.ContentId == contentId
                         join profile in dbContext.UserProfiles on user.SharedBy equals profile.Id into userprofile
                         from profile in userprofile.DefaultIfEmpty()
                         select new BE.UserProfile()
                                      {
                                          Id = user.SharedBy,
                                          CollegeId = profile.CollegeId,
                                          FirstName = (profile.FirstName != null) ? profile.FirstName : null,

                                          LastName = (profile.LastName != null) ? profile.LastName : null,
                                          ProfilePhoto = profile.ProfilePhoto
                                      }).Distinct().OrderBy(o => o.Id).Skip(skip).Take(count).ToList();
            }

            return users;
        }

        /// <summary>
        /// Saves the shared content details of the shared content.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId.</param>
        /// <param name="contentId">Id of the content to be shared.</param>
        /// <param name="userId">Id of the user who is going to share the content.</param>
        /// <param name="sharedUserId">List of users to whom the content to be shared.</param>
        /// <returns>Shared users count.</returns>
        public int Share(short contentTypeId, int contentId, int userId, List<int> sharedUserId)
        {
            int count = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var share = new C2C.DataStore.ContentShare()
                {
                    ContentId = contentId,
                    ContentTypeId = contentTypeId,
                    SharedBy = userId,
                    SharedOn = DateTime.UtcNow
                };

                dbContext.ContentShares.Add(share);
                dbContext.SaveChanges();

                count = GetSharedUsersCount(contentTypeId, contentId);
                ShareUsers(sharedUserId, share.Id);
            }

            return count;
        }

        /// <summary>
        /// Gets the list of users to whom this content to be shared.
        /// </summary>
        /// <param name="groupList">Group list of the user sharing the content.</param>
        /// <param name="userId">Id of the user who is going to share the content.</param>
        /// <param name="pager">Pager object.</param>
        /// <returns>List of Users</returns>
        public List<BE.UserProfile> GetUserList(List<BE.UserGroup> groupList, int userId, Pager pager)
        {
            List<BE.UserProfile> usersList = new List<BE.UserProfile>();
            Status status = Status.Active;
            List<BE.UserProfile> users = null;

            //Gets the users list from the group list.
            users = UserGroupWorker.GetInstance().GetGroupMembers(groupList, userId, pager, status);
            users.ForEach(p => usersList.Add(
                new BE.UserProfile()
                {
                    Id = p.Id,
                    Email = p.Email,
                    FirstName = (p.FirstName != null) ? p.FirstName : null,

                    LastName = (p.LastName != null) ? p.LastName : null,
                    ProfilePhoto = string.IsNullOrEmpty(p.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL 
                                                                        : StorageHelper.GetMediaFilePath(p.ProfilePhoto,StorageHelper.SHARE_SIZE),
                    Status = p.Status,

                    UpdatedBy = p.UpdatedBy,
                    UpdatedOn = p.UpdatedOn
                }));
            
            return usersList;
        }
    }
}
