namespace CacheDataAccess.Caching
{
    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.Interfaces;

    using Common.Logging;

    /// <summary>
    /// Extends <see cref="CacheFactory{T, K}"/> and implements methods to retrieve and create instances of <see cref="T"/> in cache
    /// </summary>
    /// <typeparam name="T">
    /// The entity this <see cref="MetabaseCacheFactory{T, K }"/> operates on.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the <typeparamref name="T"/>
    /// </typeparam>
    public class MetabaseCacheFactory<T, TK> : CacheFactory<T, TK>, IMetabaseCacheFactory<T, TK>
                where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetabaseCacheFactory{T,TK}"/> class. 
        /// </summary>
        /// <param name="baseRepository">An instance of <see cref="RepositoryBase{T,TK}"/> for storing instances in memory.</param>
        /// <param name="cache">An instnace of <see cref="ICache{T,TK}"/> to enable storage in distributed caching</param>
        /// <param name="cacheKey">An instance of <see cref="IMetabaseCacheKey{T}"/> for creating a cache key for distributed caching.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public MetabaseCacheFactory(RepositoryBase<T, TK> baseRepository, ICache<T, TK> cache, IMetabaseCacheKey<TK> cacheKey, ILog logger)
            : base(baseRepository, cache, cacheKey, logger)
        {

        }
    }
}
