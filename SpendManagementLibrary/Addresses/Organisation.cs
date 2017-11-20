namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Data;

    using Helpers;
    using Interfaces;

    public class PrimaryAddress
    {
        public int Identifier { get; set; }

        /// <summary>
        /// The friendly address name, this is the first line of the address and the postcode (if there is a postcode)
        /// </summary>
        public string FriendlyName { get; set; }
    }


    public class Organisation
    {
        public const int IntNull = -1;
        public static readonly DateTime DateTimeNull = DateTime.MinValue;

        public int Identifier { get; set; }

        public int? ParentOrganisationIdentifier { get; set; }

        public PrimaryAddress PrimaryAddress { get; set; }

        /// <summary>
        /// 256 char
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 4000 char
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 60 char
        /// </summary>
        public string Code { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int ModifiedBy { get; set; }

        /// <summary>
        ///     Obtain an organisation object populated with the appropriate information for the given account
        /// </summary>
        /// <param name="accountIdentifier">Account to retrieve the organisation from, must be positive</param>
        /// <param name="identifier">The organisation to retrieve, must be positive</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The populated organisation object</returns>
        public static Organisation Get(int accountIdentifier, int identifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            return Organisations.Get(accountIdentifier, identifier, connection);
        }

        /// <summary>
        ///     Save an organisation instance
        /// </summary>
        /// <returns>The positive identifier of the saved organisation or a negative return code</returns>
        public int Save(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            return Save(currentUser, this.Identifier, this.ParentOrganisationIdentifier, (this.PrimaryAddress == null ? (int?)null : this.PrimaryAddress.Identifier), this.Name, this.Comment, this.Code, connection);
        }

        /// <summary>
        ///     Save an organisation
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="identifier">The identity of the organisation being edited or 0 for adding a new organisation</param>
        /// <param name="parentOrganisationIdentifier"></param>
        /// <param name="primaryAddressIdentifier"></param>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        /// <param name="code"></param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The positive identifier of the saved organisation or a negative return code</returns>
        public static int Save(ICurrentUserBase currentUser, int identifier, int? parentOrganisationIdentifier, int? primaryAddressIdentifier, string name, string comment, string code, IDBConnection connection = null)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser", "The value of this argument must not be null.");
            }

            if (identifier < 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0 (if editing) or equal to 0 (if saving new).");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@OrganisationID", identifier);
                databaseConnection.AddWithValue("@Name", name, 256);
                databaseConnection.AddWithValue("@ParentOrganisationID", (object)parentOrganisationIdentifier ?? DBNull.Value);
                databaseConnection.AddWithValue("@PrimaryAddressID", (object)primaryAddressIdentifier ?? DBNull.Value);
                databaseConnection.AddWithValue("@Comment", (object)comment ?? DBNull.Value, 4000);
                databaseConnection.AddWithValue("@Code", (object)code ?? DBNull.Value, 60);
                databaseConnection.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value);

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveOrganisation");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// Flip the archived status of the organisation
        /// </summary>
        /// <param name="currentUser">The current user object</param>
        /// <param name="identifier">The organisation to change the archive status of</param>
        /// <param name="connection">The connection to use instead of the default, usually for unit testing</param>
        /// <returns>1 on success or -1 on error</returns>
        public static int ToggleArchive(ICurrentUserBase currentUser, int identifier, IDBConnection connection = null)
        {
            if (identifier <= 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@OrganisationID", identifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("ArchiveOrganisation");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        ///     Delete an organisation by its identifier
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="identifier">The organisation identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A zero indicating success or a negative return code</returns>
        public static int Delete(ICurrentUserBase currentUser, int identifier, IDBConnection connection = null)
        {
            if (identifier <= 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@OrganisationID", identifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("DeleteOrganisation");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }
    }
}