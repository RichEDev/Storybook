using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Text;
using FWBase;
using SpendManagementLibrary;

namespace FWClasses
{
    public class cCustomerAccount
    {
        private int nCustomerId;
        public int CustomerId
        {
            get { return nCustomerId; }
            set { nCustomerId = value; }
        }
        private string sCustomerName;
        public string CustomerName
        {
            get { return sCustomerName; }
        }
        private int nCustomerNo;
        public int CustomerNumber
        {
            get { return nCustomerNo; }
            set { nCustomerNo = value; }
        }
        private string sKey;
        public string URL_Key
        {
            get { return sKey; }
        }
        private bool bArchived;
        public bool isArchived
        {
            get { return bArchived; }
        }

        public cCustomerAccount(int customerid, string name,int customerno, string key, bool archived)
        {
            nCustomerId = customerid;
            sCustomerName = name;
            nCustomerNo = customerno;
            sKey = key;
            bArchived = archived;
        }        
    }

    public class cCustomerAccounts
    {
        SortedList<int, cCustomerAccount> accounts;
        private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        private string connectionstring;

        public cCustomerAccounts(string meta_connectionstring)
        {
            connectionstring = meta_connectionstring;

            CreateDependency();

            if (Cache["Metabase_Customers"] == null)
            {
                accounts = CacheAccounts();
            }
            else
            {
                accounts = (SortedList<int, cCustomerAccount>)Cache["Metabase_Customers"];
            }
        }

        private SortedList<int, cCustomerAccount> CacheAccounts()
        {
            SortedList<int, cCustomerAccount> customers = new SortedList<int, cCustomerAccount>();

            cFWDBConnection db = new cFWDBConnection();
            db.DBOpenMetabase(connectionstring, false);

            string sql = "SELECT * FROM customers WHERE [Archived] = 0";
            System.Data.SqlClient.SqlDataReader reader;

            reader = db.GetReader(sql);
            while (reader.Read())
            {
                int customerid = reader.GetInt32(reader.GetOrdinal("Customer Id"));
                string name = reader.GetString(reader.GetOrdinal("Customer Name"));
                int customernum = 0;
                if (!reader.IsDBNull(reader.GetOrdinal("Customer Number")))
                {
                    customernum = reader.GetInt32(reader.GetOrdinal("Customer Number"));
                }
                string key = "";
                if (!reader.IsDBNull(reader.GetOrdinal("URL Key")))
                {
                    key = reader.GetString(reader.GetOrdinal("URL Key"));
                }
                bool archived = reader.GetBoolean(reader.GetOrdinal("Archived"));
                cCustomerAccount newaccount = new cCustomerAccount(customerid, name,customernum, key, archived);

                customers.Add(customerid, newaccount);
            }
            reader.Close();
            db.DBClose();

            if (customers.Count > 0)
            {
                Cache.Insert("Metabase_Customers", customers, getDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), CacheItemPriority.NotRemovable, null);
            }

            return customers;
        }

        System.Web.Caching.CacheDependency getDependency()
        {
            System.Web.Caching.CacheDependency dep;
            String[] dependency;
            dependency = new string[1];
            dependency[0] = "Metabase_Accounts_dependency";
            dep = new System.Web.Caching.CacheDependency(null, dependency);
            return dep;
        }

        private void CreateDependency()
        {
            if (Cache["Metabase_Accounts_dependency"] == null)
            {
                Cache.Insert("Metabase_Accounts_dependency", 1);
            }
        }

        public void InvalidateCache()
        {
            Cache.Remove("Metabase_Accounts_dependency");
            CreateDependency();
        }

        public int Count
        {
            get { return accounts.Count; }
        }

