using C2C.BusinessEntities.C2CEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C2C.UI.ViewModels
{
    /// <summary>
    /// AddUserToRoleViewModel containing the list of all permisssions available and ones selected by the user
    /// </summary>
    public class AddUserToRoleViewModel
    {
        public List<UserProfile> UserProfiles { get; set; }
        [Required(ErrorMessage = "Select at least one user")]
        public List<int> SelectedUserIds { get; set; }	
    }
}