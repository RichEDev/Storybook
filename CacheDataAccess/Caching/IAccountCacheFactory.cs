namespace CacheDataAccess.Caching
{
    using BusinessLogic.Accounts;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Extends <see cref="ICacheFactory{T, K}"/> and implements methods to retrieve and create instances of <see cref="T"/> in cache for <see cref="IAccount"/> specific objects.
    /// </summary>
    /// <typeparam name="T">
    /// The entity this <see cref="IAccountCacheFactory{T, K }"/> operates on.
    /// </typeparam>
    /// <typeparam name="TK">The data type of the <typeparamref name="T"/>
    /// </typeparam>
    public interface IAccountCacheFactory<T, TK> : ICacheFactory<T, TK>
        where T : class, IIdentifier<TK>
    {
    }
}