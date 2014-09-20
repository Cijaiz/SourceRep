#region References
using System.Collections.Generic;
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using DAL = C2C.DataAccessLogic;
#endregion

namespace C2C.BusinessLogic
{
    public static class CommentManager
    {
        /// <summary>
        /// Updates the status of the contentId to Status.Deleted
        /// </summary>
        /// <param name="id">ContentId</param>
        public static int Delete(int id)
        {
            ProcessResponse response = null;
            int count = 0;

            ContentComment comment = DAL.CommentWorker.GetInstance().Get(id);
            comment.Status = Status.Deleted;
            response =  DAL.CommentWorker.GetInstance().Update(comment);
            count = response.RefId;

            return count;    
        }

        /// <summary>
        /// Updates the status of the contentId to Status.Active
        /// </summary>
        /// <param name="id">ContentId</param>
        public static void Approve(int id)
        {
            ContentComment comment = DAL.CommentWorker.GetInstance().Get(id);
            comment.Status = Status.Active;
            DAL.CommentWorker.GetInstance().Update(comment);
        }

        /// <summary>
        /// Updates the status of the contentId to Status.Inactive
        /// </summary>
        /// <param name="id">ContentId</param>
        public static void UnApprove(int id)
        {
            ContentComment comment = DAL.CommentWorker.GetInstance().Get(id);
            comment.Status = Status.InActive;
            DAL.CommentWorker.GetInstance().Update(comment);
        }

        /// <summary>
        /// Save the comment texted by the user
        /// </summary>
        /// <param name="newComment">Comment object with details about new comment posted</param>
        /// <returns>Process response with comment count for that content</returns>
        public static ProcessResponse SaveComment(ContentComment comment)
        {
            ProcessResponse response = null;
            response = DAL.CommentWorker.GetInstance().SaveComment(comment);

            return response;
        }

        /// <summary>
        /// To calculate the comments count for a particular content
        /// </summary>
        /// <param name="contentTypeId">Module Id (Eg. Blog)</param>
        /// <param name="contentId">Content id in that module (Eg. particular blog post)</param>
        /// <returns>Comments count for that content</returns>
        public static int GetCommentsCountForContent(short contentTypeId, int contentId)
        {
            int count = 0;
            count = DAL.CommentWorker.GetInstance().GetCommentsCountForContent(contentTypeId, contentId);

            return count;
        }

        /// <summary>
        /// To get the list of comments for particular content
        /// </summary>
        /// <param name="contentTypeId">Module Id (Eg. Blog)</param>
        /// <param name="contentId">Content id in that module (Eg. particular blog post)</param>
        /// <returns>Comments for that content</returns>
        public static List<ContentComment> GetCommentsForContent(short contentTypeId, int contentId, Pager pager)
        {
            List<ContentComment> comments = null;
            comments = DAL.CommentWorker.GetInstance().GetCommentsForContent(contentTypeId, contentId,pager);

            return comments;
        }
    }
}
