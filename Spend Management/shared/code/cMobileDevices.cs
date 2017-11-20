namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using SpendManagementLibrary;
    using System.Configuration;
    using System.Web.Caching;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.Mobile;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Enumerators.Expedite;

    /// <summary>
    /// cMobileDevices - implements caching for the base class cMobileDevicesBase
    /// </summary>
    public class cMobileDevices : cMobileDevicesBase
    {
        private System.Web.Caching.Cache WebCache = System.Web.HttpRuntime.Cache;

        /// <summary>
        /// Default constructor for cMobileDevices
        /// </summary>
        /// <param name="accountID">AccountID for the current account</param>
        public cMobileDevices(int accountID)
            : base(accountID, cAccounts.getConnectionString(accountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString)
        {
            this.CacheMobileDeviceTypesList();
            this.CacheMobileDeviceOperatingSystemTypesList();
            this.CacheMobileDevices();
        }

        /// <summary>
        /// Constructor for use by unit testing classes
        /// </summary>
        /// <param name="currentUser">ICurrentUser interface</param>
        /// <param name="metabaseConnectionString">Connectionstring to the unit test METABASE</param>
        public cMobileDevices(ICurrentUser currentUser, string metabaseConnectionString)
            : base(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID), metabaseConnectionString)
        {
            this.CacheMobileDeviceTypesList();
            this.CacheMobileDeviceOperatingSystemTypesList();
            this.CacheMobileDevices();
        }

        #region Public Methods

        /// <summary>
        /// Caches the Mobile Device Types from the metabase
        /// </summary>
        public void CacheMobileDeviceTypesList()
        {
            Dictionary<int, MobileDeviceType> deviceTypes = WebCache[TypesCacheKey] as Dictionary<int, MobileDeviceType>;
            if (deviceTypes == null)
            {
                lstCachedMobileDeviceTypes = GetMobileDeviceTypeListToCache();
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    var reqDbCon = new DBConnection(sMetabaseConnectionString);
                    var dep = reqDbCon.CreateSQLCacheDependency(TypesSqlDependancySQL, new SortedList<string, object>());
                    WebCache.Insert(TypesCacheKey, lstCachedMobileDeviceTypes, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
                }
            }
            else
            {
                lstCachedMobileDeviceTypes = deviceTypes;
            }
        }

        /// <summary>
        /// Caches the Mobile Device Operating System Types from the METABASE
        /// </summary>
        public void CacheMobileDeviceOperatingSystemTypesList()
        {
            Dictionary<int, MobileDeviceOsType> deviceOsTypes = WebCache[OsTypesCacheKey] as Dictionary<int, MobileDeviceOsType>;
            if (deviceOsTypes == null)
            {
                this.lstCachedMobileDeviceOsTypes = this.GetMobileDeviceOsTypeListToCache();
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    var reqDbCon = new DBConnection(sMetabaseConnectionString);
                    SqlCacheDependency dep = reqDbCon.CreateSQLCacheDependency(DeviceTypesOsSqlDependancySql, new SortedList<string, object>());
                    this.WebCache.Insert(this.OsTypesCacheKey, this.lstCachedMobileDeviceOsTypes, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
                }
            }
            else
            {
                this.lstCachedMobileDeviceOsTypes = deviceOsTypes;
            }
        }

        /// <summary>
        /// Retrieves a mobile expense item by its ID
        /// </summary>
        /// <param name="id">ID of the expense item to retrieve</param>
        /// <returns>Expense item object class</returns>
        public SpendManagementLibrary.Mobile.ExpenseItem getMobileItemByID(int id)
        {
            DBConnection data = new DBConnection(sAccountConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@mobileID", id);
            SpendManagementLibrary.Mobile.ExpenseItem item = null;
            using (SqlDataReader reader = data.GetReader("select mobileExpenseItems.mobileid, employeeid, otherdetails, reasonid, total, subcatid, date, currencyid, miles, quantity, fromLocation, toLocation, allowancestartdate, allowanceenddate, mobileExpenseItemReceipts.mobileID as HasReceipt, deviceTypeId, itemNotes, allowanceid, allowancededuct,mileageJourneySteps from mobileExpenseItems left join mobileExpenseItemReceipts on mobileExpenseItemReceipts.mobileID = mobileExpenseItems.mobileID where mobileExpenseItems.mobileID = @mobileID"))
            {
                while (reader.Read())
                {
                    item = new SpendManagementLibrary.Mobile.ExpenseItem { MobileID = id };
                    if (reader.IsDBNull(reader.GetOrdinal("otherdetails")))
                    {
                        item.OtherDetails = string.Empty;
                    }
                    else
                    {
                        item.OtherDetails = reader.GetString(reader.GetOrdinal("otherdetails"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("reasonid")))
                    {
                        item.ReasonID = null;
                    }
                    else
                    {
                        item.ReasonID = reader.GetInt32(reader.GetOrdinal("reasonid"));
                    }
                    item.Total = reader.GetDecimal(reader.GetOrdinal("total"));
                    item.SubcatID = reader.GetInt32(reader.GetOrdinal("subcatid"));
                    item.dtDate = reader.GetDateTime(reader.GetOrdinal("date"));
                    item.CreatedBy = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    if (reader.IsDBNull(reader.GetOrdinal("currencyid")))
                    {
                        item.CurrencyID = null;
                    }
                    else
                    {
                        item.CurrencyID = reader.GetInt32(reader.GetOrdinal("currencyid"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("miles")))
                    {
                        item.Miles = 0;
                    }
                    else
                    {
                        item.Miles = reader.GetDecimal(reader.GetOrdinal("miles"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("quantity")))
                    {
                        item.Quantity = 0;
                    }
                    else
                    {
                        item.Quantity = reader.GetDouble(reader.GetOrdinal("quantity"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("fromLocation")))
                    {
                        item.FromLocation = string.Empty;
                    }
                    else
                    {
                        item.FromLocation = reader.GetString(reader.GetOrdinal("fromLocation"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("toLocation")))
                    {
                        item.ToLocation = string.Empty;
                    }
                    else
                    {
                        item.ToLocation = reader.GetString(reader.GetOrdinal("toLocation"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("allowanceStartDate")))
                    {
                        item.dtAllowanceStartDate = null;
                    }
                    else
                    {
                        item.dtAllowanceStartDate = reader.GetDateTime(reader.GetOrdinal("allowanceStartDate"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("allowanceEndDate")))
                    {
                        item.dtAllowanceEndDate = null;
                    }
                    else
                    {
                        item.dtAllowanceEndDate = reader.GetDateTime(reader.GetOrdinal("allowanceEndDate"));
                    }
                    item.HasReceipt = !reader.IsDBNull(reader.GetOrdinal("HasReceipt"));
                    if (!reader.IsDBNull(reader.GetOrdinal("deviceTypeId")))
                    {
                        item.MobileDeviceTypeId = reader.GetInt32(reader.GetOrdinal("deviceTypeId"));
                    }
                    else
                    {
                        item.MobileDeviceTypeId = null;
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("itemNotes")))
                    {
                        item.ItemNotes = reader.GetString(reader.GetOrdinal("itemNotes"));
                    }
                    else
                    {
                        item.ItemNotes = null;
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("allowanceid")))
                    {
                        item.AllowanceTypeID = reader.GetInt32(reader.GetOrdinal("allowanceid"));
                    }
                    else
                    {
                        item.AllowanceTypeID = null;
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("allowancededuct")))
                    {
                        item.AllowanceDeductAmount = reader.GetDecimal(reader.GetOrdinal("allowancededuct"));
                    }
                    else
                    {
                        item.AllowanceDeductAmount = 0;
                    }

                    item.JourneySteps = !reader.IsDBNull(reader.GetOrdinal("mileageJourneySteps")) ? reader.GetString(reader.GetOrdinal("mileageJourneySteps")) : string.Empty;

                }
                reader.Close();
            }
            return item;
        }

        /// <summary>
        /// Gets a mobile journey by its ID
        /// </summary>
        /// <param name="journeyID">
        /// The ID of the journey to retrieve
        /// </param>
        /// <param name="connection">An alternative database connection to use</param>
        /// <returns>
        /// A <see cref="MobileJourney"/>
        /// </returns>
        public MobileJourney GetMobileJourney(int journeyID, IDBConnection connection = null)
        {
            MobileJourney journey = null;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(this.sAccountConnectionString))
            {
                const string Sql = "SELECT JourneyJson,JourneyDate,CreatedBy,SubcatID FROM mobileJourneys WHERE JourneyID = @JourneyID";

                databaseConnection.AddWithValue("@JourneyID", journeyID);

                using (IDataReader reader = databaseConnection.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        journey = new MobileJourney
                        {
                            JourneyId = journeyID,
                            SubcatId =
                                reader.IsDBNull(reader.GetOrdinal("SubcatID"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("SubcatID")),
                            JourneyDateTime = reader.GetDateTime(reader.GetOrdinal("JourneyDate")),
                            JourneyJson =
                                reader.IsDBNull(reader.GetOrdinal("JourneyJson"))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal("JourneyJson")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"))
                        };
                    }

                    reader.Close();
                    databaseConnection.sqlexecute.Parameters.Clear();
                }
            }


            return journey;
        }

        /// <summary>
        /// Saves a receipt for a mobile expense item against a claim and expense item
        /// </summary>
        /// <param name="mobileID">ID of the mobile device</param>
        /// <param name="expenseID">ID of the expense item</param>
        /// <param name="claimID">Claim ID to associate to</param>
        public void saveReceiptFromMobile(int mobileID, int expenseID, int claimID)
        {
            var connection = new DBConnection(this.sAccountConnectionString);
            byte[] receipt = null;
            int ndx = 0;
            connection.sqlexecute.Parameters.AddWithValue("@mobileID", mobileID);
            using (SqlDataReader reader = connection.GetReader("select receiptData from mobileExpenseItemReceipts where mobileID = @mobileID"))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0)) continue;

                    long size = reader.GetBytes(ndx, 0, null, 0, 0);  //get the length of data
                    receipt = new byte[size];

                    const int bufferSize = 1024;
                    long bytesRead = 0;
                    int curPos = 0;

                    while (bytesRead < size)
                    {
                        bytesRead += reader.GetBytes(ndx, curPos, receipt, curPos, bufferSize);
                        curPos += bufferSize;
                    }
                }
                reader.Close();
            }

            if (receipt == null || receipt.Length <= 0) return;

            CurrentUser user = cMisc.GetCurrentUser();

            var data = new SpendManagementLibrary.Expedite.Receipts(user.AccountID, user.EmployeeID);
            var dbreceipt = new Receipt
            {
                CreationMethod = ReceiptCreationMethod.UploadedByMobile,
                Extension = ".png",
                ModifiedBy = user.EmployeeID,
                ModifiedOn = DateTime.UtcNow
            };
            dbreceipt = data.AddReceipt(dbreceipt, receipt);
            dbreceipt = data.LinkToClaimLine(dbreceipt.ReceiptId, expenseID);
        }

        /// <summary>
        /// Adds each MobileDeviceType to a DropDownList
        /// </summary>
        /// <param name="ddl">Reference to drop down list to populate</param>
        /// <param name="includeNoneOption">Specify TRUE to include [None] option at top of the list</param>
        /// <param name="selectedMobileDeviceTypeID">ID of mobile device to pre-select in list. NULL for no pre-selection.</param>
        public void CreateDropDown(ref DropDownList ddl, bool includeNoneOption, int? selectedMobileDeviceTypeID = null)
        {
            var lstSorted = new SortedList<string, int>();

            foreach (MobileDeviceType mdt in lstCachedMobileDeviceTypes.Values)
            {
                lstSorted.Add(mdt.DeviceTypeDescription, mdt.DeviceTypeId);
            }

            if (includeNoneOption)
                ddl.Items.Add(new ListItem("[None]", "0"));

            foreach (KeyValuePair<string, int> kvp in lstSorted)
            {
                ddl.Items.Add(new ListItem(kvp.Key, kvp.Value.ToString()));
                if (selectedMobileDeviceTypeID.HasValue && kvp.Value == selectedMobileDeviceTypeID.Value)
                {
                    ddl.Items[ddl.Items.Count - 1].Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets a mobile device by its pairing key
        /// </summary>
        /// <param name="pairingKey">Pairing to match device for</param>
        /// <returns>MobileDevice class object</returns>
        public MobileDevice GetDeviceByPairingKey(string pairingKey)
        {
            MobileDevice device = (from x in lstCachedMobileDevices.Values
                                   where x.PairingKey == pairingKey
                                   select x).FirstOrDefault();

            return device;
        }

        /// <summary>
        /// Gets a mobile device by its ID
        /// </summary>
        /// <param name="mobileDeviceID">Mobile device ID to retrieve</param>
        /// <returns>MobileDevice class object</returns>
        public MobileDevice GetMobileDeviceById(int mobileDeviceID)
        {
            MobileDevice device = (from x in lstCachedMobileDevices.Values
                                   where x.MobileDeviceID == mobileDeviceID
                                   select x).FirstOrDefault();

            return device;
        }

        /// <summary>
        /// Gets all mobile devices in the system.
        /// note: Only used by the API currently.
        /// </summary>
        /// <returns>MobileDevices</returns>
        public List<MobileDevice> GetAllDevicesAsList()
        {
            return lstCachedMobileDevices.Values.ToList();
        }

        /// <summary>
        /// Gets a list of mobile devices for a particular employee
        /// </summary>
        /// <param name="employeeID">Employee ID to retrieve devices for</param>
        /// <returns>Dictionary collection of MobileDevice class objects</returns>
        public Dictionary<int, MobileDevice> GetMobileDevicesByEmployeeId(int employeeID)
        {
            Dictionary<int, MobileDevice> devices = (from x in lstCachedMobileDevices.Values
                                                     where x.EmployeeID == employeeID
                                                     select x).ToDictionary(x => x.MobileDeviceID);
            return devices;
        }

        /// <summary>
        /// Removes an item from the unallocated mobile expense items table
        /// </summary>
        /// <param name="mobileItemId">
        /// Mobile Expense Item ID to delete
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeleteMobileItemByID(object mobileItemId)
        {
            try
            {
                var data = new DBConnection(sAccountConnectionString);
                data.sqlexecute.Parameters.AddWithValue("@mobileID", mobileItemId);

                const string SQL = "delete from mobileExpenseItems where mobileID = @mobileID";
                data.ExecuteSQL(SQL);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes an item from the unallocated mobile journeys table
        /// </summary>
        /// <param name="mobileJourneyId">
        /// Mobile Journey ID to delete
        /// </param>
        /// <returns>
        /// True on a successful delete, false otherwise.
        /// </returns>
        public bool DeleteMobileJourney(object mobileJourneyId)
        {      
            return base.DeleteJourneyFromDB((int) mobileJourneyId);        
        }

        /// <summary>
        /// Delete a specific mobile device from this account
        /// </summary>
        /// <param name="mobileDeviceID">The mobile device to delete</param>
        /// <param name="employeeID">Employee ID of user performing deletion</param>
        /// <param name="delegateID">Employee ID of user acting as delegate for employee</param>
        /// <returns>TRUE deletion successful, otherwise FALSE</returns>
        public bool DeleteMobileDevice(int mobileDeviceID, int employeeID, int? delegateID)
        {
            int affectedRows = 0;
            DBConnection reqDbCon = new DBConnection(sAccountConnectionString);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@mobileDeviceID", mobileDeviceID);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            if (delegateID.HasValue)
            {
                reqDbCon.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID.Value);
            }
            else
            {
                reqDbCon.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            reqDbCon.sqlexecute.Parameters.Add("@affectedRows", SqlDbType.Int);
            reqDbCon.sqlexecute.Parameters["@affectedRows"].Direction = ParameterDirection.ReturnValue;
            reqDbCon.ExecuteProc("dbo.deleteMobileDevice");
            affectedRows = (int)reqDbCon.sqlexecute.Parameters["@affectedRows"].Value;
            reqDbCon.sqlexecute.Parameters.Clear();

            RefreshCache();

            return affectedRows > 0;
        }

        /// <summary>
        /// Save changes to an existing or new mobile device
        /// </summary>
        /// <param name="reqMobileDevice">Mobile device class object</param>
        /// <param name="reqEmployeeId">Employee performing the save</param>
        /// <returns></returns>
        public int SaveMobileDevice(MobileDevice reqMobileDevice, int reqEmployeeId)
        {
            DBConnection reqDbCon = new DBConnection(sAccountConnectionString);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@mobileDeviceID", reqMobileDevice.MobileDeviceID);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@employeeID", reqMobileDevice.EmployeeID);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@deviceTypeID", reqMobileDevice.DeviceType.DeviceTypeId);
            reqDbCon.AddWithValue("@deviceName", reqMobileDevice.DeviceName, 100);

            if (string.IsNullOrWhiteSpace(reqMobileDevice.PairingKey))
            {
                reqMobileDevice.PairingKey = string.Empty;
            }

            reqDbCon.AddWithValue("@pairingKey", reqMobileDevice.PairingKey, 30);
            if (string.IsNullOrWhiteSpace(reqMobileDevice.SerialKey))
            {
                reqMobileDevice.SerialKey = string.Empty;
            }
            reqDbCon.AddWithValue("@deviceSerialKey", reqMobileDevice.SerialKey, 200);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@requestorEmployeeId", reqEmployeeId);

            reqDbCon.sqlexecute.Parameters.Add("@newMobileDeviceID", SqlDbType.Int);
            reqDbCon.sqlexecute.Parameters["@newMobileDeviceID"].Direction = ParameterDirection.ReturnValue;
            reqDbCon.ExecuteProc("dbo.saveMobileDevice");

            int mobileDeviceID = (int)reqDbCon.sqlexecute.Parameters["@newMobileDeviceID"].Value;

            reqDbCon.sqlexecute.Parameters.Clear();

            RefreshCache();

            return mobileDeviceID;
        }

        /// <summary>
        /// Activates a mobile device in the system using its pairing key
        /// </summary>
        /// <param name="pairingKey">The pairing key -  (0#####-0####-0####} - accountID - epoc (5) - employeeID 0 padded</param>
        /// <param name="serialKey">Serial key of the mobile hardware device</param>
        /// <returns>Mobile device status return code</returns>
        public MobileReturnCode PairMobileDevice(PairingKey pairingKey, string serialKey)
        {
            DBConnection dbCon = new DBConnection(this.sAccountConnectionString);
            dbCon.AddWithValue("@pairingKey", pairingKey.Pairingkey, pairingKey.Pairingkey.Length);
            dbCon.AddWithValue("@serialKey", serialKey, serialKey.Length);
            dbCon.sqlexecute.Parameters.AddWithValue("@employeeID", pairingKey.EmployeeID);
            dbCon.sqlexecute.Parameters.Add("@retVal", SqlDbType.Int);
            dbCon.sqlexecute.Parameters["@retVal"].Direction = ParameterDirection.ReturnValue;

            //const string strSQL = "UPDATE mobileDevices SET deviceSerialKey=@serialKey WHERE employeeID=@employeeID AND pairingKey=@pairingKey";

            dbCon.ExecuteProc("pairMobileDevice"); //dbCon.ExecuteSQLWithAffectedRows(strSQL) > 0;
            MobileReturnCode retVal = (MobileReturnCode)((int)dbCon.sqlexecute.Parameters["@retVal"].Value);
            dbCon.sqlexecute.Parameters.Clear();

            RefreshCache();

            return retVal;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Informs whether mobile item has a receipt attachment
        /// </summary>
        /// <param name="accountID">Expense Customer Account ID</param>
        /// <param name="mobileItemID">Mobile Expense Item to check</param>
        /// <returns>TRUE if a receipt is saved, otherwise FALSE</returns>
        public static bool MobileItemHasReceipt(int accountID, int mobileItemID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountID));
            const string sql = "select count(mobileID) from dbo.mobileExpenseItemReceipts where mobileID = @mobileID";
            db.sqlexecute.Parameters.AddWithValue("@mobileID", mobileItemID);

            return db.getcount(sql) > 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Caches the mobile devices for the current account
        /// </summary>
        private void CacheMobileDevices()
        {
            Dictionary<int, MobileDevice> mobileDevices = WebCache[CacheKey] as Dictionary<int, MobileDevice>;
            if (mobileDevices == null)
            {
                this.lstCachedMobileDevices = this.GetMobileDevicesToCache();
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    var reqDbCon = new DBConnection(this.sAccountConnectionString);
                    SqlCacheDependency dep = reqDbCon.CreateSQLCacheDependency(this.DeviceSqlDependencySQL + " WHERE " + this.nAccountID.ToString() + " = " + this.nAccountID.ToString(), new SortedList<string, object>());
                    var metaDbCon = new DBConnection(this.sMetabaseConnectionString);
                    SqlCacheDependency typeDep = metaDbCon.CreateSQLCacheDependency(this.TypesSqlDependancySQL, new SortedList<string, object>());
                    var aggDep = new AggregateCacheDependency();
                    aggDep.Add(new CacheDependency[] { dep, typeDep });
                    this.WebCache.Insert(this.CacheKey, this.lstCachedMobileDevices, aggDep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
                }
            }
            else
            {
                this.lstCachedMobileDevices = mobileDevices;
            }
        }

        /// <summary>
        /// Forcibly removes devices from cache (required by unit tests to avoid dependency latency)
        /// </summary>
        private void RefreshCache()
        {
            this.WebCache.Remove(this.CacheKey);

            this.CacheMobileDeviceTypesList();
            this.CacheMobileDeviceOperatingSystemTypesList();
            this.CacheMobileDevices();
        }
        #endregion
    }
}