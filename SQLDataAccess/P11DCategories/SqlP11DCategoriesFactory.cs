namespace SQLDataAccess.P11DCategories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using BusinessLogic.P11DCategories;

    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IP11DCategory"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class SqlP11DCategoriesFactory : IDataFactory<IP11DCategory, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IP11DCategory"/> instances.
        /// </summary>
        private readonly AccountCacheFactory<IP11DCategory, int> _cacheFactory;

        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlP11DCategoriesFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlP11DCategoriesFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IP11DCategory"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="identityProvider">An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityProvider"/> is <see langword="null" />.</exception>
        public SqlP11DCategoriesFactory(AccountCacheFactory<IP11DCategory, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, IdentityProvider identityProvider, ILog logger)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(identityProvider, nameof(identityProvider));
            Guard.ThrowIfNull(logger, nameof(logger));
            
            this._cacheFactory = cacheFactory;
            this._customerDataConnection = customerDataConnection;
            this._identityProvider = identityProvider;
            this._logger = logger;
        }

        /// <summary>
        /// Gets an instance of <see cref="IP11DCategory"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IP11DCategory"/> you want to retrieve</param>
        /// <returns>The required <see cref="IP11DCategory"/> or <see langword="null" /> if it cannot be found</returns>
        public IP11DCategory this[int id]
        {
            get
            {
                IP11DCategory p11DCategory = this._cacheFactory[id];

                if (p11DCategory == null)
                {
                    var projectCodes = this.GetFromDatabase(id);
                    if (projectCodes != null && projectCodes.Count > 0)
                    {
                        p11DCategory = projectCodes[0];
                        this._cacheFactory.Save(p11DCategory);
                    }
                }

                return p11DCategory;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IP11DCategory"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="p11DCategory">The <see cref="IP11DCategory"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IP11DCategory"/>.</returns>
        public virtual IP11DCategory Save(IP11DCategory p11DCategory)
        {
            if (p11DCategory == null)
            {
                return null;
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("SqlP11DCategoriesFactory.Add(p11DCategory) called.");
                this._logger.Debug(p11DCategory);
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@p11DCategoryId", SqlDbType.Int) { Value = p11DCategory.Id });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@p11DCategoryName", SqlDbType.NVarChar) { Value = p11DCategory.Name });

            UserIdentity identity = this._identityProvider.GetUserIdentity();
            
            this._customerDataConnection.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = identity.EmployeeId });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@savedDate", SqlDbType.DateTime) { Value = DateTime.Now });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@returnvalue", SqlDbType.Int) {Direction = ParameterDirection.ReturnValue});

            this._customerDataConnection.Parameters.AddAuditing(identity);

            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";

            int id = this._customerDataConnection.ExecuteProc<int>("saveP11DCategory");

            this._customerDataConnection.Parameters.Clear();

            p11DCategory.Id = id;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Add completed with id {id}.");
            }

            if (p11DCategory.Id > 0)
            {
                p11DCategory = this._cacheFactory.Save(p11DCategory);
            }

            return p11DCategory;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IP11DCategory"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IP11DCategory"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            UserIdentity identity = this._identityProvider.GetUserIdentity();

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlP11DCategoriesFactory.Delete({id}) called.");
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@p11DCategoryId", SqlDbType.Int) { Value = id });
            this._customerDataConnection.Parameters.AddAuditing(identity);
            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";
            int returnCode = this._customerDataConnection.ExecuteProc<int>("DeleteP11DCategory");

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Delete completed with id {id} and return code {returnCode}.");
            }

            // 0 is a successful delete; do not delete unless it has actually been deleted from the database
            if (returnCode == 0)
            {
                this._cacheFactory.Delete(id);
            }

            return returnCode;
        }

        /// <summary>
        /// Gets a list of all availible <see cref="IP11DCategory"/>
        /// </summary>
        /// <returns>The list of <see cref="IP11DCategory"/></returns>
        public IList<IP11DCategory> Get()
        {
            IList<IP11DCategory> p11DCategories = this._cacheFactory.Get();

            if (p11DCategories == null)
            {
                p11DCategories = this.GetFromDatabase(null);

                if (p11DCategories.Count > 0)
                {
                    this._cacheFactory.Add(p11DCategories);
                }
            }

            return p11DCategories;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all availible <see cref="IP11DCategory"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IP11DCategory"/> that match <paramref name="predicate"/>.</returns>
        public IList<IP11DCategory> Get(Predicate<IP11DCategory> predicate)
        {
            IList<IP11DCategory> p11DCategories = this.Get();

            if (predicate == null)
            {
                return p11DCategories;
            }

            List<IP11DCategory> matchP11DCategories = new List<IP11DCategory>();

            foreach (IP11DCategory p11DCategory in p11DCategories)
            {
                if (predicate.Invoke(p11DCategory))
                {
                    matchP11DCategories.Add(p11DCategory);
                }
            }

            return matchP11DCategories;
        }

        /// <summary>
        /// Gets a collection of <see cref="IP11DCategory"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IP11DCategory"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IP11DCategory"/></param>
        /// <returns>The required <see cref="IP11DCategory"/> or <see langword="null" /> or a collection of <see cref="IP11DCategory"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private List<IP11DCategory> GetFromDatabase(int? id)
        {
            List<IP11DCategory> p11DCategories = new List<IP11DCategory>();

            string sql = @"SELECT pdcatid, pdname FROM pdcats";

            if (id.HasValue)
            {
                sql += " WHERE pdcatid=@p11DCategoryId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@p11DCategoryId", SqlDbType.Int) { Value = id.Value });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int p11DCategoryIdOrdinal = reader.GetOrdinal("pdcatid");
                int codeOrdinal = reader.GetOrdinal("pdname");

                while (reader.Read())
                {
                    int p11DCategoryId = reader.GetInt32(p11DCategoryIdOrdinal);
                    string code = reader.GetString(codeOrdinal);

                    IP11DCategory p11DCategory = new P11DCategory(p11DCategoryId, code);

                    p11DCategories.Add(p11DCategory);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return p11DCategories;
        }
    }
}
