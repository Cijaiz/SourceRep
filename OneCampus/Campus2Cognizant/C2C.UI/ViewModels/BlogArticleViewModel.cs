using C2C.BusinessEntities.C2CEntities;

namespace C2C.UI.ViewModels
{
    public class BlogArticleViewModel
    {
        public BlogPost Post { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
        public int shareCount { get; set; }
        public int commentCount { get; set; }
        public int SharedPageCount { get; set; }
    }
}