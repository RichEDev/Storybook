namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defines a <see cref="ICacheKey{T}"/> for usage with <see cref="ICache{T,TK}"/>.
    /// </summary>
    /// <typeparam name="T">The data type of the Key property.</typeparam>
    public interface ICacheKey<T>
    {
        /// <summary>
        /// Gets or sets the area for this <see cref="ICacheKey{T}"/>.
        /// </summary>
        string Area { get; set; }

        /// <summary>
        /// Gets or sets the key for this <see cref="ICacheKey{T}"/>.
        /// </summary>
        T Key { get; set; }

        /// <summary>
        /// Gets the cache key for this <see cref="ICacheKey{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> key for this <see cref="ICacheKey{T}"/></returns>
        string GetCacheKey();

        /// <summary>
        /// Gets the string key for accessing a hash appended with <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName"></param>
        /// <returns>The formatted cache key for accessing the hash</returns>
        string GetCacheKeyHash(string hashName);
    }
}
