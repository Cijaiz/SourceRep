using C2C.BusinessEntities;
using System.Collections.Generic;
using BE = C2C.BusinessEntities.C2CEntities;
using DAL = C2C.DataAccessLogic;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Like Manager calls the like worker to do like specific operations.
    /// </summary>
    public static class LikeManager
    {
        /// <summary>
        /// Performs business manipulation on like.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <param name="userId">id of the users who liked it</param>
        /// <returns>total liked users count</returns>
        public static int Like(short contentTypeId, int contentId,int userId)
        {
            return DAL.LikeWorker.GetInstance().LikeSave(contentTypeId, contentId, userId);
        }

        /// <summary>
        /// Performs business manipulation on unlike.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <param name="userId">id of the users who un-liked it</param>
        /// <returns>total liked users count</returns>
        public static int UnLike(short contentTypeId, int contentId, int userId)
        {
            return DAL.LikeWorker.GetInstance().UnLikeSave(contentTypeId, contentId,userId);
        }

        /// <summary>
        /// Checks whether the user liked the content
        /// </summary>
        /// <param name="userId">id of the user to check</param>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>Bool depending on user liked or not</returns>
        public static bool IsLiked(int userId, short contentTypeId, int contentId)
        {
            return DAL.LikeWorker.GetInstance().IsLiked(userId, contentTypeId, contentId);
        }

        /// <summary>
        /// Gets the count of liked users
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>total liked users count</returns>
        public static int GetLikedUsersCount(short contentTypeId, int contentId)
        {
            return DAL.LikeWorker.GetInstance().GetLikedUsersCount(contentTypeId, contentId);
        }

        /// <summary>
        /// Gets the list of liked users.
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>list of users who liked the content</returns>
        public static List<BE.UserProfile> GetLikedUsers(short contentTypeId, int contentId,Pager pager)
        {
            return DAL.LikeWorker.GetInstance().GetLikedUsers(contentTypeId, contentId,pager);
        }
    }
}
