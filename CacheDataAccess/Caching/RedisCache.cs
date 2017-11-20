namespace CacheDataAccess.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.Interfaces;

    using Common.Logging;

    using Configuration.Interface;

    using StackExchange.Redis;

    /// <summary>
    /// Implementation of Redis cache
    /// </summary>
    /// <typeparam name="T">The entity type to cache</typeparam>
    /// <typeparam name="TK">The data type of the <see cref="IIdentifier{TK}"/> of <typeparamref name="T"/></typeparam>
    /// <remarks>Must be created as a singleton via IoC</remarks>
    public class RedisCache<T, TK> : ICache<T, TK> where T : class
    {
        /// <summary>
        /// An instance of <see cref="IDatabase"/> currently being used for caching via Redis
        /// </summary>
        private readonly IDatabase _database;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// An instance of <see cref="ISerialize"/> to use when (de)serializing data for storage in or retrieval from Redis.
        /// </summary>
        private readonly ISerialize _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache{T,TK}"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <param name="configurationManager">An instance of <see cref="IConfigurationManager"/> to access configuration files.</param>
        /// <param name="serializer">An instance of <see cref="ISerialize"/> to use when (de)serializing data for storage in or retrieval from Redis.</param>
        public RedisCache(ILog logger, IConfigurationManager configurationManager, ISerialize serializer)
        {
            Guard.ThrowIfNull(logger, nameof(logger));
            Guard.ThrowIfNull(configurationManager, nameof(configurationManager));
            Guard.ThrowIfNull(serializer, nameof(serializer));

            this._database = RedisClientWrapper.Connection(configurationManager.ConnectionStrings["redis"].ConnectionString);
            this._log = logger;
            this._serializer = serializer;
        }

        /// <summary>
        /// Checks if Redis contains a key value pair with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not a key value pair exists in Redis with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </returns>
        public bool Contains(ICacheKey<TK> key)
        {
            string cacheKey = key.GetCacheKey();
            bool response = this._database.KeyExists(cacheKey);

            if (this._log.IsDebugEnabled)
            {
                this._log.DebugFormat("Checking if Redis contains the key {0} and returned {1}", cacheKey, response ? "exists" : "does not exist");
            }

            return response;
        }

        /// <summary>
        /// Deletes the specified <see cref="ICache{T,TK}"/> <paramref name="key"/> and associated value from Redis.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not <see cref="ICache{T,TK}"/> <paramref name="key"/> was deleted from Redis.
        /// </returns>
        public bool Delete(ICacheKey<TK> key)
        {
            string cacheKey = key.GetCacheKey();
            bool response = this._database.KeyDelete(cacheKey);

            if (this._log.IsDebugEnabled)
            {
                this._log.DebugFormat("Deleting the key {0} from Redis {1}", cacheKey, response ? "succeeded" : "failed");
            }

            return response;
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key. 
        /// This command overwrites any existing fields in the hash. If key does not exist,
        /// a new key holding a hash is created.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to add.</param>
        /// <param name="values">The values to store and their associated fields.</param>
        public void HashAdd(ICacheKey<TK> key, string hashName, IDictionary<string, object> values)
        {
            string cacheKey = key.GetCacheKeyHash(hashName);

            List<HashEntry> entries = new List<HashEntry>();

            foreach (KeyValuePair<string, object> keyValuePair in values)
            {
                entries.Add(new HashEntry(keyValuePair.Key, this._serializer.Serialize(keyValuePair.Value)));
            }
            
            this._database.HashSet(cacheKey, entries.ToArray());

            this._log.InfoFormat("HashAdd cache with key {0} succeeded", cacheKey);
        }

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
        public bool HashAdd(ICacheKey<TK> key, string hashName, string field, object value)
        {
            string cacheKey = key.GetCacheKeyHash(hashName);
            bool response = this._database.HashSet(cacheKey, field, this._serializer.Serialize(value));

            if (response && this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("HashAdd cache with key {0} succeeded", cacheKey);
            }

            if (response == false && this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("HashAdd cache with key {0} already exists, value updated", cacheKey);
            }

            return response;
        }

        /// <summary>
        /// Removes the specified field from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this command returns false.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to delete from.</param>
        /// <param name="field">The field in <paramref name="hashName" /> to delete.</param>
        /// <returns>A bool indicating whether the field was deleted from the hash.</returns>
        public bool HashDelete(ICacheKey<TK> key, string hashName, string field)
        {
            string cacheKey = key.GetCacheKeyHash(hashName);
            bool response = this._database.HashDelete(cacheKey, field);

            if (this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("HashDelete cache with key {0} for value [{1}] {2}", cacheKey, field, response ? "succeeded" : "failed");
            }

            return response;
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The name of the hash to get the value from.</param>
        /// <param name="field">The name of the field to get the value from within the hash.</param>
        /// <returns>The value associated with field, or null when field is not present in the hash or key does not exist.</returns>
        public T HashGet(ICacheKey<TK> key, string hashName, string field)
        {
            RedisValue redisValue = this._database.HashGet(key.GetCacheKeyHash(hashName), field);

            if (redisValue.HasValue == false)
            {
                return null;
            }

            T response = this._serializer.Deserialize<T>(redisValue);

            return response;
        }

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{TK}"/> for this item.</param>
        /// <param name="hashName">The hash name to get all entries for.</param>
        /// <returns>A list of fields and their values stored in the hash, or null when the list is empty or when key does not exist.</returns>
        public IList<T> HashGetAll(ICacheKey<TK> key, string hashName)
        {
            IList<T> list = new List<T>();
            HashEntry[] entries = this._database.HashGetAll(key.GetCacheKeyHash(hashName));

            if (entries.Length == 0)
            {
                return null;
            }

            foreach (HashEntry hashEntry in entries)
            {
                list.Add(this._serializer.Deserialize<T>(hashEntry.Value));
            }

            return list;
        }

        /// <summary>
        /// Add a key value pair to Redis with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <see cref="ICacheKey{T}"/> for this item.</param>
        /// <param name="value">The object to place in cache.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not this item was successfully stored in Redis.</returns>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the <paramref name="value" /> parameter is not marked as serializable. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public bool StringAdd(ICacheKey<TK> key, object value)
        {
            string cacheKey = key.GetCacheKey();
            bool response = this._database.StringSet(cacheKey, this._serializer.Serialize(value));
            
            if (response && this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("Add cache with key {0} succeeded", cacheKey);
            }

            if (response == false && this._log.IsWarnEnabled)
            {
                this._log.WarnFormat("Add cache with key {0} failed", cacheKey);
            }

            return response;
        }

        /// <summary>
        /// Add a collection of key value pairs to Redis with a specified <see cref="ICache{T,TK}"/>.
        /// </summary>
        /// <param name="values">Collection of <see cref="ICacheKey{T}"/> and values to add to Redis</param>
        /// <returns>A <see cref="bool"/> indicating whether or not these items were successfully stored in Redis.</returns>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the <paramref name="values" /> parameter is not marked as serializable. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public bool StringAdd(IDictionary<ICacheKey<TK>, object> values)
        {
            List<KeyValuePair<RedisKey, RedisValue>> serializedValues = new List<KeyValuePair<RedisKey, RedisValue>>();

            foreach (KeyValuePair<ICacheKey<TK>, object> keyValuePair in values)
            {
                RedisKey key = keyValuePair.Key.GetCacheKey();
                RedisValue value = this._serializer.Serialize(keyValuePair.Value);

                serializedValues.Add(new KeyValuePair<RedisKey, RedisValue>(key, value));

            }
            
            bool response = this._database.StringSet(serializedValues.ToArray());

            if (response && this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("Add {0} cache keys succeeded", values.Count);
            }

            if (response == false && this._log.IsWarnEnabled)
            {
                this._log.InfoFormat("Add {0} cache keys failed", values.Count);
            }

            return response;
        }

        /// <summary>
        /// Add a key value pair to Redis with the specified <see cref="ICache{T,TK}"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <param name="value">
        /// The object to place into Redis.
        /// </param>
        /// <param name="timeOutMinutes">The duration this object should remain in Redis.</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether or not this item was successfully stored in Redis.
        /// </returns>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the <paramref name="value" /> parameter is not marked as serializable. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public bool StringAdd(ICacheKey<TK> key, object value, int timeOutMinutes)
        {
            string cacheKey = key.GetCacheKey();
            bool response = this._database.StringSet(cacheKey, this._serializer.Serialize(value), new TimeSpan(0, timeOutMinutes, 0));

            if (response && this._log.IsInfoEnabled)
            {
                this._log.InfoFormat("Add cache with key {0} succeeded", cacheKey);
            }

            if (response == false && this._log.IsWarnEnabled)
            {
                this._log.WarnFormat("Add cache with key {0} failed", cacheKey);
            }

            return response;
        }

        /// <summary>
        /// Gets an object from Redis with a matching <see cref="ICache{T,TK}"/> <paramref name="key"/> if present 
        /// </summary>
        /// <param name="key">
        /// The <see cref="ICacheKey{TK}"/> for this item.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> if found in Redis or <see langword="null"/> if not.
        /// </returns>
        /// <exception cref="SerializationException">The response from Redis cannot be deserialized. -or-The target type is a <see cref="T:System.Decimal" />, but the value is out of range of the <see cref="T:System.Decimal" /> type.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public T StringGet(ICacheKey<TK> key)
        {
            string cacheKey = key.GetCacheKey();
            RedisValue redisValue = this._database.StringGet(cacheKey);

            if (redisValue.HasValue == false)
            {
                if (this._log.IsDebugEnabled)
                {
                    this._log.DebugFormat("Get the value for key {0} from Redis returned null", cacheKey);
                }

                return null;
            }

            T response = this._serializer.Deserialize<T>(redisValue);

            if (this._log.IsDebugEnabled)
            {
                this._log.DebugFormat("Get the value for key {0} from Redis returned an instance of {1}", cacheKey, response.GetType().Name);
            }

            return response;
        }
    }
}
