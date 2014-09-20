using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.ViewModels
{
    public class UserListViewModel
    {
        public UserListViewModel()
        {
            User = new List<User>();
            Filter = new UserListFilter();
        }

        public List<User> User { get; set; }
        public UserListFilter Filter { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
        public int PageCount { get; set; }
    }

    public class UserListFilter
    {
        public string Name { get; set; }
        [Display(Name="Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
    }   
}