using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.Core.Logger;
using DynamicLoadRelay;
using Microsoft.WindowsAzure.Storage.Queue;
using Octane.NotificationEngineInterfaces;
using Octane.NotificationUtility;
using System;
using System.Collections.Generic;

namespace Octane.NotificationWorker.Engine
{
    /// <summary>
    /// Methods for processing messages
    /// </summary>
    public class MessageDispatcher
    {
        private INotificationRelay notificationRelay = null;

        public MessageDispatcher(INotificationRelay relay)
        {
            notificationRelay = relay;
        }

        /// <summary>
        /// Processes the message based on the Eventcode.
        /// </summary>
        /// <param name="message">processes the downloaded message</param>
        public bool ProcessMessage(CloudQueueMessage message)
        {
            bool isMessageProcessed = true;
            INotificationEngine engine = null;
            string contentMessage = null;

            try
            {
                Guard.IsNotNull(message, "Queue Downloaded Message");

                //Deserialization logic goes here.. to pull the evenid and eventcode from message.
                var eventDetails = JsonHelper.JsonDeserialize<PublisherEvents>(message.AsString);
                string eventId = eventDetails.EventId;
                string eventCode = eventDetails.EventCode;

                Logger.Debug(string.Format("Fresh Message Downloaded with Event Code {0} and Event Id {1} and Message details As below \n {2}",
                                                                            eventCode,
                                                                            eventId,
                                                                            !string.IsNullOrEmpty(message.AsString) ? message.AsString : "Message details were Empty"));

                //Check whether there are any extra messages for processing.. (now notification engine is also used as processing engine...)
                if (!string.IsNullOrEmpty(eventDetails.NotificationContent))
                {
                    contentMessage = eventDetails.NotificationContent;
                    Logger.Debug(string.Format("Downlaoded message content is \n {0}", contentMessage));
                    engine = new EmailNotificationEngine(contentMessage);
                }
                else //This is just a simple notification engine with or without notify option..
                    engine = new EmailNotificationEngine();

                engine.NotificationRelay = notificationRelay; // Set the Dynamically loaded relay..
                Logger.Debug("Started to get to address count");
                int addresscount = engine.FetchToAddressCountFromDataStore(eventId);
                Logger.Debug("Got to address count");
                int retrivalIterations = engine.GetRetrivalIterationForFetchToAddresses(addresscount);

                Logger.Debug(string.Format("Total To Address counts fetched from store is: {0} and Total Retrival Iteration set are : {1}",
                                                                                                                    addresscount.ToString(),
                                                                                                                    retrivalIterations.ToString()));
                for (int pageno = 1; pageno <= retrivalIterations; pageno++)
                {
                    Logger.Debug("Started to get to addresses");
                    ICollection<string> fetchedToaddresses = engine.FetchToAddressesFromDataStore(eventId, pageno);
                    Logger.Debug("Got to addresses");
                    isMessageProcessed = engine.PrepareNotifications(eventId, eventCode);
                }
            }
            catch (Exception generalException)
            {
                isMessageProcessed = false;
                Logger.Error(string.Format("Error Processing message Id: {0} with message detail as below \n {1} \n Error Message: {2}", message.Id, message.AsString, generalException));


                //Catch it silently to avoid Aggregatesum exception from Application pool.
            }
            return isMessageProcessed;
        }

        /// <summary>
        /// Processes Errors the message based on the Eventcode.
        /// </summary>
        /// <param name="message">processes the downloaded message</param>
        public void ProcessErrorMessage(CloudQueueMessage message)
        {
            try
            {
                //Deserialization logic goes here.. to pull the evenid and eventcode from message.
                var eventDetails = JsonHelper.Deserialize<PublisherEvents>(message.AsString);
                string eventId = eventDetails.EventId;
                string eventCode = eventDetails.EventCode;

                //Send error email to admin.. 
                INotificationEngine engine = new EmailNotificationEngine();
                engine.FetchErrorToAddressesFromDataStore();
                engine.PrepareErrorNotification(eventId, eventCode, EventCodes.ERROR_EMAIL);
            }
            catch (Exception generalException)
            {
                Logger.Error(string.Format("Error Processing message Id: {0} and Error details {1}: Inner Exception would be {2}", message.Id,
                    generalException.Message,
                    generalException.InnerException != null ? generalException.InnerException.Message : "NA"));
                //Catch it silently to avoid Aggregatesum exception from Application pool. Just log it and remvoe message from queue..
            }
        }
    }
}
