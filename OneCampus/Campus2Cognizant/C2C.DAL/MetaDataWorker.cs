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
    /// Performs Data manipulation operations on MetaMaster and MetaValue Entity Libraries.
    /// </summary>
    public class MetaDataWorker
    {       
        /// <summary>
        /// Creates a new instance for MetaDataWorker class
        /// </summary>
        /// <returns>Returns the new instance</returns>
        public static MetaDataWorker GetInstance()
        {
            return new MetaDataWorker();
        }

        /// <summary>
        /// Gets the MetaValue using MetaValue Id
        /// </summary>
        /// <param name="id">MetaValue Id</param>
        /// <returns>KeyValue</returns>
        public BE.MetaValue GetMetaValue(int id)
        {
            BE.MetaValue metaValue = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaValue = dbContext.MetaValues
                                        .Where(p => p.Id == id && p.IsDeleted == false)
                                        .FirstOrDefault();

                //Populating the MetaValue Business Entity.
                if (entityMetaValue != null && entityMetaValue.Id > 0)
                {
                    metaValue = new BE.MetaValue()
                    {
                        Id = entityMetaValue.Id,
                        MetaMaster = new BE.MetaMaster()
                        {
                            Id = entityMetaValue.MetaMaster.Id,
                            KeyReference = entityMetaValue.MetaMaster.KeyReference
                        },
                        Value = entityMetaValue.Value,
                        UpdatedBy = entityMetaValue.UpdatedBy.HasValue ? entityMetaValue.UpdatedBy.Value : entityMetaValue.CreatedBy,
                        UpdatedOn = entityMetaValue.UpdatedOn.HasValue ? entityMetaValue.UpdatedOn.Value : entityMetaValue.CreatedOn
                    };
                }
            }

            return metaValue;
        }

        /// <summary>
        /// Gets the MetaMaster using MetaMaster id
        /// </summary>
        /// <param name="id">MetaMaster Id</param>
        /// <returns>MetaMaster</returns>
        public BE.MetaMaster GetMetaMaster(int id)
        {
            BE.MetaMaster metaMaster = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaMaster = dbContext.MetaMasters
                                                .Where(c => c.Id == id)
                                                .FirstOrDefault();

                //Populating the MetaMaster Business Entity
                metaMaster = new BE.MetaMaster()
                {
                    Id = entityMetaMaster.Id,
                    KeyReference = entityMetaMaster.KeyReference,
                };
            }

            return metaMaster;
        }

        /// <summary>
        /// Gets all the MetaMasterList
        /// </summary>
        /// <returns>MetaMaster List</returns>
        public List<BE.MetaMaster> GetMetaMasterList()
        {
            List<BE.MetaMaster> metaMasters = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaMasters = dbContext.MetaMasters.ToList();

                if (entityMetaMasters != null && entityMetaMasters.Count() > 0)
                {
                    metaMasters = new List<BE.MetaMaster>();

                    //Populating the Business Entities with each data entity.
                    entityMetaMasters.ForEach(p => metaMasters.Add(
                                                                    new BE.MetaMaster()
                                                                    {
                                                                        Id = p.Id,
                                                                        KeyReference = p.KeyReference,
                                                                        Description = p.Description
                                                                    }));
                }
            }

            return metaMasters;
        }


        /// <summary>
        /// Deletes the MetaValue using Id
        /// </summary>
        /// <param name="id">MetaValue Id</param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse DeleteMetaValue(int id)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaValue = dbContext.MetaValues
                                        .Where(c => c.Id == id && c.IsDeleted == false)
                                        .FirstOrDefault();

                if (entityMetaValue != null && entityMetaValue.Id > 0)
                {
                    entityMetaValue.IsDeleted = true;

                    if (dbContext.SaveChanges() > 0)
                    {
                        //populating success Response
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Success,
                            Message = Message.DELETED,
                            RefId = entityMetaValue.MetaMasterId,
                        };

                    }
                    else
                    {
                        //populating failure Response if Delete fails
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Failed,
                            Message = string.Format(Message.FAILED),
                        };
                    }
                }
                else
                {
                    //populating Failure Response if No Record found with pacified id
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
        /// Gets the MetaValueList for Single MetaMaster using MetaMaster Id
        /// </summary>
        /// <param name="metaMasterId">Metamaster Id</param>
        /// <returns>Metamaster with its MetaValue List</returns>
        public BE.MetaMaster GetMetaValueList(int metaMasterId)
        {
            BE.MetaMaster metaMaster = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaValues = dbContext.MetaValues
                                         .Where(key => key.MetaMasterId == metaMasterId && key.IsDeleted == false)
                                         .ToList();

                if (entityMetaValues != null && entityMetaValues.Count() > 0)
                {
                    //Populating the Business Entities.
                    metaMaster = new BE.MetaMaster();

                    metaMaster.Id = entityMetaValues.First().MetaMasterId;
                    metaMaster.KeyReference = entityMetaValues.First().MetaMaster.KeyReference;

                    //Populate the Meta Key value list from each Data Entity.
                    entityMetaValues.ForEach(p => metaMaster.MetaValues.Add(
                                                                            new BE.MetaValue()
                                                                            {
                                                                                Id = p.Id,
                                                                                Value = p.Value,
                                                                                UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                                                                                UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                                                                            }));

                }
            }

            return metaMaster;
        }

        /// <summary>
        /// Updates the MetaValue Content for a MetaMaster.
        /// </summary>
        /// <param name="metaValue">MetaValue</param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse UpdateMetaValue(BE.MetaValue metaValue)
        {
            ProcessResponse response = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var entityMetaValue = dbContext.MetaValues
                                            .Where(c => c.Id == metaValue.Id && c.IsDeleted == false)
                                            .FirstOrDefault();

                //Updating the Data Entity with business Entity.
                if (entityMetaValue != null && entityMetaValue.Id > 0)
                {
                    entityMetaValue.MetaMaster.Id = metaValue.MetaMaster.Id;
                    entityMetaValue.Value = metaValue.Value;
                    entityMetaValue.UpdatedBy = metaValue.UpdatedBy;
                    entityMetaValue.UpdatedOn = DateTime.UtcNow;

                    int count = dbContext.SaveChanges();

                    if (count > 0)
                    {
                        //populating success Response
                        response = new ProcessResponse()
                                                        {
                                                            Status = ResponseStatus.Success,
                                                            Message = string.Format(Message.UPDATED_COUNT, count),
                                                        };
                    }
                    else
                    {
                        //populating Failure Response if Update fails
                        response = new ProcessResponse()
                                                        {
                                                            Status = ResponseStatus.Failed,
                                                            Message = string.Format(Message.FAILED),
                                                        };
                    }
                }
                else
                {
                    //populating Failure Response if No Record found with specified id
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
        /// Creates a MetaValue for a MetaMaster
        /// </summary>
        /// <param name="metaValue">MetaValue</param>
        /// <returns>Response with Created MetaValue</returns>
        public ProcessResponse<BE.MetaValue> CreateMetaValue(BE.MetaValue metaValue)
        {
            ProcessResponse<BE.MetaValue> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Passing the Business Entity Value to Data Entity.
                var entityMetaValue = new C2C.DataStore.MetaValue()
                {
                    MetaMasterId = metaValue.MetaMaster.Id,
                    Value = metaValue.Value,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = metaValue.UpdatedBy,
                    UpdatedOn = DateTime.UtcNow
                };

                dbContext.MetaValues.Add(entityMetaValue);

                //Checking whether the KeyValue is created in DB
                if (dbContext.SaveChanges() > 0)
                {
                    metaValue.Id = entityMetaValue.Id;

                    //populating success response
                    response = new ProcessResponse<BE.MetaValue>()
                    {
                        Status = ResponseStatus.Success,
                        Object = metaValue,
                        Message = Message.CREATED
                    };
                }
                else
                {
                    //populating failure response
                    response = new ProcessResponse<BE.MetaValue>()
                    {
                        Status = ResponseStatus.Success,
                        Object = metaValue,
                        Message = Message.FAILED
                    };
                }
            }

            return response;
        }

    }
}
