using C2C.BusinessEntities;
using C2C.BusinessLogic;
using C2C.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using C2C.UI.Filters;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using System.Threading.Tasks;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Role Controller calls the business layer to perform role specific operations
    /// </summary>
    [Authorize]
    public class RoleController : BaseController
    {

        /// <summary>
        /// Creates a role by calling the business layer
        /// </summary>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult Create()
        {
            //TO DO:Authorize
            RoleViewModel roleViewModel = new RoleViewModel();

            roleViewModel.Rights = RoleManager.GetAllPermissions();
            roleViewModel.SelectedPermissionIds = new List<int>();
            return View(roleViewModel);
        }

        /// <summary>
        /// Creates a role by calling the business layer
        /// </summary>
        /// <param name="roleViewModel">Model holding parameters to create a role.</param>
        /// remarks: Post Action.
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        [HttpPost]
        public ActionResult Create(RoleViewModel roleViewModel)
        {
            //TO DO:Authorize
            Role role = new Role();
            int userId = User.UserId;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (ModelState.IsValid)
            {
                if (roleViewModel != null)
                {
                    role.Name = roleViewModel.Name;
                    role.PermissionsIDs = roleViewModel.SelectedPermissionIds;
                    roleViewModel.Rights = role.Permissions;
                    role.UpdatedBy = userId;

                    var response = RoleManager.Create(role);

                    if (response.Status == ResponseStatus.Success)
                    {
                        responseMessage.AddSuccess(response.Message);
                    }
                    else
                    {
                        responseMessage.AddError(response.Message);
                    }
                    TempData["Response"] = responseMessage;
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits a role for the given roleId.
        /// </summary>
        /// <param name="id">Id of the role to be edited.</param>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult Edit(int id)
        {
            RoleViewModel roleViewModel = new RoleViewModel();

            Role role = RoleManager.GetRoleById(id);

            roleViewModel.Name = role.Name;
            roleViewModel.RoleId = role.Id;
            role.PermissionsIDs = role.Permissions.Select(a => a.Id).ToList();
            roleViewModel.SelectedPermissionIds = role.PermissionsIDs;

            roleViewModel.Rights = RoleManager.GetAllPermissions();

            return View(roleViewModel);
        }

        /// <summary>
        /// Edits a role
        /// </summary>
        /// <param name="roleViewModel">>Model holding parameters to edit a role.<</param>
        /// remarks: Post Action.
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        [HttpPost]
        public ActionResult Edit(RoleViewModel roleViewModel)
        {
            int userId = User.UserId;
            Role role = new Role();
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (ModelState.IsValid)
            {
                if (roleViewModel != null)
                {
                    role.Name = roleViewModel.Name;
                    role.Id = roleViewModel.RoleId;
                    role.PermissionsIDs = roleViewModel.SelectedPermissionIds;
                    role.UpdatedBy = userId;

                    var response = RoleManager.Update(role);

                    if (response.Status == ResponseStatus.Success)
                    {
                        responseMessage.AddSuccess(response.Message);
                    }
                    else
                    {
                        responseMessage.AddError(response.Message);
                    }
                    TempData["Response"] = responseMessage;
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get all the roles by calling the business layer
        /// </summary>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult Index()
        {
            RoleResponseViewModel roleList = new RoleResponseViewModel();
            Pager pager = new Pager();

            List<Role> roles = RoleManager.Get(pager);
            roleList.Roles = roles;
            roleList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View(roleList);
        }

        /// <summary>
        /// Deletes a role with the specific id 
        /// </summary>
        /// <param name="id">Id of the role to be deleted.</param>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult Delete(int id)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            Role role = RoleManager.GetRoleById(id);
            role.UpdatedBy = User.UserId;

            ProcessResponse response = RoleManager.Delete(role);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the Users for the role 
        /// </summary>
        /// <param name="id">Id of the role for which the users will be rendered.</param>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult GetUsersForRole(int id)
        {
            RoleResponseViewModel roleList = new RoleResponseViewModel();
            Pager pager = new Pager();
            List<UserProfile> Users = RoleManager.GetUsersforRole(id);

            roleList.UserProfile = Users;
            roleList.ResponseMessage = (PageNotificationViewModel)TempData["Response"];
            ViewBag.RoleId = id;

            return View(roleList);
        }

        /// <summary>
        /// Deletes the user from the role 
        /// </summary>
        /// <param name="userId">Id of the user to be deleted.</param>
        /// <param name="roleId">Id of the role from which the user would be removed.</param>
        /// <returns>Action Result</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult DeleteUserFromRole(int userId, int roleId)
        {
            int updatedBy = User.UserId;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            ProcessResponse response = RoleManager.DeleteUserFromRole(userId, roleId, updatedBy);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("GetUsersForRole", new { id = roleId });
        }

        /// <summary>
        /// Add Users to the role
        /// </summary>
        /// <param name="id">Id of the role in which the user is to be added.</param>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        public ActionResult AddUsersToRole(int id)
        {
            AddUserToRoleViewModel model = new AddUserToRoleViewModel();

            model.UserProfiles = RoleManager.GetNonUsersforRole(id);
            model.SelectedUserIds = new List<int>();
            ViewBag.RoleId = id;
            return View(model);
        }

        /// <summary>
        /// Add Users to the role
        /// </summary>
        /// <param name="addUserToRoleViewModel">Model holding parameters to required to add  a role.</param>
        /// <param name="roleId">Id of the role in which the user will be added.</param>
        /// <returns>ActionResult</returns>
        [CheckPermission(ApplicationPermission.ManageRoles)]
        [HttpPost]
        public ActionResult AddUsersToRole(AddUserToRoleViewModel addUserToRoleViewModel, int roleId)
        {

            int updatedBy = User.UserId;
            if (ModelState.IsValid)
            {
                if (addUserToRoleViewModel.SelectedUserIds.Count() > 0)
                {
                    PageNotificationViewModel responseMessage = new PageNotificationViewModel();

                    var response = RoleManager.AddUsersToRole(addUserToRoleViewModel.SelectedUserIds, roleId, updatedBy);
                    if (response.Status == ResponseStatus.Success)
                    {
                        responseMessage.AddSuccess(response.Message);
                    }
                    else
                    {
                        responseMessage.AddError(response.Message);
                    }
                    TempData["Response"] = responseMessage;
                }
            }
            return RedirectToAction("Index");
        }
    }
}
