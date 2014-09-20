using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    /// <summary>
    ///  Like Worker hits the database to do crud operations & retrieve other like specific data 
    /// </summary>
    public class LikeWorker
    {
        /// <summary>
        /// Creates an instance for LikeWorker class.
        /// </summary>
        /// <returns>an instance of LikeWorker</returns>
        public static LikeWorker GetInstance()
        {
            return new LikeWorker();
        }

        /// <summary>
        /// An entry is inserted in ContentLike table for given contentTypeId,contentId,userId as the user liked it.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <param name="userId">id of the user who liked the content</param>
        /// <returns>total liked users count</returns>
        public int LikeSave(short contentTypeId, int contentId, int userId)
        {
            using (C2CStoreEntities likeStore = RepositoryManager.GetStoreEntity())
            {
                var like = new C2C.DataStore.ContentLike()
                 {
                     ContentId = contentId,
                     ContentTypeId = contentTypeId,
                     LikedOn = DateTime.UtcNow,
                     LikedBy = userId
                 };
                likeStore.ContentLikes.Add(like);
                likeStore.SaveChanges();

                int count = GetLikedUsersCount(contentTypeId, contentId);
                return count;
            }
        }

        /// <summary>
        /// An entry is deleted from ContentLike table for given contentTypeId,contentId,userId as the user un-liked it.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <param name="userId">id of the user who un-liked the content</param>
        /// <returns>total liked users count</returns>
        public int UnLikeSave(short contentTypeId, int contentId, int userId)
        {
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                C2C.DataStore.ContentLike like = dbContext.ContentLikes.Where(c => c.LikedBy == userId && c.ContentTypeId == contentTypeId && c.ContentId == contentId).FirstOrDefault();
                dbContext.ContentLikes.Remove(like);
                dbContext.SaveChanges();
                int count = GetLikedUsersCount(contentTypeId, contentId);
                return count;
            }
        }

        /// <summary>
        /// Gets the list of liked users.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>list of users who liked the content</returns>
        public List<BE.UserProfile> GetLikedUsers(short contentTypeId, int contentId,Pager pager)
        {
            List<BE.UserProfile> users = new List<BE.UserProfile>();
            int count = pager.PageSize;
            int skip = pager.SkipCount;

            dynamic likedUsers;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                likedUsers = (from user in dbContext.ContentLikes
                              where user.ContentTypeId == contentTypeId && user.ContentId == contentId
                              join profile in dbContext.UserProfiles on user.LikedBy equals profile.Id into userprofile
                              from profile in userprofile.DefaultIfEmpty()
                              select new { user.Id, profile.CollegeId, profile.FirstName, profile.LastName, profile.ProfilePhoto }).Distinct().OrderBy(o => o.Id).Skip(skip).Take(count).ToList();

                if (likedUsers != null)
                {
                    foreach (var item in likedUsers)
                    {
                        BE.UserProfile user = new BE.UserProfile();
                        user.Id = item.Id;
                        user.FirstName = (item.FirstName != null) ? item.FirstName : null;
                        user.LastName = (item.LastName != null) ? item.LastName : null;  
                        user.ProfilePhoto = string.IsNullOrEmpty(item.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL :
                        StorageHelper.GetMediaFilePath(item.ProfilePhoto, StorageHelper.SHARE_SIZE);
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Gets the count of liked users
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>total liked users count</returns>
        public int GetLikedUsersCount(short contentTypeId, int contentId)
        {
            int count = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                count = dbContext.ContentLikes.Where(c => c.ContentTypeId == contentTypeId && c.ContentId == contentId).Count();
            }

            return count;
        }

        /// <summary>
        /// Checks if a user liked a particular content
        /// </summary>
        /// <param name="userId">id of the user to check</param>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>returns true or false depending on whether the user liked the given content or not</returns>
        public bool IsLiked(int userId, short contentTypeId, int contentId)
        {
            bool isLiked;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                int likedUser = dbContext.ContentLikes.Where(c => c.ContentTypeId == contentTypeId && c.ContentId == contentId && c.LikedBy == userId).Count();
                if (likedUser > 0)
                    isLiked = true;
                else
                    isLiked = false;
            }
            return isLiked;
        }
    }
}
