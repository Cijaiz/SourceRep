using C2C.BusinessEntities.NotificationEntities.MailingEntities;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace MailingService
{
    [ServiceContract]
    public interface IMailingService
    {
        [OperationContract]
        [WebInvoke(Method = "POST")]
        SendMailResponse SendingEmail(EmailMessage message);
    }
}
