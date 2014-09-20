using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.ViewModels
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
            CollegeList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "FirstName Max Length is 50")]
        [RegularExpression("([a-zA-Z ]+)", ErrorMessage = "Alphabets only allowed")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "LastName Max Length is 50")]
        [RegularExpression("([a-zA-Z ]+)", ErrorMessage = "Alphabets only allowed")]
        public string LastName { get; set; }

        public string Email { get; set; }
        public int? CollegeId { get; set; }
        public string College { get; set; }

        public string ProfilePhoto { get; set; }
        public HttpPostedFileBase File { get; set; }

        public bool HasPermission { get; set; }
        public List<SelectListItem> CollegeList { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }
}

