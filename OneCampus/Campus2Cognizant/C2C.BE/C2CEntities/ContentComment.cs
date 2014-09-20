using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using C2C.Core.Constants.C2CWeb;

namespace C2C.BusinessEntities.C2CEntities
{
    public class ContentComment : Audit
    {
        public int Id { get; set; }
        public int CommentedBy { get; set; }
        public System.DateTime CommentedOn { get; set; }
       
        public short ContentTypeId { get; set; }
        public int ContentId { get; set; }
        
        [Required]
        [StringLength(550, ErrorMessage = "Comment Max Length is 550")]
        public string Comment { get; set; }
        
        public Status Status { get; set; }

        public UserProfile User { get; set; }
    }
}
