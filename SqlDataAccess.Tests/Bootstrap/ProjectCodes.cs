namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;

    using CacheDataAccess.Caching;

    using NSubstitute;

    using SQLDataAccess.ProjectCodes;

    /// <summary>
    /// The "Project Code" Mocked objects.
    /// </summary>
    public class ProjectCodes : IProjectCodes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCodes"/> class.
        /// </summary>
        /// <param name="system">
        /// An instance of <see cref="ISystem"/>.
        /// </param>
        /// <param name="fields">
        /// An instance of <see cref="IFields"/>.
        /// </param>
        /// <param name="tables">
        /// An instance of <see cref="ITables"/>.
        /// </param>
        public ProjectCodes(ISystem system, IFields fields, ITables tables)
        {
            this.Cache = Substitute.For<ICache<IProjectCode, int>>();
            this.CacheKey = Substitute.For<AccountCacheKey<int>>(Substitute.For<IAccount>());
            this.RepositoryBase = Substitute.For<RepositoryBase<IProjectCode, int>>(system.Logger);

            this.CacheFactory = Substitute.For<AccountCacheFactory<IProjectCode, int>>(this.RepositoryBase, this.Cache, this.CacheKey, system.Logger);
            this.SqlProjectCodesFactory = Substitute.For<SqlProjectCodesFactory>(
                this.CacheFactory,
                system.CustomerDataConnection,
                system.IdentityProvider,
                system.Logger);
            
            this.SqlProjectCodesWithUserDefinedValuesFactory = new SqlProjectCodesWithUserDefinedValuesFactory(this.SqlProjectCodesFactory, fields.UserDefinedFieldValueRepository, system.CustomerDataConnection, system.Logger, fields.UserDefinedFieldRepository, tables.TableRepository);
        }
        
        /// <summary>
        /// Gets or sets the Mock'd <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        public SqlProjectCodesWithUserDefinedValuesFactory SqlProjectCodesWithUserDefinedValuesFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="AccountCacheFactory{T,TK}"/>
        /// </summary>
        public AccountCacheFactory<IProjectCode, int> CacheFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IProjectCodes.SqlProjectCodesFactory"/>
        /// </summary>
        public SqlProjectCodesFactory SqlProjectCodesFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="ICache{T,TK}"/>.
        /// </summary>
        public ICache<IProjectCode, int> Cache { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IAccountCacheKey{T}"/>.
        /// </summary>
        public IAccountCacheKey<int> CacheKey { get; set; }

        /// <summary>
        /// Gets or sets the an instance of <see cref="IProjectCodes.RepositoryBase"/>
        /// </summary>
        public RepositoryBase<IProjectCode, int> RepositoryBase { get; set; }
    }
}