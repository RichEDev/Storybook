using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Data.SqlClient;
namespace SpendManagementLibrary
{
    public class Caching
    {
        public MemoryCache Cache { get { return MemoryCache.Default; } }

        /// <summary>
        /// Caching with dependency on a single SQL statement with no parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="monitorSQL"></param>
        /// <param name="timeSpan"></param>
        /// <param name="accountID"></param>
        /// <param name="priority"></param>
        /// <param name="databaseType"></param>
        public void Add(string key, object value, string monitorSQL, CacheTimeSpans timeSpan, CacheDatabaseType databaseType, int? accountID = null, CacheItemPriority priority = CacheItemPriority.Default)
        {
            if (!GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                return;
            }
            Dictionary<string, Dictionary<string, object>> lstSQLMonitors =
                new Dictionary<string, Dictionary<string, object>> {{monitorSQL, new Dictionary<string, object>()}};
            this.Add(key, value, lstSQLMonitors, timeSpan, databaseType, accountID, priority);
        }

        /// <summary>
        /// Caching with dependency on a list of SQL statements with no parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lstMonitorSQL"></param>
        /// <param name="timeSpan"></param>
        /// <param name="databaseType"></param>
        /// <param name="accountID"></param>
        /// <param name="priority"></param>
        public void Add(string key, object value, List<string> lstMonitorSQL, CacheTimeSpans timeSpan, CacheDatabaseType databaseType, int? accountID = null, CacheItemPriority priority = CacheItemPriority.Default)
        {
            if (!GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                return;
            }
            Dictionary<string, Dictionary<string, object>> lstConvertedMonitorSQL = new Dictionary<string,Dictionary<string,object>>();
            foreach(string sql in lstMonitorSQL) 
            {
                lstConvertedMonitorSQL.Add(sql, null);
            }
            this.Add(key, value, lstConvertedMonitorSQL, timeSpan, databaseType, accountID, priority);
        }

        /// <summary>
        /// Caching with dependency on a single SQL statement with parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lstMonitorSQL"></param>
        /// <param name="timeSpan"></param>
        /// <param name="accountID"></param>
        /// <param name="priority"></param>
        /// <param name="databaseType"></param>
        public void Add(string key, object value, Dictionary<string, Dictionary<string, object>> lstMonitorSQL, CacheTimeSpans timeSpan, CacheDatabaseType databaseType, int? accountID = null, CacheItemPriority priority = CacheItemPriority.Default)
        {
            if (!GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                return;
            }
            DBConnection dbConnection;

            if (databaseType == CacheDatabaseType.Metabase)
            {
                dbConnection = new DBConnection(GlobalVariables.MetabaseConnectionString);
            }
            else
            {
                if (accountID.HasValue)
                {
                    dbConnection = new DBConnection(cAccounts.getConnectionString(accountID.Value));
                }
                else
                {
                    return;
                }
            }

            #region Policy
            CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes((int)timeSpan), Priority = priority };

            #endregion

            foreach (KeyValuePair<string, Dictionary<string, object>> kvp in lstMonitorSQL)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    dbConnection.sqlexecute.CommandText = kvp.Key;
                    SqlChangeMonitor monitor = new SqlChangeMonitor(dbConnection.CreateSQLCacheDependency(kvp.Value, dbConnection.sqlexecute.CommandText));
                    policy.ChangeMonitors.Add(monitor);
                }
            }

            this.Cache.Add(key, value, policy);
        }

        public void invalidatedCache()
        {

        }

        public enum CacheDatabaseType
        {
            Customer,
            Metabase
        }

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
    }
}