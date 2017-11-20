namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    public class Favourites
    {
        //Constructor
        public Favourites(int accountIdentifier)
        {
            if (accountIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value for the account identifier should be zero or greater.");
            }

            this.accountIdentifier = accountIdentifier;
        }

        private readonly int accountIdentifier;
        private readonly List<Favourite> favourites = new List<Favourite>();
        private bool disposed = false;

        public bool Disposed { get { return disposed; } }

        /// <summary>
        /// Get a list of favourites
        /// </summary>
        /// <param name="addressFilter">Filter options</param>
        /// <param name="employeeID">Optional Employee identifier for the filter AddressFilter.Employee</param>
        /// <param name="connection">Optional Database connection override, usually for unit testing</param>
        /// <returns>A list of Favourites</returns>
        #region Public Methods and Operators
        public List<Favourite> Get(ICurrentUserBase currentUser, AddressFilter addressFilter, int employeeID = 0, IDBConnection connection = null)
        {
            if (this.favourites.Count == 0)
            {
                this.GetFavouritesFromDatabase(currentUser, connection);
            }

            return this.GetFilteredResults(addressFilter, employeeID);
        }
        #endregion

        #region Private Methods and Operators

        private void GetFavouritesFromDatabase(ICurrentUserBase currentUser, IDBConnection connection)
        {
            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountIdentifier)))
            {
                using (IDataReader reader = databaseConnection.GetReader("GetFavourites", CommandType.StoredProcedure))
                {
                    int favouriteIDOrdinal = reader.GetOrdinal("FavouriteID"),
                        employeeIDOrdinal = reader.GetOrdinal("EmployeeID"),
                        addressIDOrdinal = reader.GetOrdinal("AddressID"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        cityOrdinal = reader.GetOrdinal("City");

                    while (reader.Read())
                    {
                        this.favourites.Add(new Favourite
                        {
                            FavouriteID = reader.GetInt32(favouriteIDOrdinal),
                            EmployeeID = reader.IsDBNull(employeeIDOrdinal) ? 0 : reader.GetInt32(employeeIDOrdinal),
                            AddressID = reader.GetInt32(addressIDOrdinal),
                            GlobalIdentifier = reader.IsDBNull(globalIdentifierOrdinal) ? string.Empty : reader.GetString(globalIdentifierOrdinal),
                            AddressFriendlyName = Address.GenerateFriendlyName(
                                reader.IsDBNull(addressNameOrdinal) ? string.Empty : reader.GetString(addressNameOrdinal),
                                reader.IsDBNull(line1Ordinal) ? string.Empty : reader.GetString(line1Ordinal),
                                reader.IsDBNull(postcodeOrdinal) ? string.Empty : reader.GetString(postcodeOrdinal),
                                reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal)
                            )
                        });
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }

                using (IDataReader reader = databaseConnection.GetReader("SELECT AddressID, AddressName, Line1, Postcode, City, GlobalIdentifier FROM [Addresses] WHERE AccountWideFavourite = 1 AND Archived = 0;"))
                {
                    int addressIdOrdinal = reader.GetOrdinal("AddressID"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        globalIdentifierOrdinal = reader.GetOrdinal("GlobalIdentifier");

                    while (reader.Read())
                    {
                        this.favourites.Add(new Favourite
                        {
                            AddressID = reader.GetInt32(addressIdOrdinal),
                            EmployeeID = 0,
                            GlobalIdentifier = reader.IsDBNull(globalIdentifierOrdinal) ? string.Empty : reader.GetString(globalIdentifierOrdinal),
                            AddressFriendlyName = Address.GenerateFriendlyName(
                                reader.IsDBNull(addressNameOrdinal) ? string.Empty : reader.GetString(addressNameOrdinal),
                                reader.IsDBNull(line1Ordinal) ? string.Empty : reader.GetString(line1Ordinal),
                                reader.IsDBNull(postcodeOrdinal) ? string.Empty : reader.GetString(postcodeOrdinal),
                                reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal)
                            ),
                            IsAccountWide = true
                        });
                    }

                    reader.Close();
                }
            }
        }

        private List<Favourite> GetFilteredResults(AddressFilter addressFilter, int employeeID = 0)
        {
            var filteredResults = new List<Favourite>();

            switch (addressFilter)
            {
                case AddressFilter.All:
                    filteredResults = this.favourites;
                    break;
                case AddressFilter.AccountWide:
                    filteredResults.AddRange(this.favourites.Where(f => f.IsAccountWide));
                    break;
                case AddressFilter.Employee:
                    filteredResults.AddRange(this.favourites.Where(f => f.EmployeeID == employeeID));
                    break;
            }

            return filteredResults;
        }

        #endregion

        #region Object Disposal
        // Full explanation of this disposal technique can be found here: http://lostechies.com/chrispatterson/2012/11/29/idisposable-done-right/

        ~Favourites()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this); // Supress Garbage Collector
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                if (disposing) // If disposing equals true, dispose all managed resources that also implement IDisposable
                {
                    foreach (var favourite in favourites)
                    {
                        //favourite.dispose();
                    }
                }

                // Set any unmanaged object references to null.   
                favourites.Clear();

                this.disposed = true; // Disposing has been done.
            }
        }

        #endregion
    }
}
