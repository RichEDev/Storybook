using System.Diagnostics;

namespace SpendManagementLibrary
{
    public class cEventlog
    {
        /// <summary>
        /// Used for logging debug information
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void LogEntry(string message)
        {
            LogEntry(message, false, EventLogEntryType.Information, ErrorCode.DebugInformation);
        }

        /// <summary>
        /// Stores a detailed entry into the eventlog for the specified module
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="includeStack">
        /// The include Stack.
        /// </param>
        /// <param name="logEntryType">
        /// The log Entry Type.
        /// </param>
        /// <param name="errorCode">
        /// The error Code.
        /// </param>
        public static void LogEntry(string message, bool includeStack, EventLogEntryType logEntryType, ErrorCode errorCode = ErrorCode.Unspecified) 
        {
            try
            {
                Modules activeModule = GlobalVariables.DefaultModule;
                if (EventLog.SourceExists(activeModule.ToString()) == false)
                {
                    EventLog.CreateEventSource(activeModule.ToString(), activeModule.ToString());
                }

                EventLog.WriteEntry(activeModule.ToString(), message, logEntryType, (int)errorCode);
            }
            catch
            {
                EventLog.WriteEntry("Application", "Unable to write event log entry.");
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// No error code specified
            /// </summary>
            Unspecified = 1,
            /// <summary>
            /// Generic information used to debug
            /// </summary>
            DebugInformation = 2
        }
    }
}
