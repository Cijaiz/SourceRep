using System.ComponentModel.DataAnnotations;

namespace C2C.BusinessEntities.C2CEntities
{
    public class MetaValue : Audit
    {
        public MetaValue()
        {
            MetaMaster = new MetaMaster();
        }

        public int Id { get; set; }
        public MetaMaster MetaMaster { get; set; }
        
        [StringLength(50, ErrorMessage = "Value Max Length is 50")]
        public string Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}
