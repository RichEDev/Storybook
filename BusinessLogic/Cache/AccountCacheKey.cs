namespace BusinessLogic.Cache
{
    using BusinessLogic.Accounts;

    /// <summary>
    /// Defines a key for an account object for usage with cache.
    /// </summary>
    /// <typeparam name="T">
    /// The data type of the key this <see cref="AccountCacheKey{T}"/> should use.
    /// </typeparam>
    public class AccountCacheKey<T> : CacheKey<T>, IAccountCacheKey<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountCacheKey{T}"/> class. 
        /// </summary>
        /// <param name="account">
        /// An <see cref="IAccount"/> to cache items for.
        /// </param>
        public AccountCacheKey(IAccount account)
        {
            Guard.ThrowIfNull(account, nameof(account));

            this.AccountId = account.Id;
        }

        /// <summary>
        /// Gets the account id.
        /// </summary>
        public int AccountId { get; }

        /// <summary>
        /// Gets the cache key for this <see cref="AccountCacheKey{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> key for this <see cref="AccountCacheKey{T}"/></returns>
        public override string GetCacheKey()
        {
            return $"A:{this.AccountId}:{base.GetCacheKey()}";
        }

        /// <summary>
        /// Gets the string key for accessing a hash appended with <paramref name="hashName"/>.
        /// </summary>
        /// <param name="hashName"></param>
        /// <returns>The formatted cache key for accessing the hash</returns>
        public override string GetCacheKeyHash(string hashName)
        {
            return $"A:{this.AccountId}:{base.GetCacheKeyHash(hashName)}";
        }
    }
}