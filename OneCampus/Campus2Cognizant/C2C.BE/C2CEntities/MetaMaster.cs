using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
    public class MetaMaster
    {
        public MetaMaster()
        {
            MetaValues = new List<MetaValue>();
        }

        public short Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "KeyReference Max Length is 50")]
        public string KeyReference { get; set; }

        [StringLength(100, ErrorMessage = "Description Max Length is 100")]
        public string Description { get; set; }
        public List<MetaValue> MetaValues { get; set; }
    }
}
