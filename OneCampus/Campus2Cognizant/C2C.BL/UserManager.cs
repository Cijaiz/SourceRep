using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// UserManager calls the user worker to do user specific operations
    /// </summary>

    public static class UserManager
    {
        /// <summary>
        /// Activates the given user by calling the Data Access layer
        /// </summary>
        /// <param name="id">User Id to activate</param>
        public static void Activate(int id)
        {
            User user = DAL.UserWorker.GetInstance().Get(id);
            user.Status = Status.Active;
            DAL.UserWorker.GetInstance().Update(user);
        }

        /// <summary>
        /// Creates the user using the user entity by calling the Data Access layer
        /// </summary>
        /// <param name="user">User Entity</param>
        /// <returns>Success/Failure Response with user profile detail</returns>
        public static ProcessResponse<User> Create(User user)
        {
            Guard.IsNotBlank(user.UserName,"UserName");
            //Guard.IsNotBlank(user.Profile.FirstName, "FirstName");
            //Guard.IsNotBlank(user.Profile.LastName, "LastName");
            //Guard.IsNotBlank(user.Profile.Email, "EmailAddress");

            ProcessResponse<User> responce = null;
            user.PasswordSalt = CryptoHelper.GenerateSalt();
            user.Password = CryptoHelper.HashData(user.PasswordSalt, user.Password);
            user.Status = Status.Active;

            if (user.Profile == null)
            {
                user.Profile = new UserProfile();
            }

            //if (string.IsNullOrEmpty(user.Profile.FirstName)) user.Profile.FirstName = user.UserName;
            user.Profile.Status = user.Status;
            user.Profile.UpdatedBy = user.UpdatedBy;

            if (user.Profile.UserSetting == null)
            {
                user.Profile.UserSetting = new UserSetting() { NotificationSetting = NotificationSetting.WithinCollege };
            }

            user.Profile.UserSetting.UpdatedBy = user.UpdatedBy;
            
            responce = DAL.UserWorker.GetInstance().Create(user);

            return responce;
        }

        /// <summary>
        /// Creates the UserProfile by calling the Data Access layer
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success/Failure Response with user profile detail</returns>
        public static ProcessResponse<UserProfile> CreateUserProfile(User user)
        {
            var response = DAL.ProfileWorker.GetInstance().Create(user);
            return response;
        }

        /// <summary>
        /// Deactivates the given user by calling the Data Access layer
        /// </summary>
        /// <param name="id">User Id to deactivate</param>
        public static void Deactivate(int id)
        {
            User user = DAL.UserWorker.GetInstance().Get(id);
            user.Status = Status.InActive;
            DAL.UserWorker.GetInstance().Update(user);
        }

        /// <summary>
        /// Deletes the given user by calling the Data Access layer
        /// </summary>
        /// <param name="id">User Id to delete</param>
        public static void Delete(int id)
        {
            User user = DAL.UserWorker.GetInstance().Get(id);
            user.Status = Status.Deleted;
            DAL.UserWorker.GetInstance().Update(user);
        }

        /// <summary>
        /// Gets the User Entity when User Id is given,by calling the Data Access layer
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Entity</returns>
        public static User Get(int id)
        {
            User user = null;
            if (id >= 0)
            {
                user = DAL.UserWorker.GetInstance().Get(id);
            }

            return user;
        }

        /// <summary>
        /// Gets lists of user depending on the status, by calling the Data Access layer.
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="status">Status of the user</param>
        /// <returns>List of user entity</returns>
        public static List<User> Get(Pager pager, Status? status = null)
        {
            return DAL.UserWorker.GetInstance().Get(pager, status);
        }

        /// <summary>
        /// Gets lists of user depending on the status,name by calling the Data Access layer.
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="name">Name of the user</param>
        /// <param name="status">Status of the user</param>
        /// <returns>List of user entity</returns>
        public static List<User> Get(Pager pager, string name, Status? status)
        {
            return DAL.UserWorker.GetInstance().Get(pager, name, status);
        }

        /// <summary>
        /// Gets the count of user based on status by calling the  Data Access layer.
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="status">Status of the user</param>
        /// <returns>int</returns>
        public static int GetCount(Pager pager, Status? status = null)
        {
            return DAL.UserWorker.GetInstance().GetCount(pager, status);
        }

        /// <summary>
        /// Gets the count of user based on status and name by calling the  Data Access layer.
        /// </summary>
        /// <param name="pager">Pager parameters for Pagination</param>
        /// <param name="name">Name of the user</param>
        /// <param name="status">Status of the user</param>
        /// <returns>int</returns>
        public static int GetCount(Pager pager, string name, Status? status)
        {
            return DAL.UserWorker.GetInstance().GetCount(pager, name, status);
        }

        /// <summary>
        /// Gets the UserProfile Detail using UserId by calling the  Data Access layer.
        /// </summary>
        /// <param name="id">Id of the user to get Profile</param>
        /// <returns>UserProfile Entity</returns>
        public static UserProfile GetUserProfile(int id)
        {
            Guard.IsNotZero(id, "User Id");

            var profile = DAL.ProfileWorker.GetInstance().Get(id);
            return profile;
        }

        /// <summary>
        /// Gets the Email ids of the admin users by calling the  Data Access layer.
        /// </summary>
        /// <returns>Admin email ids as list of string</returns>
        public static List<string> GetAdminEmailIds()
        {
            List<string> emailIds = DAL.ProfileWorker.GetInstance().GetAdminEmailIds();
            return emailIds;
        }

        /// <summary>
        /// Gets the count of user based on the given status by calling the  Data Access layer.
        /// </summary>
        /// <param name="status">Status of the user</param>
        /// <returns>int</returns>
        public static int GetCount(Status? status = null)
        {
            return DAL.UserWorker.GetInstance().GetCount(status);
        }

        /// <summary>
        /// Gets the Notification setting using userId
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Setting</returns>
        public static UserSetting GetNotificationSetting(int id)
        {
            Guard.IsNotZero(id, "User Id");

            var userSetting = DAL.ProfileWorker.GetInstance().GetNotificationSetting(id);
            return userSetting;
        }

        /// <summary>
        /// Updates the user Entity  by calling the  Data Access layer.
        /// </summary>
        /// <param name="user">User Entity</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse Update(User user)
        {
            ProcessResponse responce = null;
            responce = DAL.UserWorker.GetInstance().Update(user);
            return responce;
        }

        /// <summary>
        /// Updates the user Profile Detail
        /// </summary>
        /// <param name="userProfile">Updated User Profile</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse<UserProfile> Update(UserProfile userProfile)
        {
            Guard.IsNotNull(userProfile, "User Profile");
            Guard.IsNotBlank(userProfile.FirstName, "First Name");
            Guard.IsNotBlank(userProfile.LastName, "Last Name");
            Guard.IsNotZero(userProfile.Id, "User Id");

            var response = DAL.ProfileWorker.GetInstance().Update(userProfile);
            return response;
        }

        /// <summary>
        /// Creates/Updates the user Notification setting
        /// </summary>
        /// <param name="setting">User Setting</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse UpdateNotificationSetting(UserSetting setting)
        {
            Guard.IsNotNull(setting, "User Settings");
            Guard.IsNotZero(setting.Id, "User Id");
            Guard.IsNotZero(setting.NotificationSettingId, "NotificationSetting");

            var notificationSetting = DAL.ProfileWorker.GetInstance().UpdateNotificationSetting(setting);
            return notificationSetting;
        }

        /// <summary>
        /// Gets the user Entity based on the username
        /// </summary>
        /// <param name="userName">userName</param>
        /// <returns>User Entity</returns>
        public static User Get(string userName)
        {
            var user = DAL.UserWorker.GetInstance().Get(userName);
            return user;
        }

        /// <summary>
        /// User Import Through CHire. Updates the profile, user, usergroup and userrole Model
        /// </summary>
        /// <param name="user">user Model with profile, group and role date</param>
        /// <returns>Success/Failure response</returns>
        public static ProcessResponse<User> UserImport(User user)
        {
            Guard.IsNotNull(user, "User");

            //returns the existing groupId else creates new group and returns the created groupId
            var groupId = DAL.GroupWorker.GetInstance().GetGroupId(user.Group);
            user.Group.Id = groupId;

            //Get Role by RoleName
            var role = DAL.RoleWorker.GetInstance().GetRoleByName(DefaultValue.GENERAL_ROLE);
            user.Role.Id = role.Id;
            var userResponse = DAL.UserWorker.GetInstance().UserImport(user);

            return userResponse;
        }
    }
}