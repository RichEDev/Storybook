namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    [Serializable]
    public class AddressLabel
    {
        #region Public Properties

        public string AddressFriendlyName { get; set; }

        public int AddressID { get; set; }

        public int AddressLabelID { get; set; }

        public int? EmployeeID { get; set; }

        public string GlobalIdentifier { get; set; }

        public string Text { get; set; }

        public bool Primary { get; set; }

        #endregion

        //TODO Set up CRUD operations

        #region Public Methods and Operators

        /// <summary>
        /// Get a label
        /// </summary>
        /// <param name="currentUser">The current user object needed for the account</param>
        /// <param name="labelIdentifier">The label to get</param>
        /// <param name="connection">The connection interface object override, usually used for unit testing</param>
        public static AddressLabel Get(ICurrentUserBase currentUser, int labelIdentifier, IDBConnection connection = null)
        {
            AddressLabel label = null;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@AddressLabelID", labelIdentifier);

                using (IDataReader reader = databaseConnection.GetReader("SELECT [AddressLabelID], [AddressID], [EmployeeID], [Label], [IsPrimary] FROM [dbo].[addressLabels] WHERE [AddressLabelID] = @AddressLabelID"))
                {
                    int addressLabelIDOrdinal = reader.GetOrdinal("AddressLabelID"),
                        employeeIDOrdinal = reader.GetOrdinal("EmployeeID"),
                        addressIDOrdinal = reader.GetOrdinal("AddressID"),
                        labelOrdinal = reader.GetOrdinal("Label"),
                        primaryOrdinal = reader.GetOrdinal("IsPrimary");

                    while (reader.Read())
                    {
                        label = new AddressLabel
                        {
                            AddressLabelID = reader.GetInt32(addressLabelIDOrdinal),
                            EmployeeID = reader.IsDBNull(employeeIDOrdinal) ? 0 : reader.GetInt32(employeeIDOrdinal),
                            AddressID = reader.GetInt32(addressIDOrdinal),
                            Text = reader.IsDBNull(labelOrdinal) ? string.Empty : reader.GetString(labelOrdinal),
                            Primary = reader.GetBoolean(primaryOrdinal),
                            GlobalIdentifier = string.Empty,
                            AddressFriendlyName = string.Empty
                        };
                    }

                    reader.Close();
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return label;
        }

        /// <summary>
        /// Save an address label
        /// </summary>
        /// <param name="currentUser">The current user object</param>
        /// <param name="label">The text of the label</param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int Save(ICurrentUserBase currentUser, AddressLabel label, IDBConnection connection = null)
        {
            int returnValue;

            if (label == null)
            {
                throw new ArgumentNullException("label", "This argument must not be null.");
            }

            if (label.AddressID <= 0)
            {
                throw new ArgumentOutOfRangeException("label", label.AddressID, "The value of this argument's AddressID property must be greater than 0.");
            }

            if (label.AddressLabelID < 0)
            {
                throw new ArgumentOutOfRangeException("label", label.AddressLabelID, "The value of this argument's AddressLabelID property must be greater than (if editing) or equal to 0 (if saving new).");
            }

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@EmployeeID", DBNull.Value);
                databaseConnection.AddWithValue("@AddressID", label.AddressID);
                databaseConnection.AddWithValue("@AddressLabelID", label.AddressLabelID);
                databaseConnection.AddWithValue("@Label", label.Text, 50);
                databaseConnection.AddWithValue("@IsPrimary", label.Primary);
                databaseConnection.AddWithValue("@UserID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (label.EmployeeID.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters["@EmployeeID"].Value = label.EmployeeID.Value;
                }

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
        ///     Delete an address label by its identifier
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate identifier</param>
        /// <param name="identifier">The AddressLabel identifier</param>
        /// <param name="connection">The connection to override the default connection with, usually for unit testing</param>
        /// <returns>A positive value indicating success or a negative return code</returns>
        public static int Delete(ICurrentUserBase currentUser, int identifier, IDBConnection connection = null)
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

        #endregion
    }
}