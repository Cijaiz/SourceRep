using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MailingServiceTestClient
{
    public static class ExtentionHelper
    {
        public static string JsonSerilize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonDeserilize<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static string Post(string url, string jsonData, bool isAcsEnabled = false)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            try
            {
                if (isAcsEnabled)
                {
                    using (WebClient wc = new WebClient())
                    {
                        string headerValue = string.Format("WRAP access_token=\"{0}\"", ACSTestHelper.GetACSToken());
                        wc.Headers.Add("Authorization", headerValue);
                        var headervalue = wc.Headers.Get("Authorization");
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string response = wc.UploadString(url, jsonData);

                        return response;
                    }
                }
                else
                {
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        return client.UploadString(url, jsonData);
                    }
                }
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
            catch (NotSupportedException ex)
            {
                return ex.Message;
            }
        }

        public static string Get(string url, bool isAcsEnabled = false)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            //Console.WriteLine("URL:"+ url);
            if (isAcsEnabled)
            {
                //string headerValue = string.Format("WRAP access_token=\"{0}\"", ACSTestHelper.GetACSToken());
                using (var client = new WebClient())
                {
                    //client.Headers.Add("Authorization", headerValue);
                    return client.DownloadString(url);
                }
            }
            else
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
        }

        public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
