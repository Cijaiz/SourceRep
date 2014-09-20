using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace C2C.UI.ViewModels
{
    /// <summary>
    /// RoleViewModel defining role and set of it permissions
    /// </summary>
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Role Name Is Required")]
        [StringLength(50, ErrorMessage = "Role Name should be less than 50 characters", MinimumLength = 1)]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers for role name")]
        public string Name { get; set; }

        public List<Permission> Rights { get; set; }

        public int RoleId { get; set; }

        public List<int> SelectedPermissionIds { get; set; }
    }
}