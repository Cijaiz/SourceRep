using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on UserProfile Entity Libraries.
    /// </summary>
    public class ProfileWorker
    {
        /// <summary>
        /// Creates a new instance for ProfileWorker
        /// </summary>
        /// <returns>Newly created Instance</returns>
        public static ProfileWorker GetInstance()
        {
            return new ProfileWorker();
        }

        /// <summary>
        /// Creates a UserProfile while creating user
        /// </summary>
        /// <param name="user">Business Entity UserModel </param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse<BE.UserProfile> Create(BE.User user)
        {
            ProcessResponse<BE.UserProfile> response = null;
            using (C2CStoreEntities profileStore = RepositoryManager.GetStoreEntity())
            {
                var userProfile = new UserProfile()
                {
                    Id = user.Id,
                    FirstName = user.UserName,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = user.UpdatedBy,
                    Status = (byte)Status.Active
                };

                profileStore.UserProfiles.Add(userProfile);

                if (profileStore.SaveChanges() > 0)
                {
                    BE.UserProfile newUserProfile = new BE.UserProfile();
                    newUserProfile.Id = userProfile.Id;

                    response = new ProcessResponse<BE.UserProfile>()
                                                                        {
                                                                            Status = ResponseStatus.Success,
                                                                            Message = Message.CREATED,
                                                                            Object = newUserProfile
                                                                        };
                }
                else
                {
                    response = new ProcessResponse<BE.UserProfile>()
                                                                        {
                                                                            Status = ResponseStatus.Failed,
                                                                            Message = Message.FAILED
                                                                        };
                }
            }

            return response;
        }

        /// <summary>
        /// Updates the UserProfile using  userId
        /// </summary>
        /// <param name="profile">Updated user Profile</param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse<BE.UserProfile> Update(BE.UserProfile profile)
        {
            ProcessResponse<BE.UserProfile> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var userProfile = dbContext.UserProfiles
                                           .Where(a => a.Id == profile.Id)
                                           .FirstOrDefault();

                if (userProfile != null && userProfile.Id > 0)
                {
                    if (userProfile.CollegeId.HasValue)
                    {
                        if (profile.CollegeId.HasValue)
                        {
                            var userGroup = userProfile.UserGroups
                                                       .Where(p => p.GroupId == userProfile.CollegeId && p.Status == (byte)Status.Active)
                                                       .FirstOrDefault();

                            if (userGroup.GroupId != profile.CollegeId)
                            {
                                userGroup.Status = (byte)Status.Deleted;
                                userGroup.UpdatedBy = profile.UpdatedBy;
                                userGroup.UpdatedOn = DateTime.UtcNow;

                                userProfile.CollegeId = profile.CollegeId;
                                AddUserInGroup(userProfile);
                            }
                        }
                    }
                    else if(profile.CollegeId.HasValue)
                    {
                        userProfile.CollegeId = profile.CollegeId;
                        AddUserInGroup(userProfile);
                    }

                    //populating data entity using business entity
                    userProfile.CollegeId = profile.CollegeId;

                    userProfile.Email = profile.Email;

                    userProfile.FirstName = profile.FirstName;
                    userProfile.LastName = profile.LastName;

                    if ((!string.IsNullOrEmpty(profile.ProfilePhoto) || !string.IsNullOrWhiteSpace(profile.ProfilePhoto)))
                        userProfile.ProfilePhoto = profile.ProfilePhoto;
                    else if ((!string.IsNullOrEmpty(userProfile.ProfilePhoto) || !string.IsNullOrWhiteSpace(userProfile.ProfilePhoto)))
                        profile.ProfilePhoto = userProfile.ProfilePhoto;

                    userProfile.UpdatedOn = DateTime.UtcNow;
                    userProfile.UpdatedBy = profile.UpdatedBy;

                    int count = dbContext.SaveChanges();

                    response = new ProcessResponse<BE.UserProfile>()
                                                    {
                                                        Status = ResponseStatus.Success,
                                                        Message = Message.UPDATED,
                                                        Object = profile
                                                    };
                }
                else
                {
                    response = new ProcessResponse<BE.UserProfile>()
                                                    {
                                                        Status = ResponseStatus.Failed,
                                                        Message = Message.RECORED_NOT_FOUND
                                                    };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the User profile Detail using userId
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Profile</returns>
        public BE.UserProfile Get(int id)
        {
            BE.UserProfile profile = new BE.UserProfile();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var userProfile = dbContext.UserProfiles
                                           .Where(p => p.Id == id)
                                           .FirstOrDefault();

                if (userProfile != null && userProfile.Id > 0)
                {
                    //populating business entity from data entity
                    profile.Id = userProfile.Id;
                    profile.FirstName = userProfile.FirstName;
                    profile.LastName = userProfile.LastName;
                    profile.Email = userProfile.Email;
                    profile.CollegeId = userProfile.CollegeId;
                    profile.ProfilePhoto = userProfile.ProfilePhoto;
                    profile.UpdatedBy = userProfile.UpdatedBy;
                    profile.UpdatedOn = userProfile.UpdatedOn;
                }
            }

            return profile;
        }

        /// <summary>
        /// Gets the Notification setting using userId
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User setting</returns>
        public BE.UserSetting GetNotificationSetting(int id)
        {
            BE.UserSetting userSetting = new BE.UserSetting();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var settings = dbContext.UserSettings
                                        .Where(c => c.Id == id)
                                        .FirstOrDefault();

                //populating business entity from data tentity
                if (settings != null && settings.Id > 0)
                {
                    userSetting.Id = settings.Id;
                    userSetting.NotificationSetting = (BE.NotificationSetting)settings.NotificationSetting;
                    userSetting.UpdatedBy = settings.UpdatedBy;
                    userSetting.UpdatedOn = settings.UpdatedOn;
                }
            }
            return userSetting;
        }

        /// <summary>
        /// Creates/Updates the user Notification setting 
        /// </summary>
        /// <param name="userSetting">User Notification setting</param>
        /// <returns>Success/ Failure response</returns>
        public ProcessResponse UpdateNotificationSetting(BE.UserSetting userSetting)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var settings = dbContext.UserSettings
                                        .Where(c => c.Id == userSetting.Id)
                                        .FirstOrDefault();

                //populating data entity from business tentity incase of update
                if (settings != null && settings.Id > 0)
                {
                    settings.NotificationSetting = userSetting.NotificationSettingId;
                    settings.UpdatedBy = userSetting.UpdatedBy;
                    settings.UpdatedOn = DateTime.UtcNow;
                }
                else
                {
                    //populating data entity from business tentity incase of create
                    UserSetting user = new UserSetting();
                    user.Id = userSetting.Id;
                    user.NotificationSetting = userSetting.NotificationSettingId;
                    user.UpdatedBy = userSetting.UpdatedBy;
                    user.UpdatedOn = DateTime.UtcNow;
                    dbContext.UserSettings.Add(user);
                }

                if (dbContext.SaveChanges() > 0)
                {
                    response = new ProcessResponse()
                                                        {
                                                            Status = ResponseStatus.Success,
                                                            Message = Message.UPDATED
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

        public List<string> GetAdminEmailIds()
        {
            List<string> emailIds = new List<string>();
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                emailIds = dbContext.UserRoles.Where(p => p.RoleId == 1).Select(q => q.UserProfile.Email).ToList();
            }
            return emailIds;
        }

        private void AddUserInGroup(UserProfile profile)
        {
            UserGroup userGroup =  new UserGroup()
                                                {
                                                    GroupId = profile.CollegeId.Value,
                                                    UserId = profile.Id,
                                                    Status = (byte)Status.Active,
                                                    CreatedOn = DateTime.UtcNow,
                                                    CreatedBy = profile.UpdatedBy
                                                };
            profile.UserGroups.Add(userGroup);
            //return userGroup;
        }
    }
}
