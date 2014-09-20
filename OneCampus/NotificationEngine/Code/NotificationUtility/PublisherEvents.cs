using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Octane.NotificationUtility
{
    [DataContract]
    public class PublisherEvents
    {
        [DataMember]
        public string EventId { get; set; }
        [DataMember]
        public string EventCode { get; set; }
    }
}