using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementLibrary
{
    #region cImportHistoryItem
    /// <summary>
    /// cESRImportHistoryItem class
    /// </summary>
    public class cImportHistoryItem
    {
        /// <summary>
        /// History Id
        /// </summary>
        private int nHistoryId;
        /// <summary>
        /// Unique ID of previous import
        /// </summary>
        private int nImportId;
        /// <summary>
        /// Id of import log
        /// </summary>
        private int nLogId;
        /// <summary>
        /// Date the import was run
        /// </summary>
        private DateTime dtImportedDate;
        /// <summary>
        /// Stores the success status of the import
        /// </summary>
        private ImportHistoryStatus eImportStatus;
        /// <summary>
        /// Application Type
        /// </summary>
        private ApplicationType appType;
        /// <summary>
        /// ID of the byte[] data
        /// </summary>
        private int nDataID;
        /// <summary>
        /// Date history record created
        /// </summary>
        private DateTime dtCreatedOn;
        /// <summary>
        /// History record last modified date
        /// </summary>
        private DateTime? dtModifiedOn;
        
        #region properties
        /// <summary>
        /// Get the history Id of the database record
        /// </summary>
        public int HistoryId
        {
            get { return nHistoryId; }
        }
        /// <summary>
        /// Gets the Import Id
        /// </summary>
        public int ImportID
        {
            get { return nImportId; }
        }
        /// <summary>
        /// Gets the associated LogID for the import
        /// </summary>
        public int LogId
        {
            get { return nLogId;}
        }
        /// <summary>
        /// Gets the date the import took place
        /// </summary>
        public DateTime ImportedDate
        {
            get { return dtImportedDate;}
        }
        /// <summary>
        /// Gets the import status
        /// </summary>
        public ImportHistoryStatus ImportStatus
        {
            get { return eImportStatus; }
        }
        /// <summary>
        /// Gets the enumerated application import type
        /// </summary>
        public ApplicationType applicationType
        {
            get { return appType; }
        }
        /// <summary>
        /// ID of the import data
        /// </summary>
        public int ImportDataID
        {
            get { return nDataID; }
        }
        /// <summary>
        /// Gets date history record created
        /// </summary>
        public DateTime CreatedOn
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// Gets the date the history record was last modified (NULL if unmodified)
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }
        #endregion

        /// <summary>
        /// cImportHistoryItem class constructor
        /// </summary>
        /// <param name="historyid">History ID (0 if adding new record)</param>
        /// <param name="importid">Import ID</param>
        /// <param name="logid">Associated Log ID</param>
        /// <param name="importeddate">Date the import took place</param>
        /// <param name="apptype">Application Type</param>
        /// <param name="dataid">ID of the import data record</param>
        /// <param name="createdon">Date history record created</param>
        /// <param name="modifiedon">History record last modified date</param>
        public cImportHistoryItem(int historyid, int importid, int logid, DateTime importeddate, ImportHistoryStatus importStatus, ApplicationType apptype, int dataid, DateTime createdon, DateTime? modifiedon)
        {
            nHistoryId = historyid;
            nImportId = importid;
            nLogId = logid;
            dtImportedDate = importeddate;
            eImportStatus = importStatus;
            appType = apptype;
            nDataID = dataid;
            dtCreatedOn = createdon;
            dtModifiedOn = modifiedon;
        }
    }
    #endregion
}
