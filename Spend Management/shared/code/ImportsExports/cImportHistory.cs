using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    #region cImportHistory
    /// <summary>
    /// cImportHistory class
    /// </summary>
    public class cImportHistory
    {
        /// <summary>
        /// Collection of esr import history
        /// </summary>
        private Dictionary<int, cImportHistoryItem> historyitems;
        /// <summary>
        /// Current customer account id
        /// </summary>
        private int nAccountId;
        /// <summary>
        /// Current user id
        /// </summary>
        private int nEmployeeId;
        /// <summary>
        /// Reference to the server cache
        /// </summary>
        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        #region properties
        /// <summary>
        /// Gets the current customer account id
        /// </summary>
        public int AccountID
        {
            get { return nAccountId; }
        }
        /// <summary>
        /// Retrieve SQL for generation by cGridNew class
        /// </summary>
        public string getHistoryGridSQL
        {
            get { return "select historyId, importId, logId, importedDate, importStatus, applicationType, dataId from importHistory"; }
        }
        /// <summary>
        /// Retrieve SQL for caching of database items
        /// </summary>
        private string getHistoryCacheSQL
        {
            get { return "select historyId, importId, logId, importedDate, importStatus, applicationType, dataId, createdOn, modifiedOn from dbo.importHistory"; }
        }
        #endregion

        /// <summary>
        /// cImportHistory constructor
        /// </summary>
        /// <param name="accountid">The account id to obtain import history for</param>
        public cImportHistory(int accountid)
        {
            nAccountId = accountid;

            InitialiseData();

            return;
        }

        /// <summary>
        /// Initialise history item collection
        /// </summary>
        private void InitialiseData()
        {
            historyitems = (Dictionary<int, cImportHistoryItem>)Cache["ihistory" + AccountID.ToString()];
            if (historyitems == null)
            {
                historyitems = CacheItems();
            }
            return;
        }

        /// <summary>
        /// Load the data into cache memory
        /// </summary>
        private Dictionary<int, cImportHistoryItem> CacheItems()
        {
            Dictionary<int, cImportHistoryItem> hItems = new Dictionary<int, cImportHistoryItem>();

            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            string sql = getHistoryCacheSQL;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = db.CreateSQLCacheDependency(string.Format("{0} where {1} = {1}", getHistoryGridSQL, AccountID), null);
                Cache.Insert("ihistory" + AccountID.ToString(), hItems, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromHours((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);
            }

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int historyId = reader.GetInt32(reader.GetOrdinal("historyId"));
                    int importId = reader.GetInt32(reader.GetOrdinal("importId"));
                    int logId = reader.GetInt32(reader.GetOrdinal("logId"));
                    DateTime importdate = reader.GetDateTime(reader.GetOrdinal("importedDate"));
                    ImportHistoryStatus importStatus = (ImportHistoryStatus)reader.GetInt32(reader.GetOrdinal("importStatus"));
                    ApplicationType apptype = (ApplicationType)reader.GetInt32(reader.GetOrdinal("applicationType"));
                    int dataId = 0;
                    if (!reader.IsDBNull(reader.GetOrdinal("dataId")))
                    {
                        dataId = reader.GetInt32(reader.GetOrdinal("dataId"));
                    }
                    DateTime createdon = reader.GetDateTime(reader.GetOrdinal("createdOn"));

                    DateTime? modifiedon = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedOn")))
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                    }

                    cImportHistoryItem item = new cImportHistoryItem(historyId, importId, logId, importdate, importStatus, apptype, dataId, createdon, modifiedon);
                    hItems.Add(historyId, item);
                }
                reader.Close();
            }

            return hItems;
        }

        /// <summary>
        /// Force an update of the cache in this object
        /// </summary>
        public void ResetCache()
        {
            Cache.Remove("ihistory" + AccountID.ToString());
            historyitems = null;
            InitialiseData();
        }

        /// <summary>
        /// Gets an individual import history record by its ID
        /// </summary>
        /// <param name="historyId">ID of the history record to retrieve</param>
        /// <returns>cImportHistoryItem class entity</returns>
        public cImportHistoryItem getHistoryById(int historyId)
        {
            cImportHistoryItem retItem = null;
            if (historyitems.ContainsKey(historyId))
            {
                retItem = historyitems[historyId];
            }
            return retItem;
        }

        /// <summary>
        /// Delete an import history entry. Routine will also result in deletion of associated import log.
        /// </summary>
        /// <param name="historyId">ID of the import history item to delete</param>
        public void deleteHistory(int historyId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            db.sqlexecute.Parameters.AddWithValue("@historyId", historyId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("dbo.deleteImportHistory");
            ResetCache();
            return;
        }

        /// <summary>
        /// Saves the defined history record to the database
        /// </summary>
        /// <param name="historyItem">cImportHistoryItem class entity</param>
        /// <returns></returns>
        public int saveHistory(cImportHistoryItem historyItem)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@historyId", historyItem.HistoryId);
            db.sqlexecute.Parameters.AddWithValue("@importId", historyItem.ImportID);
            db.sqlexecute.Parameters.AddWithValue("@logId", historyItem.LogId);

            if (historyItem.ImportedDate == new DateTime(0001, 01, 01))
            {
                db.sqlexecute.Parameters.AddWithValue("@importDate", DateTime.Now);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@importDate", historyItem.ImportedDate);
            }

            db.sqlexecute.Parameters.AddWithValue("@status", historyItem.ImportStatus);
            db.sqlexecute.Parameters.AddWithValue("@appType", historyItem.applicationType);
            db.sqlexecute.Parameters.AddWithValue("@dataId", historyItem.ImportDataID);
            //db.sqlexecute.Parameters.AddWithValue("@userId", EmployeeId);
            
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if(currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("dbo.saveImportHistory");

            int historyId = (int)db.sqlexecute.Parameters["@Identity"].Value;

            ResetCache();

            return historyId;
        }

        /// <summary>
        /// Method to rerun the import file
        /// </summary>
        /// <param name="data">Byte[] of the file data</param>
        /// <param name="HistoryID">ID of the import history</param>
        public void rerunOutboundImport(int dataID, int HistoryID)
        {
            //IESR clsESR = (IESR)Activator.GetObject(typeof(IESR), ConfigurationManager.AppSettings["ESRService"] + "/ESR.rem");
            byte[] data = new byte[0] ;

            cImportHistoryItem historyItem = getHistoryById(HistoryID);
            cImportTemplates clsImportTemps = new cImportTemplates(AccountID);

            clsImportTemps.processESROutboundImport(historyItem.ImportID,dataID, data);

        }
    }
    #endregion
}