namespace SQLDataAccess.AccountProperties
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    public class SqlAccountPropertiesFactory : IDataFactory<IAccountProperty, AccountPropertyCacheKey>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IAccountProperty"/> instances.
        /// </summary>
        private readonly AccountCacheFactory<IAccountProperty, string> _cacheFactory;

        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlAccountPropertiesFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountPropertiesFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IAccountProperty"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="identityProvider">An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityProvider"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" />.</exception>
        public SqlAccountPropertiesFactory(AccountCacheFactory<IAccountProperty, string> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, IdentityProvider identityProvider, ILog logger)
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
        /// Gets an instance of <see cref="IAccountProperty"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The subAccountId and Id of the <see cref="IAccountProperty"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAccountProperty"/> or <see langword="null" /> if it cannot be found</returns>
        public IAccountProperty this[AccountPropertyCacheKey id]
        {
            get
            {
                IAccountProperty accountProperty = this._cacheFactory[id.CacheKey];

                if (accountProperty == null)
                {
                    var accountProperties = this.GetFromDatabase(id);
                    if (accountProperties != null && accountProperties.Count > 0)
                    {
                        accountProperty = accountProperties[0];
                        this._cacheFactory.Save(accountProperty);
                    }
                }

                return accountProperty;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IAccountProperty"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="entity">The <see cref="IAccountProperty"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IAccountProperty"/>.</returns>
        public IAccountProperty Save(IAccountProperty entity)
        {
            if (string.IsNullOrWhiteSpace(entity?.Id))
            {
                return null;
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("SqlAccountPropertiesFactory.Save(accountProperty) called.");
                this._logger.Debug(entity);
            }

            this._customerDataConnection.Parameters.Clear();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@stringKey", SqlDbType.NVarChar, 150) { Value = entity.Key });

            if (entity.Value == null)
            {
                entity.Value = string.Empty;
            }

            if (string.IsNullOrEmpty(entity.Value))
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@stringValue", DBNull.Value) { Value = DBNull.Value });
            }
            else
            {
                if (entity.Value.ToLowerInvariant() == "false")
                {
                    entity.Value = "0";
                }

                if (entity.Value.ToLowerInvariant() == "true")
                {
                    entity.Value = "1";
                }

                this._customerDataConnection.Parameters.Add(new SqlParameter("@stringValue", SqlDbType.NVarChar) { Value = entity.Value.ToString() });
            }

            UserIdentity identity = this._identityProvider.GetUserIdentity();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeID", SqlDbType.Int) { Value = identity.EmployeeId });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@modifiedBy", SqlDbType.Int) { Value = identity.EmployeeId });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@subAccountID", SqlDbType.Int) { Value = entity.SubAccountId });


            this._customerDataConnection.Parameters.Add(identity.DelegateId.HasValue
                ? new SqlParameter("@delegateID", SqlDbType.Int) {Value = identity.DelegateId}
                : new SqlParameter("@delegateID", DBNull.Value) {Value = DBNull.Value});

            this._customerDataConnection.ExecuteProc("saveSubAccountProperties");

            this._customerDataConnection.Parameters.Clear();

            if (this._logger.IsDebugEnabled && !string.IsNullOrWhiteSpace(entity.Id))
            {
                this._logger.DebugFormat($"Add completed with id {entity.Id}.");
            }

            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                this._cacheFactory.Save(entity);
            }

            return entity;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IAccountProperty"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IAccountProperty"/> to delete.</param>
        /// <returns>An <see cref="string"/> containing the result of the delete.</returns>
        public int Delete(AccountPropertyCacheKey id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all available <see cref="IAccountProperty"/>
        /// </summary>
        /// <returns>The list of <see cref="IAccountProperty"/></returns>
        public IList<IAccountProperty> Get()
        {
            IList<IAccountProperty> accountProperties = this._cacheFactory.Get();

            if (accountProperties == null)
            {
                accountProperties = this.GetFromDatabase(null);

                if (accountProperties.Count > 0)
                {
                    this._cacheFactory.Add(accountProperties);
                }
            }

            return accountProperties;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IAccountProperty"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IAccountProperty"/> that match <paramref name="predicate"/>.</returns>
        public IList<IAccountProperty> Get(Predicate<IAccountProperty> predicate)
        {
            IList<IAccountProperty> accountProperties = this.Get();

            if (predicate == null)
            {
                return accountProperties;
            }

            List<IAccountProperty> matchAccountProperties = new List<IAccountProperty>();

            foreach (IAccountProperty accountProperty in accountProperties)
            {
                if (predicate.Invoke(accountProperty))
                {
                    matchAccountProperties.Add(accountProperty);
                }
            }

            return matchAccountProperties;
        }

        /// <summary>
        /// Gets a collection of <see cref="IAccountProperty"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IAccountProperty"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IAccountProperty"/></param>
        /// <returns>The required <see cref="IAccountProperty"/> or <see langword="null" /> or a collection of <see cref="IAccountProperty"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private IList<IAccountProperty> GetFromDatabase(AccountPropertyCacheKey accountPropertyCacheKey)
        {
            List<IAccountProperty> accountProperties = new List<IAccountProperty>();

            string sql = @"SELECT subAccountId, stringKey, stringValue, formpostkey, isglobal FROM accountProperties";

            if (!string.IsNullOrWhiteSpace(accountPropertyCacheKey?.Key) && !string.IsNullOrWhiteSpace(accountPropertyCacheKey?.SubAccountId))
            {
                sql += " WHERE stringKey = @stringKey and subAccountId = @subAccountId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@stringKey", SqlDbType.NVarChar) { Value = accountPropertyCacheKey.Key });
                this._customerDataConnection.Parameters.Add(new SqlParameter("@subAccountId", SqlDbType.NVarChar) { Value = accountPropertyCacheKey.SubAccountId });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int subAccountIdOrdinal = reader.GetOrdinal("subAccountId");
                int stringKeyOrdinal = reader.GetOrdinal("stringKey");
                int stringValueOrdinal = reader.GetOrdinal("stringValue");
                int formPostKeyOrdinal = reader.GetOrdinal("formpostkey");
                int isGlobalOrdinal = reader.GetOrdinal("isglobal");

                while (reader.Read())
                {
                    int subAccountId = reader.GetInt32(subAccountIdOrdinal);
                    string stringKey = reader.GetString(stringKeyOrdinal);
                    string stringValue = reader.IsDBNull(stringValueOrdinal) ? string.Empty : reader.GetString(stringValueOrdinal);
                    string formPostKey = reader.IsDBNull(formPostKeyOrdinal) ? string.Empty : reader.GetString(formPostKeyOrdinal);
                    bool isGlobal = reader.GetBoolean(isGlobalOrdinal);

                    IAccountProperty accountProperty = new AccountProperty(stringKey, stringValue, subAccountId)
                    {
                        PostKey = formPostKey,
                        IsGlobal = isGlobal
                    };

                    accountProperties.Add(accountProperty);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return accountProperties;
        }
    }
}
