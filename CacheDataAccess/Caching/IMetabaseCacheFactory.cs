namespace CacheDataAccess.Caching
{
    using System.Collections.Generic;

    using BusinessLogic.Interfaces;

    public interface IMetabaseCacheFactory<T, TK> : ICacheFactory<T, TK>
        where T : class, IIdentifier<TK>
    {
        /// <summary>
        /// Deletes an instance of <see cref="T"/> from memory and cache.
        /// </summary>
        /// <param name="id">The <see cref="TK"/> id of the <see cref="T"/> to delete.</param>
        /// <returns>A <see cref="bool"/> indicating wheter or not the <see cref="T"/> has been deleted.</returns>
        bool Delete(TK id);

        /// <summary>
        /// Gets all instances of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A collection of <typeparamref name="T"/>.</returns>
        IList<T> Get();
    }
}
