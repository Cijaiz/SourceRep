
using C2C.BusinessEntities.NotificationEntities.MailingEntities;
namespace Octane.NotificationEngineInterfaces
{
    /// <summary>
    /// Methods for building and sending email 
    /// </summary>
    public interface ISender
    {
        void Send(EmailMessage email);
    }
}