        public bool AccountKeyExists(string URL_Key)
        {
            bool exists = false;
            foreach (KeyValuePair<int, cCustomerAccount> i in accounts)
            {
                cCustomerAccount acc = (cCustomerAccount)i.Value;
                if (acc.URL_Key.ToLower().Trim() == URL_Key.ToLower().Trim())
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public int UpdateAccount(cCustomerAccount account)
        {
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpenMetabase(connectionstring, false);

            db.SetFieldValue("Customer Name",account.CustomerName,"S",true);
            db.SetFieldValue("URL Key",account.URL_Key,"S",false);
            db.SetFieldValue("Archived",account.isArchived,"B",false);

            int accountid = 0;
            if (account.CustomerId == 0)
            {
                // must be adding
                if (!AccountKeyExists(account.URL_Key))
                {
                    db.FWDb("W", "customers", "", "", "", "", "", "", "", "", "", "", "", "");
                    accountid = db.glIdentity;
                    account.CustomerId = accountid;
                    accounts.Add(accountid, account);
                }
                else
                {
                    // already exists
                    accountid = -1;
                }
            }
            else
            {
                if (accounts.ContainsKey(account.CustomerId))
                {
                    db.FWDb("A", "customers", "Customer Id", account.CustomerId, "", "", "", "", "", "", "", "", "", "");
                    accountid = account.CustomerId;
                    accounts[accountid] = account;
                }
            }

            db.DBClose();
            return accountid;
        }

        public ArrayList GetAccounts()
        {
            ArrayList retAccounts = new ArrayList();

            foreach (KeyValuePair<int, cCustomerAccount> i in accounts)
            {
                cCustomerAccount acc = (cCustomerAccount)i.Value;

                retAccounts.Add(acc);
            }
            return retAccounts;
        }

        public cCustomerAccount GetAccountByKey(string key)
        {
            cCustomerAccount ret_account = null;
            foreach (KeyValuePair<int, cCustomerAccount> i in accounts)
            {
                cCustomerAccount acc = (cCustomerAccount)i.Value;
                if (acc.URL_Key.ToLower().Trim() == key.ToLower().Trim())
                {
                    ret_account = acc;
                    break;
                }
            }
            return ret_account;
        }

        public cCustomerAccount GetCustomerById(int customerid)
        {
            if (accounts.ContainsKey(customerid))
            {
                return accounts[customerid];
            }
            else
            {
                return null;
            }
        }

        public System.Web.UI.WebControls.ListItem[] GetListItems(bool addNoneSelection)
        {
            List<System.Web.UI.WebControls.ListItem> newItem = new List<System.Web.UI.WebControls.ListItem>();

            foreach (KeyValuePair<int, cCustomerAccount> i in accounts)
            {
                cCustomerAccount acc = (cCustomerAccount)i.Value;
                newItem.Add(new System.Web.UI.WebControls.ListItem(acc.CustomerName, acc.CustomerId.ToString()));
            }

            if (addNoneSelection)
            {
                newItem.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
            }

            return newItem.ToArray();
        }

    }

    public class cConnections
    {
        SortedList<int, cFWSettings> connections;
        private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        private string connectionstring;
        public cConnections(string meta_connectionstring)
        {
            connectionstring = meta_connectionstring;

            CreateDependency();

            if (Cache["Metabase_Connections"] == null)
            {
                connections = CacheConnections();
            }
            else
            {
                connections = (SortedList<int, cFWSettings>)Cache["Metabase_Connections"];
            }
        }

        private SortedList<int, cFWSettings> CacheConnections()
        {
            // open the metabase
            SortedList<int, cFWSettings>  slConnections = new SortedList<int, cFWSettings>();
            cCustomerAccounts customers = new cCustomerAccounts(connectionstring);

            cFWDBConnection db = new cFWDBConnection();
            db.DBOpenMetabase(connectionstring, false);

            string sql = "SELECT * FROM fwsettings INNER JOIN customers ON customers.[Customer Id] = fwsettings.[Customer Id] WHERE customers.[Archived] = 0";
            System.Data.SqlClient.SqlDataReader reader;

            reader = db.GetReader(sql);
            while (reader.Read())
            {
                cFWSettings fws = new cFWSettings();

                fws.MetabaseConnectionId = reader.GetInt32(reader.GetOrdinal("Setting Id"));
                fws.MetabaseCustomerId = reader.GetInt32(reader.GetOrdinal("Customer Id"));
                cCustomerAccount acc = customers.GetCustomerById(fws.MetabaseCustomerId);
                fws.glCompanyName = acc.CustomerName;
                fws.MetabaseAccountKey = acc.URL_Key;
                fws.glDBEngine = reader.GetInt16(reader.GetOrdinal("DB Engine"));
                if (!reader.IsDBNull(reader.GetOrdinal("Database")))
                {
                    fws.glDatabase = reader.GetString(reader.GetOrdinal("Database"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("DB Server")))
                {
                    fws.glServer = reader.GetString(reader.GetOrdinal("DB Server"));
                }
                PwdMethod pwdmethod;
                if (!reader.IsDBNull(reader.GetOrdinal("Password Method")))
                {
                    pwdmethod = (PwdMethod)reader.GetInt16(reader.GetOrdinal("Password Method"));
                }
                else
                {
                    pwdmethod = PwdMethod.SHA_Hash;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("DB User")))
                {
                    fws.glDBUserId = reader.GetString(reader.GetOrdinal("DB User"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("DB Password")))
                {
                    fws.glDBPassword = reader.GetString(reader.GetOrdinal("DB Password"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("DB Timeout")))
                {
                    fws.glDBTimeout = reader.GetInt32(reader.GetOrdinal("DB Timeout"));
                }
                else
                {
                    fws.glDBTimeout = 15;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Email Log Path")))
                {
                    fws.glEmailLog = reader.GetString(reader.GetOrdinal("Email Log Path"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Mail Server")))
                {
                    fws.glMailServer = reader.GetString(reader.GetOrdinal("Mail Server"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Mail From")))
                {
                    fws.glMailFrom = reader.GetString(reader.GetOrdinal("Mail From"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("FWLogo Path")))
                {
                    fws.glFWLogo = "./images/seLogo.gif";
                }
                else
                {
                    fws.glFWLogo = reader.GetString(reader.GetOrdinal("FWLogo Path"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Page Size")))
                {
                    fws.glPageSize = reader.GetInt32(reader.GetOrdinal("Page Size"));
                }
                else
                {
                    fws.glPageSize = 0;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Unique Key Prefix")))
                {
                    fws.KeyPrefix = reader.GetString(reader.GetOrdinal("Unique Key Prefix"));
                }
                else
                {
                    fws.KeyPrefix = "CD";
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Doc Repository")))
                {
                    fws.glDocRepository = reader.GetString(reader.GetOrdinal("Doc Repository"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Auditor List")))
                {
                    fws.glAuditorList = reader.GetString(reader.GetOrdinal("Auditor List"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd Expiry")))
                {
                    fws.glPwdExpiry = reader.GetBoolean(reader.GetOrdinal("Pwd Expiry"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd Expiry Days")))
                {
                    fws.glPwdExpiryDays = reader.GetInt32(reader.GetOrdinal("Pwd Expiry Days"));
                }
                else
                {
                    fws.glPwdExpiryDays = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("Pwd Constraint")))
                {
                    fws.glPwdLengthSetting = PasswordLengthSetting.AnyLength;
                }
                else
                {
                    fws.glPwdLengthSetting = (PasswordLengthSetting)reader.GetInt16(reader.GetOrdinal("Pwd Constraint"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd L1")))
                {
                    fws.glPwdLength1 = reader.GetInt16(reader.GetOrdinal("Pwd L1"));
                }
                else
                {
                    fws.glPwdLength1 = 0;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd L2")))
                {
                    fws.glPwdLength2 = reader.GetInt16(reader.GetOrdinal("Pwd L2"));
                }
                else
                {
                    fws.glPwdLength2 = 0;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd MCU")))
                {
                    fws.glPwdUCase = reader.GetBoolean(reader.GetOrdinal("Pwd MCU"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd MCN")))
                {
                    fws.glPwdNums = reader.GetBoolean(reader.GetOrdinal("Pwd MCN"));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd Max Retries")))
                {
                    fws.glMaxRetries = reader.GetInt16(reader.GetOrdinal("Pwd Max Retries"));
                }
                else
                {
                    fws.glMaxRetries = 5;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Pwd History Num")))
                {
                    fws.glNumPwdHistory = reader.GetInt32(reader.GetOrdinal("Pwd History Num"));
                }
                else
                {
                    fws.glNumPwdHistory = 0;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("Init View")))
                {
                    fws.glInitViewType = (ViewType)reader.GetInt16(reader.GetOrdinal("Init View"));
                }
                else
                {
                    fws.glInitViewType = ViewType.Enhanced;
                }
                if (reader.GetBoolean(reader.GetOrdinal("Keep Forecasts")))
                {
                    fws.glKeepForecast = 1;
                }
                else
                {
                    fws.glKeepForecast = 0;
                }
                fws.glUseDUNS = reader.GetBoolean(reader.GetOrdinal("Use DUNS"));
                fws.glAllowMenuAdd = reader.GetBoolean(reader.GetOrdinal("Allow Menu Add"));
                fws.glAllowNotesAdd = reader.GetBoolean(reader.GetOrdinal("Allow Notes Add"));
                fws.glShowProductonSearch = reader.GetBoolean(reader.GetOrdinal("Show Product"));
                if (reader.IsDBNull(reader.GetOrdinal("Web URL")))
                {
                    fws.glWebEmailerURL = "";
                }
                else
                {
                    fws.glWebEmailerURL = reader.GetString(reader.GetOrdinal("Web URL"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("Web Email Admin")))
                {
                    fws.glWebEmailerAdmin = "";
                }
                else
                {
                    fws.glWebEmailerAdmin = reader.GetString(reader.GetOrdinal("Web Email Admin"));
                }                
                fws.glAutoUpdateCV = reader.GetBoolean(reader.GetOrdinal("Auto Update CV"));
                if (reader.IsDBNull(reader.GetOrdinal("Application Path")))
                {
                    fws.glApplicationPath = "";
                }
                else
                {
                    fws.glApplicationPath = reader.GetString(reader.GetOrdinal("Application Path"));
                }
                fws.glUseRechargeFunction = reader.GetBoolean(reader.GetOrdinal("Use Recharge"));
                fws.glUseSavings = reader.GetBoolean(reader.GetOrdinal("Use Savings"));
                if (reader.IsDBNull(reader.GetOrdinal("Error Submit")))
                {
                    fws.glErrorSubmitEmail = "framework-errors@software-europe.co.uk";
                }
                else
                {
                    fws.glErrorSubmitEmail = reader.GetString(reader.GetOrdinal("Error Submit"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("Error Submit From")))
                {
                    fws.glErrorSubmitFrom = "unknown_customer@aserver.com";
                }
                else
                {
                    fws.glErrorSubmitFrom = reader.GetString(reader.GetOrdinal("Error Submit From"));
                }

                slConnections.Add(fws.MetabaseCustomerId, fws);
            }
            reader.Close();

            // obtain concurrent user licence for each customer account
            foreach (KeyValuePair<int, cFWSettings> i in slConnections)
            {
                cFWSettings conn = (cFWSettings)i.Value;
                cFWDBConnection customer_db = new cFWDBConnection();
                customer_db.DBOpen(conn, false);
                if (!customer_db.glError)
                {
                    customer_db.FWDb("R3", "system_parameters", "", "", "", "", "", "", "", "", "", "", "", "");

                    if (customer_db.FWDb3Flag)
                    {
                        try
                        {
                            FWCrypt crypt = new FWCrypt(FWCrypt.Providers.RC2);
                            string tmpUsers = crypt.Decrypt(customer_db.FWDbFindVal("Licensed Users", 3), cDef.EncryptionKey);
                            if (tmpUsers.Trim() == "")
                            {
                                conn.glLicencedUsers = 0;
                            }
                            else
                            {
                                conn.glLicencedUsers = int.Parse(tmpUsers);
                            }
                            conn.glActiveDBVersion = int.Parse(customer_db.FWDbFindVal("DBVersion", 3));
                        }
                        catch
                        {
                            conn.glLicencedUsers = 0;
                        }
                    }
                }
                customer_db.DBClose();
            }

            if (slConnections.Count > 0)
            {
                // cache the connections
                Cache.Insert("Metabase_Connections", slConnections, getDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), CacheItemPriority.NotRemovable, null);
            }
            
            db.DBClose();

            return slConnections;
        }

        private void CreateDependency()
        {
            if (Cache["Metabase_Connections_dependency"] == null)
            {
                Cache.Insert("Metabase_Connections_dependency", 1);
            }
        }

        System.Web.Caching.CacheDependency getDependency()
        {
            System.Web.Caching.CacheDependency dep;
            String[] dependency;
            dependency = new string[1];
            dependency[0] = "Metabase_Connections_dependency";
            dep = new System.Web.Caching.CacheDependency(null, dependency);
            return dep;
        }

        public void InvalidateCache()
        {
            Cache.Remove("Metabase_Connections_dependency");
            CreateDependency();
        }

        public int Count
        {
            get { return connections.Count; }
        }

        public cFWSettings GetConnectionById()
        {
            if (connections.Count > 0)
            {
                return (cFWSettings)connections.Values[0];
            }
            else
            {
                return new cFWSettings();
            }
        }

        public cFWSettings GetConnectionById(int customerid)
        {
            if (connections.ContainsKey(customerid))
            {
                return (cFWSettings)connections[customerid];
            }
            else
            {
                return new cFWSettings();
            }
        }

        public int UpdateConnection(cFWSettings fws)
        {
            int connectionid = fws.MetabaseConnectionId;
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpenMetabase(connectionstring, false);

            db.SetFieldValue("Customer Id", fws.MetabaseCustomerId, "N", true);
            db.SetFieldValue("DB Engine", fws.glDBEngine, "N", false);
            db.SetFieldValue("Database", fws.glDatabase, "S", false);
            db.SetFieldValue("Password Method", (int)PwdMethod.SHA_Hash, "N", false);
            db.SetFieldValue("DB User", fws.glDBUserId, "S", false);
            if (fws.glServer.Trim() == "")
            {
                fws.glServer = ".";
            }
            db.SetFieldValue("DB Server", fws.glServer, "S", false);
            db.SetFieldValue("DB Password", fws.glDBPassword, "S", false);
            db.SetFieldValue("DB Timeout", fws.glDBTimeout, "N", false);
            db.SetFieldValue("Email Log Path", fws.glEmailLog, "S", false);
            db.SetFieldValue("Mail Server", fws.glMailServer, "S", false);
            db.SetFieldValue("Mail From", fws.glMailFrom, "S", false);
            if (fws.glFWLogo.Trim() == "")
            {
                fws.glFWLogo = "./images/seLogo.gif";
            }
            db.SetFieldValue("FWLogo Path", fws.glFWLogo, "S", false);
            db.SetFieldValue("Page Size", fws.glPageSize, "N", false);
            db.SetFieldValue("Unique Key Prefix", fws.KeyPrefix, "S", false);
            db.SetFieldValue("Doc Repository", fws.glDocRepository, "S", false);
            db.SetFieldValue("Auditor List", fws.glAuditorList, "S", false);
            db.SetFieldValue("Pwd Expiry", fws.glPwdExpiry, "B", false);
            db.SetFieldValue("Pwd Expiry Days", fws.glPwdExpiryDays, "N", false);
            db.SetFieldValue("Pwd Constraint", (int)fws.glPwdLengthSetting, "N", false);
            db.SetFieldValue("Pwd L1", fws.glPwdLength1, "N", false);
            db.SetFieldValue("Pwd L2", fws.glPwdLength2, "N", false);
            db.SetFieldValue("Pwd MCU", fws.glPwdUCase, "B", false);
            db.SetFieldValue("Pwd MCN", fws.glPwdNums, "B", false);
            db.SetFieldValue("Pwd Max Retries", fws.glMaxRetries, "N", false);
            db.SetFieldValue("Pwd History Num", fws.glNumPwdHistory, "N", false);
            db.SetFieldValue("Init View", (int)fws.glInitViewType, "N", false);
            db.SetFieldValue("Keep Forecasts",fws.glKeepForecast,"B",false);
            db.SetFieldValue("Use DUNS", fws.glUseDUNS, "B", false);
            db.SetFieldValue("Allow Menu Add", fws.glAllowMenuAdd, "B", false);
            db.SetFieldValue("Allow Notes Add", fws.glAllowNotesAdd, "B", false);
            db.SetFieldValue("Show Product", fws.glShowProductonSearch, "B", false);
            db.SetFieldValue("Web URL",fws.glWebEmailerURL ,"S",false);
            db.SetFieldValue("Web Email Admin", fws.glWebEmailerAdmin, "S", false);
            db.SetFieldValue("Auto Update CV", fws.glAutoUpdateCV, "B", false);
            db.SetFieldValue("Application Path", fws.glApplicationPath, "S", false);
            db.SetFieldValue("Use Recharge", fws.glUseRechargeFunction, "B", false);
            db.SetFieldValue("Use Savings", fws.glUseSavings, "B", false);
            if (fws.glErrorSubmitEmail.Trim() == "")
            {
                fws.glErrorSubmitEmail = "framework-errors@software-europe.co.uk";
            }
            db.SetFieldValue("Error Submit", fws.glErrorSubmitEmail, "S", false);
            if (fws.glErrorSubmitFrom.Trim() == "")
            {
                fws.glErrorSubmitFrom = "unknown_customer@aserver.com";
            }
            db.SetFieldValue("Error Submit From", fws.glErrorSubmitFrom, "S", false);
            
            if (fws.MetabaseConnectionId == 0)
            {
                if (fws.MetabaseCustomerId != 0)
                {
                    // no current connection setting exists at the moment
                    db.FWDb("W", "fwsettings", "", "", "", "", "", "", "", "", "", "", "", "");
                    fws.MetabaseConnectionId = db.glIdentity;
                    connectionid = fws.MetabaseConnectionId;
                    connections.Add(fws.MetabaseCustomerId, fws);
                }
            }
            else
            {
                // updating existing fws connection
                db.FWDb("A", "fwsettings", "Setting Id", fws.MetabaseConnectionId, "", "", "", "", "", "", "", "", "", "");
                connections[fws.MetabaseCustomerId] = fws;
            }

            db.DBClose();
            return connectionid;
        }
    }
}
