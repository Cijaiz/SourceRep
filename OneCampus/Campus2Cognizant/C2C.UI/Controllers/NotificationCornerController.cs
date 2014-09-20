#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Extensions;
using C2C.UI.ViewModels;
using C2C.Core.Constants.Hub;
using C2C.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using C2C.Core.Constants.Engine;
using C2C.Core.Logger;
using System.Threading;
#endregion

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides Feed read and show operations for Notification data.
    /// </summary>
    [Authorize]
    public class NotificationCornerController : BaseController
    {
        // Gets the notification URL.
        private string _notificationUrl = string.Format("{0}/{1}", CommonHelper.GetConfigSetting(C2C.Core.Constants.C2CWeb.Key.HUB_URL), "NotificationCorner");
        private string _error;

        /// <summary>
        /// Makes the read feed as MarkAsRead.
        /// </summary>
        /// <returns>Json result.</returns>
        public ActionResult MarkAsRead()
        {
            bool error = false;
            int userId = User.UserId;

            try
            {
                WebClient webClient = new WebClient();
                //Mark all the unread feeds as read here
                string MarkasreadUrl = string.Format("{0}/{1}",
                    _notificationUrl,
                    "MarkAsRead");

                webClient.WebClientPostRequest(MarkasreadUrl, new UserDetail() { UserId = userId }, CommonHelper.GetHeader(Consumer.Hub));
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                _error = "Error Occurred";
            }

            return Json(new { Error = error });
        }

        /// <summary>
        /// Gets the count of the feed.
        /// </summary>
        /// <returns>Get feed count view.</returns>
        public ActionResult GetFeedCount()
        {
            int count = 0;

            try
            {
                count = ReturnFeedCount(FeedFilterStatus.UnRead);
            }
            catch (Exception ex)
            {
                 Logger.Error(ex.Message);
                _error = "Error Occurred";
            }

            return View("FeedCount", new FeedCountViewModel { FeedCount = count,Error=_error });
        }
        
        /// <summary>
        /// Gets the unread feeds.
        /// </summary>
        /// <returns>UnReadFeeds view.</returns>
        public ActionResult GetUnReadFeeds()
        {
            int userId = User.UserId;
            List<NotificationContent> items = null;
            WebClient webClient = new WebClient();

            try
            {
                var text = webClient.SafeWebClientProcessing(string.Format("{0}/{1}?userid={2}&feedFilterStatus={3}&page=1",
                            _notificationUrl,
                            "GetFeeds",
                            userId,
                            (int)FeedFilterStatus.UnRead), CommonHelper.GetHeader(Consumer.Hub));

                items = DecodedNotificationList(SerializationHelper.JsonDeserialize<List<NotificationContent>>(text));
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                _error = "Error Occurred";
            }

            return View("UnReadFeeds", new UnreadFeedsViewModel { Error = _error, NotificationContents = items });
        }

        /// <summary>
        /// Gets all the feeds.
        /// </summary>
        /// <param name="pager">Pager object.</param>
        /// <returns>All feeds view.</returns>
        public ActionResult GetAllFeeds(Pager pager)
        {
            string userId = Convert.ToString(User.UserId);
            List<NotificationContent> items = null;
            int itemsCount = 0;

            try
            {
                WebClient webClient = new WebClient();
                var text = webClient.SafeWebClientProcessing(string.Format("{0}/{1}?userid={2}&feedFilterStatus={3}&page={4}&pageSize={5}",
                         _notificationUrl,
                         "GetFeeds",
                         userId,
                         (int)FeedFilterStatus.All, pager.PageNo, pager.PageSize), CommonHelper.GetHeader(Consumer.Hub));
                items = DecodedNotificationList(SerializationHelper.JsonDeserialize<List<NotificationContent>>(text));
                itemsCount = ReturnFeedCount(FeedFilterStatus.All);
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                _error = "Error Occurred";
            }

            return View("AllFeeds", new AllFeedListViewModel { Pager = pager, NotificationContents = items, Error = _error });
        }

        /// <summary>
        /// Gets the feed count.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>Feed count.</returns>
        private int ReturnFeedCount(FeedFilterStatus status)
        {
            int count = 0;
            string userId = User.UserId.ToString();
            WebClient webClient = new WebClient();

            var text = webClient.SafeWebClientProcessing(string.Format("{0}/{1}?userid={2}&feedFilterStatus={3}", _notificationUrl, "GetFeedCount", userId, (int)status), CommonHelper.GetHeader(Consumer.Hub));
            count = SerializationHelper.JsonDeserialize<int>(text);

            return count;
        }

        /// <summary>
        /// Decodes the notification list items.
        /// </summary>
        /// <param name="items">Notification content items.</param>
        /// <returns>List of Notification content items.</returns>
        private List<NotificationContent> DecodedNotificationList(List<NotificationContent> items)
        {
            IEnumerable<NotificationContent> decodedItems = null;
            decodedItems = from item in items
                           select new NotificationContent()
                           {
                               ContentType = item.ContentType,
                               ContentTitle = WebUtility.HtmlDecode(item.ContentTitle),
                               Description = WebUtility.HtmlDecode(item.Description),

                               Groups = item.Groups,
                               Id = item.Id,
                               IsRead = item.IsRead,

                               SharedBy = item.SharedBy,
                               URL = item.URL,
                               Users = item.Users,

                               ValidFrom = item.ValidFrom,
                               ValidTo = item.ValidTo,
                           };

            return decodedItems.ToList();
        }
    }
}