namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The sel log reader. 
    /// Usage: Create an instance, set the MachineName, Log and NumberOfRecords, 
    /// call the GetEvents method. Use the QueryResult property for success/failure with the LogQueryException property if required. 
    /// A list of EventLogEntries are returned for parsing. Null is returned for no records or error.
    /// </summary>
    [Serializable]
    public class LogReader
    {
        /// <summary>
        /// Gets or sets the internal event log.
        /// </summary>
        private EventLog eventLog;

        /// <summary>
        /// The event type to interrogate.
        /// </summary>
        [Serializable]
        public enum LogType
        {
            /// <summary>
            /// The sql server.
            /// </summary>
            SqlServer = 0,

            /// <summary>
            /// The sel application.
            /// </summary>
            SelApplication = 1,

            /// <summary>
            /// The expenses reports application.
            /// </summary>
            ExpensesReports = 2,

            /// <summary>
            /// The framework reports application.
            /// </summary>
            FrameworkReports = 3,

            /// <summary>
            /// The expenses scheduler.
            /// </summary>
            ExpensesScheduler = 4,

            /// <summary>
            /// The framework scheduler.
            /// </summary>
            FrameworkScheduler = 5,

            /// <summary>
            /// The scheduler.
            /// </summary>
            Miscellaneous = 6,

            /// <summary>
            /// The application.
            /// </summary>
            Application = 7,

            /// <summary>
            /// The system.
            /// </summary>
            System = 8,

            /// <summary>
            /// The ESR NHS Hub service.
            /// </summary>
            EsrNhsHub = 9,

            /// <summary>
            /// The ESR Router service.
            /// </summary>
            EsrRouter = 10
        }

        /// <summary>
        /// The log query result.
        /// </summary>
        [Serializable]
        public enum LogQueryResult
        {
            /// <summary>
            /// The machine not found on network.
            /// </summary>
            MachineNotFound,

            /// <summary>
            /// The log source was not found.
            /// </summary>
            LogNotFound,

            /// <summary>
            /// General error.
            /// </summary>
            GeneralError,

            /// <summary>
            /// Retrieval Success.
            /// </summary>
            Success
        }

        /// <summary>
        /// Gets the log query exception, or returns null.
        /// </summary>
        public Exception LogQueryException { get; private set; }

        /// <summary>
        /// Gets or sets the Log Type.
        /// </summary>
        public LogType Log { get; set; }

        /// <summary>
        /// Gets the query result from the last execution of GetEvents.
        /// </summary>
        public LogQueryResult QueryResult { get; private set; }

        /// <summary>
        /// Gets or sets the machine name to use for the reader.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the number of records to be retrieved.
        /// </summary>
        public int NumberOfRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the date order of records to be retrieved.
        /// </summary>
        public SortDirection SortDirection { get; set; }
        
        /// <summary>
        /// Get the event log entries for the specified source. 
        /// </summary>
        /// <returns>
        /// A list of EventLogEntry
        /// </returns>
        public List<EventLogEntry> GetEvents()
        {
            if (this.eventLog == null)
            {
                this.eventLog = new EventLog();
            }

            this.LogQueryException = null;
            this.eventLog.MachineName = this.MachineName;
            this.NumberOfRecords = this.NumberOfRecords == 0 ? 10 : this.NumberOfRecords;
            this.SortDirection = this.SortDirection == SortDirection.None ? SortDirection.Descending : this.SortDirection;
            List<EventLogEntry> eventLogEntries;

            try
            {
                switch (this.Log)
                {
                    case LogType.SqlServer:
                        this.eventLog.Log = "Application";
                        this.eventLog.Source = "MSSQLSERVER";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.SelApplication:
                        this.eventLog.Log = "0";
                        this.eventLog.Source = "0";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.ExpensesReports:
                        this.eventLog.Log = "expenses";
                        this.eventLog.Source = "expenses";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("ReportEngine"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("ReportEngine"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.FrameworkReports:
                        this.eventLog.Log = "contracts";
                        this.eventLog.Source = "contracts";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("ReportEngine"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("ReportEngine"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.ExpensesScheduler:
                        this.eventLog.Log = "expenses";
                        this.eventLog.Source = "expenses";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("Scheduler"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("Scheduler"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.FrameworkScheduler:
                        this.eventLog.Log = "contracts";
                        this.eventLog.Source = "contracts";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("Scheduler"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Where(x => x.Message.Contains("Scheduler"))
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.Miscellaneous:
                        this.eventLog.Log = "9";
                        this.eventLog.Source = "9";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.System:
                        this.eventLog.Log = "System";
                        this.eventLog.Source = string.Empty;
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.EsrNhsHub:
                        this.eventLog.Log = "ESR Services";
                        this.eventLog.Source = "File Transfer Service - ESR NHS Hub";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    case LogType.EsrRouter:
                        this.eventLog.Log = "ESR Services";
                        this.eventLog.Source = "File Transfer Service - ESR Router";
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                    default:
                        this.eventLog.Log = "Application";
                        this.eventLog.Source = string.Empty;
                        eventLogEntries = this.SortDirection == SortDirection.Descending
                                              ? this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderByDescending(x => x.TimeGenerated)
                                                    .Where(a => a.Source != "MSSQLSERVER")
                                                    .Take(this.NumberOfRecords)
                                                    .ToList()
                                              : this.eventLog.Entries.Cast<EventLogEntry>()
                                                    .OrderBy(x => x.TimeGenerated)
                                                    .Where(a => a.Source != "MSSQLSERVER")
                                                    .Take(this.NumberOfRecords)
                                                    .ToList();
                        break;
                }

                this.QueryResult = LogQueryResult.Success;
            }
            catch (IOException)
            {
                // machine doesn't exist or not found
                eventLogEntries = null;
                this.QueryResult = LogQueryResult.MachineNotFound;
            }
            catch (InvalidOperationException)
            {
                // log doesn't exist for this machine. Pass back empty result set.
                eventLogEntries = null;
                this.QueryResult = LogQueryResult.LogNotFound;
            }
            catch (Exception ex)
            {
                // other error
                eventLogEntries = null;
                this.LogQueryException = new Exception("EventLog Query Error", ex);
                this.QueryResult = LogQueryResult.GeneralError;
            }

            return eventLogEntries;
        }
    }
}
