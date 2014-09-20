namespace C2C.Core.Security.Extensions
{
    #region Reference

    using System.Security.Principal;
    using System.Web.Security;
    using C2C.Core.Security.Structure;
    
    #endregion

    /// <summary>
    /// Identity Extensions
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Extension method to convert IIdentity to UserIdentity
        /// </summary>
        /// <param name="identity">Logged in user's IIdentity</param>
        /// <returns>User Identity</returns>
        public static UserIdentity ToUserIdentity(this IIdentity identity)
        {
            var formsIdentity = (FormsIdentity)identity;
            return new UserIdentity(formsIdentity.Ticket);
        }
    }
}
