using System;

namespace C2C.BusinessEntities.C2CEntities
{
    public class ContentLike
    {
        public int Id { get; set; }
        public int LikedBy { get; set; }
        public int ContentTypeId { get; set; }

        public int ContentId { get; set; }
        public DateTime LikedOn { get; set; }
    }
}
