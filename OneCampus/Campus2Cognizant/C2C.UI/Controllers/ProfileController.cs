using C2C.BusinessEntities;
using C2C.BusinessLogic;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using C2C.Core.Helper;
using C2C.Core.Constants.C2CWeb;
using C2C.UI.Filters;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.UI.Publisher;
using C2C.Core.Security.Structure;
using C2C.Core.Security;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides the View and Edit Operations for UserProfile Data Entity Library.
    /// </summary>
    [Authorize]
    public class ProfileController : BaseController
    {
        /// <summary>
        /// Displays the View Profile Page 
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>View Profile Page</returns>
        public ActionResult Index()
        {
            UserProfileViewModel profileViewModel = new UserProfileViewModel();

            var userProfile = UserManager.GetUserProfile(User.UserId);

            if (userProfile != null && userProfile.Id > 0)
            {
                // Gets the College name from MetaMaster Data Entity Library
                if (userProfile.CollegeId.HasValue)
                {
                    profileViewModel.College = GroupManager.Get(userProfile.CollegeId.Value).Title;
                }

                //populate the value from Business entity to view model
                profileViewModel.Id = userProfile.Id;
                profileViewModel.FirstName = userProfile.FirstName;
                profileViewModel.LastName = userProfile.LastName;
                if (!string.IsNullOrEmpty(userProfile.ProfilePhoto) || !string.IsNullOrWhiteSpace(userProfile.ProfilePhoto))
                {
                    //Gets Media FilePath from Local FileSystem/ Azure
                    profileViewModel.ProfilePhoto = StorageHelper.GetMediaFilePath(userProfile.ProfilePhoto, StorageHelper.PROFILE_SIZE);
                }
                else
                {
                    profileViewModel.ProfilePhoto = C2C.Core.Constants.C2CWeb.DefaultValue.PROFILE_DEFAULT_IMAGE_URL;
                }
            }

            return View(profileViewModel);
        }

        /// <summary>
        /// Displays the Edit Profile Page
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Edit Profile Page</returns>
        public ActionResult Edit(int id)
        {
            UserProfileViewModel profileViewModel = new UserProfileViewModel();

            profileViewModel.ResponseMessage = (PageNotificationViewModel)TempData["profileResponse"];

            if (id > 0)
            {
                if (id == User.UserId || User.HasPermission(ApplicationPermission.ManageProfile))
                {
                    var userProfile = UserManager.GetUserProfile(id);
                    if (userProfile != null && userProfile.Id > 0)
                    {
                        // Gets the College name from MetaMaster Data Entity Library
                        if (userProfile.CollegeId.HasValue)
                        {
                            var college = GroupManager.Get(userProfile.CollegeId.Value);
                            profileViewModel.College = college.Title;
                            profileViewModel.CollegeId = college.Id;
                        }

                        //populate the value from Business entity to view model
                        profileViewModel.Id = userProfile.Id;
                        profileViewModel.FirstName = userProfile.FirstName;
                        profileViewModel.LastName = userProfile.LastName;

                        if (User.HasPermission(ApplicationPermission.ManageProfile))
                        {
                            profileViewModel.HasPermission = true;

                            var collegeList = GroupManager.GetList(true);
                            if(collegeList != null && collegeList.Count() > 0)
                                collegeList.ForEach(c => profileViewModel.CollegeList.Add(new SelectListItem() { Text = c.Title, Value = c.Id.ToString() }));
                        }

                        if (!string.IsNullOrEmpty(userProfile.ProfilePhoto) || !string.IsNullOrWhiteSpace(userProfile.ProfilePhoto))
                        {
                            //Gets Media FilePath from Local FileSystem/ Azure
                            profileViewModel.ProfilePhoto = StorageHelper.GetMediaFilePath(userProfile.ProfilePhoto, StorageHelper.PROFILE_SIZE);
                        }
                        else
                        {
                            profileViewModel.ProfilePhoto = C2C.Core.Constants.C2CWeb.DefaultValue.PROFILE_DEFAULT_IMAGE_URL;
                        }
                    }

                }
                else
                {
                    throw new HttpException(401, "Not Authorized to Perform this Action");
                }
            }

            return View(profileViewModel);
        }

        /// <summary>
        /// Performs the Update Operation
        /// </summary>
        /// <param name="profileViewModel">UserProfileViewModel with updated Value</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserProfileViewModel profileViewModel)
        {
            ProcessResponse<UserProfile> response = null;
            UserProfile userProfile = new UserProfile();
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (profileViewModel.Id > 0)
            {
                if (!string.IsNullOrEmpty(profileViewModel.FirstName) || !string.IsNullOrWhiteSpace(profileViewModel.LastName))
                {
                    //populate the value from view model to Business entity
                    userProfile.Id = profileViewModel.Id;
                    userProfile.FirstName = profileViewModel.FirstName;
                    userProfile.LastName = profileViewModel.LastName;
                    userProfile.UpdatedBy = User.UserId;

                    if (profileViewModel.File != null && !string.IsNullOrEmpty(profileViewModel.File.FileName))
                    {
                        //Uploads the Media FilePath into Local FileSystem/ Azure
                        userProfile.ProfilePhoto = StorageHelper.UploadMediaFile("Profile", profileViewModel.File, true);
                    }

                    if (User.HasPermission(ApplicationPermission.ManageUsers))
                    {
                        if (profileViewModel.CollegeId.HasValue)
                        {
                            userProfile.CollegeId = profileViewModel.CollegeId;
                        }
                    }

                    response = UserManager.Update(userProfile);


                    if (response.Status == ResponseStatus.Success)
                    {
                        responseMessage.AddSuccess(response.Message);

                        if (User.UserId == profileViewModel.Id)
                        {
                            UserContext context = new UserContext()
                            {
                                FirstName = response.Object.FirstName,
                                LastName = response.Object.LastName,
                                PhotoPath = response.Object.ProfilePhoto,
                                UserId = response.Object.Id,
                                LastLogon = DateTime.UtcNow,
                                Permissions = RoleManager.GetPermissionsforUser(response.Object.Id).Select(a => a.Id).ToList()
                            };
                            FormsAuthenticationProvider.UpdateAuthCookie(User.Identity.Name, context, true);
                        }
                    }
                    else 
                    {
                        responseMessage.AddError(response.Message);
                    }
                    TempData["profileResponse"] = responseMessage;
                }

                return RedirectToAction("Edit", new { id = profileViewModel.Id });
            }
            else
                return RedirectToAction("Index");
        }

        /// <summary>
        /// Shows the User Notification setting page
        /// </summary>
        /// <returns>user setting page</returns>
        public ActionResult GetSetting()
        {
            UserSetting setting = null;
            ViewData["profilesettingResponse"] = (PageNotificationViewModel)TempData["profilesettingResponse"];

            var userSetting = UserManager.GetNotificationSetting(User.UserId);

            if (userSetting != null && userSetting.Id > 0)
            {
                setting = userSetting;
            }
            else
            {
                setting = new UserSetting()
                {
                    Id = User.UserId
                };
            }
            return View(setting);
        }

        /// <summary>
        /// Updates the new notification setting
        /// </summary>
        /// <param name="setting">user setting</param>
        /// <returns>Redirects to Get setting action</returns>
        [HttpPost, ActionName("GetSetting")]
        public ActionResult UpdateSetting(UserSetting setting)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (setting != null && setting.Id > 0)
            {
                setting.NotificationSettingId = (short)setting.NotificationSetting;
                setting.Id = setting.Id;
                setting.UpdatedBy = User.UserId;
                var response = UserManager.UpdateNotificationSetting(setting);
                UserProfile profile = UserManager.GetUserProfile(setting.Id);

                UserDetail userDetail = new UserDetail()
                {
                    DisplayName = User.FirstName,
                    Email = profile.Email,
                    NotificationSetting = setting.NotificationSetting,
                    UserId = User.UserId
                };

                PublisherEvents publisherEvents = new PublisherEvents()
                {
                    EventId = setting.Id.ToString(),
                    EventCode = EventCodes.USER_PROFILE_SYNC,
                    NotificationContent = SerializationHelper.JsonSerialize<UserDetail>(userDetail)
                };

                NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
                TempData["profilesettingResponse"] = responseMessage;
            }
            return RedirectToAction("GetSetting");

        }

    }
}
