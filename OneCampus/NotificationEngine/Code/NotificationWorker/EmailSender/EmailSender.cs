namespace NotificationWorker.Engine.EmailSender
{
    using System;
    using System.Net.Mail;   
    using Octane.NotificationEngineInterfaces;
    using Octane.NotificationUtility;
    using Octane.NotificationWorker.Engine;
    using C2C.Core.Helper;
    using C2C.BusinessEntities.NotificationEntities.MailingEntities;    

    /// <summary>
    ///Methods For Sending Emails.
    /// </summary>
    public class EmailSender : ISender
    {
        public EmailSender()
        {
            CreateClientFactory = CreateDefaultClientFactory();
            CreateMailMessageFactory = CreateDefaultMailMessageFactory();
        }

        public Func<ISmtpClient> CreateClientFactory { get; set; }

        public Func<EmailMessage, MailMessage> CreateMailMessageFactory { get; set; }

        /// <summary>
        /// Sending email
        /// </summary>
        public void Send(EmailMessage email)
        {
            Guard.IsNotNull(email, "email");

            using (var message = CreateMailMessageFactory(email))
            {
                using (var client = CreateClientFactory())
                {
                    client.Send(message);
                }
            }
        }

        /// <summary>
        /// Creates Smtp Client
        /// </summary>
        private static Func<ISmtpClient> CreateDefaultClientFactory()
        {
            return () => new SmtpClientWrapper(new SmtpClient());
        }

        /// <summary>
        /// Creates Smtp Mail Message
        /// </summary>
        private static Func<EmailMessage, MailMessage> CreateDefaultMailMessageFactory()
        {
            return email =>
                       {
                           var message = new MailMessage { From = new MailAddress(email.From), Subject = email.Subject };

                           if (!string.IsNullOrEmpty(email.Sender))
                           {
                               message.Sender = new MailAddress(email.Sender);
                           }

                           email.To.Each(to => message.To.Add(to));
                           email.ReplyTo.Each(to => message.ReplyToList.Add(to));
                           email.CC.Each(cc => message.CC.Add(cc));
                           email.Bcc.Each(bcc => message.Bcc.Add(bcc));
                           email.Headers.Each(pair => message.Headers[pair.Key] = pair.Value);

                           if (!string.IsNullOrEmpty(email.MessageBody))
                           {
                               message.Body = email.MessageBody;
                               message.IsBodyHtml = true;
                           }

                           return message;
                       };
        }
    }
}
