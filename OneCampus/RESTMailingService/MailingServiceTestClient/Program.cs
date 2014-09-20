using C2C.BusinessEntities.NotificationEntities.MailingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailingServiceTestClient
{
    class Program
    {
        static string BaseUrl = "https://127.0.0.1/MailingService.svc";

        static void Main(string[] args)
        {
            //BaseUrl = "https://c2cmailingservice.cloudapp.net/MailingService.svc";

            Console.WriteLine("Press a key to start testing mailing service");
            Console.ReadKey();

            TestMailSending();

            Console.WriteLine("Press a enter to close testing console.");
            Console.ReadLine();
        }

        internal static void TestMailSending(EmailMessage data = null)
        {
            Console.WriteLine("Assert Sending Mail:");

            if (data == null)
            {
                Console.WriteLine("Default data");
                data = new EmailMessage();
                data.From = "RakeshPrabhu.C@cognizant.com";
                data.ReplyTo.Add("RakeshPrabhu.C@cognizant.com");
                data.CC.Add("Rajarajan.R2@cognizant.com");
                data.Bcc.Add("Ramya.Rangarajan@cognizant.com");
                data.MessageBody = string.Format("<h1>This is a test message from newly built mailing service with ACS authentication.</h1><p>Test on{0}</p><p>Regards: Rakesh</p>", DateTime.UtcNow);
                data.Sender = "RakeshPrabhu.C@cognizant.com";
                data.Subject = "Testing HttpMailing App";
                data.To.Add("rakeshprabhu.c@cognizant.com");
                data.To.Add("Ramya.Rangarajan@cognizant.com");
                data.To.Add("Vijayananthan.JC@cognizant.com");
                data.To.Add("Revathi.I@cognizant.com");
                data.To.Add("Rajarajan.R2@cognizant.com");
                data.To.Add("Suresh.T2@cognizant.com");
            }

            var inputData = ExtentionHelper.JsonSerilize<EmailMessage>(data);
            Console.WriteLine("Data:{0}", inputData);

            var responce = ExtentionHelper.Post(string.Format("{0}/SendingEmail", BaseUrl), inputData, true);

            Console.WriteLine("Response:{0}", responce.ToString());
            Console.WriteLine("--------------------------- End Assert ----------------------");
        }
    }
}
