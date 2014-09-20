using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.DataAccessLogic
{
    public class SiteWorker
    {
        public static SiteWorker GetInstance()
        {
            return new SiteWorker();
        }
    }
}
