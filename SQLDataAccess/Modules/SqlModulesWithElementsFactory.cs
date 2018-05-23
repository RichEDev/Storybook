namespace SQLDataAccess.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Elements;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;
    using BusinessLogic.ProductModules.Elements;

    using CacheDataAccess.Caching;

    using SQLDataAccess.Elements;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IProductModuleWithElements"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class SqlModulesWithElementsFactory : IDataFactory<IProductModuleWithElements, Modules>
    {
        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IProductModuleWithElements"/> instances.
        /// </summary>
        private readonly MetabaseCacheFactory<IProductModuleWithElements, int> _cacheFactory;

        /// <summary>
        /// Backing instance of accounts factory.
        /// </summary>
        private readonly SqlModulesFactory _sqlModulesFactory;

        /// <summary>
        /// Backing instance of elements factory.
        /// </summary>
        private readonly SqlElementFactory _sqlElementFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlModulesWithElementsFactory"/> class.
        /// </summary>
        /// <param name="cacheFactory">
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IProductModuleWithElements"/> instances.
        /// </param>
        /// <param name="sqlModulesFactory">
        /// An instance of <see cref="SqlModulesFactory"/> to decorate with licenced elements
        /// </param>
        /// <param name="sqlElementFactory">
        /// An instance of <see cref="SqlElementFactory"/> to get the <see cref="IElement"/> to decorate the <see cref="IProductModule"/>
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metabaseDataConnection"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sqlModulesFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sqlElementFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cacheFactory"/> is <see langword="null"/>.
        /// </exception>
        public SqlModulesWithElementsFactory(MetabaseCacheFactory<IProductModuleWithElements, int> cacheFactory, SqlModulesFactory sqlModulesFactory, SqlElementFactory sqlElementFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            Guard.ThrowIfNull(metabaseDataConnection, nameof(metabaseDataConnection));
            Guard.ThrowIfNull(sqlModulesFactory, nameof(sqlModulesFactory));
            Guard.ThrowIfNull(sqlElementFactory, nameof(sqlElementFactory));
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));

            this._cacheFactory = cacheFactory;
            this._sqlModulesFactory = sqlModulesFactory;
            this._sqlElementFactory = sqlElementFactory;
            this._metabaseDataConnection = metabaseDataConnection;
        }

        /// <summary>
        /// Gets an instance of <see cref="IProductModuleWithElements"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IProductModuleWithElements"/> you want to retrieve</param>
        /// <returns>The required <see cref="IProductModuleWithElements"/> or <see langword="null" /> if it cannot be found</returns>
        public IProductModuleWithElements this[Modules id]
        {
            get
            {
                IProductModuleWithElements accountWithElement = this._cacheFactory[(int)id];

                if (accountWithElement == null)
                {
                    IProductModule account = this._sqlModulesFactory[id];

                    if (account != null)
                    {
                        accountWithElement = this.Convert(new List<IProductModule> { account })[0];
                        this._cacheFactory.Save(accountWithElement);
                    }
                }

                return accountWithElement;
            }
        }


        /// <summary>
        /// Adds or updates the specified instance of <see cref="IProductModuleWithElements"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="entity">The <see cref="IProductModuleWithElements"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IProductModuleWithElements"/>.</returns>
        public IProductModuleWithElements Save(IProductModuleWithElements entity)
        {
            return this._cacheFactory.Save(entity);
        }

        /// <summary>
        /// Deletes the instance of <see cref="IProductModuleWithElements"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProductModuleWithElements"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(Modules id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all available <see cref="IProductModuleWithElements"/>
        /// </summary>
        /// <returns>The list of <see cref="IProductModuleWithElements"/></returns>
        public IList<IProductModuleWithElements> Get()
        {
            IList<IProductModuleWithElements> accountWithElement = this._cacheFactory.Get();

            if (accountWithElement == null)
            {
                IList<IProductModule> accounts = this._sqlModulesFactory.Get();

                if (accounts != null && accounts.Count >= 1)
                {
                    accountWithElement = this.Convert(accounts);
                    this._cacheFactory.Add(accountWithElement);
                }
            }

            return accountWithElement;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IProductModuleWithElements"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IProductModuleWithElements"/> that match <paramref name="predicate"/>.</returns>
        public IList<IProductModuleWithElements> Get(Predicate<IProductModuleWithElements> predicate)
        {
            if (predicate == null)
            {
                return null;
            }

            IList<IProductModuleWithElements> accountsWithElements = this.Get();

            if (accountsWithElements == null)
            {
                return null;
            }

            List<IProductModuleWithElements> matchedAccountWithElements = new List<IProductModuleWithElements>();

            foreach (IProductModuleWithElements accountWithElements in accountsWithElements)
            {
                if (predicate.Invoke(accountWithElements))
                {
                    matchedAccountWithElements.Add(accountWithElements);
                }
            }

            return matchedAccountWithElements;
        }

        /// <summary>
        /// Converts instances of <see cref="IProductModule"/> to <see cref="IProductModuleWithElements"/>.
        /// </summary>
        /// <param name="modules">The collection of <see cref="IProductModule"/> to convert.</param>
        /// <returns>A collection of <see cref="IProductModuleWithElements"/> converted from <paramref name="modules"/>.</returns>
        private IList<IProductModuleWithElements> Convert(IList<IProductModule> modules)
        {
            IList<IProductModuleWithElements> convertModulesWithElements = new List<IProductModuleWithElements>();

            string sql =
                @"SELECT moduleID, elementID FROM dbo.moduleElementBase";

            if (modules.Count == 1)
            {
                sql += " WHERE moduleID = @ModuleID";
                this._metabaseDataConnection.Parameters.Add(new SqlParameter("@ModuleID", SqlDbType.Int) { Value = modules[0].Id });
            }

            Dictionary<int, IList<IElement>> elements = new Dictionary<int, IList<IElement>>();
            var theElements = this._sqlElementFactory.Get();

            using (DbDataReader reader = this._metabaseDataConnection.GetReader(sql))
            {
                while (reader.Read())
                {
                    var accountIdOrdinal = reader.GetOrdinal("moduleID");
                    var elementIdOrd = reader.GetOrdinal("elementID");

                    if (!reader.IsDBNull(accountIdOrdinal))
                    {
                        var moduleID = reader.GetInt32(accountIdOrdinal);
                        var elementId = reader.GetInt32(elementIdOrd);

                        if (!elements.ContainsKey(moduleID))
                        {
                            elements.Add(moduleID, new List<IElement>());
                        }

                        var element = theElements.Find(x => x.Id == elementId);

                        if (element != null)
                        {
                            elements[moduleID].Add(element);
                        }
                    }
                }
            }

            this._metabaseDataConnection.Parameters.Clear();

            foreach (IProductModule productModule in modules)
            {
                elements.TryGetValue(productModule.Id, out IList<IElement> elementsList);

                IProductModuleWithElements moduleWithElements = null;

                switch ((Modules)productModule.Id)
                {
                    case Modules.PurchaseOrders:
                        moduleWithElements = new PurchaseOrderProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.Expenses:
                        moduleWithElements = new ExpensesProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.Contracts:
                        moduleWithElements = new FrameworkProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.SpendManagement:
                        moduleWithElements = new SpendManagementProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        moduleWithElements = new CorporateDilligenceProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.Greenlight:
                        moduleWithElements = new GreenLightProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.GreenlightWorkforce:
                        moduleWithElements = new GreenLightWorkForceProductModuleWithElements( productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    case Modules.ESR:
                        moduleWithElements = new EsrProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                    default:
                        moduleWithElements = new NullProductModuleWithElements(productModule.Id, productModule.Name, productModule.Description, productModule.BrandName, productModule.BrandNameHtml, elementsList);
                        break;
                }

                convertModulesWithElements.Add(moduleWithElements);
            }

            return convertModulesWithElements;
        }
    }
}
