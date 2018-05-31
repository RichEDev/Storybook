namespace SQLDataAccess.Reasons
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using BusinessLogic.Reasons;

    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IReason"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class SqlReasonsFactory : IDataFactoryCustom<IReason, int, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IReason"/> instances.
        /// </summary>
        private readonly AccountCacheFactory<IReason, int> _cacheFactory;

        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlReasonsFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlReasonsFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IReason"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="identityProvider">An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityProvider"/> is <see langword="null" />.</exception>
        public SqlReasonsFactory(AccountCacheFactory<IReason, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, IdentityProvider identityProvider, ILog logger)
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
        /// Gets an instance of <see cref="IReason"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IReason"/> you want to retrieve</param>
        /// <returns>The required <see cref="IReason"/> or <see langword="null" /> if it cannot be found</returns>
        public IReason this[int id]
        {
            get
            {
                IReason reason = this._cacheFactory[id];

                if (reason == null)
                {
                    var reasons = this.GetFromDatabase(id);
                    if (reasons != null && reasons.Count > 0)
                    {
                        reason = reasons[0];
                        this._cacheFactory.OnAdd += this.AddToMap;
                        this._cacheFactory.Save(reason);
                    }
                }

                return reason;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IReason"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="reason">The <see cref="IReason"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IReason"/>.</returns>
        public IReason Save(IReason reason)
        {
            if (reason == null)
            {
                return null;
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("SqlReasonsFactory.Add(reason) called.");
                this._logger.Debug(reason);
            }

            var identity = this._identityProvider.GetUserIdentity();

            if (reason.Id == 0)
            {
                reason.CreatedBy = identity.EmployeeId;
                reason.CreatedOn = DateTime.Now;
            }
            else
            {
                reason.ModifiedBy = identity.EmployeeId;
                reason.ModifiedOn = DateTime.Now;
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@reasonid", SqlDbType.Int) { Value = reason.Id });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@reason", SqlDbType.NVarChar, 50) { Value = reason.Name });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 4000) { Value = reason.Description ?? string.Empty });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@accountcodevat", SqlDbType.NVarChar, 50) { Value = reason.AccountCodeVat ?? string.Empty });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@accountcodenovat", SqlDbType.NVarChar, 50) { Value = reason.AccountCodeNoVat ?? string.Empty });

            if (reason.ModifiedBy == null)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime) { Value = reason.CreatedOn });
                this._customerDataConnection.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int) { Value = reason.CreatedBy });
            }
            else
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime) { Value = reason.ModifiedOn });
                this._customerDataConnection.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int) { Value = reason.ModifiedBy });
            }

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeID", SqlDbType.Int) { Value = identity.EmployeeId });

            if (identity.DelegateId.HasValue)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = identity.DelegateId });
            }
            else
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = DBNull.Value });
            }

            this._customerDataConnection.Parameters.ReturnValue = "@identity";

            int id = this._customerDataConnection.ExecuteProc<int>("saveReason");

            reason.Id = id;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Add completed with id {id}.");
            }

            if (reason.Id > 0)
            {
                this._cacheFactory.OnAdd += this.AddToMap;
                reason = this._cacheFactory.Save(reason);
            }

            return reason;
        }

        /// <summary>
        /// Inverse the archive state of the <see cref="IReason"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="IReason"/> to change the Archive state on.</param>
        /// <returns>A bool indicating the current archive state of the <see cref="IReason"/> after execution.</returns>
        public int Archive(int id)
        {
            IReason reason = this[id];

            // Inverse the archived status
            reason.Archived = !reason.Archived;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlReasonFactory.Archive({reason.Id}) called.");
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@reasonid", SqlDbType.Int) { Value = reason.Id });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@archive", SqlDbType.Bit) { Value = Convert.ToByte(reason.Archived) });

            var identity = this._identityProvider.GetUserIdentity();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeID", SqlDbType.Int) { Value = identity.EmployeeId});

            if (identity.DelegateId.HasValue)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = identity.DelegateId });
            }
            else
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = DBNull.Value });
            }

            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";

            int returnCode = this._customerDataConnection.ExecuteProc<int>("changeReasonStatus");

            this._customerDataConnection.Parameters.Clear();

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"SqlReasonFactory.Archive({id}) completed");
            }

            if (returnCode == 0)
            {
                this._cacheFactory.Save(reason);
            }

            return returnCode;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IReason"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IReason"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            var identity = this._identityProvider.GetUserIdentity();

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlReasonFactory.Delete({id}) called.");
            }

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeID", SqlDbType.Int) { Value = identity.EmployeeId });

            if (identity.DelegateId.HasValue)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = identity.DelegateId });
            }
            else
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@delegateID", SqlDbType.Int) { Value = DBNull.Value });
            }

            this._customerDataConnection.Parameters.Add(new SqlParameter("@reasonid", SqlDbType.Int) { Value = id });

            this._customerDataConnection.Parameters.ReturnValue = "@retCode";

            int returnCode =  this._customerDataConnection.ExecuteProc<int>("deleteReason");

            if (returnCode == 0)
            {
                this._cacheFactory.OnDelete += this.DeleteFromHash;
                this._cacheFactory.Delete(id);
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"SqlReasonFactory.Delete({id}) completed");
            }

            return returnCode;
        }

        /// <summary>
        /// Gets an List of all available <see cref="IReason"/> 
        /// </summary>    
        /// <returns>The list of <see cref="IReason"/>.</returns>
        public IList<IReason> Get()
        {
            IList<IReason> reasons = this._cacheFactory.Get();

            if (reasons == null)
            {
                reasons = this.GetFromDatabase(null);

                if (reasons.Count > 0)
                {
                    this._cacheFactory.OnAddMultiple += this.AddToMapMultiple;
                    this._cacheFactory.Add(reasons);
                }
            }

            return reasons;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IReason"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IReason"/> that match <paramref name="predicate"/>.</returns>
        public IList<IReason> Get(Predicate<IReason> predicate)
        {
            IList<IReason> reasons = this.Get();

            if (predicate == null)
            {
                return reasons;
            }

            IList<IReason> matchReasons = new List<IReason>();

            foreach (IReason reason in reasons)
            {
                if (predicate.Invoke(reason))
                {
                    matchReasons.Add(reason);
                }
            }

            return matchReasons;
        }

        /// <summary>
        /// Gets an instance of <see cref="IReason"/> if it matches the supplied <paramref name="customGet"/>.
        /// </summary>
        /// <param name="customGet">An instance of <see cref="GetByCustom"/> to retrieve a <see cref="IReason"/> that matches the defined rules.</param>
        /// <returns>An instance of <see cref="IReason"/> that matches the rules defined in <paramref name="customGet"/>.</returns>
        public IReason GetByCustom(GetByCustom customGet)
        {
            return this._cacheFactory.HashGet(customGet.HashName, customGet.Field);
        }

        /// <summary>
        /// Adds an instance of <see cref="IReason"/> to the names and descriptions hash in cache.
        /// </summary>
        /// <param name="reason">The <see cref="IReason"/> to add to cache.</param>
        private void AddToMap(IReason reason)
        {
            if (reason.Id > 0)
            {
                IReason cachedReason = this._cacheFactory[reason.Id];

                if (cachedReason != null)
                {
                    this.DeleteFromHash(cachedReason);
                }
            }

            this._cacheFactory.HashAdd("names", reason.Name, reason);
        }

        /// <summary>
        /// Adds multiple instances of <see cref="IReason"/> to the names and descriptions hash in cache.
        /// </summary>
        /// <param name="reasons">Collection of <see cref="IReason"/> to add to cache.</param>
        private void AddToMapMultiple(IList<IReason> reasons)
        {
            IDictionary<string, object> hashReasonsByName = new Dictionary<string, object>();

            foreach (IReason reason in reasons)
            {
                if (hashReasonsByName.ContainsKey(reason.Name) == false)
                {
                    hashReasonsByName.Add(reason.Name, reason);
                }
            }

            this._cacheFactory.HashAdd("names", hashReasonsByName);
        }

        /// <summary>
        /// Deletes <paramref name="reason"/> from the names and descriptions hash in cache.
        /// </summary>
        /// <param name="reason">The <see cref="IReason"/> to delete.</param>
        private void DeleteFromHash(IReason reason)
        {
            this._cacheFactory.HashDelete("names", reason.Name);
        }


        /// <summary>
        /// Gets a collection of <see cref="IReason"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IReason"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IReason"/></param>
        /// <returns>The required <see cref="IReason"/> or <see langword="null" /> or a collection of <see cref="IReason"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private List<IReason> GetFromDatabase(int? id)
        {
            List<IReason> reasons = new List<IReason>();

            string sql = @"SELECT reasonid, reason, description, accountcodevat, accountcodenovat, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived FROM dbo.reasons";

            if (id.HasValue)
            {
                sql += " WHERE reasonid = @reasonId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@reasonId", SqlDbType.Int) { Value = id.Value });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int idOrdinal = reader.GetOrdinal("reasonid");
                int nameOrdinal = reader.GetOrdinal("reason");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int accountCodeVatOrdinal = reader.GetOrdinal("accountcodevat");
                int accountCodeNoVatOrdinal = reader.GetOrdinal("accountcodenovat");
                int archivedOrdinal = reader.GetOrdinal("Archived");
                int createdOnOrdinal = reader.GetOrdinal("createdon");
                int createdByOrdinal = reader.GetOrdinal("createdby");
                int modifiedOnOrdinal = reader.GetOrdinal("modifiedon");
                int modifiedByOrdinal = reader.GetOrdinal("modifiedby");

                while (reader.Read())
                {
                    var reasonId = reader.GetInt32(idOrdinal);
                    var name = reader.GetString(nameOrdinal);
                    var description = !reader.IsDBNull(descriptionOrdinal) ? reader.GetString(descriptionOrdinal) : string.Empty;
                    var accountCodeVat = !reader.IsDBNull(accountCodeVatOrdinal) ? reader.GetString(accountCodeVatOrdinal) : string.Empty;
                    var accountCodeNoVat = !reader.IsDBNull(accountCodeNoVatOrdinal) ? reader.GetString(accountCodeNoVatOrdinal) : string.Empty;
                    var archived = reader.GetBoolean(archivedOrdinal);
                    var createdOn = !reader.IsDBNull(createdOnOrdinal) ? reader.GetDateTime(createdOnOrdinal) : new DateTime(1900, 01, 01);
                    var createdBy = !reader.IsDBNull(createdByOrdinal) ? reader.GetInt32(createdByOrdinal) : 0;
                    var modifiedOn = !reader.IsDBNull(modifiedOnOrdinal) ? reader.GetDateTime(modifiedOnOrdinal) : (DateTime?)null;
                    var modifiedBy = !reader.IsDBNull(modifiedByOrdinal) ? reader.GetInt32(modifiedByOrdinal) : (int?)null;

                    IReason reason = new Reason(reasonId, archived, description, name, accountCodeVat, accountCodeNoVat, createdBy, createdOn, modifiedBy, modifiedOn);

                    reasons.Add(reason);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return reasons;
        }
    }
}
