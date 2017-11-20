namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Data;

    using Common.Logging;

    using Newtonsoft.Json;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Address Distance Data Transfer Object used to store <see cref="AddressDistance"/> in cache  
    /// IF I AM EVER CHANGED, PLEASE RERUN REDIS CACHE PRELOADER
    /// </summary>
    [Serializable]
    public partial class AddressDistanceDTO
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<AddressDistanceDTO>().GetLogger();

        /// <summary>
        /// Created an instance of Address distance data transfer object
        /// </summary>
        public AddressDistanceDTO()
        {
            this.AtoB = new AddressPart();
            this.BtoA = new AddressPart();
        }

        /// <summary>
        /// Gets or sets the distance values from origin to destination address
        /// </summary>
        [JsonProperty("a:to:b")]
        public AddressPart AtoB { get; set; }

        /// <summary>
        /// Gets or sets the distance values from destination to origin address
        /// </summary>
        [JsonProperty("b:to:a")]
        public AddressPart BtoA { get; set; }

        /// <summary>
        /// Gets the address distance values between the given from and to id  
        /// </summary>
        /// <param name="accountId">The account id of current user</param>
        /// <param name="fromId">The address id of the origin address</param>
        /// <param name="toId">The address id of the destination address</param>
        /// <param name="connection">An instance of <see cref="DBConnection"/></param>
        /// <returns>An instance of <see cref="AddressDistanceDTO"/></returns>
        public static AddressDistanceDTO Get(int accountId, int fromId, int toId, IDBConnection connection = null)
        {

            Log.Debug($"Get address distance for addresses between fromId - {fromId} and toId - {toId}");

            AddressDistanceDTO addressDistanceDTO = null;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.AddWithValue("@fromId", fromId);
                databaseConnection.AddWithValue("@toId", toId);

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
                             LEFT JOIN addressDistances BtoA ON BtoA.AddressIDA = AtoB.AddressIDB AND BtoA.AddressIDB = AtoB.AddressIDA
                             WHERE 
                             AtoB.AddressIDA = @fromId AND  AtoB.AddressIDB=@toId";

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

                        if (Log.IsInfoEnabled)
                        {
                            Log.Info($"Instantiate class for address part {fromId} to {toId} and address part {toId} to {fromId}");
                        }

                        addressDistanceDTO = new AddressDistanceDTO
                                    {
                                        AtoB =
                                            {
                                                FromId = fromId,
                                                ToId = toId,
                                                CustomDistance = customAtoB,
                                                DistanceId = distanceIdAtoB,
                                                FastestDistance = fastestAtoB,
                                                ShortestDistance = shortestAtoB
                                            },
                                        BtoA =
                                            {
                                                FromId = toId,
                                                ToId = fromId,
                                                CustomDistance = customBtoA,
                                                DistanceId = distanceIdBtoA,
                                                FastestDistance = fastestBtoA,
                                                ShortestDistance = shortestBtoA
                                            }
                                    };
                    }
                }
            }

            Log.Debug($"Succesfully added address distances between fromId - {fromId} and toId - {toId}");

            return addressDistanceDTO;
        }
    }
}