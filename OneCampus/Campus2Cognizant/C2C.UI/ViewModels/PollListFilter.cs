using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;

namespace C2C.UI.ViewModels
{
    public class PollListFilter
    {
        public string SearchTitle { get; set; }
        //[Display(Name = "Status")]
        //public string Status { get; set; }
        //public List<SelectListItem> StatusList { get; set; }
    }
}