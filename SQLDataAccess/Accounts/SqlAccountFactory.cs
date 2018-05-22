namespace SQLDataAccess.Accounts
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    using Utilities.Cryptography;

    /// <summary>
    /// The sql account factory composition.
    /// </summary>
    public class SqlAccountFactory : IDataFactory<IAccount, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/>
        /// </summary>1
        private readonly IMetabaseCacheFactory<IAccount, int> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="ICryptography"/> for decrypting.
        /// </summary>
        private readonly ICryptography _cryptography;

        /// <summary>
        /// An instance of <see cref="_databaseServerFactory"/> to use when creating new instances of <see cref="IAccount"/>
        /// </summary>
        private readonly IDataFactory<IDatabaseServer, int> _databaseServerFactory;

        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">
        /// The cache factory to use for dealing with <see cref="IAccount"/> objects.
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </param>
        /// <param name="databaseServerFactory">
        /// <see cref="IDataFactory{T,K}"/> to use when creating new instances of <see cref="IAccount"/>
        /// </param>
        /// <param name="cryptography">
        /// An instance of <see cref="ICryptography"/>to use when decoding passwords.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cacheFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metabaseDataConnection"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="databaseServerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cryptography"/> is <see langword="null"/>.
        /// </exception>
        public SqlAccountFactory(IMetabaseCacheFactory<IAccount, int> cacheFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection, IDataFactory<IDatabaseServer, int> databaseServerFactory, ICryptography cryptography)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(metabaseDataConnection, nameof(metabaseDataConnection));
            Guard.ThrowIfNull(databaseServerFactory, nameof(databaseServerFactory));
            Guard.ThrowIfNull(cryptography, nameof(cryptography));
            
            this._cacheFactory = cacheFactory;
            this._metabaseDataConnection = metabaseDataConnection;
            this._databaseServerFactory = databaseServerFactory;
            this._cryptography = cryptography;
        }

        /// <summary>
        /// Gets an instance of <see cref="IAccount"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally SQL
        /// </summary>
        /// <param name="id">The ID of the <see cref="IAccount"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAccount"/> or null if it cannot be found</returns>
        public IAccount this[int id]
        {
            get
            {
                IAccount account = this._cacheFactory[id];

                if (account == null)
                {
                    account = this.Get(id);

                    if (account != null)
                    {
                        this._cacheFactory.Save(account);
                    }
                }

                return account;
            }
        }

        /// <summary>
        /// Adds an <see cref="IAccount"/> to cache.
        /// </summary>
        /// <param name="entity">
        /// The entity to add to cache.
        /// </param>
        /// <returns>
        /// The <see cref="IAccount"/>.
        /// </returns>
        public IAccount Save(IAccount entity)
        {
            if (entity == null)
            {
                return null;
            }

            return this._cacheFactory.Save(entity);
        }

        /// <summary>
        /// Gets an instance of <see cref="IAccount"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IAccount"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAccount"/></returns>
        private IAccount Get(int id)
        {
            const string Sql = "SELECT accountid, dbname, dbusername, dbpassword, dbserver, archived FROM registeredusers WHERE accountid=@accountID";

            this._metabaseDataConnection.Parameters.Add(new SqlParameter("@accountID", SqlDbType.Int) { Value = id });

            IAccount account = null;

            using (IDataReader reader = this._metabaseDataConnection.GetReader(Sql))
            {
                while (reader.Read())
                {
                    int accountId = reader.GetInt32(0);
                    string databaseCatalogueName = reader.GetString(1);
                    string username = reader.GetString(2);
                    string password = reader.GetString(3);
                    int databaseServerId = reader.GetInt32(4);
                    bool archived = reader.GetBoolean(5);

                    IDatabaseServer databaseServer = this._databaseServerFactory[databaseServerId];

                    // Tightly coupled due to the catalogue technically being part of an Account but isolated for seperation of concerns
                    IDatabaseCatalogue databaseCatalogue = new DatabaseCatalogue(databaseServer, databaseCatalogueName, username, password, this._cryptography);

                    account = new Account(accountId, databaseCatalogue, archived);
                }

                this._metabaseDataConnection.Parameters.Clear();
            }

            return account;
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IList<IAccount> Get()
        {
            List<IAccount> accounts = new List<IAccount>();

            const string Sql = "SELECT accountid, dbname, dbusername, dbpassword, dbserver, archived, isNHSCustomer FROM registeredusers";

            using (IDataReader reader = this._metabaseDataConnection.GetReader(Sql))
            {
                while (reader.Read())
                {
                    int accountId = reader.GetInt32(0);
                    string databaseCatalogueName = reader.GetString(1);
                    string username = reader.GetString(2);
                    string password = reader.GetString(3);
                    int databaseServerId = reader.GetInt32(4);
                    bool archived = reader.GetBoolean(5);
                    bool nhsCustomer = reader.GetBoolean(6);

                    IDatabaseServer databaseServer = this._databaseServerFactory[databaseServerId];

                    // Tightly coupled due to the catalogue technically being part of an Account but isolated for seperation of concerns
                    IDatabaseCatalogue databaseCatalogue = new DatabaseCatalogue(databaseServer, databaseCatalogueName, username, password, this._cryptography);

                    if (nhsCustomer)
                    {
                        accounts.Add(new NhsAccount(accountId, databaseCatalogue, archived));
                    }
                    else
                    {
                        accounts.Add(new Account(accountId, databaseCatalogue, archived));
                    }
                }

                this._metabaseDataConnection.Parameters.Clear();
            }

            return accounts;
        }

        /// <inheritdoc />
        public IList<IAccount> Get(Predicate<IAccount> predicate)
        {
            IList<IAccount> accounts = this.Get();

            if (predicate == null)
            {
                return accounts;
            }

            List<IAccount> matchedAccounts = new List<IAccount>();

            foreach (IAccount account in accounts)
            {
                if (predicate.Invoke(account))
                {
                    matchedAccounts.Add(account);
                }
            }

            return matchedAccounts;
        }
    }
}
