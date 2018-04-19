namespace BusinessLogic.GeneralOptions
{
    using BusinessLogic.AccountProperties;

    /// <summary>
    /// Defines a <see cref="AccountPropertyCacheKey"/> and all it's members
    /// </summary>
    public class AccountPropertyCacheKey
    {
        /// <summary>
        /// Constructor for this <see cref="AccountPropertyCacheKey"/> that creates the unique identifier to  get account properties from cache
        /// </summary>
        /// <param name="key">The key for the <see cref="IAccountProperty"/></param>
        /// <param name="subAccountId">The sub account id to get the <see cref="IAccountProperty"/> for</param>
        public AccountPropertyCacheKey(string key, string subAccountId)
        {
            this.CacheKey = $"{subAccountId}/{key}";
            this.SubAccountId = subAccountId;
            this.Key = key;
        }

        /// <summary>
        /// Get the cache key for the <see cref="IAccountProperty"/>
        /// </summary>
        public string CacheKey { get; }

        /// <summary>
        /// Get the key for the <see cref="IAccountProperty"/>
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Get the key for the <see cref="IAccountProperty"/>
        /// </summary>
        public string SubAccountId { get; }
    }
}
