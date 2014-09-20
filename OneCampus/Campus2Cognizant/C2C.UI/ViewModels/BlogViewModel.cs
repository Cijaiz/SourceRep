using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace C2C.UI.ViewModels
{
    public class BlogViewModel
    {
        [Display(Name = "Blog Category")]
        public string Category { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        [Display(Name = "Group")]
        //public bool Group { get; set; }
        public List<Group> GroupList { get; set; }
        public BlogPost BlogPost { get; set; }
        public List<int> SelectedGroupIds { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }       
    }

    public class BlogCategory
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}