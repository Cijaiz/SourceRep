#region References
using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#endregion

namespace C2C.UI.ViewModels
{
    public class ShareViewModel
    {
        public int ContentTypeId { get; set; }
        public int ContentId { get; set; }
        
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for Title")]
        public string ContentTitle { get; set; }
        
        public List<UserProfile> UsersList { get; set; }
        public string SharedTo { get; set; }

        public string Description { get; set; }
        public int UserCount { get; set; }
    }
}