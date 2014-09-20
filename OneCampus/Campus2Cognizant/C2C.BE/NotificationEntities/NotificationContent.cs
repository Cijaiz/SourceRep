using C2C.Core.Constants.C2CWeb;
using System;
using System.Collections.Generic;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class NotificationContent
    {
        public int Id { get; set; }

        public string Description { get; set; }
        public string URL { get; set; }
        public Module ContentType { get; set; }
        public string ContentTitle { get; set; }

        public int SharedBy { get; set; }
        public bool IsRead { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public List<int> Users { get; set; }
        public List<int> Groups { get; set; }

        public string Subject { get; set; }
    }
}
