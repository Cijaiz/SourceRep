using C2C.Core.Constants.Hub;
using System.Collections.Generic;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class UserFeedStatus
    {
        public int UserId { get; set; }
        public List<int> FeedId { get; set; }
        public FeedStatus FeedStatus { get; set; }
    }
}
