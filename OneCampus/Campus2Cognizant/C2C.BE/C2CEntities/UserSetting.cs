using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
    public class UserSetting : Audit
    {
        public int Id { get; set; }
        public short NotificationSettingId { get; set; }
        [Required]
        public NotificationSetting NotificationSetting { get; set; }
    }

    public enum NotificationSetting : short
    {
        WithinCollege = 1,
        OutsideCollege = 2,
        DoNotDisturb = 4
    }
}
