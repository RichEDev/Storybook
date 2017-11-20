namespace SQLDataAccess.Databases
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    public class SqlDatabaseServerFactory : IDataFactory<IDatabaseServer, int>
    {
        private readonly MetabaseCacheFactory<IDatabaseServer, int> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseServerFactory"/> class.
        /// </summary>
        /// <param name="cacheFactory">
        /// The cache Factory.
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </param>
        public SqlDatabaseServerFactory(MetabaseCacheFactory<IDatabaseServer, int> cacheFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            this._metabaseDataConnection = metabaseDataConnection;
            this._cacheFactory = cacheFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="IDatabaseServer"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally SQL
        /// </summary>
        /// <param name="id">The ID of the <see cref="IDatabaseServer"/> you want to retrieve</param>
        /// <returns>The required <see cref="IDatabaseServer"/> or null if it cannot be found</returns>
        public IDatabaseServer this[int id]
        {
            get
            {
                IDatabaseServer databaseServer = this._cacheFactory[id];

                if (databaseServer == null)
                {
                    databaseServer = this.Get(id);
                    this._cacheFactory.Add(databaseServer);
                }

                return databaseServer;
            }
        }

        /// <summary>
        /// Adds an instance of <see cref="IDatabaseServer"/> to the collection
        /// </summary>
        /// <param name="entity">
        /// The <see cref="IDatabaseServer"/> you want to add
        /// </param>
        /// <returns>
        /// The <see cref="IDatabaseServer"/> inserted into the collection.
        /// </returns>
        public IDatabaseServer Add(IDatabaseServer entity)
        {
            return this._cacheFactory.Add(entity);
        }

        /// <summary>
        /// Retrieves an instance of <see cref="IDatabaseServer"/> with the matching <param name="id"></param>
        /// </summary>
        /// <param name="id">The Id of the <see cref="IDatabaseServer"/> to retrieve</param>
        /// <returns>The instance of <see cref="IDatabaseServer"/> or null if no matching instance found.</returns>
        private IDatabaseServer Get(int id)
        {
            const string Sql = "SELECT databaseID, hostname FROM databases WHERE databaseID=@databaseID";
            this._metabaseDataConnection.Parameters.Add(new SqlParameter("@databaseID", SqlDbType.Int) { Value = id });

            using (IDataReader reader = this._metabaseDataConnection.GetReader(Sql))
            {
                while (reader != null && reader.Read())
                {
                    int databaseId = reader.GetInt32(0);
                    string hostname = reader.GetString(1);

                    return new DatabaseServer(databaseId, hostname);
                }

                return null;
            }
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<IDatabaseServer> Get()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IDatabaseServer> Get(Predicate<IDatabaseServer> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
