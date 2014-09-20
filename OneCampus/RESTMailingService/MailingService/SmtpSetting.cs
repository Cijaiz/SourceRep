using Microsoft.WindowsAzure;
using C2C.Core.Constants.Mailing;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using C2C.Core.Logger;

namespace MailingService
{
    public static class SmtpSetting
    {
        public static int Port { get; private set; }

        public static SmtpDeliveryMethod SmtpDeliveryMethod { get; private set; }

        public static bool EnableSsl { get; private set; }

        public static string Host { get; private set; }

        public static bool UseDefaultCredentials { get; private set; }

        public static NetworkCredential Credential { get; private set; }

        /// <summary>
        /// This method is used to update the SMTP setting property from the service configuration.
        /// </summary>
        public static void UpdateSetting()
        {
            SmtpSetting.Host = CloudConfigurationManager.GetSetting(DefaultValue.MAILING_SERVICE_CONFIG_SMTP_HOST);
            SmtpSetting.Port = Convert.ToInt32(CloudConfigurationManager.GetSetting(DefaultValue.MAILING_SERVICE_CONFIG_SMTP_PORT));
            SmtpSetting.SmtpDeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpSetting.UseDefaultCredentials = false;
            SmtpSetting.Credential = new NetworkCredential()
            {
                UserName = CloudConfigurationManager.GetSetting(DefaultValue.MAILING_SERVICE_CONFIG_SMTP_USERNAME),
                Password = CloudConfigurationManager.GetSetting(DefaultValue.MAILING_SERVICE_CONFIG_SMTP_PASSWORD)
            };
            SmtpSetting.EnableSsl = Convert.ToBoolean(CloudConfigurationManager.GetSetting(DefaultValue.MAILING_SERVICE_CONFIG_SMTP_ENABLESSL));

            Logger.Debug("SMTP setting's update from cloud configuration. update on:{0}",DateTime.UtcNow);
        }
    }
}