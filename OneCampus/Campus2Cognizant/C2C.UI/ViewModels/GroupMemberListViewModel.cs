#region References
using C2C.Core.Constants.C2CWeb;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
#endregion

namespace C2C.UI.ViewModels
{
    public class GroupMemberListViewModel
    {
        public GroupMemberListViewModel()
        {
            GroupMembers = new List<GroupMemberViewModel>();
        }

        public int Id { get; set; }
        public int GroupId { get; set; }
        public List<GroupMemberViewModel> GroupMembers { get; set; }
        
        public List<int> SelectedUserIds { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public Status Status { get; set; }

        public string SearchText { get; set; }
        public dynamic Pager { get; set; }
        public int UserPageCount { get; set; }

        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for Title")]
        public string Title { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
        public string memberStatus { get; set; }
    }

    public class GroupMemberViewModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string GroupTitle { get; set; }
        public string UserImage { get; set; }
        public Status Status { get; set; }

        public DateTime RequestedOn { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool IsChecked { get; set; }
    }
}