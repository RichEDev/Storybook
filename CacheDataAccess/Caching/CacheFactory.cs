namespace CacheDataAccess.Caching
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.Interfaces;

    using Common.Logging;

    /// <summary>
    /// A delegate for when an item is inserted into cache.
    /// </summary>
    /// <typeparam name="T">The entity type <see cref="CacheFactory{T,TK}"/> operates on.</typeparam>
    /// <param name="value">The object being inserted into cache.</param>
    public delegate void CacheAddEventHandler<in T>(T value);

    /// <summary>
    /// A delegate for when multiple items are inserted into cache.
    /// </summary>
    /// <typeparam name="T">The entity type <see cref="CacheFactory{T,TK}"/> operates on.</typeparam>
    /// <param name="values">The objects being inserted into cache.</param>
    public delegate void CacheAddMultipleEventHandler<T>(IList<T> values);

    /// <summary>
    /// A delegate for when an item is deleted from cache.
    /// </summary>
    /// <typeparam name="T">The entity type <see cref="CacheFactory{T,TK}"/> operates on.</typeparam>
    /// <param name="value">The object being deleted from cache.</param>
    public delegate void CacheDeleteEventHandler<in T>(T value);

    /// <summary>
    /// A delegate for when multple items are deleted from cache.
    /// </summary>
    /// <typeparam name="T">The entity type <see cref="CacheFactory{T,TK}"/> operates on.</typeparam>
    /// <param name="values">The objects being deleted from cache.</param>
    public delegate void CacheDeleteMultipleEventHandler<T>(IList<T> values);

    /// <summary>
    /// Extends <see cref="CacheFactory{T, K}"/> and implements methods to retrieve and create instances of <see cref="T"/> in cache
    /// </summary>
    /// <typeparam name="T">
    /// The entity this <see cref="CacheFactory{T, K }"/> operates on.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the <typeparamref name="T"/>
    /// </typeparam>
    public abstract class CacheFactory<T, TK> : ICacheFactory<T, TK>
                where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// An instance of <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        private readonly RepositoryBase<T, TK> _baseRepository;

        /// <summary>
        /// An instance of cache to use for retrieving and creating cache objects
        /// </summary>
        private readonly ICache<T, TK> _cache;

        /// <summary>
        /// <see cref="CacheKey{TK}"/> used for caching this <typeparamref name="T"/>.
        /// </summary>
        private readonly ICacheKey<TK> _cacheKey;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="CacheFactory{T, TK}"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheFactory{T,TK}"/> class.  
        /// </summary>
        /// <param name="baseRepository">
        /// An instance of <see cref="RepositoryBase{T, TK}"/> to use for retrieving and creating instances of <see cref="T"/> in memory.
        /// </param>
        /// <param name="cache">
        /// An instance of <see cref="ICache{T, TK}"/> to use for retrieving and creating instances of <see cref="T"/> in cache
        /// </param>
        /// <param name="cacheKey">
        /// An instance of <see cref="CacheKey{T}"/> to use for caching instances of <typeparamref name="T"/>
        /// </param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="baseRepository{T}"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cacheKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
        protected CacheFactory(RepositoryBase<T, TK> baseRepository, ICache<T, TK> cache, ICacheKey<TK> cacheKey, ILog logger)
        {
            Guard.ThrowIfNull(baseRepository, nameof(baseRepository));
            Guard.ThrowIfNull(cache, nameof(cache));
            Guard.ThrowIfNull(cacheKey, nameof(cacheKey));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._cacheKey = cacheKey;

            // Set cache key so the area part of the key will be equal to the type of T
            this._cacheKey.Area = typeof(T).Name;

            this._cache = cache;
            this._baseRepository = baseRepository;
            this._logger = logger;
        }

        /// <summary>
        /// The event hook for when an item is inserted into cache.
        /// </summary>
        public event CacheAddEventHandler<T> OnAdd;

        /// <summary>
        /// The event hook for when multiple items are inserted into cache.
        /// </summary>
        public event CacheAddMultipleEventHandler<T> OnAddMultiple;

        /// <summary>
        /// The event hook for when an item is deleted from cache.
        /// </summary>
        public event CacheDeleteEventHandler<T> OnDelete;

        /// <summary>
        /// The event hook for when multiple items are deleted from cache.
        /// </summary>
        /// 
        public event CacheDeleteMultipleEventHandler<T> OnDeleteMultiple;

        /// <summary>
        /// Gets an instance of <see cref="T"/> with a matching ID from memory if possible, if not it will search cache for an entry, and add to memory
        /// </summary>
        /// <param name="id">The ID of the <see cref="T"/> you want to retrieve</param>
        /// <returns>The required <see cref="T"/> or null if it cannot be found</returns>
        public T this[TK id]
        {
            get
            {
                T entity = this._baseRepository[id];

                if (entity == null)
                {
                    entity = this.Get(id);

                    if (entity != null)
                    {
                        this._baseRepository.Add(entity);
                    }
                }

                return entity;
            }
        }

        /// <summary>
        /// Adds an instance of <see cref="T"/> to memory and cache
        /// </summary>
        /// <param name="entity">The instance of <see cref="T"/> to add to memory and cache</param>
        /// <returns>The <see cref="T"/>.</returns>
        public T Add(T entity)
        {
            if (entity != null)
            {
                this.OnAdd?.Invoke(entity);
                this.HashAdd("list", entity.Id.ToString(), entity);
                this._baseRepository.Add(entity);
            }
            
            return entity;
        }

        /// <summary>
        /// Adds multiple instances of <see cref="T"/> to memory and cache
        /// </summary>
        /// <param name="entities">The instances of <see cref="T"/> to add to memory and cache</param>
        /// <returns>The instances of <see cref="T"/>.</returns>
        public IList<T> Add(IList<T> entities)
        {
            Guard.ThrowIfNull(entities, nameof(entities));

            if (entities.Count > 0)
            {
                IDictionary<string, object> cacheList = new Dictionary<string, object>();

                foreach (T entity in entities)
                {
                    this._baseRepository.Add(entity);
                    cacheList.Add(entity.Id.ToString(), entity);
                }

                this.HashAdd("list", cacheList);

                this.OnAddMultiple?.Invoke(entities);
            }

            return entities;
        }

        /// <summary>
        /// Deletes an instance of <see cref="T"/> from memory and cache.
        /// </summary>
        /// <param name="id">The <see cref="TK"/> id of the <see cref="T"/> to delete.</param>
        /// <returns>A <see cref="bool"/> indicating wheter or not the <see cref="T"/> has been deleted.</returns>
        public bool Delete(TK id)
        {
            this._cacheKey.Key = id;

            if (this.OnDelete != null)
            {
                T entity = this[id];

                this.OnDelete.Invoke(entity);
            }

            return this._cache.HashDelete(this._cacheKey, "list", id.ToString());
        }

        /// <summary>
        /// Gets all instances of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A collection of <typeparamref name="T"/>.</returns>
        public IList<T> Get()
        {
            return this._cache.HashGetAll(this._cacheKey, "list");
        }

        /// <summary>
        /// Adds a value to the specified <paramref name="hashName"/> at <paramref name="field"/>.
        /// </summary>
        /// <param name="hashName">The hash to add the <see cref="IDictionary{TKey,TValue}"/>.</param>
        /// <param name="field">The name of the field to set this value with.</param>
        /// <param name="value">The <see cref="IDictionary{TKey,TValue}"/> to add to the specified <paramref name="hashName"/>.</param>
        public bool HashAdd(string hashName, string field, object value)
        {
            return this._cache.HashAdd(this._cacheKey, hashName, field, value);
        }

        /// <summary>
        /// Adds a collection of <see cref="IDictionary{TKey,TValue}"/> to the specified <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName">The hash to add the <see cref="IDictionary{TKey,TValue}"/>.</param>
        /// <param name="value">The <see cref="IDictionary{TKey,TValue}"/> to add to the specified <paramref name="hashName"/>.</param>
        public void HashAdd(string hashName, IDictionary<string, object> value)
        {
            this._cache.HashAdd(this._cacheKey, hashName, value);
        }

        /// <summary>
        /// Deletes <paramref name="field"/> from the specified <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName">The hash to delete the <paramref name="field"/> from.</param>
        /// <param name="field">The field to delete from the specified <paramref name="hashName"/>.</param>
        public void HashDelete(string hashName, string field)
        {
            Guard.ThrowIfNullOrWhiteSpace(hashName, nameof(hashName));
            Guard.ThrowIfNullOrWhiteSpace(field, nameof(field));

            this._cache.HashDelete(this._cacheKey, hashName, field);
        }

        /// <summary>
        /// Gets a specific <typeparamref name="T"/> from the specified <paramref name="hashName"/> and <paramref name="field"/>.
        /// </summary>
        /// <param name="hashName">The hashName to get the <typeparamref name="T"/> instance from.</param>
        /// <param name="field">The field to get the specific <typeparamref name="T"/> from.</param>
        /// <returns>The instance of <typeparamref name="T"/> or null if it does not exist in cache or memory.</returns>
        public T HashGet(string hashName, string field)
        {
            return this._cache.HashGet(this._cacheKey, hashName, field);
        }

        /// <summary>
        /// Gets an instance of <see cref="T"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="T"/> you want to retrieve</param>
        /// <returns>The required <see cref="T"/></returns>
        private T Get(TK id)
        {
            this._cacheKey.Key = id;
            return this._cache.HashGet(this._cacheKey, "list", this._cacheKey.Key.ToString());
        }
    }
}
