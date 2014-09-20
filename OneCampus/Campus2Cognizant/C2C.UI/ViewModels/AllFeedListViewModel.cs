using C2C.BusinessEntities.NotificationEntities;
using System.Collections.Generic;

namespace C2C.UI.ViewModels
{
    public class AllFeedListViewModel
    {
        public List<NotificationContent> NotificationContents { get; set; }
        public string Error { get; set; }
        public dynamic Pager { get; set; }
    }
}