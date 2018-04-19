namespace SQLDataAccess.GeneralOptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SQLDataAccess.AccountProperties;
    using SQLDataAccess.Builder;

    public class SqlGeneralOptionsFactory : IDataFactory<IGeneralOptions, int>
    {
        /// <summary>
        /// A backing instance of the <see cref="SqlAccountPropertiesFactory"/>
        /// </summary>
        private readonly SqlAccountPropertiesFactory _sqlAccountPropertiesFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlGeneralOptionsFactory"/> class. 
        /// </summary>
        /// <param name="sqlAccountPropertiesFactory">An instance of <see cref="SqlAccountPropertiesFactory"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="sqlAccountPropertiesFactory"/> is <see langword="null" />.</exception>
        public SqlGeneralOptionsFactory(SqlAccountPropertiesFactory sqlAccountPropertiesFactory)
        {
            Guard.ThrowIfNull(sqlAccountPropertiesFactory, nameof(sqlAccountPropertiesFactory));

            this._sqlAccountPropertiesFactory = sqlAccountPropertiesFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="IGeneralOptions"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The subAccountId of the <see cref="IGeneralOptions"/> you want to retrieve</param>
        /// <returns>The required <see cref="IGeneralOptions"/> or <see langword="null" /> if it cannot be found</returns>
        public IGeneralOptions this[int id]
        {
            get
            {
                return new GeneralOptionsBuilder(this._sqlAccountPropertiesFactory.Get().Where(bob => bob.SubAccountId == id).ToList());
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IGeneralOptions"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="entity">The <see cref="IGeneralOptions"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IGeneralOptions"/>.</returns>
        public IGeneralOptions Save(IGeneralOptions entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the instance of <see cref="IGeneralOptions"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IGeneralOptions"/> to delete.</param>
        /// <returns>An <see cref="string"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all available <see cref="IGeneralOptions"/>
        /// </summary>
        /// <returns>The list of <see cref="IGeneralOptions"/></returns>
        public IList<IGeneralOptions> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IGeneralOptions"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IGeneralOptions"/> that match <paramref name="predicate"/>.</returns>
        public IList<IGeneralOptions> Get(Predicate<IGeneralOptions> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
