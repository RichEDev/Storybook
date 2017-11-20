namespace CacheDataAccess.Caching
{
    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Interfaces;

    using Common.Logging;

    /// <summary>
    /// Extends <see cref="CacheFactory{T, K}"/> and implements methods to retrieve and create instances of <see cref="T"/> in cache for <see cref="IAccount"/> specific objects.
    /// </summary>
    /// <typeparam name="T">
    /// The entity this <see cref="AccountCacheFactory{T, K }"/> operates on.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the <typeparamref name="T"/>
    /// </typeparam>
    public class AccountCacheFactory<T, TK> : CacheFactory<T, TK>, IAccountCacheFactory<T, TK>
                where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountCacheFactory{T,TK}"/> class. 
        /// </summary>
        /// <param name="baseRepository">An instance of <see cref="RepositoryBase{T,TK}"/> for storing instances in memory.</param>
        /// <param name="cache">An instnace of <see cref="ICache{T,TK}"/> to enable storage in distributed caching</param>
        /// <param name="cacheKey">An instance of <see cref="IAccountCacheKey{T}"/> for creating a cache key for distributed caching.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public AccountCacheFactory(RepositoryBase<T, TK> baseRepository, ICache<T, TK> cache, IAccountCacheKey<TK> cacheKey, ILog logger)
            : base(baseRepository, cache, cacheKey, logger)
        {
        }
    }
}
