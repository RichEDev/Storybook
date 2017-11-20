namespace BusinessLogic.Cache
{
    using System.Collections.Generic;

    /// <summary>
    /// The Cache interface.
    /// </summary>
    /// <typeparam name="T">The entity type to cache</typeparam>
    /// <typeparam name="TK">The data type of the <see cref="ICacheKey{T}"/></typeparam>
    public interface ICache<T, TK>
    {
        /// <summary>
        /// Checks if cache contains a key value pair with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not a key value pair exists in cache with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </returns>
        bool Contains(ICacheKey<TK> key);
        
        /// <summary>
        /// Deletes the specified <see cref="ICache{T,TK}"/> <paramref name="key"/> and associated value from cache.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not <see cref="ICache{T,TK}"/> <paramref name="key"/> was deleted from cache.
        /// </returns>
        bool Delete(ICacheKey<TK> key);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key. 
        /// This command overwrites any existing fields in the hash. If key does not exist,
        /// a new key holding a hash is created.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to add.</param>
        /// <param name="field">The field to store the <paramref name="value"/> at.</param>  
        /// <param name="value">The value to store.</param>
        /// <returns>A bool indicating whether or not the value was stored successfully.</returns>
        bool HashAdd(ICacheKey<TK> key, string hashName, string field, object value);

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key. 
        /// This command overwrites any existing fields in the hash. If key does not exist,
        /// a new key holding a hash is created.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to add.</param>
        /// <param name="values">The values to store and their associated fields.</param>
        void HashAdd(ICacheKey<TK> key, string hashName, IDictionary<string, object> values);

        /// <summary>
        /// Removes the specified field from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this command returns false.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to delete from.</param>
        /// <param name="field">The field in <paramref name="hashName" /> to delete.</param>
        /// <returns>A bool indicating whether the field was deleted from the hash.</returns>
        bool HashDelete(ICacheKey<TK> key, string hashName, string field);

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to get the value from.</param>
        /// <param name="field">The name of the field to get the value from within the hash.</param>
        /// <returns>The value associated with field, or null when field is not present in the hash or key does not exist.</returns>
        T HashGet(ICacheKey<TK> key, string hashName, string field);

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The hash name to get all entries for.</param>
        /// <returns>A list of fields and their values stored in the hash, or null when the list is empty or when key does not exist.</returns>
        IList<T> HashGetAll(ICacheKey<TK> key, string hashName);

        /// <summary>
        /// Add a key value pair to cache for the specified account.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{T}"/> for this item.
        /// </param>
        /// <param name="value">
        /// The object to place in cache.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not this item was successfully stored in cache.
        /// </returns>
        bool StringAdd(ICacheKey<TK> key, object value);

        /// <summary>
        /// Add a key value pair to cache with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <param name="value">
        /// The object to place into cache.
        /// </param>
        /// <param name="timeOutMinutes">The duration this object should remain in cache.</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not this item was successfully stored in cache.
        /// </returns>
        bool StringAdd(ICacheKey<TK> key, object value, int timeOutMinutes);

        /// <summary>
        /// Adds a collection of objects to cache with the specified <see cref="ICache{T,TK}"/> and associated <see cref="ICacheKey{T}"/>
        /// </summary>
        /// <param name="values">The collection to add to cache.</param>
        /// <returns>A bool indicating whether or not the entities where successfully stored in cache.</returns>
        bool StringAdd(IDictionary<ICacheKey<TK>, object> values);

        /// <summary>
        /// Gets an object from cache with a matching <see cref="ICache{T,TK}"/> <paramref name="key"/> if present 
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> if found in cache.
        /// </returns>
        T StringGet(ICacheKey<TK> key);
    }
}