using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SpendManagementLibrary;
using FWBase;

namespace FWClasses
{
    public class cEmailSenderConnections
    {
        string connectionstring;

        public cEmailSenderConnections(string meta_connectionstring)
        {
            connectionstring = meta_connectionstring;
        }

        public SortedList<int, cCustomerAccount> GetCustomerAccounts()
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
                cCustomerAccount newaccount = new cCustomerAccount(customerid, name, customernum, key, archived);

                customers.Add(customerid, newaccount);
            }
            reader.Close();
            db.DBClose();

            return customers;
        }

        public SortedList<int, cFWSettings> GetCustomerConnections(SortedList<int,cCustomerAccount> customers)
        {
            // open the metabase
            SortedList<int, cFWSettings> slConnections = new SortedList<int, cFWSettings>();

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
                if (customers.ContainsKey(fws.MetabaseCustomerId))
                {
                    cCustomerAccount acc = (cCustomerAccount)customers[fws.MetabaseCustomerId];
                    fws.glCompanyName = acc.CustomerName;
                    fws.MetabaseAccountKey = acc.URL_Key;
                }
                else
                {
                    fws.glCompanyName = "Unknown";
                    fws.MetabaseAccountKey = "";
                }
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

            db.DBClose();

            return slConnections;
        }
    }
}
