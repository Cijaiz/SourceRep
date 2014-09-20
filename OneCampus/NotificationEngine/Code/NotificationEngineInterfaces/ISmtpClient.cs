
namespace Octane.NotificationWorker.Engine
{
    using System;
    using System.Net.Mail;

    /// <summary>
    /// Methods for SmtpClient
    /// </summary>
    public interface ISmtpClient : IDisposable
    {
        void Send(MailMessage message);
    }
}
