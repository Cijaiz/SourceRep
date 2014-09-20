using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.ViewModels
{
    public class MetaValueViewModel
    {     
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Maximum Length is 25")]
        public string Value { get; set; }

        public short MetaMasterId { get; set; }
        public string MetaMasterKeyReference { get; set; }
    }
}