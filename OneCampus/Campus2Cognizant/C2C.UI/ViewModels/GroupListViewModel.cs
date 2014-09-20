#region References
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion

namespace C2C.UI.ViewModels
{
    public class GroupListViewModel 
    {
        public GroupListViewModel()
        {
            GroupList = new List<GroupViewModel>();
        }
        public string SearchText { get; set; }

        public int PageCount { get; set; }
        public List<GroupViewModel> GroupList { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }

    public class GroupViewModel
    {
        public int Id { get; set; }
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for Title")]
        public string Title { get; set; }
        public bool IsCollege { get; set; }
    }
}