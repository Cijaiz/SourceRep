using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Hub;
using C2C.Core.Logger;
using C2C.Core.Extensions;
using NotificationHub.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotificationHub.Controllers
{
    public class NotificationCornerController : ApiController
    {
        /// <summary>
        /// This action is used to get user's feed count.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="feedFilterStatus">feedFilterStatus of type FeedFilterStatus Enum.</param>
        /// <returns>Count</returns>
        [HttpGet]
        public int GetFeedCount(int userId, int feedFilterStatus)
        {
            int responce = 0;
            try
            {
                FeedFilterStatus filterStatus;

                if (Enum.IsDefined(typeof(FeedFilterStatus), feedFilterStatus))
                {
                    filterStatus = (FeedFilterStatus)feedFilterStatus;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                responce = FeedManager.Instance().FeedCount(userId, filterStatus);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return responce;
        }

        /// <summary>
        /// This action is used to get list of user's feeds
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="feedFilterStatus">feedFilterStatus of type FeedFilterStatus Enum.</param>
        /// <param name="page">Page no</param>
        /// <param name="pageSize">Records per page, Default page size is 10.</param>
        /// <returns>List<NotificationContent></returns>
        [HttpGet]
        public List<NotificationContent> GetFeeds(int userId, int feedFilterStatus, int page, int pageSize = 10)
        {
            List<NotificationContent> responce = null;

            try
            {
                FeedFilterStatus filterStatus;
                if (Enum.IsDefined(typeof(FeedFilterStatus), feedFilterStatus))
                {
                    filterStatus = (FeedFilterStatus)feedFilterStatus;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                FeedManager manager = FeedManager.Instance();
                responce = manager.GetFeeds(userId, filterStatus, page, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return responce;
        }

        /// <summary>
        /// This action is used to Insert feeds.
        /// </summary>
        /// <param name="feed">Feed information.</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse InsertFeed(NotificationContent feed)
        {
            ProcessResponse responce = null;
            try
            {
                if (FeedManager.Instance().InsertFeed(feed))
                {
                    responce = new ProcessResponse() { Status = ResponseStatus.Success };
                }
                else
                {
                    responce = new ProcessResponse() { Status = ResponseStatus.Failed };
                }
            }
            catch (ArgumentException ex)
            {
                responce = new ProcessResponse() { Status = ResponseStatus.Failed, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                responce = new ProcessResponse() { Status = ResponseStatus.Error, Message = "Internal server error." };
            }

            return responce;

        }

        /// <summary>
        /// This action is used to updated user's feed status.
        /// </summary>
        /// <param name="userFeedStatus">User feed and status information.</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse UpdateFeedStatus(UserFeedStatus userFeedStatus)
        {
            ProcessResponse responce = null;
            try
            {
                if (FeedManager.Instance().UpdateFeedStatus(userFeedStatus))
                {
                    responce = new ProcessResponse() { Status = ResponseStatus.Success };
                }
                else
                {
                    responce = new ProcessResponse() { Status = ResponseStatus.Failed };
                }
            }
            catch (ArgumentException ex)
            {
                responce = new ProcessResponse() { Status = ResponseStatus.Failed, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                responce = new ProcessResponse() { Status = ResponseStatus.Error, Message = "Internel server error." };
            }

            return responce;
        }

        /// <summary>
        /// This action is used to mark all unread user feeds to read
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse MarkAsRead(UserDetail userDetail)
        {
            ProcessResponse responce = null;
            try
            {
                FeedManager.Instance().MarkAsRead(userDetail.UserId);

                responce = new ProcessResponse() { Status = ResponseStatus.Success };

            }
            catch (ArgumentException ex)
            {
                responce = new ProcessResponse() { Status = ResponseStatus.Failed, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                responce = new ProcessResponse() { Status = ResponseStatus.Error, Message = "rakesh server error." };
            }

            return responce;
        }
    }
}
