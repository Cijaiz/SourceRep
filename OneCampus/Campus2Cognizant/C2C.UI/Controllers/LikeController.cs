using C2C.BusinessLogic;
using System.Collections.Generic;
using System.Web.Mvc;
using C2C.BusinessEntities.C2CEntities;
using C2C.UI.ViewModels;
using C2C.BusinessEntities;
using System;
using C2C.UI.Filters;
using C2C.Core.Constants.C2CWeb;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Like Controller calls the business layer to perform Like specific operations
    /// </summary>
    [Authorize]
    public class LikeController : BaseController
    {
        /// <summary>
        /// Un-likes the content
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>JSON containing liked users count and is liked bool field</returns>
        [CheckPermission(ApplicationPermission.CanLike)]
        public ActionResult UnLike(short contentTypeId, int contentId)
        {
            int userId = User.UserId;
            int count = LikeManager.UnLike(contentTypeId, contentId, userId);
            bool isLiked = LikeManager.IsLiked(userId, contentTypeId, contentId);
            return Json(new { LikedUsersCount = count, IsLiked = isLiked }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Likes the content
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>JSON containing liked users count and is-liked bool field</returns>
        /// 
        [CheckPermission(ApplicationPermission.CanLike)]
        public JsonResult Like(short contentTypeId, int contentId)
        {
            int userId = User.UserId;
            int count = LikeManager.Like(contentTypeId, contentId, userId);
            bool isLiked = LikeManager.IsLiked(userId, contentTypeId, contentId);
            return Json(new { LikedUsersCount = count, IsLiked = isLiked }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets all liked users
        /// </summary>
        /// <param name="contentTypeId">id of the module eg.Blog</param>
        /// <param name="contentId">id of the content eg.blogpost</param>
        /// <returns>view displaying all liked users</returns>
        public ActionResult LikedUsers(short contentTypeId, int contentId)
        {
            int likePageCount = 0;
            Pager pager = new Pager() { PageNo = 1, PageSize = 15 };

            var userCount = LikeManager.GetLikedUsersCount(contentTypeId, contentId);
            likePageCount = (int)Math.Ceiling((double)userCount / (double)pager.PageSize);

            List<UserProfile> likedUsersList = new List<UserProfile>();
            LikeViewModel like = new LikeViewModel()
                                     {
                                         ContentId = contentId,
                                         ContentTypeId = contentTypeId,

                                         UsersList = likedUsersList,
                                         UserCount = likePageCount
                                     };

            return View(like);
        }

        /// <summary>
        /// Displays the list of liked users.
        /// </summary>
        /// <param name="page">Pager parameters for Pagination</param>
        /// <param name="contentTypeId">Module Id(eg.Blog,Poll etc)</param>
        /// <param name="contentId">Id of the Blog</param>
        /// <returns></returns>
        public ActionResult LikedUserList(int page, short contentTypeId, int contentId)
        {
            Pager pager = new Pager() { PageNo = page, PageSize = 15 };

            List<UserProfile> likedUsersList = LikeManager.GetLikedUsers(contentTypeId, contentId,pager);
            return View("_LikedUserList", likedUsersList);
        }
    }
}
