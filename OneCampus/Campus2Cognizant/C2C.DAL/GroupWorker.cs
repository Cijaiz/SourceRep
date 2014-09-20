#region References
using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;
#endregion

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on Group Entity Libraries.
    /// </summary>
    public class GroupWorker
    {
        /// <summary>
        /// Creates a new instance for GroupWorker class
        /// </summary>
        /// <returns>Returns the new instance</returns>
        public static GroupWorker GetInstance()
        {
            return new GroupWorker();
        }

        /// <summary>
        /// Gets the group details using group id.
        /// </summary>
        /// <param name="id">Group id.</param>
        /// <returns>Group detail for the particular id.</returns>
        public BE.Group Get(int id)
        {
            BE.Group group = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = dbContext.Groups.Where(p => p.Id == id).FirstOrDefault();

                if (groupDetail != null && groupDetail.Id > 0)
                {
                    group = new BE.Group()
                    {
                        Id = groupDetail.Id,
                        Title = groupDetail.Name,
                        IsCollege = groupDetail.IsCollege,

                        UpdatedBy = groupDetail.UpdatedBy.HasValue ? groupDetail.UpdatedBy.Value : groupDetail.CreatedBy,
                        UpdatedOn = groupDetail.UpdatedOn.HasValue ? groupDetail.UpdatedOn.Value : groupDetail.CreatedOn
                    };
                }
            }

            return group;
        }       

        /// <summary>
        /// Get all the groups as a list.
        /// </summary>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <returns>Group List.</returns>
        public List<BE.Group> GetList(Pager pager)
        {
            List<BE.Group> groupList = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groups = dbContext.Groups.OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();

                if (groups != null && groups.Count() > 0)
                {
                    groupList = new List<BE.Group>();
                    groups.ForEach(p => groupList.Add(
                        new BE.Group()
                        {
                            Id = p.Id,
                            Title = p.Name,
                            IsCollege = p.IsCollege,

                            UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                            UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                        }));
                }
            }

            return groupList;
        }

        /// <summary>
        /// Checks whether the group name already exist and returns true or false.
        /// </summary>
        /// <param name="groupName">The group name to be checked for exist.</param>
        /// <returns>Is group exist or not/</returns>
        public bool IsGroupExist(string groupName)
        {
            bool isExist = false;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Checks whether the group name already exist and returns true or false.
                isExist = dbContext.Groups.Where(p => p.Name.ToLower() == groupName.Trim().ToLower()).Count() > 0;
            }

            return isExist;
        }

        /// <summary>
        /// Creates group and returns groupId if already exists returns existing group Id
        /// </summary>
        /// <param name="groupName">The group name to be checked for exist.</param>
        /// <returns>Grou/</returns>
        public int GetGroupId(BE.Group groupModel)
        {
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Checks whether the group name already exist and returns true or false.
                var group = dbContext.Groups.Where(p => p.Name.ToLower() == groupModel.Title.Trim().ToLower()).FirstOrDefault();

                if (group == null)
                {
                    //If group not exists it'll create and returns the created Group Id
                    var response = Create(groupModel, groupModel.UserId);

                    //If group has been created successfully
                    if (response.Status == ResponseStatus.Success)
                    {
                        group = new Group()
                                            {
                                                Id = response.Object.Id
                                            };
                    }
                }

                return group.Id;
            }

        }

        /// <summary>
        /// Updates the group using group object.
        /// </summary>
        /// <param name="group">Group object which contains the details of the group to be created.</param>
        /// <returns>Process Response.</returns>
        public ProcessResponse Update(BE.Group group)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = dbContext.Groups.Where(p => p.Id == group.Id).FirstOrDefault();

                if (groupDetail != null)
                {
                    //populating business entity from data entity
                    groupDetail.Id = group.Id;
                    groupDetail.Name = group.Title;
                    groupDetail.IsCollege = group.IsCollege;
                    groupDetail.UpdatedBy = group.UpdatedBy;
                    groupDetail.UpdatedOn = DateTime.UtcNow;

                    int count = dbContext.SaveChanges();

                    response = new ProcessResponse() { Status = ResponseStatus.Success, Message = Message.UPDATED };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the group count.
        /// </summary>
        /// <param name="searchText">Search text which contains the group name.</param>
        /// <returns>Group count.</returns>
        public static int GetCount(string searchText = null)
        {
            int count = 0;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (string.IsNullOrEmpty(searchText))
                    count = dbContext.Groups.Count();
                else
                    count = dbContext.Groups.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).Count();
            }
            return count;
        }

        /// <summary>
        /// Get all the groups as a list.
        /// </summary>
        /// <returns>Group List.</returns>
        public List<BE.Group> GetList(bool? isCollege = false)
        {
            List<BE.Group> groupList = null;
            bool isCollegeGroup = (isCollege == false) ? false : (bool)isCollege;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<Group> groups = null;
                if (isCollegeGroup)
                {
                    groups = dbContext.Groups.Where(q => q.IsCollege == isCollegeGroup).ToList();
                }
                else
                {
                    groups = dbContext.Groups.ToList();
                }

                if (groups != null && groups.Count() > 0)
                {
                    groupList = new List<BE.Group>();
                    groups.ForEach(p => groupList.Add(
                        new BE.Group()
                        {
                            Id = p.Id,
                            Title = p.Name,
                            IsCollege = p.IsCollege,

                            UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                            UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                        }));
                }
            }

            return groupList;
        }

        /// <summary>
        /// Gets the group list by using the search text.
        /// </summary>
        /// <param name="searchText">Search text for which the list of groups shown.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <returns>Group List according to the search text.</returns>
        public List<BE.Group> Search(string searchText, Pager pager)
        {
            List<BE.Group> groupList = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groups = dbContext.Groups.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();

                if (groups != null && groups.Count() > 0)
                {
                    groupList = new List<BE.Group>();
                    groups.ForEach(p => groupList.Add(
                        new BE.Group()
                        {
                            Id = p.Id,
                            Title = p.Name,
                            IsCollege = p.IsCollege,

                            UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                            UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                        }));
                }
            }

            return groupList;
        }

        /// <summary>
        /// Creates the group using group object and User Id who is going to create.
        /// </summary>
        /// <param name="group">Group object which contains the details of the group to be created.</param>
        /// <param name="userId">User Id by which the group is going to be created.</param>
        /// <returns>Process Response.</returns>
        public ProcessResponse<BE.Group> Create(BE.Group group, int userId)
        {
            ProcessResponse<BE.Group> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var groupDetail = new Group()
                {
                    Name = group.Title.ToLower(),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userId,

                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = userId,
                    IsCollege = group.IsCollege
                };

                dbContext.Groups.Add(groupDetail);

                if (dbContext.SaveChanges() > 0)
                {
                    group.Id = groupDetail.Id;
                    response = new ProcessResponse<BE.Group>()
                    {
                        Status = ResponseStatus.Success,
                        Object = group,
                        Message = Message.CREATED
                    };
                }
                else
                {
                    response = new ProcessResponse<BE.Group>()
                    {
                        Status = ResponseStatus.Failed,
                        Object = group,
                        Message = Message.FAILED
                    };
                }
            }

            return response;
        }
    }
}
