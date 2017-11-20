using System;

namespace SpendManagementLibrary.DistributedCaching.Implementations
{
    using System.Collections.Generic;
    using Utilities.DistributedCaching;
    using Utilities.DistributedCaching.Interface;
    using Enyim.Caching.Memcached.Results;

    class NoCache : ICacheable
    {
        public bool Add(int accountId, string area, string key, object value)
        {
            return false;
        }

        public bool Set(int accountId, string area, string key, object value)
        {
            return false;
        }

        public bool Add(int accountId, string area, string key, object value, Cache.CacheTimeSpans timeOutMinutes)
        {
            return false;
        }

        public bool Delete(int accountId, string area, string key)
        {
            return false;
        }


        /// <summary>
        /// Deletes a cache entry with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Please use the alternative Delete method. This is only used as a stop-gap whilst cProjectCodes still exists.")]
        public bool Delete(string key)
        {
            return false;    
        }

        public object Get(int accountId, string area, string key)
        {
            return null;
        }

        public bool Update(int accountId, string area, string key, object value)
        {
            return false;
        }

        public bool Contains(int accountId, string area, string key)
        {
            return false;
        }

        public bool Contains(string key)
        {
            return false;
        }

        public List<string> Statistics()
        {
            return new List<string>();
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
            throw new System.NotImplementedException();
        }
    }
}
