namespace C2C.Core.Security.Skeleton
{
    #region Reference

    using C2C.Core.Constants.C2CWeb;
    using System;
    using System.Security.Principal;
    
    #endregion Reference

    /// <summary>
    /// This interface implements IPrincipal along with custom properties and method.
    /// </summary>
    public interface ICustomPrincipal : IPrincipal
    {
        /// <summary>
        /// Gets First name
        /// </summary>
        string FirstName { get; }

        /// <summary>
        /// Gets Last Logon
        /// </summary>
        DateTime? LastLogon { get; }

        /// <summary>
        /// Gets Last Name
        /// </summary>
        string LastName { get; }

        /// <summary>
        /// Gets Photo Path
        /// </summary>
        string PhotoPath { get; }

        /// <summary>
        /// Gets User Id
        /// </summary>
        int UserId { get; }

        /// <summary>
        /// Returns whether user has permission.
        /// </summary>
        /// <param name="appPermission">The application permission checked against user's permission.</param>
        /// <returns>True if user has permission otherwise false.</returns>
        bool HasPermission(ApplicationPermission appPermission);
    }
}