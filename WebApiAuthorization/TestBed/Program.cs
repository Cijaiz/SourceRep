using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using C2C.Core.Extensions;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            var wc = new WebClient();
            SetBasicAuthHeader(wc, "user", "password");
            var returnBytes = wc.SafeWebClientProcessing("http://localhost:1548/api/home/getdata?id=5");
        }

        private static void SetBasicAuthHeader(WebClient request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}
