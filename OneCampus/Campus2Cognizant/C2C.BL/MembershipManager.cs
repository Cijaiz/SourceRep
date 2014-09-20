namespace C2C.BusinessLogic
{
    #region Reference
    using C2C.BusinessEntities;
    using C2C.BusinessEntities.C2CEntities;
    using C2C.Core.Constants.C2CWeb;
    using C2C.Core.Helper;
    using C2C.Core.Security;
    using C2C.Core.Security.Structure;
    using C2C.DataAccessLogic;
    using System;
    using System.Linq;
    #endregion

    /// <summary>
    /// Membership manager provides all the 
    /// </summary>
    public static class MembershipManager
    {
        /// <summary>
        /// This method validate the user credential and authenticate.
        /// </summary>
        /// <param name="login">user credential</param>
        /// <returns>Response the user status and his UserContext information.</returns>
        public static ProcessResponse<UserContext> IsAuthenticated(Login login)
        {
            ProcessResponse<UserContext> response = null;
            var userWorker = UserWorker.GetInstance();
            var user = userWorker.Get(login.UserName);

            /// Check is valid username
            if (user != null)
            {
                /// Check his account status for active

                if ((Status)user.Status == Status.Active)
                {
                    /// Check for his valid password
                    if (CryptoHelper.HashData(user.PasswordSalt, login.Password) == user.Password)
                    {
                        /// Check his account is not locked
                        if (!user.IsLocked)
                        {
                            DateTime? lastLoggedOn = user.LastLogon;
                            user.LastLogon = DateTime.UtcNow;
                            user.RetryAttempt = 0;
                            var res = userWorker.Update(user);
                            if (res.Status == ResponseStatus.Success)
                            {
                                var profile = ProfileWorker.GetInstance().Get(user.Id);
                                UserContext context = new UserContext()
                                {
                                    FirstName = profile.FirstName,
                                    LastName = profile.LastName,
                                    PhotoPath = profile.ProfilePhoto,
                                    UserId = user.Id,
                                    LastLogon = lastLoggedOn,
                                    Permissions = RoleManager.GetPermissionsforUser(profile.Id).Select(a => a.Id).ToList()
                                };

                                response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Success, Message = Message.MEMBERSHIP_VALID_USER, Object = context };
                            }
                            else
                            {
                                response = new ProcessResponse<UserContext>() { Status = res.Status, Message = res.Message };
                            }
                        }
                        else
                        {
                            // Note following code is added update for locked user.
                            //user.RetryAttempt = (byte)(user.RetryAttempt + 1);
                            //user.LastBadLogon = DateTime.UtcNow;
                            //var res = UserWorker.GetInstance().Update(user);

                            response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Failed, Message = Message.MEMBERSHIP_ACCOUNT_LOCKED };
                        }
                    }
                    else
                    {
                        user.RetryAttempt = (byte)(user.RetryAttempt + 1);
                        user.LastBadLogon = DateTime.UtcNow;
                        if (user.RetryAttempt >= DefaultValue.MAX_LOGIN_RETRY_ATTEMPT)
                        {
                            user.IsLocked = true;
                        }

                        var res = UserWorker.GetInstance().Update(user);

                        response = new ProcessResponse<UserContext>()
                        {
                            Status = ResponseStatus.Failed,
                            Message = user.IsLocked ? Message.MEMBERSHIP_ACCOUNT_LOCKED : Message.MEMBERSHIP_INVALID_CREDENTIALS
                        };
                    }
                }
                else
                {
                    response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Failed, Message = Message.MEMBERSHIP_ACCOUNT_NOT_ACTIVE };
                }
            }
            else
            {
                response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Failed, Message = Message.MEMBERSHIP_INVALID_CREDENTIALS };
            }

            return response;
        }

        /// <summary>
        /// Builds the UserContext based on the UserName..
        /// </summary>
        /// <param name="userName">Username to be Searched in the database.</param>
        /// <returns>Process Response which returns UserContext retrived from the DB.</returns>
        public static ProcessResponse<UserContext> GetUserContext(string userName)
        {
            Guard.IsNotBlank(userName, "UserName");

            ProcessResponse<UserContext> response = null;
            var userWorker = UserWorker.GetInstance();
            var user = userWorker.Get(userName);

            if (user == null)
                return response;

            if ((Status)user.Status == Status.Active)
            {
                if (!user.IsLocked)
                {
                    DateTime? lastLoggedOn = user.LastLogon;
                    user.LastLogon = DateTime.UtcNow;
                    user.RetryAttempt = 0;
                    var res = userWorker.Update(user);
                    if (res.Status == ResponseStatus.Success)
                    {
                        var profile = ProfileWorker.GetInstance().Get(user.Id);
                        UserContext context = new UserContext()
                        {
                            FirstName = profile.FirstName,
                            LastName = profile.LastName,
                            UserName = user.UserName,

                            UserId = user.Id,
                            LastLogon = lastLoggedOn,
                            PhotoPath = profile.ProfilePhoto,

                            Permissions = RoleManager.GetPermissionsforUser(profile.Id).Select(a => a.Id).ToList()
                        };

                        response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Success, Message = Message.MEMBERSHIP_VALID_USER, Object = context };
                    }
                    else
                    {
                        response = new ProcessResponse<UserContext>() { Status = res.Status, Message = res.Message };
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Builds the user context from the user and profile Object.
        /// </summary>
        /// <param name="user">User entity for which the context needs to be builded.</param>
        /// <param name="userProfile">Profile Entity for which the context needs to be builded.</param>
        /// <returns>Process Response which returns Freshly builded User Context.</returns>
        public static ProcessResponse<UserContext> BuildUserContext(User user, UserProfile userProfile)
        {
            Guard.IsNotNull(user, "user");
            Guard.IsNotNull(userProfile, "UserProfile");

            ProcessResponse<UserContext> response = null;
            UserContext userContext = new UserContext()
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                UserName = user.UserName,

                UserId = user.Id,
                LastLogon = user.LastLogon,
                PhotoPath = userProfile.ProfilePhoto,

                Permissions = RoleManager.GetPermissionsforUser(userProfile.Id).Select(a => a.Id).ToList()
            };
            response = new ProcessResponse<UserContext>() { Status = ResponseStatus.Success, Message = Message.MEMBERSHIP_VALID_USER, Object = userContext };

            return response;
        }

        /// <summary>
        /// This method is used to change user's credentials.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>Response the status</returns>
        public static ProcessResponse ChangePassword(string userName, string oldPassword, string newPassword)
        {
            ProcessResponse response = null;
            var user = UserWorker.GetInstance().Get(userName);
            /// Check is valid username
            if (user != null)
            {
                if (CryptoHelper.HashData(user.PasswordSalt, oldPassword) == user.Password)
                {
                    user.Password = CryptoHelper.HashData(user.PasswordSalt, newPassword);
                    var res = UserWorker.GetInstance().Update(user);
                    response = new ProcessResponse() { Status = ResponseStatus.Success, Message = Message.MEMBERSHIP_CHANGE_PASSWORD_SUCCESS };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.MEMBERSHIP_INVALID_CREDENTIALS };
                }
            }
            else
            {
                response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
            }
            return response;
        }
    }
}
