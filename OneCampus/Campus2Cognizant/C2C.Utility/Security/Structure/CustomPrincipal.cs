namespace C2C.Core.Security.Structure
{
    #region Reference
    
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web.Security;
    using C2C.Core.Security.Extensions;
    using C2C.Core.Constants.C2CWeb;

    #endregion Reference

    /// <summary>
    /// This class implements ICustomPrincipal
    /// </summary>
    public class CustomPrincipal : C2C.Core.Security.Skeleton.ICustomPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPrincipal"/> class.
        /// </summary>
        /// <param name="ticket">Forms Authentication Ticket</param>
        public CustomPrincipal(FormsAuthenticationTicket ticket)
        {
            this.Identity = new UserIdentity(ticket);
            var userIdentity = this.Identity.ToUserIdentity();
            this.UserId = userIdentity.Context.UserId;
            this.FirstName = userIdentity.Context.FirstName;
            this.LastName = userIdentity.Context.LastName;
            this.PhotoPath = userIdentity.Context.PhotoPath;
            this.LastLogon = userIdentity.Context.LastLogon;
            this.Permissions = userIdentity.Context.Permissions;
        }

        /// <summary>
        /// Gets First name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets Identity
        /// </summary>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Gets Last Logon
        /// </summary>
        public DateTime? LastLogon { get; private set; }

        /// <summary>
        /// Gets Last name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets Photo Path
        /// </summary>
        public string PhotoPath { get; private set; }

        /// <summary>
        /// Gets User Id
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets or sets Permissions
        /// </summary>
        private List<int> Permissions { get; set; }

        /// <summary>
        /// Returns whether user has permission.
        /// </summary>
        /// <param name="appPermission">The application permission checked against user's permission.</param>
        /// <returns>True if user has permission otherwise false.</returns>
        public bool HasPermission(ApplicationPermission appPermission)
        {
            return this.Permissions != null ? this.Permissions.Exists(a => a == (int)appPermission) : false;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
        public bool IsInRole(string role)
        {
            return false;
        }
    }
}