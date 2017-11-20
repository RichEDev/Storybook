namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeItemRoles
    {
        /// <summary>
        /// The _backing collection for employee item roles
        /// </summary>
        private List<EmployeeItemRole> _backingCollection;

        /// <summary>
        /// The account id of the employee who's item role(s) are being loaded
        /// </summary>
        private readonly int _accountId;

        /// <summary>
        /// The id of the employee who's item roles are being loaded
        /// </summary>
        private readonly int _employeeId;

        /// <summary>
        /// The cache key
        /// </summary>
        public const string CacheArea = "employeeItemRoles";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeItemRoles"/> class.
        /// </summary>
        /// <param name="accountID">
        /// The account id of the employee
        /// </param>
        /// <param name="employeeID">
        /// The employee id who's item roles are need to be loaded
        /// </param>
        public EmployeeItemRoles(int accountID, int employeeID)
        {
            this._accountId = accountID;
            this._employeeId = employeeID;
            this._backingCollection = this.Get();
        }

        /// <summary>
        /// Gets the count of the number of item roles against the employee
        /// </summary>
        public int Count => this._backingCollection.Count;

        /// <summary>
        /// Gets the item roles for the employee from the backing collection which is loaded when initialised
        /// </summary>
        public List<EmployeeItemRole> ItemRoles => this._backingCollection;

        /// <summary>
        /// Gets the employee item role for the given item role
        /// </summary>
        /// <param name="index">
        /// The index (item role id)
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeItemRole"/> that matches the index
        /// </returns>
        public EmployeeItemRole this[int index]
        {
            get
            {
                return this._backingCollection.Single(x => x.ItemRoleId == index);
            }
        }

        /// <summary>
        /// Returns true if the item role belongs to the employee
        /// </summary>
        /// <param name="itemRoleId">
        /// The item role id.
        /// </param>
        /// <returns>
        /// The true if item role is present
        /// </returns>
        public bool Contains(int itemRoleId)
        {
            return this._backingCollection.Any(x => x.ItemRoleId == itemRoleId);
        }

        /// <summary>
        /// Removes the given employee item role against the employee from the database and the backling collection
        /// </summary>
        /// <param name="roleId">
        /// The item role id that needs to be deleted from the employee
        /// </param>
        /// <param name="currentUser">
        /// The current user who is logged in
        /// </param>
        /// <param name="connection">
        /// The database connection.
        /// </param>
        public void Remove(int roleId, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var itemRoleToRemove = this._backingCollection.Single(x => x.ItemRoleId == roleId);

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                this._backingCollection.Remove(itemRoleToRemove);
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@roleID", roleId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

                databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("deleteEmployeeItemRole");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
            
            this._backingCollection.Remove(itemRoleToRemove);

            this.CacheDelete();

            var employeeSubCats = new EmployeeSubCategories(this._accountId, this._employeeId);
            employeeSubCats.CacheDelete();
        }

        /// <summary>
        /// Adds a new item role against the employee
        /// </summary>
        /// <param name="itemRoles">
        /// The item roles that need to be added
        /// </param>
        /// <param name="currentUser">
        /// The current user who's logged in
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void Add(List<EmployeeItemRole> itemRoles, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                foreach (var itemRole in itemRoles)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeId);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@roleID", itemRole.ItemRoleId);
                    if (itemRole.StartDate != null)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@startdate", itemRole.StartDate);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);
                    }
                    if (itemRole.EndDate != null)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@enddate", itemRole.EndDate);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);
                    }

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

                    databaseConnection.ExecuteProc("saveEmployeeItemRoles");
                    databaseConnection.sqlexecute.Parameters.Clear();
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);

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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("recordEmployeeModified");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this._backingCollection.Clear();
            this._backingCollection = itemRoles;
            this.CacheDelete();
        }

        /// <summary>
        /// Gets all the items roles for the employee
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of employee item roles from the database
        /// </returns>
        private List<EmployeeItemRole> Get(IDBConnection connection = null)
        {
            List<EmployeeItemRole> roles;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                roles = this.CacheGet();

                if (roles == null)
                {
                    roles = new List<EmployeeItemRole>();

                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string Sql = "select item_roles.itemroleid, employee_roles.startdate, employee_roles.enddate from item_roles inner join employee_roles on employee_roles.itemroleid = item_roles.itemroleid where employee_roles.employeeid = @employeeid order by employee_roles.[order]";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeId);

                    using (IDataReader reader = databaseConnection.GetReader(Sql))
                    {

                        var itemRoleIdOrd = reader.GetOrdinal("itemroleid");
                        var startDateOrd = reader.GetOrdinal("startdate");
                        var endDateOrd = reader.GetOrdinal("enddate");

                        while (reader.Read())
                        {
                            var itemRoleId = reader.GetInt32(itemRoleIdOrd);
                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            if (!reader.IsDBNull(startDateOrd))
                            {
                                startDate = reader.GetDateTime(startDateOrd);
                            }

                            if (!reader.IsDBNull(endDateOrd))
                            {
                                endDate = reader.GetDateTime(endDateOrd);
                            }
                            
                            roles.Add(new EmployeeItemRole(itemRoleId, startDate, endDate));
                        }
                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    this.CacheAdd(roles);
                }
            }

            return roles;
        }

        /// <summary>
        /// Updates the employee item role
        /// </summary>
        /// <param name="itemRole">
        /// The item role that needs to be updated
        /// </param>
        /// <param name="currentUser">
        /// The current user who is logged in
        /// </param>
        /// <param name="connection">
        /// The database connection
        /// </param>
        public bool Update(EmployeeItemRole itemRole, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var itemRoleToRemove = this._backingCollection.Single(x => x.ItemRoleId == itemRole.ItemRoleId);
            if (itemRoleToRemove.StartDate == itemRole.StartDate && itemRoleToRemove.EndDate == itemRole.EndDate)
            {
                return true;
            }

            using (var databaseConnection =
                connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@roleID", itemRole.ItemRoleId);
                if (itemRole.StartDate != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@startdate", itemRole.StartDate);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);
                }
                if (itemRole.EndDate != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@enddate", itemRole.EndDate);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);
                }

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue(
                            "@CUdelegateID",
                            currentUser.Delegate.EmployeeID);
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

                databaseConnection.ExecuteProc("saveEmployeeItemRoles");
                databaseConnection.sqlexecute.Parameters.Clear();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);

                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue(
                            "@CUdelegateID",
                            currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("recordEmployeeModified");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this._backingCollection.Remove(itemRoleToRemove);
            this._backingCollection.Add(new EmployeeItemRole(itemRole.ItemRoleId, itemRole.StartDate, itemRole.EndDate));
            this.CacheDelete();
            return true;
        }

        /// <summary>
        /// Adds the list of employee item roles to the cache
        /// </summary>
        /// <param name="itemRoles">
        /// The item roles for the employee
        /// </param>
        private void CacheAdd(List<EmployeeItemRole> itemRoles)
        {
            Cache cache = new Cache();
            cache.Add(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture), itemRoles);
        }

        /// <summary>
        /// Gets all employee item roles from the cache
        /// </summary>
        /// <returns>
        /// The <see cref="List"/> of employee item roles for the employee
        /// </returns>
        private List<EmployeeItemRole> CacheGet()
        {
            Cache cache = new Cache();
            return (List<EmployeeItemRole>)cache.Get(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Deletes employee item roles for the employee from the cache
        /// </summary>
        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture));
        }
    }
}
