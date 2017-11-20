namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defines a key for a metabase object for usage with cache.
    /// </summary>
    /// <typeparam name="T">
    /// The data type this <see cref="MetabaseCacheKey{T}"/> uses for its Key property.
    /// </typeparam>
    public class MetabaseCacheKey<T> : CacheKey<T>, IMetabaseCacheKey<T>
    {
        /// <summary>
        /// Gets the cache key for this <see cref="MetabaseCacheKey{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> key for this <see cref="MetabaseCacheKey{T}"/></returns>
        public override string GetCacheKey()
        {
            return $"M:{base.GetCacheKey()}";
        }

        /// <summary>
        /// Gets the string key for accessing a hash appended with <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName"></param>
        /// <returns>The formatted cache key for accessing the hash</returns>
        public override string GetCacheKeyHash(string hashName)
        {
            return $"M:{base.GetCacheKeyHash(hashName)}";
        }
    }
}