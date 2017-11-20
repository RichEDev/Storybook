using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    public class cUserdefinedFieldGroupings : cUserDefinedFieldGroupingsBase
    {
        public Cache Cache = HttpRuntime.Cache;

        public cUserdefinedFieldGroupings(int accountid)
        {
            sConnectionString = cAccounts.getConnectionString(accountid);
            nAccountID = accountid;
            InitialiseData();
        }

        private static ConcurrentDictionary<int, long> lastReadFromDatabaseTicks = new ConcurrentDictionary<int, long>();
        public void InitialiseData()
        {
            if (nAccountID > 0)
            {
                long lastUpdatedAllServers = cUserDefinedFieldsBase.GetLastUpdatedFromCache(nAccountID);
                long lastReadFromDatabaseThisServer = lastReadFromDatabaseTicks.GetOrAdd(nAccountID, 0);
                var forceUpdateFromDatabase = lastUpdatedAllServers > lastReadFromDatabaseThisServer;
                if (forceUpdateFromDatabase)
                {
                    SortedList<int, cUserdefinedFieldGrouping> oldValue;
                    allGroupings.TryRemove(nAccountID, out oldValue);
                }
            }

            lstGroupings = allGroupings.GetOrAdd(nAccountID, GetCollection);
        }

        public void ResetCache(DateTime lastUpdated)
        {
            lstGroupings = null;
            cUserDefinedFieldsBase.SaveLastUpdatedToCache(nAccountID, lastUpdated);
        }

        /// <summary>
        /// Save a single grouping
        /// </summary>
        /// <param name="grouping"></param>
        /// <returns></returns>
        public int SaveGrouping(cUserdefinedFieldGrouping grouping)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();

            DateTime lastUpdated;
            int id = SaveGroupingBase(clsCurrentUser, grouping, out lastUpdated);
            if (id > 0)
            {
                ResetCache(lastUpdated);
            }
            return id;
        }

        /// <summary>
        /// Delete an individual grouping
        /// </summary>
        /// <param name="userDefinedGroupID"></param>
        /// <param name="employeeID"></param>
        public void DeleteGrouping(int userDefinedGroupID, int employeeID)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();

            DateTime lastUpdated;
            DeleteGroupingBase(clsCurrentUser, userDefinedGroupID, employeeID, out lastUpdated);
            ResetCache(lastUpdated);
        }

        /// <summary>
        /// Saves the order of user defined groupings
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public void SaveOrders(Dictionary<int, int> orders, cTable appliesTo)
        {
            DateTime lastUpdated;
            SaveOrdersBase(orders, appliesTo, out lastUpdated);

            ResetCache(lastUpdated);
        }
    }
}
