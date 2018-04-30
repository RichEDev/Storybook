namespace APICallsHelper
{
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
   
    /// <summary>
    /// Class to log the events on the event log of windows operating system
    /// </summary>
    public class EventLogger
    {
        /// <summary>
        /// Project name used to log in the enventLog.
        /// </summary>
        private const string ProjectName = "APICallsHelper";

        /// <summary>
        /// Log an entry of the status in windows event log.
        /// </summary>
        /// <param name="actionName">The operation name</param>
        /// <param name="action">The url which was called</param>
        /// <param name="message">Message that needs to be logged</param>
        public void MakeEventLogEntry(string actionName, string action, string message)
        {
            var entry =
                $"{actionName}. URL: {"http://" + ConfigurationManager.AppSettings["domain"]}/{action}. Message : {message} . ";
            EventLog.WriteEntry(ProjectName, entry);
        }

        /// <summary>
        /// Log an entry of the status in windows event log.
        /// </summary>
        /// <param name="actionName">The operation name</param>
        /// <param name="action">The url which was called</param>
        /// <param name="message">Message that needs to be logged</param>
        /// <param name="statusCode">Instance of type <see cref="HttpStatusCode"/> which is the result of api request.</param>
        public void MakeEventLogEntry(string actionName, string action, string message, HttpStatusCode statusCode)
        {
            var entry =
                $"{actionName}. URL: {"http://" + ConfigurationManager.AppSettings["domain"]}/{action}. Message : {message} . Status code: ({statusCode})";
            if (statusCode == HttpStatusCode.OK)
            {
                EventLog.WriteEntry(ProjectName, entry);
                return;
            }

            EventLog.WriteEntry(ProjectName, entry, EventLogEntryType.Error);
        }
    }
}
