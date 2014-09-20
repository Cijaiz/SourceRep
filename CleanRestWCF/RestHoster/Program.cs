using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using CleanRestWCF;

namespace RestHoster
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebServiceHost host = new WebServiceHost(typeof(Service1)))
            {
                host.Open();
                Console.WriteLine("Service is up and Running at {0}... \n Press any key to shut the service down..", host.BaseAddresses[0].AbsoluteUri);
                Console.ReadKey();
                host.Close();
            }

            Console.WriteLine("Service has shut down..");
            Console.ReadKey();
        }
    }
}
