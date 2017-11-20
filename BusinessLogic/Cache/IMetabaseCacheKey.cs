namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defines a key for a metabase object for usage with cache.
    /// </summary>
    /// <typeparam name="T">
    /// The data type this <see cref="IMetabaseCacheKey{T}"/> uses for its Key property.
    /// </typeparam>
    public interface IMetabaseCacheKey<T> : ICacheKey<T>
    {
    }
}
