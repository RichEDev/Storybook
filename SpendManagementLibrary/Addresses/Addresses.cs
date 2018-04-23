namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;

    using Helpers;
    using Interfaces;

    using SpendManagementLibrary.Helpers.AuditLogger;

    public class Addresses
    {
        #region Constants

        public const string CacheArea = "Addresses";

        public const string GridSql = "SELECT [AddressID], [AddressName], [Line1], [City], [Postcode], [Archived], [CreationMethod], [AccountWideFavourite] FROM [dbo].[Addresses]";

        #endregion Constants

        #region Fields

        private int _accountIdentifier;

        #endregion Fields

        #region Constructors and Destructors

        public Addresses(int accountIdentifier)
        {
            if (accountIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value for the account identifier should be zero or greater.");
            }

            this._accountIdentifier = accountIdentifier;
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        /// <summary>
        /// Gets all the Addresses in the Customer Address table.
        /// Currently only used by the API.
        /// </summary>
        /// <returns>A List of Address.</returns>
        public List<Address> All(IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountIdentifier)))
            {
                var matchedAddresses = new List<Address>();
                
                using (var reader = databaseConnection.GetReader("GetAllAddresses", CommandType.StoredProcedure))
                {
                    matchedAddresses = ReadAndBuildAddresses(reader, matchedAddresses);
                }

                return matchedAddresses;
            }
        }

        /// <summary>Gets all the Addresses in the Customer Address table. Currently only used by the API.</summary>
        /// <param name="id">The Id of the Address.</param>
        /// <param name="connection">A concrete instance of IDBConnection (for testing).</param>
        /// <returns>A List of Address.</returns>
        public Address GetAddressById(int id, IDBConnection connection = null)
        {
            if (id <= 0)
            {
                throw new ArgumentException("The Id must be a positive integer.");
            }

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressID", id);
                var matchedAddresses = new List<Address>();
                
                using (var reader = databaseConnection.GetReader("GetAddressById", CommandType.StoredProcedure))
                {
                    matchedAddresses = ReadAndBuildAddresses(reader, matchedAddresses);
                }

                databaseConnection.sqlexecute.Parameters.Clear();
                return matchedAddresses.FirstOrDefault();
            }
        }

        /// <summary>
        /// Searches the customer database for any address which match the given search term.
        /// </summary>
        /// <param name="searchCriteria">The string to search for</param>
        /// <param name="globalCountryId">The database ID of the global country</param>
        /// <param name="displayEsrAddresses">Whether or not to include ESR imported addresses in the search results</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>A list of matching addresses</returns>
        public List<Address> Search(string searchCriteria, int globalCountryId, bool displayEsrAddresses = false, IDBConnection connection = null)
        {
            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountIdentifier)))
            {
                var matchedAddresses = new List<Address>();

                databaseConnection.AddWithValue("searchCriteria", searchCriteria, 250);
                databaseConnection.AddWithValue("globalCountryId", globalCountryId);
                databaseConnection.AddWithValue("displayEsrAddresses", displayEsrAddresses);

                using (IDataReader reader = databaseConnection.GetReader("SearchAddresses", CommandType.StoredProcedure))
                {
                    matchedAddresses = ReadAndBuildAddresses(reader, matchedAddresses);
                }

                return matchedAddresses;
            }
        }

        /// <summary>
        /// Searches the customer database for any address which match the given search term and returns the entire resultset.
        /// </summary>
        /// <param name="searchCriteria">The string to search for</param>
        /// <param name="globalCountryId">The database ID of the global country</param>
        /// <param name="displayEsrAddresses">Whether or not to include ESR imported addresses in the search results</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>A list of matching addresses</returns>
        public List<Address> ApiSearch(string searchCriteria, int globalCountryId, bool displayEsrAddresses, IDBConnection connection = null)
        {
            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountIdentifier)))
            {
                var matchedAddresses = new List<Address>();

                databaseConnection.AddWithValue("searchCriteria", searchCriteria, 250);
                databaseConnection.AddWithValue("globalCountryId", globalCountryId);
                databaseConnection.AddWithValue("displayEsrAddresses", displayEsrAddresses);

                using (IDataReader reader = databaseConnection.GetReader("ApiSearchAddresses", CommandType.StoredProcedure))
                {
                    matchedAddresses = ReadAndBuildAddresses(reader, matchedAddresses);
                }

                return matchedAddresses;
            }
        }

        /// <summary>
        /// Audits the viewing of an address
        /// </summary>
        /// <param name="addressLine1">The first line of the address to audit</param>
        /// <param name="city">The city of the address to audit</param>
        /// <param name="postcode">The postcode of the address to audit</param>
        /// <param name="user">The current user</param>
        public void AuditViewAddress(string addressLine1, string city, string postcode, ICurrentUserBase user)
        {
            AuditLogger auditLog = new AuditLogger();
            auditLog.ViewRecordAuditLog(user, SpendManagementElement.Addresses, $"{addressLine1}, {city}, {postcode}");
        }

        /// <summary>
        /// Audits the viewing of an address
        /// </summary>
        /// <param name="addressFriendlyName">The friendly name of the address to audit</param>
        /// <param name="user">The current user</param>
        public void AuditViewAddress(string addressFriendlyName, ICurrentUserBase user)
        {
            AuditLogger auditLog = new AuditLogger();
            auditLog.ViewRecordAuditLog(user, SpendManagementElement.Addresses, $"{addressFriendlyName}");
        }

        private static List<Address> ReadAndBuildAddresses(IDataReader reader, List<Address> matchedAddresses)
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
                archivedOrdinal = reader.GetOrdinal("Archived"),
                creationMethodOrdinal = reader.GetOrdinal("CreationMethod"),
                lookupDateOrdinal = reader.GetOrdinal("LookupDate"),
                subAccountIdentifierOrdinal = reader.GetOrdinal("SubAccountID"),
                globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                longitudeOrdinal = reader.GetOrdinal("Longitude"),
                latitudeOrdinal = reader.GetOrdinal("Latitude"),
                modifiedOnOrdinal = reader.GetOrdinal("ModifiedOn"),
                modifiedByOrdinal = reader.GetOrdinal("ModifiedBy"),
                accountWideFavouriteOrdinal = reader.GetOrdinal("AccountWideFavourite");


            while (reader.Read())
            {
                matchedAddresses.Add(new Address
                {
                    Identifier = reader.GetInt32(identifierOrdinal),
                    Postcode = reader.IsDBNull(postcodeOrdinal) ? string.Empty : reader.GetString(postcodeOrdinal),
                    AddressName = reader.IsDBNull(addressNameOrdinal) ? string.Empty : reader.GetString(addressNameOrdinal),
                    Line1 = reader.IsDBNull(line1Ordinal) ? string.Empty : reader.GetString(line1Ordinal),
                    Line2 = reader.IsDBNull(line2Ordinal) ? string.Empty : reader.GetString(line2Ordinal),
                    Line3 = reader.IsDBNull(line3Ordinal) ? string.Empty : reader.GetString(line3Ordinal),
                    City = reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal),
                    County = reader.IsDBNull(countyOrdinal) ? string.Empty : reader.GetString(countyOrdinal),
                    Country = reader.IsDBNull(countryOrdinal) ? 0 : reader.GetInt32(countryOrdinal),
                    Archived = reader.IsDBNull(archivedOrdinal) ? false : reader.GetBoolean(archivedOrdinal),
                    CreationMethod = reader.IsDBNull(creationMethodOrdinal) 
                        ? Address.AddressCreationMethod.None
                        : (Address.AddressCreationMethod) reader.GetInt32(creationMethodOrdinal),
                    LookupDate = reader.IsDBNull(lookupDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(lookupDateOrdinal),
                    SubAccountIdentifier = reader.IsDBNull(subAccountIdentifierOrdinal) ? -1 : reader.GetInt32(subAccountIdentifierOrdinal), 
                    GlobalIdentifier = reader.IsDBNull(globalIdentifierOrdinal) ? string.Empty : reader.GetString(globalIdentifierOrdinal),
                    Longitude = reader.IsDBNull(longitudeOrdinal) ? string.Empty : reader.GetString(longitudeOrdinal),
                    Latitude = reader.IsDBNull(latitudeOrdinal) ? string.Empty : reader.GetString(latitudeOrdinal),
                    ModifiedOn = reader.IsDBNull(modifiedOnOrdinal) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrdinal),
                    ModifiedBy = reader.IsDBNull(modifiedByOrdinal) ? -1 : reader.GetInt32(modifiedByOrdinal),
                    AccountWideFavourite = reader.IsDBNull(accountWideFavouriteOrdinal) ? false : reader.GetBoolean(accountWideFavouriteOrdinal)
                });
            }

            return matchedAddresses;
        }

        #endregion Public Methods and Operators
    }
}