using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Logger;
using C2C.Core.Extensions;
using NotificationHub.Manager;
using System;
using System.Web.Http;
using System.Collections.Generic;

namespace NotificationHub.Controllers
{
    public class ActivityLogController : ApiController
    {
        /// <summary>
        /// Sync User log and browser information.
        /// </summary>
        /// <param name="userLog">user log and browser information.</param>
        /// <returns>Process response</returns>
        [HttpPost]
        public ProcessResponse SyncUserLog(UserLog userLog)
        {
            ProcessResponse responce = null;
            try
            {
                if (ActivityManager.Instance().SyncUserLog(userLog))
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

        [HttpGet]
        public ProcessResponse<OnlineUserStat> ActiveUserStat()
        {
            ProcessResponse<OnlineUserStat> responce = null;
            try
            {
                var stat = ActivityManager.Instance().GetOnlineUserStat();

                responce = new ProcessResponse<OnlineUserStat>() { Status = ResponseStatus.Success, Object = stat };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                responce = new ProcessResponse<OnlineUserStat>() { Status = ResponseStatus.Error, Message = "Internal server error." };
            }
            return responce;
        }

        [HttpGet]
        public ProcessResponse<List<BrowserStat>> UserBrowserStat()
        {
            ProcessResponse<List<BrowserStat>> responce = null;
            try
            {
                var stat = ActivityManager.Instance().GetUserBrowserStat();

                responce = new ProcessResponse<List<BrowserStat>>() { Status = ResponseStatus.Success, Object = stat };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
                responce = new ProcessResponse<List<BrowserStat>>() { Status = ResponseStatus.Error, Message = "Internal server error." };
            }
            return responce;
        }
    }
}
