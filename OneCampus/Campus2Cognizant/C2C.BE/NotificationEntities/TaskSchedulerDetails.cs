using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class TaskSchedulerDetails : TableEntity
    {
        public TaskSchedulerDetails() : base() { }
        public TaskSchedulerDetails(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }
        public string TaskId { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public string ErrorDescription { get; set; }
    }
}
