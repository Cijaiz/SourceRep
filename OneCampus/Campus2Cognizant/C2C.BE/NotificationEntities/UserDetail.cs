using C2C.Core.Constants.C2CWeb;
using C2C.BusinessEntities.C2CEntities;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class UserDetail
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public NotificationSetting NotificationSetting { get; set; }
    }
}
