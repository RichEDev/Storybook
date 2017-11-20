using Utilities.DistributedCaching;

namespace Auto_Tests.Tools
{
    using System;
    using System.Configuration;

    /// <summary>
    /// The cache utilities.
    /// </summary>
    public static class CacheUtilities
    {

        /// <summary>
        /// Initialises static members of the <see cref="CacheUtilities"/> class.
        /// </summary>
        static CacheUtilities()
        {
            AccountId = int.Parse(ConfigurationManager.AppSettings["accountID"]);
        }

        /// <summary>
        /// Gets the account id.
        /// </summary>
        public static int AccountId { get; private set; }

        /// <summary>
        /// The delete cached tables and fields.
        /// </summary>
        /// <param name="lastUpdateDate">
        /// The last Update Date.
        /// </param>
        public static void DeleteCachedTablesAndFields(DateTime lastUpdateDate)
        {
            var cache = new Cache();
            cache.Set(AccountId, string.Empty, "userdefinedfieldslatest", lastUpdateDate.Ticks);
            cache.Delete(AccountId, string.Empty, "userdefinedfieldslatest");
        }

        /// <summary>
        /// The delete cached employee.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        public static void DeleteCachedEmployee(int accountId, string employeeId)
        {
            var cache = new Cache();
            cache.Delete(accountId, "employee", employeeId);
        }

        /// <summary>
        /// The delete cached employee access roles.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        public static void DeleteCachedEmployeeAccessRoles(int accountId, string employeeId)
        {
            var cache = new Cache();
            cache.Delete(accountId, "employeeAccessRoles", employeeId);
        }

        /// <summary>
        /// Saves the latest update to the cache so other servers will know to refresh their in-memory lists from the database.
        /// Called when e.g. a user defined field is saved
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="lastUpdated">The ticks value of the date time at which the update occurred.</param>
        public static void SaveLastUpdatedToCache(int accountId, DateTime lastUpdated)
        {
            // Cache.Set(accountId, string.Empty, LatestUpdateKey, lastUpdated.Ticks);
        }
        
    }
}
