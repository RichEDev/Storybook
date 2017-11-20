namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using Configuration.Core;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Address Distance Data Transfer Object used to store <see cref="AddressDistance"/> in cache
    /// </summary>
    public class AddressDistances
    {

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<AddressDistances>().GetLogger();

        /// <summary>
        /// Gets all the address distance values in the given account
        /// </summary>
        /// <param name="accountId">The account id of current user</param>
        /// <returns>A list of <see cref="AddressDistanceDTO"/></returns>
        private static IEnumerable<AddressDistanceDTO> GetDistances(int accountId)
        {
            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                string sql = @" SELECT 
                             AtoB.AddressDistanceID [distanceIDAtoB],
                             AtoB.AddressIDA [fromA], 
                             AtoB.AddressIDB [toB], 
                             AtoB.CustomDistance [customAtoB], 
                             AtoB.PostcodeAnywhereFastestDistance [fastestAtoB], 
                             AtoB.PostcodeAnywhereShortestDistance [shortestAtoB],
                             BtoA.AddressDistanceID [distanceIDBtoA],
                             BtoA.CustomDistance [customBtoA], 
                             BtoA.PostcodeAnywhereFastestDistance [fastestBtoA], 
                             BtoA.PostcodeAnywhereShortestDistance [shortestBtoA]
                             FROM addressDistances AtoB
                             LEFT JOIN addressDistances BtoA ON BtoA.AddressIDA = AtoB.AddressIDB AND BtoA.AddressIDB = AtoB.AddressIDA";

                using (IDataReader reader = databaseConnection.GetReader(sql))
                {
                    int addressIdFromOrdinal = reader.GetOrdinal("fromA");
                    int addressIdToOrdinal = reader.GetOrdinal("toB");
                    int distanceIdAtoBOrdinal = reader.GetOrdinal("distanceIDatoB");
                    int distanceIdBtoAOrdinal = reader.GetOrdinal("distanceIDBtoA");
                    int customAtoBOrdinal = reader.GetOrdinal("customAtoB");
                    int fastestAtoBOrdinal = reader.GetOrdinal("fastestAtoB");
                    int shortestAtoBOrdinal = reader.GetOrdinal("shortestAtoB");
                    int customBtoAOrdinal = reader.GetOrdinal("customBtoA");
                    int fastestBtoAOrdinal = reader.GetOrdinal("fastestBtoA");
                    int shortestBtoAOrdinal = reader.GetOrdinal("shortestBtoA");

                    while (reader.Read())
                    {
                        int fromA = reader.GetInt32(addressIdFromOrdinal);
                        int toB = reader.GetInt32(addressIdToOrdinal);
                        int distanceIdAtoB = reader.GetInt32(distanceIdAtoBOrdinal);
                        int? distanceIdBtoA = reader.IsDBNull(distanceIdBtoAOrdinal) ? (int?)null : reader.GetInt32(distanceIdBtoAOrdinal);
                        decimal customAtoB = reader.GetDecimal(customAtoBOrdinal);
                        decimal? fastestAtoB = reader.IsDBNull(fastestAtoBOrdinal) ? (decimal?)null : reader.GetDecimal(fastestAtoBOrdinal);
                        decimal? shortestAtoB = reader.IsDBNull(shortestAtoBOrdinal) ? (decimal?)null : reader.GetDecimal(shortestAtoBOrdinal);
                        decimal? customBtoA = reader.IsDBNull(customBtoAOrdinal) ? (decimal?)null : reader.GetDecimal(customBtoAOrdinal);
                        decimal? fastestBtoA = reader.IsDBNull(fastestBtoAOrdinal) ? (decimal?)null : reader.GetDecimal(fastestBtoAOrdinal);
                        decimal? shortestBtoA = reader.IsDBNull(shortestBtoAOrdinal) ? (decimal?)null : reader.GetDecimal(shortestBtoAOrdinal);

                        var noSql = new AddressDistanceDTO
                                        {
                                            AtoB =
                                                {
                                                    FromId = fromA,
                                                    ToId = toB,
                                                    CustomDistance = customAtoB,
                                                    DistanceId = distanceIdAtoB,
                                                    FastestDistance = fastestAtoB,
                                                    ShortestDistance = shortestAtoB
                                                },
                                            BtoA =
                                                {
                                                    FromId = toB,
                                                    ToId = fromA,
                                                    CustomDistance = customBtoA,
                                                    DistanceId = distanceIdBtoA,
                                                    FastestDistance = fastestBtoA,
                                                    ShortestDistance = shortestBtoA
                                                }
                                        };

                        yield return noSql;
                    }
                }
            }
        }

        /// <summary>
        /// Adds all address distance lookups to cache
        /// </summary>
        /// <param name="accountId">
        /// The account id of each and every customer
        /// </param>
        public static void LoadAll(int accountId)
        {
            Log.Debug($"Preload address distances for {accountId}");

            RedisCache<AddressDistanceDTO, int> redisCache = new RedisCache<AddressDistanceDTO, int>(new LogFactory<AddressDistance>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());
            var cacheKey = new AccountCacheKey<int>(new Account(accountId, null, false)) { Area = typeof(AddressDistance).Name };

            int i = 1;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var list = new Dictionary<string, object>();
            foreach (AddressDistanceDTO addressDistance in GetDistances(accountId))
            {
                if (!list.ContainsKey($"{addressDistance.AtoB.FromId}:{addressDistance.AtoB.ToId}"))
                {
                    list.Add($"{addressDistance.AtoB.FromId}:{addressDistance.AtoB.ToId}", addressDistance);
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"Adding distance - {addressDistance.AtoB.FromId}-{addressDistance.AtoB.ToId}");
                    }
                }

                i++;
            }

            redisCache.HashAdd(cacheKey, "list", list);
            stopwatch.Stop();

            Log.Debug($"{i} address distances preloaded for account {accountId} in {stopwatch.Elapsed}");
        }

    }
}
