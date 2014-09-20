using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane.NotificationEngineInterfaces;

namespace Octane.NotificationWorker.Engine
{
    /// <summary>
    /// Methods for Sending SMS
    /// </summary>
    public class SMSNotificationEngine : INotificationEngine
    {
        public ICollection<string> FetchToAddressesFromDataStore(string eventId, int pageNo)
        {
            throw new NotImplementedException();
        }

        public bool PrepareNotifications(string eventId, string eventCode)
        {
            throw new NotImplementedException();
        }

        public void PrepareErrorNotification(string eventId, string eventCode, string errorEventCode)
        {
            throw new NotImplementedException();
        }


        public ICollection<string> FetchErrorToAddressesFromDataStore()
        {
            throw new NotImplementedException();
        }

        public int FetchToAddressCountFromDataStore(string eventId)
        {
            throw new NotImplementedException();
        }

        public int GetRetrivalIterationForFetchToAddresses(int addresscount)
        {
            throw new NotImplementedException();
        }

        public void StartEngine(string templateLoaderConfigurationPath, string dataPublisherUrl, string notificationFeederUrl, string serviceUrl, string fromAddress)
        {
            throw new NotImplementedException();
        }

        public DynamicLoadRelay.INotificationRelay NotificationRelay
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
