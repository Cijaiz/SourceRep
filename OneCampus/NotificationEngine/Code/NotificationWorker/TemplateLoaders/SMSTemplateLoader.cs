using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane.NotificationEngineInterfaces;
using System.Collections.Concurrent;

namespace Octane.NotificationWorker.TemplateLoaders
{
    /// <summary>
    /// This loads all the templates and parses the templates with data.
    /// </summary>
    public class SMSTemplateLoader : ITemplateLoader
    {
        public string ConfigurationFilePath
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

        public ConcurrentDictionary<string, ITemplate> LoadAllTemplates()
        {
            throw new NotImplementedException();
        }
    }
}
