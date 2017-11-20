namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeWorkAddresses
    {
        private readonly IDictionary<int, cEmployeeWorkLocation> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeWorkAddresses";

        public EmployeeWorkAddresses(int accountID, int employeeID, IDBConnection connection = null)
        {
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get(connection);
        }

        public Dictionary<int, cEmployeeWorkLocation> WorkLocations
        {
            get
            {
                return (Dictionary<int, cEmployeeWorkLocation>)this._backingCollection;
            }
        }

        public int Count
        {
            get
            {
                return this._backingCollection.Count;
            }
        }

        public bool ContainsKey(int employeeLocationID)
        {
            return this._backingCollection.ContainsKey(employeeLocationID);
        }

        public cEmployeeWorkLocation this[int employeeLocationID]
        {
            get
            {
                return this._backingCollection[employeeLocationID];
            }
        }

        public int Add(cEmployeeWorkLocation location, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var returnVal = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeWorkAddressId", location.EmployeeWorkAddressId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", location.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@addressId", location.LocationID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@active", location.Active);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@temporary", location.Temporary);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@rotational", location.Rotational);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@primaryrotational", location.PrimaryRotational);
                if (location.StartDate == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@startDate", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@startDate", location.StartDate);
                }

                if (location.EndDate == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@endDate", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@endDate", location.EndDate);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.UtcNow);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userId", currentUser.EmployeeID);
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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userId", location.EmployeeID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@ESRAssignmentLocationId", (object)location.EsrAssignmentLocationId ?? DBNull.Value);

                databaseConnection.AddReturn("@returnAddressId", SqlDbType.Int);
                databaseConnection.ExecuteProc("SaveEmployeeWorkAddress");
                returnVal = databaseConnection.GetReturnValue<int>("@returnAddressId");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            if (this._backingCollection.ContainsKey(location.EmployeeWorkAddressId))
            {
                this._backingCollection.Remove(location.EmployeeWorkAddressId);
            }

            this._backingCollection.Add(location.EmployeeWorkAddressId, location);
            this.CacheDelete();
            return returnVal;
        }

        public void Remove(int employeeLocationID, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeWorkAddressId", employeeLocationID);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", this._employeeID);

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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userDelegateId", DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userEmployeeId", 0);
                }
                databaseConnection.ExecuteProc("DeleteEmployeeWorkAddress");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
            this._backingCollection.Remove(employeeLocationID);
            this.CacheDelete();
        }

        public cEmployeeWorkLocation GetBy(int workLocationID)
        {
            cEmployeeWorkLocation location;
            this._backingCollection.TryGetValue(workLocationID, out location);

            return location;
        }

        /// <summary>
        /// Get the current work address for the given esr assignment (if any).
        /// If not, get work address by date.
        /// </summary>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/></param>
        /// <param name="date">The expense item date</param>
        /// <param name="esrAssignId">The esr assignment id.</param>
        /// <returns>The selected work location or null if none found</returns>
        public cEmployeeWorkLocation GetBy(ICurrentUserBase currentUser, DateTime date, int? esrAssignId)
        {
            if (esrAssignId != null && esrAssignId > 0)
            {
                var esrAssignmentLocation = new EsrAssignmentLocations(currentUser.AccountID).GetEsrAssignmentLocationById((int)esrAssignId, date);
                if (esrAssignmentLocation != null)
                {
                    var assignmentAddresses = this.WorkLocations.Where(ewa => ewa.Value.EsrAssignmentLocationId == esrAssignmentLocation.EsrAssignmentLocationId).ToArray();
                    if (assignmentAddresses.Any())
                    {
                        return assignmentAddresses[0].Value;
                    }
                }
            }
            return this.GetBy(date);
        }

        /// <summary>
        /// Get all current work addresses for the given esr assignment (if any).
        /// If not, get work address by date.
        /// </summary>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/></param>
        /// <param name="date">The expense item date</param>
        /// <param name="esrAssignId">The esr assignment id.</param>
        /// <returns>The selected work location or null if none found</returns>
        public List<cEmployeeWorkLocation> GetAllBy(ICurrentUserBase currentUser, DateTime date, int? esrAssignId)
        {
            List<cEmployeeWorkLocation> reqWorkLocation = new List<cEmployeeWorkLocation>();
            if (esrAssignId != null && esrAssignId > 0)
            {
                var esrAssignmentLocation = new EsrAssignmentLocations(currentUser.AccountID).GetEsrAssignmentLocationById((int)esrAssignId, date);
                if (esrAssignmentLocation != null)
                {
                    var assignmentAddresses = this.WorkLocations.Where(ewa => ewa.Value.EsrAssignmentLocationId == esrAssignmentLocation.EsrAssignmentLocationId).ToArray();
                    if (assignmentAddresses.Any())
                    {
                        reqWorkLocation.Add(assignmentAddresses[0].Value);
                        return reqWorkLocation;
                    }
                }
            }
            return this.GetAllBy(date);
        }


        /// <summary>
        /// Get the current work address based on the date.
        /// </summary>
        /// <param name="date">The expense item date</param>
        /// <returns>The selected work location or null if none found</returns>
        public cEmployeeWorkLocation GetBy(DateTime date)
        {
            DateTime? oldDate = null;
            cEmployeeWorkLocation reqWorkLocation = null;
            SortedList<DateTime?, List<cEmployeeWorkLocation>> lstLocs = CreateListOfAddressesByStartDate();

            //Iterate through the collection in date order
            foreach (KeyValuePair<DateTime?, List<cEmployeeWorkLocation>> empLoc in lstLocs)
            {
                if (empLoc.Value.Count > 1)
                {
                    foreach (cEmployeeWorkLocation workLoc in empLoc.Value)
                    {
                        if (empLoc.Key <= date && (workLoc.EndDate == null || workLoc.EndDate >= date))
                        {
                            reqWorkLocation = workLoc;
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
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }
                    else
                    {
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date) && oldDate < empLoc.Key)
                        {
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }
                    oldDate = empLoc.Key;
                }
            }
            return reqWorkLocation;
        }

        /// <summary>
        /// Get all current work addresses based on the date.
        /// </summary>
        /// <param name="date">The expense item date</param>
        /// <returns>The selected work location or null if none found</returns>
        public List<cEmployeeWorkLocation> GetAllBy(DateTime date)
        {
            DateTime? oldDate = null;
            List<cEmployeeWorkLocation> reqWorkLocation = new List<cEmployeeWorkLocation>();
            SortedList<DateTime?, List<cEmployeeWorkLocation>> lstLocs = CreateListOfAddressesByStartDate();

            //Iterate through the collection in date order
            foreach (KeyValuePair<DateTime?, List<cEmployeeWorkLocation>> empLoc in lstLocs)
            {
                if (empLoc.Value.Count > 1)
                {
                    foreach (cEmployeeWorkLocation workLoc in empLoc.Value)
                    {
                        if (empLoc.Key <= date && (workLoc.EndDate == null || workLoc.EndDate >= date))
                        {
                            reqWorkLocation.Add(workLoc);
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
                            reqWorkLocation.Add(empLoc.Value[0]);
                        }
                    }
                    else
                    {
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date) && oldDate < empLoc.Key)
                        {
                            reqWorkLocation.Add(empLoc.Value[0]);
                        }
                    }
                    oldDate = empLoc.Key;
                }
            }
            return reqWorkLocation;
        }

        /// <summary>
        /// Get work address that is marked as Primary Rotational and is valid for the given date.
        /// </summary>
        /// <param name="date">The date of teh claim</param>
        /// <param name="primaryRotationalOnly">Must be set to true.</param>
        /// <returns></returns>
        public cEmployeeWorkLocation GetBy(DateTime date, bool primaryRotationalOnly)
        {
            if (!primaryRotationalOnly)
            {
                return null;
            }

            SortedList<DateTime?, List<cEmployeeWorkLocation>> lstLocs = CreateListOfAddressesByStartDate();
            cEmployeeWorkLocation reqWorkLocation = null;
            DateTime? oldDate = null;

            //Iterate through the collection in date order
            foreach (KeyValuePair<DateTime?, List<cEmployeeWorkLocation>> empLoc in lstLocs)
            {
                if (empLoc.Value.Count > 1)
                {
                    foreach (cEmployeeWorkLocation workLoc in empLoc.Value.Where(x => x.PrimaryRotational))
                    {
                        if (empLoc.Key <= date && (workLoc.EndDate == null || workLoc.EndDate >= date))
                        {
                            reqWorkLocation = workLoc;
                        }
                    }
                }
                else
                {
                    if (oldDate == null)
                    {
                        //empLoc.Key is the start date of the location
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date) && empLoc.Value[0].PrimaryRotational)
                        {
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }
                    else
                    {
                        if (empLoc.Key <= date && (empLoc.Value[0].EndDate == null || empLoc.Value[0].EndDate >= date) && oldDate < empLoc.Key && empLoc.Value[0].PrimaryRotational)
                        {
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }
                    oldDate = empLoc.Key;
                }
            }
            return reqWorkLocation;
        }

        private SortedList<DateTime?, List<cEmployeeWorkLocation>> CreateListOfAddressesByStartDate()
        {
            SortedList<DateTime?, List<cEmployeeWorkLocation>> lstLocs = new SortedList<DateTime?, List<cEmployeeWorkLocation>>();

            //Sort the locations in ascending order of start date. If more than one location has the same start date then they are added
            //to the same location collection for the associated start time key
            foreach (cEmployeeWorkLocation location in this._backingCollection.Values)
            {
                DateTime? startDate = location.StartDate ?? new DateTime(1899, 01, 01);

                if (!lstLocs.ContainsKey(startDate))
                {
                    List<cEmployeeWorkLocation> workLoc = new List<cEmployeeWorkLocation> { location };
                    lstLocs.Add(startDate, workLoc);
                }
                else
                {
                    lstLocs[startDate].Add(location);
                }
            }

            return lstLocs;
        }

       

        public IDictionary<int, cEmployeeWorkLocation> Get(IDBConnection connection = null)
        {
            Dictionary<int, cEmployeeWorkLocation> locations;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                locations = this.CacheGet();

                if (locations == null)
                {
                    locations = new Dictionary<int, cEmployeeWorkLocation>();
                    databaseConnection.sqlexecute.Parameters.Clear();

                    const string SQL = "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, ESRAssignmentLocationId, rotational, primaryRotational FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);

                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        int esrAssignmentLocationIdOrdinal = reader.GetOrdinal("ESRAssignmentLocationId");
                        while (reader.Read())
                        {
                            int employeeLocationID = reader.GetInt32(reader.GetOrdinal("EmployeeWorkAddressId"));
                            int locationID = reader.GetInt32(reader.GetOrdinal("AddressId"));
                            DateTime? startDate = (reader.IsDBNull(reader.GetOrdinal("StartDate")) == false)
                                                      ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("StartDate"))
                                                      : null;
                            DateTime? endDate = (reader.IsDBNull(reader.GetOrdinal("EndDate")) == false)
                                                    ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                                    : null;
                            bool active = reader.GetBoolean(reader.GetOrdinal("Active"));
                            DateTime createdOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                            int createdBy = (reader.IsDBNull(reader.GetOrdinal("CreatedBy")) == false)
                                                  ? reader.GetInt32(reader.GetOrdinal("CreatedBy"))
                                                  : 0;
                            DateTime? modifiedOn = (reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) == false)
                                                       ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("ModifiedOn"))
                                                       : null;
                            int? modifiedBy = (reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) == false)
                                                  ? (int?)reader.GetInt32(reader.GetOrdinal("ModifiedBy"))
                                                  : null;
                            bool temporary = reader.GetBoolean(reader.GetOrdinal("Temporary"));

                            var rotational = reader.IsDBNull(reader.GetOrdinal("rotational")) ? false : reader.GetBoolean(reader.GetOrdinal("rotational"));
                            var primaryrotational = reader.IsDBNull(reader.GetOrdinal("primaryrotational")) ? false : reader.GetBoolean(reader.GetOrdinal("primaryrotational"));

                            int? esrAssignmentLocationId = reader.IsDBNull(esrAssignmentLocationIdOrdinal)
                                ? null
                                : (int?)reader.GetInt32(esrAssignmentLocationIdOrdinal);

                            locations.Add(employeeLocationID, new cEmployeeWorkLocation(employeeLocationID, this._employeeID, locationID, startDate, endDate, active, temporary, createdOn, createdBy, modifiedOn, modifiedBy, esrAssignmentLocationId, primaryrotational));
                        }

                        reader.Close();
                    }

                    this.CacheAdd(locations);

                    databaseConnection.sqlexecute.Parameters.Clear();
                }
            }

            return locations;
        }

        private List<Tuple<int, long>> GetCompanyEsrAllocations(IDBConnection connection = null)
        {
            var result = new List<Tuple<int, long>>();
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                // todo = check company to address
                const string SQL = "select addressId, esrlocationid  from AddressEsrAllocation WHERE EsrLocationID is not null";
                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    while (reader.Read())
                    {
                        result.Add(new Tuple<int, long>(reader.GetInt32(0), reader.GetInt64(1)));
                    }
                }
            }

            return result;
        }

        public IEnumerator<KeyValuePair<int, cEmployeeWorkLocation>> GetEnumerator()
        {
            return this._backingCollection.GetEnumerator();
        }

        private void CacheAdd(Dictionary<int, cEmployeeWorkLocation> gridSorts)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), gridSorts);
        }

        private Dictionary<int, cEmployeeWorkLocation> CacheGet()
        {
            Cache cache = new Cache();
            return (Dictionary<int, cEmployeeWorkLocation>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the previous work address
        /// </summary>
        /// <param name="currentWorkAddress">
        /// The current work address.
        /// </param>
        /// <returns>
        /// The <see cref="cEmployeeWorkLocation"/> of the previous work address.
        /// </returns>
        public cEmployeeWorkLocation GetPreviousWorkLocation(cEmployeeWorkLocation currentWorkAddress)
        {
            if (currentWorkAddress == null || currentWorkAddress.StartDate == null)
            {
                return null;
            }

            var date = ((DateTime)currentWorkAddress.StartDate).AddDays(-1);
            var currentWorkAddressPostcode = Address.Get(
                    this._accountID,
                    currentWorkAddress.LocationID).Postcode.Replace(" ", string.Empty).ToUpper();
            DateTime? oldDate = null;
            cEmployeeWorkLocation reqWorkLocation = null;

            var lstLocs = new SortedList<DateTime?, List<cEmployeeWorkLocation>>();

            // Sort the locations in ascending order of start date. If a location has the same postcode 
            // as the current work address it gets ignored. Of the remaining addresses, if more than one
            // location has the same start date then they are added to the same location collection for 
            // the associated start time key
            foreach (var location in this._backingCollection.Values)
            {
                var locationPostcode = Address.Get(
                    this._accountID,
                    location.LocationID).Postcode.Replace(" ", string.Empty).ToUpper();

                if (locationPostcode.Equals(currentWorkAddressPostcode))
                {
                    continue;
                }

                DateTime? startDate = location.StartDate ?? new DateTime(1899, 01, 01);

                if (location.LocationID != currentWorkAddress.LocationID)
                {
                    if (!lstLocs.ContainsKey(startDate))
                    {
                        var workLoc = new List<cEmployeeWorkLocation> { location };
                        lstLocs.Add(startDate, workLoc);
                    }
                    else
                    {
                        lstLocs[startDate].Add(location);
                    }
                }
            }

            // Iterate through the collection in date order
            foreach (KeyValuePair<DateTime?, List<cEmployeeWorkLocation>> empLoc in lstLocs)
            {
                if (empLoc.Value.Count > 1)
                {
                    foreach (var workLoc in empLoc.Value.Where(workLoc => empLoc.Key <= date))
                    {
                        reqWorkLocation = workLoc;
                    }
                }
                else
                {
                    if (oldDate == null)
                    {
                        // empLoc.Key is the start date of the location
                        if (empLoc.Key <= date)
                        {
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }
                    else
                    {
                        if (empLoc.Key <= date && oldDate < empLoc.Key)
                        {
                            reqWorkLocation = empLoc.Value[0];
                        }
                    }

                    oldDate = empLoc.Key;
                }
            }

            return reqWorkLocation;
        }
    }
}
