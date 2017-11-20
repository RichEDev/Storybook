namespace CacheDataAccess.Tables
{
    using System;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Tables;
    using BusinessLogic.Tables.Type;

    using Common.Logging;

    /// <summary>
    /// The cache table factory.
    /// </summary>
    public class CacheTableFactory : TableRepository
    {
        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "Table";

        /// <summary>
        /// The _account.
        /// </summary>
        protected readonly IAccount Account;

        /// <summary>
        /// The _cache.
        /// </summary>
        private readonly ICache<ITable, Guid> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheTableFactory"/> class. 
        /// </summary>
        /// <param name="account">An implementation of <see cref="IAccount"/></param>
        /// <param name="cache">An implementation of <see cref="ICache{T, TK}"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public CacheTableFactory(IAccount account, ICache<ITable, Guid> cache, ILog logger) : base(logger)
        {
            this.Account = account;
            this._cache = cache;
        }

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> with a matching ID from memory if possible.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ITable"/> you want to retrieve</param>
        /// <returns>The required <see cref="ITable"/> or null if it cannot be found</returns>
        public override ITable this[Guid id]
        {
            get
            {
                ITable table = base[id];

                if (table == null)
                {
                    table = this.Get(id);
                    this.Add(table);
                }

                return table;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> with a matching name from memory if possible.
        /// </summary>
        /// <param name="name">The name of the <see cref="ITable"/> you want to retrieve</param>
        /// <returns>The required <see cref="ITable"/> or null if it cannot be found</returns>
        public override ITable this[string name] => null;

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> which is the parent of the <see cref="Guid"/> given.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ITable"/></param>
        /// <returns>the parent <see cref="ITable"/></returns>
        public override ITable GetParentTable(Guid id)
        {
            return null;
        }

        /// <summary>
        /// Get a specific <see cref="ITable"/> by its Id.
        /// </summary>
        /// <param name="id">
        /// The id to get.
        /// </param>
        /// <returns>
        /// The <see cref="ITable"/> or null if not found.
        /// </returns>
        private ITable Get(Guid id)
        {
            CacheKey<Guid> cacheKey = new AccountCacheKey<Guid>(this.Account)
            {
                Area = typeof(ITable).FullName,
                Key = id
            };

            return this._cache.StringGet(cacheKey);
        }
    }
}
