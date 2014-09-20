#region References
using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace C2C.BusinessEntities
{
    public class ContentShare
    {
        public int Id { get; set; }
        public int SharedBy { get; set; }
        public DateTime SharedOn { get; set; }

        public int ContentTypeId { get; set; }
        public int ContentId { get; set; }

        [StringLength(150, ErrorMessage = "Description Max Length is 150")]
        public string Description { get; set; }
    }
}
