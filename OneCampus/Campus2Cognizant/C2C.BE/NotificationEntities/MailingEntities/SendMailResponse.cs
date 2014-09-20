using System.Runtime.Serialization;

namespace C2C.BusinessEntities.NotificationEntities.MailingEntities
{
    [DataContract]
    public class SendMailResponse
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool Status { get; set; }
    }
}
