namespace CacheDataAccess.UserDefinedFields
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using Common.Logging;

    public class CacheUserDefinedFieldFactory : UserDefinedFieldRepository
    {
        private readonly IAccount _account;

        private readonly ICache<IField, Guid> _cache;
        public const string CacheArea = "UserDefinedField";

        /// <summary>
        /// Create an implementation of <see cref="CacheUserDefinedFieldFactory"/>
        /// </summary>
        /// <param name="account">An implementation of <see cref="IAccount"/></param>
        /// <param name="cache">An implementation of <see cref="ICache{T, TK}"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public CacheUserDefinedFieldFactory(IAccount account, ICache<IField, Guid> cache, ILog logger) : base(logger)
        {
            this._account = account;
            this._cache = cache;
        }

        /// <summary>
        /// Gets an instance of <see cref="IField"/> with a matching name from memory if possible.
        /// </summary>
        /// <param name="name">The name of the <see cref="IField"/> you want to retrieve</param>
        /// <returns>The required <see cref="IField"/> or null if it cannot be found</returns>
        public override IField this[string name] => null;

        /// <summary>
        /// Get a <see cref="List{IField}"/> for the given <seealso cref="ITable"/>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public override List<IField> this[ITable table] => new List<IField>();

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
