using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Caching;
using System.Data.SqlClient;

using SpendManagementLibrary;

namespace SEL_ReportEngine
{
    public class cAccounts
    {
        private static SortedList<int, cAccount> list;
        private static SortedList<string, cAccount> lstcompanyids;

        public cAccounts()
        {
            InitialiseData();
        }

        private void InitialiseData()
        {
            if (list == null)
            {
                list = CacheList();
                CreateSqlDependencies();
            }
        }

        public SortedList<int, cAccount> accounts
        {
            get { return list; }
        }

        private SortedList<int, cAccount> CacheList()
        {
            DBConnection db = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            SortedList<int, cAccount> lst = new SortedList<int, cAccount>();
            lstcompanyids = new SortedList<string, cAccount>();

            cAccount accnt;
            int accountid, dbserverid;
            string companyname, companyid, contact, dbserver = "", dbname, dbusername, dbpassword;
            byte accounttype;
            bool archived;
            SortedList<int, string> servers = getDBServers();
            int nousers;
            DateTime expiry;
            bool autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled;
            int expensesConnectLicenses;
            int? reportDatabaseID;
            int hostnameID;
            string hostname;
            bool isNHSCustomer;
            string licencedUsers;

            string strsql = "SELECT accountid, companyname, companyid, contact, nousers, accounttype, expiry, dbserver, dbname, dbusername, dbpassword, archived, autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled, expensesConnectLicenses, reportDatabaseID, hostnameID,  isNHSCustomer, licencedUsers FROM dbo.registeredusers";
            db.sqlexecute.CommandText = strsql;

            SqlDataReader reader;

            SqlDependency dep = new SqlDependency(db.sqlexecute);
            dep.OnChange += new OnChangeEventHandler(dep_OnChange);
            using (reader = db.GetReader(strsql))
            {
                db.ExecuteSQL(strsql);

                while (reader.Read())
                {
                    accountid = reader.GetInt32(reader.GetOrdinal("accountid"));
                    companyname = reader.GetString(reader.GetOrdinal("companyname"));
                    companyid = reader.GetString(reader.GetOrdinal("companyid"));
                    contact = reader.GetString(reader.GetOrdinal("contact"));
                    accounttype = reader.GetByte(reader.GetOrdinal("accounttype"));
                    nousers = reader.GetInt32(reader.GetOrdinal("nousers"));
                    expiry = reader.GetDateTime(reader.GetOrdinal("expiry"));
                    if (reader.IsDBNull(reader.GetOrdinal("dbserver")) == true)
                    {
                        dbserverid = 0;
                    }
                    else
                    {
                        dbserverid = reader.GetInt32(reader.GetOrdinal("dbserver"));
                    }
                    if (dbserverid > 0)
                    {
                        if (servers.Count > 0)
                        {
                            dbserver = servers[dbserverid];
                        }
                    }
                    else
                    {
                        dbserver = "";
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("dbname")) == true)
                    {
                        dbname = "";
                    }
                    else
                    {
                        dbname = reader.GetString(reader.GetOrdinal("dbname"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("dbusername")) == true)
                    {
                        dbusername = "";
                    }
                    else
                    {
                        dbusername = reader.GetString(reader.GetOrdinal("dbusername"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("dbpassword")) == true)
                    {
                        dbpassword = "";
                    }
                    else
                    {
                        dbpassword = reader.GetString(reader.GetOrdinal("dbpassword"));
                    }
                    archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                    autologEnabled = reader.GetBoolean(reader.GetOrdinal("autologEnabled"));
                    quickEntryFormsEnabled = reader.GetBoolean(reader.GetOrdinal("quickEntryFormsEnabled"));
                    employeeSearchEnabled = reader.GetBoolean(reader.GetOrdinal("employeeSearchEnabled"));
                    hotelReviewsEnabled = reader.GetBoolean(reader.GetOrdinal("hotelReviewsEnabled"));
                    advancesEnabled = reader.GetBoolean(reader.GetOrdinal("advancesEnabled"));
                    postcodeAnyWhereEnabled = reader.GetBoolean(reader.GetOrdinal("postcodeAnyWhereEnabled"));
                    corporateCardsEnabled = reader.GetBoolean(reader.GetOrdinal("corporateCardsEnabled"));
                    if (reader.IsDBNull(reader.GetOrdinal("expensesConnectLicenses")) == true)
                    {
                        expensesConnectLicenses = 0;
                    }
                    else
                    {
                        expensesConnectLicenses = reader.GetInt32(reader.GetOrdinal("expensesConnectLicenses"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("reportDatabaseID")) == true)
                    {
                        reportDatabaseID = null;
                    }
                    else
                    {
                        reportDatabaseID = reader.GetInt32(reader.GetOrdinal("reportDatabaseID"));
                    }

                    hostnameID = reader.GetInt32(reader.GetOrdinal("hostnameID"));
                    hostname = GetHostnameByID(hostnameID);
                    isNHSCustomer = reader.GetBoolean(reader.GetOrdinal("isNHSCustomer"));
                    if (!reader.IsDBNull(reader.GetOrdinal("licencedUsers")))
                    {
                        licencedUsers = reader.GetString(reader.GetOrdinal("licencedUsers"));
                    }
                    else
                    {
                        licencedUsers = "";
                    }
                    accnt = new cAccount(accountid, companyname, companyid, contact, nousers, expiry, accounttype, dbserver, dbname, dbusername, dbpassword, archived, autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled, expensesConnectLicenses, reportDatabaseID, hostnameID, hostname, isNHSCustomer, 0, false, "", licencedUsers);

                    if (lst.ContainsKey(accountid) == false)
                    {
                        lst.Add(accountid, accnt);
                        lstcompanyids.Add(companyid, accnt);
                    }
                }
                reader.Close();
            }
            return lst;
        }

        void dep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            list = null;
            lstcompanyids = null;
            list = CacheList();
            CreateSqlDependencies();
        }

        private SortedList<int, string> getDBServers()
        {
            SortedList<int, string> servers = new SortedList<int, string>();

            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            System.Data.SqlClient.SqlDataReader reader;
            int databaseid;
            string hostname;

            string strsql = "select databaseid, hostname from databases";
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    databaseid = reader.GetInt32(reader.GetOrdinal("databaseid"));
                    hostname = reader.GetString(reader.GetOrdinal("hostname"));
                    servers.Add(databaseid, hostname);
                }
                reader.Close();
            }

