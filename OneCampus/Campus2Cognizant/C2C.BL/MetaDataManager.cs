using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the MetaMaster and MetaValue and Uses MetaWorker to do Data manipulation
    /// </summary>
    public static class MetaDataManager
    {
        /// <summary>
        /// Gets the MetaValue using MetaValue Id
        /// </summary>
        /// <param name="id">MetaValue Id</param>
        /// <returns>MetaValue</returns>
        public static MetaValue GetMetaValue(int id)
        {
            var metaValue = DAL.MetaDataWorker.GetInstance().GetMetaValue(id);
            return metaValue;
        }
        
        /// <summary>
        /// Deletes the MetaValue using Id
        /// </summary>
        /// <param name="id">MetaValue Id</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse DeleteMetaValue(int id)
        {
            var response = DAL.MetaDataWorker.GetInstance().DeleteMetaValue(id);
            return response;
        }

        /// <summary>
        /// Gets the KeyReference using KeyReference id
        /// </summary>
        /// <param name="id">KeyReference Id</param>
        /// <returns>Key Reference</returns>
        public static MetaMaster GetMetaMaster(int id)
        {
            var metaMaster = DAL.MetaDataWorker.GetInstance().GetMetaMaster(id);
            return metaMaster;
        }

        /// <summary>
        /// Gets all the KeyReferenceList
        /// </summary>
        /// <returns>KeyReference List</returns>
        public static List<MetaMaster> GetMetaMasterList()
        {
            List<MetaMaster> metaMasters = DAL.MetaDataWorker.GetInstance().GetMetaMasterList();
            return metaMasters;
        }
               
        /// <summary>
        /// Gets the MetaValueList for Single KeyReference using keyReference Id
        /// </summary>
        /// <param name="metaMasterId">KeyReference Id</param>
        /// <returns>KeyReference with its MetaValue List</returns>
        public static MetaMaster GetMetaValueList(int metaMasterId)
        {
            MetaMaster metaMaster = DAL.MetaDataWorker.GetInstance().GetMetaValueList(metaMasterId);
            return metaMaster;
        }

        /// <summary>
        /// Updates the MetaValue Content for a KeyReference.
        /// </summary>
        /// <param name="metaValue">Key Value</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse UpdateMetaValue(MetaValue metaValue)
        {
            var response = DAL.MetaDataWorker.GetInstance().UpdateMetaValue(metaValue);
            return response;
        }

        /// <summary>
        /// Creates a MetaValue for a keyReference
        /// </summary>
        /// <param name="metaValue">Key Value</param>
        /// <returns>Response with Created MetaValue</returns>
        public static ProcessResponse<MetaValue> CreateMetaValue(MetaValue metaValue)
        {
            var response = DAL.MetaDataWorker.GetInstance().CreateMetaValue(metaValue);
            return response;
        }
    }
}
