namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Address related data object and functionality.
    /// </summary>
    [Serializable]
    public class Address
    {
        #region Constructors and Destructors

        /// <summary>
        /// Basic address constructor
        /// </summary>
        public Address()
        {
            this.Identifier = 0;
            this.CreatedOn = DateTime.UtcNow;
        }

        #endregion Constructors and Destructors

        #region Enums

        /// <summary>
        ///     The feature that was used to create the address
        /// </summary>
        public enum AddressCreationMethod
        {
            None = 0,

            CapturePlus = 1,

            Global = 2,

            EsrOutbound = 3,

            ImplementationImportRoutine = 4,

            DataImportWizard = 5,

            ManualByAdministrator = 6,

            ManualByClaimant = 7
        }

        /// <summary>
        /// The information source used as the address locator
        /// </summary>
        public enum LocatorType
        {
            None = 0,

            Postcode = 1,

            LatLongs = 2
        }

        #endregion Enums

        #region Public Properties

        /// <summary>
        /// Is the address favourited for the entire account.
        /// </summary>
        public bool AccountWideFavourite { get; set; }

        /// <summary>
        /// A name for the address, usually a company name when supplied.
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// Is the address logically "soft deleted" and only kept for historical data.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// The city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The country (global country id) the address is in.
        /// </summary>
        public int Country { get; set; }

        /// <summary>
        /// The country (alpha 3 country code) the address is in.
        /// </summary>
        public string CountryAlpha3Code { get; set; }

        /// <summary>
        /// The name of the country, e.g. "United Kingdom"
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// The number of anonymous lookup usages used, which is incremented so that it can be restricted.
        /// </summary>
        public int AnonymousLookupUsages { get; set; }

        /// <summary>
        ///     The county the address is in
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// The user that created the address.
        /// </summary>
        public int CreatedBy { get; private set; }

        /// <summary>
        /// The date the address was created.
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// The way in which the address was created.
        /// </summary>
        public AddressCreationMethod CreationMethod { get; set; }


        /// <summary>
        /// The friendly address name.
        /// </summary>
        private string friendlyName;

        /// <summary>
        /// Gets or sets the friendly address name, this is the address name, first line of the address, city and the postcode.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (string.IsNullOrEmpty(this.friendlyName))
                {
                    this.friendlyName = GenerateFriendlyName(this.AddressName, this.Line1, this.Postcode, this.City);
                }

                return this.friendlyName;
            }

            set
            {
                this.friendlyName = value;
            }
        }

        /// <summary>
        /// The identifier from the external address source (capture plus).
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// The unique identifier for this address.
        /// </summary>
        public int Identifier { get; set; }

        /// <summary>
        /// The latitude of the address.
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// The first line of the address, usually a number and street.
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// The second line of the address.
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// The second line of the address.
        /// </summary>
        public string Line3 { get; set; }

        /// <summary>
        /// The longitude of the address.
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// The last time the address was fetched from an external source.
        /// </summary>
        public DateTime LookupDate { get; set; }

        /// <summary>
        /// The user that last modified the address.
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// The date the address was last modified.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the UDPRN (Unique delivery point reference number).
        /// </summary>
        public int Udprn { get; set; }

        /// <summary>
        /// The address's postal code
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// The subaccount that this address belongs to, not yet used.
        /// </summary>
        public int? SubAccountIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is private.
        /// </summary>
        public bool IsPrivate { get; set; }

        #endregion Public Properties

        #region Public Methods and Operators

        /// <summary>
        /// Cleanse an address by its identifier
        /// Removes all labels, awabels, favourites, awavourites and recommended distances associated with an address
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="addressIdentifier">The address identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A positive number indicating success</returns>
        public static int Cleanse(ICurrentUserBase currentUser, int addressIdentifier, IDBConnection connection = null)
        {
            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifier",
                    addressIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            var address = Get(currentUser.AccountID, addressIdentifier);
            var addressArchiveToggled = false;

            // if the address is archived then the favourites and labels would be ignored, so temporily un-archive it
            if (address.Archived)
            {
                ToggleArchive(currentUser, address.Identifier);
                addressArchiveToggled = true;
            }

            // remove all favourites and account-wide favourites
            var favourites = new Favourites(currentUser.AccountID).Get(currentUser, AddressFilter.All);
            foreach (var favourite in favourites)
            {
                if (favourite.AddressID != address.Identifier)
                {
                    continue;
                }

                if (favourite.EmployeeID != 0)
                {
                    Favourite.Delete(currentUser, favourite.FavouriteID);
                }
                else if (address.AccountWideFavourite)
                {
                    ToggleAccountWideFavourite(currentUser, address.Identifier);
                }
            }

            // remove all labels and account-wide labels
            var labels = new AddressLabels(currentUser.AccountID).Search(string.Empty, AddressFilter.All);
            foreach (var label in labels)
            {
                if (label.AddressID == address.Identifier)
                {
                    AddressLabel.Delete(currentUser, label.AddressLabelID);
                }
            }

            // remove all recommended distances
            var addressDistances = AddressDistance.GetForAddress(currentUser.AccountID, address.Identifier);
            foreach (var addressDistance in addressDistances)
            {
                if (addressDistance.OutboundIdentifier > 0)
                {
                    AddressDistance.Delete(currentUser, addressDistance.OutboundIdentifier);
                }

                if (addressDistance.ReturnIdentifier > 0)
                {
                    AddressDistance.Delete(currentUser, addressDistance.ReturnIdentifier);
                }
            }

            // re-archive the address if it was previously archived
            if (addressArchiveToggled)
            {
                ToggleArchive(currentUser, address.Identifier);
            }

            return 1;
        }

        /// <summary>
        ///     Delete an address by its identifier
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="addressIdentifier">The address identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A zero indicating success or a negative return code</returns>
        public static int Delete(ICurrentUserBase currentUser, int addressIdentifier, IDBConnection connection = null)
        {
            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifier",
                    addressIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            int returnValue;
            var addressDistancesToDelete = AddressDistance.GetAddressDistancesToDelete(currentUser.AccountID, addressIdentifier);
            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("DeleteAddresses");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            if (returnValue == 0)
            {
                foreach (var addressDistance in addressDistancesToDelete)
                {
                    AddressDistance.DeleteFromCache(
                        currentUser.AccountID,
                        addressDistance.OutboundIdentifier,
                        addressDistance.ReturnIdentifier);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Creates a friendly string representation of an address from the various address parts
        /// </summary>
        /// <param name="addressName">The name of the address if available, generally a company name</param>
        /// <param name="line1">The first line of the address</param>
        /// <param name="postcode">The postcode</param>
        /// <returns>The friendly address string</returns>
        public static string GenerateFriendlyName(string addressName, string line1, string postcode, string city)
        {
            var addressParts = new[] { addressName, line1, city, postcode };

            return string.Join(", ", addressParts.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        /// <summary>
        ///     Obtain an address object populated with the appropriate information for the given account and address
        /// </summary>
        /// <param name="accountIdentifier">Account to retrieve the address from, must be positive</param>
        /// <param name="addressIdentifier">The address to retrieve, must be positive</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The populated address object</returns>
        public static Address Get(int accountIdentifier, int addressIdentifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "accountIdentifier",
                    accountIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            return GetFromDatabase(accountIdentifier, new SearchFieldAndIdentifiers(addressIdentifier), connection);
        }

        /// <summary>
        ///     Obtain an address object populated with the appropriate information for the given account and address
        ///     If the address isn't found then retrieve it from PostcodeAnywhere, store it, then return it
        ///     If storing it fails due to a duplicate with a different GlobalIdentifier, then search for the duplicate, then return it
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="globalIdentifier">The global identifier (capture plus) for the address to retrieve, must be positive</param>
        /// <param name="countries">The countries object, needed for resolving the SEL country ID from a country's Alpha3 code</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The populated address object</returns>
        public static Address Get(
            ICurrentUserBase currentUser,
            string globalIdentifier,
            IGlobalCountries countries,
            IDBConnection connection = null)
        {
            // attempt to retrieve the address from the database
            Address address = null;
            int? accountId = null;
            int? subAccountId = null;
            int? employeeId = null;
            if (currentUser != null)
            {
                address = GetFromDatabase(
                    currentUser.AccountID,
                    new SearchFieldAndIdentifiers(globalIdentifier),
                    connection);
                accountId = currentUser.AccountID;
                subAccountId = currentUser.CurrentSubAccountId;
                employeeId = currentUser.EmployeeID;

                // if the address wasn't found attempt to retrieve it from the global address database
                if (address == null)
                {
                    address = GetFromGlobalDatabase(currentUser, globalIdentifier, countries);
                }
            }

            // if the address wasn't found retrieve it from postcode anywhere, then store it in the database
            if (address == null)
            {
                var postcodeAnywhere = new PostcodeAnywhere(accountId);
                var results = postcodeAnywhere.CapturePlusInteractiveRetrieveById(globalIdentifier);
                var commonRows = results.Rows.Cast<DataRow>();
                var rows = commonRows as IList<DataRow> ?? commonRows.ToList();

                var addressName =
                    rows.Where(r => r["FieldName"].ToString() == "Company")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var line1 =
                    rows.Where(r => r["FieldName"].ToString() == "Line1")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var line2 =
                    rows.Where(r => r["FieldName"].ToString() == "Line2")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var city =
                    rows.Where(r => r["FieldName"].ToString() == "City")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var province =
                    rows.Where(r => r["FieldName"].ToString() == "ProvinceName")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var countryCode =
                    rows.Where(r => r["FieldName"].ToString() == "CountryCode")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();
                var postcode =
                    rows.Where(r => r["FieldName"].ToString() == "PostalCode")
                        .Select(r => r["FormattedValue"])
                        .First()
                        .ToString();

                var latitude =
                    rows.Where(r => r["FieldName"].ToString() == "Latitude")
                        .Select(r => r["FormattedValue"])
                        .DefaultIfEmpty(string.Empty)
                        .First()
                        .ToString();
                var longitude =
                    rows.Where(r => r["FieldName"].ToString() == "Longitude")
                        .Select(r => r["FormattedValue"])
                        .DefaultIfEmpty(string.Empty)
                        .First()
                        .ToString();

                int udprn;
                int.TryParse(
                    rows.Where(
                        r => r["FieldName"].ToString().Equals("UDPRN", StringComparison.InvariantCultureIgnoreCase))
                        .Select(r => r["FormattedValue"])
                        .DefaultIfEmpty("0")
                        .First()
                        .ToString(),
                    out udprn);

                // resolve the country identifier from the country code (e.g. "GBR")
                int countryId = countries.GetGlobalCountryIdByAlpha3Code(countryCode);
                string countryName = countries.getGlobalCountryById(countryId).Country;

                if (string.IsNullOrWhiteSpace(line1))
                {
                    line1 = "Street Record";
                }

                // save the address in the global database to avoid looking it up again for other accounts
                SaveGlobal(
                    addressName,
                    line1,
                    line2,
                    string.Empty,
                    city,
                    province,
                    countryId,
                    postcode,
                    latitude,
                    longitude,
                    globalIdentifier,
                    udprn);

                // save the address and create a new object to return
                var identifier = currentUser == null
                                     ? 0
                                     : Save(
                                         currentUser,
                                         0,
                                         addressName,
                                         line1,
                                         line2,
                                         string.Empty,
                                         city,
                                         province,
                                         countryId,
                                         postcode,
                                         latitude,
                                         longitude,
                                         globalIdentifier,
                                         udprn,
                                         false,
                                         AddressCreationMethod.CapturePlus,
                                         connection);

                // if the item was a duplicate, fetch the duplicate
                address = new Address()
                              {
                                  Identifier = identifier,
                                  Postcode = postcode,
                                  AddressName = addressName,
                                  Line1 = line1,
                                  Line2 = line2,
                                  Line3 = string.Empty,
                                  City = city,
                                  County = province,
                                  Country = countryId,
                                  CountryAlpha3Code = countryCode,
                                  CountryName = countryName,
                                  Archived = false,
                                  CreationMethod = AddressCreationMethod.CapturePlus,
                                  LookupDate = DateTime.UtcNow,
                                  SubAccountIdentifier = subAccountId,
                                  GlobalIdentifier = globalIdentifier,
                                  Longitude = longitude,
                                  Latitude = latitude,
                                  Udprn = udprn,
                                  CreatedOn = DateTime.UtcNow,
                                  CreatedBy = employeeId ?? 0,
                                  ModifiedOn = DateTime.MinValue,
                                  ModifiedBy = -1
                              };

                if (currentUser != null)
                {
                    PostcodeAnywhere.LogAddressLookup(currentUser, address);
                }
            }

            if (address.Identifier == -1)
            {
                address = GetDuplicateFromDatabase(
                    currentUser,
                    0,
                    address.AddressName,
                    address.Line1,
                    address.City,
                    address.Country,
                    address.Postcode,
                    address.CreationMethod,
                    connection);
            }

            return address;
        }

        /// <summary>
        ///     Obtain address objects populated with the appropriate information for the given account and addresses
        /// </summary>
        /// <param name="accountIdentifier">Account to retrieve the address from, must be positive</param>
        /// <param name="addressIdentifiers">The addresses to retrieve</param>
        /// <param name="usePrimaryLabelFriendlyName">Override the normal friendly name with the primary label if one exists</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The populated address object</returns>
        public static List<Address> Get(
            int accountIdentifier,
            List<int> addressIdentifiers,
            bool usePrimaryLabelFriendlyName = false,
            IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "accountIdentifier",
                    accountIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            if (addressIdentifiers.Count == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifiers",
                    addressIdentifiers,
                    "This argument must contain values.");
            }

            return GetFromDatabase(accountIdentifier, addressIdentifiers, usePrimaryLabelFriendlyName, connection);
        }

        /// <summary>
        ///     Obtain an address object based on a given keyword (e.g. "home", "office"), date and optionally ESR Assignment
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="keyword">The keyword</param>
        /// <param name="date">A date which will be used to determine the correct address</param>
        /// <param name="esrAssignId">The user's selected ESR assignment</param>
        /// <returns>The populated address object</returns>
        public static Address GetByReservedKeyword(
            ICurrentUserBase currentUser,
            string keyword,
            DateTime date,
            cAccountProperties accountProperties,
            int? esrAssignId = null)
        {
            keyword = keyword.Trim().ToLower();

            Address address = null;

            if (keyword == accountProperties.HomeAddressKeyword.ToLower() || keyword == "home")
            {
                cEmployeeHomeLocation tmpHomeLocation = currentUser.Employee.GetHomeAddresses().GetBy(date);
                if (tmpHomeLocation != null)
                {
                    address = Get(currentUser.AccountID, tmpHomeLocation.LocationID);
                }

                if (address != null)
                {
                    address.FriendlyName = accountProperties.HomeAddressKeyword;
                }
            }
            else if (keyword == accountProperties.WorkAddressKeyword.ToLower() || keyword == "office")
            {
                cEmployeeWorkLocation tmpWorkLocation = currentUser.Employee.GetWorkAddresses()
                    .GetBy(currentUser, date, esrAssignId);
                if (tmpWorkLocation != null)
                {
                    address = Get(currentUser.AccountID, tmpWorkLocation.LocationID);
                }
            }

            return address;
        }


        /// <summary>
        ///  Obtain a list addresses based on a given keyword (e.g. "home", "office"), date and optionally ESR Assignment. Returns a list of all work addresses, but just the top home address.
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="keyword">The keyword</param>
        /// <param name="date">A date which will be used to determine the correct address</param>
        /// <param name="accountProperties">The account properties</param>
        /// <param name="esrAssignId">The user's selected ESR assignment</param>
        /// <returns>The populated address object</returns>
        public static List<Address> GetAllByReservedKeyword(
            ICurrentUserBase currentUser,
            string keyword,
            DateTime date,
            cAccountProperties accountProperties,
            int? esrAssignId = null)
        {
            keyword = keyword.Trim().ToLower();

            List<Address> address = new List<Address>();

            if (keyword == accountProperties.HomeAddressKeyword.ToLower() || keyword == "home")
            {
                cEmployeeHomeLocation tmpHomeLocation = currentUser.Employee.GetHomeAddresses().GetBy(date);
                if (tmpHomeLocation != null)
                {
                    address.Add(Get(currentUser.AccountID, tmpHomeLocation.LocationID));
                }
                if (address.Count > 0) 
                {
                    address[0].FriendlyName = accountProperties.HomeAddressKeyword;
                }
            }
            else if (keyword == accountProperties.WorkAddressKeyword.ToLower() || keyword == "office")
            {
                List<cEmployeeWorkLocation> lstTmpWorkLocation = currentUser.Employee.GetWorkAddresses().GetAllBy(currentUser, date, accountProperties.MultipleWorkAddress ? null : esrAssignId);
                if (lstTmpWorkLocation != null)
                {
                    foreach (var tmpWorkLocation in lstTmpWorkLocation)
                    {
                        address.Add(Get(currentUser.AccountID, tmpWorkLocation.LocationID));
                    }
                    
                }
            }

            return address;
        }

        /// <summary>
        ///     Save an address
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="addressIdentifier">The identity of the address being edited or 0 for adding a new address</param>
        /// <param name="addressName">The name of the address, generally a company name if supplied</param>
        /// <param name="line1">The first line of an address, generally a number and street</param>
        /// <param name="line2">The second line of an address</param>
        /// <param name="line3">The third line of an address</param>
        /// <param name="city">The city the address is in</param>
        /// <param name="county">The county, region or locality the address is in</param>
        /// <param name="country">The identifier of the country from the global countries</param>
        /// <param name="postcode">The postal code for the address</param>
        /// <param name="latitude">latitude from external address lookup</param>
        /// <param name="longitude">Longitude from external address lookup</param>
        /// <param name="globalIdentifier">The identifier from an external address data source (capture plus)</param>
        /// <param name="udprn">The official Unique Delivery Point Reference Number</param>
        /// <param name="accountWideFavourite">Is the address an account-wide favourite</param>
        /// <param name="creationMethod">How was the address created</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The positive identifier of the saved address or a negative return code</returns>
        public static int Save(
            ICurrentUserBase currentUser,
            int addressIdentifier,
            string addressName,
            string line1,
            string line2,
            string line3,
            string city,
            string county,
            int country,
            string postcode,
            string latitude,
            string longitude,
            string globalIdentifier,
            int udprn,
            bool accountWideFavourite,
            AddressCreationMethod creationMethod = AddressCreationMethod.None,
            IDBConnection connection = null)
        {
            if (addressIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifier",
                    addressIdentifier,
                    "The value of this argument must be greater than (if editing) or equal to 0 (if saving new).");
            }

            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser", "The value of this argument must not be null.");
            }

            int returnValue;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue(
                    "@SubAccountID",
                    currentUser == null ? (int?)null : currentUser.CurrentSubAccountId);
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.AddWithValue("@AddressName", addressName, 256);
                databaseConnection.AddWithValue("@Line1", line1, 256);
                databaseConnection.AddWithValue("@Line2", line2, 256);
                databaseConnection.AddWithValue("@Line3", line3, 256);
                databaseConnection.AddWithValue("@City", city, 256);
                databaseConnection.AddWithValue("@County", county, 256);
                databaseConnection.AddWithValue("@Country", country);
                databaseConnection.AddWithValue("@Postcode", postcode, 32);
                databaseConnection.AddWithValue("@CreationMethod", creationMethod);
                databaseConnection.AddWithValue("@Latitude", latitude, 20);
                databaseConnection.AddWithValue("@Longitude", longitude, 20);
                databaseConnection.AddWithValue("@LookupDate", DBNull.Value);
                databaseConnection.AddWithValue("@GlobalIdentifier", globalIdentifier);
                databaseConnection.AddWithValue("@Udprn", udprn);
                databaseConnection.AddWithValue("@AccountWideFavourite", accountWideFavourite);
                databaseConnection.AddWithValue("@UserID", currentUser == null ? (int?)null : currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveAddress");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        ///     Save an address or get the duplicate entry if one already exists
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="addressIdentifier">The identity of the address being edited or 0 for adding a new address</param>
        /// <param name="line1">The first line of an address, generally a number and street</param>
        /// <param name="line2">The second line of an address</param>
        /// <param name="line3">The third line of an address</param>
        /// <param name="city">The city the address is in</param>
        /// <param name="county">The county, region or locality the address is in</param>
        /// <param name="country">The identifier of the country from the global countries</param>
        /// <param name="postcode">The postal code for the address</param>
        /// <param name="latitude">latitude from external address lookup</param>
        /// <param name="longitude">Longitude from external address lookup</param>
        /// <param name="globalIdentifier">The identifier from an external address data source (capture plus)</param>
        /// <param name="udprn">The official Unique Delivery Point Reference Number</param>
        /// <param name="accountWideFavourite">Is the address an account-wide favourite</param>
        /// <param name="creationMethod">How was the address created</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The address object with a positive identifier of the saved/duplicate address, or null</returns>
        public static Address SaveOrGetDuplicate(
            ICurrentUserBase currentUser,
            int addressIdentifier,
            string addressName,
            string line1,
            string line2,
            string line3,
            string city,
            string county,
            int country,
            string postcode,
            string latitude,
            string longitude,
            string globalIdentifier,
            int udprn,
            bool accountWideFavourite,
            AddressCreationMethod creationMethod,
            IDBConnection connection = null)
        {
            Address address = null;

            int returnValue = Save(
                currentUser,
                addressIdentifier,
                addressName,
                line1,
                line2,
                line3,
                city,
                county,
                country,
                postcode,
                latitude,
                longitude,
                globalIdentifier,
                udprn,
                accountWideFavourite,
                creationMethod,
                connection);

            if (returnValue > 0)
            {
                address = new Address()
                              {
                                  Identifier = returnValue,
                                  Postcode = postcode,
                                  AddressName = addressName,
                                  Line1 = line1,
                                  Line2 = line2,
                                  Line3 = line3,
                                  City = city,
                                  County = county,
                                  Country = country,
                                  Archived = false,
                                  CreationMethod = creationMethod,
                                  GlobalIdentifier = globalIdentifier,
                                  Longitude = longitude,
                                  Latitude = latitude,
                                  Udprn = udprn,
                                  CreatedOn = DateTime.UtcNow,
                                  CreatedBy = currentUser.EmployeeID,
                                  ModifiedOn = DateTime.MinValue,
                                  ModifiedBy = -1
                              };
            }
            else if (returnValue == -1)
            {
                address = GetDuplicateFromDatabase(
                    currentUser,
                    addressIdentifier,
                    addressName,
                    line1,
                    city,
                    country,
                    postcode,
                    creationMethod,
                    connection);
            }

            return address;
        }

        /// <summary>
        ///     Save an address in the global database
        /// </summary>
        /// <param name="addressName">The name of the address if available, generally a company name</param>
        /// <param name="line1">The first line of an address, generally a number and street</param>
        /// <param name="line2">The second line of an address</param>
        /// <param name="line3">The third line of an address</param>
        /// <param name="city">The city the address is in</param>
        /// <param name="county">The county, region or locality the address is in</param>
        /// <param name="country">The identifier of the country from the global countries</param>
        /// <param name="postcode">The postal code for the address</param>
        /// <param name="latitude">latitude from external address lookup</param>
        /// <param name="longitude">Longitude from external address lookup</param>
        /// <param name="globalIdentifier">The identifier from an external address data source (capture plus)</param>
        /// <param name="udprn">The official Unique Delivery Point Reference Number</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The positive identifier of the saved address or a negative return code</returns>
        public static int SaveGlobal(
            string addressName,
            string line1,
            string line2,
            string line3,
            string city,
            string county,
            int country,
            string postcode,
            string latitude,
            string longitude,
            string globalIdentifier,
            int udprn,
            IDBConnection connection = null)
        {
            int returnValue;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          GlobalVariables.GlobalAddressDatabaseConnectionString))
            {
                databaseConnection.AddWithValue("@AddressName", addressName, 250);
                databaseConnection.AddWithValue("@Line1", line1, 256);
                databaseConnection.AddWithValue("@Line2", line2, 256);
                databaseConnection.AddWithValue("@Line3", line3, 256);
                databaseConnection.AddWithValue("@City", city, 256);
                databaseConnection.AddWithValue("@County", county, 256);
                databaseConnection.AddWithValue("@Country", country);
                databaseConnection.AddWithValue("@Postcode", postcode, 32);
                databaseConnection.AddWithValue("@Latitude", latitude, 20);
                databaseConnection.AddWithValue("@Longitude", longitude, 20);
                databaseConnection.AddWithValue("@LookupDate", DBNull.Value);
                databaseConnection.AddWithValue("@ProviderIdentifier", globalIdentifier);
                databaseConnection.AddWithValue("@Udprn", udprn);

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveAddress");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// Flip the archived status of the address
        /// </summary>
        /// <param name="currentUser">The current user object</param>
        /// <param name="addressIdentifier">The address to change the archive status of</param>
        /// <param name="connection">The connection to use instead of the default, usually for unit testing</param>
        /// <returns>1 on success or -99 on error</returns>
        public static int ToggleArchive(
            ICurrentUserBase currentUser,
            int addressIdentifier,
            IDBConnection connection = null)
        {
            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifier",
                    addressIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("ArchiveAddress");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// Flip the account wide favourite status of the address
        /// </summary>
        /// <param name="currentUser">The current user object</param>
        /// <param name="addressIdentifier">The address to change the favourite status of</param>
        /// <param name="connection">The connection to use instead of the default, usually for unit testing</param>
        /// <returns>1 on success or -99 on error</returns>
        public static int ToggleAccountWideFavourite(
            ICurrentUserBase currentUser,
            int addressIdentifier,
            IDBConnection connection = null)
        {
            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "addressIdentifier",
                    addressIdentifier,
                    "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("ToggleAddressAccountWideFavourite");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        ///     Save an instance of an address
        /// </summary>
        /// <param name="currentUser">The currentuser object for accountID etc</param>
        /// <param name="connection">An override connection object, generally for unit testing</param>
        /// <returns>Positive address identifier or negative error code</returns>
        public int Save(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            return Save(
                currentUser,
                this.Identifier,
                this.AddressName ?? string.Empty,
                this.Line1 ?? string.Empty,
                this.Line2 ?? string.Empty,
                this.Line3 ?? string.Empty,
                this.City ?? string.Empty,
                this.County ?? string.Empty,
                this.Country,
                this.Postcode ?? string.Empty,
                this.Latitude ?? string.Empty,
                this.Longitude ?? string.Empty,
                this.GlobalIdentifier ?? string.Empty,
                this.Udprn,
                this.AccountWideFavourite,
                this.CreationMethod,
                connection);
        }

        /// <summary>
        ///     Get existing ESR address
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="addressLineOne">The first line of the address, must not be null or empty</param>
        /// <param name="postcode">The postcode of the address, must not be null or empty</param>
        /// <param name="connection">An override connection object, generally for unit testing</param>
        /// <returns>The id of the address</returns>
        public static int FindForEsr(
            int accountIdentifier,
            string addressLineOne,
            string postcode,
            IDBConnection connection = null)
        {
            if (string.IsNullOrWhiteSpace(addressLineOne))
            {
                throw new ArgumentOutOfRangeException(
                    "addressLineOne",
                    addressLineOne,
                    "The value of this argument must be provided.");
            }

            if (string.IsNullOrWhiteSpace(addressLineOne))
            {
                throw new ArgumentOutOfRangeException(
                    "postcode",
                    postcode,
                    "The value of this argument must be provided.");
            }

            int addressIdentifier = 0;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(accountIdentifier)))
            {
                const string StrSql =
                    "SELECT TOP 1 AddressID FROM addresses WHERE Line1 = @addressOne AND Postcode = @postcode AND CreationMethod = @creationMethod ORDER BY AddressID";

                databaseConnection.AddWithValue("@addressOne", addressLineOne, 256);
                databaseConnection.AddWithValue("@postcode", postcode, 32);
                databaseConnection.AddWithValue("@creationMethod", AddressCreationMethod.EsrOutbound);

                using (IDataReader reader = databaseConnection.GetReader(StrSql))
                {
                    if (reader.Read())
                    {
                        addressIdentifier = reader.GetInt32(0);
                    }

                    reader.Close();
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return addressIdentifier;
        }

        /// <summary>
        /// Processes an employee's home and office locations, turning them into a list of <see cref="Address"/>
        /// </summary>
        /// <param name="employeeHomeLocations">
        /// The employee's home locations.
        /// </param>
        /// <param name="employeeOfficeLocations">
        /// The employee's office locations.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <returns>
        /// The a list of <see cref="Address"/>
        /// </returns>
        public static List<Address> ProcessHomeAndOfficeLocationIds(
            List<cEmployeeHomeLocation> employeeHomeLocations,
            IDictionary<int, cEmployeeWorkLocation> employeeOfficeLocations,
            ICurrentUserBase currentUser)
        {
            var addressIdentifiers = new List<int>();

            if (employeeHomeLocations != null)
            {
                addressIdentifiers.AddRange(employeeHomeLocations.Select(x => x.LocationID));
            }

            if (employeeOfficeLocations != null)
            {
                addressIdentifiers.AddRange(employeeOfficeLocations.Values.Select(x => x.LocationID));
            }

            var addresses = new List<Address>();

            if (addressIdentifiers.Count > 0)
            {
                addresses = Get(currentUser.AccountID, addressIdentifiers);
            }

            return addresses;
        }

       /// <summary>
        /// Whether the address is the home address of the employee
        /// </summary>
        /// <param name="employee">The employee</param>
        /// <param name="addressId">The ID of the address</param>
        /// <returns>Whether the address is an employees home address or not</returns>
        public static bool IsHomeAddress(Employee employee, int addressId)
        {
            return employee.GetHomeAddresses().HomeLocations.Any(h => h.LocationID == addressId);
        }

        /// <summary>
        /// Determines whether the route can be viewed or not if it contains a home address for the employee who owns the claim
        /// </summary>
        /// <param name="claimEmployeeId">
        /// The Id of the employee who owns the claim
        /// </param>
        /// <param name="addressIdentifiers">
        /// The address identifiers.
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="accountSubAccount">
        /// The current account sub account.
        /// </param>
        /// <param name="homeAddresses">
        ///  A IDictionary of the claim owners home addresses
        /// </param>
        /// <returns>
        /// Whether the route can be viewed or not
        /// </returns>
        public static bool CanViewRoute(int claimEmployeeId, List<int> addressIdentifiers, ICurrentUserBase user, cAccountSubAccountsBase accountSubAccount, ReadOnlyCollection<cEmployeeHomeLocation> homeAddresses)
        {
            var subAccount = accountSubAccount.getSubAccountById(user.CurrentSubAccountId);
            var otherUsersClaim = user.isDelegate || user.EmployeeID != claimEmployeeId;

            if (claimEmployeeId != 0 && otherUsersClaim && !subAccount.SubAccountProperties.ShowFullHomeAddressOnClaims)
            {
                //potential data protection - are there any home addresses?  
                if (homeAddresses.Any(h => addressIdentifiers.Contains(h.LocationID)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks addresses can be processed by ensuring they have a post code.
        /// </summary>
        /// <param name="addressIdentifiers">
        /// The list of address identifiers.
        /// </param>
        /// <param name="addresses">
        /// The list of <see cref="addresses">addresses</see>
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="account">
        /// The current account.
        /// </param>
        /// <returns>
        /// Whether the provided addresses have post codes set or not 
        /// </returns>
        public static bool CheckAddressesHavePostcode(List<int> addressIdentifiers, List<Address> addresses, ICurrentUserBase user, cAccount account)
        {
            foreach (var addressIdentifier in addressIdentifiers)
            {
                var address = addresses.FirstOrDefault(x => x.Identifier == addressIdentifier) ?? new Address();

                // if any address doesn't contain a postcode then we can't continue
                if (string.IsNullOrWhiteSpace(address.Postcode))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the provided addresse's way points 
        /// </summary>
        /// <param name="addressIdentifiers">
        /// The list of address identifiers to get the coordinates for
        /// </param>
        /// <param name="addresses">
        /// The list of <see cref="addresses">addresses</see> to get the coordinates for
        /// </param>  
        /// <param name="account">
        /// The current account.
        /// </param>
        /// <returns>
        /// A <see cref="List">list</see> of way points
        /// </returns>
        public static List<string> GetAddressCoordinates(List<int> addressIdentifiers, List<Address> addresses, cAccount account)
        {
            var wayPoints = new List<string>();
            foreach (var addressIdentifier in addressIdentifiers)
            {
                var address = addresses.FirstOrDefault(x => x.Identifier == addressIdentifier) ?? new Address();
                wayPoints.Add(address.GetLookupLocator(account.AddressInternationalLookupsAndCoordinates, account.AddressLookupProvider));
            }

            return wayPoints;
        }

        #endregion Public Methods and Operators

        #region Methods

        /// <summary>
        ///     Retrieve an address from the database
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the address from</param>
        /// <param name="addressIdentifier">The identifier for the address</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>An address object populated from the database entry or null if the address cannot be found</returns>
        private static Address GetFromDatabase(
            int accountIdentifier,
            int addressIdentifier,
            IDBConnection connection = null)
        {
            return GetFromDatabase(accountIdentifier, new SearchFieldAndIdentifiers(addressIdentifier), connection);
        }

        /// <summary>
        ///     Retrieve an address from the database
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the address from</param>
        /// <param name="globalIdentifier">A global identifier to retrieve the address by (capture plus)</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>An address object populated from the database entry or null if the address cannot be found</returns>
        private static Address GetFromDatabase(
            int accountIdentifier,
            string globalIdentifier,
            IDBConnection connection = null)
        {
            return GetFromDatabase(accountIdentifier, new SearchFieldAndIdentifiers(globalIdentifier), connection);
        }

        /// <summary>
        ///     Retrieve an address from the database
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the address from</param>
        /// <param name="identifier">The identifier for the address and the identifier field to look for the identifier in</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>An address object populated from the database entry or null if the address cannot be found</returns>
        private static Address GetFromDatabase(
            int accountIdentifier,
            SearchFieldAndIdentifiers identifier,
            IDBConnection connection = null)
        {
            Address address = null;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(accountIdentifier)))
            {
                const string SQL =
                    "SELECT a.[AddressID], a.[Postcode], a.[AddressName], a.[Line1], a.[Line2], a.[Line3], a.[City], a.[County], a.[Country], gc.[Country] [CountryName], gc.[Alpha3CountryCode], a.[Archived], a.[CreationMethod], a.[LookupDate], a.[SubAccountID], a.[GlobalIdentifier], a.[Udprn], a.[Longitude], a.[Latitude], a.[CreatedOn], a.[CreatedBy], a.[ModifiedOn], a.[ModifiedBy], a.[AccountWideFavourite] FROM [dbo].[Addresses] a left join global_countries gc on a.Country = gc.globalcountryid WHERE a.[{0}] = @identifier";

                switch (identifier.SqlType)
                {
                    case SqlDbType.NVarChar:
                        databaseConnection.AddWithValue("@identifier", identifier.Identifier, 50);
                        break;

                    default:
                        databaseConnection.AddWithValue("@identifier", identifier.Identifier);
                        break;
                }

                using (IDataReader reader = databaseConnection.GetReader(string.Format(SQL, identifier.Field)))
                {
                    int identifierOrdinal = reader.GetOrdinal("AddressID"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        line2Ordinal = reader.GetOrdinal("Line2"),
                        line3Ordinal = reader.GetOrdinal("Line3"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        countyOrdinal = reader.GetOrdinal("County"),
                        countryOrdinal = reader.GetOrdinal("Country"),
                        countryNameOrdinal = reader.GetOrdinal("CountryName"),
                        countryAlpha3CodeOrdinal = reader.GetOrdinal("Alpha3CountryCode"),
                        archivedOrdinal = reader.GetOrdinal("Archived"),
                        creationMethodOrdinal = reader.GetOrdinal("CreationMethod"),
                        lookupDateOrdinal = reader.GetOrdinal("LookupDate"),
                        subAccountIdentifierOrdinal = reader.GetOrdinal("SubAccountID"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                        longitudeOrdinal = reader.GetOrdinal("Longitude"),
                        latitudeOrdinal = reader.GetOrdinal("Latitude"),
                        udprnOrdinal = reader.GetOrdinal("Udprn"),
                        accountWideFavouriteOrdinal = reader.GetOrdinal("AccountWideFavourite"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy"),
                        modifiedOnOrdinal = reader.GetOrdinal("ModifiedOn"),
                        modifiedByOrdinal = reader.GetOrdinal("ModifiedBy");

                    while (reader.Read())
                    {
                        address = new Address
                                      {
                                          Identifier = reader.GetInt32(identifierOrdinal),
                                          Postcode =
                                              reader.IsDBNull(postcodeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(postcodeOrdinal),
                                          AddressName =
                                              reader.IsDBNull(addressNameOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(addressNameOrdinal),
                                          Line1 =
                                              reader.IsDBNull(line1Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line1Ordinal),
                                          Line2 =
                                              reader.IsDBNull(line2Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line2Ordinal),
                                          Line3 =
                                              reader.IsDBNull(line3Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line3Ordinal),
                                          City =
                                              reader.IsDBNull(cityOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(cityOrdinal),
                                          County =
                                              reader.IsDBNull(countyOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countyOrdinal),
                                          Country =
                                              reader.IsDBNull(countryOrdinal)
                                                  ? 0
                                                  : reader.GetInt32(countryOrdinal),
                                          CountryName =
                                              reader.IsDBNull(countryNameOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countryNameOrdinal),
                                          CountryAlpha3Code =
                                              reader.IsDBNull(countryAlpha3CodeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countryAlpha3CodeOrdinal),
                                          Archived =
                                              reader.IsDBNull(archivedOrdinal)
                                                  ? false
                                                  : reader.GetBoolean(archivedOrdinal),
                                          CreationMethod =
                                              reader.IsDBNull(creationMethodOrdinal)
                                                  ? AddressCreationMethod.None
                                                  : (AddressCreationMethod)reader.GetInt32(creationMethodOrdinal),
                                          LookupDate =
                                              reader.IsDBNull(lookupDateOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(lookupDateOrdinal),
                                          SubAccountIdentifier =
                                              reader.IsDBNull(subAccountIdentifierOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(subAccountIdentifierOrdinal),
                                          GlobalIdentifier =
                                              reader.IsDBNull(globalIdentifierOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(globalIdentifierOrdinal),
                                          Longitude =
                                              reader.IsDBNull(longitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(longitudeOrdinal),
                                          Latitude =
                                              reader.IsDBNull(latitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(latitudeOrdinal),
                                          Udprn =
                                              reader.IsDBNull(udprnOrdinal) ? 0 : reader.GetInt32(udprnOrdinal),
                                          AccountWideFavourite = reader.GetBoolean(accountWideFavouriteOrdinal),
                                          CreatedOn =
                                              reader.IsDBNull(createdOnOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(createdOnOrdinal),
                                          CreatedBy =
                                              reader.IsDBNull(createdByOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(createdByOrdinal),
                                          ModifiedOn =
                                              reader.IsDBNull(modifiedOnOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(modifiedOnOrdinal),
                                          ModifiedBy =
                                              reader.IsDBNull(modifiedByOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(modifiedByOrdinal)
                                      };
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return address;
        }

        /// <summary>
        ///     Retrieve multiple addresses from the database
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the addresses from</param>
        /// <param name="identifiers">The identifiers for the addresses</param>
        /// <param name="usePrimaryLabelFriendlyName">Override the normal friendly name with the primary label if one exists</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>A list of address objects populated from the database entry or empty if the addresses cannot be found</returns>
        private static List<Address> GetFromDatabase(
            int accountIdentifier,
            List<int> identifiers,
            bool usePrimaryLabelFriendlyName,
            IDBConnection connection = null)
        {
            List<Address> addresses = new List<Address>();

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(accountIdentifier)))
            {
                const string SQL =
                    "SELECT a.[AddressID], a.[Postcode], a.[AddressName], a.[Line1], a.[Line2], a.[Line3], a.[City], a.[County], a.[Country], gc.[Country] [CountryName], gc.[Alpha3CountryCode], a.[Archived], a.[CreationMethod], a.[LookupDate], a.[SubAccountID], a.[GlobalIdentifier], a.[Udprn], a.[Longitude], a.[Latitude], a.[CreatedOn], a.[CreatedBy], a.[ModifiedOn], a.[ModifiedBy], a.[AccountWideFavourite], al.[Label] FROM [dbo].[Addresses] a LEFT JOIN global_countries gc ON a.Country = gc.globalcountryid LEFT JOIN addressLabels al ON al.AddressID = a.AddressID AND al.IsPrimary = 1 AND al.EmployeeID IS NULL INNER JOIN @identifiers AS ids ON ids.c1 = a.AddressID;";

                databaseConnection.AddWithValue("@identifiers", identifiers);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int identifierOrdinal = reader.GetOrdinal("AddressID"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        line2Ordinal = reader.GetOrdinal("Line2"),
                        line3Ordinal = reader.GetOrdinal("Line3"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        countyOrdinal = reader.GetOrdinal("County"),
                        countryOrdinal = reader.GetOrdinal("Country"),
                        countryNameOrdinal = reader.GetOrdinal("CountryName"),
                        countryAlpha3CodeOrdinal = reader.GetOrdinal("Alpha3CountryCode"),
                        archivedOrdinal = reader.GetOrdinal("Archived"),
                        creationMethodOrdinal = reader.GetOrdinal("CreationMethod"),
                        lookupDateOrdinal = reader.GetOrdinal("LookupDate"),
                        subAccountIdentifierOrdinal = reader.GetOrdinal("SubAccountID"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                        longitudeOrdinal = reader.GetOrdinal("Longitude"),
                        latitudeOrdinal = reader.GetOrdinal("Latitude"),
                        udprnOrdinal = reader.GetOrdinal("Udprn"),
                        accountWideFavouriteOrdinal = reader.GetOrdinal("AccountWideFavourite"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy"),
                        modifiedOnOrdinal = reader.GetOrdinal("ModifiedOn"),
                        modifiedByOrdinal = reader.GetOrdinal("ModifiedBy"),
                        labelOrdinal = reader.GetOrdinal("Label");

                    while (reader.Read())
                    {
                        Address address = new Address
                                              {
                                                  Identifier = reader.GetInt32(identifierOrdinal),
                                                  Postcode =
                                                      reader.IsDBNull(postcodeOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(postcodeOrdinal),
                                                  AddressName =
                                                      reader.IsDBNull(addressNameOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(addressNameOrdinal),
                                                  Line1 =
                                                      reader.IsDBNull(line1Ordinal)
                                                          ? string.Empty
                                                          : reader.GetString(line1Ordinal),
                                                  Line2 =
                                                      reader.IsDBNull(line2Ordinal)
                                                          ? string.Empty
                                                          : reader.GetString(line2Ordinal),
                                                  Line3 =
                                                      reader.IsDBNull(line3Ordinal)
                                                          ? string.Empty
                                                          : reader.GetString(line3Ordinal),
                                                  City =
                                                      reader.IsDBNull(cityOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(cityOrdinal),
                                                  County =
                                                      reader.IsDBNull(countyOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(countyOrdinal),
                                                  Country =
                                                      reader.IsDBNull(countryOrdinal)
                                                          ? 0
                                                          : reader.GetInt32(countryOrdinal),
                                                  CountryName =
                                                      reader.IsDBNull(countryNameOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(countryNameOrdinal),
                                                  CountryAlpha3Code =
                                                      reader.IsDBNull(countryAlpha3CodeOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(countryAlpha3CodeOrdinal),
                                                  Archived =
                                                      reader.IsDBNull(archivedOrdinal)
                                                          ? false
                                                          : reader.GetBoolean(archivedOrdinal),
                                                  CreationMethod =
                                                      reader.IsDBNull(creationMethodOrdinal)
                                                          ? AddressCreationMethod.None
                                                          : (AddressCreationMethod)
                                                            reader.GetInt32(creationMethodOrdinal),
                                                  LookupDate =
                                                      reader.IsDBNull(lookupDateOrdinal)
                                                          ? DateTime.MinValue
                                                          : reader.GetDateTime(lookupDateOrdinal),
                                                  SubAccountIdentifier =
                                                      reader.IsDBNull(subAccountIdentifierOrdinal)
                                                          ? -1
                                                          : reader.GetInt32(subAccountIdentifierOrdinal),
                                                  GlobalIdentifier =
                                                      reader.IsDBNull(globalIdentifierOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(globalIdentifierOrdinal),
                                                  Longitude =
                                                      reader.IsDBNull(longitudeOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(longitudeOrdinal),
                                                  Latitude =
                                                      reader.IsDBNull(latitudeOrdinal)
                                                          ? string.Empty
                                                          : reader.GetString(latitudeOrdinal),
                                                  Udprn =
                                                      reader.IsDBNull(udprnOrdinal)
                                                          ? 0
                                                          : reader.GetInt32(udprnOrdinal),
                                                  AccountWideFavourite =
                                                      reader.GetBoolean(accountWideFavouriteOrdinal),
                                                  CreatedOn =
                                                      reader.IsDBNull(createdOnOrdinal)
                                                          ? DateTime.MinValue
                                                          : reader.GetDateTime(createdOnOrdinal),
                                                  CreatedBy =
                                                      reader.IsDBNull(createdByOrdinal)
                                                          ? -1
                                                          : reader.GetInt32(createdByOrdinal),
                                                  ModifiedOn =
                                                      reader.IsDBNull(modifiedOnOrdinal)
                                                          ? DateTime.MinValue
                                                          : reader.GetDateTime(modifiedOnOrdinal),
                                                  ModifiedBy =
                                                      reader.IsDBNull(modifiedByOrdinal)
                                                          ? -1
                                                          : reader.GetInt32(modifiedByOrdinal)
                                              };

                        if (usePrimaryLabelFriendlyName && !reader.IsDBNull(labelOrdinal))
                        {
                            address.friendlyName = reader.GetString(labelOrdinal);
                        }

                        addresses.Add(address);
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return addresses;
        }


        /// <summary>
        ///     Retrieve an address from the global database
        /// </summary>
        /// <param name="currentUser">The currentuser object for accountID etc</param>
        /// <param name="globalIdentifier">A global identifier to retrieve the address by (capture plus)</param>
        /// <param name="countries">The countries object, needed for resolving the country's Alpha3 code and name from the ID</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>An address object populated from the database entry or null if the address cannot be found</returns>
        private static Address GetFromGlobalDatabase(
            ICurrentUserBase currentUser,
            string globalIdentifier,
            IGlobalCountries countries,
            IDBConnection connection = null)
        {
            Address address = null;
            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          GlobalVariables.GlobalAddressDatabaseConnectionString))
            {
                const string SQL =
                    "SELECT a.[AddressID], a.[Postcode], a.[AddressName], a.[Line1], a.[Line2], a.[Line3], a.[City], a.[County], a.[Country], a.[LookupDate], a.[ProviderIdentifier], a.[Udprn], a.[Longitude], a.[Latitude], a.[CreatedOn] FROM [dbo].[addresses] a WHERE a.[ProviderIdentifier] = @identifier";
                databaseConnection.AddWithValue("@identifier", globalIdentifier, 50);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        line2Ordinal = reader.GetOrdinal("Line2"),
                        line3Ordinal = reader.GetOrdinal("Line3"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        countyOrdinal = reader.GetOrdinal("County"),
                        countryOrdinal = reader.GetOrdinal("Country"),
                        lookupDateOrdinal = reader.GetOrdinal("LookupDate"),
                        providerIdentifierOrdinal = reader.GetOrdinal("ProviderIdentifier"),
                        longitudeOrdinal = reader.GetOrdinal("Longitude"),
                        latitudeOrdinal = reader.GetOrdinal("Latitude"),
                        udprnOrdinal = reader.GetOrdinal("Udprn");

                    while (reader.Read())
                    {
                        address = new Address
                                      {
                                          Identifier = 0,
                                          Postcode =
                                              reader.IsDBNull(postcodeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(postcodeOrdinal),
                                          AddressName =
                                              reader.IsDBNull(addressNameOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(addressNameOrdinal),
                                          Line1 =
                                              reader.IsDBNull(line1Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line1Ordinal),
                                          Line2 =
                                              reader.IsDBNull(line2Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line2Ordinal),
                                          Line3 =
                                              reader.IsDBNull(line3Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line3Ordinal),
                                          City =
                                              reader.IsDBNull(cityOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(cityOrdinal),
                                          County =
                                              reader.IsDBNull(countyOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countyOrdinal),
                                          Country =
                                              reader.IsDBNull(countryOrdinal)
                                                  ? 0
                                                  : reader.GetInt32(countryOrdinal),
                                          LookupDate =
                                              reader.IsDBNull(lookupDateOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(lookupDateOrdinal),
                                          GlobalIdentifier =
                                              reader.IsDBNull(providerIdentifierOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(providerIdentifierOrdinal),
                                          Longitude =
                                              reader.IsDBNull(longitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(longitudeOrdinal),
                                          Latitude =
                                              reader.IsDBNull(latitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(latitudeOrdinal),
                                          Udprn =
                                              reader.IsDBNull(udprnOrdinal) ? 0 : reader.GetInt32(udprnOrdinal),
                                          CreationMethod = AddressCreationMethod.CapturePlus,
                                          Archived = false,
                                          SubAccountIdentifier = currentUser.CurrentSubAccountId,
                                          CreatedOn = DateTime.UtcNow,
                                          CreatedBy = currentUser.EmployeeID,
                                          ModifiedOn = DateTime.MinValue,
                                          ModifiedBy = -1
                                      };

                        cGlobalCountry country = countries.getGlobalCountryById(address.Country);
                        address.CountryName = country.Country;
                        address.CountryAlpha3Code = country.Alpha3CountryCode;

                        address.Identifier = address.Save(currentUser);
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }

            }

            return address;
        }

        /// <summary>
        ///     Retrieve an address from the "save" details that failed due to a duplicate entry in the database
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="addressIdentifier">The identity of the address being edited or 0 for adding a new address</param>
        /// <param name="addressName">The name of the address if available, generally a company name</param>
        /// <param name="line1">The first line of an address, generally a number and street</param>
        /// <param name="city">The city the address is in</param>
        /// <param name="country">The identifier of the country from the global countries</param>
        /// <param name="postcode">The postal code for the address</param>
        /// <param name="creationMethod">How was the address created</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>An address object populated from the database entry or null if the address cannot be found</returns>
        private static Address GetDuplicateFromDatabase(
            ICurrentUserBase currentUser,
            int addressIdentifier,
            string addressName,
            string line1,
            string city,
            int country,
            string postcode,
            AddressCreationMethod creationMethod = AddressCreationMethod.None,
            IDBConnection connection = null)
        {
            Address address = null;

            using (
                IDBConnection databaseConnection = connection
                                                   ?? new DatabaseConnection(
                                                          cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.AddWithValue("@Postcode", postcode, 32);
                databaseConnection.AddWithValue("@AddressName", addressName, 250);
                databaseConnection.AddWithValue("@Line1", line1, 256);
                databaseConnection.AddWithValue("@City", city, 256);
                databaseConnection.AddWithValue("@Country", country);
                databaseConnection.AddWithValue("@CreationMethod", (int)creationMethod);

                using (
                    IDataReader reader = databaseConnection.GetReader(
                        "GetAddressFromFailedDuplicateCheck",
                        CommandType.StoredProcedure))
                {
                    int identifierOrdinal = reader.GetOrdinal("AddressID"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        line2Ordinal = reader.GetOrdinal("Line2"),
                        line3Ordinal = reader.GetOrdinal("Line3"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        countyOrdinal = reader.GetOrdinal("County"),
                        countryOrdinal = reader.GetOrdinal("Country"),
                        countryNameOrdinal = reader.GetOrdinal("CountryName"),
                        countryAlpha3CodeOrdinal = reader.GetOrdinal("Alpha3CountryCode"),
                        archivedOrdinal = reader.GetOrdinal("Archived"),
                        creationMethodOrdinal = reader.GetOrdinal("CreationMethod"),
                        lookupDateOrdinal = reader.GetOrdinal("LookupDate"),
                        subAccountIdentifierOrdinal = reader.GetOrdinal("SubAccountID"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                        longitudeOrdinal = reader.GetOrdinal("Longitude"),
                        latitudeOrdinal = reader.GetOrdinal("Latitude"),
                        udprnOrdinal = reader.GetOrdinal("Udprn"),
                        accountWideFavouriteOrdinal = reader.GetOrdinal("AccountWideFavourite"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy"),
                        modifiedOnOrdinal = reader.GetOrdinal("ModifiedOn"),
                        modifiedByOrdinal = reader.GetOrdinal("ModifiedBy");

                    if (reader.Read())
                    {
                        address = new Address
                                      {
                                          Identifier = reader.GetInt32(identifierOrdinal),
                                          Postcode =
                                              reader.IsDBNull(postcodeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(postcodeOrdinal),
                                          AddressName =
                                              reader.IsDBNull(addressNameOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(addressNameOrdinal),
                                          Line1 =
                                              reader.IsDBNull(line1Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line1Ordinal),
                                          Line2 =
                                              reader.IsDBNull(line2Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line2Ordinal),
                                          Line3 =
                                              reader.IsDBNull(line3Ordinal)
                                                  ? string.Empty
                                                  : reader.GetString(line3Ordinal),
                                          City =
                                              reader.IsDBNull(cityOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(cityOrdinal),
                                          County =
                                              reader.IsDBNull(countyOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countyOrdinal),
                                          Country =
                                              reader.IsDBNull(countryOrdinal)
                                                  ? 0
                                                  : reader.GetInt32(countryOrdinal),
                                          CountryName =
                                              reader.IsDBNull(countryNameOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countryNameOrdinal),
                                          CountryAlpha3Code =
                                              reader.IsDBNull(countryAlpha3CodeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(countryAlpha3CodeOrdinal),
                                          Archived =
                                              reader.IsDBNull(archivedOrdinal)
                                                  ? false
                                                  : reader.GetBoolean(archivedOrdinal),
                                          CreationMethod =
                                              reader.IsDBNull(creationMethodOrdinal)
                                                  ? AddressCreationMethod.None
                                                  : (AddressCreationMethod)reader.GetInt32(creationMethodOrdinal),
                                          LookupDate =
                                              reader.IsDBNull(lookupDateOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(lookupDateOrdinal),
                                          SubAccountIdentifier =
                                              reader.IsDBNull(subAccountIdentifierOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(subAccountIdentifierOrdinal),
                                          GlobalIdentifier =
                                              reader.IsDBNull(globalIdentifierOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(globalIdentifierOrdinal),
                                          Longitude =
                                              reader.IsDBNull(longitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(longitudeOrdinal),
                                          Latitude =
                                              reader.IsDBNull(latitudeOrdinal)
                                                  ? string.Empty
                                                  : reader.GetString(latitudeOrdinal),
                                          Udprn =
                                              reader.IsDBNull(udprnOrdinal) ? 0 : reader.GetInt32(udprnOrdinal),
                                          AccountWideFavourite = reader.GetBoolean(accountWideFavouriteOrdinal),
                                          CreatedOn =
                                              reader.IsDBNull(createdOnOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(createdOnOrdinal),
                                          CreatedBy =
                                              reader.IsDBNull(createdByOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(createdByOrdinal),
                                          ModifiedOn =
                                              reader.IsDBNull(modifiedOnOrdinal)
                                                  ? DateTime.MinValue
                                                  : reader.GetDateTime(modifiedOnOrdinal),
                                          ModifiedBy =
                                              reader.IsDBNull(modifiedByOrdinal)
                                                  ? -1
                                                  : reader.GetInt32(modifiedByOrdinal)
                                      };
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return address;
        }

        /// <summary>
        /// Gets an address distance lookup locator (postcode or lat/long pair)
        /// </summary>
        /// <param name="accountUsesInternationalLookups">The account option flag for international and lat/long lookups</param>
        /// <param name="lookupProvider">The account's lookup provider</param>
        /// <returns>Either the postcode or comma separated lat/long</returns>
        public string GetLookupLocator(bool accountUsesInternationalLookups, AddressLookupProvider lookupProvider)
        {
            LocatorType locator = this.GetLocatorType(accountUsesInternationalLookups, lookupProvider);

            switch (locator)
            {
                case LocatorType.Postcode:
                    return this.Postcode;
                case LocatorType.LatLongs:
                    return this.Latitude + "," + this.Longitude;
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Gets an address lookup locator type (postcode or lat/long pair)
        /// </summary>
        /// <param name="accountUsesInternationalLookups">The account option flag for international and lat/long lookups</param>
        /// <param name="lookupProvider">The account's lookup provider</param>
        /// <returns>Either the postcode or comma separated lat/long</returns>
        public LocatorType GetLocatorType(bool accountUsesInternationalLookups, AddressLookupProvider lookupProvider)
        {
            // determine whether to use postcodes or long/lat pairs for distance lookups
            // use lat/long if there are any lat/longs and the account option for international lookups is enabled, but not if the account uses Teleatlas and both addresses are in the UK
            var isCountryOtherThanGbr = this.CountryAlpha3Code != "GBR" && !string.IsNullOrEmpty(this.CountryAlpha3Code);

            if (string.IsNullOrEmpty(this.Latitude) == false && string.IsNullOrEmpty(this.Longitude) == false
                && accountUsesInternationalLookups
                && ((lookupProvider == AddressLookupProvider.Teleatlas && isCountryOtherThanGbr)
                    || (lookupProvider == AddressLookupProvider.Paf
                        || lookupProvider == AddressLookupProvider.AddressBase)))
            {
                return LocatorType.LatLongs;
            }

            if (!isCountryOtherThanGbr)
            {
                //if not using international, then we might as well use the postcode
                //if the country *might* be UK 
                //(it's either GBR or blank - we don't know for a fact that it's something else)
                return LocatorType.Postcode;
            }

            return LocatorType.None;
        }

        #endregion Methods

        /// <summary>
        /// The column to search on for a Get operation
        /// </summary>
        private class SearchFieldAndIdentifiers
        {
            #region Fields

            private readonly SearchField _field = SearchField.None;

            private readonly int _intIdentifier;

            private readonly SqlDbType _sqlType = SqlDbType.Int;

            private readonly string _stringIdentifier = string.Empty;

            #endregion Fields

            #region Constructors and Destructors

            /// <summary>
            /// Get an address by specifying the AddressID column
            /// </summary>
            /// <param name="addressIdentifier">The address to Get</param>
            public SearchFieldAndIdentifiers(int addressIdentifier)
            {
                if (addressIdentifier <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "addressIdentifier",
                        addressIdentifier,
                        "The argument must be greater than 0.");
                }

                this._field = SearchField.AddressID;
                this._intIdentifier = addressIdentifier;
                this._sqlType = SqlDbType.Int;
            }

            /// <summary>
            /// Get an address by specifying the GlobalIdentifier column
            /// </summary>
            /// <param name="globalIdentifier">The address to Get</param>
            public SearchFieldAndIdentifiers(string globalIdentifier)
            {
                if (string.IsNullOrWhiteSpace(globalIdentifier))
                {
                    throw new ArgumentOutOfRangeException(
                        "globalIdentifier",
                        globalIdentifier,
                        "The argument must contain a value that is not whitespace.");
                }

                this._field = SearchField.GlobalIdentifier;
                this._stringIdentifier = globalIdentifier;
                this._sqlType = SqlDbType.NVarChar;
            }

            #endregion Constructors and Destructors

            #region Enums

            /// <summary>
            /// The column name to look for the id in 
            /// these values must match the column name exactly
            /// </summary>
            public enum SearchField
            {
                None = 0,

                AddressID = 1,

                GlobalIdentifier = 2
            }

            #endregion Enums

            #region Public Properties

            /// <summary>
            /// The column to find the identifier in
            /// </summary>
            public SearchField Field
            {
                get
                {
                    return this._field;
                }
            }

            /// <summary>
            /// The address identifier to look for in the Field, used in a SqlCommand AddWithValue
            /// </summary>
            public object Identifier
            {
                get
                {
                    switch (this._field)
                    {
                        case SearchField.None:
                            return string.Empty;

                        case SearchField.AddressID:
                            return this._intIdentifier;

                        case SearchField.GlobalIdentifier:
                            return this._stringIdentifier;
                    }

                    throw new FieldAccessException("The value for the field type has become invalid.");
                }
            }

            /// <summary>
            /// The SqlDbType that corresponds to the Identifier
            /// </summary>
            public SqlDbType SqlType
            {
                get
                {
                    return this._sqlType;
                }
            }

            #endregion Public Properties
        }

        /// <summary>
        /// Gets the ID of an existing address from the database.
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="line1">THe Line1 field of the address to retrieve</param>
        /// <param name="city">The city (only used in case the postcode is not specified)</param>
        /// <param name="postcode">Postcode of the address to retrieve</param>
        /// <param name="country">The country code</param>
        /// <returns>THe AddressID of the existing address</returns>
        public static int Find(
            ICurrentUserBase currentUser,
            string addressName,
            string line1,
            string city,
            string postcode,
            int country)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser", "The value of this argument must not be null.");
            }

            using (
                IDBConnection dbConnection = new DatabaseConnection(
                    cAccounts.getConnectionString(currentUser.AccountID)))
            {
                dbConnection.AddWithValue("@AddressName", addressName);
                dbConnection.AddWithValue("@Line1", line1);
                dbConnection.AddWithValue("@Country", country);
                if (string.IsNullOrEmpty(postcode))
                {
                    dbConnection.AddWithValue("@Postcode", postcode);
                    return
                        dbConnection.ExecuteScalar<int>(
                            "select AddressID from Addresses where AddressName = @AddressName and Line1 = @Line1 and Postcode = @Postcode and Country = @Country;");
                }
                else
                {
                    dbConnection.AddWithValue("@City", city);
                    return
                        dbConnection.ExecuteScalar<int>(
                            "select AddressID from Addresses where AddressName = @AddressName and Line1 = @Line1 and City = @City and Country = @Country;");
                }
            }
        }
    }
}

       
    