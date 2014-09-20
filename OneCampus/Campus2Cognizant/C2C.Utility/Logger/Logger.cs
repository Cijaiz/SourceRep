namespace C2C.Core.Logger
{
    #region Reference

    using System.Diagnostics;
    using C2C.Core.Logger.Skeleton;
    using C2C.Core.Logger.Structure;
    using Microsoft.WindowsAzure.Diagnostics;
    
    #endregion

    /// <summary>
    /// To log information.
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// Variable to hold current logger instance.
        /// </summary>
        private static ILogger log = null;

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            if (Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
            {
                log = new AzureLogger();
            }
            else
            {
                log = new EventLogger();
            }
        }

        /// <summary>
        /// Writes the information message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public static void Debug(string message)
        {
            log.WriteInformation(message);
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
        public static void Debug(string format, params object[] args)
        {
            log.WriteInformation(format, args);
        }

        /// <summary>
        /// Writes the error message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public static void Error(string message)
        {
            log.WriteError(message);
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
        public static void Error(string format, params object[] args)
        {
            log.WriteError(format, args);
        }

        /// <summary>
        /// Flushes the output buffer, and causes buffered data to be written to the listeners.
        /// </summary>
        public static void Flush()
        {
            log.Flush();
        }
        /// <summary>
        /// Writes the warning message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public static void Warning(string message)
        {
            log.WriteWarning(message);
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
        public static void Warning(string format, params object[] args)
        {
            log.WriteWarning(format, args);
        }
    }
}
