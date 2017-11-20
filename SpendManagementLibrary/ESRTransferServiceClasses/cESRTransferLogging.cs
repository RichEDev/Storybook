namespace SpendManagementLibrary.ESRTransferServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using BusinessLogic.Cache;
    using Common.Logging;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// This class will deal with the logging for the ESR Inbound and Outbound file processing
    /// </summary>
    [Serializable()]
    public class cESRTransferLogging
    {
        private string sConnectionString;
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cESRTransferLogging>().GetLogger();


        #region Properties

        /// <summary>
        /// The database connection string 
        /// </summary>
        public string ConnectionString
        {
            get { return sConnectionString; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public cESRTransferLogging(string ConnectionString)
        {
            this.sConnectionString = ConnectionString;
        }

        /// <summary>
        /// Add/Update a log item to the database
        /// </summary>
        /// <param name="logItem"></param>
        public void SaveLogItem(cESRTransferLogItem logItem)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info($"Save ESR Transfer Log Item - {logItem.Filename}  - - {logItem.LogItemType} -  {logItem.LogItemBody}");
            }

            using (var smData = new DatabaseConnection(ConnectionString))
            {
                smData.sqlexecute.Parameters.AddWithValue("@LogItemID", logItem.LogItemID);
                smData.sqlexecute.Parameters.AddWithValue("@LogItemType", (int)logItem.LogItemType);
                smData.sqlexecute.Parameters.AddWithValue("@TransferType", (int)logItem.TransferType);
                smData.sqlexecute.Parameters.AddWithValue("@NHSTrustID", logItem.NHSTrustID);
                smData.sqlexecute.Parameters.AddWithValue("@Filename", logItem.Filename);
                smData.sqlexecute.Parameters.AddWithValue("@EmailSent", logItem.EmailSent);

                if (logItem.LogItemBody == string.Empty)
                {
                    smData.sqlexecute.Parameters.AddWithValue("@LogItemBody", DBNull.Value);
                }
                else
                {
                    smData.sqlexecute.Parameters.AddWithValue("@LogItemBody", logItem.LogItemBody);
                }

                smData.ExecuteProc("SaveInterfaceLogItem");
                smData.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Get a list of transfer log items that need to be emailed
        /// </summary>
        /// <returns></returns>
        public SortedList<string, cESRTransferLogItem> GetTransferLogItemsToEmail()
        {
            SortedList<string, cESRTransferLogItem> lstLogItems;
            using (var smData = new DatabaseConnection(ConnectionString))
            {
                lstLogItems = new SortedList<string, cESRTransferLogItem>();
                TransferLogItemType logItemType;
                AutomatedTransferType transferType;

                using (var reader = smData.GetReader("GetInterfaceLogItemsToEmail", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var logItemId = reader.GetInt32(0);
                        logItemType = (TransferLogItemType)reader.GetByte(1);
                        transferType = (AutomatedTransferType)reader.GetByte(2);
                        var nhsTrustId = reader.GetInt32(3);
                        var fileName = reader.GetString(4);
                        var emailSent = reader.GetBoolean(5);

                        var logItemBody = "";
                        if(!reader.IsDBNull(6))
                        {
                            logItemBody = reader.GetString(6);
                        }

                        DateTime? createdOn = reader.GetDateTime(7);

                        DateTime? modifiedOn = null;
                        if (!reader.IsDBNull(8))
                        {
                            modifiedOn = reader.GetDateTime(8);
                        }

                        lstLogItems.Add($"{nhsTrustId}{createdOn}{logItemId}",  new cESRTransferLogItem(logItemId, logItemType, transferType, nhsTrustId, fileName, emailSent, logItemBody, createdOn, modifiedOn));
                    }

                    reader.Close();
                }
            }

            return lstLogItems;
        }
    }

    /// <summary>
    /// Object thats stores the ESR Transfer log item information
    /// </summary>
    public class cESRTransferLogItem
    {
        #region Class Fields

        private int nLogItemID;
        private TransferLogItemType enLogItemType;
        private AutomatedTransferType enTransferType;
        private int nNHSTrustID;
        private string sFilename;
        private bool bEmailSent;
        private string sLogItemBody;
        private DateTime? dtCreatedOn;
        private DateTime? dtModifiedOn;

        #endregion

        /// <summary>
        /// Constructor with all the parameter values to set for the class object
        /// </summary>
        /// <param name="LogItemID"></param>
        /// <param name="LogItemType"></param>
        /// <param name="TransferType"></param>
        /// <param name="NHSTrustID"></param>
        /// <param name="Filename"></param>
        /// <param name="EmailSent"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="ModifiedOn"></param>
        public cESRTransferLogItem(int LogItemID, TransferLogItemType LogItemType, AutomatedTransferType TransferType, int NHSTrustID, string Filename, bool EmailSent, string LogItemBody, DateTime? CreatedOn, DateTime? ModifiedOn)
        {
            nLogItemID = LogItemID;
            enLogItemType = LogItemType;
            enTransferType = TransferType;
            nNHSTrustID = NHSTrustID;
            sFilename = Filename;
            bEmailSent = EmailSent;
            sLogItemBody = LogItemBody;
            dtCreatedOn = CreatedOn;
            dtModifiedOn = ModifiedOn;
        }

        #region Properties

        /// <summary>
        /// Unique ID for the log item
        /// </summary>
        public int LogItemID
        {
            get { return nLogItemID; }
        }

        /// <summary>
        /// This is the log item type for example an Outbound file has downloaded 
        /// </summary>
        public TransferLogItemType LogItemType
        {
            get { return enLogItemType; }
        }

        /// <summary>
        /// The interface transfer type whether it be Outbound or Inbound
        /// </summary>
        public AutomatedTransferType TransferType
        {
            get { return enTransferType; }
        }

        /// <summary>
        /// The ID of the associated trust the trabsfer is processing for
        /// </summary>
        public int NHSTrustID
        {
            get { return nNHSTrustID; }
        }

        /// <summary>
        /// File name of the file being processed
        /// </summary>
        public string Filename
        {
            get { return sFilename; }
        }

        /// <summary>
        /// Flag to say if the log item has been sent via email or not
        /// </summary>
        public bool EmailSent
        {
            get { return bEmailSent; }
            set { this.bEmailSent = value; }
        }

        /// <summary>
        /// This contains any additional information about the log item
        /// </summary>
        public string LogItemBody
        {
            get { return sLogItemBody; }
        }

        /// <summary>
        /// The date and time the logitem was created on
        /// </summary>
        public DateTime? CreatedOn
        {
            get { return dtCreatedOn; }
        }

        /// <summary>
        /// The date and time the log item was modified on, usually the email sent flag being updated 
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }

        #endregion

    }

    /// <summary>
    /// Enumerable type for the log item ESR Transfer status
    /// </summary>
    [Serializable()]
    public enum TransferLogItemType
    {
        /// <summary>
        /// The default state
        /// </summary>
        None = 0,

        /// <summary>
        /// Outbound file downloaded successfully
        /// </summary>
        OutboundFileDownloaded,             // = 1

        /// <summary>
        /// Outbound file has downloaded but there is no data
        /// </summary>
        OutboundFileDownloadedWithoutData,  // = 2

        /// <summary>
        /// Outbound file import started
        /// </summary>
        OutboundFileImportStarted,          // = 3

        /// <summary>
        /// Outbound file import was successful
        /// </summary>
        OutboundFileImportedSuccessfully,   // = 4

        /// <summary>
        /// Outbound file import was unsuccessful
        /// </summary>
        OutboundFileImportFailed,           // = 5

        /// <summary>
        /// Inbound file processed to the ESRFileTransfer database successfully
        /// </summary>
        InboundFileProcessedSuccessfully,   // = 6

        /// <summary>
        /// Inbound file processed to the ESRFileTransfer database unsuccessfully
        /// </summary>
        InboundFileProcessingFailed,        // = 7

        /// <summary>
        /// Inbound file successfully uploaded to the NHS hub
        /// </summary>
        InboundFileUploadedSuccessfully,    // = 8

        /// <summary>
        /// Inbound file failed to upload to the NHS hub
        /// </summary>
        InboundFileUploadFailed,            // = 9

        /// <summary>
        /// The outbound file format does not meet the validation requirements
        /// </summary>
        OutboundFileValidationFailed,       // = 10

        /// <summary>
        /// Log notification of the SEL File Service on devserv stopping
        /// </summary>
        SELFileServiceStopped,              // = 11

        /// <summary>
        /// Log notification of the SEL File Service on devserv erroring
        /// </summary>
        SELFileServiceErrored,              // = 12

        /// <summary>
        /// Log notification of the ESR Service on the N3 machine stopping
        /// </summary>
        ESRServiceStopped,                  // = 13

        /// <summary>
        /// Log notification of the ESR Service on the N3 machine erroring
        /// </summary>
        ESRServiceErrored,                  // = 14

        /// <summary>
        /// This is a notification to inform that the outbound file could not be downloaded from the N3 FTP site
        /// </summary>
        OutboundFileDownloadFailed          // = 15
    }
}
