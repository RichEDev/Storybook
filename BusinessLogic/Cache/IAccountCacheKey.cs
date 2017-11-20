namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defines a key for an account object for usage with cache.
    /// </summary>
    /// <typeparam name="T">
    /// The data type of the key this <see cref="IAccountCacheKey{T}"/> should use.
    /// </typeparam>
    public interface IAccountCacheKey<T> : ICacheKey<T>
    {
    }
}
