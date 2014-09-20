using DynamicLoadRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotificationRelay
{
    public class EngineData : MarshalByRefObject, INotificationRelay
    {
        public RelayResponse FetchNotificationContentUrlInformation(string eventId, string eventCode)
        {
            RelayResponse controllersRouteInformation = null;
            switch (eventCode)
            {
                case RelayConstants.BLOG_POST:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString();
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_CONTROLLER, RelayConstants.NOTIFICATIONFEED_INSERTFEED_ACTION);
                    break;

                case RelayConstants.BLOG_POST1:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString();
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_CONTROLLER, RelayConstants.NOTIFICATIONFEED_INSERTFEED_ACTION);
                    break;

                case RelayConstants.POLL_PUBLISH:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString(); 
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_CONTROLLER, RelayConstants.NOTIFICATIONFEED_INSERTFEED_ACTION);
                    break;

                #region NOTIFICATION HUB

                case RelayConstants.GROUP_ADD_USER:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString(); 
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_USER_CONTROLLER, RelayConstants.NOTIFICATIONFEED_ADDMEMBERSTOGROUP_ACTION);
                   
                    break;

                case RelayConstants.GROUP_REMOVE_USER:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString();
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_USER_CONTROLLER, RelayConstants.NOTIFICATIONFEED_REMOVEMEMBERSFROMGROUP_ACTION);
                   
                    break;

                case RelayConstants.USER_PROFILE_SYNC:
                    controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                    controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString();
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_USER_CONTROLLER, RelayConstants.NOTIFICATIONFEED_USERPROFILESYNC_ACTION);
                    break;


                case RelayConstants.SHARE_CONTENT:
                     controllersRouteInformation = new RelayResponse();
                    //Call insertfeed action of Notification Feeder
                     controllersRouteInformation.PushBaseUrl = BaseUrl.Hub.ToString();                   
                     controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.NOTIFICATIONFEED_CONTROLLER, RelayConstants.NOTIFICATIONFEED_INSERTFEED_ACTION);
                    break;

                #endregion

                #region ONBOARDING
                case RelayConstants.ONBOARDING_INTEGRATION:
                    controllersRouteInformation = new RelayResponse();
                    controllersRouteInformation.PushBaseUrl = BaseUrl.C2C.ToString();
                    controllersRouteInformation.PushUrl = string.Format("{0}/{1}", RelayConstants.ONBOARDING_INTEGRATION_CONTROLLER, RelayConstants.ONBOARDING_INTEGRATION_ACTION);
                    controllersRouteInformation.GetUrl = string.Format("{0}/{1}", RelayConstants.GET_USER_EMAILADDRESS);
                    break;
                #endregion

                case RelayConstants.WEEKLY_EMAIL:
                    controllersRouteInformation = new RelayResponse();
                    controllersRouteInformation.GetBaseUrl = BaseUrl.C2C.ToString();
                    controllersRouteInformation.GetUrl = string.Format("{0}/{1}", RelayConstants.WEEKLY_EMAIL_CONTROLLER, RelayConstants.WEEKLY_EMAIL_ACTION);
                    break;

                default:
                    break;
            }
            return controllersRouteInformation;
        }
    }
}
