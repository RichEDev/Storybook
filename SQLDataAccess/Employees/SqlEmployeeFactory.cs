namespace SQLDataAccess.Employees
{
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
        private readonly AccountCacheFactory<IEmployee, int> _cacheFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEmployeeFactory"/> class to use when creating or retrieving new instances of <see cref="IEmployee"/>
        /// </summary>
        public SqlEmployeeFactory(AccountCacheFactory<IEmployee, int> cacheFactory)
        {
            this._cacheFactory = cacheFactory;
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
                    employee = this.Get(id);
                    this._cacheFactory.Add(employee);
                }

                return employee;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IEmployee"/> to SQL, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="employee">The <see cref="IEmployee"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IEmployee"/>.</returns>
        public IEmployee Add(IEmployee employee)
        {
            this._cacheFactory.Add(employee);

            return employee;
        }

        /// <summary>
        /// Gets an instance of <see cref="IEmployee"/> from SQL
        /// </summary>
        /// <param name="id">The ID of the <see cref="IEmployee"/> you want to retrieve from <see cref="IDataConnection{T}"/></param>
        /// <returns>The required <see cref="IEmployee"/> or null if it does not exist in <see cref="IDataConnection{T}"/></returns>
        public IEmployee Get(int id)
        {
            return null;
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<IEmployee> Get()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IEmployee> Get(Predicate<IEmployee> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
