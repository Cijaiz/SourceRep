using System;
using System.Collections.Generic;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class PollPublish
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public IList<string> ToAddress { get; set; }
    }
}
