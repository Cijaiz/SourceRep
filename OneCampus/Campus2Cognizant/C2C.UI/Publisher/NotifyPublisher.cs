using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.Core.Logger;
using C2C.Core.Extensions;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using C2C.Core.Helper.AzureHelper;

namespace C2C.UI.Publisher
{
    public class NotifyPublisher
    {
        private static string hubUrl = CommonHelper.GetConfigSetting(C2C.Core.Constants.C2CWeb.Key.HUB_URL);

        public static string highPriorityQueueName = DefaultValue.HIGH_PRIORITY_QUEUE_NAME;
        public static string lowPriorityQueueName = DefaultValue.LOW_PRIORITY_QUEUE_NAME;
        public static string storageConnectionstring = CommonHelper.GetConfigSetting(DefaultValue.NOTIFICATION_ENGINE_STORAGE);


        public static void Notify(PublisherEvents publishEvent, NotificationPriority priority)
        {
            string json = SerializationHelper.JsonSerialize(publishEvent);
            CloudQueueMessage message = new CloudQueueMessage(json);

            //Checking for the priority and taking the necessary queue name
            switch (priority)
            {
                case NotificationPriority.High:
                    Queue.GetInstance(storageConnectionstring).Push(highPriorityQueueName, true, message);
                    break;

                case NotificationPriority.Low:
                    Queue.GetInstance(storageConnectionstring).Push(lowPriorityQueueName, true, message);
                    break;
            }
        }

        public static void NotifyUserLog(UserLog log)
        {
            try
            {
                WebClient webClient = new WebClient();
                // Constructing hub URL
                string url = string.Format("{0}/{1}/{2}",
                    hubUrl,
                    C2C.Core.Constants.Hub.DefaultValue.ACTIVITY_LOG_CONTROLLER,
                    C2C.Core.Constants.Hub.DefaultValue.ACTIVITY_LOG_SYNC_USER_LOG_ACTION);

                webClient.WebClientPostRequest(url, log, CommonHelper.GetHeader(Consumer.Hub));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}