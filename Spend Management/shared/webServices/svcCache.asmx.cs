

namespace Spend_Management.shared.webServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using SpendManagementLibrary;
    using Utilities.DistributedCaching.Interface;
    using System.Diagnostics;
    /// <summary>
    /// Summary description for svcCache
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcCache : WebService
    {
        private Cache _cache = (Cache)HttpRuntime.Cache;

        private MemoryCache _memoryCache = MemoryCache.Default;

        private ICacheable distributedCache = new Utilities.DistributedCaching.Cache();

        /// <summary>
        /// The number of cache entries.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public long NumberOfCacheEntries()
        {
            return _cache.Count + _memoryCache.GetCount();
        }

        [WebMethod]
        public List<string> GetLimits()
        {
            List<string> limits = new List<string>
                                      {
                                          "System.Web.Cache - Effective Private Bytes Limit: " + this._cache.EffectivePrivateBytesLimit.ToString(CultureInfo.InvariantCulture), 
                                          "System.Web.Cache - Effective Percentage Physical Memory Limit: " + this._cache.EffectivePercentagePhysicalMemoryLimit.ToString(CultureInfo.InvariantCulture), 
                                          "System.Runtime.Caching - CacheMemoryLimit: " + this._memoryCache.CacheMemoryLimit.ToString(CultureInfo.InvariantCulture), 
                                          "System.Runtime.Caching - PhysicalMemoryLimit: " + this._memoryCache.PhysicalMemoryLimit.ToString(CultureInfo.InvariantCulture), 
                                          "System.Runtime.Caching - PollingInterval: " + this._memoryCache.PollingInterval.ToString("c")
                                      };

            return limits;
        }

        [WebMethod]
        public string AllKeysCSV()
        {
            List<CacheKeySplit> splitKeys = this.GetKeySplits();

            StringBuilder csv = new StringBuilder();

            foreach (CacheKeySplit cacheKeySplit in splitKeys)
            {
                csv.AppendLine(string.Format("{0},{1},{2}", cacheKeySplit.AlphaKey, cacheKeySplit.AccountID, cacheKeySplit.RecordID));
            }

            return csv.ToString();
        }

        [WebMethod]
        public List<CacheKeySplit> AllKeys()
        {
            return this.GetKeySplits();
        }

        public class CacheKeySplit
        {
            public string AlphaKey { get; set; }
            public int AccountID { get; set; }
            public int RecordID { get; set; }

            private CacheKeySplit()
            {
                
            }

            public CacheKeySplit(string alpha, int accountID, int recordID)
            {
                this.AlphaKey = alpha;
                this.AccountID = accountID;
                this.RecordID = recordID;
            }
        }

        private List<CacheKeySplit> GetKeySplits()
        {
            List<CacheKeySplit> cacheKeysSplit = new List<CacheKeySplit>();

            foreach (DictionaryEntry cacheEntry in this._cache)
            {
                if (cacheEntry.Key.ToString().StartsWith("System.Web.Services.Protocols.HttpServerProtocol") == false)
                {
                    cacheKeysSplit.Add(this.SplitCacheKey(cacheEntry.Key.ToString()));
                }
            }

            foreach (KeyValuePair<string, object> cacheEntry in _memoryCache)
            {
                cacheKeysSplit.Add(this.SplitCacheKey(cacheEntry.Key));
            }

            return cacheKeysSplit;
        }

        private CacheKeySplit SplitCacheKey(string cacheKey)
        {
            string alpha = Regex.Replace(cacheKey, @"[\d]", string.Empty).TrimEnd('_').TrimEnd('-');
            string[] keys = cacheKey.Replace(alpha.TrimStart('_').TrimStart('-'), string.Empty).Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

            int accountID = 0;

            if (keys.Length > 0)
            {
                int.TryParse(keys[0], out accountID);
            }

            int recordID = 0;

            if (keys.Length == 2)
            {
                int.TryParse(keys[1], out recordID);
            }

            return new CacheKeySplit(alpha, accountID, recordID);
        }

        /// <summary>
        /// The cache keys.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<string> CacheKeys()
        {
            return (from DictionaryEntry entry in this._cache select entry.Key.ToString()).ToList();
        }

        /// <summary>
        /// The cache items.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<CacheEntry> CacheItems(string cacheKey)
        {
            var result = new List<CacheEntry>();
            var jss = new JavaScriptSerializer();            

            if (string.IsNullOrEmpty(cacheKey))
            {
                foreach (DictionaryEntry entry in this._cache)
                {
                    string value;

                    try
                    {
                        value = jss.Serialize(entry.Value);
                    }
                    catch (Exception)
                    {
                        value = entry.Value.ToString();
                    }

                    var cacheItem = new CacheEntry() { ValueType = entry.Value.ToString(), Key = entry.Key.ToString(), Value = new List<string> { value } };
                    result.Add(cacheItem);
                }    
            }
            else
            {
                var valueList = new List<string>();

                foreach (DictionaryEntry entry in this._cache)
                {
                    if (entry.Key.ToString().Contains(cacheKey))
                    {
                        try
                        {
                            var expected = entry.Value;

                            valueList = SerializeObject(expected);
                        }
                        catch (Exception)
                        {
                            valueList = new List<string> { entry.Value == null ? "The cache entry has been nullified" : entry.Value.ToString() };
                        }

                        if (valueList.Count == 0)
                        {
                            valueList = new List<string> { entry.Value == null ? "The cache entry has been nullified" : entry.Value.ToString() };
                        }

                        var cacheItem = new CacheEntry()
                            {
                                ValueType = entry.Value == null ? "The cache entry has been nullified" : entry.Value.GetType().Name,
                                Key = entry.Value == null ? "The cache entry has been nullified" : entry.Key.ToString(),
                                Value = valueList
                            };

                        result.Add(cacheItem);
                    }
                }
            }
            

            return result;
        }

        private List<string> SerializeObject(object expected)
        {
            var valueList = new List<string>();
            if (expected.GetType().IsValueType)
            {
                valueList.Add(expected.ToString());
            }

            if (expected is IEnumerable)
            {
                var collection = expected as ICollection;

                if (collection != null)
                {
                    if (expected.GetType().IsArray && expected.GetType().GetArrayRank() > 1)
                    {
                        if (expected.GetType().GetArrayRank() == 2)
                        {
                            for (var i = 0; i < ((Array)expected).GetLength(0); i++)
                            {
                                for (var j = 0; j < ((Array)expected).GetLength(1); j++)
                                {
                                    valueList.Add(((Array)expected).GetValue(i, j).ToString());
                                }
                            }
                        }

                        if (expected.GetType().GetArrayRank() == 3)
                        {
                            for (var i = 0; i < ((Array)expected).GetLength(0); i++)
                            {
                                for (var j = 0; j < ((Array)expected).GetLength(1); j++)
                                {
                                    for (var k = 0; k < ((Array)expected).GetLength(2); k++)
                                    {
                                        valueList.Add(((Array)expected).GetValue(i, j, k).ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var expectedDictionaryEntries = expected as IDictionary;
                        if (expectedDictionaryEntries != null)
                        {
                            foreach (DictionaryEntry dictionaryEntry in expectedDictionaryEntries)
                            {
                                valueList.Add(dictionaryEntry.Key.ToString());
                                valueList.AddRange(this.SerializeObject(dictionaryEntry.Value));
                            }
                        }

                        var expectedItems = expected as IList;
                        if (expectedItems != null)
                        {
                            foreach (object expectedItem in expectedItems)
                            {
                                valueList.AddRange(this.SerializeObject(expectedItem));
                            }
                        }
                    }
                }
            }
            else
            {
                Type t = expected.GetType();
                PropertyInfo[] propertiesInformation = t.GetProperties();

                valueList.AddRange(
                    from pi in propertiesInformation
                    where pi.GetValue(expected, null) != null
                    select string.Format("{0} - {1}", pi.Name, pi.GetValue(expected, null)));
            }

            return valueList;
        }

        /// <summary>
        /// The distributed cache items.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<string> DistributedCacheKeys()
        {
            var user = cMisc.GetCurrentUser();
            List<string> viewRows = this.distributedCache.GetView("AllKeys");

            if (viewRows.Count == 0)
            {
                this.CreateView("AllKeys");
                viewRows = this.distributedCache.GetView("AllKeys");
            }

            return viewRows.Where(key => key.StartsWith(user.AccountID.ToString() + "_")).Where(key => this.distributedCache.Contains(key)).ToList();
        }

        /// <summary>
        /// Creates a view on the Couchbase server
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        private void CreateView(string viewName)
        {
            var doc = new
            {
                views = new
                {          
                    AllKeys = new
                    {
                        map = "function (doc, meta) { emit(meta.id, null); }"
                    }
                }
            };

            try
            {
               this.distributedCache.CreateView(viewName, doc);
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("Error creating view: {0} in Couchbase. {1}", viewName, ex.Message);      
                EventLog.WriteEntry("Application", errMsg);                        
            }

        }



        /// <summary>
        /// The clear distributed cache item.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<CacheEntry> ShowDistributedCacheItem(string cacheKey)
        {
            var splits = cacheKey.Split('_');
            var entry = distributedCache.Get(int.Parse(splits[0]), splits[1], splits[2]);
            var valueList = new List<string>();
            var result = new List<CacheEntry>();
            try
            {
                valueList = this.SerializeObject(entry);
            }
            catch (Exception)
            {
                valueList = new List<string> { entry == null ? "The cache entry has been nullified" : entry.ToString() };
            }

            if (valueList.Count == 0)
            {
                valueList = new List<string> { entry == null ? "The cache entry has been nullified" : entry.ToString() };
            }

            var cacheItem = new CacheEntry()
            {
                ValueType = entry == null ? "The cache entry has been nullified" : entry.GetType().Name,
                Key = cacheKey,
                Value = valueList
            };

            result.Add(cacheItem);

            return result;
        }

        /// <summary>
        /// The clear distributed cache item.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public bool ClearDistributedCacheItem(string cacheKey)
        {
            var splits = cacheKey.Split('_');
            if (splits.GetUpperBound(0) > 2)
            {
                splits[1] = splits[1] + "_";
                splits[2] = splits[3];
            }
            return distributedCache.Delete(int.Parse(splits[0]), splits[1], splits[2]);
        }
        /// <summary>
        /// The method to delete all keys of an account
        /// </summary>
        /// <returns>flag indicating whether or not all keys from an account were removed</returns>
        [WebMethod(EnableSession = true)]
        public bool ClearDistributedCacheItems()
        {
            bool successFlag = true;
            foreach (var key in this.DistributedCacheKeys())
            {
                if (!this.ClearDistributedCacheItem(key))
                {
                    successFlag = false;
                } 
            }
            return successFlag;
        }
        /// <summary>
        /// The distributed cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<string> DistributedCacheStatistics()
        {
            return this.distributedCache.Statistics();
        }

        /// <summary>
        /// Gets the list of currently cached account id
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<int> AccountCacheKeys()
        {
            return cMisc.GetCurrentUser().Employee.AdminOverride ? cAccounts.CachedAccounts.Keys.ToList() : new List<int>();
        }

        /// <summary>
        /// Reset the cache manually
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool ResetAccountCache()
        {
            HostManager.ResetHostInformation();
            return cMisc.GetCurrentUser().Employee.AdminOverride && new cAccounts().CacheList();
        }
    }

    /// <summary>
    /// The cache entry.
    /// </summary>
    public class CacheEntry
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value type.
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public List<string> Value { get; set; }
    }
}
