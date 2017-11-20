namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    public class AddressLabels : IDisposable
    {
        #region Constants

        public const string GridSql = "SELECT [AddressLabelID], [Label], [IsPrimary] FROM [dbo].[addressLabels]";
        public const string GridApi = "SELECT [AddressLabelID], [AddressID], [Label], [IsPrimary] FROM [dbo].[addressLabels]";

        #endregion

        #region Fields

        private readonly int accountIdentifier;
        private readonly List<AddressLabel> labels = new List<AddressLabel>();
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor which sets the internal account identifier
        /// </summary>
        /// <param name="accountIdentifier">Account identifier for use in database connection string retrieval, etc</param>
        public AddressLabels(int accountIdentifier)
        {
            if (accountIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value for the account identifier should be zero or greater.");
            }

            this.accountIdentifier = accountIdentifier;
        }

        /// <summary>
        /// Destructor/finaliser for the garbage collector that doesn't call dispose on related sub-assets
        /// </summary>
        ~AddressLabels()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators


        /// <summary>
        /// Gets a list of all the Account wide labels for an Address Id.
        /// </summary>
        /// <param name="addressId">The Id of the Address.</param>
        /// <param name="connection">an options IDBconnection (for testing).</param>
        /// <returns></returns>
        public List<AddressLabel> GetAccountWideLabelsForAddress(int addressId, IDBConnection connection = null)
        {
            const string sql = GridApi + " WHERE AddressID = @addressId AND EmployeeID IS NULL;";
            var matchingLabels = new List<AddressLabel>();

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountIdentifier)))
            {
                databaseConnection.AddWithValue("@addressId", addressId);

                using (var reader = databaseConnection.GetReader(sql))
                {
                    int addressLabelIDOrdinal = reader.GetOrdinal("AddressLabelID"),
                        addressIDOrdinal = reader.GetOrdinal("AddressID"),
                        labelOrdinal = reader.GetOrdinal("Label"),
                        labelPrimaryOrdinal = reader.GetOrdinal("IsPrimary");

                    while (reader.Read())
                    {
                        matchingLabels.Add(new SpendManagementLibrary.Addresses.AddressLabel
                        {
                            AddressLabelID = reader.GetInt32(addressLabelIDOrdinal),
                            AddressID = reader.GetInt32(addressIDOrdinal),
                            Text = reader.IsDBNull(labelOrdinal) ? string.Empty : reader.GetString(labelOrdinal),
                            Primary = reader.GetBoolean(labelPrimaryOrdinal)
                        });
                    }
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return matchingLabels;
        }

        /// <summary>
        /// Save new address label to an existing address. If globalIdentifier cannot be parsed as an int then create a new address for the label.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="identifier">Can be an AddressID or a Postcode Anywhere ID</param>
        /// <param name="labelText">The text for the new label</param>
        /// <param name="countries"></param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The positive identifier of the saved label or a negative return code</returns>
        public static int SaveAddressLabel(ICurrentUserBase currentUser, string identifier, string labelText, IGlobalCountries countries, IDBConnection connection = null)
        {
            int addressID;
            if (int.TryParse(identifier, out addressID)) //globalIdentifier is an int then save label against existing address
            {
                return SaveLabelForExistingAddress(currentUser, labelText, addressID, connection);
            }

            //globalIdentifier must be a Postcode Anywhere ID, create a new Address for this label
            return GetNewAddressAndSaveLabel(currentUser, labelText, identifier, countries, connection);
        }

        /// <summary>
        ///     Delete an address label by its identifier
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="identifier">The AddressLabel identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A positive value indicating success or a negative return code</returns>
        public int DeleteAddressLabel(ICurrentUserBase currentUser, int identifier, IDBConnection connection = null)
        {
            if (identifier <= 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AddressLabelID", identifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("DeleteAddressLabel");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// The dispose method that can be called directly, will call dispose on any related resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this); // Supress Garbage Collector
        }

        /// <summary>
        /// Searches the customer database for any labels that match the given search term.
        /// </summary>
        /// <param name="searchTerm">The string to search for</param>
        /// <param name="addressFilter">Filter options</param>
        /// <param name="employeeID">Optional Employee identifier for the filter AddressFilter.Employee</param>
        /// <param name="connection">Optional Database connection override, usually for unit testing</param>
        /// <returns>A list of matching labels</returns>
        public List<AddressLabel> Search(string searchTerm, AddressFilter addressFilter, int employeeID = 0, IDBConnection connection = null)
        {
            if (this.labels.Count == 0)
            {
                this.GetSearchResults(searchTerm, connection);
            }

            return this.GetFilteredResults(addressFilter, employeeID);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Full explanation of this disposal technique can be found here: http://lostechies.com/chrispatterson/2012/11/29/idisposable-done-right/
        /// </summary>
        /// <param name="disposing">Has Dispose() been called? (otherwise it is the GC finalising)</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (this.disposed)
            {
                return;
            }

            if (disposing) // If disposing equals true, dispose all managed resources that also implement IDisposable
            {
                foreach (AddressLabel addressLabel in this.labels)
                {
                    //addressLabel.dispose();
                }
            }

            // Set any unmanaged object references to null.   
            this.labels.Clear();

            this.disposed = true; // Disposing has been done.
        }

        /// <summary>
        /// Gets an address by its global identifier and then saves a label for it
        /// </summary>
        /// <param name="currentUser">The current user object to pass through</param>
        /// <param name="labelText">The label to save</param>
        /// <param name="globalIdentifier">The global identifier from capture plus</param>
        /// <param name="countries">The global countries management object</param>
        /// <param name="connection">The connection interface object override, usually used for unit tests</param>
        /// <returns>The positive identifier of the saved label or a negative return code</returns>
        private static int GetNewAddressAndSaveLabel(ICurrentUserBase currentUser, string labelText, string globalIdentifier, IGlobalCountries countries, IDBConnection connection)
        {
            Address address = Address.Get(currentUser, globalIdentifier, countries);

            if (address != null)
            {
                return SaveLabelForExistingAddress(currentUser, labelText, address.Identifier, connection);
            }

            return -1;
        }

        /// <summary>
        /// Save a label for a specified address
        /// </summary>
        /// <param name="currentUser">The currentuser object used for employee, delegate and account information</param>
        /// <param name="labelText">The label</param>
        /// <param name="addressID">The address to save the label against</param>
        /// <param name="connection">The connection interface object override, usually used for unit tests</param>
        /// <returns>A positive label identifier on success or a negative return code on error</returns>
        private static int SaveLabelForExistingAddress(ICurrentUserBase currentUser, string labelText, int addressID, IDBConnection connection)
        {
            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@EmployeeID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@AddressID", addressID);
                databaseConnection.AddWithValue("@Label", labelText);
                databaseConnection.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);
                databaseConnection.AddWithValue("@IsPrimary", false);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveAddressLabel");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// Return a list of labels of the appropriate type(s)
        /// </summary>
        /// <param name="addressFilter">The address label type to return</param>
        /// <param name="employeeID">The employee to get labels for</param>
        /// <returns>A list of address labels of the type(s) specified</returns>
        public List<AddressLabel> GetFilteredResults(AddressFilter addressFilter, int employeeID)
        {
            List<AddressLabel> filteredResults = new List<AddressLabel>();

            switch (addressFilter)
            {
                case AddressFilter.All:
                    filteredResults = this.labels;
                    break;

                case AddressFilter.AccountWide:
                case AddressFilter.Employee:
                    filteredResults.AddRange(this.labels.Where(al => al.EmployeeID == employeeID));
                    break;
            }

            return filteredResults;
        }

        /// <summary>
        /// Search for address labels
        /// </summary>
        /// <param name="searchTerm">The search term to look for in labels</param>
        /// <param name="connection">The connection interface object override, usually used for unit testing</param>
        private void GetSearchResults(string searchTerm, IDBConnection connection)
        {
            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountIdentifier)))
            {
                databaseConnection.AddWithValue("searchTerm", searchTerm, 250);

                using (IDataReader reader = databaseConnection.GetReader("SearchAddressesLabels", CommandType.StoredProcedure))
                {
                    int addressLabelIDOrdinal = reader.GetOrdinal("AddressLabelID"),
                        employeeIDOrdinal = reader.GetOrdinal("EmployeeID"),
                        addressIDOrdinal = reader.GetOrdinal("AddressID"),
                        labelOrdinal = reader.GetOrdinal("Label"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                        addressLine1Ordinal = reader.GetOrdinal("Line1"),
                        addressCityOrdinal = reader.GetOrdinal("City"),
                        addressPostcodeOrdinal = reader.GetOrdinal("Postcode"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        labelPrimaryOrdinal = reader.GetOrdinal("IsPrimary");

                    while (reader.Read())
                    {
                        string addressLine1 = reader.GetString(addressLine1Ordinal);
                        string addressPostcode = reader.IsDBNull(addressPostcodeOrdinal) ? string.Empty : reader.GetString(addressPostcodeOrdinal);
                        string addressName = reader.IsDBNull(addressNameOrdinal) ? string.Empty : reader.GetString(addressNameOrdinal);
                        string addressCity = reader.IsDBNull(addressCityOrdinal) ? string.Empty : reader.GetString(addressCityOrdinal);

                        this.labels.Add(new AddressLabel
                        {
                            AddressLabelID = reader.GetInt32(addressLabelIDOrdinal),
                            EmployeeID = reader.IsDBNull(employeeIDOrdinal) ? 0 : reader.GetInt32(employeeIDOrdinal),
                            AddressID = reader.GetInt32(addressIDOrdinal),
                            Text = reader.IsDBNull(labelOrdinal) ? string.Empty : reader.GetString(labelOrdinal),
                            GlobalIdentifier = reader.IsDBNull(globalIdentifierOrdinal) ? string.Empty : reader.GetString(globalIdentifierOrdinal),
                            AddressFriendlyName = Address.GenerateFriendlyName(addressName, addressLine1, addressPostcode, addressCity),
                            Primary = reader.GetBoolean(labelPrimaryOrdinal)
                        });
                    }
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        #endregion
    }
}
