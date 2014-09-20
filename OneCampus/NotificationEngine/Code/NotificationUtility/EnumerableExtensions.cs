namespace Octane.NotificationUtility
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;
    using System.Net.Security;

    /// <summary>
    /// Extension methods for Notification Engine
    /// </summary>
    public static class EnumerableExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            if (instance == null)
                return;

            foreach (T item in instance)
            {
                action(item);
            }
        }

        /// <summary>
        /// Extension method for WebClient to download stream from specified URL.
        /// </summary>
        /// <param name="webClient">Web Client object </param>
        /// <param name="fetchUrl">Destination Url</param>
        /// <returns>Returns the downloaded string</returns>
        public static string SafeWebClientProcessing(this WebClient webClient, string fetchUrl)
        {
            String request = string.Empty;
            try
            {
                System.IO.Stream stream = webClient.OpenRead(fetchUrl);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                request = reader.ReadToEnd();
            }
            catch (WebException webException)
            {
                throw webException;
            }

            catch (ArgumentException argException)
            {
                throw argException;
            }
            return request;
        }

        public static string SafeWebClientProcessingWithAcs(this WebClient webClient, string fetchUrl, C2C.Core.Constants.Engine.Consumer consumer)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            //AuthenticationTokenBuilder authenticationBuilder = new AuthenticationTokenBuilder();

            //switch (consumer)
            //{
            //    case NotificationDataEntities.Constants.Consumer.Hub:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.HUB_REALM;
            //        break;
            //    case NotificationDataEntities.Constants.Consumer.MailingService:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.MAILINGSERVICE_REALM;
            //        break;
            //    case NotificationDataEntities.Constants.Consumer.Orchard:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.ORCHARD_REALM;
            //        break;
            //    default:
            //        break;
            //}

            //authenticationBuilder.ACSNameSpaceSettingKey = Octane.NotificationDataEntities.Constants.ACS_NAMESPACE;
            //authenticationBuilder.ACSUserIdSettingKey = Octane.NotificationDataEntities.Constants.SERVICEIDENTITY_USERNAME;
            //authenticationBuilder.ACSPasswordSettingKey = Octane.NotificationDataEntities.Constants.SERVICEIDENTITY_PASSWORD;

            string token = string.Empty;  //authenticationBuilder.BuildACSToken();

            String request = string.Empty;
            try
            {
                string headerValue = string.Format("WRAP access_token=\"{0}\"", token);
                webClient.Headers.Add("Authorization", headerValue);
                System.IO.Stream stream = webClient.OpenRead(fetchUrl);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                request = reader.ReadToEnd();

            }
            catch (WebException webException)
            {
                WebResponse resp = webException.Response;
                if (resp != null)
                {
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream(), true))
                    {
                        //This is where details about this 403 message can be found
                        string responseText = sr.ReadToEnd();
                    }
                }
                throw webException;
            }

            catch (ArgumentException argException)
            {
                throw argException;
            }
            return request;
        }

        /// <summary>
        /// Extension Method for WebClient to call Post method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webClient">Web Client object</param>
        /// <param name="fetchUrl">Destination URL</param>
        /// <param name="postData">CHire data is the input to be passed to the post method</param>
        /// <returns>Returns the json response from the CHitr Import Controller</returns>
        public static string WebClientPostRequest<T>(this WebClient webClient, string fetchUrl, T postData)
        {
            string postMessage = string.Empty;

            if (postData.GetType().FullName.Equals("System.String"))
                postMessage = postData.ToString();
            else
                postMessage = JsonHelper.JsonSerialize<T>(postData);

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = wc.UploadString(fetchUrl, postMessage);

                return response;
            }
        }

        public static string WebClientPostRequestWithAcs<T>(this WebClient webClient, string fetchUrl, T postData, C2C.Core.Constants.Engine.Consumer consumer)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            //AuthenticationTokenBuilder authenticationBuilder = new AuthenticationTokenBuilder();

            //switch (consumer)
            //{
            //    case NotificationDataEntities.Constants.Consumer.Hub:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.HUB_REALM;
            //        break;
            //    case NotificationDataEntities.Constants.Consumer.MailingService:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.MAILINGSERVICE_REALM;
            //        break;
            //    case NotificationDataEntities.Constants.Consumer.Orchard:
            //        authenticationBuilder.ACSRealmSettingKey = Octane.NotificationDataEntities.Constants.ORCHARD_REALM;
            //        break;
            //    default:
            //        break;
            //}

            //authenticationBuilder.ACSNameSpaceSettingKey = Octane.NotificationDataEntities.Constants.ACS_NAMESPACE;
            //authenticationBuilder.ACSUserIdSettingKey = Octane.NotificationDataEntities.Constants.SERVICEIDENTITY_USERNAME;
            //authenticationBuilder.ACSPasswordSettingKey = Octane.NotificationDataEntities.Constants.SERVICEIDENTITY_PASSWORD;

            string token = string.Empty; //authenticationBuilder.BuildACSToken();

            //string token = AzureHelper.GetTokenFromACS(consumer);

            string postMessage = string.Empty;

            if (postData.GetType().FullName.Equals("System.String"))
                postMessage = postData.ToString();
            else
                postMessage = JsonHelper.JsonSerialize<T>(postData);

            string headerValue = string.Format("WRAP access_token=\"{0}\"", token);
            webClient.Headers.Add("Authorization", headerValue);
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            string response = webClient.UploadString(fetchUrl, postMessage);

            return response;

        }

        private static System.Security.Cryptography.X509Certificates.X509Certificate2 getStoreCertificate(string thumbprint)
        {
            List<System.Security.Cryptography.X509Certificates.StoreLocation> locations = new List<StoreLocation>
            { 
                StoreLocation.CurrentUser, 
                StoreLocation.LocalMachine ,
            };

            foreach (var location in locations)
            {
                X509Store store = new System.Security.Cryptography.X509Certificates.X509Store("My", location);
                try
                {
                    store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificates = store.Certificates.Find(
                        X509FindType.FindByThumbprint, thumbprint, false);
                    if (certificates.Count == 1)
                    {
                        return certificates[0];
                    }
                }
                finally
                {
                    store.Close();
                }
            }

            throw new ArgumentException(string.Format(
                "A Certificate with thumbprint '{0}' could not be located.",
                thumbprint));
        }

        public static string WebPostRequest<T>(this HttpWebRequest webRequest, T requestData)
        {
            string responseFromServer = null;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            try
            {
                webRequest.Method = "POST";

                byte[] messageInBytes = Encoding.UTF8.GetBytes(JsonHelper.SerializeToXML<T>(requestData));


                // Set the ContentType property of the WebRequest.
                webRequest.ContentType = "application/xml";

                // Set the ContentLength property of the WebRequest.
                webRequest.ContentLength = messageInBytes.Length;

                //X509Certificate cert = new X509Certificate("OnTrees.cer");

                X509Certificate cert = getStoreCertificate("C7E2F410FA6830BA0380B1320E65EBF01E677E23");
                webRequest.ClientCertificates.Add(cert);

                // Get the request stream.
                Stream dataStream = webRequest.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(messageInBytes, 0, messageInBytes.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = webRequest.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            return responseFromServer;
        }

        public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
