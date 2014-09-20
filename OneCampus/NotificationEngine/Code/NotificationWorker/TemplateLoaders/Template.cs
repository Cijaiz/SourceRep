using System;
using Microsoft.WindowsAzure.Storage;
using Octane.NotificationEngineInterfaces;
using RazorEngine;
using Octane.NotificationUtility;
using System.IO;
using C2C.Core.Helper;
using C2C.Core.Logger;

namespace Octane.NotificationWorker.TemplateLoaders
{
    /// <summary>
    /// This downloads templates and parses the templates.
    /// </summary>
    public class Template : ITemplate
    {
        public string BodyContent { get; set; }
        public bool Notify { get; set; }

        /// <summary>
        /// Downloads the cshtml template based on the URL.
        /// </summary>
        /// <param name="templateUri">Template URL</param>
        /// <returns>Returns bool</returns>
        public bool DownloadTemplate(string templateUri)
        {
            bool isDownloaded = false;
            Guard.IsNotBlank("templateUri", "Blob Template URI");
            try
            {
                //Downloads the blob's contents as stream
                Stream templateStream = AzureHelper.DownloadContentsFromBlob(templateUri);
                // convert stream to string
                StreamReader reader = new StreamReader(templateStream);
                this.BodyContent = reader.ReadToEnd();
                isDownloaded = true;
            }
            catch (OutOfMemoryException outOfMemoryException)
            {
                Logger.Error("Out of Memory exception when reading the blob contents as stream");
                throw outOfMemoryException;
            }
            catch (IOException ioException)
            {
                Logger.Error("I/O error occured when reading blob contents");
                throw ioException;
            }
            catch (Exception generalException)
            {
                Logger.Error("Error while reading the contents of blob");
                throw generalException;
            }
            return isDownloaded;
        }

        public string ParseTemplate<T>(T modelData)
        {
            Guard.IsNotBlank("BodyContent", "BodyContent in ParseTemplate");
            Guard.IsNotNull(modelData, "Model Data in ParseTemplate");
            return Razor.Parse(BodyContent, modelData);
        }
    }
}
