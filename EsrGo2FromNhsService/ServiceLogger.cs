using System;

namespace EsrGo2FromNhsService
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;

    public static class ServiceLogger
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Writes an error entry to the event log if extended logging is enabled
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void Error(string message, params object[] args)
        {
            Event(EventLogEntryType.Error, message, args);
        }

        /// <summary>
        ///     Writes an error entry to the event log
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void ImportantError(string message, params object[] args)
        {
            Event(EventLogEntryType.Error, message, args);
        }

        /// <summary>
        ///     Writes an information entry to the event log
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void ImportantInformation(string message, params object[] args)
        {
            Event(EventLogEntryType.Information, message, args);
        }

        /// <summary>
        ///     Writes an information entry to the event log if extended logging is enabled
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void Information(string message, params object[] args)
        {
            Event(EventLogEntryType.Information, message, args);
        }

        /// <summary>
        ///     Writes a success audit information entry to the event log if extended logging is enabled
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void SuccessAudit(string message, params object[] args)
        {
            Event(EventLogEntryType.SuccessAudit, message, args);
        }

        /// <summary>
        ///     Writes a warning entry to the event log if extended logging is enabled
        /// </summary>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        public static void Warning(string message, params object[] args)
        {
            Event(EventLogEntryType.Warning, message, args);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The standard thread identifiers to add to all event log entries
        /// </summary> 
        /// <returns>Two lines with the old and new style thread identifiers</returns>
        private static string CurrentThreadIdentifier()
        {
            return string.Format("[{0}] AppDomain.GetCurrentThreadId()\n[{1}] Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)\n\n", AppDomain.GetCurrentThreadId(), Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
        }                                                                                                                   

        /// <summary>
        ///     Writes an entry to the provided Event Log
        /// </summary>
        /// <param name="eventLogEntryType">The type of entry to write - <value>Information, Warning, Error, etc</value></param>
        /// <param name="message">The text to write, can contain format tokens</param>
        /// <param name="args">The variables to use with the format tokens</param>
        private static void Event(EventLogEntryType eventLogEntryType, string message, params object[] args)
        {
            message = CurrentThreadIdentifier() + message;
            const string LogName = "Esr Nhs Go2 From Nhs Log";
            const string SourceName = "EsrGo2FromNhsService";

            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, LogName);
            }

            var eventLog = new EventLog("")
            {
                Source = SourceName
            };

            eventLog.WriteEntry(string.Format(message, args), eventLogEntryType);
        }

        #endregion
    }
}
