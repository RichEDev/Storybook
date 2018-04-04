namespace SQLDataAccess.Accounts
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    using SQLDataAccess.Elements;

    public class SqlAccountWithElementsFactory : IGetBy<IAccountWithElement, int>, IGetAll<IAccountWithElement>
    {
        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IAccountWithElement"/> instances.
        /// </summary>
        private readonly MetabaseCacheFactory<IAccountWithElement, int> _cacheFactory;

        /// <summary>
        /// Backing instance of accounts factory.
        /// </summary>
        private readonly SqlAccountFactory _sqlAccountFactory;

        /// <summary>
        /// Backing instance of accounts factory.
        /// </summary>
        private readonly SqlElementFactory _sqlElementFactory;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IAccountWithElement"/> instances.
        /// </param>
        /// <param name="sqlAccountFactory">
        /// An instance of <see cref="SqlAccountFactory"/> to decorate with licenced elements
        /// </param>
        /// <param name="sqlElementFactory">
        /// An instance of <see cref="SqlElementFactory"/> to get the <see cref="IElement"/> to decorate the <see cref="IAccount"/>
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metabaseDataConnection"/> is <see langword="null"/>.
        /// </exception>
        public SqlAccountWithElementsFactory(MetabaseCacheFactory<IAccountWithElement, int> cacheFactory, SqlAccountFactory sqlAccountFactory, SqlElementFactory sqlElementFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            Guard.ThrowIfNull(metabaseDataConnection, nameof(metabaseDataConnection));
            Guard.ThrowIfNull(sqlAccountFactory, nameof(sqlAccountFactory));
            Guard.ThrowIfNull(sqlElementFactory, nameof(sqlElementFactory));
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));

            this._cacheFactory = cacheFactory;
            this._sqlAccountFactory = sqlAccountFactory;
            this._sqlElementFactory = sqlElementFactory;
            this._metabaseDataConnection = metabaseDataConnection;
        }

        /// <summary>
        /// Gets an instance of <see cref="IAccountWithElement"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IAccountWithElement"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAccountWithElement"/> or <see langword="null" /> if it cannot be found</returns>
        public IAccountWithElement this[int id] => this.Get(id);

        /// <summary>
        /// Gets a list of all available <see cref="IAccountWithElement"/>
        /// </summary>
        /// <returns>The list of <see cref="IAccountWithElement"/></returns>
        public IList<IAccountWithElement> Get()
        {
            IList<IAccountWithElement> accountWithElement = this._cacheFactory.Get();

            if (accountWithElement == null)
            {
                IList<IAccount> accounts = this._sqlAccountFactory.Get();

                if (accounts != null && accounts.Count >= 1)
                {
                    accountWithElement = this.Convert(accounts);
                    this._cacheFactory.Add(accountWithElement);
                }
            }

            return accountWithElement;
        }

        /// <summary>
        /// Gets a <see cref="IAccountWithElement"/> by the Id of the <see cref="Account"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="Account"/> you want to retrieve</param>
        /// <returns>A <see cref="IAccountWithElement"/></returns>
        public IAccountWithElement Get(int id)
        {
            IAccountWithElement accountWithElement = this._cacheFactory[id];

            if (accountWithElement == null)
            {
                IAccount account = this._sqlAccountFactory[id];

                if (account != null)
                {
                    accountWithElement = this.Convert(new List<IAccount> { account })[0];
                    this._cacheFactory.Save(accountWithElement);
                }
            }

            return accountWithElement;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IAccountWithElement"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IAccountWithElement"/> that match <paramref name="predicate"/>.</returns>
        public IList<IAccountWithElement> Get(Predicate<IAccountWithElement> predicate)
        {
            if (predicate == null)
            {
                return null;
            }

            IList<IAccountWithElement> accountsWithElements = this.Get();

            if (accountsWithElements == null)
            {
                return null;
            }

            List<IAccountWithElement> matchedAccountWithElementss = new List<IAccountWithElement>();

            foreach (IAccountWithElement accountWithElements in accountsWithElements)
            {
                if (predicate.Invoke(accountWithElements))
                {
                    matchedAccountWithElementss.Add(accountWithElements);
                }
            }

            return matchedAccountWithElementss;
        }

        /// <summary>
        /// Converts instances of <see cref="IAccount"/> to <see cref="IAccountWithElement"/>.
        /// </summary>
        /// <param name="accounts">The collection of <see cref="IAccount"/> to convert.</param>
        /// <returns>A collection of <see cref="IAccountWithElement"/> converted from <paramref name="accounts"/>.</returns>
        private IList<IAccountWithElement> Convert(IList<IAccount> accounts)
        {
            IList<IAccountWithElement> convertAccountWithElements = new List<IAccountWithElement>();

            string sql =
                @"SELECT accountID, elementID FROM dbo.accountsLicencedElements";

            if (accounts.Count == 1)
            {
                sql += " WHERE accountID = @AccountId";
                this._metabaseDataConnection.Parameters.Add(new SqlParameter("@AccountId", SqlDbType.Int) { Value = accounts[0].Id });
            }

            Dictionary<int, IList<IElement>> elements = new Dictionary<int, IList<IElement>>();
            IElement element = null;
            var theElements = this._sqlElementFactory.Get();

            using (DbDataReader reader = this._metabaseDataConnection.GetReader(sql))
            {
                while (reader.Read())
                {
                    var accountIdOrdinal = reader.GetOrdinal("accountID");
                    var elementIdOrd = reader.GetOrdinal("elementID");

                    if (!reader.IsDBNull(accountIdOrdinal))
                    {
                        var accountId = reader.GetInt32(accountIdOrdinal);
                        var elementId = reader.GetInt32(elementIdOrd);

                        if (!elements.ContainsKey(accountId))
                        {
                            elements.Add(accountId, new List<IElement>());
                        }

                        element = theElements.Find(x => x.Id == elementId);

                        if (element != null)
                        {
                            elements[accountId].Add(element);
                        }
                    }
                }
            }

            this._metabaseDataConnection.Parameters.Clear();

            foreach (IAccount account in accounts)
            {
                elements.TryGetValue(account.Id, out IList<IElement> elementsList);
                AccountWithElements accountWithElements = new AccountWithElements(account.Id, account.DatabaseCatalogue, account.Archived, elementsList);
                convertAccountWithElements.Add(accountWithElements);
            }

            return convertAccountWithElements;
        }
    }
}
