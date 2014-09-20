using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Octane.NotificationEngineInterfaces
{
    /// <summary>
    /// Methods for Loading all the templates from Blob
    /// </summary>
    public interface ITemplateLoader
    {
        string ConfigurationFilePath { get; set; }
        ConcurrentDictionary<string, ITemplate> LoadAllTemplates();
    }
}
