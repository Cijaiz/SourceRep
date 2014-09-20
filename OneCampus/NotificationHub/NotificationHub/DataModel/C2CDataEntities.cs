using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NotificationHub.DataModel
{
    public partial class C2CDataEntities : DbContext
    {
        /// <summary>
        /// Initialize a new NotificationHubEntities object.
        /// </summary>
        public C2CDataEntities(string connectionString)
            : base(connectionString)
        {
        }
    }
}