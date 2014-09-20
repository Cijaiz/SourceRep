using System;
using System.Linq;
using NotificationHub.DataModel;
using NE = C2C.BusinessEntities.NotificationEntities;
using System.Data.Objects;
using System.Collections.Generic;

namespace NotificationHub.Manager
{
    public class ActivityManager
    {
        internal static ActivityManager Instance()
        {
            return new ActivityManager();
        }

        /// <summary>
        /// Sync user log information and his browser information.
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        internal bool SyncUserLog(NE.UserLog userLog)
        {
            int processCount = 0;
            if (userLog != null && userLog.UserId > 0)
            {
                using (var dbContext = Utility.GetDbContext())
                {
                    var log = dbContext.UserLogs.Where(p =>
                        p.UserId == userLog.UserId &&
                        p.LastActivityOn > EntityFunctions.AddMinutes(userLog.LastActivityOn, -20) &&
                        p.Browser == userLog.Browser && p.BrowserVersion == p.BrowserVersion).FirstOrDefault();

                    if (log != null)
                    {
                        log.LastActivityOn = userLog.LastActivityOn;
                    }
                    else
                    {
                        dbContext.UserLogs.Add(
                            new UserLog()
                            {
                                Browser = userLog.Browser,
                                BrowserVersion = userLog.BrowserVersion,
                                IsMobileDevice = userLog.IsMobileDevice,
                                LastActivityOn = userLog.LastActivityOn == DateTime.MinValue ? DateTime.UtcNow : userLog.LastActivityOn,
                                LoggedOn = userLog.LoggedOn == DateTime.MinValue ? DateTime.UtcNow : userLog.LoggedOn,
                                UserId = userLog.UserId,
                                IPAddress = userLog.IPAddress
                            });
                    }

                    processCount = dbContext.SaveChanges();
                }
            }

            return processCount > 0 ? true : false;
        }

        internal NE.OnlineUserStat GetOnlineUserStat()
        {
            NE.OnlineUserStat stat = new NE.OnlineUserStat();

            using (var dbContext = Utility.GetDbContext())
            {
                stat.OnlineUserCount = dbContext.UserLogs.Where(p => p.LastActivityOn > EntityFunctions.AddMinutes(DateTime.UtcNow, -3)).Count();
                stat.TotalUserCount = dbContext.UserLogs.Count();
                stat.OfflineUserCount = stat.TotalUserCount - stat.OnlineUserCount;
            }

            return stat;
        }

        internal List<NE.BrowserStat> GetUserBrowserStat()
        {
            List<NE.BrowserStat> stat = new List<NE.BrowserStat>();
            using (var dbContext = Utility.GetDbContext())
            {
                stat = dbContext.UserLogs.
                    GroupBy(g => new { g.Browser, g.BrowserVersion }, (key, group) =>
                        new NE.BrowserStat
                        {
                            Browser = key.Browser,
                            Version = key.BrowserVersion,
                            Count = group.Count()
                        }).ToList();
            }
            return stat;
        }
    }
}