using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.C2CEntities
{
    public class Role : Audit
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name Max Length is 50")]
        public string Name { get; set; }
        public List<int> PermissionsIDs { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
