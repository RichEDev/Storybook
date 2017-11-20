namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeHomeAddresses
    {
        private readonly List<cEmployeeHomeLocation> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        private readonly Cache _cache;

        public const string CacheArea = "employeeHomeAddresses";

        public EmployeeHomeAddresses(int accountID, int employeeID)
        {
            this._cache = new Cache();
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get();
        }

        public ReadOnlyCollection<cEmployeeHomeLocation> HomeLocations
        {
            get
            {
                return this._backingCollection.AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                return this._backingCollection.Count;
            }
        }

        /// <summary>
        /// Get a valid <see cref="cEmployeeHomeLocation"/> based on the locaton ID
        /// </summary>
        /// <param name="employeeLocationId">The location ID to return</param>
        /// <returns>A <see cref="cEmployeeHomeLocation"/> or null if not found</returns>
        public cEmployeeHomeLocation GetBy(int employeeLocationId)
        {
            return 
                this._backingCollection.FirstOrDefault(loc => loc.EmployeeLocationID == employeeLocationId);
        }

        /// <summary>
        /// Get a valid <see cref="cEmployeeHomeLocation"/> based on the locaton ID and the Esr location Id
        /// </summary>
        /// <param name="employeeLocationId">The location ID to return</param>
        /// <param name="esrLocationId">The Esr Location Id to check for</param>
        /// <returns>A <see cref="cEmployeeHomeLocation"/> or null if not found</returns>
        public cEmployeeHomeLocation GetBy(int employeeLocationId, long? esrLocationId)
        {
            if (!esrLocationId.HasValue || esrLocationId.Value == 0)
            {
                return this.GetBy(employeeLocationId);
            }

            return
                this._backingCollection.FirstOrDefault(loc => loc.EmployeeLocationID == employeeLocationId && loc.EsrAddressId == esrLocationId.Value);
        }

        public cEmployeeHomeLocation GetBy(DateTime date)
        {
            DateTime? oldDate = null;
            cEmployeeHomeLocation reqHomeLocation = null;
            SortedList<DateTime?, List<cEmployeeHomeLocation>> lstLocs = new SortedList<DateTime?, List<cEmployeeHomeLocation>>();
            
            //Sort the locations in ascending order of start date. If more than one location has the same start date then they are added
            //to the same location collection for the associated start time key

            foreach (cEmployeeHomeLocation location in this._backingCollection)
            {
                DateTime? startDate = location.StartDate ?? new DateTime(1899, 01, 01);

                if (!lstLocs.ContainsKey(startDate))
                {
                    List<cEmployeeHomeLocation> homeLoc = new List<cEmployeeHomeLocation> { location };
                    lstLocs.Add(startDate, homeLoc);
                }
                else
                {
                    lstLocs[startDate].Add(location);
                }
            }

            //Iterate through the collection in date order
            foreach (KeyValuePair<DateTime?, List<cEmployeeHomeLocation>> empLoc in lstLocs)
            {
                if (empLoc.Value.Count > 1)
                {
                    foreach (cEmployeeHomeLocation homeLoc in empLoc.Value)
                    {
                        if (empLoc.Key <= date && (homeLoc.EndDate == null || homeLoc.EndDate >= date))
                        {
                            reqHomeLocation = homeLoc;
                        }
                    }
                }
                else
                {
                    if (oldDate == null)
                    {
                        //empLoc.Key is the start date of the location
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date))
                        {
                            reqHomeLocation = empLoc.Value[0];
                        }
                    }
                    else
                    {
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date) && oldDate < empLoc.Key)
                        {
                            reqHomeLocation = empLoc.Value[0];
                        }
                    }
                    oldDate = empLoc.Key;
                }
            }
            return reqHomeLocation;
        }

        public List<cEmployeeHomeLocation> Get(IDBConnection connection = null)
        {
            List<cEmployeeHomeLocation> locations;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                locations = this.CacheGet();

                if (locations == null)
                {
                    locations = new List<cEmployeeHomeLocation>();


                    databaseConnection.sqlexecute.Parameters.Clear();

                    const string SQL = "select employeeHomeAddressid, addressid,StartDate, EndDate,CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, ESRAddressID from HomeAddresses where employeeid = @employeeid";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            int employeeLocationID = reader.GetInt32(reader.GetOrdinal("EmployeeHomeAddressId"));
                            int locationID = reader.GetInt32(reader.GetOrdinal("AddressId"));
                            DateTime? startDate = (reader.IsDBNull(reader.GetOrdinal("StartDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("StartDate")) : null;
                            DateTime? endDate = (reader.IsDBNull(reader.GetOrdinal("EndDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("EndDate")) : null;
                            DateTime createdOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                            int createdBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
                            DateTime? modifiedOn = (reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("ModifiedOn")) : null;
                            int? modifiedBy = (reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) == false) ? (int?)reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : null;
                            long? esrAddressId = (reader.IsDBNull(reader.GetOrdinal("ESRAddressId")) == false) ? (long?)reader.GetInt64(reader.GetOrdinal("ESRAddressId")) : null;

                            locations.Add(new cEmployeeHomeLocation(employeeLocationID, this._employeeID, locationID, startDate, endDate, createdOn, createdBy, modifiedOn, modifiedBy, esrAddressId));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    this.CacheAdd(locations);
                }
            }

            return locations;
        }


        public int Add(cEmployeeHomeLocation location, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var returnVal = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.AddWithValue("@employeeHomeAddressId", location.EmployeeLocationID);
                databaseConnection.AddWithValue("@employeeId", location.EmployeeID);
                databaseConnection.AddWithValue("@addressId", location.LocationID);
                if (location.StartDate == null)
                {
                    databaseConnection.AddWithValue("@startDate", DBNull.Value);
                }
                else
                {
                    databaseConnection.AddWithValue("@startDate", location.StartDate);
                }
                if (location.EndDate == null)
                {
                    databaseConnection.AddWithValue("@endDate", DBNull.Value);
                }
                else
                {
                    databaseConnection.AddWithValue("@endDate", location.EndDate);
                }

                  databaseConnection.AddWithValue("@date", DateTime.UtcNow);

                if (currentUser != null)
                {
                    databaseConnection.AddWithValue("@userid", currentUser.EmployeeID);
                    databaseConnection.AddWithValue("@userEmployeeId", currentUser.EmployeeID);
                           
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.AddWithValue("@userDelegateId", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.AddWithValue("@userDelegateId", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userId", location.EmployeeID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", DBNull.Value);
                }

                databaseConnection.AddReturn("@returnAddressId", SqlDbType.Int);
                databaseConnection.ExecuteProc("saveEmployeeHomeAddress");
                returnVal = databaseConnection.GetReturnValue<int>("@returnAddressId");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            if (this._backingCollection.Contains(location))
            {
                this._backingCollection.Remove(location);
            }

            this._backingCollection.Add(location);
            this.CacheDelete();
            return returnVal;
        }

        public void Remove(int employeelocationid, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeHomeAddressId", employeelocationid);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", currentUser.EmployeeID);

                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", DBNull.Value);
                }
                databaseConnection.ExecuteProc("deleteEmployeeHomeAddress");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            var currentLocation = this._backingCollection.FirstOrDefault(l => l.EmployeeLocationID == employeelocationid);

            if (currentLocation != null)
            {
                this._backingCollection.Remove(currentLocation);
            }

            this.CacheDelete();
        }

        public List<cEmployeeHomeLocation>.Enumerator GetEnumerator()
        {
            return this._backingCollection.GetEnumerator();
        }

        private void CacheAdd(List<cEmployeeHomeLocation> gridSorts)
        {
            this._cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), gridSorts);
        }

        private List<cEmployeeHomeLocation> CacheGet()
        {
            return this._cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture)) as List<cEmployeeHomeLocation>;
        }

        private void CacheDelete()
        {
            this._cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
