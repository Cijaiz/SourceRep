namespace C2C.Core.Logger.Skeleton
{
    /// <summary>
    /// Logger interface exposes custom logging methods.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Flushes the output buffer, and causes buffered data to be written to the listeners.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes the error message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        void WriteError(string message);

        /// <summary>
        /// Writes an error message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void WriteError(string format, params object[] args);

        /// <summary>
        /// Writes the information message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        void WriteInformation(string message);

        /// <summary>
        /// Writes an information message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void WriteInformation(string format, params object[] args);

        /// <summary>
        /// Writes the warning message to the listener.
        /// </summary>
        /// <param name="message">A message to write.</param>
        void WriteWarning(string message);

        /// <summary>
        /// Writes an warning message to the trace listeners
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond
        /// to objects in the args array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        void WriteWarning(string format, params object[] args);
    }
}
