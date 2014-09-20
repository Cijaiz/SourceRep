using System.Collections.Generic;
using System.Runtime.Serialization;

namespace C2C.BusinessEntities.NotificationEntities.MailingEntities
{
    /// <summary>
    /// Creates a Email Message that sends Email
    /// </summary>  
    [DataContract(Namespace = "urn:sample", Name = "EmailMessage")]
    public class EmailMessage
    {
        public EmailMessage()
        {
            To = new List<string>();
            ReplyTo = new List<string>();
            CC = new List<string>();
            Bcc = new List<string>();
            Attachments = new List<string>();
            Headers = new Dictionary<string, string>();
        }
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string MessageBody { get; set; }
        [DataMember]
        public string MessageType { get; set; }
        [DataMember]
        public ICollection<string> CC { get; private set; }
        [DataMember]
        public ICollection<string> Bcc { get; private set; }
        [DataMember]
        public ICollection<string> To { get; set; }
        [DataMember]
        public ICollection<string> ReplyTo { get; private set; }
        [DataMember]
        public ICollection<string> Attachments { get; private set; }
        [DataMember]
        public IDictionary<string, string> Headers { get; private set; }
    }
}
