namespace BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Interfaces;

    using Common.Logging;

    /// <summary>
    /// Repository for storing and accessing instances of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity this class should be working with.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the Id of <typeparamref name="T"/>.
    /// </typeparam>
    public class RepositoryBase<T, TK> : IGetBy<T, TK>, ISave<T>
        where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// Backing collection of <see cref="T"/>.
        /// </summary>
        protected readonly List<T> BackingCollection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="RepositoryBase{T,TK}"/> diagnostics and information.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{T,TK}"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public RepositoryBase(ILog logger)
        {
            Guard.ThrowIfNull(logger, nameof(logger));

            this.BackingCollection = new List<T>();
            this._logger = logger;
        }

        /// <summary>
        /// Gets an instance of <see cref="T"/> with a matching ID from memory if possible.
        /// </summary>
        /// <param name="id">The ID of the <see cref="T"/> you want to retrieve</param>
        /// <returns>The required <see cref="T"/> or null if it cannot be found</returns>
        public virtual T this[TK id]
        {
            get
            {
                return this.BackingCollection.FirstOrDefault(x => x.Id.Equals(id));
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="T"/> to memory in this instance of <see cref="RepositoryBase{T,K}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="T"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="T"/>.</returns>
        public virtual T Save(T entity)
        {
            if (entity != null)
            {
                // Remove any existing entity with matching ID, prevents duplicates and provides a quick and easy way to update the entity
                this.BackingCollection.RemoveAll(x => x.Id.Equals(entity.Id));
                this.BackingCollection.Add(entity);
            }

            return entity;
        }
    }
}
