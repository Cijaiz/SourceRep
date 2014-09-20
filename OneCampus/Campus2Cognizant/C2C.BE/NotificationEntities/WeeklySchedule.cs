using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class WeeklySchedule
    {
        public string Subject { get; set; }
        public List<BlogPost> blogPost { get; set; }
    }
}
