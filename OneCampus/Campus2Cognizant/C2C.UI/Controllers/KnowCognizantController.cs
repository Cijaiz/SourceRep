using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    [Authorize]
    public class KnowCognizantController : BaseController
    {
    
        // GET: /KnowCognizant/
        /// <returns>Returns the Index Page </returns>

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creates the article1 view page
        /// </summary>
      
        /// <returns>Returns the Article1 Page </returns>
        public ActionResult Article1()
        {
            return View();
        }

        /// <summary>
        /// Creates the article2 view page
        /// </summary>
       
       /// <returns>Returns the Article2 Page</returns>
        public ActionResult Article2()
        {
             return View();
        }
    }
}
