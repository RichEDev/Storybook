namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defines a <see cref="CacheKey{T}"/> for usage with <see cref="ICache{T,TK}"/>.
    /// </summary>
    /// <typeparam name="T">The data type of the Key property.</typeparam>
    public abstract class CacheKey<T> : ICacheKey<T>
    {
        /// <summary>
        /// Gets or sets the area for this <see cref="CacheKey{T}"/>.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the key for this <see cref="CacheKey{T}"/>.
        /// </summary>
        public T Key { get; set; }

        /// <summary>
        /// Gets the cache key for this <see cref="CacheKey{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> key for this <see cref="CacheKey{T}"/></returns>
        public virtual string GetCacheKey()
        {
            Guard.ThrowIfNullOrWhiteSpace(this.Area, nameof(this.Area));
            Guard.ThrowIfNull(this.Key, nameof(this.Key));

            return $"{this.Area}:{this.Key}";
        }

        /// <summary>
        /// Gets the string key for accessing a hash appended with <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName"></param>
        /// <returns>The formatted cache key for accessing the hash</returns>
        public virtual string GetCacheKeyHash(string hashName)
        {
            Guard.ThrowIfNullOrWhiteSpace(this.Area, nameof(this.Area));
            Guard.ThrowIfNullOrWhiteSpace(hashName, nameof(hashName));

            return $"{this.Area}:{hashName}";
        }
    }
}