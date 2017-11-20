// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Memcached.cs" company="Software (Europe) Ltd">
//   Copyright (c) Software (Europe) Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SpendManagementLibrary.DistributedCaching.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Enyim.Caching;
    using Enyim.Caching.Memcached;
    using Utilities.DistributedCaching;
    using Utilities.DistributedCaching.Interface;
    using Enyim.Caching.Memcached.Results;

    /// <summary>
    /// The MEMCACHED implementation class.
    /// </summary>
    public class Memcached : ICacheable
    {
        #region Fields

        /// <summary>
        ///     The cache.
        /// </summary>
        private MemcachedClient cache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Memcached"/> class. 
        /// </summary>
        internal Memcached()
        {
            if (this.cache == null)
            {
                this.cache = new MemcachedClient();
            }
        }

        #endregion

        #region Public Methods and Operators

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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Add(int accountId, string area, string key, object value)
        {
            return AddOrSet(accountId, area, key, value, StoreMode.Add);
        }

        /// <summary>
        /// The set.
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Set(int accountId, string area, string key, object value)
        {
            return AddOrSet(accountId, area, key, value, StoreMode.Set);
        }

        /// <summary>
        /// Add or set.
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AddOrSet(int accountId, string area, string key, object value, StoreMode storeMode)
        {
            var result = this.cache.Store(storeMode, this.FormatKey(accountId, area, key), value);
            if (!result)
            {
                var found = this.Get(accountId, area, key);
                if (found == null)
                {
                    Debug.WriteLine("Memcached Add {0} {1} = {2}", area, key, result);
                }
            }

            return result;
        }

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
        /// <param name="timeOutMinutes">
        /// The time out minutes.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Add(int accountId, string area, string key, object value, Cache.CacheTimeSpans timeOutMinutes)
        {
            var result = this.cache.Store(StoreMode.Add, this.FormatKey(accountId, area, key), value, new TimeSpan(0, 0, (int)timeOutMinutes, 0));
            if (!result)
            {
                var found = this.Get(accountId, area, key);
                if (found == null)
                {
                    Debug.WriteLine("Memcached Add {0} {1} = {2}", area, key, result);
                }
            }

            return result;
        }

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
        public bool Contains(int accountId, string area, string key)
        {
            object result;
            var containsResult = this.cache.TryGet(this.FormatKey(accountId, area, key), out result);
            return containsResult;
        }

        public bool Contains(string key)
        {
            return false;
        }

        /// <summary>
        /// The delete.
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
        public bool Delete(int accountId, string area, string key)
        {
            var result = this.cache.Remove(this.FormatKey(accountId, area, key));
            return result;
        }

        /// <summary>
        /// Deletes a cache entry with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Please use the alternative Delete method. This is only used as a stop-gap whilst cProjectCodes still exists.")]
        public bool Delete(string key)
        {
            var result = this.cache.Remove(key);
            return result;
        }

        /// <summary>
        /// The get.
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
        /// The <see cref="object"/>.
        /// </returns>
        public object Get(int accountId, string area, string key)
        {
            var result = this.cache.Get(this.FormatKey(accountId, area, key));
            return result;
        }

        /// <summary>
        /// The update.
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Update(int accountId, string area, string key, object value)
        {
            var result = this.cache.Store(StoreMode.Replace, this.FormatKey(accountId, area, key), value);
            if (!result)
            {
                Debug.WriteLine("Memcached Update = {0} {1} = {2}", area, key, result);
            }

            return result;
        }

        /// <summary>
        /// The cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="ServerStats"/>.
        /// </returns>
        public List<string> Statistics()
        {
            var serverStats = this.cache.Stats();
            
            var items = new[]
                            {
                                "curr_connections", "curr_items",
                                "total_connections", "connection_structures", "cmd_get", "cmd_flush", "get_hits",
                                "get_misses", "delete_misses", "delete_hits", "incr_misses", "incr_hits",
                                "decr_misses", "decr_hits", "cas_misses", "cas_hits", "cas_badval", "auth_cmds",
                                "auth_errors", "bytes_read", "bytes_written", "limit_maxbytes", "accepting_conns",
                                "listen_disabled_num", "threads", "conn_yields", "bytes",  "total_items",
                                "evictions"
                            };

            return (from item in items let stat = serverStats.GetRaw(item) select string.Format("{0} = {1}", item, GetTotal(stat))).ToList();
        }

        public List<string> GetView(string viewName)
        {
            return null;
        }

        public bool CreateView(string name, object document)
        {
            return false;
        }

        public IMutateOperationResult IncrementCounter(int accountId, string area, string key)
        {
            throw new NotImplementedException();
        }

        private static long GetTotal(IEnumerable<KeyValuePair<IPEndPoint, string>> stat)
        {
            long total = 0;
            foreach (KeyValuePair<IPEndPoint, string> valuePair in stat)
            {
                if (valuePair.Value != null)
                {
                    total = total + long.Parse(valuePair.Value);
                }
            }

            return total;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The format key.
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
        /// The <see cref="string"/>.
        /// </returns>
        private string FormatKey(int accountId, string area, string key)
        {
            return string.Format("{0}_{1}_{2}", accountId, area, key);
        }

        #endregion
    }
}