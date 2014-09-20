using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;

namespace C2C.Core.Helper.AzureHelper
{
    public sealed class Table
    {
        private CloudStorageAccount cloudStorageAccount = null;
        private CloudTableClient cloudTableClient = null;

        private Table(string storageConnectionString)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            IRetryPolicy exponentialRetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);
            cloudTableClient.RetryPolicy = exponentialRetryPolicy;
        }

        public static Table GetInstance(string storageConnectionString)
        {
            return new Table(storageConnectionString);
        }

        public bool Create(string tableName)
        {
            try
            {
                CloudTable table = cloudTableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {
                    return false;
                }
                throw;
            }
        }

        /// <summary>
        ///Create Task
        /// <param name="tableName">tableName</param>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns></returns>
        /// </summary>
        public void InsertRow<T>(T rowEntity, string tableName) where T : TableEntity
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            cloudTable.Execute(TableOperation.Insert(rowEntity));
        }

        /// <summary>
        /// Query Tasks
        /// <param name="tableName">tableName</param>
        /// <returns>Collection of entities </returns>
        /// </summary>
        public IEnumerable<T> Query<T>(string tableName, TableQuery<T> query) where T : TableEntity, new()
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            return cloudTable.ExecuteQuery<T>(query);
        }
    }
}
