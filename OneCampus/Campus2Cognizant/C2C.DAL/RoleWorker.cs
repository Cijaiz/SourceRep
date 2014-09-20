using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Role Worker hits the database to do crud operations & retrieve other role specific data 
    /// </summary>
    public class RoleWorker
    {
        public static RoleWorker GetInstance()
        {
            return new RoleWorker();
        }

        /// <summary>
        /// Get all the roles 
        /// </summary>
        /// <param name="pager">Pager parameters defining the size and its count</param>
        /// <returns>Entity defining role and its permissions</returns>
        public List<BE.Role> Get(Pager pager)
        {
            List<BE.Role> roles = new List<BE.Role>();
            List<Role> selectedRoles = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                selectedRoles = dbContext.Roles.OrderBy(o => o.Name).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
            }
            foreach (var role in selectedRoles)
            {
                roles.Add(new BE.Role
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }
            return roles;
        }

        /// <summary>
        /// Creates a role with the given new role entity
        /// </summary>
        /// <param name="newRole">Entity defining role and its permissions</param>
        /// <returns>Entity defining role and its permissions, with response defining success or failure</returns>
        public ProcessResponse<BE.Role> Create(BE.Role newRole)
        {
            ProcessResponse<BE.Role> response = null;
            List<Permission> permissionDB = new List<Permission>();
            var role = new Role()
            {
                Name = newRole.Name,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = newRole.UpdatedBy
            };

            role.RolePermissions = new List<RolePermission>();
            if (newRole.PermissionsIDs != null)
            {
                newRole.PermissionsIDs.ForEach(p => role.RolePermissions.Add(
                    new RolePermission()
                    {
                        HasPermission = true,
                        RoleId = role.Id,
                        PermissionId = p,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = newRole.UpdatedBy
                    }));
            }

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                dbContext.Roles.Add(role);

                if (dbContext.SaveChanges() > 0)
                {
                    if (newRole.PermissionsIDs != null)
                    {
                        //Select only those permission details to be added to the business entity
                        permissionDB = dbContext.Permissions.Where(a => newRole.PermissionsIDs.Contains(a.Id)).ToList();
                    }
                    newRole.Id = role.Id;
                    newRole.Permissions = new List<BE.Permission>();

                    foreach (var entry in permissionDB)
                    {
                        var permission = new BE.Permission
                        {
                            Id = entry.Id,
                            Name = entry.Name,
                            Description = entry.Description,
                            ContentTypeId = (Module)entry.ContentTypeId
                        };
                        newRole.Permissions.Add(permission);
                    }

                    response = new ProcessResponse<BE.Role>() { Status = ResponseStatus.Success, Message = Message.CREATED, Object = newRole };
                }
                else
                {
                    response = new ProcessResponse<BE.Role>() { Status = ResponseStatus.Failed, Message = Message.FAILED };
                }
            }

            return response;
        }

        /// <summary>
        /// Updates a role with the given new role entity
        /// </summary>
        /// <param name="role">Entity defining role and its permissions</param>
        /// <returns>Response defining the Success or Failure of the operation</returns>
        public ProcessResponse Update(BE.Role role)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var roleDb = dbContext.Roles.Where(a => a.Id == role.Id).SingleOrDefault();

                if (role != null)
                {
                    roleDb.Name = role.Name;
                    roleDb.UpdatedBy = role.UpdatedBy;
                    roleDb.UpdatedOn = DateTime.UtcNow;

                    //Remove all the permissions for the role
                    List<RolePermission> rolePermissions = dbContext.RolePermissions.Where(a => a.RoleId == role.Id).ToList();
                    rolePermissions.ForEach(p => dbContext.RolePermissions.Remove(p));
                    if (role.PermissionsIDs != null)
                    {
                        //Update with New Permissions
                        foreach (var Id in role.PermissionsIDs)
                        {
                            var rolePermission = new RolePermission
                            {
                                RoleId = Convert.ToInt16(role.Id),
                                PermissionId = Id,
                                HasPermission = true,
                                CreatedOn = DateTime.UtcNow,
                            };
                            dbContext.RolePermissions.Add(rolePermission);
                        }
                    }
                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse()
                                {
                                    Status = ResponseStatus.Success,
                                    Message = string.Format(Message.UPDATED)
                                };
                }
                else
                {
                    response = new ProcessResponse()
                                {
                                    Status = ResponseStatus.Failed,
                                    Message = Message.RECORED_NOT_FOUND
                                };
                }
            }
            return response;
        }

        /// <summary>
        /// Delete the role 
        /// </summary>
        /// <param name="role">Entity defining role and its permissions</param>
        /// <returns>Response Defining the Success or Failure of the operation</returns>
        public ProcessResponse Delete(BE.Role role)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Delete only if its doesn't exists in User Roles table 
                if (dbContext.UserRoles.Where(a => a.RoleId == role.Id).ToList().Count == 0)
                {
                    List<RolePermission> roles = dbContext.RolePermissions.Where(a => a.RoleId == role.Id).ToList();
                    roles.ForEach(a => dbContext.RolePermissions.Remove(a));

                    Role roleDb = dbContext.Roles.Where(a => a.Id == role.Id).SingleOrDefault();
                    dbContext.Roles.Remove(roleDb);
                    roleDb.UpdatedBy = role.UpdatedBy;
                    roleDb.UpdatedOn = DateTime.UtcNow;
                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse()
                                                {
                                                    Status = ResponseStatus.Success,
                                                    Message = Message.DELETED
                                                };
                }
                else
                {
                    response = new ProcessResponse()
                                                {
                                                    Status = ResponseStatus.Failed,
                                                    Message = Message.ROLE_USER_EXSISTS
                                                };
                }
            }
            return response;
        }

        /// <summary>
        /// Gets all the permissions that can be set to the role
        /// </summary>
        /// <returns>List Defining Permissions</returns>
        public List<BE.Permission> GetAllPermissions()
        {
            List<BE.Permission> permissions = new List<BE.Permission>();
            List<Permission> selectedPermissions = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                selectedPermissions = dbContext.Permissions.OrderBy(o => o.ContentTypeId).ToList();
            }

            foreach (var permission in selectedPermissions)
            {
                permissions.Add(new BE.Permission
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description,
                    ContentTypeId = (Module)permission.ContentTypeId
                });
            }
            return permissions;
        }

        /// <summary>
        /// Gets the role with the name specified
        /// </summary>
        /// <param name="name">Name of the role</param>
        /// <returns>Entity defining role and its permissions</returns>
        public BE.Role GetRoleByName(string name)
        {
            BE.Role role = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                Role roleDb = dbContext.Roles.Where(a => a.Name == name).SingleOrDefault();
                if (roleDb != null)
                {
                    role = new BE.Role();
                    role.Id = roleDb.Id;
                    role.Name = roleDb.Name;

                    var rolePermissionList = dbContext.RolePermissions
                        .Where(a => a.RoleId == roleDb.Id && a.HasPermission == true)
                        .Select(a => new
                        {
                            PermissionId = a.PermissionId,
                            PermissionName = a.Permission.Name,
                            Description = a.Permission.Description,
                            ContentTypeId = a.Permission.ContentTypeId,
                            RoleId = a.RoleId,
                            RoleName = a.Role.Name
                        }).ToList();

                    if (rolePermissionList != null)
                    {
                        role.Permissions = new List<BE.Permission>();

                        rolePermissionList.ForEach(p => role.Permissions.Add(new BE.Permission
                        {
                            Id = p.PermissionId,
                            Name = p.PermissionName,
                            Description = p.Description,
                            ContentTypeId = (Module)p.ContentTypeId
                        }));

                        role.PermissionsIDs = role.Permissions.Select(a => a.Id).ToList();
                    }
                }
            }
            return role;
        }

        /// <summary>
        /// Get the role with specified id
        /// </summary>
        /// <param name="id">Id of the role to be  deleted</param>
        /// <returns>Entity defining role and its permissions</returns>
        public BE.Role GetRoleById(int id)
        {
            BE.Role role = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                Role roleDb = dbContext.Roles.Where(a => a.Id == id).SingleOrDefault();
                if (roleDb != null)
                {
                    role = new BE.Role();
                    role.Id = roleDb.Id;
                    role.Name = roleDb.Name;
                    role.Permissions = roleDb.RolePermissions.
                        Where(p => p.HasPermission == true).
                        Select(q => new BE.Permission()
                        {
                            Id = q.PermissionId,
                            Name = q.Permission.Name,
                            Description = q.Permission.Description,
                            ContentTypeId = (Module)q.Permission.ContentTypeId
                        }).ToList();
                }
            }
            return role;

        }

        /// <summary>
        /// Gets the Users for the role for the given roleId
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <returns>List containing user profile details</returns>
        public List<BE.UserProfile> GetUsersforRole(int id)
        {
            List<BE.UserProfile> userProfiles = new List<BE.UserProfile>();
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<UserProfile> userProfilesDb = dbContext.UserRoles.Where(a => a.RoleId == id && a.IsDeleted == false).Select(a => a.UserProfile).ToList();
                if (userProfilesDb.Count() > 0)
                {
                    userProfilesDb.ForEach(a => userProfiles.Add(new BE.UserProfile
                    {

                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        CollegeId = a.CollegeId,
                        ProfilePhoto = a.ProfilePhoto,
                        UpdatedBy = a.UpdatedBy,
                        UpdatedOn = a.UpdatedOn
                    }));

                }

            }
            return userProfiles;
        }

        /// <summary>
        /// Deletes the User from the Specified Role
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="roleId">Id of the role</param>
        /// <param name="updatedBy">updatedBy defining id of user who updates</param>
        /// <returns>Response Defining the Success or Failure of the operation</returns>
        /// 
        public ProcessResponse DeleteUserFromRole(int userId, int roleId, int updatedBy)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                UserRole userRoleDb = dbContext.UserRoles.Where(a => a.RoleId == roleId && a.UserId == userId).SingleOrDefault();
                userRoleDb.IsDeleted = true;
                userRoleDb.UpdatedBy = updatedBy;
                userRoleDb.UpdatedOn = DateTime.UtcNow;
                int count = dbContext.SaveChanges();
                if (count > 0)
                {
                    response = new ProcessResponse()
                                                {
                                                    Status = ResponseStatus.Success,
                                                    Message = Message.DELETED
                                                };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.ROLE_USER_EXSISTS };
                }
            }
            return response;
        }
        /// <summary>
        /// Gets the list of users who doesnt belong to the role
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <returns>List containing user profile details</returns>
        public List<BE.UserProfile> GetNonUsersforRole(int id)
        {
            List<BE.UserProfile> userProfiles = new List<BE.UserProfile>();
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<UserProfile> userProfilesDb = null;
                List<UserProfile> users = null;

                //1.Add users who doesnt belong to this role
                userProfilesDb = dbContext.UserRoles.Where(a => a.RoleId != id && a.IsDeleted == false).Select(a => a.UserProfile).ToList();

                //2.Add Users who belonged to this role and been deleted 
                users = dbContext.UserRoles.Where(a => a.RoleId == id && a.IsDeleted == true).Select(a => a.UserProfile).ToList();
                if (users.Count() > 0)
                {
                    //Add only unique users from condition 1 &2
                    users.Except(userProfilesDb).ToList().ForEach(a => userProfilesDb.Add(a));
                }

                //Select all users who havent mapped to any role
                var values = from userProfile in dbContext.UserProfiles
                             where !(dbContext.UserRoles.Any(a => a.UserId == userProfile.Id))
                             select new { userProfile.FirstName, userProfile.LastName, userProfile.Id, userProfile.ProfilePhoto };

                //Add non role users to list
                foreach (var item in values)
                {
                    userProfilesDb.Add(new UserProfile { Id = item.Id, FirstName = item.FirstName, LastName = item.LastName, ProfilePhoto = item.LastName });
                }


                if (userProfilesDb.Count() > 0)
                {
                    userProfilesDb.ForEach(a => userProfiles.Add(new BE.UserProfile
                    {

                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        CollegeId = a.CollegeId,
                        ProfilePhoto = a.ProfilePhoto,
                        UpdatedBy = a.UpdatedBy,
                        UpdatedOn = a.UpdatedOn
                    }));

                }

            }
            return userProfiles;
        }
        /// <summary>
        /// Adds the list of users to the specified role
        /// </summary>
        /// <param name="userIds">Id of the role</param>
        /// <param name="roleId">Id of the role</param>
        /// <param name="updatedBy">updatedBy defining id of user who updates</param>
        /// <returns>Response defining the success or failure of the operation</returns>
        public ProcessResponse AddUsersToRole(List<int> userIds, int roleId, int updatedBy)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Reset the role for user if that was soft deleted before
                List<UserRole> userRoles = dbContext.UserRoles.Where(a => a.RoleId == roleId && userIds.Contains(a.UserId)).ToList();
                foreach (var item in userRoles)
                {
                    item.IsDeleted = false;
                    item.UpdatedBy = updatedBy;
                    item.UpdatedOn = DateTime.UtcNow;
                }
                //Add only new users who have not been mapped before
                foreach (var userId in userIds.Except(userRoles.Select(a => a.UserId).ToList()))
                {
                    dbContext.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = Convert.ToInt16(roleId),
                        IsDeleted = false,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = updatedBy
                    });
                }
                int count = dbContext.SaveChanges();
                if (count > 0)
                {
                    response = new ProcessResponse()
                                                    {
                                                        Status = ResponseStatus.Success,
                                                        Message = string.Format(Message.UPDATED_COUNT, count)
                                                    };
                }
                else
                {
                    response = new ProcessResponse()
                                                    {
                                                        Status = ResponseStatus.Failed,
                                                        Message = Message.RECORED_NOT_FOUND
                                                    };
                }
            }
            return response;
        }
        /// <summary>
        /// Gets the list of permissions for the user with the given id.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of permissions contained by the user</returns>
        public List<BE.Permission> GetPermissionsforUser(int userId)
        {
            List<BE.Permission> permissions = new List<BE.Permission>();

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<int> roleIds = dbContext.UserRoles.Where(a => a.UserId == userId).Select(a => a.Role.Id).ToList();
                foreach (var id in roleIds)
                {
                    var rolePermissionList = dbContext.RolePermissions
                     .Where(a => a.RoleId == id && a.HasPermission == true)
                     .Select(a => new
                     {
                         PermissionId = a.PermissionId,
                         PermissionName = a.Permission.Name,
                         Description = a.Permission.Description,
                         ContentTypeId = a.Permission.ContentTypeId,
                         RoleId = a.RoleId,
                         RoleName = a.Role.Name
                     }).ToList();

                    if (rolePermissionList != null)
                    {
                        rolePermissionList.ForEach(p => permissions.Add(new BE.Permission
                       {
                           Id = p.PermissionId,
                           Name = p.PermissionName,
                           Description = p.Description,
                           ContentTypeId = (Module)p.ContentTypeId
                       }));
                    }
                }
                return permissions;
            }

        }
    }
}
