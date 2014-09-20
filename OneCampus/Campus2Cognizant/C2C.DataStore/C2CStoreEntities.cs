using System.Data.Entity;

namespace C2C.DataStore
{
    public partial class C2CStoreEntities : DbContext
    {
        /// <summary>
        /// Initialize a new NotificationHubEntities object.
        /// </summary>
        public C2CStoreEntities(string connectionString)
            : base(connectionString)
        {
        }
    }
}
