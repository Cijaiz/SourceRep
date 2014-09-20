using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace C2C.BusinessEntities.NotificationEntities
{
   
    public class PublisherEvents
    {
        public string EventId { get; set; }      
        public string EventCode { get; set; }     
        public string TaskId { get; set; }      
        public string NotificationContent { get; set; }
    }
}
