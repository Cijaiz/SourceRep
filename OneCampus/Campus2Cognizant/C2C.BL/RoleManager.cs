using System.Collections.Generic;
using C2C.BusinessEntities;
using DAL = C2C.DataAccessLogic;
using C2C.Core.Helper;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// RoleManager calls the role worker to do role specific operations
    /// </summary>

    public static class RoleManager
    {

        /// <summary>
        /// Gets all the roles by calling the Data Access layer
        /// </summary>
        /// <param name="pager">Pager Parameters</param>
        /// <returns></returns>
        public static List<Role> Get(Pager pager)
        {
            return DAL.RoleWorker.GetInstance().Get(pager);
        }

        /// <summary>
        /// Creates a new role by calling the Data Access layer
        /// </summary>
        /// <param name="newRole">Entity defining role and its permissions</param>
        /// <returns>Entity defining role and its permissions & response defining Success orFailure of the operation</returns>
        public static ProcessResponse<Role> Create(Role newRole)
        {
            Guard.IsNotNull(newRole, "Role");
            ProcessResponse<Role> response = null;
            //Check if the Role Name Exists 
            if (DAL.RoleWorker.GetInstance().GetRoleByName(newRole.Name) == null)
            {
                response = DAL.RoleWorker.GetInstance().Create(newRole);
            }
            else
            {
                response = new ProcessResponse<Role>() { Status = ResponseStatus.Failed, Message = Message.ROLE_EXSISTS };
            }
            return response;
        }

        /// <summary>
        /// Updates a role by calling the Data Access layer
        /// </summary>
        /// <param name="role">Entity defining role and its permissions</param>
        /// <returns>Response defining Success or Failure of the operation</returns>
        public static ProcessResponse Update(Role role)
        {
            Guard.IsNotNull(role, "Role");
            ProcessResponse response = null;
            response = DAL.RoleWorker.GetInstance().Update(role);
            //TODO:Notify the user on the response
            return response;
        }

        /// <summary>
        /// Deletes a role by calling the Data Access layer
        /// </summary>
        /// <param name="role">Entity defining role and its permissions</param>
        /// <returns>Response defining Success or Failure of the operation</returns>
        public static ProcessResponse Delete(Role role)
        {
            Guard.IsNotNull(role, "Role");
            ProcessResponse response = null;
            response = DAL.RoleWorker.GetInstance().Delete(role);
            //TODO:Notify the user on the response
            return response;
        }

        /// <summary>
        /// Gets the role & permission based on the id by calling the Data Access layer
        /// </summary>
        /// <param name="id">RoleId</param>
        /// <returns>Entity defining role and its permissions</returns>
        public static Role GetRoleById(int id)
        {
            Guard.IsNotZero(id, "RoleId");
            Role role = null;
            role = DAL.RoleWorker.GetInstance().GetRoleById(id);

            return role;
        }

        /// <summary>
        /// Get all the permissions that can be set for the role by calling the Data Access layer
        /// </summary>
        /// <returns>List of entities defining Permissions</returns>
        public static List<Permission> GetAllPermissions()
        {
            return DAL.RoleWorker.GetInstance().GetAllPermissions();
        }

        /// <summary>
        /// Get all Users belonging to the role by calling the Data Access layer
        /// </summary>
        /// <param name="id">id of the role</param>
        /// <returns>List of entities containing user permission</returns>
        public static List<UserProfile> GetUsersforRole(int id)
        {
         Guard.IsNotZero(id, "RoleId");
         return DAL.RoleWorker.GetInstance().GetUsersforRole(id);
           
        }

        /// <summary>
        /// Get all Users who belong  to the role by calling the Data Access layer
        /// </summary>
        /// <param name="id">id of the role</param>
        /// <returns>List of entities containing user profile</returns>
        public static List<UserProfile> GetNonUsersforRole(int id)
        {
            Guard.IsNotZero(id, "RoleId");
            return DAL.RoleWorker.GetInstance().GetNonUsersforRole(id);

        }

        /// <summary>
        /// Deletes the given User from a role by calling Data Access Layer
        /// </summary>
        /// <param name="userId">Id of the User</param>
        /// <param name="roleId">Id of the Role</param>
        /// <returns>Response defining Success or Failure of the operation</returns>
        public static ProcessResponse DeleteUserFromRole(int userId, int roleId, int updatedBy)
        {
            updatedBy = 0;
            Guard.IsNotZero(userId, "userId");
            Guard.IsNotZero(roleId, "roleId");
            //Guard.IsNotZero(updatedBy, "UpdatedBy");
            ProcessResponse response = null;
            response = DAL.RoleWorker.GetInstance().DeleteUserFromRole(userId,roleId,updatedBy);
            //TODO:Notify the user on the response
            return response;
        }

        /// <summary>
        /// Add the given set of users to specific role
        /// </summary>
        /// <param name="userIds">list of userIds</param>
        /// <returns>Response defining Success or Failure of the operation</returns>
        public static ProcessResponse AddUsersToRole(List<int> userIds, int roleId, int updatedBy)
        {
            updatedBy = 0;
            Guard.IsNotNull(userIds, "UserId");
            Guard.IsNotZero(roleId, "roleId");
            //Guard.IsNotZero(updatedBy, "UpdatedBy");
            return DAL.RoleWorker.GetInstance().AddUsersToRole(userIds, roleId,updatedBy);
        }

        /// <summary>
        /// Gets the list of permissions for the user with the given id by calling the DAL layer
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of permissions contained by the user</returns>
        public static List<Permission> GetPermissionsforUser(int userId)
        {
            Guard.IsNotZero(userId, "userId");
            return DAL.RoleWorker.GetInstance().GetPermissionsforUser(userId);
        }
        
    }
}
