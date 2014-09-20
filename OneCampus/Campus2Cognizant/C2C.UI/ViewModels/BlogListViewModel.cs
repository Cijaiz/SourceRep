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
    public class BlogListViewModel
    {
        public BlogListViewModel()
        {
            BlogPostList = new List<BlogPost>();
        }
        public List<BlogPost> BlogPostList { get; set; }
        public BlogListFilter Filter { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
        public int PageCount { get; set; }
    }


    public class BlogListFilter
    {
        public string SearchTitle { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
    }   

}