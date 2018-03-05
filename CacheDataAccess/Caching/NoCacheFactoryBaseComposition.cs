namespace CacheDataAccess.Caching
{
    using System;
    using BusinessLogic;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Extends <see cref="NoCacheFactoryBase{T, K}"/> and implements methods to retrieve and create instances of <see cref="T"/> in cache
    /// </summary>
    /// <typeparam name="T">
    /// The entity this <see cref="NoCacheFactoryBase{T, K }"/> operates on.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the <typeparamref name="T"/>
    /// </typeparam>
    public class NoCacheFactoryBase<T, TK> : ICacheFactory<T, TK>
                where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// An instance of <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        private readonly RepositoryBase<T, TK> _baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoCacheFactoryBase{T,TK}"/> class.  
        /// </summary>
        /// <param name="baseRepository">
        /// An instance of <see cref="RepositoryBase{T, TK}"/> to use for retrieving and creating instances of <see cref="T"/> in memory.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseRepository{T}"/> is <see langword="null"/>.
        /// </exception>
        public NoCacheFactoryBase(RepositoryBase<T, TK> baseRepository)
        {
            if (baseRepository == null)
            {
                throw new ArgumentNullException(nameof(baseRepository));
            }

            this._baseRepository = baseRepository;
        }

        /// <summary>
        /// Gets an instance of <see cref="T"/> with a matching ID from memory if possible, if not it will search cache for an entry, and add to memory
        /// </summary>
        /// <param name="id">The ID of the <see cref="T"/> you want to retrieve</param>
        /// <returns>The required <see cref="T"/> or null if it cannot be found</returns>
        public T this[TK id] => this._baseRepository[id];

        /// <summary>
        /// Adds an instance of <see cref="T"/> to memory and cache
        /// </summary>
        /// <param name="entity">
        /// The instance of <see cref="T"/> to add to memory and cache
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Save(T entity)
        {
            if (entity != null)
            {
                this._baseRepository.Save(entity);
            }

            return entity;
        }
    }
}
