using C2C.BusinessEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Logger;
using C2C.Core.Extensions;
using NotificationHub.Manager;
using System;
using System.Web.Http;

namespace NotificationHub.Controllers
{
    public class UserController : ApiController
    {
        /// <summary>
        /// This action is used to create or update user detail.
        /// </summary>
        /// <param name="userDetail">User information.</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse UpdateProfile(UserDetail userDetail)
        {
            ProcessResponse responce = null;
            try
            {
                if (UserManager.Instance().UpdateUserDetail(userDetail))
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
        /// This action is used to add members to specific group
        /// </summary>
        /// <param name="groupMember">Group and Member information.</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse AddToGroup(GroupMember groupMember)
        {
            ProcessResponse responce = null;
            try
            {
                if (UserManager.Instance().AddUserToGroup(groupMember))
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
        /// This action is used to remove user from a group.
        /// </summary>
        /// <param name="groupMember">Group and Member information.</param>
        /// <returns>ProcessResponse</returns>
        [HttpPost]
        public ProcessResponse RemoveFromGroup(GroupMember groupMember)
        {
            ProcessResponse responce = null;
            try
            {
                if (UserManager.Instance().RemoveUserFromGroup(groupMember))
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
    }
}
