#region References
using System.ComponentModel.DataAnnotations;
#endregion

namespace C2C.BusinessEntities.C2CEntities
{
    public class Group : Audit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Maximum Length is 25")]
        [RegularExpression("([a-zA-Z0-9 @]*$)", ErrorMessage = "Can enter only alphabets, numbers and '@' for Title")]
        public string Title { get; set; }

        public bool IsCollege { get; set; }
        public int UserId { get; set; }
    }
}
