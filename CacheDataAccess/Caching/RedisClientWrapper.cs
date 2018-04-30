namespace CacheDataAccess.Caching
{
    using System;
    using StackExchange.Redis;

    /// <summary>
    /// Creates an instance of <see cref="ConnectionMultiplexer"/> using a singleton pattern (via <see cref="Lazy{T}"/>).
    /// </summary>
    internal static class RedisClientWrapper
    {
        /// <summary>
        /// Lazy implementation to obtain an instance of <see cref="ConnectionMultiplexer"/> as a singleton as there should only ever be one instance of it within an application due to performance and pool limits.
        /// </summary>
        private static Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConnectionString));

        /// <summary>
        /// Gets or sets the connection string to Redis which is used as a singleton.
        /// </summary>
        private static string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="IDatabase"/> which is used as a singleton for all usages.
        /// </summary>
        private static IDatabase Database { get; set; }

        /// <summary>
        /// Creates a pass-through <see cref="IDatabase"/> to enable Redis access.
        /// </summary>
        /// <param name="connectionString">The connection string to the Redis server.</param>
        /// <param name="dbNumber">The database number for the redis database to use on the Redis server</param>
        /// <returns>A pass through instance of <see cref="IDatabase"/> to enable interaction with Redis</returns>
        /// <exception cref="MemberAccessException">The <see cref="T:System.Lazy`1" /> instance is initialized to use the default constructor of the type that is being lazily initialized, and permissions to access the constructor are missing. </exception>
        /// <exception cref="MissingMemberException">The <see cref="T:System.Lazy`1" /> instance is initialized to use the default constructor of the type that is being lazily initialized, and that type does not have a public, parameterless constructor. </exception>
        /// <exception cref="InvalidOperationException">The initialization function tries to access <see cref="P:System.Lazy`1.Value" /> on this instance. </exception>
        public static IDatabase Connection(string connectionString, int dbNumber)
        {
            ConnectionString = connectionString;

            return Database ?? (Database = LazyConnection.Value.GetDatabase(dbNumber));
        }
    }
}
