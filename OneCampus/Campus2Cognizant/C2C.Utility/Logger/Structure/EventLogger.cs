namespace C2C.Core.Logger.Structure
{
    #region Reference
    using C2C.Core.Logger.Skeleton;
    using System.Diagnostics;
    #endregion

    /// <summary>
    /// To log information in event viewer.
    /// </summary>
    internal class EventLogger : ILogger
    {
        /// <summary>
        /// Initializes static members of the <see cref="EventLogger"/> class.
        /// Initialize EventLogTraceListener for event Logger.
        /// </summary>
        static EventLogger()
        {
            if (!Trace.Listeners.Contains(new EventLogTraceListener()))
            {
                // Create a trace listener for the event log.
                EventLogTraceListener myTraceListener = new EventLogTraceListener("C2C_EventLogSource");

                // Add the event log trace listener to the collection.
                Trace.Listeners.Add(myTraceListener);
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
    }
}
