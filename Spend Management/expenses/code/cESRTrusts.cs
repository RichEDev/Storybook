using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Web.Caching;

namespace Spend_Management
{
    using System.Data;

    public class cESRTrusts
    {
        private int nAccountID;
        private SortedList<int, cESRTrust> lstCachedTrusts;
        private System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

        /// <summary>
        /// SortedList of Trusts indexed by trustid
        /// </summary>
        public SortedList<int, cESRTrust> Trusts
        {
            get { return lstCachedTrusts; }
        }

        public cESRTrusts(int accountID)
        {
            nAccountID = accountID;
            InitializeData();
        }

        /// <summary>
        /// Initialize the caching in this object
        /// </summary>
        private void InitializeData() 
        {
            lstCachedTrusts = (SortedList<int, cESRTrust>)Cache["ESRTrustRecords" + nAccountID];

            if (lstCachedTrusts == null)
            {
                lstCachedTrusts = CacheList();
            }
        }

        /// <summary>
        /// Force an update of the cache in this object
        /// </summary>
        public void ResetCache()
        {
            Cache.Remove("ESRTrustRecords" + nAccountID);
            lstCachedTrusts = null;
            InitializeData();
        }

        private bool TrustNameInUse(cESRTrust trust)
        {
            foreach (cESRTrust tmpTrust in lstCachedTrusts.Values)
            {
                if ((trust.TrustName == tmpTrust.TrustName && trust.TrustID == 0) || (trust.TrustName == tmpTrust.TrustName && trust.TrustID > 0 && trust.TrustID != tmpTrust.TrustID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary> 
        /// Adds or updates the trust record in the database, passing 0 as the trustID will create a new record. Passing an existing trust will update it.
        /// </summary>
        /// <param name="trust"></param>
        /// <returns></returns>
        public List<int> SaveTrust(cESRTrust trust)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            List<int> retValues = new List<int>();

            if (TrustNameInUse(trust) == false)
            {
                if (trust != null)
                {
                    DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
                    smData.sqlexecute.Parameters.AddWithValue("@trustID", trust.TrustID);
                    smData.sqlexecute.Parameters.AddWithValue("@trustName", trust.TrustName);
                    smData.sqlexecute.Parameters.AddWithValue("@trustVPD", trust.TrustVPD);
                    smData.sqlexecute.Parameters.AddWithValue("@periodType", trust.PeriodType);
                    smData.sqlexecute.Parameters.AddWithValue("@periodRun", trust.PeriodRun);

                    if (trust.FTPAddress == "")
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@ftpAddress", DBNull.Value);
                    }
                    else
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@ftpAddress", trust.FTPAddress);
                    }

                    if (trust.FTPUsername == "")
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@ftpUsername", DBNull.Value);
                    }
                    else
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@ftpUsername", trust.FTPUsername);
                    }

                    if (trust.FTPPassword == "")
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@ftpPassword", DBNull.Value);
                    }
                    else
                    {
                        var clsSecure = new cSecureData();
                        var currentTrust = this.GetESRTrustByID(trust.TrustID);
                        string encryptedPassword = trust.FTPPassword;

                        if (currentTrust != null)
                        {
                            if (trust.FTPPassword != currentTrust.FTPPassword)
                            {
                                encryptedPassword = clsSecure.Encrypt(trust.FTPPassword);        
                            }
                        }
                        else
                        {
                            encryptedPassword = clsSecure.Encrypt(trust.FTPPassword);        
                        }
                        
                        smData.sqlexecute.Parameters.AddWithValue("@ftpPassword", encryptedPassword);
                    }

                    smData.sqlexecute.Parameters.AddWithValue("@runSequenceNumber", trust.RunSequenceNumber);

                    smData.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

                    if (currentUser.isDelegate == true)
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        smData.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }

                    smData.AddWithValue("@delimiterCharacter", trust.DelimiterCharacter, 5);

                    smData.sqlexecute.Parameters.AddWithValue("@esrTrustVersionNumber", trust.EsrInterfaceVersionNumber);
                    smData.sqlexecute.Parameters.Add("@versionNumberChanged", SqlDbType.TinyInt);
                    smData.sqlexecute.Parameters["@versionNumberChanged"].Direction = ParameterDirection.Output;

                    smData.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    smData.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                    smData.ExecuteProc("SaveESRTrust");

                    int savedTrustID = (int)smData.sqlexecute.Parameters["@identity"].Value;
                    retValues.Add(savedTrustID);
                    retValues.Add(Convert.ToInt32((byte)smData.sqlexecute.Parameters["@versionNumberChanged"].Value));

