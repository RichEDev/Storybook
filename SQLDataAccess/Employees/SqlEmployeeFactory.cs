namespace SQLDataAccess.Employees
{
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees;

    using CacheDataAccess.Caching;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extends <see cref="SqlEmployeeFactory"/> and implements methods to retrieve and create instances of <see cref="IEmployee"/> 
    /// </summary>
    public class SqlEmployeeFactory : IDataFactory<IEmployee, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IEmployee"/> instances.
        /// </summary>
        private readonly AccountCacheFactory<IEmployee, int> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEmployeeFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IEmployee"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        public SqlEmployeeFactory(AccountCacheFactory<IEmployee, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));

            this._cacheFactory = cacheFactory;
            this._customerDataConnection = customerDataConnection;
        }

        /// <summary>
        /// Gets an instance of <see cref="IEmployee"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally sql
        /// </summary>
        /// <param name="id">The ID of the <see cref="IEmployee"/> you want to retrieve</param>
        /// <returns>The required <see cref="IEmployee"/> or null if it cannot be found</returns>
        public IEmployee this[int id]
        {
            get
            {
                IEmployee employee = this._cacheFactory[id];

                if (employee == null)
                {
                    var employees = this.GetFromDatabase(id);
                    if (employees != null && employees.Count > 0)
                    {
                        employee = employees[0];
                        this._cacheFactory.Save(employee);
                    }
                }

                return employee;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IEmployee"/> to SQL, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="employee">The <see cref="IEmployee"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IEmployee"/>.</returns>
        public IEmployee Save(IEmployee employee)
        {
            this._cacheFactory.Save(employee);

            return employee;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IEmployee"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IEmployee"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all available <see cref="IEmployee"/>
        /// </summary>
        /// <returns>The list of <see cref="IEmployee"/></returns>
        public IList<IEmployee> Get()
        {
            IList<IEmployee> employees = this._cacheFactory.Get();

            if (employees == null)
            {
                employees = this.GetFromDatabase(null);

                if (employees.Count > 0)
                {
                    this._cacheFactory.Add(employees);
                }
            }

            return employees;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IEmployee"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IEmployee"/> that match <paramref name="predicate"/>.</returns>
        public IList<IEmployee> Get(Predicate<IEmployee> predicate)
        {
            IList<IEmployee> employees = this.Get();

            if (predicate == null)
            {
                return employees;
            }

            List<IEmployee> matchEmployees = new List<IEmployee>();

            foreach (IEmployee employee in matchEmployees)
            {
                if (predicate.Invoke(employee))
                {
                    matchEmployees.Add(employee);
                }
            }

            return matchEmployees;
        }

        /// <summary>
        /// Gets a collection of <see cref="IEmployee"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IEmployee"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IEmployee"/></param>
        /// <returns>The required <see cref="IEmployee"/> or <see langword="null" /> or a collection of <see cref="IEmployee"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private IList<IEmployee> GetFromDatabase(int? id)
        {
            IList<IEmployee> employees = new List<IEmployee>();

            string sql = @"SELECT employeeid, defaultSubAccountId FROM employees";

            if (id.HasValue)
            {
                sql += " WHERE employeeid = @employeeId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeId", SqlDbType.NVarChar) { Value = id.Value });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int employeeIdOrdinal = reader.GetOrdinal("employeeid");
                int subAccountIdOrdinal = reader.GetOrdinal("defaultSubAccountId");

                while (reader.Read())
                {
                    int employeeId = reader.GetInt32(employeeIdOrdinal);
                    int subAccountId = reader.GetInt32(subAccountIdOrdinal);

                    IEmployee employee = new Employee(employeeId, subAccountId);

                    employees.Add(employee);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return employees;
        }
    }
}
