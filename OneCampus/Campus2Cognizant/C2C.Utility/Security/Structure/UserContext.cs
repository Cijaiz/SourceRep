namespace C2C.Core.Security.Structure
{
    #region Reference

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// UserContext class is to hold user's context information which can be Serialized
    /// </summary>
    [Serializable]
    public class UserContext
    {
        /// <summary>
        /// Gets or sets First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets Last logon
        /// </summary>
        public DateTime? LastLogon { get; set; }

        /// <summary>
        /// Gets or sets Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets Permissions
        /// </summary>
        public List<int> Permissions { get; set; }

        /// <summary>
        /// Gets or sets Photo Path
        /// </summary>
        public string PhotoPath { get; set; }

        /// <summary>
        /// Gets or sets User Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        public string UserName { get; set; }
    }
}
