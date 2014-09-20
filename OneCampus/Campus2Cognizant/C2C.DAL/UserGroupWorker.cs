#region References
using C2C.BusinessEntities;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
#endregion

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on UserGroup Entity Libraries.
    /// </summary>
    public class UserGroupWorker
    {
        /// <summary>
        /// Creates a new instance for UserGroupWorker class
        /// </summary>
        /// <returns>Returns the new instance</returns>
        public static UserGroupWorker GetInstance()
        {
            return new UserGroupWorker();
        }

        /// <summary>
        /// Gets the list of groups for the particular user.
        /// </summary>
        /// <param name="userId">User id for which the list of groups to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <returns>List of groups.</returns>
        public List<BE.UserGroup> Get(int userId)
        {
            List<BE.UserGroup> userGroupList = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groups = dbContext.UserGroups.OrderBy(o => o.Id).Where(g => g.UserId == userId && g.Status == (byte)Status.Active).ToList();

                if (groups != null && groups.Count() > 0)
                {
                    userGroupList = new List<BE.UserGroup>();
                    groups.ForEach(p => userGroupList.Add(
                        new BE.UserGroup()
                        {
                            Id = p.Id,
                            GroupId = p.GroupId,
                            UserId = p.UserId,

                            Status = (Status)p.Status,
                            UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                            UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                        }));
                }
            }

            return userGroupList;
        }

        /// <summary>
        /// Gets the list of groups for the user.
        /// </summary>
        /// <param name="userId">UserId for which the group list will be given.</param>
        /// <param name="pager">Pager.</param>
        /// <returns>List of groups.</returns>
        public List<BE.Group> GetUserGroups(int userId, Pager pager)
        {
            List<BE.Group> userGroupList = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groups = dbContext.Groups.Where(g => g.UserGroups.Where(a => a.UserId == userId && a.Status == (byte)Status.Active).Count() > 0).ToList();

                if (groups != null && groups.Count() > 0)
                {
                    userGroupList = new List<BE.Group>();
                    groups.ForEach(p => userGroupList.Add(
                        new BE.Group()
                        {
                            Id = p.Id,
                            IsCollege = p.IsCollege,
                            Title = p.Name
                        }));
                }
            }

            return userGroupList;
        }

        /// <summary>
        /// Adds member for the particular group.
        /// </summary>
        /// <param name="groupId">Group id for which member to be added.</param>
        /// <param name="memberId">Member id to be added.</param>
        /// <param name="userId">Id of the user who is going to add the member.</param>
        /// <returns>Process Response.</returns>
        public ProcessResponse Add(int groupId, int memberId, int userId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = new UserGroup()
                {
                    GroupId = groupId,
                    UserId = memberId,
                    Status = (byte)Status.Active,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    UpdatedOn = DateTime.UtcNow
                };

                dbContext.UserGroups.Add(groupDetail);

                if (dbContext.SaveChanges() > 0)
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Success,
                        Message = Message.CREATED
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

        /// <summary>
        /// Deletes the particular member from the Group.
        /// </summary>
        /// <param name="groupId">Group id from which the member is to deleted.</param>
        /// <param name="memberId">Member id to be deleted.</param>
        /// <param name="userId">Id of the user who is going to delete the member.</param>
        /// <returns>Process response.</returns>
        public ProcessResponse Delete(int groupId, int memberId, int userId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = dbContext.UserGroups.Where(p => p.UserId == memberId && p.GroupId == groupId).FirstOrDefault();

                if (groupDetail != null && groupDetail.Id > 0)
                {
                    groupDetail.Status = (byte)Status.Deleted;
                    groupDetail.UpdatedOn = DateTime.UtcNow;
                    groupDetail.UpdatedBy = userId;

                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse() { Status = ResponseStatus.Success, Message = string.Format(Message.DELETED, count) };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the members count for the particular group and with status if given.
        /// </summary>
        /// <param name="groupId">Group id for which the members count to be given.</param>
        /// <param name="status">Status of the members in that group.</param>
        /// <returns>Members count for the group.</returns>
        public int GetMembersCount(int groupId, int userId, Status? status = null)
        {
            List<BE.UserProfile> users = new List<BE.UserProfile>();
            int statusValue = (status == null) ? 10 : (int)status;
            int userCount = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (status.HasValue)
                {
                    // gets the count users those who all are part of the group by joining user profile and user group tables with status.
                    userCount = (from usersRecord in dbContext.UserProfiles
                                 join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                                 where userGroup.Status == statusValue && userGroup.GroupId == groupId && usersRecord.Id != userId
                                 select new { usersRecord.FirstName, usersRecord.LastName, usersRecord.Id, usersRecord.ProfilePhoto, userGroup.Status }).Distinct().Count();

                }
                else
                {
                    userCount = (from usersRecord in dbContext.UserProfiles
                                 join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                                 where userGroup.Status != (byte)Status.Deleted && userGroup.GroupId == groupId && usersRecord.Id != userId
                                 select new { usersRecord.FirstName, usersRecord.LastName, usersRecord.Id, usersRecord.ProfilePhoto, userGroup.Status }).Distinct().Count();
                }
            }

            return userCount;
        }

        /// <summary>
        /// Adds list of members for the particular group.
        /// </summary>
        /// <param name="groupId">Group id for which members to be added.</param>
        /// <param name="memberId">Member id's to be added.</param>
        /// <param name="userId">Id of the user who is going to add the members</param>
        /// <returns></returns>
        public ProcessResponse AddMembers(int groupId, List<int> memberId, int userId)
        {
            ProcessResponse response = null;

            if (memberId != null && memberId.Count() > 0)
            {
                foreach (var item in memberId)
                {
                    response = Add(groupId, item, userId);
                }
            }
            else
            {
                response = new ProcessResponse()
                {
                    Status = ResponseStatus.Failed,
                    Message = Message.FAILED
                };
            }

            return response;
        }

        /// <summary>
        /// Updates the member for the particular group and with status.
        /// </summary>
        /// <param name="groupId">Group id for which the member to be updated.</param>
        /// <param name="memberId">Member id which is to be updated.</param>
        /// <param name="userId">Id of the user who is going to update.</param>
        /// <param name="status">Status of the member to be updated.</param>
        /// <returns>Process response.</returns>
        public ProcessResponse Update(int groupId, int memberId, int userId, Status status)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = dbContext.UserGroups.Where(p => p.UserId == memberId && p.GroupId == groupId).FirstOrDefault();

                if (groupDetail != null && groupDetail.Id > 0)
                {
                    groupDetail.Status = (byte)status;
                    groupDetail.UpdatedOn = DateTime.UtcNow;
                    groupDetail.UpdatedBy = userId;

                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse() { Status = ResponseStatus.Success, Message = string.Format(Message.UPDATED, count) };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the non members for the particular group and with search text if given.
        /// </summary>
        /// <param name="groupId">Group id for which the non members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <param name="searchText">Search text for which the member name contains to be given.</param>
        /// <returns>List of members those who are not part of the particular group.</returns>
        public List<BE.UserProfile> GetNonMembers(int groupId, Pager pager, string searchText)
        {
            List<BE.UserProfile> users = new List<BE.UserProfile>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<UserProfile> userList = null;
                if (string.IsNullOrEmpty(searchText))
                {
                    //Take the users from the user group table in which the group id is not same.
                    userList = dbContext.UserProfiles.Where(a => a.UserGroups.Where(q => q.GroupId == groupId && q.Status == (byte)Status.Active).Count() == 0)
                                                                             .OrderBy(o => o.Id)
                                                                             .Skip(pager.SkipCount)
                                                                             .Take(pager.PageSize)
                                                                             .ToList();
                }
                else
                {
                    userList = dbContext.UserProfiles.Where(a => a.UserGroups.Where(q => q.GroupId == groupId && q.Status == (byte)Status.Active).Count() == 0)
                                                                             .Where(p => p.FirstName.Contains(searchText))
                                                                             .OrderBy(o => o.Id)
                                                                             .Skip(pager.SkipCount)
                                                                             .Take(pager.PageSize)
                                                                             .ToList();
                }

                if (userList != null)
                {
                    foreach (var item in userList)
                    {
                        BE.UserProfile user = new BE.UserProfile();
                        user.Id = item.Id;
                        user.FirstName = (item.FirstName != null) ? item.FirstName : item.User.UserName;

                        user.LastName = (item.LastName != null) ? item.LastName : null;
                        user.ProfilePhoto = string.IsNullOrEmpty(item.ProfilePhoto) ? DefaultValue.PROFILE_DEFAULT_IMAGE_URL
                                                                                    : StorageHelper.GetMediaFilePath(item.ProfilePhoto, StorageHelper.PROFILESUMMARY_SIZE);
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Gets the non members count for the particular group and with status if given.
        /// </summary>
        /// <param name="groupId">Group id for which the non members count to be given.</param>
        /// <param name="status">Status of the non members in that group.</param>
        /// <returns>Non members count for the group.</returns>
        public int GetNonMembersCount(int groupId, Status? status = null, string searchText = null)
        {
            List<BE.UserProfile> users = new List<BE.UserProfile>();
            int statusValue = (status == null) ? 2 : (int)status;
            int userCount = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (string.IsNullOrEmpty(searchText))
                    userCount = dbContext.UserProfiles.Where(a => a.UserGroups.Where(q => q.GroupId == groupId && q.Status == statusValue).Count() == 0).Count();

                else
                    userCount = dbContext.UserProfiles.Where(a => a.UserGroups.Where(q => q.GroupId == groupId && q.Status == statusValue).Count() == 0).Where(p => p.FirstName.ToLower().Contains(searchText.ToLower())).Count();
            }

            return userCount;
        }

        /// <summary>
        /// Gets the members count for the list of groups and with status if given.
        /// </summary>
        /// <param name="groupList">List of groups which belongs to logged in user.</param>
        /// <param name="userId">User id for which the members count to be given.</param>
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>Members Count.</returns>
        public int GroupMembersCount(List<BE.UserGroup> groupList, int userId, Status? status = null)
        {
            int count = 0;
            int statusValue = (status == null) ? 2 : (int)status;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                count = (from groups in groupList
                         from usersRecord in dbContext.UserProfiles
                         join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                         where userGroup.Status == statusValue && userGroup.GroupId == groups.GroupId && usersRecord.Id != userId && usersRecord.Status == (byte)Status.Active
                         select new BE.UserProfile() { }).Distinct().Count();
            }

            return count;
        }

        /// <summary>
        /// Updates the members for the particular group and with status.
        /// </summary>
        /// <param name="groupId">Group id for which the members to be updated.</param>
        /// <param name="memberId">List of member id's which is to be updated.</param>
        /// <param name="userId">Id of the user who is going to update.</param>
        /// <param name="status">Status of the member to be updated.</param>
        /// <returns>Process response.</returns>
        public ProcessResponse UpdateMembers(int groupId, List<int> memberId, int userId, Status status)
        {
            ProcessResponse response = null;

            if (memberId != null && memberId.Count() > 0)
            {
                foreach (var item in memberId)
                {
                    response = Update(groupId, item, userId, status);
                }
            }
            else
            {
                response = new ProcessResponse()
                {
                    Status = ResponseStatus.Failed,
                    Message = Message.FAILED
                };
            }

            return response;
        }

        /// <summary>
        /// Gets the members for the particular group and with search text if given.
        /// </summary>
        /// <param name="groupId">Group id for which the members to be given.</param>
        /// <param name="userId">User id for which the members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>List of members those who are part of the group.</returns>
        public List<BE.UserProfile> GetMembers(int groupId, int userId, Pager pager, Status? status = null)
        {
            List<BE.UserProfile> users = null;
            int statusValue = (status == null) ? 10 : (int)status;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (status.HasValue)
                {
                    users = (from usersRecord in dbContext.UserProfiles
                             join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                             where userGroup.Status == statusValue && userGroup.GroupId == groupId && usersRecord.Status == (byte)Status.Active
                             select new BE.UserProfile()
                                        {
                                            FirstName = usersRecord.FirstName,
                                            LastName = (usersRecord.LastName != null) ? usersRecord.LastName : null,
                                            Id = usersRecord.Id,

                                            ProfilePhoto = usersRecord.ProfilePhoto,
                                            Status = (Status)userGroup.Status
                                        }).Distinct().OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                }
                else
                {
                    users = (from usersRecord in dbContext.UserProfiles
                             join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                             where userGroup.Status != (byte)Status.Deleted && userGroup.GroupId == groupId && usersRecord.Status == (byte)Status.Active
                             select new BE.UserProfile()
                             {
                                 FirstName = usersRecord.FirstName,
                                 LastName = (usersRecord.LastName != null) ? usersRecord.LastName : null,
                                 Id = usersRecord.Id,

                                 ProfilePhoto = usersRecord.ProfilePhoto,
                                 Status = (Status)userGroup.Status
                             }).Distinct().OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                }
            }

            return users;
        }

        /// <summary>
        /// Gets the members for the particular list of groups of the logged in user.
        /// </summary>
        /// <param name="groupList">List of groups which belongs to logged in user.</param>
        /// <param name="userId">User id for which the members to be given.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <param name="status">Status of the member to be shown.</param>
        /// <returns>List of members those who are part of the group.</returns>
        public List<BE.UserProfile> GetGroupMembers(List<BE.UserGroup> groupList, int userId, Pager pager, Status? status = null)
        {
            List<BE.UserProfile> users = null;
            int statusValue = (status == null) ? 2 : (int)status;
            int count = pager.PageSize;
            int skip = pager.SkipCount;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                users = (from groups in groupList
                         from usersRecord in dbContext.UserProfiles
                         join userGroup in dbContext.UserGroups on usersRecord.Id equals userGroup.UserId
                         where userGroup.Status == statusValue && userGroup.GroupId == groups.GroupId && usersRecord.Id != userId && usersRecord.Status == (byte)Status.Active
                         group usersRecord by usersRecord.Id into userList
                         select userList.OrderBy(o => o.Id).First() into member
                         select new BE.UserProfile()
                                    {
                                        FirstName = member.FirstName,
                                        LastName = (member.LastName != null) ? member.LastName : null,
                                        Id = member.Id,

                                        ProfilePhoto = member.ProfilePhoto,
                                        Status = (Status)member.Status
                                    }).Skip(skip).Take(count).ToList();
            }
            return users;
        }
    }
}
