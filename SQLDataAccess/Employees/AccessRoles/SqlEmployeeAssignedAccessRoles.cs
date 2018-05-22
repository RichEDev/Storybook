namespace SQLDataAccess.Employees.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// On change delegate for when a <see cref="IAssignedAccessRole"/> is created, modified or deleted. 
    /// </summary>
    /// <param name="userIdentity">The user identity.</param>
    /// <param name="assignedAccessRole">The assigned access role.</param>
    public delegate void OnChangeEmployeeAssignedAccessRole(UserIdentity userIdentity, IAssignedAccessRole assignedAccessRole);

    /// <summary>
    /// Defines members to operate and obtain instances of <see cref="IAssignedAccessRole"/> for an employee.
    /// </summary>
    public interface IAssignedAccessRolesFactory
    {
        /// <summary>
        /// Gets a collection of <see cref="IAssignedAccessRole"/> for the specified <paramref name="employeeId"/>.
        /// </summary>
        /// <param name="employeeId">The employee id to get the assigned <see cref="IAssignedAccessRole"/> collection for.</param>
        /// <returns>The <see cref="IEnumerable{IAssignedAccessRole}"/> collection for the specified <paramref name="employeeId"/>.</returns>
        IEnumerable<IAssignedAccessRole> this[int employeeId] { get; }

        /// <summary>
        /// Adds a <see cref="IAssignedAccessRole"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IAssignedAccessRole"/> to add.</param>
        /// <returns>The <see cref="IAssignedAccessRole"/>.</returns>
        IAssignedAccessRole Add(IAssignedAccessRole entity);

        /// <summary>
        /// Deletes an <see cref="IAssignedAccessRole"/>.
        /// </summary>
        /// <param name="employeeId">The employee id of the <see cref="IAssignedAccessRole"/>.</param>
        /// <param name="accessRoleId">The access role id of the <see cref="IAssignedAccessRole"/>.</param>
        /// <param name="subAccountId">The sub account id of the <see cref="IAssignedAccessRole"/>.</param>
        /// <returns>The result of the delete request.</returns>
        int Delete(int employeeId, int accessRoleId, int subAccountId);

        /// <summary>
        /// Gets a collection of <see cref="IAssignedAccessRole"/> for the specified <paramref name="employeeId"/> filtered by the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="employeeId">The employee id to get the assigned <see cref="IAssignedAccessRole"/> collection for.</param>
        /// <param name="predicate">The predicate to apply to the collection of <see cref="IAssignedAccessRole"/> objects.</param>
        /// <returns>The <see cref="IEnumerable{IAssignedAccessRole}"/> collection for the specified <paramref name="employeeId"/>.</returns>
        IEnumerable<IAssignedAccessRole> Get(int employeeId, Predicate<IAssignedAccessRole> predicate);

        /// <summary>
        /// Gets a collection of <see cref="IAssignedAccessRole"/> for the specified <paramref name="employeeId"/>.
        /// </summary>
        /// <param name="employeeId">The employee id to get the assigned <see cref="IAssignedAccessRole"/> collection for.</param>
        /// <returns>The <see cref="IEnumerable{IAssignedAccessRole}"/> collection for the specified <paramref name="employeeId"/>.</returns>
        IEnumerable<IAssignedAccessRole> Get(int employeeId);
    }

    /// <summary>
    /// Enables the creation, modification and deletion of instances of <see cref="IAssignedAccessRole"/> objects.
    /// </summary>
    public class SqlEmployeeAssignedAccessRoles : IAssignedAccessRolesFactory
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IEmployeeAccessScope"/> instances.
        /// </summary>
        private readonly IAccountCacheFactory<IAssignedAccessRole, string> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlEmployeeAssignedAccessRoles"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEmployeeAssignedAccessRoles"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IEmployeeAccessScope"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        public SqlEmployeeAssignedAccessRoles(IAccountCacheFactory<IAssignedAccessRole, string> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, ILog logger)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._cacheFactory = cacheFactory;
            this._customerDataConnection = customerDataConnection;
            this._logger = logger;
        }

        /// <summary>
        /// Triggered when a <see cref="IAssignedAccessRole"/> is created, modified or deleted.
        /// </summary>
        public event OnChangeEmployeeAssignedAccessRole OnChange;

        /// <inheritdoc />
        public IEnumerable<IAssignedAccessRole> this[int employeeId] => this.Get(employeeId);

        /// <inheritdoc />
        public IAssignedAccessRole Add(IAssignedAccessRole entity)
        {
            /* 
             * Add a reference to an access role / subaccount
             * Needs to trigger any subscribers on change once assigned access roles is replaced, until then caching is not enabled with AssignedAccessRoles or CombinedAccessRoles
             * this.OnChange?.Invoke();
            */

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int Delete(int employeeId, int accessRoleId, int subAccountId)
        {
            throw new NotImplementedException();

            /* 
            const string Sql = "DELETE FROM employeeAccessRoles WHERE employeeID=@employeeId AND accessRoleID=@accessRoleId AND subAccountID=@subAccountId";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleId", SqlDbType.Int) { Value = accessRoleId });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@subAccountId", SqlDbType.Int) { Value = subAccountId });
            int affectedRows = this._customerDataConnection.ExecuteNonQuery(Sql);
            this._customerDataConnection.Parameters.Clear();

            Needs to trigger any subscribers on change once assigned access roles is replaced, until then caching is not enabled with AssignedAccessRoles or CombinedAccessRoles
             * this.OnChange?.Invoke();
             * 
            return affectedRows;
            */
        }

        /// <inheritdoc />
        public IEnumerable<IAssignedAccessRole> Get(int employeeId)
        {
            List<IAssignedAccessRole> assignedAccessRoles = new List<IAssignedAccessRole>();

            string sql = "SELECT accessRoleID, subAccountID FROM employeeAccessRoles WHERE employeeID=@employeeId ORDER BY accessRoleID";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId });

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int accessRoleIdOrdinal = reader.GetOrdinal("accessRoleID");
                int subAccountIdOrdinal = reader.GetOrdinal("subAccountID");

                while (reader.Read())
                {
                    int accessRoleId = reader.GetInt32(accessRoleIdOrdinal);
                    int subAccountId = reader.GetInt32(subAccountIdOrdinal);

                    assignedAccessRoles.Add(new AssignedAccessRole(employeeId, accessRoleId, subAccountId));
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return assignedAccessRoles;
        }

        /// <inheritdoc />
        public IEnumerable<IAssignedAccessRole> Get(int employeeId, Predicate<IAssignedAccessRole> predicate)
        {
            List<IAssignedAccessRole> assignedAccessRoles = this.Get(employeeId).ToList();
            
            return assignedAccessRoles.FindAll(predicate);
        }
    }
}
