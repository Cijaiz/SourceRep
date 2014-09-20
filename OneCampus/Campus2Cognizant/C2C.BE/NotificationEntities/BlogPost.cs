using System;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class BlogPost
    {
        public int Id { get; set; }      
        public string Title { get; set; }      
        public string Description { get; set; }      
        public string PostContent { get; set; }
        public string Author { get; set; }       
        public DateTime VisibleFrom { get; set; }     
        public DateTime? VisibleTill { get; set; }
        public string URL { get; set; }
        public DateTime PublishedOn { get; set; }
        public string PublishedBy { get; set; }
      
    }
}
