namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Data;
    using System.Globalization;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    /// <summary>
    /// Defines members of a user and methods to use it.
    /// </summary>
    [Serializable]
    public class User : IOwnership
    {
        /// <summary>
        /// The account id this employee belongs to.
        /// </summary>
        public readonly int AccountID;

        public User()
        {
            
        }

        public User(int accountID, int employeeID, string username, string title, string forename, string surname)
        {
            this.AccountID = accountID;
            this.EmployeeID = employeeID;
            this.Username = username;
            this.Title = title;
            this.Forename = forename;
            this.Surname = surname;
        }

        public string CombinedItemKey
        {
            get
            {
                return String.Format("{0},{1}", (int)this.ElementType(), this.ItemPrimaryID());
            }
        }

        public string CombinedOwnerKey
        {
            get
            {
                return String.Format("{0},{1}", (int)this.OwnerElementType(), this.OwnerId());
            }
        }

        /// <summary>
        /// Gets or sets the employee ID for this employee.
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the forename of this individual.
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Gets the full name of this employee ("Forename Surname" only).
        /// </summary>
        public string FullName
        {
            get
            {
                return this.Forename + " " + this.Surname;
            }
        }

        /// <summary>
        /// Gets the full name and username of this employee - "Forename Surname (Username)"
        /// </summary>
        public string FullNameUsername
        {
            get
            {
                return string.Format("{0} ({1})", this.FullName, this.Username);
            }
        }

        /// <summary>
        /// Gets or sets the surname of this individual.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the title of this individual (e.g. Mr, Mrs, Dr)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the username of this employee.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Ads an instance of this employee to the cache object.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <param name="accountID">The accountID this employee belongs to.</param>
        public static void CacheAdd(Employee employee, int accountID)
        {
            Cache caching = new Cache();
            caching.Add(accountID, Employee.CacheArea, employee.EmployeeID.ToString(CultureInfo.InvariantCulture), employee);
        }

        /// <summary>
        /// Deletes this employee from the cache object.
        /// </summary>
        /// <param name="employeeID">The employee to remove from cache.</param>
        /// <param name="accountID">The accountID this employee belongs to.</param>
        public static void CacheRemove(int employeeID, int accountID)
        {
            Cache caching = new Cache();
            caching.Delete(accountID, Employee.CacheArea, employeeID.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Updates the cache object with a new instance of this employee.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <param name="accountID">The accountID this employee belongs to.</param>
        public static void CacheUpdate(Employee employee, int accountID)
        {
            Cache caching = new Cache();
            caching.Update(accountID, Employee.CacheArea, employee.EmployeeID.ToString(CultureInfo.InvariantCulture), employee);
        }

        /// <summary>
        /// Deletes the employee.
        /// </summary>
        /// <param name="currentUser">The current user executing this method.</param>
        /// <returns>The return value from the delete employee stored procedure.</returns>
        public int Delete(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnvalue;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this.EmployeeID);
                // Following stored procedure checks if any Authoriser Level assigned to current approver. Returns NULL for no AL, -1 for DAL, or actual amount if assigned.
                decimal? authoriserLevelAmount = databaseConnection.ExecuteScalar<decimal?>("dbo.GetAuthoriserLevelAmountByEmployee", CommandType.StoredProcedure);
                databaseConnection.sqlexecute.Parameters.Clear();
                if (authoriserLevelAmount == -1 && authoriserLevelAmount != null)
                {
                    // No authoriser level assigned or it is assigned Default Authoriser Level
                    return 17; 
                }
                databaseConnection.sqlexecute.Parameters.Clear();
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);

                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this.EmployeeID);
                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("deleteEmployee");
                returnvalue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            CacheRemove(this.EmployeeID, this.AccountID);

            new Cache().Delete(this.AccountID, "advances", "0");

            return returnvalue;
        }
        public virtual SpendManagementElement ElementType()
        {
            return SpendManagementElement.Employees;
        }

        public virtual string ItemDefinition()
        {
            return String.Format("{0} (Employee)", this.FullName);
        }

        public virtual int ItemPrimaryID()
        {
            return this.EmployeeID;
        }
        public virtual string OwnerDefinition()
        {
            return String.Empty;
        }

        public virtual SpendManagementElement OwnerElementType()
        {
            return SpendManagementElement.None;
        }

        public virtual int? OwnerId()
        {
            return null;
        }
    }
}
