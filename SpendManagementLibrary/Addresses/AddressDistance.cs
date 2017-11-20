namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using Configuration.Core;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// An object to encapsulate distance information to an address (from an address that is being examined)
    /// </summary>
    public class AddressDistance
    {
        #region Constructors and Destructors

        /// <summary>
        /// The standard constructor that gives all properties default values
        /// </summary>
        public AddressDistance()
        {
            this.DestinationFriendlyName = "";
            this.DestinationIdentifier = 0;
            this.OutboundIdentifier = 0;
            this.Outbound = 0m;
            this.OutboundFastest = null;
            this.OutboundShortest = null;
            this.ReturnIdentifier = 0;
            this.Return = 0m;
            this.ReturnFastest = null;
            this.ReturnShortest = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The friendly name for an address, currently - Line1, City, Postcode
        /// </summary>
        public string DestinationFriendlyName { get; set; }

        /// <summary>
        /// The identifier for the destination address (the origin is known from the calling address)
        /// </summary>
        public int DestinationIdentifier { get; set; }

        /// <summary>
        /// The custom distance specified for the outbound journey
        /// </summary>
        public decimal Outbound { get; set; }

        /// <summary>
        /// The postcode anywhere fastest distance for the outbound journey
        /// </summary>
        public decimal? OutboundFastest { get; set; }

        /// <summary>
        /// The AddressDistance record identifier where the DestinationIdentifier is AddressIDB
        /// </summary>
        public int OutboundIdentifier { get; set; }

        /// <summary>
        /// The postcode anywhere shortest distance for the outbound journey
        /// </summary>
        public decimal? OutboundShortest { get; set; }

        /// <summary>
        /// The custom distance specified for the return journey
        /// </summary>
        public decimal Return { get; set; }

        /// <summary>
        /// The postcode anywhere fastest distance for the return journey
        /// </summary>
        public decimal? ReturnFastest { get; set; }

        /// <summary>
        /// The AddressDistance record identifier where the DestinationIdentifier is AddressIDA
        /// </summary>
        public int ReturnIdentifier { get; set; }

        /// <summary>
        /// The postcode anywhere shortest distance for the return journey
        /// </summary>
        public decimal? ReturnShortest { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Delete an address custom and recommended distances
        /// </summary>
        /// <param name="currentUser">The current user object for account, employee and delegate information</param>
        /// <param name="identifier">The address distance to delete</param>
        /// <param name="removeRecommendedDistances">Remove the recommended distances along with the custom distance</param>
        /// <param name="connection">The connection interface override, usually used for unit testing</param>
        /// <returns>An negative number on error, or positive identifier on success</returns>
        public static int Delete(ICurrentUserBase currentUser, int identifier, bool removeRecommendedDistances = false, IDBConnection connection = null)
        {
            if (identifier <= 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AddressDistanceID", identifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@RemoveRecommendedDistances", removeRecommendedDistances);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeID", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("DeleteAddressDistance");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            AddressDistance distance = GetById(currentUser.AccountID, identifier);
            if (distance != null)
            {
                UpdateCache(currentUser.AccountID, distance.OutboundIdentifier, distance.ReturnIdentifier);
            }

            return returnValue;
        }

        /// <summary>
        /// Get custom and recommended distance information for a journey, but by the Id rather than the two Address Ids.
        /// This is used by the API.
        /// Important: The destination Id is the ID in any AddressDistances returned from this method.
        /// Also, every appears on the outbound properties, since they apply to the supplied address and will be converted.
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="distanceId">The Id of the AddressDistance.</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>An address distance object with the relevant values EXCEPT FriendlyName populated, or null if no distance information exists</returns>
        public static AddressDistance GetById(int accountIdentifier, int distanceId, IDBConnection connection = null)
        {
            if (distanceId <= 0)
            {
                throw new ArgumentOutOfRangeException("distanceId", distanceId, "The value of this argument must be greater than 0.");
            }

            AddressDistance distances = null;

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressDistanceID", distanceId);
                using (IDataReader reader = databaseConnection.GetReader("GetAddressDistanceById", CommandType.StoredProcedure))
                {
                    int addressDistanceIdentifierOrdinal = reader.GetOrdinal("AddressDistanceID"),
                        customDistanceOrdinal = reader.GetOrdinal("CustomDistance"),
                        postcodeAnywhereFastestDistanceOrdinal = reader.GetOrdinal("PostcodeAnywhereFastestDistance"),
                        postcodeAnywhereShortestDistanceOrdinal = reader.GetOrdinal("PostcodeAnywhereShortestDistance"),
                        addressAOrdinal = reader.GetOrdinal("AddressIDA"),
                        addressBOrdinal = reader.GetOrdinal("AddressIDB");

                    while (reader.Read())
                    {
                        int identifier = reader.GetInt32(addressDistanceIdentifierOrdinal);
                        decimal customDistance = reader.GetDecimal(customDistanceOrdinal);
                        decimal? fastestDistance = reader.IsDBNull(postcodeAnywhereFastestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereFastestDistanceOrdinal);
                        decimal? shortestDistance = reader.IsDBNull(postcodeAnywhereShortestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereShortestDistanceOrdinal);
                        int ida = reader.GetInt32(addressAOrdinal);
                        int idb = reader.GetInt32(addressBOrdinal);

                        distances = distances ?? new AddressDistance
                        {
                            DestinationIdentifier = identifier,
                            OutboundIdentifier = ida,
                            Outbound = customDistance,
                            OutboundFastest = fastestDistance,
                            OutboundShortest = shortestDistance,
                            ReturnIdentifier = idb
                        };
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return distances;
        }

        /// <summary>
        /// Convert the given <see cref="AddressDistanceDTO"/> to an <see cref="AddressDistance"/> object
        /// </summary>
        /// <param name="addressDistanceNoSql"><see cref="AddressDistanceDTO"/> that needs to be converted</param>
        /// <returns>An instance of <see cref="AddressDistance"/></returns>
        private static AddressDistance ConvertToAddressDistanceFromDTO(AddressDistanceDTO addressDistanceDTO)
        {
            if (addressDistanceDTO == null || addressDistanceDTO.AtoB.DistanceId.HasValue == false)
            {
                return null;
            }

            AddressDistance addressDistance = new AddressDistance
                                                  {
                                                      DestinationIdentifier = addressDistanceDTO.AtoB.ToId,
                                                      OutboundIdentifier = addressDistanceDTO.AtoB.DistanceId.Value,
                                                      Outbound = addressDistanceDTO.AtoB.CustomDistance ?? 0,
                                                      OutboundFastest = addressDistanceDTO.AtoB.FastestDistance,
                                                      OutboundShortest = addressDistanceDTO.AtoB.ShortestDistance,
                                                  };

            if (addressDistanceDTO.BtoA.DistanceId.HasValue)
            {
                addressDistance.ReturnIdentifier = addressDistanceDTO.AtoB.DistanceId.Value; // Yes this is set to AtoB on purpose to mirror the original method...
                addressDistance.Return = addressDistanceDTO.AtoB.CustomDistance ?? 0;
                addressDistance.ReturnFastest = addressDistanceDTO.AtoB.FastestDistance;
                addressDistance.ReturnShortest = addressDistanceDTO.AtoB.ShortestDistance;
            }

            return addressDistance;
        }

        /// <summary>
        /// Updates the given address distance in cache.
        /// </summary>
        /// <param name="accountId">The account id of the current user</param>
        /// <param name="fromId">The address id of the origin address</param>
        /// <param name="toId">The address id of the destination address</param>
        /// <returns>
        /// An instance of <see cref="AddressDistance"/> from origin to destination address
        /// </returns>
        public static void UpdateCache(int accountId, int fromId, int toId)
        {
            return;
            RedisCache<AddressDistanceDTO, int> redisCache = new RedisCache<AddressDistanceDTO, int>(new LogFactory<AddressDistance>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());
            var cacheKey = new AccountCacheKey<int>(new Account(accountId, null, false)) { Area = typeof(AddressDistance).Name };
            AddressDistanceDTO result = AddressDistanceDTO.Get(accountId, fromId, toId);
            if (result != null)
            {
                redisCache.HashAdd(
                    cacheKey,
                    "list",
                    $"{result.AtoB.FromId}:{result.AtoB.ToId}",
                    result);
            }
        }

        /// <summary>
        /// Gets address distance from cache with the given from and to id
        /// </summary>
        /// <param name="accountId">
        /// The account id of the current user
        /// </param>
        /// <param name="fromId">
        /// The address id of the origin address
        /// </param>
        /// <param name="toId">
        /// The address id of the destination address
        /// </param>
        /// <returns>
        /// An instance of <see cref="AddressDistance"/> from origin to destination address
        /// </returns>
        private static AddressDistance GetFromCache(int accountId, int fromId, int toId)
        {
            return null;
            RedisCache<AddressDistanceDTO, int> redisCache = new RedisCache<AddressDistanceDTO, int>(new LogFactory<AddressDistance>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());
            var cacheKey = new AccountCacheKey<int>(new Account(accountId, null, false)) { Area = typeof(AddressDistance).Name };
            AddressDistanceDTO result = redisCache.HashGet(cacheKey, "list", $"{fromId}:{toId}");

            return result == null ? null : ConvertToAddressDistanceFromDTO(result);
        }

        /// <summary>
        /// Deletes the given value address distance value from cache
        /// </summary>
        /// <param name="accountId">The account id of the current user</param>
        /// <param name="fromId">The address id of the origin address</param>
        /// <param name="toId">The address id of the destination address</param>
        /// <returns>True if it was successfully deleted from cache</returns>
        public static bool DeleteFromCache(int accountId, int fromId, int toId)
        {
            RedisCache<AddressDistanceDTO, int> redisCache = new RedisCache<AddressDistanceDTO, int>(new LogFactory<AddressDistance>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());
            var cacheKey = new AccountCacheKey<int>(new Account(accountId, null, false)) { Area = typeof(AddressDistance).Name };
            return redisCache.HashDelete(cacheKey, "list", $"{fromId}:{toId}");
        }

        /// <summary>
        /// Get custom and recommended distance information for a journey
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="originIdentifier">The origin address</param>
        /// <param name="destinationIdentifier">The destination address</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>An address distance object with the relevant values EXCEPT FriendlyName populated, or null if no distance information exists</returns>
        public static AddressDistance Get(int accountIdentifier, int originIdentifier, int destinationIdentifier, IDBConnection connection = null)
        {
            if (originIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("originIdentifier", originIdentifier, "The value of this argument must be greater than 0.");
            }

            if (destinationIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("destinationIdentifier", destinationIdentifier, "The value of this argument must be greater than 0.");
            }

            AddressDistance distances = GetFromCache(accountIdentifier, originIdentifier, destinationIdentifier);
            if (distances != null)
            {
                return distances;
            }

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@InternationalLookup", new cAccounts().GetAccountByID(accountIdentifier).AddressInternationalLookupsAndCoordinates);
                databaseConnection.AddWithValue("@OriginAddressID", originIdentifier);
                databaseConnection.AddWithValue("@DestinationAddressID", destinationIdentifier);

                using (IDataReader reader = databaseConnection.GetReader("GetAddressToAddressDistance", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        int addressDistanceIdentifierOrdinal = reader.GetOrdinal("AddressDistanceID"),
                            customDistanceOrdinal = reader.GetOrdinal("CustomDistance"),
                            postcodeAnywhereFastestDistanceOrdinal =
                                reader.GetOrdinal("PostcodeAnywhereFastestDistance"),
                            postcodeAnywhereShortestDistanceOrdinal =
                                reader.GetOrdinal("PostcodeAnywhereShortestDistance"),
                            outboundOrdinal = reader.GetOrdinal("Outbound");

                        do
                        {
                            int identifier = reader.GetInt32(addressDistanceIdentifierOrdinal);
                            decimal customDistance = reader.GetDecimal(customDistanceOrdinal);
                            decimal? fastestDistance =
                                reader.IsDBNull(postcodeAnywhereFastestDistanceOrdinal)
                                    ? (decimal?) null
                                    : reader.GetDecimal(postcodeAnywhereFastestDistanceOrdinal);
                            decimal? shortestDistance =
                                reader.IsDBNull(postcodeAnywhereShortestDistanceOrdinal)
                                    ? (decimal?) null
                                    : reader.GetDecimal(postcodeAnywhereShortestDistanceOrdinal);
                            bool outbound = reader.GetBoolean(outboundOrdinal);

                            distances =
                                distances ?? new AddressDistance {DestinationIdentifier = destinationIdentifier};

                            if (outbound)
                            {
                                distances.OutboundIdentifier = identifier;
                                distances.Outbound = customDistance;
                                distances.OutboundFastest = fastestDistance;
                                distances.OutboundShortest = shortestDistance;
                            }
                            else
                            {
                                distances.ReturnIdentifier = identifier;
                                distances.Return = customDistance;
                                distances.ReturnFastest = fastestDistance;
                                distances.ReturnShortest = shortestDistance;
                            }
                        } while (reader.Read());

                        databaseConnection.sqlexecute.Parameters.Clear();
                        reader.Close();
                    }
                }
            }

            if (distances != null)
            {
                UpdateCache(accountIdentifier, originIdentifier, destinationIdentifier);
            }

            return distances;
        }

        /// <summary>
        /// Get only the manually added distance information for a journey
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="originIdentifier">The origin address</param>
        /// <param name="destinationIdentifier">The destination address</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>A distance or null.</returns>
        public static decimal? GetCustom(int accountIdentifier, int originIdentifier, int destinationIdentifier, IDBConnection connection = null)
        {
            decimal? distance = null;

            AddressDistance addressDistance = Get(accountIdentifier, originIdentifier, destinationIdentifier, connection);

            if (addressDistance != null && addressDistance.OutboundIdentifier != 0 && addressDistance.Outbound > 0)
            {
                distance = addressDistance.Outbound;
            }

            return distance;
        }

        /// <summary>
        /// Get all address distances related to an origin address
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="addressIdentifier">The origin address to get all distances outbound from or returning to</param>
        /// <param name="connection">The connection object interface, usually used for unit testing</param>
        /// <returns>A list of all distances relating to the origin address</returns>
        public static List<AddressDistance> GetForAddress(int accountIdentifier, int addressIdentifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than 0.");
            }

            List<AddressDistance> addressDistances = new List<AddressDistance>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);

                using (IDataReader reader = databaseConnection.GetReader("GetRelatedAddressDistances", CommandType.StoredProcedure))
                {
                    int addressDistanceIDOrdinal = reader.GetOrdinal("AddressDistanceID"),
                        addressIdOrdinal = reader.GetOrdinal("AddressID"),
                        customDistanceOrdinal = reader.GetOrdinal("CustomDistance"),
                        postcodeAnywhereFastestDistanceOrdinal = reader.GetOrdinal("PostcodeAnywhereFastestDistance"),
                        postcodeAnywhereShortestDistanceOrdinal = reader.GetOrdinal("PostcodeAnywhereShortestDistance"),
                        outboundOrdinal = reader.GetOrdinal("Outbound"),
                        addressNameOrdinal = reader.GetOrdinal("AddressName"),
                        line1Ordinal = reader.GetOrdinal("Line1"),
                        cityOrdinal = reader.GetOrdinal("City"),
                        postcodeOrdinal = reader.GetOrdinal("Postcode");

                    while (reader.Read())
                    {
                        bool isOutbound = reader.GetBoolean(outboundOrdinal);
                        int destinationIdentifier = reader.GetInt32(addressIdOrdinal);
                        AddressDistance addressDistance = addressDistances.FirstOrDefault(x => x.DestinationIdentifier == destinationIdentifier);
                        string friendlyName = Address.GenerateFriendlyName(reader.IsDBNull(addressNameOrdinal) ? string.Empty : reader.GetString(addressNameOrdinal), reader.IsDBNull(line1Ordinal) ? string.Empty : reader.GetString(line1Ordinal), reader.IsDBNull(postcodeOrdinal) ? string.Empty : reader.GetString(postcodeOrdinal), reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal));

                        if (addressDistance == null)
                        {
                            addressDistance = new AddressDistance
                            {
                                DestinationIdentifier = destinationIdentifier,
                                DestinationFriendlyName = friendlyName
                            };
                            addressDistances.Add(addressDistance);
                        }

                        if (isOutbound)
                        {
                            addressDistance.OutboundIdentifier = reader.GetInt32(addressDistanceIDOrdinal);
                            addressDistance.Outbound = reader.GetDecimal(customDistanceOrdinal);
                            addressDistance.OutboundFastest = reader.IsDBNull(postcodeAnywhereFastestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereFastestDistanceOrdinal);
                            addressDistance.OutboundShortest = reader.IsDBNull(postcodeAnywhereShortestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereShortestDistanceOrdinal);
                        }
                        else
                        {
                            addressDistance.ReturnIdentifier = reader.GetInt32(addressDistanceIDOrdinal);
                            addressDistance.Return = reader.GetDecimal(customDistanceOrdinal);
                            addressDistance.ReturnFastest = reader.IsDBNull(postcodeAnywhereFastestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereFastestDistanceOrdinal);
                            addressDistance.ReturnShortest = reader.IsDBNull(postcodeAnywhereShortestDistanceOrdinal) ? (decimal?)null : reader.GetDecimal(postcodeAnywhereShortestDistanceOrdinal);
                        }
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            addressDistances.RemoveAll(x => x.Outbound <= 0 && x.Return <= 0);

            return addressDistances;
        }

        /// <summary>
        /// Get all address distances related to an origin address - for the API.
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="addressIdentifier">The origin address to get all distances outbound from or returning to</param>
        /// <param name="connection">The connection object interface, usually used for unit testing</param>
        /// <returns>A list of all address distances (these do not match the grid in expenses)</returns>
        public static List<AddressDistance> GetForAddressApi(int accountIdentifier, int addressIdentifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than 0.");
            }

            var addressDistances = new List<AddressDistance>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);

                using (IDataReader reader = databaseConnection.GetReader("GetRelatedAddressDistancesApi", CommandType.StoredProcedure))
                {
                    int addressDistanceIdOrdinal = reader.GetOrdinal("AddressDistanceID"),
                        addressAOrdinal = reader.GetOrdinal("AddressIDA"),
                        addressBOrdinal = reader.GetOrdinal("AddressIDB"),
                        customDistanceOrdinal = reader.GetOrdinal("CustomDistance");

                    while (reader.Read())
                    {
                        var distance = reader.GetDecimal(customDistanceOrdinal);
                        if (distance > 0)
                        {
                            addressDistances.Add(new AddressDistance
                            {
                                DestinationIdentifier = reader.GetInt32(addressDistanceIdOrdinal),
                                Outbound = distance,
                                OutboundIdentifier = reader.GetInt32(addressAOrdinal),
                                ReturnIdentifier = reader.GetInt32(addressBOrdinal)
                            });
                        }
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return addressDistances;
        }

        /// <summary>
        /// Get all address distances related to an origin address - for the API.
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="addressIdentifier">The origin address to get all distances outbound from or returning to</param>
        /// <param name="connection">The connection object interface, usually used for unit testing</param>
        /// <returns>A list of all address distances (these do not match the grid in expenses)</returns>
        public static List<AddressDistance> GetAddressDistancesToDelete(int accountIdentifier, int addressIdentifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than 0.");
            }

            var addressDistances = new List<AddressDistance>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);
                string sql =
                    "Select AddressIDA, AddressIDB from addressDistances where AddressIDA = @AddressID or  AddressIDB = @AddressID";
                using (IDataReader reader = databaseConnection.GetReader(sql))
                {
                    int addressAOrdinal = reader.GetOrdinal("AddressIDA"),
                        addressBOrdinal = reader.GetOrdinal("AddressIDB");

                    while (reader.Read())
                    {
                            addressDistances.Add(new AddressDistance
                            {
                                OutboundIdentifier = reader.GetInt32(addressAOrdinal),
                                ReturnIdentifier = reader.GetInt32(addressBOrdinal)
                            });
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return addressDistances;
        }

        /// <summary>
        /// Get all address distances related to an origin address - for the API where the ids returned are the AddressDistanceId
        /// </summary>
        /// <param name="accountIdentifier">The current account</param>
        /// <param name="addressIdentifier">The origin address to get all distances outbound from or returning to</param>
        /// <param name="connection">The connection object interface, usually used for unit testing</param>
        /// <returns>A list of all address distance Ids relating to the origin address</returns>
        public static List<int> GetIdsForAddressApi(int accountIdentifier, int addressIdentifier, IDBConnection connection = null)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than 0.");
            }

            var addressDistances = new List<int>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@AddressID", addressIdentifier);

                using (IDataReader reader = databaseConnection.GetReader("GetRelatedAddressDistanceIds", CommandType.StoredProcedure))
                {
                    int addressDistanceIdOrdinal = reader.GetOrdinal("AddressDistanceID"),
                        customDistanceOrdinal = reader.GetOrdinal("CustomDistance");

                    while (reader.Read())
                    {
                        var id = reader.GetInt32(addressDistanceIdOrdinal);
                        var distance = reader.GetDecimal(customDistanceOrdinal);
                        if (distance > 0)
                        {
                            addressDistances.Add(id);
                        }
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return addressDistances;
        }

        /// <summary>
        /// Retrieves any current distance from the customer database or performs a PostcodeAnywhere lookup 
        /// </summary>
        /// <param name="originIdentifier">The origin "from" address identifier</param>
        /// <param name="destinationIdentifier">The destination "to" address identifier</param>
        /// <param name="mapsEnabled">Determines which lookup method is used</param>
        /// <param name="mileageCalculationType">Determines which mileage calculation is used</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns></returns>
        public static decimal? GetRecommendedDistance(ICurrentUserBase currentUser, int originIdentifier, int destinationIdentifier, int mileageCalculationType, bool mapsEnabled, IDBConnection connection = null)
        {
            if (originIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("originIdentifier", originIdentifier, "The value of this argument must be greater than 0.");
            }

            if (destinationIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("destinationIdentifier", destinationIdentifier, "The value of this argument must be greater than 0.");
            }

            AddressDistance addressDistance = Get(currentUser.AccountID, originIdentifier, destinationIdentifier);
            decimal? distance = null;
            if (addressDistance != null)
            {
                if (addressDistance.Outbound > 0)
                {
                    distance = addressDistance.Outbound;
                }
                else
                {
                    distance = (mileageCalculationType == 1)
                                   ? addressDistance.OutboundShortest
                                   : addressDistance.OutboundFastest;
                }
            }

            if (distance == null)
            {
                var origin = Address.Get(currentUser.AccountID, originIdentifier);
                var destination = Address.Get(currentUser.AccountID, destinationIdentifier);

                if (origin != null && destination != null)
                {
                    var account = new cAccounts().GetAccountByID(currentUser.AccountID);

                    var originLocator = origin.GetLookupLocator(account.AddressInternationalLookupsAndCoordinates, account.AddressLookupProvider);
                    var destinationLocator = destination.GetLookupLocator(account.AddressInternationalLookupsAndCoordinates, account.AddressLookupProvider);

                    // get from Postcode anywhere
                    if (originLocator.Length > 0 && destinationLocator.Length > 0)
                    {
                        var postcodeAnywhere = new PostcodeAnywhere(currentUser.AccountID);

                        if (mapsEnabled)
                        {
                            distance = postcodeAnywhere.GetDistanceByClosestRoadOnRouteToPostcodeCentre(currentUser, originLocator, destinationLocator, string.Empty, mileageCalculationType == 1 ? DistanceType.Shortest : DistanceType.Fastest);
                        }
                        else
                        {
                            distance = postcodeAnywhere.LookupDistanceByPostcodeCentres(currentUser, originLocator, destinationLocator, DistanceType.DRIVETIME, mileageCalculationType == 1 ? PostcodeAnywhereCost.Distance : PostcodeAnywhereCost.Time);
                        }

                        if (distance.HasValue)
                        {
                            // store the retrieved distance for next time
                            Save(currentUser, originIdentifier, destinationIdentifier, mileageCalculationType, distance.Value);
                        }
                    }
                }
            }

            return distance;
        }


        /// <summary>
        /// Saves an address distance
        /// </summary>
        /// <param name="currentUser">The current user object, used for account, employee and delegate information</param>
        /// <param name="originIdentifier">The origin address</param>
        /// <param name="destinationIdentifier">The destination address</param>
        /// <param name="mileageCalculationType">The distance type that is being used - 1 is shortest distance, anything else is fastest distance</param>
        /// <param name="distance">The distance between the two addresses</param>
        /// <param name="connection">The connection interface object override, usually used in unit testing</param>
        /// <param name="saveCustomDistance">If true, takes the distance argument and places it in the CustomDistance field.</param>
        /// <returns>The positive address distance identifier or a negative return code</returns>
        public static int Save(ICurrentUserBase currentUser, int originIdentifier, int destinationIdentifier, int mileageCalculationType, decimal distance, IDBConnection connection = null, bool saveCustomDistance = false)
        {
            if (originIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("originIdentifier", originIdentifier, "The value of this argument must be greater than (if editing) or equal to 0 (if saving new).");
            }

            if (destinationIdentifier < 0)
            {
                throw new ArgumentOutOfRangeException("destinationIdentifier", destinationIdentifier, "The value of this argument must be greater than (if editing) or equal to 0 (if saving new).");
            }

            int returnValue;

            string lookupDistanceParameterName = (mileageCalculationType == 1) ? "@PostcodeAnywhereShortestDistance" : "@PostcodeAnywhereFastestDistance";

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue("@AddressIDA", originIdentifier);
                databaseConnection.AddWithValue("@AddressIDB", destinationIdentifier);
                
                databaseConnection.AddWithValue("@CreatedBy", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@ModifiedBy", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@EmployeeID", currentUser.EmployeeID);

                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);
                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                if (saveCustomDistance)
                {
                    databaseConnection.AddWithValue("@CustomDistance", distance);
                }
                else
                {
                    databaseConnection.AddWithValue("@CustomDistance", 0m);    
                    databaseConnection.AddWithValue(lookupDistanceParameterName, distance, 18, 2);
                }
                
                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveAddressDistance");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            UpdateCache(currentUser.AccountID, originIdentifier, destinationIdentifier);
            return returnValue;
        }

        /// <summary>
        /// Save distances related to an address
        /// </summary>
        /// <param name="currentUser">The current user object used for employee, delegate and account</param>
        /// <param name="addressIdentifier">The origin address that all distances relate to</param>
        /// <param name="addressDistances">A list of the address destinations with the related distance information</param>
        /// <param name="connection">A connection interface, mostly used for unit testing</param>
        /// <returns>A positive value indicating success or a negative error code</returns>
        public static int SaveForAddress(ICurrentUserBase currentUser, int addressIdentifier, List<AddressDistance> addressDistances, IDBConnection connection = null)
        {
            if (addressIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("addressIdentifier", addressIdentifier, "The value of this argument must be greater than 0.");
            }

            int returnValue = 0;

            // todo: skipping unchanged items

            List<SqlDataRecord> addressDistanceRecords = new List<SqlDataRecord>();
            // Generate a sql table param and pass into the stored proc
            SqlMetaData[] addressSqlTableParam = { new SqlMetaData("c1", SqlDbType.Int), new SqlMetaData("c2", SqlDbType.Int), new SqlMetaData("c3", SqlDbType.Int), new SqlMetaData("c4", SqlDbType.Decimal, 18, 2) };

            foreach (AddressDistance addressDistance in addressDistances)
            {
                if (addressDistance.Outbound > 0 || addressDistance.OutboundIdentifier > 0)
                {
                    SqlDataRecord row = new SqlDataRecord(addressSqlTableParam);
                    row.SetInt32(0, addressDistance.OutboundIdentifier);
                    row.SetInt32(1, addressIdentifier);
                    row.SetInt32(2, addressDistance.DestinationIdentifier);
                    row.SetDecimal(3, addressDistance.Outbound);
                    addressDistanceRecords.Add(row);
                }

                if (addressDistance.Return > 0 || addressDistance.ReturnIdentifier > 0)
                {
                    SqlDataRecord row = new SqlDataRecord(addressSqlTableParam);
                    row.SetInt32(0, addressDistance.ReturnIdentifier);
                    row.SetInt32(1, addressDistance.DestinationIdentifier);
                    row.SetInt32(2, addressIdentifier);
                    row.SetDecimal(3, addressDistance.Return);
                    addressDistanceRecords.Add(row);
                }
            }

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                if (addressDistanceRecords.Count > 0)
                {
                    databaseConnection.sqlexecute.Parameters.Add("@distances", SqlDbType.Structured);
                    databaseConnection.sqlexecute.Parameters["@distances"].Direction = ParameterDirection.Input;
                    databaseConnection.sqlexecute.Parameters["@distances"].Value = addressDistanceRecords;
                }

                databaseConnection.AddWithValue("@EmployeeID", currentUser.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = currentUser.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveAddressDistances");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            foreach (var record in addressDistanceRecords)
            {
                UpdateCache(currentUser.AccountID, record.GetInt32(1), record.GetInt32(2));
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddressId">The start address</param>
        /// <param name="toAddressId">The endd address</param>
        /// <param name="accountid"></param>
        /// <param name="subAccount"></param>
        /// <param name="currentUser"></param>
        /// <returns>The distance between the two addresses</returns>
        public static decimal? GetRecommendedOrCustomDistance(int fromAddressId, int toAddressId, int accountid, cAccountSubAccount subAccount, ICurrentUserBase currentUser)
        {
            return AddressDistance.GetRecommendedOrCustomDistance(fromAddressId, toAddressId, subAccount.SubAccountProperties.UseMapPoint, subAccount.SubAccountProperties.MileageCalcType, currentUser);
        }

        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddressId">The start address</param>
        /// <param name="toAddressId">The endd address</param>
        /// <param name="accountid"></param>
        /// <param name="subAccount"></param>
        /// <param name="currentUser"></param>
        /// <returns>The distance between the two addresses</returns>
        public static decimal? GetRecommendedOrCustomDistance(int fromAddressId, int toAddressId, bool useMapPoint, int mileageCalculationType, ICurrentUserBase currentUser)
        {
            if (fromAddressId == 0 || toAddressId == 0)
            {
                return null;
            }

            if (fromAddressId == toAddressId)
            {
                return 0;
            }

            decimal? distance = GetCustom(currentUser.AccountID, fromAddressId, toAddressId);

            if (useMapPoint && (distance.HasValue && distance.Value > 0) == false)
            {
                cAccount account = new cAccounts().GetAccountByID(currentUser.AccountID);
                distance = GetRecommendedDistance(currentUser, fromAddressId, toAddressId,
                                                                  mileageCalculationType,
                                                                  account.MapsEnabled);
            }
            return distance;
        }

        #endregion
    }
}