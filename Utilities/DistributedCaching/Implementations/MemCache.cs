
namespace Utilities.DistributedCaching.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;
    using Enyim.Caching.Memcached.Results;
    using Utilities.DistributedCaching.Interface;

    /// <summary>
    /// The memory cache.
    /// </summary>
    public class MemCache : ICacheable 
    {
        /// <summary>
        /// The memory cache.
        /// </summary>
        private static readonly MemoryCache MemoryCache;

        static MemCache()
        {
            MemoryCache = new MemoryCache("SpendManagement");
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Add(int accountId, string area, string key, object value)
        {
            string completeKey = this.FormatKey(accountId, area, key);
            return MemoryCache.Add(completeKey, value, DateTime.Now.AddMinutes(10));
        }

        
        public bool Set(int accountId, string area, string key, object value)
        {
            string completeKey = this.FormatKey(accountId, area, key);
            MemoryCache.Set(completeKey, value, DateTime.Now.AddMinutes(10));
            return true;
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
            string completeKey = this.FormatKey(accountId, area, key);
            return MemoryCache.Add(completeKey, value, DateTime.Now.AddMinutes((int)timeOutMinutes));
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
            string completeKey = this.FormatKey(accountId, area, key);
            return MemoryCache.Remove(completeKey) != null;
        }

        /// <summary>
        /// Deletes a cache entry with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Please use the alternative Delete method. This is only used as a stop-gap whilst cProjectCodes still exists.")]
        public bool Delete(string key)
        {
            return MemoryCache.Remove(key) != null;
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
            string completeKey = this.FormatKey(accountId, area, key);
            return MemoryCache.Get(completeKey);
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
            string completeKey = this.FormatKey(accountId, area, key);
            MemoryCache.Remove(completeKey);
            return this.Add(accountId, area, key, value);
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
            string completeKey = this.FormatKey(accountId, area, key);
            return MemoryCache.Contains(completeKey);
        }

        public bool Contains(string key)
        {
            return false;
        }

        /// <summary>
        /// The Cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> Statistics()
        {
            return new List<string> { MemoryCache.GetCount().ToString() };
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
    }
}
