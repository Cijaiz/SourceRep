
using System.Collections.Generic;
namespace C2C.UI.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public string CommentDescription { get; set; }
        public string CommentedBy { get; set; }
        public string ProfilePhoto { get; set; }
        public bool ManageComment { get; set; }
    }

    public class CommentListViewModel
    {
        public List<CommentViewModel> CommentListModel { get; set; }

        public int ContentTypeId { get; set; }
        public int ContentId { get; set; }
        public int CommentCount { get; set; }
        public int PageCount { get; set; }
    }
}