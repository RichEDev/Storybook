namespace SQLDataAccess.ProductModules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using CacheDataAccess.Caching;

    /// <summary>
    /// The sql modules factory.
    /// </summary>
    public class SqlProductModulesFactory : IDataFactory<IProductModule, Modules>
    {
        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IProductModule"/> instances.
        /// </summary>
        private readonly MetabaseCacheFactory<IProductModule, int> _cacheFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProductModulesFactory"/> class. 
        /// </summary>
        /// <param name="cacheFactory">
        /// An instance of <see cref="MetabaseCacheFactory{T,TK}"/> to handle caching of <see cref="IProductModule"/> instances.
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use for accessing SQL
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cacheFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metabaseDataConnection"/> is <see langword="null"/>.
        /// </exception>
        public SqlProductModulesFactory(MetabaseCacheFactory<IProductModule, int> cacheFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(metabaseDataConnection, nameof(metabaseDataConnection));

            this._cacheFactory = cacheFactory;
            this._metabaseDataConnection = metabaseDataConnection;
        }

        /// <summary>
        /// Gets an instance of <see cref="IProductModule"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IProductModule"/> you want to retrieve</param>
        /// <returns>The required <see cref="IProductModule"/> or <see langword="null" /> if it cannot be found</returns>
        public IProductModule this[Modules id]
        {
            get
            {
                IProductModule productModule = this._cacheFactory[(int)id];

                if (productModule == null)
                {
                    var productModules = this.GetFromDatabase((int)id);

                    if (productModules != null && productModules.Count > 0)
                    {
                        productModule = productModules[0];
                        this._cacheFactory.Save(productModule);
                    }
                }

                return productModule;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IProductModule"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="ICache{T,TK}"/> and <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="entity">The <see cref="IProductModule"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IProductModule"/>.</returns>
        public IProductModule Save(IProductModule entity)
        {
            if (entity == null)
            {
                return null;
            }

            return this._cacheFactory.Save(entity);
        }

        /// <summary>
        /// Deletes the instance of <see cref="IProductModule"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProductModule"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(Modules id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all available <see cref="IProductModule"/>
        /// </summary>
        /// <returns>The list of <see cref="IProductModule"/></returns>
        public IList<IProductModule> Get()
        {
            IList<IProductModule> productModules = this._cacheFactory.Get();

            if (productModules == null)
            {
                productModules = this.GetFromDatabase(null);

                if (productModules.Count > 0)
                {
                    this._cacheFactory.Add(productModules);
                }
            }

            return productModules;
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IProductModule"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IProductModule"/> that match <paramref name="predicate"/>.</returns>
        public IList<IProductModule> Get(Predicate<IProductModule> predicate)
        {
            if (predicate == null)
            {
                return null;
            }

            IList<IProductModule> productModules = this.Get();

            if (productModules == null)
            {
                return null;
            }

            List<IProductModule> matchedProductModules = new List<IProductModule>();

            foreach (IProductModule productModule in productModules)
            {
                if (predicate.Invoke(productModule))
                {
                    matchedProductModules.Add(productModule);
                }
            }

            return matchedProductModules;
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductModule"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IProductModule"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IProductModule"/></param>
        /// <returns>The required <see cref="IProductModule"/> or <see langword="null" /> or a collection of <see cref="IProductModule"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private IList<IProductModule> GetFromDatabase(int? id)
        {
            IList<IProductModule> modules = new List<IProductModule>();

            string sql = @"SELECT moduleID, moduleName, description, brandName, brandNameHTML FROM dbo.moduleBase";
            
            if (id.HasValue)
            {
                sql += " WHERE moduleID=@moduleID";
                this._metabaseDataConnection.Parameters.Add(new SqlParameter("@moduleID", SqlDbType.Int) { Value = id });
            }

            using (var reader = this._metabaseDataConnection.GetReader(sql))
            {
                int moduleIdOrdinal = reader.GetOrdinal("moduleID");
                int moduleNameOrdinal = reader.GetOrdinal("moduleName");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int brandNameOrdinal = reader.GetOrdinal("brandName");
                int brandNameHtmlOrdinal = reader.GetOrdinal("brandNameHTML");

                while (reader.Read())
                {
                    int moduleId = reader.GetInt32(moduleIdOrdinal);
                    string moduleName = reader.GetString(moduleNameOrdinal);
                    string description = reader.IsDBNull(descriptionOrdinal) ? string.Empty : reader.GetString(descriptionOrdinal);
                    string brandName = reader.GetString(brandNameOrdinal);
                    string brandNameHtml = reader.GetString(brandNameHtmlOrdinal);

                    IProductModule module = null;

                    switch ((Modules)moduleId)
                    {
                        case Modules.PurchaseOrders:
                            module = new PurchaseOrderProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.Expenses:
                            module = new ExpensesProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.Contracts:
                            module = new FrameworkProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.SpendManagement:
                            module = new SpendManagementProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.SmartDiligence:
                        case Modules.CorporateDiligence:
                            module = new CorporateDilligenceProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.Greenlight:
                            module = new GreenLightProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.GreenlightWorkforce:
                            module = new GreenLightWorkForceProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        case Modules.ESR:
                            module = new EsrProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                        default:
                            module = new NullProductModule(moduleId, moduleName, description, brandName, brandNameHtml);
                            break;
                    }

                    if (module != null)
                    {
                        modules.Add(module);
                    }
                }
            }

            this._metabaseDataConnection.Parameters.Clear();
            return modules;
        }
    }
}
