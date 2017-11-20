using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Object that stores the information on a log
    /// </summary>
    [Serializable()]
    public class cLog
    {
        private int nLogID;
        private string sLogName;
        private LogType eLogType;
        private int nNumSuccessfulUpdates;
        private int nNumFailedUpdates;
        private int nNumWarningUpdates;
        private int nNumOfExpectedFileLines;
        private int nNumOfProcessedFileLines;
        private List<cLogData> lstLogData;
        private DateTime dtCreatedOn;
        private DateTime? dtModifiedOn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="logName"></param>
        /// <param name="loggingType"></param>
        /// <param name="numSuccessfulUpdates"></param>
        /// <param name="numFailedUpdates"></param>
        /// <param name="numWarningUpdates"></param>
        /// <param name="numOfExpectedFileLines"></param>
        /// <param name="numOfProcessedFileLines"></param>
        /// <param name="logData"></param>
        /// <param name="createdOn"></param>
        /// <param name="modifiedOn"></param>
        public cLog(int logID, string logName, LogType loggingType, int numSuccessfulUpdates, int numFailedUpdates, int numWarningUpdates, int numOfExpectedFileLines, int numOfProcessedFileLines, List<cLogData> logData, DateTime createdOn, DateTime? modifiedOn)
        {
            nLogID = logID;
            sLogName = logName;
            eLogType = loggingType;
            nNumSuccessfulUpdates = numSuccessfulUpdates;
            nNumFailedUpdates = numFailedUpdates;
            nNumWarningUpdates = numWarningUpdates;
            nNumOfExpectedFileLines = numOfExpectedFileLines;
            nNumOfProcessedFileLines = numOfProcessedFileLines;
            lstLogData = logData;
            dtCreatedOn = createdOn;
            dtModifiedOn = modifiedOn;
        }

        #region Properties

        /// <summary>
        /// ID of the log
        /// </summary>
        public int logID
        {
            get { return nLogID; }
        }

        /// <summary>
        /// Name of the log
        /// </summary>
        public string logName
        {
            get { return sLogName; }
        }

        /// <summary>
        /// The Enumerable type of log, what it will be used with
        /// </summary>
        public LogType loggingType
        {
            get { return eLogType; }
        }

        /// <summary>
        /// The count of successfully run lines
        /// </summary>
        public int numSuccessfulUpdates
        {
            get { return nNumSuccessfulUpdates; }
        }

        /// <summary>
        /// The count of failed lines
        /// </summary>
        public int numFailedUpdates
        {
            get { return nNumFailedUpdates; }
        }

        /// <summary>
        /// The count of warning lines
        /// </summary>
        public int numWarningUpdates
        {
            get { return nNumWarningUpdates; }
        }

        /// <summary>
        /// The count of expected file lines
        /// </summary>
        public int numOfExpectedFileLines
        {
            get { return nNumOfExpectedFileLines; }
        }

        /// <summary>
        /// The count of processed file lines
        /// </summary>
        public int numOfProcessedFileLines
        {
            get { return nNumOfProcessedFileLines; }
        }

        /// <summary>
        /// List of log data items for this log
        /// </summary>
        public List<cLogData> logData
        {
            get { return lstLogData; }
        }

        /// <summary>
        /// Date object created on
        /// </summary>
        public DateTime createdOn
        {
            get { return dtCreatedOn; }
        }

        /// <summary>
        /// Date object modified on
        /// </summary>
        public DateTime? modifiedOn
        {
            get { return dtModifiedOn; }
        }

        #endregion
    }

    [Serializable()]
    public class cLogData
    {
        private int nLogID;
        private int nLogReasonID;
        private int nLogElementID;
        private string sLogDataItem;
        private DateTime dtCreatedOn;

        public cLogData(int logID, int logReasonID, int logElementID, string logDataItem, DateTime createdOn)
        {
            nLogID = logID;
            nLogReasonID = logReasonID;
            nLogElementID = logElementID;
            sLogDataItem = logDataItem;
            dtCreatedOn = createdOn;
        }

        #region Properties

        /// <summary>
        /// ID of the associated log
        /// </summary>
        public int logID
        {
            get { return nLogID; }
        }

        /// <summary>
        /// ID of the associated log reason 
        /// </summary>
        public int logReasonID
        {
            get { return nLogReasonID; }
        }

        /// <summary>
        /// ID of the associated metabase element
        /// </summary>
        public int logElementID
        {
            get { return nLogElementID; }
        }

        /// <summary>
        /// The log data item 
        /// </summary>
        public string logDataItem
        {
            get { return sLogDataItem; }
        }

        /// <summary>
        /// Date the object was created
        /// </summary>
        public DateTime createdOn
        {
            get { return dtCreatedOn; }
        }

        #endregion
    }

    /// <summary>
    /// Object that stores information on the log errors
    /// </summary>
    [Serializable()]
    public class cLogErrorReason
    {
        private int nLogReasonID;
        private LogReasonType eLogReasonType;
        private string sReason;
        private DateTime dtCreatedOn;
        private DateTime dtModifiedOn;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="logReasonID"></param>
        /// <param name="logReasonType"></param>
        /// <param name="reason"></param>
        /// <param name="createdOn"></param>
        /// <param name="modifiedOn"></param>
        public cLogErrorReason(int logReasonID, LogReasonType logReasonType, string reason, DateTime createdOn, DateTime modifiedOn)
        {
            nLogReasonID = logReasonID;
            eLogReasonType = logReasonType;
            sReason = reason;
            dtCreatedOn = createdOn;
            dtModifiedOn = modifiedOn;
        }

        #region Properties

        /// <summary>
        /// ID of the log reason
        /// </summary>
        public int logReasonID
        {
            get { return nLogReasonID; }
        }

        /// <summary>
        /// Enumerable type of the type of log reason
        /// </summary>
        public LogReasonType logReasonType
        {
            get { return eLogReasonType; }
        }

        /// <summary>
        /// The log error reason
        /// </summary>
        public string reason
        {
            get { return sReason; }
        }

        /// <summary>
        /// Date the object was created
        /// </summary>
        public DateTime createdOn
        {
            get { return dtCreatedOn; }
        }

        /// <summary>
        /// User who created the object
        /// </summary>
        public DateTime modifiedOn
        {
            get { return dtModifiedOn; }
        }

        #endregion
    }

    /// <summary>
    /// The type of log created
    /// </summary>
    [Serializable()]
    public enum LogType
    {
        /// <summary>
        /// Implementation spreadsheet import
        /// </summary>
        SpreadsheetImport = 0,

        /// <summary>
        /// ESR Import
        /// </summary>
        ESROutboundImport = 1,

        /// <summary>
        /// The ESR out bound import v 2.
        /// </summary>
        EsrOutBoundImportV2 = 2
    }

    /// <summary>
    /// Values for the log item reason type
    /// </summary>
    [Serializable()]
    public enum LogReasonType
    {
        None = 0,
        MaxLengthExceeded,
        MandatoryField,
        UniqueField,
        WrongDataType, 
        Success, 
        Warning,
        Error,
        SQLError,
        SuccessAdd,
        SuccessUpdate,
        SuccessDelete
    }

    /// <summary>
    /// This filters the actual log information shown to the user
    /// </summary>
    [Serializable()]
    public enum LogViewType
    {
        /// <summary>
        /// Show all log information
        /// </summary>
        All = 0,

        /// <summary>
        /// Show just the summary log information
        /// </summary>
        Summary,

        /// <summary>
        /// Show all the item log information, no summary
        /// </summary>
        Items,

        /// <summary>
        /// Show the log information for records added
        /// </summary>
        Added,

        /// <summary>
        /// Show the log information for records updated
        /// </summary>
        Updated,

        /// <summary>
        /// Show the log information for records deleted
        /// </summary>
        Deleted,

        /// <summary>
        /// Show the log information for Successes
        /// </summary>
        Successes,

        /// <summary>
        /// Show the log information for Failures
        /// </summary>
        Failures,

        /// <summary>
        /// Show the log information for Warnings
        /// </summary>
        Warnings,

        /// <summary>
        /// Show the log information for Unclassified items
        /// </summary>
        Other
    }

    /// <summary>
    /// Static values used for the logs
    /// </summary>
    [Serializable()]
    public static class cLoggingValues
    {
        public static string filler1 = "########";
        public static string space = " ";
        public static string timeStamp = DateTime.Now.ToString();
    }
}
