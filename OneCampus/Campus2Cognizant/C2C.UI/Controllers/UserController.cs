using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.UI.Filters;
using C2C.UI.Publisher;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// User Controller calls the business layer to perform user specific operations
    /// </summary>
    [Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// Lists all the users
        /// </summary>
        /// <param name="filter">values based on which users are filtered(e.g Status,Name)</param>
        /// <returns>ActionResult</returns>    
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Index(UserListFilter filter = null)
        {
            Pager pager = new Pager();
            Status? Status = null;
            int count;
            List<User> userList = new List<User>();
            UserListViewModel userListViewModel = new UserListViewModel();

            userListViewModel.Filter.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();

            if (string.IsNullOrEmpty(filter.Name) && (string.IsNullOrEmpty(filter.Status) || filter.Status == "All"))
                count = UserManager.GetCount(pager);
            else
            {
                if (filter.Status != "All")
                {
                    Status = (Status)Enum.Parse(typeof(Status), filter.Status, true);
                }
                if (string.IsNullOrEmpty(filter.Name))
                    count = UserManager.GetCount(pager, Status);
                else
                    count = UserManager.GetCount(pager, filter.Name, Status);
            }

            int totalPageCount = (int)Math.Ceiling((double)count / (double)pager.PageSize);

            userListViewModel.Filter.StatusList.Add(new SelectListItem { Text = "All", Selected = true, });
            userListViewModel.User = userList;
            userListViewModel.Filter.Status = filter.Status;
            userListViewModel.Filter.Name = filter.Name;
            userListViewModel.PageCount = totalPageCount;
            userListViewModel.ResponseMessage = (PageNotificationViewModel)TempData["Response"];

            return View(userListViewModel);
        }

        /// <summary>
        /// Gets the list of user based on the filter conditions
        /// </summary>
        /// <param name="page">for Pagination</param>
        /// <param name="name">User name</param>
        /// <param name="status">Status of the user</param>
        /// <returns>Lists of user</returns>
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult List(int page, string name, string status)
        {
            Pager pager = new Pager();
            pager.PageNo = page;
            Status? Status = null;
            List<User> userList = new List<User>();

            if (string.IsNullOrEmpty(name) && (string.IsNullOrEmpty(status) || status == "All"))
                userList = UserManager.Get(pager);
            else
            {
                if (status != "All")
                {
                    Status = (Status)Enum.Parse(typeof(Status), status, true);
                }
                if (string.IsNullOrEmpty(name))
                    userList = UserManager.Get(pager, Status);
                else
                    userList = UserManager.Get(pager, name, Status);
            }

            return View(userList);
        }

        /// <summary>
        /// Displays the list of group for the particular user.
        /// </summary>
        /// <param name="userId">User Is for which group list is given.</param>
        /// <returns>Group List.</returns>
        public ActionResult UserGroups(int userId)
        {
            Pager pager = new Pager();
            var userGroups = UserGroupManager.GetGroups(userId, pager);
            List<string> groups = new List<string>();
            groups.Add("Groups");
            int i = 1;
            if (userGroups != null)
            {
                foreach (var item in userGroups)
                {
                    groups.Add(string.Format("{0}.{1}", i, item.Title));
                    i++;
                }
            }
            var groupList = string.Join("\n", groups);

            return Json(new { Status = true, userGroups = groupList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays the user create page using user model.
        /// </summary>
        /// <returns>Create Page</returns>
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the user
        /// </summary>
        /// <param name="user">User entity to create the user</param>
        /// <returns>Index Page</returns>
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Create(User user)
        {
            PageNotificationViewModel notificationMsg = new PageNotificationViewModel();
            if (ModelState.IsValid)
            {
                //Logged in User Id
                user.UpdatedBy = User.UserId;

                var response = UserManager.Create(user);

                if (response.Status == ResponseStatus.Success)
                {
                    UserDetail userDetail = new UserDetail()
                    {
                        DisplayName = response.Object.UserName,
                        Email = string.Empty,
                        NotificationSetting = NotificationSetting.WithinCollege,
                        UserId = response.Object.Id
                    };

                    PublisherEvents publisherEvents = new PublisherEvents()
                    {
                        EventId = response.Object.Id.ToString(),
                        EventCode = EventCodes.USER_PROFILE_SYNC,
                        NotificationContent = SerializationHelper.JsonSerialize<UserDetail>(userDetail)
                    };

                    C2C.UI.Publisher.NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);

                    notificationMsg.AddSuccess(response.Message);
                    TempData["Response"] = notificationMsg;

                    return RedirectToAction("Index");
                }
                else
                {
                    notificationMsg.AddError(Message.USER_EXSISTS);

                    ViewData.Add("Notification", notificationMsg);
                    return View(user);
                }
            }
            else
            {
                notificationMsg.AddError(Message.VALIDATION_FAILED);

                ViewData.Add("Notification", notificationMsg);
                return View(user);
            }
        }

        /// <summary>
        /// Deletes the given user
        /// </summary>
        /// <param name="id">User Id to delete</param>
        /// <returns>Index Page</returns>
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Delete(int id)
        {
            UserManager.Delete(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Activates the given user
        /// </summary>
        /// <param name="id">User Id to activate</param>
        /// <returns>Index Page</returns>
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Activate(int id)
        {
            UserManager.Activate(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deactivates the given user
        /// </summary>
        /// <param name="id">User Id to Deactivate</param>
        /// <returns>Index Page</returns>
        [CheckPermission(ApplicationPermission.ManageUsers)]
        public ActionResult Deactivate(int id)
        {
            UserManager.Deactivate(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the Page to do file upload
        /// </summary>
        /// <returns>File Upload Page</returns>
        public ActionResult FileUpload()
        {
            ViewData["fileUploadResponse"] = (PageNotificationViewModel)TempData["fileUploadResponse"];
            return View();
        }

        /// <summary>
        /// Uploads the given file into Azure Blob storage
        /// </summary>
        /// <param name="file">the file to upload</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            try
            {
                if (ValidateUploadedDocument(file))
                {
                    byte[] buffer = new byte[file.ContentLength];
                    file.InputStream.Position = 0;
                    file.InputStream.Read(buffer, 0, file.ContentLength);
                    string fileName = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
                    string uploadedFilePath = StorageHelper.UploadMediaFile("CHireImport", file.FileName, buffer.ToArray());

                    string blobURL = StorageHelper.GetMediaFilePath(uploadedFilePath);

                    
                    NotifyPublisher(blobURL, fileName);
                    responseMessage.AddSuccess(Message.FILE_UPLOAD_SUCCESS);
                    //_orchardServices.Notifier.Information(T("File has been successfully Uploaded"));
                }
            }
            catch
            {
                responseMessage.AddError(Message.FILE_UPLOAD_FAILURE);
                //_orchardServices.Notifier.Error(T("Error in uploading the file."));
            }
            TempData["fileUploadResponse"] = responseMessage;
            return RedirectToAction("FileUpload");
        }

        /// <summary>
        /// Validates the document(Checks for correct columns and Data)
        /// </summary>
        /// <param name="file">file to validate</param>
        /// <returns>File is valid or not</returns>
        private bool ValidateUploadedDocument(HttpPostedFileBase file)
        {
           
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();
            if (file != null && Path.GetExtension(file.FileName) != null && Path.GetExtension(file.FileName).Equals(".csv") && file.ContentLength > 0)
            {
                string columnNames = string.Empty;

                StreamReader inputStream = new StreamReader(file.InputStream);
                columnNames = inputStream.ReadLine();

                string[] candidateInfo = columnNames.Split(',').ToArray();

                if (candidateInfo[0].ToLower().Equals("candidateid") &&
                    candidateInfo[1].ToLower().Equals("firstname") &&
                    candidateInfo[2].ToLower().Equals("middleinitial") &&
                    candidateInfo[3].ToLower().Equals("lastname") &&
                    candidateInfo[4].ToLower().Equals("emailaddress") &&
                    candidateInfo[5].ToLower().Equals("dateofbirth") &&
                    candidateInfo[6].ToLower().Equals("tenthmarksheetno") &&
                    candidateInfo[7].ToLower().Equals("tenthyearofpassing") &&
                    candidateInfo[8].ToLower().Equals("tenthpercentage") &&
                    candidateInfo[9].ToLower().Equals("twelveth_yearofcompletion") &&
                    candidateInfo[10].ToLower().Equals("twelveth_percentage") &&
                    candidateInfo[11].ToLower().Equals("educationlevel") &&
                    candidateInfo[12].ToLower().Equals("college") &&
                    candidateInfo[13].ToLower().Equals("degree") &&
                    candidateInfo[14].ToLower().Equals("program") &&
                    candidateInfo[15].ToLower().Equals("gpa") &&
                    candidateInfo[16].ToLower().Equals("gparange") &&
                    candidateInfo[17].ToLower().Equals("graduationdate") &&
                    candidateInfo[18].ToLower().Equals("otherinstitution") &&
                    candidateInfo[19].ToLower().Equals("otherprogram") &&
                    candidateInfo[20].ToLower().Equals("privacysettings"))
                {
                    return true;
                }

                responseMessage.AddError(Message.FILE_UPLOAD_FORMAT_ERROR);
                //ModelState.AddModelError("column mismatches. Please provide correct column format"));
                return false;
            }
            else
            {
                if (file == null)
                {
                    responseMessage.AddError(Message.FILE_UPLOAD_FILE_NULL);
                    //_orchardServices.Notifier.Error(T("Upload File"));

                }
                else if (file.ContentLength <= 0)
                {
                    responseMessage.AddError(Message.FILE_UPLOAD_CONTENT_NULL);
                    //_orchardServices.Notifier.Error(T("Upload file with required content"));
                }
                else
                {
                    responseMessage.AddError(Message.FILE_UPLOAD_EXTENSION_ERROR);
                    //_orchardServices.Notifier.Error(T("File Extension is invalid. It Should be in .csv Format"));
                }
                return false;
            }
        }

        /// <summary>
        /// Pushes a message in Queue to import users from blob
        /// </summary>
        /// <param name="blobURL">BlobUrl from where the file is to be read.</param>
        /// <param name="fileName">Name of the file to import</param>
        private void NotifyPublisher(string blobURL, string fileName)
        {
            if (!string.IsNullOrEmpty(blobURL) && !string.IsNullOrEmpty(fileName))
            {
                NotificationContent notificationContent = new NotificationContent()
                {
                    //URL = _orchardServices.ContentManager.GetItemMetadata(share.ContentItem).DisplayRouteValues.ToString(),
                    URL = blobURL,
                    ContentTitle = fileName
                };

                PublisherEvents publisherEvents = new PublisherEvents()
                {
                    EventCode = EventCodes.IMPORT_USERFROM_BLOB,
                    NotificationContent = SerializationHelper.JsonSerialize<NotificationContent>(notificationContent),
                    EventId = "80",
                    TaskId = "01"
                };

                //Adding code for sending messages to Queue
                C2C.UI.Publisher.NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);
            }
        }


    }
}
