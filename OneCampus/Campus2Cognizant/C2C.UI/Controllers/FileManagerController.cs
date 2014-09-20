using C2C.Core.Helper;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    public class FileManagerController : BaseController
    {
        /// <summary>
        /// Uploads on Image under the specified module name
        /// </summary>
        /// <param name="name">Module name under which image has to be uploaded</param>
        /// <returns></returns>
        public ActionResult UploadImage(string name = "")
        {
            UploadImageViewModel model = new UploadImageViewModel() { ModuleName = name };

            model.ImageUrls = new List<string>();
            model.ImageUrls = StorageHelper.GetMediaFilesUnderContainer(model.ModuleName);
            
            return View(model);
        }

        /// <summary>
        /// Uploads the Image 
        /// </summary>
        /// <param name="model">Model containing the file base , path to upload, uploaded url</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage(UploadImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guard.IsNotNull(model.ImageFile, "ImageFile");
                if (model.ImageFile != null)
                {
                    model.Url = StorageHelper.GetMediaFilePath(StorageHelper.UploadMediaFile(model.ModuleName, model.ImageFile));
                    if (model.Url != null)
                    {
                        model.IsuploadSuccess = true;
                    }
                }
            }

            return View(model);
        }

    }
}

