namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    [Serializable]
    public class Favourite
    {
        public int FavouriteID { get; set; }
        public int? EmployeeID { get; set; }
        public int AddressID { get; set; }
        public string GlobalIdentifier { get; set; }
        public string AddressFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the favourite is account wide.
        /// </summary>
        public bool IsAccountWide { get; set; }

        #region Public Methods and Operators

        /// <summary>
        ///     Delete an address favourite by its identifier
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="favouriteId">The favourite identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A positive value indicating success or a negative return code</returns>
        public static int Delete(ICurrentUserBase currentUser, int favouriteId, IDBConnection connection = null)
        {
            if (favouriteId <= 0)
            {
                throw new ArgumentOutOfRangeException("favouriteId", favouriteId, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@FavouriteID", favouriteId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("DeleteFavourite");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        ///     Save a personal address favourite
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="addressIdentifier">The identity of the address being edited or 0 for adding a new address</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The positive identifier of the saved address or a negative return code</returns>
        public static int Save(ICurrentUserBase currentUser, int addressIdentifier, IDBConnection connection = null)
        {
            if (addressIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than (if editing) or equal to 0 (if saving new).");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@FavouriteID", 0); // todo this isn't necessary, the favourite will never be updated, see sproc
                databaseConnection.AddWithValue("@EmployeeID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);
                databaseConnection.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveFavourite");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        #endregion Public Methods and Operators
    }
}