            return servers;
        }

        public cAccount getAccountById(int accountid)
        {
            cAccount account = null;
            list.TryGetValue(accountid, out account);
            return account;
        }

        private void CreateSqlDependencies()
        {
            foreach (cAccount accnt in list.Values)
            {
                if (!accnt.archived)
                {
                    try
                    {
                        SqlDependency.Start(cAccounts.getConnectionString(accnt.accountid));
                    }
                    catch
                    {

                    }
                }
            }
        }

        public static string getConnectionString(int accountid)
        {
            string connectionstring = (string)System.Web.HttpRuntime.Cache["connectionstring" + accountid];
            if (connectionstring == null)
            {
                cAccounts clsaccounts = new cAccounts();
                cAccount account = clsaccounts.getAccountById(accountid);
                cSecureData clssecure = new cSecureData();
                connectionstring = "Data Source=" + account.dbserver + ";Initial Catalog=" + account.dbname + ";User ID=" + account.dbusername + ";Password=" + clssecure.Decrypt(account.dbpassword) + ";Max Pool Size=10000;";
                System.Web.HttpRuntime.Cache.Insert("connectionstring" + accountid, connectionstring, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
            return connectionstring;
        }

        private string GetHostnameByID(int hostnameID)
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            System.Data.SqlClient.SqlDataReader reader;
            string hostname = string.Empty;

            expdata.sqlexecute.Parameters.AddWithValue("@hostnameID", hostnameID);

            string strsql = "select hostname from hostnames WHERE hostnameID=@hostnameID";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    hostname = reader.GetString(reader.GetOrdinal("hostname"));
                }
                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();

            return hostname;
        }
    }
}
