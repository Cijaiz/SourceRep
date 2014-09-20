using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// User Worker hits the database to do crud operations & retrieve other user specific data 
    /// </summary>
    public class UserWorker
    {
        /// <summary>
        /// Creates an instance for UserWorker class.
        /// </summary>
        /// <returns>an instance of UserWorker</returns>
        public static UserWorker GetInstance()
        {
            return new UserWorker();
        }

        /// <summary>
        /// An entry is inserted into the User Table using the given User entity
        /// </summary>
        /// <param name="newUser">User Entity</param>
        /// <returns>Success/failure response along with the User Object</returns>
        public ProcessResponse<BE.User> Create(BE.User newUser)
        {
            ProcessResponse<BE.User> responce = null;
            using (C2CStoreEntities userStore = RepositoryManager.GetStoreEntity())
            {
                var user = userStore.Users.Where(p => p.UserName == newUser.UserName).FirstOrDefault();
                if (user == null)
                {
                    user = new User()
                    {
                        Password = newUser.Password,
                        PasswordSalt = newUser.PasswordSalt,
                        UserName = newUser.UserName,
                        IsLocked = newUser.IsLocked,
                        Status = (byte)newUser.Status,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = newUser.UpdatedBy,
                        UserProfile = new UserProfile()
                        {
                            CollegeId = newUser.Profile.CollegeId,
                            Status = (byte)newUser.Profile.Status,
                            FirstName = newUser.Profile.FirstName,
                            LastName = newUser.Profile.LastName,
                            Email = newUser.Profile.Email,
                            ProfilePhoto = newUser.Profile.ProfilePhoto,
                            UpdatedBy = newUser.Profile.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow,
                            UserSetting = new UserSetting()
                            {
                                NotificationSetting = (short)newUser.Profile.UserSetting.NotificationSetting,
                                UpdatedBy = newUser.Profile.UserSetting.UpdatedBy,
                                UpdatedOn = DateTime.UtcNow
                            }
                        },
                    };

                    if (newUser.AccountValidity.HasValue) user.AccountValidity = newUser.AccountValidity.Value;

                    userStore.Users.Add(user);

                    if (userStore.SaveChanges() > 0)
                    {
                        newUser.Id = user.Id;
                        newUser.Profile.Id = user.UserProfile.Id;
                        newUser.Profile.UserSetting.Id = user.UserProfile.UserSetting.Id;

                        responce = new ProcessResponse<BE.User>() { Status = ResponseStatus.Success, Message = Message.CREATED, Object = newUser };
                    }
                    else
                    {
                        responce = new ProcessResponse<BE.User>() { Status = ResponseStatus.Failed, Message = Message.FAILED };
                    }
                }
                else
                {
                    responce = new ProcessResponse<BE.User>() { Status = ResponseStatus.Failed, Message = Message.USER_EXSISTS };
                }
            }

            return responce;
        }

        /// <summary>
        /// Creates user and adds user in his college group and adds in default role and updates his/her profile while importing user through chire
        /// </summary>
        /// <param name="newUser">User Model with profile, group and role data</param>
        /// <returns>Success/ Failure Response</returns>
        public ProcessResponse<BE.User> UserImport(BE.User newUser)
        {
            ProcessResponse<BE.User> response = null;

            using (C2CStoreEntities userStore = RepositoryManager.GetStoreEntity())
            {
                var user = userStore.Users.Where(p => p.UserName == newUser.UserName).FirstOrDefault();

                if (user == null)
                {
                    user = new User()
                    {
                        Password = newUser.Password,
                        PasswordSalt = newUser.PasswordSalt,
                        UserName = newUser.UserName,
                        IsLocked = newUser.IsLocked,
                        Status = (int)Status.Active,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = newUser.UpdatedBy
                    };

                    if (newUser.AccountValidity.HasValue) user.AccountValidity = newUser.AccountValidity.Value;

                    userStore.Users.Add(user);

                    //Populating Profile deatil in Data entity
                    userStore.UserProfiles.Add(new UserProfile()
                                                                {
                                                                    Id = user.Id,
                                                                    FirstName = newUser.Profile.FirstName ?? newUser.UserName,
                                                                    LastName = newUser.Profile.LastName ?? string.Empty,
                                                                    UpdatedBy = newUser.UpdatedBy,
                                                                    UpdatedOn = DateTime.UtcNow,
                                                                    Status = (byte)Status.Active
                                                                });

                    if (newUser.Group.Id > 0)
                    {
                        //Adding user in his/her college group
                        userStore.UserGroups.Add(new UserGroup()
                                                                {
                                                                    GroupId = newUser.Group.Id,
                                                                    UserId = user.Id,
                                                                    Status = (byte)Status.Active,
                                                                    CreatedBy = newUser.UpdatedBy,
                                                                    CreatedOn = DateTime.UtcNow
                                                                });
                    }

                    if (newUser.Role.Id > 0)
                    {
                        //Adding role to user
                        userStore.UserRoles.Add(new UserRole()
                                                             {
                                                                 UserId = user.Id,
                                                                 RoleId = newUser.Role.Id,
                                                                 IsDeleted = false,
                                                                 CreatedOn = DateTime.UtcNow,
                                                                 CreatedBy = newUser.UpdatedBy
                                                             });
                    }

                    if (userStore.SaveChanges() > 0)
                    {
                        newUser.Id = user.Id;
                        response = new ProcessResponse<BE.User>() { Status = ResponseStatus.Success, Message = Message.CREATED, Object = newUser };
                    }
                    else
                    {
                        response = new ProcessResponse<BE.User>() { Status = ResponseStatus.Failed, Message = Message.FAILED };
                    }
                }
                else
                {
                    response = new ProcessResponse<BE.User>() { Status = ResponseStatus.Failed, Message = Message.USER_EXSISTS };
                }
            }


            return response;
        }

        /// <summary>
        /// Updates the entity the User Table
        /// </summary>
        /// <param name="newUser">User Entity</param>
        /// <returns>Success/failure response along with the User Object</returns>
        public ProcessResponse Update(BE.User newUser)
        {
            ProcessResponse responce = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var user = dbContext.Users.Where(p => p.UserName == newUser.UserName).FirstOrDefault();

                if (user != null)
                {
                    user.AccountValidity = newUser.AccountValidity;
                    user.IsLocked = newUser.IsLocked;
                    user.LastBadLogon = newUser.LastBadLogon;
                    user.LastLogon = newUser.LastLogon;
                    user.Password = newUser.Password;
                    user.PasswordSalt = newUser.PasswordSalt;
                    user.RetryAttempt = (byte)newUser.RetryAttempt;
                    user.Status = (byte)newUser.Status;
                    user.UpdatedBy = newUser.UpdatedBy;
                    user.UpdatedOn = DateTime.UtcNow;

                    int count = dbContext.SaveChanges();
                    responce = new ProcessResponse() { Status = ResponseStatus.Success, Message = string.Format(Message.UPDATED_COUNT, count) };
                }
                else
                {
                    responce = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
                }
            }

            return responce;
        }

        /// <summary>
        /// Gets the User Entity using the given user name in the User Table
        /// </summary>
        /// <param name="userName">userName</param>
        /// <returns>User Entity</returns>
        public BE.User Get(string userName)
        {
            BE.User user = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var selectedUser = dbContext.Users.Where(p => p.UserName.ToLower() == userName.ToLower() && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (selectedUser != null && selectedUser.Id > 0)
                {
                    user = new BE.User()
                    {
                        Id = selectedUser.Id,
                        UserName = selectedUser.UserName,
                        LastLogon = selectedUser.LastLogon,
                        LastBadLogon = selectedUser.LastBadLogon,
                        IsLocked = selectedUser.IsLocked,
                        Password = selectedUser.Password,
                        PasswordSalt = selectedUser.PasswordSalt,
                        RetryAttempt = selectedUser.RetryAttempt,
                        Status = (Status)selectedUser.Status,
                        UpdatedBy = selectedUser.UpdatedBy.HasValue ? selectedUser.UpdatedBy.Value : selectedUser.CreatedBy,
                        UpdatedOn = selectedUser.UpdatedOn.HasValue ? selectedUser.UpdatedOn.Value : selectedUser.CreatedOn,
                        AccountValidity = selectedUser.AccountValidity
                    };
                }
            }

            return user;
        }

        /// <summary>
        /// Gets the user using the given Id in the User Table
        /// </summary>
        /// <param name="userId">Id of the user to get</param>
        /// <returns>User entity</returns>
        public BE.User Get(int userId)
        {
            BE.User user = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var selectedUser = dbContext.Users.Where(p => p.Id == userId && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (selectedUser != null && selectedUser.Id > 0)
                {
                    user = new BE.User()
                    {
                        Id = selectedUser.Id,
                        UserName = selectedUser.UserName,
                        LastLogon = selectedUser.LastLogon,
                        LastBadLogon = selectedUser.LastBadLogon,
                        IsLocked = selectedUser.IsLocked,
                        Password = selectedUser.Password,
                        PasswordSalt = selectedUser.PasswordSalt,
                        RetryAttempt = selectedUser.RetryAttempt,
                        Status = (Status)selectedUser.Status,
                        UpdatedBy = selectedUser.UpdatedBy.HasValue ? selectedUser.UpdatedBy.Value : selectedUser.CreatedBy,
                        UpdatedOn = selectedUser.UpdatedOn.HasValue ? selectedUser.UpdatedOn.Value : selectedUser.CreatedOn,
                        AccountValidity = selectedUser.AccountValidity.HasValue ? selectedUser.AccountValidity : null
                    };
                }
            }

            return user;
        }

        /// <summary>
        /// Gets the count of records in User Table with the given Status
        /// </summary>
        /// <param name="status">status</param>
        /// <returns>int</returns>
        public int GetCount(Status? status)
        {
            int count = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (status.HasValue)
                {
                    count = dbContext.Users.Count(p => p.Status != (byte)status.Value);
                }
                else
                {
                    count = dbContext.Users.Count();
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the List of user entity with a given status from User Table
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="status">status</param>
        /// <returns>List of user Entity</returns>
        public List<BE.User> Get(Pager pager, Status? status)
        {
            int s = (status == null) ? 10 : (int)status;
            List<BE.User> users = new List<BE.User>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                dynamic selectedUser;
                if (status.HasValue)
                {
                    selectedUser = (from user in dbContext.Users
                                    where user.Status == s
                                    join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                                    from profile in userprofile.DefaultIfEmpty()

                                    select new { user.Id, user.IsLocked, user.AccountValidity, user.CreatedBy, user.CreatedOn, user.LastBadLogon, user.LastLogon, user.RetryAttempt, user.Status, user.UpdatedBy, user.UpdatedOn, user.UserName, profile.CollegeId, profile.FirstName, profile.LastName, profile.ProfilePhoto })
                                    .OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                }
                else
                {

                    selectedUser = (from user in dbContext.Users
                                    join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                                    from profile in userprofile.DefaultIfEmpty()
                                    select new { user.Id, user.IsLocked, user.AccountValidity, user.CreatedBy, user.CreatedOn, user.LastBadLogon, user.LastLogon, user.RetryAttempt, user.Status, user.UpdatedBy, user.UpdatedOn, user.UserName, profile.CollegeId, profile.FirstName, profile.LastName, profile.ProfilePhoto })
                                    .OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();


                }
                if (selectedUser != null)
                {
                    foreach (var item in selectedUser)
                    {
                        BE.User user = new BE.User();
                        user.Id = item.Id;
                        user.UserName = item.UserName;
                        user.RetryAttempt = item.RetryAttempt;

                        user.IsLocked = item.IsLocked;
                        user.LastLogon = item.LastLogon;
                        user.LastBadLogon = item.LastBadLogon;

                        user.AccountValidity = item.AccountValidity;
                        user.Status = (Status)item.Status;
                        user.Profile.FirstName = (item.FirstName != null) ? item.FirstName : null;

                        user.Profile.LastName = (item.LastName != null) ? item.LastName : null;
                        user.Profile.CollegeId = (item.CollegeId != null) ? item.CollegeId : null;
                        user.Profile.ProfilePhoto = string.IsNullOrEmpty(item.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL
                                                                                            : StorageHelper.GetMediaFilePath(item.ProfilePhoto, StorageHelper.COMMENT_SIZE);   
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Gets the List of Users with the given Status and name from User Table
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="name">Name of the user</param>
        /// <param name="status">status</param>
        /// <returns>List of user Entity</returns>
        public List<BE.User> Get(Pager pager, string name, Status? status)
        {
            int s = (status == null) ? 10 : (int)status;
            List<BE.User> users = new List<BE.User>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                dynamic selectedUser;
                if (status.HasValue)
                {
                    selectedUser = (from user in dbContext.Users
                                    where user.Status == s && (user.UserName.Contains(name) || user.UserProfile.FirstName.Contains(name) || user.UserProfile.LastName.Contains(name))
                                    join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                                    from profile in userprofile.DefaultIfEmpty()
                                    select new { user.Id, user.IsLocked, user.AccountValidity, user.CreatedBy, user.CreatedOn, user.LastBadLogon, user.LastLogon, user.RetryAttempt, user.Status, user.UpdatedBy, user.UpdatedOn, user.UserName, profile.CollegeId, profile.FirstName, profile.LastName, profile.ProfilePhoto })
                                    .OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                }
                else
                {
                    selectedUser = (from user in dbContext.Users
                                    where user.UserName.Contains(name) || user.UserProfile.FirstName.Contains(name) || user.UserProfile.LastName.Contains(name)
                                    join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                                    from profile in userprofile.DefaultIfEmpty()
                                    select new { user.Id, user.IsLocked, user.AccountValidity, user.CreatedBy, user.CreatedOn, user.LastBadLogon, user.LastLogon, user.RetryAttempt, user.Status, user.UpdatedBy, user.UpdatedOn, user.UserName, profile.CollegeId, profile.FirstName, profile.LastName, profile.ProfilePhoto })
                                    .OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();

                }

                if (selectedUser != null)
                {
                    foreach (var item in selectedUser)
                    {
                        BE.User user = new BE.User();
                        user.Id = item.Id;
                        user.UserName = item.UserName;
                        user.RetryAttempt = item.RetryAttempt;

                        user.IsLocked = item.IsLocked;
                        user.LastLogon = item.LastLogon;
                        user.LastBadLogon = item.LastBadLogon;

                        user.AccountValidity = item.AccountValidity;
                        user.Status = (Status)item.Status;
                        user.Profile.FirstName = (item.FirstName != null) ? item.FirstName : null;

                        user.Profile.LastName = (item.LastName != null) ? item.LastName : null;
                        user.Profile.CollegeId = (item.CollegeId != null) ? item.CollegeId : null;
                        user.Profile.ProfilePhoto = string.IsNullOrEmpty(item.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL
                                                                                            : StorageHelper.GetMediaFilePath(item.ProfilePhoto, StorageHelper.COMMENT_SIZE);  
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Gets the count of user with particular status along with pagination from User Table
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="status">status</param>
        /// <returns>int</returns>
        public int GetCount(Pager pager, Status? status)
        {
            int s = (status == null) ? 10 : (int)status;
            int count;

            List<BE.User> users = new List<BE.User>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (status.HasValue)
                {
                    count = (from user in dbContext.Users
                             where user.Status == s
                             join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                             from profile in userprofile.DefaultIfEmpty()
                             select user).Count();
                }
                else
                {

                    count = (from user in dbContext.Users
                             join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                             from profile in userprofile.DefaultIfEmpty()
                             select user).Count();
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the count of user with particular status and name from User Table along with Pagination
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="name">name</param>
        /// <param name="status">status</param>
        /// <returns>int</returns>
        public int GetCount(Pager pager, string name, Status? status)
        {
            int s = (status == null) ? 10 : (int)status;
            int count;
            List<BE.User> users = new List<BE.User>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (status.HasValue)
                {
                    count = (from user in dbContext.Users
                             where user.Status == s && (user.UserName.Contains(name) || user.UserProfile.FirstName.Contains(name) || user.UserProfile.LastName.Contains(name))
                             join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                             from profile in userprofile.DefaultIfEmpty()
                             select user).Count();
                }
                else
                {
                    count = (from user in dbContext.Users
                             where user.UserName.Contains(name) || user.UserProfile.FirstName.Contains(name) || user.UserProfile.LastName.Contains(name)
                             join profile in dbContext.UserProfiles on user.Id equals profile.Id into userprofile
                             from profile in userprofile.DefaultIfEmpty()
                             select user).Count();

                }
            }
            return count;
        }
    }
}
