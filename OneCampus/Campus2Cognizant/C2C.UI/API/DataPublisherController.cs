using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using NE = C2C.BusinessEntities.NotificationEntities;

namespace C2C.UI.API
{
    public class DataPublisherController : ApiController
    {
        public List<string> GetToAddress(string eventCode, int PageNo)
        {
            int noOfItemsPerpage = 20;
            List<string> emailList = new List<string>();
            List<string> temporaryToAddress = new List<string>();
            emailList = UserManager.GetAdminEmailIds();
            if(PageNo == 1)
                temporaryToAddress = (from toaddress in emailList select toaddress).Take(noOfItemsPerpage).ToList();
            else
                temporaryToAddress = (from toaddress in emailList select toaddress).Skip((PageNo - 1) * noOfItemsPerpage).Take(noOfItemsPerpage).ToList();

            return temporaryToAddress;

        }

        public List<string> GetToAddressForErrorEmails()
        {          
            List<string> emailList = new List<string>();
            emailList = UserManager.GetAdminEmailIds();         
            return emailList;
        }

        public WeeklySchedule GetWeeklyTaskScheduleMetadata()
        {
            DayOfWeek weekStart = DayOfWeek.Sunday;
            DayOfWeek weekEnd = DayOfWeek.Saturday;
            DateTime startingDate = DateTime.Today;
            DateTime endingDate = DateTime.Today;

            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            while (endingDate.DayOfWeek != weekEnd)
                endingDate = endingDate.AddDays(+1);

           List<NE.BlogPost> blogPostList =   BlogManager.GetWeeklyBlogs(startingDate, endingDate);
           WeeklySchedule weeklySchedule = new WeeklySchedule();
           weeklySchedule.Subject = "weekly Email";
           weeklySchedule.blogPost = blogPostList;

           return weeklySchedule;
        }

    }
}
