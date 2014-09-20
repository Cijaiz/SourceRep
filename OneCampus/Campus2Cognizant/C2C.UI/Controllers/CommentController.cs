#region References
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.UI.ViewModels;
using C2C.UI.Filters;
using C2C.BusinessEntities;
using C2C.Core.Helper;
#endregion

namespace C2C.UI.Controllers
{
    [Authorize]
    public class CommentController : BaseController
    {
        /// <summary>
        /// Soft delets the comment based on commentid passed
        /// </summary>
        /// <param name="id">Commentid</param>
        /// <returns>Redirects to List Action</returns>
        [CheckPermission(ApplicationPermission.ManageComments)]
        public ActionResult Delete(int id)
        {
            int count = CommentManager.Delete(id);

            return Json(new { CommentCount = count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Activates the comments for users to view the same.
        /// </summary>
        /// <param name="id">Commentid</param>
        /// <returns>Redirects to List Action</returns>
        [CheckPermission(ApplicationPermission.ManageComments)]
        public ActionResult Activate(int id)
        {
            CommentManager.Approve(id);
            return RedirectToAction("ListComments");
        }

        /// <summary>
        /// Hides the comment from other users. Sets the status to inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects to List Action</returns>
        [CheckPermission(ApplicationPermission.ManageComments)]
        public ActionResult Deactivate(int id)
        {
            CommentManager.UnApprove(id);
            return RedirectToAction("ListComments");
        }

        /// <summary>
        /// Lists all the comments for that particular contenttypeid and contentid.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId(Eg : Blog,Forum)</param>
        /// <param name="contentId">ContentId</param>
        /// <returns>View with all the comments listed</returns>
        public ActionResult ListComments(int page, short contentTypeId, int contentId)
        {
            Pager pager = new Pager();
            pager.PageNo = page;
            pager.PageSize = 4;

            List<ContentComment> comments = null;
            comments = CommentManager.GetCommentsForContent(contentTypeId, contentId, pager);
            List<CommentViewModel> commentModel = ConstructCommentModel(comments);

            return PartialView(commentModel);
        }

        /// <summary>
        /// To construct the CommentViewModel
        /// </summary>
        /// <param name="comments">List of comments object</param>
        /// <returns>List of CommentViewModel</returns>
        private List<CommentViewModel> ConstructCommentModel(List<ContentComment> comments)
        {
            List<CommentViewModel> commentModel = new List<CommentViewModel>();
            string commentText = string.Empty;
            string profilePhoto = string.Empty;
            if (comments != null)
            {
                foreach (ContentComment comment in comments)
                {
                    //To check if it's comment of logged in user
                    bool managaComment = User.HasPermission(ApplicationPermission.ManageComments) ? true : false;
                    profilePhoto = string.IsNullOrEmpty(comment.User.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL : StorageHelper.GetMediaFilePath(comment.User.ProfilePhoto, StorageHelper.COMMENT_SIZE);

                    // Diff between commented time and now is calculated and viewmodel is constructed based on that.
                    var time = DateTime.UtcNow - comment.CommentedOn;
                    if
                        (time.TotalDays > 7)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said on" + comment.CommentedOn;

                    else if (time.TotalDays > 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said "
                                                    + Convert.ToInt64(Math.Round(Convert.ToDouble(time.TotalDays))) + " days ago";
                    else if (time.TotalDays == 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said "
                                                    + Convert.ToInt64(Math.Round(Convert.ToDouble(time.TotalDays))) + " day ago";
                    else if (time.TotalHours > 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said "
                                                    + Convert.ToInt64(Math.Round(Convert.ToDouble(time.TotalHours))) + " hours ago";
                    else if (time.TotalHours == 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said "
                                                                + Convert.ToInt64(Math.Round(Convert.ToDouble(time.TotalHours))) + " hour ago";
                    else if
                        (time.TotalMinutes > 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said "
                                                   + Convert.ToInt64(Math.Round(Convert.ToDouble(time.TotalMinutes))) + " minutes ago";
                    else if
                       (time.TotalMinutes < 1)
                        commentText = (comment.User.FirstName != null ? comment.User.FirstName : null) + " said a while ago";

                    commentModel.Add(new CommentViewModel
                    {
                        CommentId = comment.Id,
                        CommentedBy = commentText,
                        CommentDescription = comment.Comment,
                        ProfilePhoto = profilePhoto,
                        ManageComment = managaComment
                    });
                }
            }
            return commentModel;
        }

        /// <summary>
        /// Saves the comment in DB
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId(Eg : Blog,Forum)</param>
        /// <param name="contentId">ContentId</param>
        /// <param name="newcomment">Comment entered by user</param>
        /// <returns>JsonResult with user details</returns>
        [CheckPermission(ApplicationPermission.CanComment)]
        public JsonResult SaveComment(short contentTypeId, int contentId, string newcomment)
        {
            string userName = string.Empty;
            string photopath = string.Empty;
            int commentsCount = 0;
            int commentId =0;

            if (ModelState.IsValid)
            {
                int userId = User.UserId;
                userName = User.FirstName != null ? User.FirstName : User.LastName;
                photopath = string.IsNullOrEmpty(User.PhotoPath) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL : StorageHelper.GetMediaFilePath(User.PhotoPath, StorageHelper.COMMENT_SIZE);
                ContentComment comment = new ContentComment
                {
                    ContentTypeId = contentTypeId,
                    ContentId = contentId,
                    CommentedBy = userId,
                    Comment = newcomment,
                    CommentedOn = DateTime.UtcNow,
                    Status = Status.Active
                };

                var response = CommentManager.SaveComment(comment);
                commentsCount =CommentManager.GetCommentsCountForContent(contentTypeId, contentId);
                commentId = response.RefId;
            }

            return Json(new { CommentCount = commentsCount, UserName = userName, PhotoPath = photopath,CommentId=commentId,CanDelete =User.HasPermission(ApplicationPermission.ManageComments) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieve the comment count and pass it to view.
        /// </summary>
        /// <param name="contentTypeId">ContentTypeId(Eg : Blog,Forum)</param>
        /// <param name="contentId">ContentId</param>
        /// <returns>comment model with count and details of blog</returns>
        public ActionResult Comments(short contentTypeId, int contentId)
        {
            Pager pager = new Pager();
            pager.PageSize = 4;

            int commentCount = CommentManager.GetCommentsCountForContent(contentTypeId, contentId);

            int totalcommentPages = (int)Math.Ceiling((double)commentCount / (double)pager.PageSize);
            List<CommentViewModel> commentModel = new List<CommentViewModel>();

            CommentListViewModel comment = new CommentListViewModel
            {
                ContentTypeId = contentTypeId,
                ContentId = contentId,
                CommentCount = commentCount,
                PageCount = totalcommentPages,
                CommentListModel = commentModel
            };

            return View("Comments", comment);
        }
    }
}
