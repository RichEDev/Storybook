namespace CacheDataAccess.CustomFields
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.CustomEntities;
    using BusinessLogic.Fields.Type.Base;

    using Common.Logging;

    /// <summary>
    /// Extends <see cref="CustomEntityFieldRepository"/> and implements methods to retrieve and create instances of <see cref="IField"/> in cache
    /// </summary>
    public class CacheCustomEntityFieldsFactory : CustomEntityFieldRepository
    {
        /// <summary>
        /// The current user
        /// </summary>
        private readonly IAccount _account;

        /// <summary>
        /// An instance of cache to use for retrieving and creating cache objects
        /// </summary>
        private readonly ICache<IField, Guid> _cache;

        /// <summary>
        /// Initializes a new instance of the<see cref="CacheCustomEntityFieldsFactory">CacheCustomEntityFieldsFactory</see> class
        /// </summary>
        /// <param name="account">
        /// The current <see cref="IAccount"/> for the logged in user.
        /// </param>
        /// <param name="cache">
        /// The cache mechanism
        /// </param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public CacheCustomEntityFieldsFactory(IAccount account, ICache<IField, Guid> cache, ILog logger) : base(logger)
        {
            this._account = account;
            this._cache = cache;
        }

        /// <summary>
        /// Gets a Field by its Name
        /// </summary>
        /// <param name="name">
        /// The Field Name
        /// </param>
        /// <returns>
        /// The <see cref="IField">IField</see>
        /// </returns>
        public override IField this[string name] => null;

        /// <summary>
        /// Gets an instance of <see cref="IField"/> with a matching ID from memory if possible, if not it will search cache for an entry, then add to memory
        /// </summary>
        /// <param name="id">
        /// The Field Id to lookup
        /// </param>
        /// <returns>
        /// The <see cref="IField">IField</see>
        /// </returns>
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
        /// Gets an instance of <see cref="IField"/>
        /// </summary>
        /// <param name="id">
        /// The Field Id to lookup
        /// </param>
        /// <returns>
        /// The <see cref="IField">IField</see>
        /// </returns>
        private IField Get(Guid id)
        {
            CacheKey<Guid> cacheKey = new AccountCacheKey<Guid>(this._account);
            cacheKey.Area = typeof(IField).FullName;
            return this._cache.StringGet(cacheKey);
        }
    }
}
