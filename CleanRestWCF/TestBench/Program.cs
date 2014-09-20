using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestBench
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:20498/Service1.svc/getxml/1");
            request.ContentType = "text/json";
            request.Method = "GET";
            string responseText = string.Empty;

            //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            //{
            //    streamWriter.Write("1");
            //}
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }

            Console.Write("Response returned from the Service is {0}...", responseText);
            Console.ReadKey();
        }
    }
}
