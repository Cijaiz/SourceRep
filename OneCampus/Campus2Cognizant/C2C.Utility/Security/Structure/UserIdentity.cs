namespace C2C.Core.Security.Structure
{
    #region Reference

    using System.Web.Security;
    using Newtonsoft.Json;
    
    #endregion

    /// <summary>
    /// This Class implements FormsIdentity along with Custom property UserContext.
    /// </summary>
    public class UserIdentity : FormsIdentity
    {
        /// <summary>
        /// This variable is to hold Forms authentication ticket private to this class.
        /// </summary>
        private FormsAuthenticationTicket ticket;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        /// <param name="ticket">Forms Authentication Ticket</param>
        public UserIdentity(FormsAuthenticationTicket ticket)
            : base(ticket)
        {
            this.ticket = ticket;

            // De-Serialize the UserContext
            this.Context = Helper.SerializationHelper.JsonDeserialize<UserContext>(ticket.UserData);
        }

        /// <summary>
        /// Gets Context information of logged in user.
        /// </summary>
        public UserContext Context { get; private set; }
    }
}
