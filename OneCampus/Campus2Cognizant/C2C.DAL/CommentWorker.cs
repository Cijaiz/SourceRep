#region References
using System;
using System.Collections.Generic;
using System.Linq;
using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using BE = C2C.BusinessEntities.C2CEntities;
#endregion

namespace C2C.DataAccessLogic
{
    public class CommentWorker
    {
        public static CommentWorker GetInstance()
        {
            return new CommentWorker();
        }

        /// <summary>
        /// Retrieve comment entity based on contentId.
        /// </summary>
        /// <param name="id">ContentId</param>
        /// <returns>Comment object</returns>
        public BE.ContentComment Get(int id)
        {
            BE.ContentComment contentComment = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var comment = dbContext.ContentComments.Where(c => c.Id == id && c.Status != (byte)Status.Deleted).FirstOrDefault();
                if (comment != null && comment.Id > 0)
                {
                    contentComment = new BE.ContentComment()
                    {
                        Id = comment.Id,
                        CommentedBy = comment.CommentedBy,
                        CommentedOn = comment.CommentedOn,
                        ContentTypeId = comment.ContentTypeId,
                        ContentId = comment.ContentId,
                        Comment = comment.Comment,
                        Status = (Status)comment.Status
                    };
                }
            }

            return contentComment;
        }

        /// <summary>
        /// Save the comment texted by the user
        /// </summary>
        /// <param name="newComment">Comment object with details about new comment posted</param>
        /// <returns>Process response with comment count for that content</returns>
        public ProcessResponse SaveComment(BE.ContentComment newComment)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var comment = new C2C.DataStore.ContentComment()
                {
                    ContentTypeId = newComment.ContentTypeId,
                    ContentId = newComment.ContentId,
                    Comment = newComment.Comment,
                    CommentedBy = newComment.CommentedBy,
                    CommentedOn = newComment.CommentedOn,
                    Status = (byte)newComment.Status,
                    CreatedBy = newComment.UpdatedBy,
                    CreatedOn = DateTime.UtcNow
                };
                dbContext.ContentComments.Add(comment);

                if (dbContext.SaveChanges() > 0)
                {
                    response = new ProcessResponse()
                    {
                        RefId =comment.Id,
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
        /// To calculate the comments count for a particular content
        /// </summary>
        /// <param name="contentTypeId">Module Id (Eg. Blog)</param>
        /// <param name="contentId">Content id in that module (Eg. particular blog post)</param>
        /// <returns>Comments count for that content</returns>
        public int GetCommentsCountForContent(short contentTypeId, int contentId)
        {
            int count = 0;
            using (C2CStoreEntities dbConntext = RepositoryManager.GetStoreEntity())
            {
                count = dbConntext.ContentComments.Where(c => c.Status == (byte)Status.Active && c.ContentTypeId == contentTypeId && c.ContentId == contentId).Count();
            }
            return count;
        }

        /// <summary>
        /// To get the list of comments for particular content
        /// </summary>
        /// <param name="contentTypeId">Module Id (Eg. Blog)</param>
        /// <param name="contentId">Content id in that module (Eg. particular blog post)</param>
        /// <returns>Comments for that content</returns>
        public List<BE.ContentComment> GetCommentsForContent(short contentTypeId, int contentId, Pager pager)
        {
            List<BE.ContentComment> comments = null;
            int count = pager.PageSize;
            int skip = pager.SkipCount;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var commentsList = dbContext.ContentComments.Where(c => c.Status == (byte)Status.Active && c.ContentTypeId == contentTypeId && c.ContentId == contentId).OrderBy(o => o.CommentedOn).ToList();

                if (commentsList != null && commentsList.Count() > 0)
                {
                    comments = new List<BE.ContentComment>();

                    comments = (from user in dbContext.ContentComments
                                where user.ContentTypeId == contentTypeId && user.ContentId == contentId && user.Status == (byte)Status.Active
                                join profile in dbContext.UserProfiles on user.CommentedBy equals profile.Id into userprofile
                                from profile in userprofile.DefaultIfEmpty()
                                select new BE.ContentComment()
                                {
                                    Id = user.Id,
                                    CommentedBy = user.CommentedBy,
                                    CommentedOn = user.CommentedOn,
                                    Comment = user.Comment,
                                    User = new BE.UserProfile
                                    {
                                        FirstName = profile.FirstName,
                                        LastName = profile.LastName,
                                        ProfilePhoto = profile.ProfilePhoto,
                                        Status = (Status)profile.Status
                                    }
                                }).Distinct().OrderBy(o => o.Id).Skip(skip).Take(count).ToList();
                }
            }
            return comments;
        }

        /// <summary>
        /// Updates the status of the comment in DB
        /// </summary>
        /// <param name="newComment">Comment object with details about comment entity to be updated</param>
        /// <returns>Process response with status of transaction</returns>
        public ProcessResponse Update(BE.ContentComment newcomment)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var comment = dbContext.ContentComments.Where(c => c.Id == newcomment.Id).FirstOrDefault();

                if (comment != null)
                {
                    comment.Comment = newcomment.Comment;
                    comment.Status = (byte)newcomment.Status;
                    comment.UpdatedBy = newcomment.UpdatedBy;
                    comment.UpdatedOn = DateTime.UtcNow;

                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse()
                    {
                        RefId = GetCommentsCountForContent(comment.ContentTypeId, comment.ContentId),
                        Status = ResponseStatus.Success,
                        Message = string.Format(Message.UPDATED_COUNT, count)
                    };
                }
                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.RECORED_NOT_FOUND
                    };
                }
            }

            return response;
        }
    }

}
