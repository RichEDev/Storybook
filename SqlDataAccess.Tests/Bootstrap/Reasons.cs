namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.Reasons;

    using CacheDataAccess.Caching;

    using NSubstitute;

    using SQLDataAccess.ProjectCodes;
    using SQLDataAccess.Reasons;

    /// <summary>
    /// The "Project Code" Mocked objects.
    /// </summary>
    public class Reasons : IReasons
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCodes"/> class.
        /// </summary>
        /// <param name="system">
        /// An instance of <see cref="ISystem"/>.
        /// </param>
        public Reasons(ISystem system)
        {
            this.Cache = Substitute.For<ICache<IReason, int>>();
            this.CacheKey = Substitute.For<AccountCacheKey<int>>(Substitute.For<IAccount>());
            this.RepositoryBase = Substitute.For<RepositoryBase<IReason, int>>(system.Logger);

            this.CacheFactory = Substitute.For<AccountCacheFactory<IReason, int>>(this.RepositoryBase, this.Cache, this.CacheKey, system.Logger);
            this.SqlReasonsFactory = Substitute.For<SqlReasonsFactory>(
                this.CacheFactory,
                system.CustomerDataConnection,
                system.IdentityProvider,
                system.Logger);
        }
        
        /// <summary>
        /// Gets or sets the Mock'd <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        public SqlProjectCodesWithUserDefinedValuesFactory SqlProjectCodesWithUserDefinedValuesFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="AccountCacheFactory{T,TK}"/>
        /// </summary>
        public AccountCacheFactory<IReason, int> CacheFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="SqlReasonsFactory"/>
        /// </summary>
        public SqlReasonsFactory SqlReasonsFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="ICache{T,TK}"/>.
        /// </summary>
        public ICache<IReason, int> Cache { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IAccountCacheKey{T}"/>.
        /// </summary>
        public IAccountCacheKey<int> CacheKey { get; set; }

        /// <summary>
        /// Gets or sets the an instance of <see cref="IProjectCodes.RepositoryBase"/>
        /// </summary>
        public RepositoryBase<IReason, int> RepositoryBase { get; set; }
    }
}