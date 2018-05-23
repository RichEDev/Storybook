namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.Reasons;

    using CacheDataAccess.Caching;

    using SQLDataAccess.Reasons;

    /// <summary>
    /// The "Project Code" Mocked objects.
    /// </summary>
    public interface IReasons
    {
        /// <summary>
        /// Gets or sets the Mock'd <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        SqlReasonsFactory SqlReasonsFactory { get; set;}

        /// <summary>
        /// Gets or sets an instance of <see cref="AccountCacheFactory{T,TK}"/>
        /// </summary>
        AccountCacheFactory<IReason, int> CacheFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="ICache{T,TK}"/>.
        /// </summary>
        ICache<IReason, int> Cache { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IAccountCacheKey{T}"/>.
        /// </summary>
        IAccountCacheKey<int> CacheKey { get; set; }

        /// <summary>
        /// Gets or sets the an instance of <see cref="RepositoryBase{IReasons,Int32}"/>
        /// </summary>
        RepositoryBase<IReason, int> RepositoryBase { get; set; }
    }
}