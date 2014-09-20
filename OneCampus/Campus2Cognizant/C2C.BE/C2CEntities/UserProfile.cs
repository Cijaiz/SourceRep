using C2C.Core.Constants.C2CWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
    public class UserProfile : Audit
    {
        public UserProfile()
        {
            UserSetting = new UserSetting() { NotificationSetting = NotificationSetting.WithinCollege };
        }
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "FirstName Max Length is 50")]
        [RegularExpression("([a-zA-Z ]+)", ErrorMessage = "Special Characters and Numbers are not Allowed")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "LastName Max Length is 50")]
        [RegularExpression("([a-zA-Z ]+)", ErrorMessage = "Special Characters and Numbers are not Allowed")]
        public string LastName { get; set; }

        [Display(Name = "Email ID")]
        [Required]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Your email address is not in a valid format. Example of correct format: joe.example@example.org")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Email ID Max Length is 100")]
        public string Email { get; set; }
        public int? CollegeId { get; set; }
                
        [StringLength(250, ErrorMessage = "ProfilePhoto Max Length is 250")]
        public string ProfilePhoto { get; set; }

        public Status Status { get; set; }
        public UserSetting UserSetting { get; set; }
    }
}
