using System.ComponentModel.DataAnnotations;
namespace C2C.BusinessEntities.NotificationEntities
{
    public class OnlineUserStat
    {
        [Display(Name="Online user count")]
        public int OnlineUserCount { get; set; }

        [Display(Name = "Offline user count")]
        public int OfflineUserCount { get; set; }

        [Display(Name = "Total user count")]
        public int TotalUserCount { get; set; }
    }
}
