namespace SQLDataAccess.ProjectCodes
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using BusinessLogic.ProjectCodes;
    
    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IProjectCode"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class SqlProjectCodesFactory : IDataFactoryCustom<IProjectCode, int, bool>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IProjectCode"/> instances.
        /// </summary>
        private readonly AccountCacheFactory<IProjectCode, int> _cacheFactory;

        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlProjectCodesFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProjectCodesFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IProjectCode"/> instances.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="identityProvider">An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityProvider"/> is <see langword="null" />.</exception>
        public SqlProjectCodesFactory(AccountCacheFactory<IProjectCode, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, IdentityProvider identityProvider, ILog logger)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(identityProvider, nameof(identityProvider));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._cacheFactory = cacheFactory;
            this._customerDataConnection = customerDataConnection;
            this._identityProvider = identityProvider;
            this._logger = logger;

            this.Get();
        }

        /// <summary>
        /// Gets an instance of <see cref="IProjectCode"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IProjectCode"/> you want to retrieve</param>
        /// <returns>The required <see cref="IProjectCode"/> or <see langword="null" /> if it cannot be found</returns>
        public IProjectCode this[int id]
        {
            get
            {
                IProjectCode projectCode = this._cacheFactory[id];

                if (projectCode == null)
                {
                    var projectCodes = this.GetFromDatabase(id);
                    if (projectCodes != null && projectCodes.Count > 0)
                    {
                        projectCode = projectCodes[0];
                        this._cacheFactory.OnAdd += this.AddToMap;
                        this._cacheFactory.Save(projectCode);
                    }
                }

                return projectCode;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IProjectCode"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="projectCode">The <see cref="IProjectCode"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IProjectCode"/>.</returns>
        public virtual IProjectCode Save(IProjectCode projectCode)
        {
            if (projectCode == null)
            {
                return null;
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("SqlProjectCodesFactory.Add(projectcode) called.");
                this._logger.Debug(projectCode);
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@projectcodeid", SqlDbType.Int) { Value = projectCode.Id });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@projectcode", SqlDbType.NVarChar) { Value = projectCode.Name });

            if (projectCode.Description.Length > 1999)
            {
                projectCode.Description = projectCode.Description.Substring(0, 1998);
            }

            UserIdentity identity = this._identityProvider.GetUserIdentity();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar) { Value = projectCode.Description });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@rechargeable", SqlDbType.Bit) { Value = projectCode.Rechargeable });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int) { Value = identity.EmployeeId });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime) { Value = DateTime.Now });

            this._customerDataConnection.Parameters.AddAuditing(identity);
            
            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";

            this._customerDataConnection.ExecuteProc<int>("saveProjectcode");

            int id = (int)this._customerDataConnection.Parameters.ReturnValue;

            this._customerDataConnection.Parameters.Clear();
            
            projectCode.Id = id;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Add completed with id {id}.");
            }

            if (projectCode.Id > 0)
            {
                this._cacheFactory.OnAdd += this.AddToMap;
                projectCode = this._cacheFactory.Save(projectCode);
            }

            return projectCode;
        }

        /// <summary>
        /// Inverse the archive state of the <see cref="IProjectCode"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProjectCode"/> to change the Archive state on.</param>
        /// <returns>A bool indicating the current archive state of the <see cref="IProjectCode"/> after execution.</returns>
        public bool Archive(int id)
        {
            IProjectCode projectCode = this[id];

            // Inverse the archived status
            projectCode.Archived = !projectCode.Archived;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlProjectCodesFactory.Archive({projectCode.Id}) called.");
            }

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@projectcodeid", SqlDbType.Int) { Value = projectCode.Id });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@archive", SqlDbType.Bit) { Value = projectCode.Archived });
            this._customerDataConnection.Parameters.AddAuditing(this._identityProvider.GetUserIdentity());

            this._customerDataConnection.ExecuteProc("changeProjectcodeStatus");

            this._customerDataConnection.Parameters.Clear();
            
            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"SqlProjectCodesFactory.Archive({id}) completed");
            }


            projectCode = this._cacheFactory.Save(projectCode);

            return projectCode.Archived;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IProjectCode"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProjectCode"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            UserIdentity identity = this._identityProvider.GetUserIdentity();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@projectcodeid", SqlDbType.Int) { Value = id });
            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeid", SqlDbType.Int) { Value = identity.EmployeeId });
            this._customerDataConnection.Parameters.AddAuditing(identity);
            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";
            int returnCode = this._customerDataConnection.ExecuteProc<int>("deleteProjectcode");

            // 0 is a successful delete; do not delete unless it has actually been deleted from the database
            if (returnCode == 0)
            {
                this._cacheFactory.OnDelete += this.DeleteFromHash;
                this._cacheFactory.Delete(id);
            }
            
            return returnCode;
        }

        /// <summary>
        /// Gets an List of all available <see cref="IProjectCode"/> 
        /// </summary>    
        /// <returns>The list of <see cref="IProjectCode"/>.</returns>
        public IList<IProjectCode> Get()
        {
            IList<IProjectCode> projectCodes = this._cacheFactory.Get();

            if (projectCodes == null)
            {
                projectCodes = this.GetFromDatabase(null);

                if (projectCodes.Count > 0)
                {
                    this._cacheFactory.OnAddMultiple += this.AddToMapMultiple;
                    this._cacheFactory.Add(projectCodes);
                }
            }

            return projectCodes;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IProjectCode"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IProjectCode"/> that match <paramref name="predicate"/>.</returns>
        public IList<IProjectCode> Get(Predicate<IProjectCode> predicate)
        {
            IList<IProjectCode> projectCodes = this.Get();

            if (predicate == null)
            {
                return projectCodes;
            }
            
            List<IProjectCode> matchedProjectCodes = new List<IProjectCode>();

            foreach (IProjectCode projectCode in projectCodes)
            {
                if (predicate.Invoke(projectCode))
                {
                    matchedProjectCodes.Add(projectCode);
                }
            }

            return matchedProjectCodes;
        }

        /// <summary>
        /// Gets an instance of <see cref="IProjectCode"/> if it matches the supplied <paramref name="customGet"/>.
        /// </summary>
        /// <param name="customGet">An instance of <see cref="GetByCustom"/> to retrieve a <see cref="IProjectCode"/> that matches the defined rules.</param>
        /// <returns>An instance of <see cref="IProjectCodeWithUserDefinedFields"/> that matches the rules defined in <paramref name="customGet"/>.</returns>
        public IProjectCode GetByCustom(GetByCustom customGet)
        {
            return this._cacheFactory.HashGet(customGet.HashName, customGet.Field);
        }

        /// <summary>
        /// Adds an instance of <see cref="IProjectCode"/> to the names and descriptions hash in cache.
        /// </summary>
        /// <param name="projectCode">The <see cref="IProjectCode"/> to add to cache.</param>
        private void AddToMap(IProjectCode projectCode)
        {
            if (projectCode.Id > 0)
            {
                IProjectCode cachedProjectCode = this._cacheFactory[projectCode.Id];

                if (cachedProjectCode != null)
                {
                    this.DeleteFromHash(cachedProjectCode);
                }
            }

            this._cacheFactory.HashAdd("names", projectCode.Name, projectCode);
            this._cacheFactory.HashAdd("descriptions", projectCode.Description, projectCode);
        }

        /// <summary>
        /// Adds multiple instances of <see cref="IProjectCode"/> to the names and descriptions hash in cache.
        /// </summary>
        /// <param name="projectCodes">Collection of <see cref="IProjectCode"/> to add to cache.</param>
        private void AddToMapMultiple(IList<IProjectCode> projectCodes)
        {
            IDictionary<string, object> hashProjectCodesByName = new Dictionary<string, object>();
            IDictionary<string, object> hashProjectCodesByDescription = new Dictionary<string, object>();

            foreach (IProjectCode projectCode in projectCodes)
            {
                if (hashProjectCodesByName.ContainsKey(projectCode.Name) == false)
                {
                    hashProjectCodesByName.Add(projectCode.Name, projectCode);
                }

                if (hashProjectCodesByDescription.ContainsKey(projectCode.Description) == false)
                {
                    hashProjectCodesByDescription.Add(projectCode.Description, projectCode);
                }
            }

            this._cacheFactory.HashAdd("names", hashProjectCodesByName);
            this._cacheFactory.HashAdd("descriptions", hashProjectCodesByDescription);
        }

        /// <summary>
        /// Deletes <paramref name="projectCode"/> from the names and descriptions hash in cache.
        /// </summary>
        /// <param name="projectCode">The <see cref="IProjectCode"/> to delete.</param>
        private void DeleteFromHash(IProjectCode projectCode)
        {
            this._cacheFactory.HashDelete("names", projectCode.Name);
            this._cacheFactory.HashDelete("descriptions", projectCode.Description);
        }

        /// <summary>
        /// Gets a collection of <see cref="IProjectCode"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IProjectCode"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IProjectCode"/></param>
        /// <returns>The required <see cref="IProjectCode"/> or <see langword="null" /> or a collection of <see cref="IProjectCode"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private List<IProjectCode> GetFromDatabase(int? id)
        {
            List<IProjectCode> projectCodes = new List<IProjectCode>();

            string sql = @"SELECT projectcodeid, projectcode, [description], archived, rechargeable FROM project_codes";

            if (id.HasValue)
            {
                sql += " where projectcodeid=@projectCodeId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@projectCodeId", SqlDbType.Int) { Value = id.Value });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int projectcodeIdOrdinal = reader.GetOrdinal("projectcodeid");
                int codeOrdinal = reader.GetOrdinal("projectcode");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int archivedOrdinal = reader.GetOrdinal("archived");
                int rechargeableOrdinal = reader.GetOrdinal("rechargeable");

                while (reader.Read())
                {
                    int projectCodeId = reader.GetInt32(projectcodeIdOrdinal);
                    string code = reader.GetString(codeOrdinal);
                    string description = reader.IsDBNull(descriptionOrdinal) == false ? reader.GetString(descriptionOrdinal) : string.Empty;
                    bool archived = reader.GetBoolean(archivedOrdinal);
                    bool rechargeable = reader.GetBoolean(rechargeableOrdinal);

                    IProjectCode projectCode = new ProjectCode(projectCodeId, code, description, archived, rechargeable);

                    projectCodes.Add(projectCode);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return projectCodes;
        }
    }
}
