namespace C2C.Core.Security
{
    #region Reference

    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;
    using C2C.Core.Security.Structure;

    #endregion

    /// <summary>
    /// Forms authentication provider.
    /// </summary>
    public sealed class FormsAuthenticationProvider
    {
        /// <summary>
        /// This method is to set forms authentication cookie.
        /// </summary>
        /// <param name="userName">Logged in user's name</param>
        /// <param name="userContext">Extra data UserContext that needs to persists in authentication token.</param>
        /// <param name="createPersistentCookie">Whether to persists forms authentication cookie </param>
        public static void SetAuthCookie(string userName, UserContext userContext, bool createPersistentCookie)
        {
            // serialize the UserContext to persists in session cookies
            string userData = Helper.SerializationHelper.JsonSerialize(userContext);

            HttpContext context = HttpContext.Current;
            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: userName,
                issueDate: DateTime.UtcNow,
                expiration: DateTime.UtcNow.AddMinutes(20),
                isPersistent: createPersistentCookie,
                userData: userData);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            context.Response.Cookies.Add(formsCookie);
        }

        /// <summary>
        /// To set current user principal to CustomPrincipal with added information from Authentication cookie.
        /// </summary>
        public static void SetCurrentPrinciple()
        {
            // Get the authentication cookie
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            // If the cookie can't be found, don't issue the ticket
            if (authCookie == null)
            {
                return;
            }

            // Get the authentication ticket and rebuild the principal 
            // & identity
            FormsAuthenticationTicket authTicket =
              FormsAuthentication.Decrypt(authCookie.Value);

            CustomPrincipal userPrincipal = new CustomPrincipal(authTicket);
            HttpContext.Current.User = userPrincipal;
        }

        /// <summary>
        /// Forms authentication sign out.
        /// </summary>
        public static void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Update authentication cookie with userContext information.
        /// </summary>
        /// <param name="userName">Logged in user's name</param>
        /// <param name="userContext">Extra data UserContext that needs to persists in authentication token.</param>
        /// <param name="createPersistentCookie">Whether to persists forms authentication cookie </param>
        public static void UpdateAuthCookie(string userName, UserContext userContext, bool createPersistentCookie)
        {
            // serialize the UserContext to persists in session cookies
            string userData = Helper.SerializationHelper.JsonSerialize(userContext);
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            FormsAuthenticationTicket newticket = new FormsAuthenticationTicket(
                ticket.Version,
                ticket.Name,
                ticket.IssueDate,
                ticket.Expiration,
                isPersistent: ticket.IsPersistent,
                userData: userData,
                cookiePath: ticket.CookiePath);

            // Encrypt the ticket and store it in the cookie
            cookie.Value = FormsAuthentication.Encrypt(newticket);

            context.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// It generates random password.
        /// </summary>
        /// <returns>Generated password.</returns>
        public static string GeneratePassword()
        {
            // Generates a random password of the specified length.
            return Membership.GeneratePassword(8, 1);
        }
    }
}
