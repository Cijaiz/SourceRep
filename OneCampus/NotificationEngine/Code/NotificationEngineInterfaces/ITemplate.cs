using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octane.NotificationEngineInterfaces
{
    /// <summary>
    /// Methods for Parsing Cshtml Templates
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Holds the template's content.
        /// </summary>
        string BodyContent { get; set; }

        /// <summary>
        /// Check whether the processing is an email notification or a batch processing.
        /// </summary>
        bool Notify { get; set; }

        /// <summary>
        /// Downloads the cshtml template based on the URL.
        /// </summary>
        /// <param name="templateUri">Template URL</param>
        /// <returns>Returns bool</returns>
        bool DownloadTemplate(string templateUri);

        /// <summary>
        /// Parses the template with the model data.
        /// </summary>
        /// <typeparam name="T">Any Type</typeparam>
        /// <param name="modelData">Model</param>
        /// <returns>Returns String</returns>
        string ParseTemplate<T>(T modelData);
    }
}
