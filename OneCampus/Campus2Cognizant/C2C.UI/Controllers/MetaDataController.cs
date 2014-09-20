using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessLogic;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    /// <summary>
    /// Provides CRUD operations for MetaKeyValues Entity Library and Read operation for MetaKeyReference Entity library.
    /// </summary>
    public class MetaDataController : BaseController
    {
        /// <summary>
        /// Displays the Index page with List of KeyReferences
        /// </summary>
        /// <returns>An index page</returns>
        public ActionResult Index()
        {
            var metaMasters = MetaDataManager.GetMetaMasterList();
            return View(metaMasters);
        }

        /// <summary>
        /// Gets the KeyValue using Key Value Id
        /// </summary>
        /// <param name="id">Key value Id</param>
        /// <returns>Edit Page</returns>
        public ActionResult Edit(int id)
        {
            MetaValueViewModel metaValueVM = null;

            if (id > 0)
            {
                var metaValue = MetaDataManager.GetMetaValue(id);

                //Populating ViewModel using Business Entity
                if (metaValue != null && metaValue.MetaMaster != null)
                {
                    metaValueVM = new MetaValueViewModel();
                    metaValueVM.Id = metaValue.Id;
                    metaValueVM.MetaMasterId = metaValue.MetaMaster.Id;
                    metaValueVM.Value = metaValue.Value;
                    metaValueVM.MetaMasterKeyReference = metaValue.MetaMaster.KeyReference;
                }
            }
            return View(metaValueVM);
        }

        /// <summary>
        /// Shows the List of all key values for Single Reference
        /// </summary>
        /// <param name="id">Key Reference Id</param>
        /// <returns>List Page</returns>
        public ActionResult List(int id)
        {
            MetaMasterViewModel metaMasterVM = null;

            if (id > 0)
            {
                var metaMaster = MetaDataManager.GetMetaValueList(id);

                //Populating ViewModel using Business Entity
                if (metaMaster != null && metaMaster.MetaValues.Count() > 0)
                {
                    metaMasterVM = new MetaMasterViewModel();
                    metaMasterVM.Id = id;
                    metaMasterVM.KeyReference = metaMaster.KeyReference;
                    metaMaster.MetaValues.ForEach(p => metaMasterVM.MetaValueList.Add(
                                                                                       new MetaValueViewModel
                                                                                       {
                                                                                           Id = p.Id,
                                                                                           Value = p.Value
                                                                                       }));
                }
                else
                {
                    metaMaster = MetaDataManager.GetMetaMaster(id);
                    if (metaMaster != null && metaMaster.Id > 0)
                    {
                        metaMasterVM = new MetaMasterViewModel();
                        metaMasterVM.Id = metaMaster.Id;
                        metaMasterVM.KeyReference = metaMaster.KeyReference;
                    }
                }
            }
            metaMasterVM.ResponseMessage = (PageNotificationViewModel)TempData["Response"];
            return View(metaMasterVM);
        }

        /// <summary>
        /// Gets the Keyvaluemodel
        /// </summary>
        /// <param name="id">KeyReference Id</param>
        /// <returns>Create page</returns>
        public ActionResult Create(int id)
        {
            MetaValueViewModel metaValueVM = null;

            if (id > 0)
            {
                var metaMaster = MetaDataManager.GetMetaMaster(id);

                //Populating ViewModel using Business Entity
                if (metaMaster != null && metaMaster.Id > 0)
                {
                    metaValueVM = new MetaValueViewModel();
                    metaValueVM.MetaMasterId = metaMaster.Id;
                    metaValueVM.MetaMasterKeyReference = metaMaster.KeyReference;
                }
            }

            return View(metaValueVM);
        }

        /// <summary>
        /// Deletes the key values using id
        /// </summary>
        /// <param name="id">Key Value Id</param>
        /// <returns>Returns to MetaKeyvalues List Page</returns>
        public ActionResult Delete(int id)
        {
            ProcessResponse response = null;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (id > 0)
                response = MetaDataManager.DeleteMetaValue(id);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["Response"] = responseMessage;

            return RedirectToAction("List", new { id = response.RefId });
        }

        /// <summary>
        /// Updates the KeyValue
        /// </summary>
        /// <param name="metaValueVM">Key Value ViewModel</param>
        /// <returns>Returns to MetaKeyvalues List Page</returns>
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(MetaValueViewModel metaValueVM)
        {
            if (ModelState.IsValid)
            {
                if (metaValueVM != null && metaValueVM.Id > 0)
                {
                    PageNotificationViewModel responseMessage = new PageNotificationViewModel();
                    //Populating Business Entity using Updated ViewModel
                    MetaValue metaValue = new MetaValue()
                    {
                        Id = metaValueVM.Id,
                        MetaMaster = new MetaMaster()
                                                    {
                                                        Id = metaValueVM.MetaMasterId
                                                    },
                        Value = metaValueVM.Value,
                        UpdatedBy = User.UserId,
                    };

                    var response = MetaDataManager.UpdateMetaValue(metaValue);

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

            return RedirectToAction("List", new { id = metaValueVM.MetaMasterId });
        }

        /// <summary>
        /// Creates a KeyValue under the KeyReference
        /// </summary>
        /// <param name="metaValueVM">KeyValue ViewModel</param>
        /// <returns>Returns to MetaKeyvalues List Page</returns>
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(MetaValueViewModel metaValueVM)
        {
            if (ModelState.IsValid)
            {
                if (metaValueVM != null && metaValueVM.Id > 0)
                {
                    PageNotificationViewModel responseMessage = new PageNotificationViewModel();
                    //Populating Business Entity using Updated ViewModel
                    MetaValue metaValue = new MetaValue()
                    {
                        MetaMaster = new MetaMaster
                                                    {
                                                        Id = metaValueVM.MetaMasterId
                                                    },
                        Value = metaValueVM.Value,
                        UpdatedBy = User.UserId,
                    };

                    var response = MetaDataManager.CreateMetaValue(metaValue);
                    if (response.Status == ResponseStatus.Success)
                    {
                        responseMessage.AddSuccess(response.Message);
                    }
                    else
                    {
                        responseMessage.AddError(response.Message);
                    }
                    TempData["Response"] = responseMessage;

                    if (response.Object.Id > 0)
                    {
                        metaValueVM.Id = response.Object.Id;
                    }
                }
            }

            return RedirectToAction("List", new { id = metaValueVM.MetaMasterId });
        }


    }
}
