using DynamicLoadRelay;
using System.Collections.Generic;

namespace Octane.NotificationEngineInterfaces
{
    /// <summary>
    /// Interface for Notification Engine Methods
    /// </summary>
    public interface INotificationEngine
    {
        INotificationRelay NotificationRelay { get; set; }
        ICollection<string> FetchToAddressesFromDataStore(string eventId, int pageNo);
        ICollection<string> FetchErrorToAddressesFromDataStore();
        void StartEngine(string templateLoaderConfigurationPath, string dataPublisherUrl, string notificationFeederUrl, string serviceUrl, string fromAddress);
        bool PrepareNotifications(string eventId, string eventCode);
        void PrepareErrorNotification(string eventId, string eventCode, string errorEventCode);
        int FetchToAddressCountFromDataStore(string eventId);
        int GetRetrivalIterationForFetchToAddresses(int addresscount);
    }
}
