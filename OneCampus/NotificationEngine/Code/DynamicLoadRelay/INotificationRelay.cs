using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicLoadRelay
{
    public interface INotificationRelay
    {
        RelayResponse FetchNotificationContentUrlInformation(string eventId, string eventCode);
    }
}
