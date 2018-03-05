namespace CacheDataAccess.Fields
{
    using System;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;

    using Common.Logging;

    /// <summary>
    /// The cache field factory.
    /// </summary>
    public class CacheFieldFactory : FieldRepository
    {
        /// <summary>
        /// The account.
        /// </summary>
        private readonly IAccount _account;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly ICache<IField, Guid> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheFieldFactory"/> class. 
        /// </summary>
        /// <param name="account">An implementation of <see cref="IAccount"/></param>
        /// <param name="cache">An implementation of <see cref="ICache{T,TK}"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public CacheFieldFactory(IAccount account, ICache<IField, Guid> cache, ILog logger) : base(logger)
        {
            this._account = account;
            this._cache = cache;
        }

        /// <summary>
        /// Gets an instance of <see cref="IField"/> with a matching ID from memory if possible.
        /// </summary>
        /// <param name="id">The ID of the <see cref="IField"/> you want to retrieve</param>
        /// <returns>The required <see cref="IField"/> or null if it cannot be found</returns>
        public override IField this[Guid id]
        {
            get
            {
                IField field = base[id];

                if (field == null)
                {
                    field = this.Get(id);
                    this.Save(field);
                }

                return field;
            }
        }

        /// <summary>
        /// The get a specific field.
        /// </summary>
        /// <param name="id">
        /// The id of the <see cref="IField"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IField"/> or null if none found.
        /// </returns>
        private IField Get(Guid id)
        {
            CacheKey<Guid> cacheKey = new AccountCacheKey<Guid>(this._account)
            {
                Area = typeof(IField).FullName,
                Key = id
            };

            return this._cache.StringGet(cacheKey);
        }
    }
}
