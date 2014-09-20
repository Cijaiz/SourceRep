using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class DynamicLoadHistoryEntity : TableEntity
    {
        public DynamicLoadHistoryEntity() : base() { }
        public DynamicLoadHistoryEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
