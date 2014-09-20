using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CleanRestWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetXmlData(string id)
        {
            string output = string.Empty;
            try
            {
                output = "Your requested data is :" + id;
            }
            catch (Exception general)
            {
                throw new FaultException("something went wrong");
            }
            return output;
        }


        public string GetJsonData(string id)
        {
            return "Your requested DATA IS : " + id;
        }
    }
}
