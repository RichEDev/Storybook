namespace Utilities.DistributedCaching.Interface
{
    using System;
    using System.Collections.Generic;

    using Enyim.Caching.Memcached.Results;
    using SpendManagementLibrary.DistributedCaching;

    /// <summary>
    ///     The Cache interface.
    /// </summary>
    public interface ICacheable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Add(int accountId, string area, string key, object value);

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="area">The area</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="bool"/></returns>
        bool Set(int accountId, string area, string key, object value);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="timeOut">
        /// The time out.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Add(int accountId, string area, string key, object value, Cache.CacheTimeSpans timeOutMinutes);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Delete(int accountId, string area, string key);

        /// <summary>
        /// Deletes a cache entry with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Please use the alternative Delete method. This is only used as a stop-gap whilst cProjectCodes still exists.")]
        bool Delete(string key);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Get(int accountId, string area, string key);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Update(int accountId, string area, string key, object value);

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Contains(int accountId, string area, string key);
        
        bool Contains(string key);

        /// <summary>
        /// The cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<string> Statistics();

        /// <summary>
        /// get view from couchbase.
        /// </summary>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<string> GetView(string viewName);

        bool CreateView(string name, object document);

        IMutateOperationResult IncrementCounter(int accountId, string area, string key);

        #endregion
    }
}