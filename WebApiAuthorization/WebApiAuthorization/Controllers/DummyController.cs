using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAuthorization.Controllers
{
    public class DummyController : ApiController
    {
        [HttpGet]
        public int Getdata(int Id)
        {
            return Id;
        }
    }
}
