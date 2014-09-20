using Microsoft.WindowsAzure.Storage.Table;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class CHireImportLogEntity : TableEntity
    {
        public CHireImportLogEntity() : base() { }
        public CHireImportLogEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
