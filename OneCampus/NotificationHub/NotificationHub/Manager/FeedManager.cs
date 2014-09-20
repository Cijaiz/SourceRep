using NE = C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Constants.Hub;
using NotificationHub.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C2C.BusinessEntities.C2CEntities;

namespace NotificationHub.Manager
{
    public class FeedManager
    {
        internal static FeedManager Instance()
        {
            return new FeedManager();
        }

        /// <summary>
        /// This method is used to get user's feed count.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="feedFilterStatus">feedFilterStatus of type FeedFilterStatus Enum.</param>
        /// <returns>Count</returns>
        internal int FeedCount(int userId, FeedFilterStatus feedFilterStatus)
        {
            int count = 0;
            if (userId > 0)
            {
                using (var dbContext = Utility.GetDbContext())
                {
                    switch (feedFilterStatus)
                    {
                        case FeedFilterStatus.All:
                            count = dbContext.UserNotifications
                                .Count(p =>
                                    p.UserId == userId &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow);
                            break;
                        case FeedFilterStatus.Read:
                            count = dbContext.UserNotifications
                                .Count(p =>
                                    p.UserId == userId &&
                                    p.IsRead == true &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow);
                            break;
                        case FeedFilterStatus.UnRead:
                            count = dbContext.UserNotifications
                                .Count(p =>
                                    p.UserId == userId &&
                                    p.IsRead == false &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow);
                            break;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid UserId", "userId");
            }
            return count;
        }

        /// <summary>
        /// This Method is used to get list of user's feeds
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="feedFilterStatus">feedFilterStatus of type FeedFilterStatus Enum.</param>
        /// <param name="page">Page no</param>
        /// <param name="pageSize">Records per page</param>
        /// <returns>List<NotificationContent></returns>
        internal List<NE.NotificationContent> GetFeeds(int userId, FeedFilterStatus feedFilterStatus, int page, int pageSize)
        {
            List<NE.NotificationContent> list = new List<NE.NotificationContent>();
            List<UserNotification> feeds = null;

            if (userId > 0)
            {

                if (page < 0) page = 0;
                --page;
                //Limiting Page size to 25
                if (pageSize > 25) pageSize = 25;

                using (var dbContext = Utility.GetDbContext())
                {
                    switch (feedFilterStatus)
                    {
                        case FeedFilterStatus.All:
                            feeds = dbContext.UserNotifications
                                .Where(p =>
                                    p.UserId == userId &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow)
                                .OrderByDescending(o => o.Id)
                                .Skip(page * pageSize)
                                .Take(pageSize)
                                .ToList();
                            break;
                        case FeedFilterStatus.Read:
                            feeds = dbContext.UserNotifications
                                .Where(p =>
                                    p.UserId == userId &&
                                    p.IsRead == true &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow)
                                .OrderByDescending(o => o.Id)
                                .Skip(page * pageSize)
                                .Take(pageSize)
                                .ToList();
                            break;
                        case FeedFilterStatus.UnRead:
                            feeds = dbContext.UserNotifications
                                .Where(p =>
                                    p.UserId == userId &&
                                    p.IsRead == false &&
                                    p.ValidFrom <= DateTime.UtcNow &&
                                    p.ValidTo >= DateTime.UtcNow)
                                .OrderByDescending(o => o.Id)
                                .Skip(page * pageSize)
                                .Take(pageSize)
                                .ToList();
                            break;
                    }
                }
            }

            if (feeds != null && feeds.Count > 0)
            {
                feeds.ForEach(q => list.Add(new NE.NotificationContent()
                {
                    Id = q.Id,
                    ContentType = (Module)q.ContentTypeId,
                    Description = q.Description,
                    URL = q.ContentURL,
                    ValidFrom = q.ValidFrom.GetValueOrDefault(),
                    ValidTo = q.ValidTo.GetValueOrDefault(),
                    IsRead = q.IsRead.GetValueOrDefault(),
                    SharedBy = q.SharedBy
                }));
            }

            return list;
        }

        /// <summary>
        /// This action is used to Insert feeds.
        /// </summary>
        /// <param name="feed">Feed information.</param>
        /// <returns></returns>
        internal bool InsertFeed(NE.NotificationContent feed)
        {
            int updateCount = 0;
            List<int> users = new List<int>();
            if (feed.Users != null)
            {
                users = feed.Users;
            }

            using (var dbContext = Utility.GetDbContext())
            {
                List<int> groupMembers = new List<int>();
                //Check whether group is included for notification
                if (feed.Groups != null && feed.Groups.Count > 0)
                {
                    //Get list of user id who are members of the group.
                    groupMembers = dbContext.UserGroups
                                        .Where(p => feed.Groups.Contains(p.GroupId))
                                        .Select(q => q.UserId)
                                        .Distinct()
                                        .ToList<int>();
                }

                //Combine users list and group members list and add an entity for individual notification.
                users = users.Union(groupMembers).ToList();

                //Check content type for which privacy settings needs to be applied.
                if (feed.ContentType == Module.Share)
                {
                    int doNotDisturbId = (int)NotificationSetting.DoNotDisturb,
                        oustideCollegeId = (int)NotificationSetting.OutsideCollege,
                        ownCollegeId = (int)NotificationSetting.WithinCollege;
                    // get SharedBy user's group id
                    List<int> sharedByUsersGroup = dbContext.UserGroups.Where(p => p.UserId == feed.SharedBy).Select(q => q.GroupId).ToList();

                    var filteredUsers = (from user in dbContext.UserDetails
                                         join ug in dbContext.UserGroups on user.UserId equals ug.UserId into usrgroups
                                         from item in usrgroups.DefaultIfEmpty()
                                         where
                                         (
                                             user.PrivacyStatus != doNotDisturbId &&
                                             user.PrivacyStatus == oustideCollegeId
                                         ) || (
                                             user.PrivacyStatus == ownCollegeId &&
                                             item != null && item.UserId > 0
                                         )
                                         //select user.UserId).Distinct();
                                        select new { UserId = user.UserId, PrivacyStatus = user.PrivacyStatus, GroupId = item.GroupId });

                    var filteredUserId = filteredUsers.Where(p=> sharedByUsersGroup.Contains(p.GroupId)).Select(q=>q.UserId);
                    users = users.Where(p => filteredUserId.Contains(p) && p != feed.SharedBy).ToList();
                }

                foreach (var userId in users)
                {
                    var entity = new UserNotification()
                    {
                        ContentTypeId = (short)feed.ContentType,
                        ContentURL = feed.URL,
                        CreatedOn = DateTime.UtcNow,
                        Description = feed.Description,
                        IsRead = false,
                        UserId = userId,
                        ValidFrom = feed.ValidFrom,
                        ValidTo = feed.ValidTo,
                        SharedBy = feed.SharedBy
                    };

                    dbContext.UserNotifications.Add(entity);
                }

                //Commit the changes
                updateCount = dbContext.SaveChanges();
            }

            return updateCount > 0 ? true : false;
        }

        /// <summary>
        /// This method is used to updated user's feed status.
        /// </summary>
        /// <param name="userFeedStatus">User feed and status information.</param>
        /// <returns>If at least one feed is updated then : True/ Otherwise : False</returns>
        internal bool UpdateFeedStatus(NE.UserFeedStatus userFeedStatus)
        {
            int updateCount = 0;
            using (var dbContext = Utility.GetDbContext())
            {
                if (userFeedStatus != null)
                {
                    List<UserNotification> notifications = null;
                    if (userFeedStatus.FeedId != null && userFeedStatus.FeedId.Count > 0)
                    {
                        notifications = dbContext.UserNotifications.Where(p => p.UserId == userFeedStatus.UserId && userFeedStatus.FeedId.Contains(p.Id)).ToList();

                        if (notifications != null && notifications.Count() > 0)
                            notifications.ForEach(p => p.IsRead = userFeedStatus.FeedStatus == FeedStatus.Read ? true : false);

                        updateCount = dbContext.SaveChanges();
                    }
                }
            }

            return updateCount > 0 ? true : false;
        }

        /// <summary>
        /// This method is used to mark all unread user feeds to read
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>If at least one feed marked as read then return :True / Otherwise : False.</returns>
        internal bool MarkAsRead(int userId)
        {
            int updateCount = 0;
            if (userId > 0)
            {
                using (var dbContext = Utility.GetDbContext())
                {
                    List<UserNotification> notifications = null;

                    notifications = dbContext.UserNotifications.Where(p => p.UserId == userId && p.IsRead == false).ToList();

                    if (notifications != null && notifications.Count() > 0)
                    {
                        notifications.ForEach(p => p.IsRead = true);

                        updateCount = dbContext.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("Invalid User Id");
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("UserId");
            }

            return updateCount > 0 ? true : false;
        }
    }
}