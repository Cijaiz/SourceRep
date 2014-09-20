using NotificationHub.DataModel;
using System;
using System.Linq;
using NE = C2C.BusinessEntities.NotificationEntities;

namespace NotificationHub.Manager
{
    /// <summary>
    /// This Class provide Methods related to user and his group management.
    /// </summary>
    public class UserManager
    {
        internal static UserManager Instance()
        {
            return new UserManager();
        }

        /// <summary>
        /// This method is used to add members to specific group
        /// </summary>
        /// <param name="groupMember">Group and Member information.</param>
        /// <returns>On adding member to group: True / Otherwise: false (even is member already exist in the group).</returns>
        internal bool AddUserToGroup(NE.GroupMember groupMember)
        {
            int processCount = 0;
            if (groupMember != null && groupMember.GroupId > 0 && groupMember.UserId > 0)
            {
                using (var dbContext = Utility.GetDbContext())
                {
                    int count = dbContext.UserGroups.Count(p => p.UserId == groupMember.UserId && p.GroupId == groupMember.GroupId);
                    if (count < 1)
                    {
                        dbContext.UserGroups.Add(new UserGroup() { GroupId = groupMember.GroupId, UserId = groupMember.UserId, CreatedOn = DateTime.UtcNow });

                        processCount = dbContext.SaveChanges();
                    }
                }
            }

            return processCount > 0 ? true : false;
        }

        /// <summary>
        /// This method is used to remove user from a group.
        /// </summary>
        /// <param name="groupMember">Group and Member information.</param>
        /// <returns>On removing member the group :True / Otherwise : False.</returns>
        internal bool RemoveUserFromGroup(NE.GroupMember groupMember)
        {
            int processCount = 0;
            if (groupMember != null && groupMember.GroupId > 0 && groupMember.UserId > 0)
            {
                using (var dbContext = Utility.GetDbContext())
                {
                    var groupUser = dbContext.UserGroups.FirstOrDefault(p => p.UserId == groupMember.UserId && p.GroupId == groupMember.GroupId);
                    if (groupUser != null && groupUser.Id > 0)
                    {
                        dbContext.UserGroups.Remove(groupUser);

                        processCount = dbContext.SaveChanges();
                    }
                }
            }

            return processCount > 0 ? true : false;
        }

        /// <summary>
        /// This method is used to create or update user detail.
        /// </summary>
        /// <param name="userDetail">User information.</param>
        /// <returns>On Updating/Creating it returns : True / Otherwise :False.</returns>
        internal bool UpdateUserDetail(NE.UserDetail userDetail)
        {
            int processCount = 0;

            using (var dbContext = Utility.GetDbContext())
            {
                var userProfile = dbContext.UserDetails.FirstOrDefault(p => p.UserId == userDetail.UserId);
                //User already exists. So update his new information
                if (userProfile != null)
                {
                    userProfile.DisplayName = !string.IsNullOrEmpty(userDetail.DisplayName) ? userDetail.DisplayName : userProfile.DisplayName;
                    userProfile.Email = !string.IsNullOrEmpty(userDetail.Email) ? userDetail.Email : userProfile.Email;
                    userProfile.PrivacyStatus = (int)userDetail.NotificationSetting;

                    processCount = dbContext.SaveChanges();
                }
                //User not exists. So Create user.
                else
                {
                    dbContext.UserDetails.Add(
                        new UserDetail()
                        {
                            DisplayName = userDetail.DisplayName,
                            Email = userDetail.Email,
                            PrivacyStatus = (int)userDetail.NotificationSetting,
                            UserId = userDetail.UserId
                        });

                    processCount = dbContext.SaveChanges();
                }
            }

            return processCount > 0 ? true : false;
        }
    }
}