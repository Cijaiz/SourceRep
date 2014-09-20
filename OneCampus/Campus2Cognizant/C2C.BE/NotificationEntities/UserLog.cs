using System;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class UserLog
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public bool IsMobileDevice { get; set; }
        public DateTime LoggedOn { get; set; }
        public DateTime LastActivityOn { get; set; }
        public string IPAddress { get; set; }
    }
}
