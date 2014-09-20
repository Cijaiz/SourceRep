using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;

namespace C2C.UI.ViewModels
{
    public class RoleResponseViewModel
    {
        public List<Role> Roles { get; set; }
        public List<UserProfile> UserProfile { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }
}