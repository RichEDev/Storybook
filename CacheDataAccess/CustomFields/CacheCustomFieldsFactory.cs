namespace CacheDataAccess.CustomFields
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.CustomFields;
    using BusinessLogic.Fields.Type.Base;

    using Common.Logging;

    /// <summary>
    /// An instance of <see cref="CustomFieldRepository"/> for caching..
    /// </summary>
    public class CacheCustomFieldsFactory : CustomFieldRepository
    {
        /// <summary>
        /// The _current user.
        /// </summary>
        private readonly IAccount _account;

        /// <summary>
        /// The _cache.
        /// </summary>
        private readonly ICache<IField, Guid> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheCustomFieldsFactory"/> class.
        /// </summary>
        /// <param name="account">An instance of <see cref="IAccount"/> to use for retrieving and creating instances of <see cref="IField"/></param>
        /// <param name="cache">An instance of <see cref="ICache{T, TK}"/> to use for retrieving and creating instances of <see cref="IField"/> in cache</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public CacheCustomFieldsFactory(IAccount account, ICache<IField, Guid> cache, ILog logger) : base(logger)
        {
            this._account = account;
            this._cache = cache;
        }

        /// <summary>
        /// Gets the <see cref="IField">IField</see> that matches the supplied field name
        /// </summary>
        /// <param name="name">the field name</param>
        /// <returns>the <see cref="IField">IField</see></returns>
        public override IField this[string name] => null;

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
        /// Gets a <see cref="IField"/> with the matching <param name="id"></param>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
