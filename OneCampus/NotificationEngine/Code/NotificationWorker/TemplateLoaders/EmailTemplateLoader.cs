using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Octane.NotificationEngineInterfaces;
using System;
using System.IO;
using Octane.NotificationUtility;
using C2C.Core.Logger;
using C2C.Core.Helper;

namespace Octane.NotificationWorker.TemplateLoaders
{
    /// <summary>
    /// This loads all the templates and parses the templates with data.
    /// </summary>
    public class EmailTemplateLoader : ITemplateLoader
    {
        public string ConfigurationFilePath { get; set; }

        /// <summary>
        /// Loads all the templates.
        /// </summary>
        /// <returns>Concurrent Dictonary</returns>
        public ConcurrentDictionary<string, ITemplate> LoadAllTemplates()
        {
            ConcurrentDictionary<string, ITemplate> templates = new ConcurrentDictionary<string, ITemplate>();
            IEnumerable<KeyValuePair<string, string>> appsettings = GetEmailTemplateConfiguration(ConfigurationFilePath);
            if (appsettings != null && appsettings.Count() != 0)
            {
                ITemplate emailTemplate;
                foreach (var setting in appsettings)
                {
                    emailTemplate = new Template();

                    //Check this and if there is no template url specified in the xml then its considerd as a batch job..
                    if (string.IsNullOrEmpty(setting.Value))
                    {
                        emailTemplate.Notify = false;
                        templates.TryAdd(setting.Key, emailTemplate);
                    }
                    else //If there is a template URL specified then we should download into the dictiornary. for further processing.
                    {
                        if (emailTemplate.DownloadTemplate(setting.Value))
                        {
                            emailTemplate.Notify = true;
                            templates.TryAdd(setting.Key, emailTemplate);
                        }
                        else
                        {
                            Logger.Error(string.Format("The template at {0} couldn't be downloaded", setting.Value));
                        }
                    }
                }
            }
            else
            {
                Logger.Error("The XML configuration file has zero key-value pairs");
            }
            return templates;
        }

        /// <summary>
        /// Reads the appsettings section of the XML file.
        /// </summary>
        /// <param name="XmlUri">Blob Url</param>
        /// <returns>Dictionary containing the key/value pair</returns>
        private IDictionary<string, string> GetEmailTemplateConfiguration(string XmlUri)
        {
            XElement appsettings = LoadConfigurationFileFromBlob(XmlUri);
            Guard.IsNotNull(appsettings, "AppSettings object");

            return (from appSetting in appsettings.Descendants("add")
                    select new
                    {
                        key = appSetting.Attribute("key").Value,
                        value = appSetting.Attribute("value").Value
                    })
                    .ToDictionary(setting => setting.key, cc => cc.value);
        }

        /// <summary>
        /// Downloads the XML file containing the template configuration from blob.
        /// </summary>
        /// <param name="xmlFileUri">Blob Url</param>
        /// <returns>Returns XElement</returns>
        private XElement LoadConfigurationFileFromBlob(string xmlFileUri)
        {
            Guard.IsNotBlank(xmlFileUri, "Emai lTemplate Configuration File's URL");
            MemoryStream fileStream = null;
            XElement result = null;
            var contents = AzureHelper.DownloadContentsFromBlob(xmlFileUri);
            ///Check if the downloaded content is null
            fileStream = (contents != null) ? (MemoryStream)contents : null;

            Guard.IsNotNull(fileStream, "Memory stream object");

            fileStream.Position = 0;
            result = XElement.Load(fileStream);
            return result;
        }
    }
}
