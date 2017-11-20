namespace Utilities.DistributedCaching.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;

    using Couchbase;
    using Couchbase.Management;
    using Couchbase.Operations;

    using Enyim.Caching.Memcached;
    using Enyim.Caching.Memcached.Results;

    using Newtonsoft.Json;

    using Utilities.DistributedCaching.Interface;

    public class CouchBase : ICacheable
    {
        private readonly CouchbaseClient _cache;

        public CouchBase()
        {
            this._cache = new CouchbaseClient();
        }

        #region Public Methods and Operators

        /// <summary>
        /// Add an item in cache.
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
            return this.AddOrSet(accountId, area, key, value, StoreMode.Set);
        }

        /// <summary>
        /// Add/Update an item in cache with a timeout set..
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
            IStoreOperationResult result = this._cache.ExecuteStore(
                StoreMode.Add, this.FormatKey(accountId, area, key), value, new TimeSpan(0, 0, (int)timeOutMinutes, 0), PersistTo.Zero);
            if (!result.Success)
            {
                var found = this.Get(accountId, area, key);
                if (found == null)
                {
                    EventLog.WriteEntry("0", string.Format("Add {0} {1} = {2}", area, key, result.Message));
                }
            }

            return result.Success;
        }

        /// <summary>
        /// Does the key exist in the cache..
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
            return this.Contains(this.FormatKey(accountId, area, key));
        }

        public bool Contains(string key)
        {
            try
            {
                return this._cache.KeyExists(key);
            }
            catch (NullReferenceException) // Looks like there a "feature" in Couchbase that sometimes throws a NullReferenceException
            {
                return false;
            }
        }

        public bool CreateView(string name, object document)
        {
            var couchbaseCluster = new CouchbaseCluster("couchbase");
            var st = this._cache.Stats();
            IEnumerable<KeyValuePair<IPEndPoint, string>> buckets = st.GetRaw("ep_couch_bucket");
            var bucket = buckets.First().Value;
            return couchbaseCluster.CreateDesignDocument(bucket, name, JsonConvert.SerializeObject(document));
        }

        /// <summary>
        /// Delete an item from the cache..
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
            IRemoveOperationResult result = this._cache.ExecuteRemove(this.FormatKey(accountId, area, key), PersistTo.Zero);
            return result.Success;
        }

        /// <summary>
        /// Deletes a cache entry with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Please use the alternative Delete method. This is only used as a stop-gap whilst cProjectCodes still exists.")]
        public bool Delete(string key)
        {
            IRemoveOperationResult result = this._cache.ExecuteRemove(key, PersistTo.Zero);
            return result.Success;
        }

        /// <summary>
        /// Get an item from the cache.
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
            object result;
            try
            {
                result = this._cache.Get(this.FormatKey(accountId, area, key), DateTime.MaxValue);
            }
            catch (Exception)
            {
                result = null;
                this._cache.ExecuteRemove(this.FormatKey(accountId, area, key), PersistTo.Zero);
            }
            return result;
        }

        /// <summary>
        /// The get view.
        /// </summary>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> GetView(string viewName)
        {
            var view = this._cache.GetView(viewName, viewName);
            var viewRows = new List<string>();

            if (!view.CheckExists()) return viewRows;

            viewRows.AddRange(view.Select(row => row.ItemId));

            return viewRows;
        }

        public IMutateOperationResult IncrementCounter(int accountId, string area, string key)
        {
            return this._cache.ExecuteIncrement(FormatKey(accountId, area, key), (ulong)DateTime.Today.Ticks / 10000, 1);
        }

        /// <summary>
        /// Set an item in cache.
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
        /// The cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="ServerStats"/>.
        /// </returns>
        public List<string> Statistics()
        {
            var serverStats = this._cache.Stats();

            var servers = serverStats.GetRaw("server");
            var result = new List<string> { "Servers: " };
            result.AddRange(servers.Select(server => server.ToString()).ToList());

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

            result.AddRange((from item in items let stat = serverStats.GetRaw(item) select string.Format("{0} = {1}", item, GetTotal(stat))).ToList());

            return result;
        }

        /// <summary>
        /// Update an already existing item in the cache.
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
            IStoreOperationResult result = this._cache.ExecuteStore(StoreMode.Replace, this.FormatKey(accountId, area, key), value, PersistTo.Zero);
            if (!result.Success)
            {
                EventLog.WriteEntry("0", string.Format("Add {0} {1} = {2}", area, key, result.Message));
            }
            return result.Success;
        }

        private bool AddOrSet(int accountId, string area, string key, object value, StoreMode storeMode)
        {
            IStoreOperationResult result = this._cache.ExecuteStore(storeMode, this.FormatKey(accountId, area, key), value,
                                                                   PersistTo.Zero);
            if (!result.Success)
            {
                var found = this.Get(accountId, area, key);
                if (found == null)
                {
                    EventLog.WriteEntry( "0", string.Format("Add {0} {1} = {2}", area, key, result.Message));
                }
            }

            return true;
        }
        #endregion

        #region Methods
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

