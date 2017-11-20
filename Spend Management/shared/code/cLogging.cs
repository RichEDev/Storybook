namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI.WebControls;
    using Microsoft.SqlServer.Server;
    using SpendManagementLibrary;

    public class cLogging
    {
        private int nAccountID;
        
        private SortedList<LogReasonType, cLogErrorReason> logErrorReasons;
        private Cache cache = HttpRuntime.Cache;
        
        private List<cLogItem> lstItems = new List<cLogItem>();

        /// <summary>
        /// The connection string.
        /// </summary>
        private string ConnectionString;

        public cLogging(int accountid)
        {
            nAccountID = accountid;

            var cacheKey = "logErrorReasonslist" + this.accountid;

            logErrorReasons = (SortedList<LogReasonType, cLogErrorReason>)cache[cacheKey] ?? getLogErrorReasonsFromDB();
        }

        #region properties
        public int accountid
        {
            get { return nAccountID; }
        }
        #endregion

        /// <summary>
        /// Get the log and its data items from the database based on the log reason type
        /// </summary>
        /// <param name="logID">Unique ID of the log</param>
        /// <param name="logReasonType">Type of log Reason</param>
        /// <returns>A log Object</returns>
        public cLog getLogFromDatabase(int logID, LogReasonType logReasonType)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            LogType loggingType;
            int nSuccessfulUpdates;
            int nFailedUpdates;
            int nWarningUpdates;
            int nExpectedLines;
            int nProcessedLines;
            string logName;
            DateTime createdOn, modifiedOn;
            cLog log = null;
            List<cLogData> lstLogData;

            const string strsql = "SELECT logId, logType, logName, successfulLines, failedLines, warningLines, expectedLines, processedLines, createdOn, modifiedOn  FROM logNames WHERE logid = @logid";
            expdata.sqlexecute.Parameters.AddWithValue("@logid", logID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    loggingType = (LogType)reader.GetByte(reader.GetOrdinal("logType"));
                    logName = reader.GetString(reader.GetOrdinal("logName"));

                    if (reader.IsDBNull(reader.GetOrdinal("successfulLines")) == true)
                    {
                        nSuccessfulUpdates = 0;
                    }
                    else
                    {
                        nSuccessfulUpdates = reader.GetInt32(reader.GetOrdinal("successfulLines"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("failedLines")) == true)
                    {
                        nFailedUpdates = 0;
                    }
                    else
                    {
                        nFailedUpdates = reader.GetInt32(reader.GetOrdinal("failedLines"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("warningLines")) == true)
                    {
                        nWarningUpdates = 0;
                    }
                    else
                    {
                        nWarningUpdates = reader.GetInt32(reader.GetOrdinal("warningLines"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("expectedLines")) == true)
                    {
                        nExpectedLines = 0;
                    }
                    else
                    {
                        nExpectedLines = reader.GetInt32(reader.GetOrdinal("expectedLines"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("processedLines")) == true)
                    {
                        nProcessedLines = 0;
                    }
                    else
                    {
                        nProcessedLines = reader.GetInt32(reader.GetOrdinal("processedLines"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("createdOn")) == true)
                    {
                        createdOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")) == true)
                    {
                        modifiedOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                    }

                    if (loggingType == LogType.EsrOutBoundImportV2)
                    {
                        lstLogData = logReasonType == LogReasonType.None ? this.GetV2LogData(logID) : this.GetV2LogData(logID, logReasonType);    
                    }
                    else
                    {
                        lstLogData = logReasonType == LogReasonType.None ? this.getLogData(logID) : this.getLogData(logID, logReasonType);    
                    }
                    
                    log = new cLog(logID, logName, loggingType, nSuccessfulUpdates, nFailedUpdates, nWarningUpdates, nExpectedLines, nProcessedLines, lstLogData, createdOn, modifiedOn);
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return log;
        }

        /// <summary>
        /// The get v 2 log data.
        /// </summary>
        /// <param name="logID">
        /// The log id.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<cLogData> GetV2LogData(int logID, LogReasonType reason = LogReasonType.None)
        {
            var secure = new cSecureData();
            this.ConnectionString = ConfigurationManager.ConnectionStrings["ApiLog"].ConnectionString;
            if (this.ConnectionString.Contains(ConfigurationManager.AppSettings["dbpassword"]))
            {
                this.ConnectionString = this.ConnectionString.Replace(ConfigurationManager.AppSettings["dbpassword"], secure.Decrypt(ConfigurationManager.AppSettings["dbpassword"]));
            }

            var expdata = new DBConnection(this.ConnectionString);
            
            var result = new List<cLogData>();
            string strsql = "SELECT LogItemType, Source, Message, CreatedOn, logItemreason FROM ApiLog WHERE LogId = @logid AND accountid = @accountid";    
            if (reason != LogReasonType.None)
            {
                strsql = strsql + " AND LogItemreason = @logItemreason";
                expdata.sqlexecute.Parameters.AddWithValue("@logItemreason", reason);
            }
            
            expdata.sqlexecute.Parameters.AddWithValue("@logid", logID);
            expdata.sqlexecute.Parameters.AddWithValue("@accountid", this.nAccountID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                var logItemReasonOrd = reader.GetOrdinal("LogItemreason");
                var sourceOrd = reader.GetOrdinal("source");
                var messageOrd = reader.GetOrdinal("message");
                var createdOnOrd = reader.GetOrdinal("CreatedOn");
                while (reader.Read())
                {
                    var createdon = reader.GetDateTime(createdOnOrd);
                    var message = string.Format("{0} : {1}",reader.GetString(messageOrd), createdon);
                    var logDataItem = message;
                    int logReason = 0;
                    if (!reader.IsDBNull(logItemReasonOrd))
                    {
                        var reasonid = reader.GetValue(logItemReasonOrd);
                        int.TryParse(reasonid.ToString(), out logReason);
                    }
                    
                    result.Add(new cLogData(logID, logReason, 0, logDataItem, createdon));
                }
            }
            return result;
        }

        /// <summary>
        /// Get the log data for a log 
        /// </summary>
        /// <param name="logID">ID of the log</param>
        /// <returns>List of log data items</returns>
        public List<cLogData> getLogData(int logID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<cLogData> lstLogData = new List<cLogData>();
            int logReasonID;
            int logElementID;
            string logDataItem;
            DateTime createdon;

            const string strsql = "SELECT * FROM logDataItems WHERE logid = @logid";
            expdata.sqlexecute.Parameters.AddWithValue("@logid", logID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(reader.GetOrdinal("logReasonID")) == true)
                    {
                        logReasonID = 0;
                    }
                    else
                    {
                        logReasonID = reader.GetInt32(reader.GetOrdinal("logReasonID"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("logElementID")) == true)
                    {
                        logElementID = 0;
                    }
                    else
                    {
                        logElementID = reader.GetInt32(reader.GetOrdinal("logElementID"));
                    }

                    logDataItem = reader.GetString(reader.GetOrdinal("logDataItem"));

                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    lstLogData.Add(new cLogData(logID, logReasonID, logElementID, logDataItem, createdon));
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lstLogData;
        }

        /// <summary>
        /// Get the log data for a log filtering by the log error reason passed in 
        /// </summary>
        /// <param name="logID">ID of the log</param>
        /// <param name="reasonType">Log reason type</param>
        /// <returns>List of filtered log data items</returns>
        public List<cLogData> getLogData(int logID, LogReasonType reasonType)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<cLogData> lstLogData = new List<cLogData>();
            int logReasonID = 0;
            int logElementID = 0;
            string strsql;
            string logDataItem;
            DateTime createdon;

            if (reasonType != LogReasonType.None)
            {
                logReasonID = logErrorReasons[reasonType].logReasonID;
                strsql = "SELECT * FROM logDataItems WHERE logid = @logid AND logReasonID = @logReasonID";
                expdata.sqlexecute.Parameters.AddWithValue("@logReasonID", logReasonID);
            }
            else
            {
                strsql = "SELECT * FROM logDataItems WHERE logid = @logid";
            }

            expdata.sqlexecute.Parameters.AddWithValue("@logid", logID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reasonType == LogReasonType.None)
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("logReasonID")) == false)
                        {
                            logReasonID = reader.GetInt32(reader.GetOrdinal("logReasonID"));
                        }
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("logElementID")) == false)
                    {
                        logElementID = reader.GetInt32(reader.GetOrdinal("logElementID"));
                    }
                    else
                    {
                        logElementID = 0;
                    }

                    logDataItem = reader.GetString(reader.GetOrdinal("logDataItem"));

                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    lstLogData.Add(new cLogData(logID, logReasonID, logElementID, logDataItem, createdon));
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lstLogData;
        }

        /// <summary>
        /// Get and cache all log error reasons from the database 
        /// </summary>
        /// <returns>A Sorted list of error reason types</returns>
        private SortedList<LogReasonType, cLogErrorReason> getLogErrorReasonsFromDB()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int logReasonID;
            LogReasonType reasonType;
            string reason;
            DateTime createdon;
            DateTime modifiedon;

            SortedList<LogReasonType, cLogErrorReason> lstReasons = new SortedList<LogReasonType, cLogErrorReason>();

            const string strsql = "select logReasonID, reasonType, reason, createdon, modifiedon FROM dbo.logErrorReasons";
            expdata.sqlexecute.CommandText = strsql;            

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    logReasonID = reader.GetInt32(reader.GetOrdinal("logReasonID"));
                    reasonType = (LogReasonType)reader.GetByte(reader.GetOrdinal("reasonType"));
                    reason = reader.GetString(reader.GetOrdinal("reason"));

                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                    {
                        modifiedon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }

                    lstReasons.Add(reasonType, new cLogErrorReason(logReasonID, reasonType, reason, createdon, modifiedon));
                }

                reader.Close();
            }

            this.cache.Insert("logErrorReasonslist" + this.accountid, lstReasons, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);

            return lstReasons;
        }

        /// <summary>
        /// Save the log to the database
        /// </summary>
        /// <param name="logID">ID of the log</param>
        /// <param name="logType"></para></param>
        /// <param name="numOfExpectedFileLines">Number of expected line processed</param>
        /// <param name="numOfProcessedFileLines">Number of the actual lines processed</param>
        /// <returns>The ID of the log saved</returns>
        public int saveLog(int logID, LogType logType, int numOfExpectedFileLines, int numOfProcessedFileLines, int numSuccessfulUpdates, int numFailedUpdates, int numWarningUpdates)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            string logName = createLogFileName(logType);

            expdata.sqlexecute.Parameters.AddWithValue("@logID", logID);
            expdata.sqlexecute.Parameters.AddWithValue("@logType", logType);
            expdata.sqlexecute.Parameters.AddWithValue("@logName", logName);

            if (numSuccessfulUpdates == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@successfulLines", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@successfulLines", numSuccessfulUpdates);
            }

            if (numFailedUpdates == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@failedLines", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@failedLines", numFailedUpdates);
            }

            if (numWarningUpdates == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@warningLines", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@warningLines", numWarningUpdates);
            }

            if (numOfExpectedFileLines == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expectedLines", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expectedLines", numOfExpectedFileLines);
            }

            if (numOfProcessedFileLines == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@processedLines", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@processedLines", numOfProcessedFileLines);
            }

            if (logID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.UtcNow);
                expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.UtcNow);
			}

            expdata.ExecuteProc("dbo.saveLog");

            if (logID == 0)
            {
                logID = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            }

            expdata.sqlexecute.Parameters.Clear();

            return logID;
        }

        /// <summary>
        /// Delete the log from the database
        /// </summary>
        /// <param name="logID">ID of the log</param>
        public void deleteLog(int logID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@logID", logID);
            expdata.ExecuteProc("dbo.deleteLog");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Adds a log item to the collection
        /// </summary>
        /// <param name="reasonType"></param>
        /// <param name="elementType"></param>
        /// <param name="logItem"></param>
        public void addLogItem(LogReasonType reasonType, cElement elementType, string logItem)
        {
            lstItems.Add(new cLogItem(reasonType, elementType, logItem));
        }

        /// <summary>
        /// Saves all log items in the collection to the database
        /// </summary>
        /// <param name="logid">Id of the log entry to associate entries to</param>
        public void saveAllLogItems(int logid)
        {
            int numberSuccessfulUpdates = 0;
            int numberFailedUpdates = 0;
            int numberWarningUpdates = 0;
            List<SqlDataRecord> lstItemData = new List<SqlDataRecord>();

            // Generate a sql dbo.logitem table param and pass into the stored proc
            SqlMetaData[] tvpLogItems = { new SqlMetaData("logReasonID", System.Data.SqlDbType.Int), new SqlMetaData("logElementID", System.Data.SqlDbType.Int), new SqlMetaData("logDataItem", System.Data.SqlDbType.NVarChar,4000) };
            string logItemData;
            SqlDataRecord row;

            foreach (cLogItem item in lstItems)
            {
                row = new SqlDataRecord(tvpLogItems);
                row.SetInt32(0, (int)item.ReasonType);
                if (item.ElementType == null)
                {
                    row.SetValue(1, DBNull.Value);
                }
                else
                {
                    row.SetInt32(1, (int)item.ElementType.ElementID);
                }
                switch (item.ReasonType)
                {
                    case LogReasonType.MandatoryField:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                       
                        break;
                    case LogReasonType.MaxLengthExceeded:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                        
                        break;
                    case LogReasonType.None:
                        logItemData = item.LogItem;
                        
                        break;
                    case LogReasonType.Success:
                        logItemData = "<span style=\"color: green; \">" + logErrorReasons[
                            item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberSuccessfulUpdates++;
                        
                        break;
                    case LogReasonType.SuccessAdd:
                        logItemData = "<span style=\"color: green; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberSuccessfulUpdates++;
                        
                        break;
                    case LogReasonType.SuccessUpdate:
                        logItemData = "<span style=\"color: green; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberSuccessfulUpdates++;
                        
                        break;
                    case LogReasonType.SuccessDelete:
                        logItemData = "<span style=\"color: green; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberSuccessfulUpdates++;
                        
                        break;
                    case LogReasonType.UniqueField:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                        
                        break;
                    case LogReasonType.Warning:
                        logItemData = "<span style=\"color: maroon; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberWarningUpdates++;
                        
                        break;
                    case LogReasonType.WrongDataType:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                        
                        break;
                    case LogReasonType.Error:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                        
                        break;
                    case LogReasonType.SQLError:
                        logItemData = "<span style=\"color: red; \">" + logErrorReasons[item.ReasonType].reason + cLoggingValues.space + item.LogItem + "</span>";
                        numberFailedUpdates++;
                        
                        break;
                    default:
                        return;
                }
                row.SetSqlString(2, logItemData);
                lstItemData.Add(row);
            }

            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.Add("@logitems", System.Data.SqlDbType.Structured);
            data.sqlexecute.Parameters["@logitems"].Value =  lstItemData;
            data.sqlexecute.Parameters.AddWithValue("@logid", logid);
            data.ExecuteProc("saveESRExportLog");
            data.sqlexecute.Parameters.Clear();
            lstItems.Clear();

            this.updateLogCounts(logid, numberSuccessfulUpdates, numberFailedUpdates, numberWarningUpdates);
        }

        /// <summary>
        /// Save the log item to the database
        /// </summary>
        /// <param name="logID">ID of the log</param>
        /// <param name="reasonType">The reason type of the log item</param>
        /// <param name="elementType">Element associated to the log items</param>
        /// <param name="logItem">Log item data></param>
        public void saveLogItem(int logID, LogReasonType reasonType, cElement elementType, string logItem)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int logElementID = 0;
            string logItemData;
            int numberSuccessfulUpdates = 0;
            int numberFailedUpdates = 0;
            int numberWarningUpdates = 0;

            if (elementType != null && elementType.ElementID != 0)
            {
                logElementID = elementType.ElementID;
            }

            int logReasonID = (int)reasonType;

            switch (reasonType)
            {
                case LogReasonType.MandatoryField:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                case LogReasonType.MaxLengthExceeded:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                case LogReasonType.None:
                    logItemData = logItem;
                    break;
                case LogReasonType.Success:
                    logItemData = "<span style=\"color: green; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberSuccessfulUpdates++;
                    break;
                case LogReasonType.SuccessAdd:
                    logItemData = "<span style=\"color: green; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberSuccessfulUpdates++;
                    break;
                case LogReasonType.SuccessUpdate:
                    logItemData = "<span style=\"color: green; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberSuccessfulUpdates++;
                    break;
                case LogReasonType.SuccessDelete:
                    logItemData = "<span style=\"color: green; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberSuccessfulUpdates++;
                    break;
                case LogReasonType.UniqueField:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                case LogReasonType.Warning:
                    logItemData = "<span style=\"color: maroon; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberWarningUpdates++;
                    break;
                case LogReasonType.WrongDataType:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                case LogReasonType.Error:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                case LogReasonType.SQLError:
                    logItemData = "<span style=\"color: red; \">" + logErrorReasons[reasonType].reason + cLoggingValues.space + logItem + "</span>";
                    numberFailedUpdates++;
                    break;
                default:
                    logReasonID = 0;
                    return;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@logID", logID);

            if (logReasonID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@logReasonID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@logReasonID", logReasonID);
            }
            if (logElementID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@logElementID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@logElementID", logElementID);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@logDataItem", logItemData);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            
            expdata.ExecuteProc("saveLogItem");

            this.updateLogCounts(logID, numberSuccessfulUpdates, numberFailedUpdates, numberWarningUpdates);

            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Updates the processed lines statistics against the log entry
        /// </summary>
        /// <param name="logId">ID of the log</param>
        /// <param name="successfulLines"></param>
        /// <param name="failedLines"></param>
        /// <param name="warningLines"></param>
        private void updateLogCounts(int logId, int successfulLines, int failedLines, int warningLines)
        {
            int numberSuccessfulUpdates = 0;
            int numberFailedUpdates = 0;
            int numberWarningUpdates = 0;

            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            const string CurrentCountSql = "select ISNULL(successfulLines, 0), ISNULL(failedLines, 0), ISNULL(warningLines, 0) from logNames where logID = @logId";
            data.sqlexecute.Parameters.AddWithValue("@logId", logId);

            using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(CurrentCountSql))
            {
                while (reader.Read())
                {
                    numberSuccessfulUpdates = reader.GetInt32(0);
                    numberFailedUpdates = reader.GetInt32(1);
                    numberWarningUpdates = reader.GetInt32(2);
                }
                reader.Close();
            }

            data.sqlexecute.Parameters.Clear();

            const string UpdateCountSql = "update logNames set successfulLines = @sl, failedLines = @fl, warningLines = @wl where logID = @logId";
            data.sqlexecute.Parameters.AddWithValue("@logId", logId);
            data.sqlexecute.Parameters.AddWithValue("@sl", numberSuccessfulUpdates + successfulLines);
            data.sqlexecute.Parameters.AddWithValue("@fl", numberFailedUpdates + failedLines);
            data.sqlexecute.Parameters.AddWithValue("@wl", numberWarningUpdates + warningLines);
            data.ExecuteSQL(UpdateCountSql);
        }

        /// <summary>
        /// Create the file name for the log vased on the log type
        /// </summary>
        /// <param name="loggingType">Log Type</param>
        /// <returns>The string value of the file name</returns>
        private string createLogFileName(LogType loggingType)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string logName = "";

            string strsql = "SELECT count(logName) FROM logNames WHERE logType = @logType";
            expdata.sqlexecute.Parameters.AddWithValue("@logType", Convert.ToByte(loggingType));

            int count = expdata.getcount(strsql);

            switch (loggingType)
            {
                case LogType.SpreadsheetImport:
                    logName = loggingType.ToString() + count;
                    break;

                case LogType.ESROutboundImport:
                    logName = loggingType.ToString() + count;
                    break;

                default:
                    break;
            }

            return logName;
        }

        /// <summary>
        /// Get the log summary information to output to the log
        /// </summary>
        /// <param name="logID">ID of the log</param>
        /// <param name="eLogViewType">View type from LogViewType enum</param>
        /// <param name="element">null to view all or the cElement to filter on</param>
        /// <returns>The string value for the log summary to output to the log</returns>
        public string generateLogInfo(int logID, LogViewType eLogViewType, cElement element)
        {
            StringBuilder logData = new StringBuilder();

            cLog log = getLogFromDatabase(logID, LogReasonType.None);

            if (eLogViewType == LogViewType.Items || eLogViewType == LogViewType.All)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Data Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (element == null || logItem.logElementID == element.ElementID)
                    {
                        logData.Append(logItem.logDataItem + "<br />");
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Successes)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Successful Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.Success || logItem.logReasonID == (int)LogReasonType.SuccessAdd || logItem.logReasonID == (int)LogReasonType.SuccessUpdate || logItem.logReasonID == (int)LogReasonType.SuccessDelete)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Added)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Successfully Added Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.SuccessAdd)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Updated)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Successfully Updated Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.SuccessUpdate)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Deleted)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Successfully Deleted Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.SuccessDelete)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Failures)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Failed Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.Error || logItem.logReasonID == (int)LogReasonType.MandatoryField || logItem.logReasonID == (int)LogReasonType.MaxLengthExceeded || logItem.logReasonID == (int)LogReasonType.SQLError || logItem.logReasonID == (int)LogReasonType.UniqueField || logItem.logReasonID == (int)LogReasonType.WrongDataType)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Warnings)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Warning Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.Warning)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Other)
            {
                #region Generate log data

                logData.Append(cLoggingValues.filler1 + " Unclassified Items " + cLoggingValues.filler1 + "<br />");

                foreach (cLogData logItem in log.logData)
                {
                    if (logItem.logReasonID == (int)LogReasonType.None)
                    {
                        if (element == null || logItem.logElementID == element.ElementID)
                        {
                            logData.Append(logItem.logDataItem + "<br />");
                        }
                    }
                }

                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            if (eLogViewType == LogViewType.Summary || eLogViewType == LogViewType.All)
            {
                #region Generate Log Summary

                logData.Append("<br />");

                logData.Append(cLoggingValues.filler1 + " Log Summary " + cLoggingValues.filler1 + "<br />");
                logData.Append("Number of expected data file lines = " + log.numOfExpectedFileLines + "<br />");
                logData.Append("Number of processed data file lines = " + log.numOfProcessedFileLines + "<br />");
                logData.Append("<span style=\"color: green; \">No of Successful Updates = " + log.numSuccessfulUpdates + " </span><br />");
                logData.Append("<span style=\"color: red; \">No of Failed Updates = " + log.numFailedUpdates + " </span><br />");
                logData.Append("<span style=\"color: maroon; \">No of Warning Updates = " + log.numWarningUpdates + " </span><br />");
                logData.Append(cLoggingValues.filler1 + cLoggingValues.filler1 + "<br />");

                #endregion
            }
            return logData.ToString();
        }

        /// <summary>
        /// Get a list of listitems created from the elements present on a log
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        public List<ListItem> GenerateLogElementOptions(int logID)
        {
            List<int> lstElementID = new List<int>();
            List<ListItem> optionsList = new List<ListItem>();

            cElements clsElements = new cElements();
            cElement element = null;
            int elementID = 0;

            cLog log = getLogFromDatabase(logID, LogReasonType.None);

            foreach(cLogData logItem in log.logData)
            {
                elementID = logItem.logElementID;
                if (elementID != 0 && lstElementID.Contains(elementID) == false)
                {
                    lstElementID.Add(elementID);
                    element = clsElements.GetElementByID(elementID);
                    optionsList.Add(new ListItem(element.FriendlyName, element.ElementID.ToString()));
                }
            }
            optionsList.Sort(delegate(ListItem firstItem, ListItem secondItem) { return firstItem.Text.CompareTo(secondItem.Text); });

            return optionsList;
        }

        /// <summary>
        /// The get data item file name.
        /// </summary>
        /// <param name="DataId">
        /// The Data Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDataItemFileName(int DataId)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            const string Strsql = "SELECT     logNames.logName FROM importHistory INNER JOIN logNames ON importHistory.logId = logNames.logID where importHistory.dataid = @dataId";
            expdata.sqlexecute.Parameters.AddWithValue("@dataId", DataId);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(Strsql))
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }

                reader.Close();
            }

            return string.Empty;
        }
    }

    public class cLogItem 
    {
        private LogReasonType eReasonType;
        private cElement clsElementType;
        private string sLogItem;

        public cLogItem(LogReasonType reasonType, cElement elementType, string logItem)
        {
            eReasonType = reasonType;
            clsElementType=elementType;
            sLogItem = logItem;
        }

        public LogReasonType ReasonType
        {
            get { return eReasonType; }
        }
        public cElement ElementType
        {
            get { return clsElementType; }
        }
        public string LogItem
        {
            get { return sLogItem; }
        }
    }
}
