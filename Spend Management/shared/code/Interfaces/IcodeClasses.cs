namespace Spend_Management.shared.code.Interfaces
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Web;
    using System.Web.Caching;

    using SpendManagementLibrary;

    /// <summary>
    /// The code Classes interface.
    /// </summary>
    /// <typeparam name="T">
    /// The class that is being manipulated
    /// </typeparam>
    public abstract class CodeClasses<T>
    {
        /// <summary>
        /// The db connection.
        /// </summary>
        internal DBConnection DbConnection { get; set; }

        /// <summary>
        /// The account id.
        /// </summary>
        internal readonly int AccountId;

        /// <summary>
        /// The cache.
        /// </summary>
        internal Cache Cache;

        /// <summary>
        /// Initialises a new instance of the <see cref="CodeClasses{T}"/> class.
        /// </summary>
        /// <param name="accountId">
        ///     The account id.
        /// </param>
        /// <param name="databaseConnection">
        ///     The database connection.
        /// </param>
        protected CodeClasses(int accountId, DBConnection databaseConnection)
        {
            this.AccountId = accountId;
            this.DbConnection = databaseConnection ?? new DBConnection(cAccounts.getConnectionString(this.AccountId));
            this.Cache = HttpRuntime.Cache;
        }

        /// <summary>
        ///  The initialise data.
        /// </summary>
        public abstract void InitialiseData();

        /// <summary>
        ///  The cache list.
        ///  </summary><returns>
        ///  The <see><cref>SortedList</cref></see>
        ///      .
        /// </returns>
        public abstract SortedList<int, T> CacheList();

        /// <summary>
        ///  The save.
        /// </summary><param name="entity">
        ///  The entity.
        ///  </param><returns>
        ///  The <see cref="T:System.Int32" />.
        /// </returns>
        public abstract int Save(T entity);

        /// <summary>
        ///  The read.
        ///  </summary><param name="id">
        ///  The id.
        ///  </param><returns>
        ///  The <see cref="!:T" />.
        /// </returns>
        public abstract T GetById(int id);

        /// <summary>
        ///  The get by string.
        ///  </summary><param name="searchString">
        ///  The search string.
        ///  </param><returns>
        ///  The <see cref="!:T" />.
        /// </returns>
        public abstract T GetByString(string searchString);

        /// <summary>
        ///  The count of entities in the cache.
        ///  </summary><returns>
        ///  The <see cref="T:System.Int32" />.
        /// </returns>
        public abstract int Count();

        /// <summary>
        ///  The delete.
        ///  </summary><param name="id">
        ///  The id.
        ///  </param><returns>
        ///  The <see cref="T:System.Int32" />.
        /// </returns>
        public abstract int Delete(int id);

        /// <summary>
        ///  The delete.
        ///  </summary><param name="entity">
        ///  The entity.
        ///  </param><returns>
        ///  The <see cref="T:System.Int32" />.
        /// </returns>
        public abstract int Delete(T entity);
    }
}
