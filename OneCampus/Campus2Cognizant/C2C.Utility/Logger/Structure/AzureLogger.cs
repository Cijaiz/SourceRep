namespace C2C.Core.Logger.Structure
{
    #region Reference
    using C2C.Core.Logger.Skeleton;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using System;
    using System.Diagnostics;
    
    #endregion

    /// <summary>
    /// To log information in azure environment.
    /// </summary>
    internal class AzureLogger : ILogger
    {
        /// <summary>
        /// Initializes static members of the <see cref="AzureLogger"/> class.
        /// Initialize DiagnosticMonitorTraceListener for Azure Logger.
        /// </summary>
        static AzureLogger()
        {
            if (!Trace.Listeners.Contains(new DiagnosticMonitorTraceListener()))
            {
                // Create a trace listener for the DiagnosticMonitor.
                DiagnosticMonitorTraceListener myTraceListener = new DiagnosticMonitorTraceListener();

                // Add the event log trace listener to the collection.
                Trace.Listeners.Add(myTraceListener);

                SetCustomConfiguration();
            }
        }

        /// <summary>
        /// Flushes the output buffer, and causes buffered data to be written to the listeners.
        /// </summary>
        public void Flush()
        {
            Trace.Flush();
        }

        /// <summary>
        /// Writes the error message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public void WriteError(string message)
        {
            Trace.TraceError(message);
        }

        /// <summary>
        /// Writes an error message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public void WriteError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }

        /// <summary>
        /// Writes the information message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public void WriteInformation(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Writes an information message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public void WriteInformation(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Writes the warning message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public void WriteWarning(string message)
        {
            Trace.TraceWarning(message);
        }

        /// <summary>
        /// Writes an warning message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public void WriteWarning(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
        }

        /// <summary>
        /// To get log level from settings
        /// </summary>
        /// <returns>Trace LogLevel</returns>
        private static LogLevel GetLogLevel()
        {
            var myLogLevel = LogLevel.Error;
            try
            {
                var logLevelFilter = CloudConfigurationManager.GetSetting(C2C.Core.Constants.C2CWeb.Key.DIAGNOSTICLOGLEVEL);
                switch (logLevelFilter)
                {
                    case "Information":
                        myLogLevel = LogLevel.Information;
                        break;
                    case "Verbose":
                        myLogLevel = LogLevel.Verbose;
                        break;
                    case "Warning":
                        myLogLevel = LogLevel.Warning;
                        break;
                    case "Critical":
                        myLogLevel = LogLevel.Critical;
                        break;
                    case "Error":
                        myLogLevel = LogLevel.Error;
                        break;
                    case "Undefined":
                        myLogLevel = LogLevel.Undefined;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error:{0}\n Stack Trace:{1}", ex.Message, ex.StackTrace));
            }

            return myLogLevel;
        }

        /// <summary>
        /// To set custom configuration for Diagnostic Monitor
        /// </summary>
        private static void SetCustomConfiguration()
        {
            try
            {
                DiagnosticMonitorConfiguration diagnosticConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
                var logLevel = GetLogLevel();

                // Set the transfer period for File Based Logs( IIS logs and IIS failed Request logs)
                diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

                // Configure the Windows Application and System Event logs with transfer interval to azure storage as 1 minute
                // Event log collection will be at verbose level so that it collects the complete detail
                // diagnosticConfig.WindowsEventLog.DataSources.Add("Application!*"); [System[Provider[@Name='MailingService']]]
                // Uncomment Below to narrow down Application logs collection to Web Role level
                diagnosticConfig.WindowsEventLog.DataSources.Add("Application!*");
                
                // diagnosticConfig.WindowsEventLog.DataSources.Add("System!*");
                diagnosticConfig.WindowsEventLog.BufferQuotaInMB = 100;
                diagnosticConfig.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
                diagnosticConfig.WindowsEventLog.ScheduledTransferLogLevelFilter = logLevel;

                // Configure the Trace logs with transfer interval to azure storage as 1 minute
                // Log level is configured as Undefined, so that it logs all events
                diagnosticConfig.Logs.ScheduledTransferLogLevelFilter = logLevel;
                diagnosticConfig.Logs.ScheduledTransferPeriod = System.TimeSpan.FromMinutes(1);
                diagnosticConfig.Logs.BufferQuotaInMB = 100;

                // Starts the Diagnostics Monitoring Service with the specified connection string and Diagnostics Configuration
                DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticConfig);

                Trace.WriteLine(string.Format("DiagnosticMonitor configuration updated, Log level {0}", logLevel.ToString()));
            }
            catch (Exception ex)
            {
                // Traces the diagnostics failure of the current role instance
                Trace.WriteLine(
                    string.Format(
                        "Diagnostics count not be started for Instance: {0}. Find below the error details:{1}Message:{2}{1}StackTrace:{1}{3}",
                        RoleEnvironment.CurrentRoleInstance, 
                        Environment.NewLine, 
                        ex.Message,
                        ex.StackTrace), 
                    "DiagnosticsError");
            }
        }
    }
}
