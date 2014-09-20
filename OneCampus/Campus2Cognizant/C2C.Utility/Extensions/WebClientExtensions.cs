using C2C.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace C2C.Core.Extensions
{
    public static class WebClientExtensions
    {
        /// <summary>
        /// Extension method for WebClient to download stream from specified URL.
        /// </summary>
        /// <param name="webClient">Web Client object </param>
        /// <param name="fetchUrl">Destination URL</param>
        /// <returns>Returns the downloaded string</returns>
        public static string SafeWebClientProcessing(this WebClient webClient, string fetchUrl)
        {
            String request = string.Empty;

            System.IO.Stream stream = webClient.OpenRead(fetchUrl);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            request = reader.ReadToEnd();

            return request;
        }

        /// <summary>
        /// Extension method for WebClient to download stream from specified URL.
        /// </summary>
        /// <param name="webClient">Web Client object </param>
        /// <param name="fetchUrl">Destination URL</param>
        /// <returns>Returns the downloaded string</returns>
        public static string SafeWebClientProcessing(this WebClient webClient, string fetchUrl, Dictionary<string, string> headers)
        {
            String request = string.Empty;
            
            try
            {
                //Trust all certificates
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                foreach (var item in headers)
                {
                    webClient.Headers.Add(item.Key, item.Value);
                }
                System.IO.Stream stream = webClient.OpenRead(fetchUrl);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                request = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                WebException webException = ex as WebException;
                if (webException != null && webException.Response != null)
                {
                    HttpWebResponse webHttpResponse = webException.Response as HttpWebResponse;
                    if (webHttpResponse != null)
                    {
                        if (!(webHttpResponse.StatusCode == HttpStatusCode.OK))
                        {
                            Logger.Logger.Error(string.Format("Error : StatusCode {0}", webHttpResponse.StatusCode.ToString()));
                            Logger.Logger.Error(string.Format("Error Description : {0}", webHttpResponse.StatusDescription));
                        }
                    }
                }
                Logger.Logger.Error(string.Format("Exception Occured during Processing Client Call to Controller {0}.. \n Exception Details: {1}", fetchUrl, ex.Message));
                throw new ApplicationException(string.Format("Exception Occured during Processing Client Call to Controller {0}", fetchUrl));
            }
            return request;
        }

        public static string WebClientPostRequest<T>(this WebClient webClient, string fetchUrl, T postData)
        {
            string postMessage = string.Empty;

            if (postData.GetType().FullName.Equals("System.String"))
                postMessage = postData.ToString();
            else
                postMessage = SerializationHelper.JsonSerialize<T>(postData);

            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            string response = webClient.UploadString(fetchUrl, postMessage);

            return response;

        }

        public static string WebClientPostRequest<T>(this WebClient webClient, string url, T postData, Dictionary<string, string> headers)
        {
            String response = string.Empty;
            try
            {
                //Trust all certificates
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                string postMessage = string.Empty;

                if (postData.GetType().FullName.Equals("System.String"))
                    postMessage = postData.ToString();
                else
                    postMessage = SerializationHelper.JsonSerialize<T>(postData);

                foreach (var item in headers)
                {
                    webClient.Headers.Add(item.Key, item.Value);
                }

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = webClient.UploadString(url, postMessage);
            }
            catch (Exception ex)
            {
                WebException webException = ex as WebException;
                if (webException.Response != null)
                {
                    HttpWebResponse webHttpResponse = webException.Response as HttpWebResponse;
                    if (webHttpResponse != null)
                    {
                        if (!(webHttpResponse.StatusCode == HttpStatusCode.OK))
                        {
                            Logger.Logger.Error(string.Format("Error : StatusCode {0}", webHttpResponse.StatusCode.ToString()));
                            Logger.Logger.Error(string.Format("Error Description : {0}", webHttpResponse.StatusDescription));
                        }
                    }
                }
                Logger.Logger.Error(string.Format("Exception Occured during Processing Client Call to Controller {0} \n Exception Details: {1}", url, ex.Message));
                throw new ApplicationException(string.Format("Exception Occured during Processing Client Call to Controller {0}", url));
            }
            return response;
        }

        public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
