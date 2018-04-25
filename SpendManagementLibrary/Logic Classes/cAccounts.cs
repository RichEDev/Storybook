namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic.FilePath;

    using Helpers;
    using API;
    using Enumerators;

    using SpendManagementLibrary.Logic_Classes;

    /// <summary>
    /// Manages the data access for an Account.
    /// </summary>
    public class cAccounts
    {
        #region Static Fields

        public static readonly ConcurrentDictionary<int, cAccount> CachedAccounts = new ConcurrentDictionary<int, cAccount>();
        
        private static readonly object CacheLock = new object();

        #endregion

        #region Constructors and Destructors

        public cAccounts()
        {
            this.MetabaseConnectionString = GlobalVariables.MetabaseConnectionString;
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Enumerator for ToggleDependencies
        /// </summary>
        public enum ToggleDependancy
        {
            /// <summary>
            ///     Indicates that SqlDependency.Start should be called.
            /// </summary>
            Start = 1,

            /// <summary>
            ///     Indicates that SqlDependency.Stop should be called.
            /// </summary>
            Stop = 2
        }

        #endregion

        #region Public Properties

        public string MetabaseConnectionString
        {
            get;
            set;
        }

        #endregion

        #region Public Methods and Operators

        public static string getConnectionString(int accountID, cAccounts instance = null)
        {
            string connectionString = string.Empty;
            instance = instance ?? new cAccounts();
            cAccount reqAccount = instance.GetAccountByID(accountID);
            if (reqAccount != null)
            {
                connectionString = reqAccount.ConnectionString;
            }

            return connectionString;
        }

        public bool CacheList(bool startSqlDepenencies = true)
        {
            SortedList<int, cAccount> lstAccounts = new SortedList<int, cAccount>();
            cEventlog.LogEntry("Cache List starting.");

            lock (CacheLock)
            {
                cEventlog.LogEntry("Starting to cache registeredusers.");

                DBConnection db = new DBConnection(this.MetabaseConnectionString);

                const int StartYear = 0;
                cSecureData clsSecureData = new cSecureData();
                List<int> accountIds = CachedAccounts.Keys.ToList();
                cEventlog.LogEntry("Get Database Servers.");
                Dictionary<int, string> lstDatabaseServers = this.GetDatabaseServers();
                cEventlog.LogEntry("Get Hostnames.");
                List<Tuple<int, int>> lstRegisteredUsersHostnames = this.GetHostnames();

                using (SqlDataReader reader = db.GetProcReader("dbo.GetAccounts"))
                {
                    #region Ordinals

                    int accountIDOrdID = reader.GetOrdinal("accountID");
                    int dbServerIDOrdID = reader.GetOrdinal("dbserver");
                    int companyNameOrdID = reader.GetOrdinal("companyname");
                    int companyIDOrdID = reader.GetOrdinal("companyid");
                    int contactOrdID = reader.GetOrdinal("contact");
                    int dbNameOrdID = reader.GetOrdinal("dbname");
                    int dbUsernameOrdID = reader.GetOrdinal("dbusername");
                    int dbPasswordOrdID = reader.GetOrdinal("dbpassword");
                    int accountTypeOrdID = reader.GetOrdinal("accounttype");
                    int archivedOrdID = reader.GetOrdinal("archived");
                    int numberOfUsersOrdID = reader.GetOrdinal("nousers");
                    int expiryDateOrdID = reader.GetOrdinal("expiry");
                    int quickEntryFormsEnabledOrdID = reader.GetOrdinal("quickEntryFormsEnabled");
                    int employeeSearchEnabledOrdID = reader.GetOrdinal("employeeSearchEnabled");
                    int hotelReviewsEnabledOrdID = reader.GetOrdinal("hotelReviewsEnabled");
                    int advancesEnabledOrdID = reader.GetOrdinal("advancesEnabled");
                    int postcodeAnyWhereEnabledOrdID = reader.GetOrdinal("postcodeAnyWhereEnabled");
                    int corporateCardsEnabledOrdID = reader.GetOrdinal("corporateCardsEnabled");
                    int reportDatabaseIDOrdID = reader.GetOrdinal("reportDatabaseID");
                    int isNhsCustomerOrdID = reader.GetOrdinal("isNHSCustomer");
                    int contactHelpDeskAllowedOrdID = reader.GetOrdinal("contactHelpDeskAllowed");
                    int postcodeAnywhereKeyOrdID = reader.GetOrdinal("postcodeAnywhereKey");
                    int licencedUsersOrdID = reader.GetOrdinal("licencedUsers");
                    int mapsEnabledOrdID = reader.GetOrdinal("mapsEnabled");
                    int receiptServiceEnabledOrdId = reader.GetOrdinal("ReceiptServiceEnabled");
                    int addressLookupProviderOrdinalId = reader.GetOrdinal("addressLookupProvider");
                    int addressLookupsChargeableOrdinalId = reader.GetOrdinal("addressLookupsChargeable");
                    int addressLookupPsmaAgreementOrdinalId = reader.GetOrdinal("addressLookupPsmaAgreement");
                    int addressInternationalLookupsAndCoordinatesOrdinalId = reader.GetOrdinal("addressInternationalLookupsAndCoordinates");
                    int addressLookupsRemainingOrdinalId = reader.GetOrdinal("addressLookupsRemaining");
                    int addressDistanceLookupsRemainingOrdinalId = reader.GetOrdinal("addressDistanceLookupsRemaining");
                    int licenceTypeOrdinalId = reader.GetOrdinal("licenceType");
                    int annualContractOrdinalId = reader.GetOrdinal("annualContract");
                    int renewalDateOrdinalId = reader.GetOrdinal("renewalDate");
                    int contactEmailAddressOrdinalId = reader.GetOrdinal("contactEmail");
                    int dvlaLookUpKeyOrdId = reader.GetOrdinal("DVLALookUpKey");
                    int postCodeAnywherePaymentServiceKeyOrd = reader.GetOrdinal("postCodeAnywherePaymentServiceKey");
                    #endregion Ordinals

                    while (reader.Read())
                    {
                        #region reader contents

                        int accountID = reader.GetInt32(accountIDOrdID);
                        string companyName = reader.GetString(companyNameOrdID);
                        string companyID = reader.GetString(companyIDOrdID);
                        string contact = reader.GetString(contactOrdID);
                        byte accountType = reader.GetByte(accountTypeOrdID);
                        int numberOfUsers = reader.GetInt32(numberOfUsersOrdID);
                        DateTime expiryDate = reader.GetDateTime(expiryDateOrdID);
                        bool quickEntryFormsEnabled = reader.GetBoolean(quickEntryFormsEnabledOrdID);
                        bool employeeSearchEnabled = reader.GetBoolean(employeeSearchEnabledOrdID);
                        bool hotelReviewsEnabled = reader.GetBoolean(hotelReviewsEnabledOrdID);
                        bool advancesEnabled = reader.GetBoolean(advancesEnabledOrdID);
                        bool postcodeAnyWhereEnabled = reader.GetBoolean(postcodeAnyWhereEnabledOrdID);
                        bool corporateCardsEnabled = reader.GetBoolean(corporateCardsEnabledOrdID);
                        bool archived = reader.GetBoolean(archivedOrdID);
                        bool isNhsCustomer = reader.GetBoolean(isNhsCustomerOrdID);
                        bool contactHelpDeskAllowed = reader.GetBoolean(contactHelpDeskAllowedOrdID);

                        List<int> hostnameIDs = (from usersHostname in lstRegisteredUsersHostnames
                                                 where usersHostname.Item1 == accountID
                                                 select usersHostname.Item2).ToList();

                        // Currently set to nullable, needs changing to not null in database
                        int dbServerID = reader.GetInt32(dbServerIDOrdID);
                        string dbServer;
                        lstDatabaseServers.TryGetValue(dbServerID, out dbServer);
                        string dbName = reader.GetString(dbNameOrdID);
                        string dbUsername = reader.GetString(dbUsernameOrdID);
                        string dbPassword = reader.GetString(dbPasswordOrdID);
                        string postcodeAnywhereKey = reader.IsDBNull(postcodeAnywhereKeyOrdID) ? string.Empty : reader.GetString(postcodeAnywhereKeyOrdID);
                        string licencedUsers = reader.IsDBNull(licencedUsersOrdID) ? string.Empty : reader.GetString(licencedUsersOrdID);
                        int? reportDatabaseID = reader.IsDBNull(reportDatabaseIDOrdID) ? (int?)null : reader.GetInt32(reportDatabaseIDOrdID);
                        bool mapsEnabled = reader.GetBoolean(mapsEnabledOrdID);                                       
                        AddressLookupProvider addressLookupProvider = (AddressLookupProvider)reader.GetByte(addressLookupProviderOrdinalId);
                        bool addressLookupsChargeable = reader.GetBoolean(addressLookupsChargeableOrdinalId);
                        bool addressLookupPsmaAgreement = reader.GetBoolean(addressLookupPsmaAgreementOrdinalId);
                        bool addressInternationalLookupsAndCoordinates = reader.GetBoolean(addressInternationalLookupsAndCoordinatesOrdinalId);
                        int addressLookupsRemaining = reader.GetInt32(addressLookupsRemainingOrdinalId);
                        int addressDistanceLookupsRemaining = reader.GetInt32(addressDistanceLookupsRemainingOrdinalId);
                        byte? licenceType = reader.IsDBNull(licenceTypeOrdinalId) ? (byte?)null : reader.GetByte(licenceTypeOrdinalId);
                        bool annualContract = reader.GetBoolean(annualContractOrdinalId);
                        string renewalDate = reader.IsDBNull(renewalDateOrdinalId) ? string.Empty : reader.GetString(renewalDateOrdinalId);
                        string contactEmailAddress = reader.IsDBNull(contactEmailAddressOrdinalId) ? string.Empty : reader.GetString(contactEmailAddressOrdinalId);

                        bool receiptServiceEnabled = reader.GetRequiredValue<bool>("ReceiptServiceEnabled");
                        bool validationServiceEnabled = reader.GetRequiredValue<bool>("ValidationServiceEnabled");
                        int? daysToWaitBeforeEnvelopeIsMissing = reader.GetNullable<int>("DaysToWaitBeforeSentEnvelopeIsMissing");
                        bool paymentServiceEnabled = reader.GetRequiredValue<bool>("PaymentServiceEnabled");
                        var dvlaLookUpKey = reader.IsDBNull(dvlaLookUpKeyOrdId) ? string.Empty : reader.GetString(dvlaLookUpKeyOrdId);
                        string postCodeAnywherePaymentServiceKey = reader.IsDBNull(postCodeAnywherePaymentServiceKeyOrd)
                            ? string.Empty : reader.GetString(postCodeAnywherePaymentServiceKeyOrd);
                        #region connection string builder

                        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
                        {
                            DataSource = dbServer,
                            InitialCatalog = dbName,
                            UserID = dbUsername,
                            Password = clsSecureData.Decrypt(dbPassword),
                            MaxPoolSize = 10000,
                            ApplicationName = GlobalVariables.DefaultApplicationInstanceName
                        };

                        #endregion connection string builder

                        #endregion reader contents
                        cEventlog.LogEntry(string.Format("Adding company - {0} to cache.", companyName));
                        
                        lstAccounts.Add(accountID, new cAccount(accountID, companyName, companyID, contact, numberOfUsers, expiryDate, accountType, dbServerID, dbServer, dbName, dbUsername, dbPassword, archived, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled, reportDatabaseID, hostnameIDs, isNhsCustomer, StartYear, contactHelpDeskAllowed, postcodeAnywhereKey, licencedUsers, connectionStringBuilder.ConnectionString, mapsEnabled, receiptServiceEnabled, addressLookupProvider, addressLookupsChargeable, addressLookupPsmaAgreement, addressInternationalLookupsAndCoordinates, addressLookupsRemaining, addressDistanceLookupsRemaining, dvlaLookUpKey, licenceType, annualContract, renewalDate, contactEmailAddress, validationServiceEnabled,paymentServiceEnabled,postCodeAnywherePaymentServiceKey,daysToWaitBeforeEnvelopeIsMissing));
                    }

                    reader.Close();
                }

                #region Module + Elements
                cEventlog.LogEntry("Get Account Modules.");
                List<cAccountModuleLicenses> lstAccountModuleLicenses = this.GetAccountModules();
                cEventlog.LogEntry("Got Account Modules.");
                foreach (cAccountModuleLicenses accountModule in lstAccountModuleLicenses)
                {
                    if (lstAccounts.ContainsKey(accountModule.AccountID))
                    {
                        if (lstAccounts[accountModule.AccountID].AccountModuleLicenses == null)
                        {
                            lstAccounts[accountModule.AccountID].AccountModuleLicenses = new List<cAccountModuleLicenses>();
                        }

                        lstAccounts[accountModule.AccountID].AccountModuleLicenses.Add(accountModule);
                    }
                }
                cEventlog.LogEntry("Get Account Elements.");
                SortedList<int, List<cElement>> lstElements = this.GetAccountElements();

                foreach (KeyValuePair<int, List<cElement>> kvp in lstElements)
                {
                    if (lstAccounts.ContainsKey(kvp.Key))
                    {
                        lstAccounts[kvp.Key].AccountElements = kvp.Value;
                    }
                }

                #endregion Module + Elements

                #region Update, Add or Remove cached list entries

                foreach (KeyValuePair<int, cAccount> kvp in lstAccounts)
                {
                    #region Start Year

                    if (kvp.Value.archived == false)
                    {
                        kvp.Value.startYear = this.GetCompanyStartYear(kvp.Value.ConnectionString);
                    }

                    #endregion Start Year

                    if (accountIds.Remove(kvp.Key))
                    {
                        CachedAccounts[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        CachedAccounts.TryAdd(kvp.Key, kvp.Value);
                    }
                }

                foreach (int accountId in accountIds)
                {
                    cAccount deletedAccount;
                    CachedAccounts.TryRemove(accountId, out deletedAccount);
                }

                #endregion Update, Add or Remove cached list entries

                cEventlog.LogEntry("Completed caching registeredusers.");
            }

            if (startSqlDepenencies)
            {
            cEventlog.LogEntry("Starting SqlDependency.Start on each unarchived cAccount.");
            this.ToggleDependencies(ToggleDependancy.Start);
            cEventlog.LogEntry("Completed SqlDependency.Start on each unarchived cAccount.");
            }

            return lstAccounts.Count == CachedAccounts.Count;
        }

        /// <summary>
        /// Gets all the accounts which have their <see cref="cAccount.ReceiptServiceEnabled"/> property set to True.
        /// </summary>
        /// <returns></returns>
        public List<cAccount> GetAccountsWithReceiptServiceEnabled()
        {
            if (CachedAccounts.IsEmpty)
            {
                CacheList();
            }

            return CachedAccounts.Where(a => !a.Value.archived && a.Value.ReceiptServiceEnabled).Select(a => a.Value).ToList();
        }

        /// <summary>
        /// Gets all the accounts which have their <see cref="cAccount.ValidationServiceEnabled"/> property set to True.
        /// </summary>
        /// <returns></returns>
        public List<cAccount> GetAccountsWithValidationServiceEnabled()
        {
            if (CachedAccounts.IsEmpty)
            {
                CacheList();
            }

            return CachedAccounts.Where(a => !a.Value.archived && a.Value.ValidationServiceEnabled).Select(a => a.Value).ToList();
        }

        /// <summary>
        /// Decreases an account's remaining address lookups by 1
        /// </summary>
        /// <param name="accountId">The id of the account</param>
        public void DecrementAddressLookupsRemaining(int accountId)
        {
            cAccount account;
            if (CachedAccounts.TryGetValue(accountId, out account) == false || account.AddressLookupChargeable == false)
            {
                return;
            }

            var db = new DBConnection(GlobalVariables.MetabaseConnectionString);
            string sql = string.Format("UPDATE registeredusers SET addressLookupsRemaining = {0} WHERE accountid = {1};", account.AddressLookupsRemaining - 1, account.accountid);

            db.ExecuteSQL(sql);

            account.AddressLookupsRemaining -= 1;
        }

        /// <summary>
        /// Decreases an account's remaining address distance lookups by 1
        /// </summary>
        /// <param name="accountId">The id of the account</param>
        public void DecrementAddressDistanceLookupsRemaining(int accountId)
        {
            cAccount account;
            if (CachedAccounts.TryGetValue(accountId, out account) == false || account.AddressLookupChargeable == false)
            {
                return;
            }

            var db = new DBConnection(GlobalVariables.MetabaseConnectionString);
            string sql = string.Format("UPDATE registeredusers SET AddressDistanceLookupsRemaining = {0} WHERE accountid = {1};", account.AddressDistanceLookupsRemaining - 1, account.accountid);

            db.ExecuteSQL(sql);

            account.AddressDistanceLookupsRemaining -= 1;
        }

        public cAccount GetAccountByCompanyID(string companyID)
        {
            return CachedAccounts.Values.FirstOrDefault(tmpAccount => tmpAccount.companyid.ToUpperInvariant() == companyID.ToUpperInvariant());
        }

        public virtual cAccount GetAccountByID(int accountID)
        {
            cAccount reqAccount;
            CachedAccounts.TryGetValue(accountID, out reqAccount);
            return reqAccount;
        }

        /// <summary>
        ///     Get all information from moduleLicencesBase
        /// </summary>
        /// <returns></returns>
        public List<cAccountModuleLicenses> GetAccountModules()
        {
            List<cAccountModuleLicenses> lstAccountModuleLicenses = new List<cAccountModuleLicenses>();

            DBConnection db = new DBConnection(this.MetabaseConnectionString);
            cEventlog.LogEntry("Get Account Modules - new cModules.");
            cModules clsModules = new cModules();

            using (SqlDataReader reader = db.GetReader("SELECT moduleID, accountID, expiryDate, maxUsers FROM dbo.moduleLicencesBase"))
            {
                while (reader.Read())
                {
                    int moduleID = reader.GetInt32(0);
                    int accountID = reader.GetInt32(1);
                    DateTime expiryDate = reader.GetDateTime(2);
                    int maxUsers = reader.GetInt32(3);
                    cModule tmpModule = clsModules.GetModuleByID(moduleID);

                    lstAccountModuleLicenses.Add(new cAccountModuleLicenses(moduleID, accountID, maxUsers, expiryDate, tmpModule));
                }

                reader.Close();
            }

            return lstAccountModuleLicenses;
        }

        /// <summary>
        ///     Obtain a complete list of the databases setup for this application
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetDatabaseServers()
        {
            Dictionary<int, string> lstServers = new Dictionary<int, string>();

            DBConnection db = new DBConnection(this.MetabaseConnectionString);

            using (SqlDataReader reader = db.GetReader("SELECT databaseid, hostname FROM dbo.databases"))
            {
                while (reader.Read())
                {
                    int databaseid = reader.GetInt32(0);
                    string hostname = reader.GetString(1);
                    lstServers.Add(databaseid, hostname);
                }

                reader.Close();
            }

            return lstServers;
        }

        /// <summary>
        /// Gets a single folder path, for the specified account and the FilePathType.
        /// </summary>
        /// <param name="accountId">The account to look in.</param>
        /// <param name="filePathType">The <see cref="FilePathType"/> to get the path for.</param>
        /// <returns></returns>
        [Obsolete("This functionality has been moved to SpendManagementLibrary.GlobalFolderPaths", false)]
        public string GetFilePaths(int accountId, FilePathType filePathType)
        {
            var globalFolderPaths = new GlobalFolderPaths();
            return globalFolderPaths.GetSingleFolderPath(accountId, filePathType);
        }

        public List<int> GetLicensedAccountElementIDs(int accountID)
        {
            List<cElement> lstLicencedElements = this.GetAccountByID(accountID).AccountElements;
            List<int> lstElementIDs = new List<int>();
            foreach (cElement tmpElement in lstLicencedElements)
            {
                if (lstElementIDs.Contains(tmpElement.ElementID) == false)
                {
                    lstElementIDs.Add(tmpElement.ElementID);
                }
            }

            return lstElementIDs;
        }

        public void ToggleDependencies(ToggleDependancy toggle)
        {
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                foreach (cAccount tmpAccount in CachedAccounts.Values)
                {
                    if (tmpAccount.archived == false)
                    {
                        if (toggle == ToggleDependancy.Start)
                        {
                            if (tmpAccount.SqlDependencyStarted == false)
                            {
                                cEventlog.LogEntry(string.Format("Starting dependancy on {0}.", tmpAccount.companyname));
                                SqlDependency.Start(tmpAccount.ConnectionString);
                                tmpAccount.SqlDependencyStarted = true;
                            }
                        }
                        else
                        {
                            if (tmpAccount.SqlDependencyStarted)
                            {
                                cEventlog.LogEntry(string.Format("Stopping dependancy on {0}.", tmpAccount.companyname));
                                SqlDependency.Stop(tmpAccount.ConnectionString);
                                tmpAccount.SqlDependencyStarted = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the daily free call limits for each account.
        /// </summary>
        public void ResetDailyFreeCallLimits()
        {
            var apiLicenseStatus = new ApiLicenseStatusManager();
            apiLicenseStatus.ResetDailyFreeCallLimits();
        }       

        /// <summary>
        /// Gets all the accounts which have their <see cref="cAccount.PaymentServiceEnabled"/> property set to True.
        /// </summary>
        /// <returns></returns>
        public List<cAccount> GetAccountsWithPaymentServiceEnabled()
        {
            List<cAccount> accounts = new List<cAccount>();
            if (CachedAccounts.IsEmpty)
            {
                CacheList();
            }
            if (CachedAccounts.Any(a => !a.Value.archived && a.Value.PaymentServiceEnabled))
            {
                accounts = CachedAccounts.Where(a => !a.Value.archived && a.Value.PaymentServiceEnabled).Select(a => a.Value).ToList();
            }
            return accounts;
        }

        /// <summary>
        /// Gets all the accounts
        /// </summary>
        /// <returns></returns>
        public List<cAccount> GetAllAccounts()
        {
            if (CachedAccounts.IsEmpty)
            {
                CacheList();
            }

            return CachedAccounts.Where(a => !a.Value.archived).Select(a => a.Value).ToList();
        }
        #endregion

        #region Methods

        /// <summary>
        ///     Gets all the licensed elements for all accounts, used in the caching of accounts
        /// </summary>
        /// <returns></returns>
        private SortedList<int, List<cElement>> GetAccountElements()
        {
            cElements clsElements = new cElements();
            SortedList<int, List<cElement>> lstAccountsElements = new SortedList<int, List<cElement>>();

            DBConnection db = new DBConnection(this.MetabaseConnectionString);
            using (SqlDataReader reader = db.GetReader("SELECT accountID, elementID FROM dbo.accountsLicencedElements"))
            {
                while (reader.Read())
                {
                    int accountID = reader.GetInt32(0);
                    int elementID = reader.GetInt32(1);

                    if (lstAccountsElements.ContainsKey(accountID) == false)
                    {
                        lstAccountsElements.Add(accountID, new List<cElement>());
                    }

                    cElement tmpElement = clsElements.GetElementByID(elementID);

                    if (tmpElement != null && lstAccountsElements.ContainsKey(accountID) && lstAccountsElements[accountID].Contains(tmpElement) == false)
                    {
                        lstAccountsElements[accountID].Add(tmpElement);
                    }
                }

                reader.Close();
            }

            return lstAccountsElements;
        }

        private int GetCompanyStartYear(string connectionString)
        {
            cEventlog.LogEntry("Get Company Start Year.");
            DateTime? startDate = null;
            int i;

            try
            {
                DBConnection db = new DBConnection(connectionString);
                using (SqlDataReader reader = db.GetReader("SELECT TOP 1 date FROM savedexpenses ORDER BY date ASC"))
                {
                    while (reader.Read())
                    {
                        startDate = reader.GetDateTime(0);
                    }

                    reader.Close();
                }
            }
            finally
            {
                i = startDate.HasValue ? startDate.Value.Year : 0;
            }

            return i;
        }

        /// <summary>
        ///     The get hostnames.
        /// </summary>
        private List<Tuple<int, int>> GetHostnames()
        {
            List<Tuple<int, int>> hostnames = new List<Tuple<int, int>>();

            DBConnection db = new DBConnection(this.MetabaseConnectionString);

            using (SqlDataReader reader = db.GetReader("SELECT accountid, hostnameid FROM dbo.registeredusershostnames"))
            {
                while (reader.Read())
                {
                    int accountId = reader.GetInt32(0);
                    int hostNameId = reader.GetInt32(1);
                    hostnames.Add(new Tuple<int, int>(accountId, hostNameId));
                }

                reader.Close();
            }

            return hostnames;
        }

        #endregion
    }
}