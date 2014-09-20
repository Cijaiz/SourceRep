using C2C.Core.Constants.C2CWeb;
using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
   public class Permission
    {
       public int Id { get; set; }

       [Required]
       [StringLength(50, ErrorMessage = "Name Max Length is 50")]
       public string  Name { get; set; }

       [StringLength(100, ErrorMessage = "Description Max Length is 100")]
       public string Description { get; set; }

       [Required]
       public Module ContentTypeId { get; set; }
    }
}
