// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cache.cs" company="Software (Europe) Ltd">
//   Copyright (c) Software (Europe) Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Utilities.DistributedCaching
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.DistributedCaching.Implementations;

    using Utilities.DistributedCaching.Implementations;
    using Utilities.DistributedCaching.Interface;

    using System.Diagnostics;
    using Enyim.Caching.Memcached.Results;

    /// <summary>
    ///     The distributed cache.
    /// </summary>
    public class Cache : ICacheable
    {
        /// <summary>
        /// The cache time spans.
        /// </summary>
        public enum CacheTimeSpans
        {
            /// <summary>
            /// 2 Minutes
            /// </summary>
            UltraShort = 2,

            /// <summary>
            /// 7 Minutes
            /// </summary>
            VeryShort = 7,

            /// <summary>
            /// 15 Minutes 
            /// </summary>
            Short = 15,

            /// <summary>
            /// 30 Minutes
            /// </summary>
            Medium = 30,

            /// <summary>
            /// 60 Minutes
            /// </summary>
            Permanent = 60
        }

        #region Static Fields
        /// <summary>
        /// The cache name.
        /// </summary>
        private static readonly string CacheName;

        #endregion

        #region Fields

        /// <summary>
        /// Gets the app fabric.
        /// </summary>
        private static ICacheable DistributedCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises static members of the <see cref="Cache"/> class.
        /// </summary>
        static Cache()
        {
            CreateNewConnection();
        }

        private static void  CreateNewConnection()
        {
            try
            {
                DistributedCache = new MemCache();
                
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("0", e.Message);
                DistributedCache = new NoCache();
            }
        }

        public Cache()
        {
            if (DistributedCache.GetType() == typeof(NoCache))
            {
                CreateNewConnection();
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
            return DistributedCache.Add(accountId, area, key, value);
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
        /// The time out of the cache object
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Add(int accountId, string area, string key, object value, CacheTimeSpans timeOutMinutes)
        {
            return DistributedCache.Add(accountId, area, key, value, timeOutMinutes);
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
            return DistributedCache.Set(accountId, area, key, value);
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
            return DistributedCache.Delete(accountId, area, key);
        }

        public bool Delete(string key)
        {
            return DistributedCache.Delete(key);
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
            return DistributedCache.Get(accountId, area, key);
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
            return DistributedCache.Update(accountId, area, key, value);
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
            return DistributedCache.Contains(accountId, area, key);
        }

        public bool Contains(string key)
        {
            return DistributedCache.Contains(key);
        }

        /// <summary>
        /// The Cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> Statistics()
        {
            return DistributedCache.Statistics();
        }

        public List<string> GetView(string viewName)
        {
            return DistributedCache.GetView(viewName);
        }

        public bool CreateView(string name, object document)
        {
            return DistributedCache.CreateView(name, document);
        }

        public IMutateOperationResult IncrementCounter(int accountId, string area, string key)
        {
            return DistributedCache.IncrementCounter(accountId, area, key);
        }

        #endregion
    }
}