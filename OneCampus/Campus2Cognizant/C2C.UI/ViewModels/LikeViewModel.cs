using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;

namespace C2C.UI.ViewModels
{
    public class LikeViewModel
    {
        public int ContentTypeId { get; set; }
        public int ContentId { get; set; }

        public int UserCount { get; set; }
        public List<UserProfile> UsersList { get; set; }
    }
}