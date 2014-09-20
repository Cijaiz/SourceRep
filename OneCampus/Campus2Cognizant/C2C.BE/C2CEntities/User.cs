namespace C2C.BusinessEntities.C2CEntities
{
    #region Reference
    using C2C.Core.Constants.C2CWeb;
    using System;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class User : Audit
    {
        public User()
        {
            Profile = new UserProfile();
            Group = new Group();
            Role = new Role();
        }
        public int Id { get; set; }

        [Required]
        [RegularExpression("([a-zA-Z0-9 ]+)", ErrorMessage = "Enter only alphabets and numbers for UserName")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string UserName { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
       
        public string PasswordSalt { get; set; }
       
        public int RetryAttempt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? LastBadLogon { get; set; }
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AccountValidity { get; set; }
       
        public Status Status { get; set; }
        public UserProfile Profile { get; set; }

        public Group Group { get; set; }
        public Role Role { get; set; }
    }
}
