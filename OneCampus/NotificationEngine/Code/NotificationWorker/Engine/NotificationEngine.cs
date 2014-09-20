using C2C.BusinessEntities.NotificationEntities.MailingEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Extensions;
using C2C.Core.Helper;
using C2C.Core.Logger;
using Octane.NotificationEngineInterfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Octane.NotificationWorker.Engine
{
    /// <summary>
    /// Methods for Loading templates into Concurrent Dictionary and Sending Emails
    /// </summary>
    public abstract class NotificationEngine
    {
        protected static ConcurrentDictionary<string, ITemplate> TemplatesDictionary { get; private set; }
        protected static ISender sender { get; set; }

        public string Subject { get; set; }
        public ICollection<string> ToAddresses { get; set; }

        protected static string MailingServiceUrl { get; set; }
        protected static string FromAddress { get; set; }

        /// <summary>
        /// Loads all the Templates into the Concurrent Dictionary..
        /// </summary>
        /// <param name="templateLoader">The template loader.</param>
        public void LoadAllTemplates(ITemplateLoader templateLoader)
        {
            TemplatesDictionary = new ConcurrentDictionary<string, ITemplate>();
            TemplatesDictionary = templateLoader.LoadAllTemplates();
        }

        /// <summary>
        /// Sends the notification by using the WCF RESTFul service
        /// </summary>
        /// <param name="htmlMessage">the Htmlcontent to be send out..</param>
        public bool SendNotification(string htmlMessage)
        {
            Guard.IsNotBlank(FromAddress, "From Address");
            Guard.IsNotBlank(htmlMessage, "Html message");
            Guard.IsNotBlank(Subject, "Email Subject");
            bool isSuccess = false;
            
            try
            {
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.From = FromAddress;
                emailMessage.To = ToAddresses;
                emailMessage.Subject = Subject;
                emailMessage.MessageBody = htmlMessage;
                isSuccess = SendEmail(emailMessage);
            }
            catch (WebException ex)
            {
                WebException webEx = ex as WebException;
                if (webEx != null)
                {
                    WebResponse resp = webEx.Response;
                    if (resp != null)
                    {
                        using (StreamReader sr = new StreamReader(resp.GetResponseStream(), true))
                        {
                            //This is where details about this 403 message can be found
                            string responseText = sr.ReadToEnd();
                            Logger.Error("Status:" + webEx.Status.ToString() + "\n" + "ErrorMessage:" + webEx.Message);                           
                        }
                    }
                }
            }
            return isSuccess;
        }

        // //Mail sending using Certificate authentication
        //private bool SendEmail(EmailMessage message)
        //{
        //    bool isMailSent = false;
        //    // Create a request using a URL that can receive a post. 
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MailingServiceUrl);
        //    string response = request.WebPostRequest<EmailMessage>(message);
        //    if (response != null)
        //    {
        //        XDocument doc = XDocument.Parse(response);
        //        var val = doc.Descendants().Where(p => p.Name.LocalName == "boolean").ToList();
        //        if (val != null)
        //        {
        //            string responseValue = val.FirstOrDefault().Value;
        //            if (!string.IsNullOrEmpty(responseValue))
        //            {
        //                isMailSent = Convert.ToBoolean(responseValue);
        //            }
        //        }
        //    }
        //    Logger.Debug("SendEmail is compeleted");
        //    return isMailSent;
        //}

        //Mail sending using ACS authentication
        private bool SendEmail(EmailMessage message)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            bool isMailSent = false;

            WebClient wc = new WebClient();
            string response = wc.WebClientPostRequest(MailingServiceUrl, message, CommonHelper.GetHeader(Consumer.MailingService));
            if (!string.IsNullOrEmpty(response))
            {
                SendMailResponse mailResponse = SerializationHelper.JsonDeserialize<SendMailResponse>(response);
                isMailSent = mailResponse.Status;
            }
            return isMailSent;
        }

        public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}