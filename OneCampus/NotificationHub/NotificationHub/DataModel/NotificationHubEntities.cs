using System.Data.Entity;

namespace NotificationHub.DataModel
{
    public partial class NotificationHubEntities : DbContext
    {
        /// <summary>
        /// Initialize a new NotificationHubEntities object.
        /// </summary>
        public NotificationHubEntities(string connectionString)
            : base(connectionString)
        {
        }

    }
}