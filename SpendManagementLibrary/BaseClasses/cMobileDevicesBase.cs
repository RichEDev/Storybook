namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Mobile;
    using System.Data;
    using Helpers;
    using System.Linq;

    /// <summary>
    /// cMobileDevicesBase abstract class
    /// </summary>
    public abstract class cMobileDevicesBase
    {
        #region protected variables

        protected int nAccountID;
        protected string sAccountConnectionString;
        protected string sMetabaseConnectionString;
        protected Dictionary<int, MobileDeviceType> lstCachedMobileDeviceTypes;
        protected Dictionary<int, MobileDevice> lstCachedMobileDevices;
        protected string TypesCacheKey = "MobileDeviceTypes";
        protected string TypesSqlDependancySQL = "SELECT mobileDeviceTypeID, model FROM dbo.mobileDeviceTypes";
        protected string DeviceSqlDependencySQL = "SELECT createdOn, modifiedOn FROM dbo.mobileDevices";
        protected string DeviceCacheSQL = "SELECT mobileDeviceID, employeeID, deviceTypeID, deviceName, pairingKey, deviceSerialKey, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.mobileDevices";

        /// <summary>
        /// SQL Statement to trigger the cache dependency for mobileDeviceOperating System Types
        /// </summary>
        protected string DeviceTypesOsSqlDependancySql = "SELECT mobileDeviceOSTypeId, mobileInstallFrom, mobileImage FROM dbo.mobileDeviceOSTypes";

        /// <summary>
        /// The Operating System types cache key.
        /// </summary>
        protected string OsTypesCacheKey = "MobileDeviceOsTypes";

        /// <summary>
        /// The cached list of mobile device Operating System types.
        /// </summary>
        protected Dictionary<int, MobileDeviceOsType> lstCachedMobileDeviceOsTypes;
        #endregion protected variables

        #region Properties

        protected string CacheKey
        {
            get { return "MobileDevices_" + nAccountID.ToString(); }
        }

        public Dictionary<int, MobileDeviceType> MobileDeviceTypes
        {
            get { return lstCachedMobileDeviceTypes; }
        }

        /// <summary>
        /// Gets the mobile device Operating System types list.
        /// </summary>
        public Dictionary<int, MobileDeviceOsType> MobileDeviceOsTypes
        {
            get { return lstCachedMobileDeviceOsTypes; }
        }

        #endregion Properties

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="accountID">accountID for the current account</param>
        /// <param name="accountConnectionString">The connection string for the current account</param>
        /// <param name="metabaseConnectionString">The connection string for the metabase</param>
        protected cMobileDevicesBase(int accountID, string accountConnectionString, string metabaseConnectionString)
        {
            nAccountID = accountID;
            sAccountConnectionString = accountConnectionString;
            sMetabaseConnectionString = metabaseConnectionString;
        }

        /// <summary>
        /// Gets a collection of device types for caching
        /// </summary>
        /// <returns>Collection of device type ID and Definitions</returns>
        protected Dictionary<int, MobileDeviceType> GetMobileDeviceTypeListToCache()
        {
            var lstToCache = new Dictionary<int, MobileDeviceType>();
            var reqDbCon = new DBConnection(sMetabaseConnectionString);

            using (SqlDataReader reader = reqDbCon.GetStoredProcReader("dbo.mobileDeviceTypeReader"))
            {

                int mobileDeviceIdOrdinal = reader.GetOrdinal("mobileDeviceTypeID");
                int modelOrdinal = reader.GetOrdinal("model");
                int mobiledeviceOsTypeIdOrdinal = reader.GetOrdinal("mobileDeviceOSType");
                while (reader.Read())
                {
                    int mobileDeviceId = reader.GetInt32(mobileDeviceIdOrdinal);
                    string model = reader.GetString(modelOrdinal);
                    int mobiledeviceOsTypeId = !reader.IsDBNull(mobiledeviceOsTypeIdOrdinal) ? reader.GetInt32(mobiledeviceOsTypeIdOrdinal) : 0;

                    if (lstToCache.ContainsKey(mobileDeviceId) == false)
                    {
                        lstToCache.Add(mobileDeviceId, new MobileDeviceType(mobileDeviceId, model, mobiledeviceOsTypeId));
                    }
                }
                reader.Close();
            }

            reqDbCon.sqlexecute.Parameters.Clear();

            return lstToCache;
        }

        /// <summary>
        /// The get mobile device Operating System types list into cache.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        protected Dictionary<int, MobileDeviceOsType> GetMobileDeviceOsTypeListToCache()
        {
            var lstToCache = new Dictionary<int, MobileDeviceOsType>();
            var reqDbCon = new DBConnection(this.sMetabaseConnectionString);

            using (SqlDataReader reader = reqDbCon.GetStoredProcReader("dbo.mobileDeviceTypeOsReader"))
            {
                int mobileDeviceOsIdOrdinal = reader.GetOrdinal("mobileDeviceOSTypeId");
                int mobileInstallFromOrdinal = reader.GetOrdinal("mobileInstallFrom");
                int mobileImageOrdinal = reader.GetOrdinal("mobileImage");
                while (reader.Read())
                {
                    int mobileDevicesOsTypeId = reader.GetInt32(mobileDeviceOsIdOrdinal);
                    string mobileInstallFrom = reader.GetString(mobileInstallFromOrdinal);
                    string mobileImage = reader.GetString(mobileImageOrdinal);

                    if (lstToCache.ContainsKey(mobileDevicesOsTypeId) == false)
                    {
                        lstToCache.Add(mobileDevicesOsTypeId, new MobileDeviceOsType(mobileDevicesOsTypeId, mobileInstallFrom, mobileImage));
                    }
                }

                reader.Close();
            }

            reqDbCon.sqlexecute.Parameters.Clear();

            return lstToCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<int, MobileDevice> GetMobileDevicesToCache()
        {
            Dictionary<int, MobileDevice> lstToCache = new Dictionary<int, MobileDevice>();
            DBConnection db = new DBConnection(sAccountConnectionString);

            using (SqlDataReader reader = db.GetReader(DeviceCacheSQL))
            {
                #region Ordinals
                int mobileDeviceId_Ord = reader.GetOrdinal("mobileDeviceID");
                int employeeId_Ord = reader.GetOrdinal("employeeID");
                int deviceTypeId_Ord = reader.GetOrdinal("deviceTypeID");
                int deviceName_Ord = reader.GetOrdinal("deviceName");
                int pairingKey_Ord = reader.GetOrdinal("pairingKey");
                int serialKey_Ord = reader.GetOrdinal("deviceSerialKey");
                int createdOn_Ord = reader.GetOrdinal("createdOn");
                int createdBy_Ord = reader.GetOrdinal("createdBy");
                int modifiedOn_Ord = reader.GetOrdinal("modifiedOn");
                int modifiedBy_Ord = reader.GetOrdinal("modifiedBy");
                #endregion Ordinals

                while (reader.Read())
                {
                    int mobileDeviceId = reader.GetInt32(mobileDeviceId_Ord);
                    int employeeId = reader.GetInt32(employeeId_Ord);
                    int deviceTypeId = reader.GetInt32(deviceTypeId_Ord);
                    string deviceName = reader.GetString(deviceName_Ord);
                    string pairingKey = reader.GetString(pairingKey_Ord);
                    string deviceSerialKey = "";
                    if (!reader.IsDBNull(serialKey_Ord))
                    {
                        deviceSerialKey = reader.GetString(serialKey_Ord);
                    }
                    DateTime? createdOn = null;
                    if (!reader.IsDBNull(createdOn_Ord))
                    {
                        createdOn = reader.GetDateTime(createdOn_Ord);
                    }
                    int? createdBy = null;
                    if (!reader.IsDBNull(createdBy_Ord))
                    {
                        createdBy = reader.GetInt32(createdBy_Ord);
                    }
                    DateTime? modifiedOn = null;
                    if (!reader.IsDBNull(modifiedOn_Ord))
                    {
                        modifiedOn = reader.GetDateTime(modifiedOn_Ord);
                    }
                    int? modifiedBy = null;
                    if (!reader.IsDBNull(modifiedBy_Ord))
                    {
                        modifiedBy = reader.GetInt32(modifiedBy_Ord);
                    }
                    MobileDevice device = new MobileDevice(mobileDeviceId, employeeId, GetMobileDeviceTypeById(deviceTypeId), deviceName, pairingKey, deviceSerialKey);
                    lstToCache.Add(mobileDeviceId, device);
                }
                reader.Close();
            }
            return lstToCache;
        }

        /// <summary>
        /// Generates a new pairing key 
        /// </summary>
        /// <param name="employeeID">The employee this key is for</param>
        /// <returns>The pairing key -  (0#####-0####-0####} - accountID - epoc (5) - employeeID 0 padded</returns>
        public string GeneratePairingKey(int employeeID)
        {
            string newPairingKey = string.Empty;
            // Max Range 99999
            string zeroPaddedEmployeeID = String.Format("{0:0#####}", employeeID);
            int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string zeroPaddedAccountID = String.Format("{0:0####}", nAccountID);
            return zeroPaddedAccountID + "-" + epoch.ToString().Substring(5) + "-" + zeroPaddedEmployeeID;
        }

        /// <summary>
        /// Attempts to authenticate a mobile device for an employee
        /// </summary>
        /// <param name="pairingKey">Pairing key for the mobile device</param>
        /// <param name="serialKey">Serial key for the mobile device hardware</param>
        /// <param name="employeeID">Employee ID of user being authenticated</param>
        /// <returns>Returns mobile return status code</returns>
        public MobileReturnCode authenticate(string pairingKey, string serialKey, int employeeID)
        {
            DBConnection data = new DBConnection(this.sAccountConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            data.sqlexecute.Parameters.AddWithValue("@pairingKey", pairingKey);
            data.sqlexecute.Parameters.AddWithValue("@serialKey", serialKey);
            data.sqlexecute.Parameters.Add("@count", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@count"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("authenticateMobileDevice");
            MobileReturnCode count = (MobileReturnCode)data.sqlexecute.Parameters["@count"].Value;
            data.sqlexecute.Parameters.Clear();
            return count;
        }

        /// <summary>
        /// Saves a receipt for a mobile claim item
        /// </summary>
        /// <param name="mobileID">ID of the mobile device</param>
        /// <param name="receipt">Data for scanned receipt image</param>
        public void saveMobileItemReceipt(int mobileID, byte[] receipt)
        {
            DBConnection data = new DBConnection(this.sAccountConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@mobileID", mobileID);
            data.sqlexecute.Parameters.AddWithValue("@receiptData", receipt);
            data.ExecuteProc("saveMobileReceipt");
            data.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Saves a mobile device item
        /// </summary>
        /// <param name="employeeid">Employee ID of claimant</param>
        /// <param name="otherdetails">Item Other Details</param>
        /// <param name="reasonid">ID of item reason</param>
        /// <param name="total">Gross total of claim item</param>
        /// <param name="subcatid">ID of item sub-category</param>
        /// <param name="date">Date item claimed for</param>
        /// <param name="currencyid">ID of claim item currency</param>
        /// <param name="miles">Number of miles being claimed</param>
        /// <param name="quantity">Quantity being claimed</param>
        /// <param name="fromLocation">Journey start location</param>
        /// <param name="toLocation">Journey destination location</param>
        /// <param name="allowanceStartDate">Start date for allowance being claimed</param>
        /// <param name="allowanceEndDate">End date for allowance being claimed</param>
        /// <param name="dailyallowancetype">Allowance type (for daily allowances)</param>
        /// <param name="allowancedeductamount">Allowance amount to deduct (for daily allowances)</param>
        /// <param name="itemNotes">Additional information provided from the mobile device about the expense item</param>
        /// <param name="mobileDeviceTypeId">Mobile Device Type Id expense added by</param>
        /// <param name="mobileDeviceID">The ID of the mobile device this expense came from</param>
        /// <param name="mobileExpenseID">The mobile expense ID local to the device from which this expense came from</param>
        /// <returns>ID of claim item added</returns>
        public int saveMobileItem(int employeeid, string otherdetails, int? reasonid, decimal total, int subcatid, DateTime date, int? currencyid, decimal miles, double quantity, string fromLocation, string toLocation, DateTime? allowanceStartDate, DateTime? allowanceEndDate, int? dailyallowancetype, decimal allowancedeductamount, string itemNotes, int mobileDeviceTypeId, int mobileDeviceID, int mobileExpenseID)
        {
            return saveMobileItem(employeeid, otherdetails, reasonid, total, subcatid, date, currencyid, miles, quantity, fromLocation, toLocation, allowanceStartDate, allowanceEndDate, dailyallowancetype, allowancedeductamount, itemNotes, mobileDeviceTypeId, null, mobileDeviceID: mobileDeviceID, mobileExpenseID: mobileExpenseID);
        }

        /// <summary>
        /// Saves a mobile device item with mileage JSON
        /// </summary>
        /// <param name="employeeid">Employee ID of claimant</param>
        /// <param name="otherdetails">Item Other Details</param>
        /// <param name="reasonid">ID of item reason</param>
        /// <param name="total">Gross total of claim item</param>
        /// <param name="subcatid">ID of item sub-category</param>
        /// <param name="date">Date item claimed for</param>
        /// <param name="currencyid">ID of claim item currency</param>
        /// <param name="miles">Number of miles being claimed</param>
        /// <param name="quantity">Quantity being claimed</param>
        /// <param name="fromLocation">Journey start location</param>
        /// <param name="toLocation">Journey destination location</param>
        /// <param name="allowanceStartDate">Start date for allowance being claimed</param>
        /// <param name="allowanceEndDate">End date for allowance being claimed</param>
        /// <param name="dailyallowancetype">Allowance type (for daily allowances)</param>
        /// <param name="allowancedeductamount">Allowance amount to deduct (for daily allowances)</param>
        /// <param name="itemNotes">Additional information provided from the mobile device about the expense item</param>
        /// <param name="mobileDeviceTypeId">Mobile Device Type Id expense added by</param>
        /// <param name="quickMileageItem">Whether this item was added as a quick mileage item on mobile (and thus has no subcatid)</param>
        /// <param name="mobileDeviceID">The ID of the mobile device this expense came from</param>
        /// <param name="mobileExpenseID">The mobile expense ID local to the device from which this expense came from</param>
        /// <returns>ID of claim item added</returns>
        public int saveMobileItem(int employeeid, string otherdetails, int? reasonid, decimal total, int subcatid, DateTime date, int? currencyid, decimal miles, double quantity, string fromLocation, string toLocation, DateTime? allowanceStartDate, DateTime? allowanceEndDate, int? dailyallowancetype, decimal allowancedeductamount, string itemNotes, int mobileDeviceTypeId, string mileageJson, int mobileDeviceID, int mobileExpenseID)
        {
            var data = new DBConnection(this.sAccountConnectionString);

            data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            if (!string.IsNullOrEmpty(otherdetails))
            {
                data.sqlexecute.Parameters.AddWithValue("@otherdetails", otherdetails);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@otherdetails", DBNull.Value);
            }
            if (reasonid != null && reasonid > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@reasonid", reasonid);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@reasonid", DBNull.Value);
            }
            data.sqlexecute.Parameters.AddWithValue("@total", total);
            if (subcatid > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@subcatid", DBNull.Value);
            }

            data.sqlexecute.Parameters.AddWithValue("@date", date);
            if (currencyid != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
            }
            data.sqlexecute.Parameters.AddWithValue("@miles", miles);
            data.sqlexecute.Parameters.AddWithValue("@quantity", quantity);
            if (!string.IsNullOrEmpty(fromLocation))
            {
                data.sqlexecute.Parameters.AddWithValue("@fromLocation", fromLocation);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@fromLocation", DBNull.Value);
            }
            if (!string.IsNullOrEmpty(toLocation))
            {
                data.sqlexecute.Parameters.AddWithValue("@toLocation", toLocation);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@toLocation", DBNull.Value);
            }
            if (allowanceStartDate.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@allowancestartdate", allowanceStartDate);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@allowancestartdate", DBNull.Value);
            }
            if (allowanceEndDate.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@allowanceenddate", allowanceEndDate.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@allowanceenddate", DBNull.Value);
            }
            if (!string.IsNullOrEmpty(itemNotes))
            {
                data.sqlexecute.Parameters.AddWithValue("@itemNotes", itemNotes);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@itemNotes", DBNull.Value);
            }
            if (!dailyallowancetype.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@allowancetype", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@allowancetype", dailyallowancetype.Value);
            }

            if (!string.IsNullOrEmpty(mileageJson))
            {
                data.sqlexecute.Parameters.AddWithValue("@mileageJson", mileageJson);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@mileageJson", DBNull.Value);
            }

            data.sqlexecute.Parameters.AddWithValue("@allowancedeductamount", allowancedeductamount);
            data.sqlexecute.Parameters.AddWithValue("@deviceTypeId", mobileDeviceTypeId);
            data.sqlexecute.Parameters.AddWithValue("@mobileDeviceID", mobileDeviceID);
            data.sqlexecute.Parameters.AddWithValue("@mobileExpenseID", mobileExpenseID);


            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveMobileItem");
            int mobileid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();
            return mobileid;
        }

        /// <summary>
        /// Gets the definition of a mobile device type by its id
        /// </summary>
        /// <param name="deviceTypeId">Device Id to retrieve definition for</param>
        /// <returns>Device type defintion class</returns>
        public MobileDeviceType GetMobileDeviceTypeById(int deviceTypeId)
        {
            if (!lstCachedMobileDeviceTypes.ContainsKey(deviceTypeId))
            {
                return null;
            }

            return lstCachedMobileDeviceTypes[deviceTypeId];
        }

        /// <summary>
        /// Saves a mobile journey
        /// </summary>
        /// <param name="employeeid">
        /// Employee ID of the mobile user
        /// </param>
        /// <param name="subcatid">
        /// The subcat identifier.
        /// </param>
        /// <param name="journeyJson">
        /// The JSON representation of the journey steps
        /// </param>
        /// <param name="deviceTypeId">
        /// The device Type Id.
        /// </param>
        /// <param name="date">
        /// The date of the journey
        /// </param>
        /// <param name="startTime">
        /// The start time of the journey.
        /// </param>
        /// <param name="endTime">
        /// The end time of the journey.
        /// </param>
        /// <param name="active">Whether the journey is active or not</param>
        /// <returns>
        /// ID of the journey added
        /// </returns>
        public int SaveMobileJourney(int employeeid, int subcatid, string journeyJson, int deviceTypeId, DateTime date, DateTime startTime, DateTime endTime, bool active)
        {
            int mobileId = -1;

            if (active)
            {
                this.DeleteEmployeeActiveJourneys(employeeid);
            }

            using (var data = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
                data.sqlexecute.Parameters.AddWithValue("@subcatID", subcatid);
                data.sqlexecute.Parameters.AddWithValue("@mileageJson", journeyJson);
                data.sqlexecute.Parameters.AddWithValue("@deviceTypeID", deviceTypeId);
                data.sqlexecute.Parameters.AddWithValue("@journeyDate", date);
                data.sqlexecute.Parameters.AddWithValue("@startTime", startTime);
                data.sqlexecute.Parameters.AddWithValue("@endTime", endTime);
                data.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);
                data.sqlexecute.Parameters.AddWithValue("@active", active);
                data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
                data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;

                data.ExecuteProc("SaveMobileJourney");

                mobileId = (int)data.sqlexecute.Parameters["@identity"].Value;
                data.sqlexecute.Parameters.Clear();

            }

            return mobileId;
        }

        /// <summary>
        /// Update a mobile journey steps
        /// </summary>   
        /// <param name="journeyId">
        /// journey ID of the journey
        /// </param>
        /// <param name="journeyJson">
        /// The JSON representation of the journey steps
        /// </param>
        /// <param name="active">Whether the journey is active</param>
        public void UpdateMobileJourney(int journeyId, string journeyJson, bool active)
        {     
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@journeyId", journeyId);
                data.sqlexecute.Parameters.AddWithValue("@mileageJson", journeyJson);
                data.sqlexecute.Parameters.AddWithValue("@active", active);
    
                data.ExecuteProc("UpdateMobileJourney");
                data.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Update a mobile journey steps
        /// </summary>   
        /// <param name="employeeId">
        /// The employeeId
        /// </param>
        /// <returns>A list of active <see cref="MobileJourney">MobileJourney</see></returns>
        public List<MobileJourney> GetEmployeeActiveJourneys(int employeeId)
        {
            var journeys =  new List<MobileJourney>();

            using (var data = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
              
            using (var reader = data.GetReader("GetEmployeeActiveJourneys", CommandType.StoredProcedure))
            {
                int journeyIdOrd = reader.GetOrdinal("JourneyId");
                int subcatIdOrd = reader.GetOrdinal("SubcatId");
                int journeyJsonOrd = reader.GetOrdinal("JourneyJSON");
                int createdByOrd = reader.GetOrdinal("CreatedBy");         
                int journeyDateOrd = reader.GetOrdinal("JourneyDate");
                int journeyEndTimeOrd = reader.GetOrdinal("JourneyEndTime");
                int journeyStartTimeOrd = reader.GetOrdinal("JourneyStartTime");
           
                while (reader.Read())
                {
                    int journeyId = reader.GetInt32(journeyIdOrd);
                    int subcatId =  reader.GetInt32(subcatIdOrd);               
                    DateTime journeyDate = reader.GetDateTime(journeyDateOrd);
                    string journeyJson = reader.GetString(journeyJsonOrd);
                    int createdBy = reader.GetInt32(createdByOrd);             
                    DateTime journeyEndTime =  reader.GetDateTime(journeyEndTimeOrd);
                    DateTime journeyStartTime =  reader.GetDateTime(journeyStartTimeOrd);

                    var journey = new MobileJourney
                    {
                        JourneyId = journeyId,                      
                        SubcatId = subcatId,
                        CreatedBy = createdBy,            
                        JourneyJson = journeyJson,
                        JourneyDateTime = journeyDate,
                        JourneyStartTime = journeyStartTime,
                        JourneyEndTime = journeyEndTime,
                        Active = true
                                         
                    };

                    journeys.Add(journey);
                }
            }
                data.sqlexecute.Parameters.Clear();
            }

            return journeys;
        }

        /// <summary>
        /// Gets the number of active journeys for the supplied employeeId
        /// </summary>   
        /// <param name="employeeId">
        /// The employeeId
        /// </param>
        /// <returns>A count of active journeys</returns>
        public int EmployeeActiveJourneysCount(int employeeId)
        {
            int count = 0;

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                count = databaseConnection.ExecuteScalar<int>("dbo.GetEmployeeActiveJourneysCount", CommandType.StoredProcedure);
            }

            return count;
        }

        /// <summary>
                /// Checks if the journey belongs to the employee
                /// </summary>
                /// <param name="journeyId">The journeyId</param>
                /// <param name="employeeId">The employeeId</param>
                /// <returns>The outcome of the delete journey action</returns>
            public bool DoesJourneyBelongToEmployee(int journeyId, int employeeId)
        {
            List<MobileJourney> mobileJourneys = this.GetEmployeeActiveJourneys(employeeId);
            MobileJourney mobileJourney = mobileJourneys.FirstOrDefault(x => x.JourneyId == journeyId);
          
            return mobileJourney != null;
        }

        /// <summary>
        /// Deletes a mobile journey from the database
        /// </summary>
        /// <param name="journeyId">The journeyId</param>
        /// <returns>The number of rows deleted by the action</returns>
        public bool DeleteJourneyFromDB(int journeyId)
        {
            int affectedRows = 0;

            using (var data = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@mobileJourneyID", journeyId);
                data.sqlexecute.Parameters.Add("@affectedRows", SqlDbType.Int);
                data.sqlexecute.Parameters["@affectedRows"].Direction = ParameterDirection.ReturnValue;
                data.ExecuteProc("dbo.DeleteMobileJourney");

                affectedRows = (int)data.sqlexecute.Parameters["@affectedRows"].Value;
            }
        
            return affectedRows > 0;
        }

        /// <summary>
        //Deletes all active journeys for the employee to ensure the employee only ever has one active journey at a time.
        ///  <param name="employeeId">The employeeId</param>
        /// </summary>
        private void DeleteEmployeeActiveJourneys(int employeeId)
        {           
            var journeys = this.GetEmployeeActiveJourneys(employeeId);
            {
                foreach (var journey in journeys)  
                {
                    this.DeleteJourneyFromDB(journey.JourneyId);
                }
            }
        }
    }

    /// <summary>
    /// Mobile API return status codes
    /// </summary>
    public enum MobileReturnCode
    {
        /// <summary>
        /// Pairing Key has already been used
        /// </summary>
        PairingKeyInUse = -1,

        /// <summary>
        /// Employee account is archived
        /// </summary>
        EmployeeArchived = -2,

        /// <summary>
        /// Employee account has not been activated
        /// </summary>
        EmployeeNotActivated = -3,

        /// <summary>
        /// Employee is not known on the account
        /// </summary>
        EmployeeUnknown = -4,

        /// <summary>
        /// Reason used on expense item is invalid
        /// </summary>
        ReasonIsInvalid = -5,

        /// <summary>
        /// Currency used on expense item is either archived or does not exist
        /// </summary>
        CurrencyIsInvalid = -6,

        /// <summary>
        /// Subcat used on expense item is invalid
        /// </summary>
        SubCatIsInvalid = -7,

        /// <summary>
        /// Indicates that the pairing key provided is an invalid format
        /// </summary>
        PairingKeyFormatInvalid = -8,

        /// <summary>
        /// Indicates that the account mobile device associated to is archived
        /// </summary>
        AccountArchived = -9,

        /// <summary>
        /// Pairing cannot be found on the current account
        /// </summary>
        PairingKeyNotFound = -10,

        /// <summary>
        /// Pairing key found is not associated with serial key provided
        /// </summary>
        PairingKeySerialKeyMismatch = -11,

        /// <summary>
        /// Pairing key decoded to an invalid account ID
        /// </summary>
        AccountInvalid = -12,

        /// <summary>
        /// Global Option for Use Mobile Devices on expenses account is disabled
        /// </summary>
        MobileDevicesDisabled = -13,

        /// <summary>
        /// Indicates that the employee does not have access role permission for Mobile Devices
        /// </summary>
        EmployeeHasInsufficientPermissions = -14,

        /// <summary>
        /// Indicates that the API type requested is unknown
        /// </summary>
        InvalidApiVersionType = -15,

        /// <summary>
        /// Indicates that the Allowance type is invalid
        /// </summary>
        AllowanceIsInvalid = -16,

        /// <summary>
        /// Action successful
        /// </summary>
        Success = 0,

        /// <summary>
        /// Authentication failed
        /// </summary>
        AuthenticationFailed,


        /// <summary>
        /// If the claim cannot progress throuh the signoff procedure this code will be returned.
        /// </summary>
        ApproveClaimFailed = -17,

        /// <summary>
        /// If the claim cannot progress through to payment this code will be returned
        /// </summary>
        AllocateForPaymentFailed = -18,

        /// <summary>
        /// If journeys passed are invalid, this code will be returned
        /// </summary>
        InvalidJourneys = -19,

        /// <summary>
        /// ErrorCode when DMI used is no longer available or is not allowed to file expenses against
        /// </summary>
        InvalidDefaultMilageItem= -20
        
    }
}
