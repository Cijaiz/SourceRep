#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Helper;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;
#endregion

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the Group and Uses GroupWorker to do Data manipulation.
    /// </summary>
    public static class GroupManager
    {
        /// <summary>
        /// Gets the group details using group id.
        /// </summary>
        /// <param name="id">Group id.</param>
        /// <returns>Group detail for the particular id.</returns>
        public static Group Get(int id)
        {
            Guard.IsNotZero(id, "GroupId");
            return DAL.GroupWorker.GetInstance().Get(id);
        }

        /// <summary>
        /// Get all the groups as a list.
        /// </summary>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <returns>Group List.</returns>
        public static List<Group> GetList(Pager pager)
        {
            return DAL.GroupWorker.GetInstance().GetList(pager);
        }

        /// <summary>
        /// Updates the group using group object.
        /// </summary>
        /// <param name="group">Group object which contains the details of the group to be created.</param>
        /// <returns>Process Response.</returns>
        public static ProcessResponse Update(Group group)
        {
            Guard.IsNotNull(group, "Group");
            ProcessResponse response = null;

            response = DAL.GroupWorker.GetInstance().Update(group);
            return response;
        }

        /// <summary>
        /// Gets the group count.
        /// </summary>
        /// <param name="searchText">Search text which contains the group name.</param>
        /// <returns>Group count.</returns>
        public static int GetCount(string searchText = null)
        {
            return DAL.GroupWorker.GetCount(searchText);
        }

        /// <summary>
        /// Get all the groups as a list.
        /// </summary>
        /// <returns>Group List.</returns>
        public static List<Group> GetList(bool? isCollege = false)
        {
            return DAL.GroupWorker.GetInstance().GetList(isCollege);
        }

        /// <summary>
        /// Gets the group list by using the search text.
        /// </summary>
        /// <param name="searchText">Search text for which the list of groups shown.</param>
        /// <param name="pager">Page object which gives the page size and page count.</param>
        /// <returns>Group List according to the search text.</returns>
        public static List<Group> Search(string searchText, Pager pager)
        {
            return DAL.GroupWorker.GetInstance().Search(searchText, pager);
        }

        /// <summary>
        /// Creates the group using group object and User Id who is going to create.
        /// </summary>
        /// <param name="group">Group object which contains the details of the group to be created.</param>
        /// <param name="userId">User Id by which the group is going to be created.</param>
        /// <returns>Process Response</returns>
        public static ProcessResponse<Group> Create(Group group, int userId)
        {
            Guard.IsNotNull(group, "Group");
            Guard.IsNotZero(userId, "UserId");
            ProcessResponse<Group> response = null;

            bool isGroupExist = DAL.GroupWorker.GetInstance().IsGroupExist(group.Title);

            //check whether the group name already exist.
            if (isGroupExist)
            {
                response = new ProcessResponse<Group>()
                {
                    Status = ResponseStatus.Failed,
                    Message = Message.GROUP_EXIST
                };
            }
            else
            {
                response = DAL.GroupWorker.GetInstance().Create(group, userId);
            } 

            return response;
        }
    }
}