                    smData.sqlexecute.Parameters.Clear();
                    ResetCache();
                }
            }
            else
            {
                retValues.Add(-1);
            }

            return retValues;
        }

        /// <summary>
        /// Deletes a trust from the database
        /// </summary>
        /// <param name="trustID"></param>
        public ESRTrustReturnVal DeleteTrust(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));

            smData.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                smData.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                smData.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            smData.sqlexecute.Parameters.AddWithValue("@trustID", trustID);

            smData.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            smData.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            
            smData.ExecuteProc("deleteESRTrust");
            ESRTrustReturnVal returnValue = (ESRTrustReturnVal)smData.sqlexecute.Parameters["@returnvalue"].Value;
            ResetCache();
            smData.sqlexecute.Parameters.Clear();

            return returnValue;
        }

        /// <summary>
        /// Cache all trusts within a customer database
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cESRTrust> CacheList()
        {
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
            SortedList<int, cESRTrust> lstTrusts = new SortedList<int, cESRTrust>();

            const string strSQL = "SELECT trustID, trustName, trustVPD, periodType, periodRun, runSequenceNumber, ftpAddress, ftpUsername, ftpPassword, archived, createdOn, modifiedOn, delimiterCharacter, EsrVersionNumber, currentOutboundSequence FROM dbo.esrTrusts";

            smData.sqlexecute.CommandText = strSQL;

            using (System.Data.SqlClient.SqlDataReader reader = smData.GetReader(strSQL))
            {
                smData.sqlexecute.Parameters.Clear();

                #region Reader.Read

                int trustID;
                int runSequenceNumber;
                string trustName;
                string trustVPD;
                string periodType;
                string periodRun;
                string ftpAddress;
                string ftpUsername;
                string ftpPassword;
                bool archived;
                string delimiterCharacter;
                byte esrVersionNumber;
                cESRTrust tmpTrust;

                while (reader.Read())
                {
                    trustID = reader.GetInt32(reader.GetOrdinal("trustID"));
                    runSequenceNumber = reader.GetInt32(reader.GetOrdinal("runSequenceNumber"));

                    if (reader.IsDBNull(reader.GetOrdinal("trustName")) == true)
                    {
                        trustName = string.Empty;
                    }
                    else
                    {
                        trustName = reader.GetString(reader.GetOrdinal("trustName"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("trustVPD")) == true)
                    {
                        trustVPD = string.Empty;
                    }
                    else
                    {
                        trustVPD = reader.GetString(reader.GetOrdinal("trustVPD"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("periodType")) == true)
                    {
                        periodType = string.Empty;
                    }
                    else
                    {
                        periodType = reader.GetString(reader.GetOrdinal("periodType"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("periodRun")) == true)
                    {
                        periodRun = string.Empty;
                    }
                    else
                    {
                        periodRun = reader.GetString(reader.GetOrdinal("periodRun"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("ftpAddress")) == true)
                    {
                        ftpAddress = string.Empty;
                    }
                    else
                    {
                        ftpAddress = reader.GetString(reader.GetOrdinal("ftpAddress"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("ftpUsername")) == true)
                    {
                        ftpUsername = string.Empty;
                    }
                    else
                    {
                        ftpUsername = reader.GetString(reader.GetOrdinal("ftpUsername"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("ftpPassword")) == true)
                    {
                        ftpPassword = string.Empty;
                    }
                    else
                    {
                        ftpPassword = reader.GetString(reader.GetOrdinal("ftpPassword"));
                    }

                    archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                    DateTime? createdOn = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdOn")))
                    {
                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                    }

                    DateTime? modifiedOn = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedOn")))
                    {
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                    }
                    
                    delimiterCharacter = reader.GetString(reader.GetOrdinal("delimiterCharacter"));
                    esrVersionNumber = reader.GetByte(reader.GetOrdinal("ESRVersionNumber"));
                    int? outboundSequence = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("currentOutboundSequence")))
                    {
                        outboundSequence = reader.GetInt32(reader.GetOrdinal("currentOutboundSequence"));
                    }


                    tmpTrust = new cESRTrust(trustID, trustName, trustVPD, periodType, periodRun, runSequenceNumber, ftpAddress, ftpUsername, ftpPassword, archived, createdOn, modifiedOn, delimiterCharacter, esrVersionNumber, outboundSequence);
                    lstTrusts.Add(trustID, tmpTrust);
                }

                reader.Close();

                #endregion Reader.Read
            }

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = smData.CreateSQLCacheDependency(string.Format("SELECT createdOn, modifiedOn FROM dbo.esrTrusts WHERE {0} = {0}", nAccountID), null);
                Cache.Insert("ESRTrustRecords" + nAccountID, lstTrusts, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
            }
            
            return lstTrusts;
        }

        /// <summary>
        /// Archives a trust in the database
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        public bool ArchiveTrust(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));

            smData.sqlexecute.Parameters.AddWithValue("@trustID", trustID);
            smData.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

            if (currentUser.isDelegate == true)
            {
                smData.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                smData.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            smData.sqlexecute.Parameters.Add("@archived", System.Data.SqlDbType.Bit);
            smData.sqlexecute.Parameters["@archived"].Direction = System.Data.ParameterDirection.ReturnValue;
            smData.ExecuteProc("archiveTrust");
            bool archived = Convert.ToBoolean(smData.sqlexecute.Parameters["@archived"].Value);
            ResetCache();
            return archived;
        }

        /// <summary>
        /// Get a cESRTrust object for a specific trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        public cESRTrust GetESRTrustByVPD(string trustVPD)
        {
            cESRTrust reqTrust = null;

            foreach (cESRTrust trust in lstCachedTrusts.Values)
            {
                if (trust.TrustVPD == trustVPD)
                {
                    reqTrust = trust;
                    break;
                }
            }

            return reqTrust;
        }

        /// <summary>
        /// Get a cESRTrust object for a specific trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        public virtual cESRTrust GetESRTrustByID(int trustID)
        {
            cESRTrust reqTrust = null;
            lstCachedTrusts.TryGetValue(trustID, out reqTrust);
            return reqTrust;
        }

        /// <summary>
        /// Increment the current runSequenceNumber for a specific trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <param name="sequenceNumber"></param>
        public void IncrementRunSequenceNumber(int trustID, int sequenceNumber)
        {
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
            const string strsql = "UPDATE dbo.esrTrusts SET runSequenceNumber = @sequenceNumber WHERE trustID=@trustID";
            smData.sqlexecute.Parameters.AddWithValue("@trustID", trustID);
            smData.sqlexecute.Parameters.AddWithValue("@sequenceNumber", (sequenceNumber));
            smData.ExecuteSQL(strsql);
            smData.sqlexecute.Parameters.Clear();
            ResetCache();
        }

        /// <summary>
        /// Gets the ESR Inbound filename for a trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        public string CreateESRInboundFilename(int trustID)
        {
            cESRTrust reqTrust = this.GetESRTrustByID(trustID);
            DateTime taxdate = DateTime.Today;
            int taxyear;
            int taxperiod;

            if (taxdate < new DateTime(DateTime.Today.Year, 04, 06))
            {
                taxperiod = DateTime.Today.Month + 8;
                taxyear = int.Parse(DateTime.Today.ToString("yy"));
            }
            else
            {
                taxperiod = DateTime.Today.Month - 3;
                taxyear = int.Parse(DateTime.Today.AddYears(1).Year.ToString().Substring(2, 2));
            }

            int nRunSeqNum = reqTrust.RunSequenceNumber;
            
            //Increment the file run sequence number for the trust
            nRunSeqNum++;
            IncrementRunSequenceNumber(trustID, nRunSeqNum);
            
            string runseqnum = nRunSeqNum.ToString();
            int loopindex = 8 - runseqnum.Length;
            string seqnum = "";

            for (int i = 0; i < loopindex; i++)
            {
                seqnum += "0";
            }
            seqnum += runseqnum;

            string filename = "TA_" + reqTrust.TrustVPD + "_SEL_" + reqTrust.PeriodType + reqTrust.PeriodRun + taxyear.ToString("00") + taxperiod.ToString("00") + "_" + seqnum + ".DAT";

            
            return filename;
        }

        /// <summary>
        /// Returns a list of trusts within the datbase
        /// </summary>
        /// <param name="ddl"></param>
        public void CreateDropDownList(ref System.Web.UI.WebControls.DropDownList ddl)
        {
            SortedList<string, cESRTrust> lstSorted = new SortedList<string, cESRTrust>();

            foreach (cESRTrust trust in lstCachedTrusts.Values)
            {
                lstSorted.Add(trust.TrustName, trust);                
            }

            foreach (cESRTrust trust in lstSorted.Values)
            {
                ddl.Items.Add(new System.Web.UI.WebControls.ListItem(trust.TrustName, trust.TrustID.ToString()));
            }            
        }
    }
}
