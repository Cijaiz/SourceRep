using C2C.BusinessEntities.NotificationEntities.MailingEntities;
using C2C.Core.Logger;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MailingService
{
    public class MailingService : IMailingService
    {
        public SendMailResponse SendingEmail(EmailMessage message)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            SendMailResponse responce = null;
            try
            {
                MailMessage mailMessage = ConstructMessage(message);

                using (SmtpClient client = GetSmtpClientInstance())
                {
                    client.Send(mailMessage);
                    responce = new SendMailResponse() { Status = true, Message = "Mail sent" };
                }

                Logger.Debug("E-Mail sent successfully at " + DateTime.UtcNow.ToShortTimeString() + "from " + message.From);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException failedRecipientException)
            {
                responce = new SendMailResponse() { Status = false, Message = string.Format("SmtpFailedRecipientException occurred:{0}", failedRecipientException.Message) };
                //failed recipients
                Logger.Error(string.Format("{0} \n {1}", failedRecipientException.Message, failedRecipientException.StackTrace), TraceLevel.Error.ToString());
            }
            catch (System.Net.Mail.SmtpException smtpException)
            {
                responce = new SendMailResponse() { Status = false, Message = string.Format("SmtpException occurred:{0}", smtpException.Message) };
                //Mail exceeds limit
                Logger.Error(string.Format("{0} \n {1}", smtpException.Message, smtpException.StackTrace), TraceLevel.Error.ToString());
            }
            catch (Exception generalException)
            {
                responce = new SendMailResponse() { Status = false, Message = string.Format("Exception occurred:{0}", generalException.Message) };
                Logger.Error(string.Format("{0} \n {1}", generalException.Message, generalException.StackTrace), TraceLevel.Error.ToString());
            }


            return responce;
        }

        /// <summary>
        /// Creates SMTP client instance
        /// </summary>
        private SmtpClient GetSmtpClientInstance()
        {
            SmtpClient client = new SmtpClient()
            {
                DeliveryMethod = SmtpSetting.SmtpDeliveryMethod,
                Host = SmtpSetting.Host,
                Port = SmtpSetting.Port,
                UseDefaultCredentials =  SmtpSetting.UseDefaultCredentials,
                Credentials = SmtpSetting.Credential,
                EnableSsl = SmtpSetting.EnableSsl
            };

            return client;
        }

        /// <summary>
        /// Creates SMTP Mail Message
        /// </summary>
        private MailMessage ConstructMessage(EmailMessage notificationMsg)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(notificationMsg.From);
            message.Subject = notificationMsg.Subject;
            message.Body = notificationMsg.MessageBody;

            if (!string.IsNullOrEmpty(notificationMsg.Sender))
            {
                message.Sender = new MailAddress(notificationMsg.Sender);
            }

            if (notificationMsg.To != null && notificationMsg.To.Count > 0)
                notificationMsg.To.ToList().ForEach(to => message.To.Add(to));

            if (notificationMsg.ReplyTo != null && notificationMsg.ReplyTo.Count > 0)
                notificationMsg.ReplyTo.ToList().ForEach(replyTo => message.ReplyToList.Add(replyTo));

            if (notificationMsg.CC != null && notificationMsg.CC.Count > 0)
                notificationMsg.CC.ToList().ForEach(cc => message.CC.Add(cc));

            if (notificationMsg.Bcc != null && notificationMsg.Bcc.Count > 0)
                notificationMsg.Bcc.ToList().ForEach(bcc => message.Bcc.Add(bcc));

            if (notificationMsg.Headers != null && notificationMsg.Headers.Count > 0)
                notificationMsg.Headers.ToList().ForEach(header => message.Headers[header.Key] = header.Value);

            if (notificationMsg.Attachments != null && notificationMsg.Attachments.Count > 0)
            {
                string attachedFileName = string.Empty;
                string contentType = string.Empty;
                notificationMsg.Attachments.ToList().ForEach(
                    blobUrl =>
                    {
                        Stream mailingContents = AzureHelper.DownloadStreamFromBlob(blobUrl, out attachedFileName, out contentType);
                        if (mailingContents == null)
                            throw new Exception();
                        else
                        {
                            Attachment attachment = new Attachment(mailingContents,
                                                                    new ContentType(contentType)
                                                                   );
                            attachment.Name = attachedFileName;
                            message.Attachments.Add(attachment);
                        }
                    }
                );
            }

            message.IsBodyHtml = true;

            return message;

        }
    }
}
