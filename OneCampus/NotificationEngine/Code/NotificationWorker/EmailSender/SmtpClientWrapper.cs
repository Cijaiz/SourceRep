using Octane.NotificationWorker.Engine;
namespace NotificationWorker.Engine.EmailSender
{
    using System;
    using System.Diagnostics;
    using System.Net.Mail;
    using Octane.NotificationUtility;
    using C2C.Core.Helper;

    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient realClient;
        private bool disposed;

        public SmtpClientWrapper(SmtpClient realClient)
        {
            Guard.IsNotNull(realClient, "realClient");

            this.realClient = realClient;
        }

        [DebuggerStepThrough]
        ~SmtpClientWrapper()
        {
            Dispose(false);
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Send(MailMessage message)
        {
            Guard.IsNotNull(message, "Email Message to be sent is Null.");

            try
            {
                realClient.Send(message);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException failedRecipientException)
            {
                throw failedRecipientException;
            }
            catch (System.Net.Mail.SmtpException smtpException)
            {
                throw smtpException;
            }
        }

        [DebuggerStepThrough]
        protected virtual void DisposeCore()
        {
        }

        [DebuggerStepThrough]
        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                DisposeCore();
            }

            disposed = true;
        }
    }
}