using C2C.Core.Constants.C2CWeb;
using C2C.Core.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
    public class BlogPost : Audit
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(150, ErrorMessage = "Title Max Length is 150")]
        //[RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for Title")]
        public string Title { get; set; }
        
        [Required]
        [StringLength(200, ErrorMessage = "Description Max Length is 200")]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Content")]
        public string PostContent { get; set; }

        [StringLength(50, ErrorMessage = "Author Max Length is 50")]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for Author")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Visible From")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime VisibleFrom { get; set; }

        [Display(Name = "Visible Till")]
        [DataType(DataType.Date)]
        [CompareDate("VisibleFrom", true, CompareDate.Type.Greater)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? VisibleTill { get; set; }

        public bool IsArchived { get; set; }
        public bool Notify { get; set; }
        public int BlogCategory { get; set; }
        
        public Status Status { get; set; }
        public List<int> GroupIDs { get; set; }
    }
}
