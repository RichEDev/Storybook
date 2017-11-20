namespace SpendManagementLibrary.ESRTransferServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Web.Caching;

    public class cNHSTrusts
    {
        public static SortedList<int, cESRTrust> lstTrusts;
        private string sConnectionString;
        private Cache cache = System.Web.HttpRuntime.Cache;

        public cNHSTrusts(string ConnectionString)
        {
            sConnectionString = ConnectionString;
            InitialiseData();
        }

        #region Properties

        public string ConnectionString
        {
            get { return sConnectionString; }
        }

        #endregion

        private void InitialiseData()
        {
            lstTrusts = (SortedList<int, cESRTrust>)this.cache["esrTrusts"] ?? this.CacheList();
        }

        private SortedList<int, cESRTrust> CacheList()
        {
            DBConnection smData = new DBConnection(ConnectionString);
            SortedList<int, cESRTrust> list = new SortedList<int, cESRTrust>();

            string strSQL = "SELECT trustID, expTrustID, AccountID, TrustName, TrustVPD, FTPAddress, FTPUsername, FTPPassword, archived, CreatedOn, ModifiedOn FROM dbo.NHSTrustDetails";

            smData.sqlexecute.Notification = null;
            smData.sqlexecute.CommandText = strSQL;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(smData.sqlexecute);
                this.cache.Insert("esrTrusts", list, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);
            }

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    int trustID = reader.GetInt32(reader.GetOrdinal("trustID"));
                    int expTrustID = reader.GetInt32(reader.GetOrdinal("expTrustID"));
                    int AccountID = reader.GetInt32(reader.GetOrdinal("AccountID"));

                    string trustName;
                    if (reader.IsDBNull(reader.GetOrdinal("TrustName")) == true)
                    {
                        trustName = string.Empty;
                    }
                    else
                    {
                        trustName = reader.GetString(reader.GetOrdinal("TrustName"));
                    }

                    string trustVPD;
                    if (reader.IsDBNull(reader.GetOrdinal("TrustVPD")) == true)
                    {
                        trustVPD = string.Empty;
                    }
                    else
                    {
                        trustVPD = reader.GetString(reader.GetOrdinal("TrustVPD"));
                    }

                    string ftpAddress;
                    if (reader.IsDBNull(reader.GetOrdinal("FTPAddress")) == true)
                    {
                        ftpAddress = string.Empty;
                    }
                    else
                    {
                        ftpAddress = reader.GetString(reader.GetOrdinal("FTPAddress"));
                    }

                    string ftpUsername;
                    if (reader.IsDBNull(reader.GetOrdinal("FTPUsername")) == true)
                    {
                        ftpUsername = string.Empty;
                    }
                    else
                    {
                        ftpUsername = reader.GetString(reader.GetOrdinal("FTPUsername"));
                    }

                    string ftpPassword;
                    if (reader.IsDBNull(reader.GetOrdinal("FTPPassword")) == true)
                    {
                        ftpPassword = string.Empty;
                    }
                    else
                    {
                        ftpPassword = reader.GetString(reader.GetOrdinal("FTPPassword"));
                    }

                    bool archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                    DateTime? createdOn = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("CreatedOn")))
                    {
                        createdOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                    }

                    DateTime? modifiedOn = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("ModifiedOn")))
                    {
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
                    }

                    cESRTrust tmpTrust = new cESRTrust(trustID, expTrustID, AccountID, trustName, trustVPD, ftpAddress, ftpUsername, ftpPassword, archived, createdOn, modifiedOn);
                    list.Add(trustID, tmpTrust);
                }

                reader.Close();
            }

            return list;
        }

        public void SaveTrust(cESRTrust trust, int AccountID)
        {
            if (trust != null)
            {
                DBConnection smData = new DBConnection(ConnectionString);
                smData.sqlexecute.Parameters.AddWithValue("@trustID", trust.TrustID);
                smData.sqlexecute.Parameters.AddWithValue("@expTrustID", trust.expTrustID);
                smData.sqlexecute.Parameters.AddWithValue("@AccountID", AccountID);
                smData.sqlexecute.Parameters.AddWithValue("@trustName", trust.TrustName);
                smData.sqlexecute.Parameters.AddWithValue("@trustVPD", trust.TrustVPD);

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
                    smData.sqlexecute.Parameters.AddWithValue("@ftpPassword", trust.FTPPassword);
                }

                smData.sqlexecute.Parameters.AddWithValue("@archived", trust.Archived);
                
                smData.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
                smData.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;

                smData.ExecuteProc("SaveTrust");
                int id = (int)smData.sqlexecute.Parameters["@identity"].Value;

                smData.sqlexecute.Parameters.Clear();

                if (lstTrusts.ContainsKey(id))
                {
                    lstTrusts[id] = new cESRTrust(id, trust.expTrustID, trust.AccountID, trust.TrustName, trust.TrustVPD, trust.FTPAddress, trust.FTPUsername, trust.FTPPassword, trust.Archived, trust.CreatedOn, trust.ModifiedOn);
                }
                else
                {
                    lstTrusts.Add(id, new cESRTrust(id, trust.expTrustID, trust.AccountID, trust.TrustName, trust.TrustVPD, trust.FTPAddress, trust.FTPUsername, trust.FTPPassword, trust.Archived, trust.CreatedOn, trust.ModifiedOn));
                }
                //ResetCache();
            }

        
        }

        /// <summary>
        /// Get a cESRTrust object for a specific trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        public cESRTrust GetESRTrustByID(int trustID)
        {
            cESRTrust reqTrust = null;
            lstTrusts.TryGetValue(trustID, out reqTrust);
            return reqTrust;
        }

        public int GetTrustIDByExpTrustIDAndAccountID(int expTrustID, int AccountID)
        {
            foreach (cESRTrust trust in lstTrusts.Values)
            {
                if (trust.expTrustID == expTrustID && trust.AccountID == AccountID)
                {
                    return trust.TrustID;
                }
            }
            return 0;
        }

        public SortedList<int, cESRTrust> ListTrusts()
        {
            SortedList<int, cESRTrust> lstUnarchivedTrusts = new SortedList<int, cESRTrust>();

            foreach (cESRTrust trust in lstTrusts.Values)
            {
                if (!trust.Archived)
                {
                    lstUnarchivedTrusts.Add(trust.TrustID, trust);
                }
            }
            return lstUnarchivedTrusts;
        }

    }
}
