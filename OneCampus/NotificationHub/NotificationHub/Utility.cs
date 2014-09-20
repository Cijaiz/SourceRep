using C2C.Core.Helper;
using NotificationHub.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NotificationHub
{
    public class Utility
    {
        public static string GetConnectionString()
        {
            return CommonHelper.GetConfigSetting("NotificationHubEntities");
        }

        public static NotificationHubEntities GetDbContext()
        {
            return new NotificationHubEntities(GetConnectionString());
        }
    }
}