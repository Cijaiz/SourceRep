using C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Constants.Engine;
using C2C.Core.Extensions;
using C2C.Core.Helper;
using C2C.Core.Logger;
using DynamicLoadRelay;
using Microsoft.WindowsAzure.Storage.Table;
using Octane.NotificationEngineInterfaces;
using Octane.NotificationUtility;
using Octane.NotificationWorker.TemplateLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Octane.NotificationWorker.Engine
{
    /// <summary>
    /// Emailnotification Engine.
    /// </summary>
    public class EmailNotificationEngine : NotificationEngine, INotificationEngine
    {
        #region Class Variables
        private ITemplateLoader templateLoader;
        private string notificationContent = null;
        private static string dataPublisherUrl;
        private static string notificationFeedUrl;
        public static string chireFilePath;
        private C2C.Core.Helper.AzureHelper.Table tableHelper = null;
        #endregion

        public EmailNotificationEngine()
            : base()
        {
        }

        public EmailNotificationEngine(string notificationContent)
        {
            //Bringing the message content Into the engine..
            this.notificationContent = notificationContent;
            tableHelper = C2C.Core.Helper.AzureHelper.Table.GetInstance(NotificationManager.StorageConnectionString);
        }

        #region Public Methods
        /// <summary>
        /// This Method starts the Engine to perform warmup tasks for Notification Engine.
        /// </summary>
        /// <param name="templateLoaderConfigurationPath">The Correspoinding TemplateLoaders Configuration files path. {path to the xml file where all the configurations are stored.}</param>
        public void StartEngine(string templateLoaderConfigurationPath, string publisherUrl, string notificationFeederUrl, string serviceUrl, string fromAddress)
        {
            Guard.IsNotBlank("templateLoaderConfigurationPath", "templateLoaderConfigurationPath");
            Guard.IsNotBlank("dataPublisherUrl", "dataPublisherUrl");
            dataPublisherUrl = publisherUrl;

            //Dont call the Loader Component.if the Templates Dictionary is loaded already.
            if (TemplatesDictionary != null)
                return;

            templateLoader = new EmailTemplateLoader();
            templateLoader.ConfigurationFilePath = templateLoaderConfigurationPath;
            dataPublisherUrl = publisherUrl;
            notificationFeedUrl = notificationFeederUrl;

            //RESTFul Service Url
            MailingServiceUrl = serviceUrl;

            //Assign FROM email address
            FromAddress = fromAddress;

            //Automatically Startup the Template Loader Component..
            InitializeEngine();
        }

        /// <summary>
        /// This method is used to fetch all the To Error addresses from the DataStore {ex. Database}
        /// </summary>
        /// <param name="eventId">The event id which is uniquely identified in the Dtabase.</param>
        /// <returns>Collection of To Email addresses.</returns>
        public ICollection<string> FetchErrorToAddressesFromDataStore()
        {
            var toAddress = SerializationHelper.JsonDeserialize<List<string>>(FetchErrorToAddressDataFromNotificationDataPublisher(C2C.Core.Constants.Engine.DefaultValue.ERROR_EMAIL_TOADDRESS_ACTION));
            if (ToAddresses != null)
                ToAddresses.Clear();

            ToAddresses = toAddress;
            return ToAddresses;
        }

        /// <summary>
        /// This method is used to fetch all the To addresses from the DataStore {ex. Database}
        /// </summary>
        /// <param name="eventId">The event id which is uniquely identified in the Dtabase.</param>
        /// <returns>Collection of To Email addresses.</returns>
        public ICollection<string> FetchToAddressesFromDataStore(string eventId, int pageNo)
        {
            Guard.IsNotBlank("eventId", "eventId in FetchToAddressesFromDataStore");
            var toAddress = SerializationHelper.JsonDeserialize<List<string>>(FetchToAddressDataFromNotificationDataPublisher(DefaultValue.TOADDRESS_ACTION, eventId, pageNo));

            ToAddresses = toAddress;
            return ToAddresses;
        }

        /// <summary>
        /// This method is used to fetch all the To addresses from the DataStore {ex. Database}
        /// </summary>
        /// <param name="eventId">The event id which is uniquely identified in the Dtabase.</param>
        /// <returns>Collection of To Email addresses.</returns>
        public int FetchToAddressCountFromDataStore(string eventId)
        {

            int toAddressCount = 1;
            try
            {
                Guard.IsNotBlank("eventId", "eventId in FetchToAddressesFromDataStore");

                var toAddress = SerializationHelper.JsonDeserialize<List<string>>(FetchToAddressDataFromNotificationDataPublisher(DefaultValue.TOADDRESS_ACTION, eventId, 1));

                if (toAddress.Count > 0)
                    toAddressCount = toAddress.Count;

            }
            catch
            {
                return toAddressCount;
            }
            return toAddressCount;
        }

        /// <summary>
        /// This method is used to fetch the retrival count based on the total To Addresses count
        /// </summary>
        /// <param name="addresscount">The total To Addresses count</param>
        /// <returns>Retrival value.</returns>
        public int GetRetrivalIterationForFetchToAddresses(int addresscount)
        {
            int noOfItemsPerpage = 20;
            int iterationValue = (int)Math.Ceiling(Convert.ToDouble(addresscount) / noOfItemsPerpage);

            //atleast one iteration shuld be executed..
            return iterationValue.Equals(0) ? 1 : iterationValue;
        }

        /// <summary>
        /// Prepares the notification content based on the eventcode.
        /// </summary>
        /// <param name="eventId">The eventid to identify the data to be prepared..</param>
        /// <param name="eventCode">Eventcode to be passed to pick up the appropriate model.</param>
        public bool PrepareNotifications(string eventId, string eventCode)
        {
            Guard.IsNotBlank("eventId", "eventId in PrepareNotifications");
            Guard.IsNotBlank("eventCode", "eventCode in PrepareNotifications");

            bool isNotificationSent = false;
            string htmlOutput = string.Empty;
            ITemplate executionTemplate = null;

            //Populate the email content from the database using the event id and eventcode.
            Logger.Debug(string.Format("Fetching Notification content from datastore for message with Event Code {0} and Event Id {1}.", eventCode, eventId));
            dynamic model = FetchContentDataFromDataStore(eventId, eventCode);

            if (TemplatesDictionary.TryGetValue(eventCode, out executionTemplate))
            {
                if (executionTemplate.Notify)
                {
                    if (model != null && executionTemplate != null)
                        htmlOutput = PrepareHtmlFromContent(model, executionTemplate);
                    else
                    {
                        Logger.Error(string.Format("Unable to Prepare the HTML content.. Model or Executioin template may be empty.. for EventCode.. -{0} " , eventCode));
                    }
                }
                isNotificationSent = true;
            }
            else
            {
                Logger.Error(string.Format("Unable to find the matching EventCode {0} in the Central Dictionary.", eventCode));
                throw new InvalidOperationException("Unable to find the matching EventCode in the Central Dictionary.");
            }

            if (isNotificationSent && executionTemplate != null && executionTemplate.Notify)
                isNotificationSent = SendNotification(htmlOutput);

            return isNotificationSent;
        }

        /// <summary>
        /// Prepares Error Notification for Failed Messages..
        /// </summary>
        /// <param name="eventId">Error</param>
        /// <param name="eventCode"></param>
        public void PrepareErrorNotification(string eventId, string eventCode, string errorEventCode)
        {
            Guard.IsNotBlank("eventId", "eventId");
            Guard.IsNotBlank("eventCode", "eventCode");
            Guard.IsNotBlank("errorEventCode", "errorEventCode");

            bool isNotificationSent = true;
            string htmlOutput = string.Empty;

            //Set the Subject for Error Emails..
            Subject = string.Format("There was an Error Processing Message with Event Id: {0} and Event Code: {1} at {2}.", eventId, eventCode, DateTime.Now.ToString());

            dynamic model = new { UserName = "admin", EventID = eventId, EventCode = eventCode };

            ITemplate executionTemplate = null;
            if (TemplatesDictionary.TryGetValue(errorEventCode, out executionTemplate))
                htmlOutput = PrepareHtmlFromContent(model, executionTemplate);
            else
            {
                Logger.Error("Unable to find the matching EventCode in the Central Dictionary.");
                isNotificationSent = false;
                throw new InvalidOperationException("Unable to find the matching EventCode in the Central Dictionary.");
            }

            if (isNotificationSent)
                isNotificationSent = SendNotification(htmlOutput);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the Notification Engine..
        /// </summary>
        private void InitializeEngine()
        {
            //Load all the templates..
            base.LoadAllTemplates(templateLoader);
        }

        /// <summary>
        /// GetDataFrom DataStore with the EventId.
        /// </summary>
        /// <param name="eventId">The eventid to identify the data to be prepared..</param>
        /// <param name="eventCode">Eventcode to be passed to pick up the appropriate model.</param>
        /// <returns>Model</returns>
        private dynamic FetchContentDataFromDataStore(string eventId, string eventCode)
        {
            Guard.IsNotBlank("eventId", "eventId");
            Guard.IsNotBlank("eventCode", "eventCode");

            dynamic result = null;
            #region CHIRE IMPORT
            if (eventCode == EventCodes.IMPORT_CHIREDATA)
            {
                string fileName = string.Empty;
                List<ProcessResult> processResults = new List<ProcessResult>();
                if (DateTime.UtcNow.Hour <= 12)
                {
                    fileName = DateTime.UtcNow.Date.ToString("yyyyMMdd-1") + ".csv";
                }
                else
                {
                    fileName = DateTime.UtcNow.Date.ToString("yyyyMMdd-2") + ".csv";
                }

                //fileName = "UAT_User1.csv";
                string importFileName = chireFilePath + fileName;
                //TODO : to be removed
                //importFileName = "D:/FTPServer/UAT_User1.csv";

                //IEnumerable<CHireImportLogEntity> partitionQuery = (from e in TableHelper.QueryTasks<CHireImportLogEntity>("CHireImportLogs")
                //                                                    where e.PartitionKey.Equals(fileName)
                //                                                    select e);

                TableQuery<CHireImportLogEntity> query = new TableQuery<CHireImportLogEntity>().Where(TableQuery.GenerateFilterCondition(
                                                                                                                    "PartitionKey", QueryComparisons.Equal, fileName));

                IEnumerable<CHireImportLogEntity> partitionQuery = tableHelper.Query<CHireImportLogEntity>(DefaultValue.CHIRE_LOG_TABLE, query);

                var c = partitionQuery.ToList();
                if (c.Count > 0)
                {
                    result = new
                    {
                        Status = "Failure",
                        Subject = "CHire Import Failed"
                    };
                }

                else
                {
                    //Download CHireData from sftp server
                    Logger.Debug("Downloading data from CHIRE server..");
                    MemoryStream chireStream = CHireManager.DownloadCHireFile(importFileName);

                    //Read CHire data and return list of chiredata
                    Logger.Debug("REading CHIRE file");
                    List<CHireData> CHireDataList = CHireManager.ReadCHireFile(chireStream);
                    result = ProcessUserData(CHireDataList, fileName);
                    Logger.Debug("Processed CHIRE result with summary message: " + result.SummaryMessage);

                }

            }
            #endregion
            #region DYNAMIC LOAD DLL
            else if (eventCode == EventCodes.DYNAMIC_DLL_LOAD)
            {
                NotificationContent notification = SerializationHelper.JsonDeserialize<NotificationContent>(notificationContent);
                result = new
                {
                    Subject = notification.Subject,
                    Message = notification.Description
                };
            }
            #endregion

            #region FILE UPLOAD
            else if (eventCode == EventCodes.IMPORT_USERFROM_BLOB)
            {
                NotificationContent notification = SerializationHelper.JsonDeserialize<NotificationContent>(notificationContent);
                //IEnumerable<CHireImportLogEntity> blobPartitionQuery = (from e in TableHelper.QueryTasks<CHireImportLogEntity>("CHireImportLogs")
                //                                                        where e.PartitionKey.Equals(notification.ContentTitle)
                //                                                        select e);

                TableQuery<CHireImportLogEntity> query = new TableQuery<CHireImportLogEntity>().Where(TableQuery.GenerateFilterCondition(
                                                                                                           "PartitionKey", QueryComparisons.Equal, notification.ContentTitle));

                IEnumerable<CHireImportLogEntity> blobPartitionQuery = tableHelper.Query<CHireImportLogEntity>(DefaultValue.CHIRE_LOG_TABLE, query);


                var partitionList = blobPartitionQuery.ToList();
                if (partitionList.Count > 0)
                {
                    result = new
                    {
                        Status = "Failure",
                        Subject = "CHire Import Failed"
                    };
                }
                else
                {
                    //Download FILE FROM BLOB
                    Logger.Debug("Downloading data from BLOB..");
                    Stream fileStream = AzureHelper.DownloadContentsFromBlob(notification.URL);

                    //Read CHire data and return list of chiredata
                    Logger.Debug("REading CHIRE file");
                    List<CHireData> FileDataList = CHireManager.ReadCHireFile(fileStream);

                    result = ProcessUserData(FileDataList, notification.ContentTitle);

                    Logger.Debug("Processed CHIRE result with summary message: " + result.SummaryMessage);
                }
            }
            #endregion
            else // Process the REST of the Event codes here using dynamic load dll...
            {
                if (eventCode != EventCodes.WEEKLY_EMAIL)
                {
                    if (string.IsNullOrEmpty(notificationContent))
                    {
                        Logger.Error(string.Format("Unable to load / no NotificationContent available in the downloaded queue message.. matching EventCode {0}.", eventCode));
                        throw new InvalidOperationException("Unable to load the NotificationContent from the downloaded queue message.");
                    }
                }

                Logger.Debug("Performing Controller Actions Retrivals from Dynamic DLL...");
                if (eventCode == EventCodes.BLOG_POST || eventCode == EventCodes.POLL_PUBLISH)
                    result = SerializationHelper.JsonDeserialize<NotificationContent>(notificationContent);

                //calling the Relay dll..
                Logger.Debug("calling the dll");
                RelayResponse engineResponse = NotificationRelay.FetchNotificationContentUrlInformation(eventId, eventCode);
                Logger.Debug("data fetched from dll");
                //Build REsult
                if (!string.IsNullOrEmpty(engineResponse.GetUrl) && eventCode != EventCodes.ONBOARDING_INTEGRATION)
                {
                    result = SerializationHelper.JsonDeserialize<WeeklySchedule>(FetchTaskSchedulerData(engineResponse.GetUrl));
                }

                //Hub Sync..
                if (!string.IsNullOrEmpty(engineResponse.PushUrl))
                {
                    if (eventCode == EventCodes.ONBOARDING_INTEGRATION)
                    {
                        OnBoardingEntity deserializedOnboardingEntity = SerializationHelper.JsonDeserialize<OnBoardingEntity>(notificationContent);
                        PushDataToNotificationDataPublisher(engineResponse.PushUrl, deserializedOnboardingEntity);
                        if (deserializedOnboardingEntity.OfferStatus == OfferStatus.OFFER_ACCEPTED)
                        {
                            var toAddress = SerializationHelper.JsonDeserialize<OnBoardingOfferAccepted>(FetchDataFromNotificationDataPublisher(engineResponse.GetUrl, deserializedOnboardingEntity.CandidateId));
                            ToAddresses.Clear();
                            ToAddresses.Add(toAddress.Email);
                        }

                        result = new
                        {
                            Subject = "Offer Accepted Confirmation",
                            UserName = deserializedOnboardingEntity.CandidateName
                        };
                    }
                    else
                        PushDataToNotificationFeeder(engineResponse.PushUrl, engineResponse.PushBaseUrl, notificationContent);
                }
            }
            if (result == null)
            {
                Logger.Error(string.Format("Data Unavailable for EventId {0}.", eventId));
            }

            if (result != null)
            {
                Subject = result.Subject;
            }
            return result;
        }

        /// <summary>
        /// Prepares the html by rendering the model prepared using MVC razor engine.
        /// </summary>
        /// <param name="modelData">Model data to be rendered.</param>
        /// <param name="template">Template for parsing the data.</param>
        /// <returns>Html string.</returns>
        private string PrepareHtmlFromContent(dynamic modelData, ITemplate template)
        {
            return template.ParseTemplate(modelData);
        }

        //new
        /// <summary>
        /// Method invokes the Notification data publisher Actions and returns the json object to deserialise
        /// </summary>
        /// <param name="fetchUrl">url to hit the datapublisher controller </param>
        /// <param name="eventId">Event Id</param>
        /// <returns>Returns String </returns>
        private string FetchDataFromNotificationDataPublisher(string fetchUrl, string eventId)
        {
            //Parameter Null check
            Guard.IsNotBlank(fetchUrl, "Fetch url");
            string dataPublisherUrl = string.Format("{0}/{1}", fetchUrl, eventId);
            WebClient webClient = new WebClient();
            return webClient.SafeWebClientProcessing(dataPublisherUrl, CommonHelper.GetHeader(Consumer.Orchard));
        }


        /// <summary>
        ///  Method invokes the Notification data publisher Actions and returns the json object to deserialise
        /// </summary>
        /// <param name="controllerActionName">Action Name</param>
        /// <param name="chireData">CHire Data</param>
        /// <returns>returns string</returns>
        private string PushDataToNotificationDataPublisher(string controllerActionName, CHireData chireData)
        {
            //Parameter Null check
            Guard.IsNotBlank(controllerActionName, "Controller Action Name");

            string postUrl = BuildWebControllerUrl(dataPublisherUrl, controllerActionName);

            WebClient webClient = new WebClient();
            return webClient.WebClientPostRequest(postUrl, chireData, CommonHelper.GetHeader(Consumer.Orchard));
        }

        /// <summary>
        ///  Method invokes the Notification data publisher Actions and returns the json object to deserialise
        /// </summary>
        /// <param name="pushUrl">url to hit the C2C controller</param>
        /// <param name="onBoardingEntity">OnBoardingEntity</param>
        /// <returns>returns string</returns>
        //NEW
        private string PushDataToNotificationDataPublisher(string pushUrl, OnBoardingEntity onBoardingEntity)
        {
            //Parameter Null check
            Guard.IsNotBlank(pushUrl, "Url");

            string postUrl = dataPublisherUrl + pushUrl;

            WebClient webClient = new WebClient();
            return webClient.WebClientPostRequest(postUrl, onBoardingEntity, CommonHelper.GetHeader(Consumer.Orchard));
        }


        //new 
        private string PushDataToNotificationFeeder(string postUrl, string postBaseUrl, string notificationContent)
        {
            string webClientPostResponse = "No Response Received";
            string url = string.Empty;
            //Parameter Null check
            Guard.IsNotBlank(postUrl, "Controller Name");
            Guard.IsNotNull(notificationContent, "NotificationContent Data");
            //Append the incomming Controller name and build the Web controller URL....
            if (postBaseUrl == "Hub")
                url = notificationFeedUrl + postUrl;
            if (postBaseUrl == "C2C")
                url = dataPublisherUrl + postUrl;


            WebClient webClient = new WebClient();
            webClientPostResponse = webClient.WebClientPostRequest(url, notificationContent, CommonHelper.GetHeader(Consumer.Hub));
            Logger.Debug(webClientPostResponse);

            return webClientPostResponse;
        }

        /// <summary>
        /// Method invokes the Notification data publisher Actions for fetching To Address and returns the json object to deserialise
        /// </summary>
        /// <param name="controllerActionName">Action Name </param>
        /// <param name="eventId">Event Id</param>
        /// <returns>Returns String </returns>
        private string FetchToAddressDataFromNotificationDataPublisher(string controllerActionName, string eventId, int pageNo)
        {
            List<string> defaultAdminMailAddress = new List<string>();
            try
            {
                //Parameter Null check
                Guard.IsNotBlank(controllerActionName, "Controller Action Name");

                string dataPublisherUrl = BuildToAddressUrl(controllerActionName, eventId, pageNo);
                WebClient webClient = new WebClient();
                return webClient.SafeWebClientProcessing(dataPublisherUrl, CommonHelper.GetHeader(Consumer.Orchard));
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.ToUpper().Contains("CLIENT CALL TO CONTROLLER")) // Handle when the To address retrival fails...
                    defaultAdminMailAddress.Add("revathi.i@cognizant.com");
            }
            string toAdddress = SerializationHelper.JsonSerialize(defaultAdminMailAddress);
            return toAdddress;
        }

        private string FetchErrorToAddressDataFromNotificationDataPublisher(string controllerActionName)
        {
            List<string> defaultAdminMailAddress = new List<string>();
            try
            {
                //Parameter Null check
                Guard.IsNotBlank(controllerActionName, "Controller Action Name");

                string dataPublisherUrl = BuildToAddressForErrorUrl(controllerActionName);
                WebClient webClient = new WebClient();
                return webClient.SafeWebClientProcessing(dataPublisherUrl, CommonHelper.GetHeader(Consumer.Orchard));
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.ToUpper().Contains("CLIENT CALL TO CONTROLLER")) // Handle when the To address retrival fails...
                    defaultAdminMailAddress.Add("revathi.i@cognizant.com");
            }
            string toAdddress = SerializationHelper.JsonSerialize(defaultAdminMailAddress);
            return toAdddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerActionName"></param>
        /// <returns></returns>
        /// //new
        private string FetchTaskSchedulerData(string fetchUrl)
        {
            //Parameter Null check
            Guard.IsNotBlank(fetchUrl, "Fetch Url");

            string url = string.Format("{0}/{1}", dataPublisherUrl, fetchUrl);
            WebClient webClient = new WebClient();
            return webClient.SafeWebClientProcessing(url, CommonHelper.GetHeader(Consumer.Orchard));
        }


        /// <summary>
        /// Constructs Url based on the Notification Data Publisher Url, Action Name.
        /// </summary>
        /// <param name="controllerActionName">Action Name</param>        
        /// <returns>Returns string</returns>
        private string BuildWebControllerUrl(string postUrl, string controllerActionName)
        {
            Guard.IsNotBlank(postUrl, "Contoller Name");
            return string.Format("{0}/{1}", postUrl, controllerActionName);
        }

        /// <summary>
        /// Constructs Url based on the Notification Data Publisher Url, Action Name and Event id
        /// </summary>
        /// <param name="controllerActionName">Action Name</param>
        /// <param name="eventId">Event Id</param>
        /// <returns>Returns string</returns>
        private string BuildToAddressUrl(string controllerActionName, string eventId, int pageNo)
        {
            Guard.IsNotBlank(dataPublisherUrl, "Notification Data Publisher Url");
            return string.Format("{0}/{1}?eventCode={2}&PageNo={3}", dataPublisherUrl, controllerActionName, eventId, pageNo);
        }

        private string BuildToAddressForErrorUrl(string controllerActionName)
        {
            Guard.IsNotBlank(dataPublisherUrl, "Notification Data Publisher Url");
            return string.Format("{0}/{1}", dataPublisherUrl, controllerActionName);
        }

        private dynamic ProcessUserData(List<CHireData> userList, string fileName)
        {
            List<ProcessResult> blobprocessResults = new List<ProcessResult>();
            Parallel.ForEach(userList, currentFile =>
            {

                ProcessResult processResult = SerializationHelper.JsonDeserialize<ProcessResult>(PushDataToNotificationDataPublisher
                                                                                                                       (
                                                                                                                       DefaultValue.CHIRE_IMPORT_ACTION, currentFile
                                                                                                                       ));

                //TableHelper.CreateCHireLog<CHireImportLogEntity>(new CHireImportLogEntity()
                //  {
                //      PartitionKey = fileName,
                //      RowKey = processResult.CandidateId,
                //      Status = processResult.Status,
                //      Message = processResult.Message
                //  });



                tableHelper.InsertRow<CHireImportLogEntity>(new
                    CHireImportLogEntity()
                {
                    PartitionKey = fileName,
                    RowKey = processResult.CandidateId,
                    Status = processResult.Status,
                    Message = processResult.Message
                }, DefaultValue.CHIRE_LOG_TABLE);


                blobprocessResults.Add(processResult);

            } //close lambda expression
             ); //close method invocation 


            //Prepare Summary Message
            Logger.Debug("Preparing summary message for CHIRE data.. ");
            string blobSummaryMessage = CHireManager.WriteSummaryMessage(blobprocessResults);

            //TableHelper.CreateCHireLog<CHireImportLogEntity>(new CHireImportLogEntity()
            //{
            //    PartitionKey = fileName,
            //    RowKey = fileName,
            //    Status = "Success",
            //    Message = blobSummaryMessage

            //});

            tableHelper.InsertRow<CHireImportLogEntity>(new CHireImportLogEntity()
            {
                PartitionKey = fileName,
                RowKey = fileName,
                Status = "Success",
                Message = blobSummaryMessage

            }, DefaultValue.CHIRE_LOG_TABLE);


            int blobSuccessCount = CHireManager.GetSuccessCount(blobprocessResults);
            List<ProcessResult> blobFailureList = CHireManager.GetFailureList(blobprocessResults);
            dynamic result = new
            {
                Status = "Success",
                SuccessCount = blobSuccessCount,
                FailureCount = blobFailureList.Count,
                FailureList = blobFailureList,
                Subject = "CHire Import Results",
                SummaryMessage = blobSummaryMessage
            };
            return result;
        }

        #endregion

        public INotificationRelay NotificationRelay { get; set; }
    }
}
