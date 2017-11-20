namespace SQLDataAccess.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic.DataConnections;
    using BusinessLogic.ProductModules;

    using CacheDataAccess.Caching;

    /// <summary>
    /// The sql modules factory.
    /// </summary>
    public class SqlModulesFactory : IDataFactory<IProductModule, int>
    {
        /// <summary>
        /// The customer data connection.
        /// </summary>
        protected readonly IMetabaseDataConnection<SqlParameter> MetabaseDataConnection;

        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/>
        /// </summary>1
        private readonly MetabaseCacheFactory<IProductModule, int> _cacheFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlModulesFactory"/> class.
        /// </summary>
        /// <param name="cacheFactory">
        /// The cache factory to use for dealing with <see cref="IProductModule"/> objects.
        /// </param>
        /// <param name="metabaseDataConnection">
        /// An instance of <see cref="IMetabaseDataConnection{T}">MetabaseCustomerDataConnection</see>
        /// </param>
        public SqlModulesFactory(MetabaseCacheFactory<IProductModule, int> cacheFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            this._cacheFactory = cacheFactory;
            this.MetabaseDataConnection = metabaseDataConnection;
        }

        /// <summary>
        /// Gets a <see cref="IProductModule">IProductModule</see> by its Id from memory
        /// </summary>
        /// <param name="id">
        /// The Id of the <see cref="IProductModule">IProductModule</see>
        /// </param>
        /// <returns>
        /// A <see cref="IProductModule">IProductModule</see>
        /// </returns>
        public IProductModule this[int id]
        {
            get
            {
                IProductModule productModule = this._cacheFactory[id];

                if (productModule == null)
                {
                    productModule = this.Get(id);
                    this._cacheFactory.Add(productModule);
                }

                return productModule;
            }
        }

        /// <summary>
        /// Adds an <see cref="IProductModule"/> to cache.
        /// </summary>
        /// <param name="entity">
        /// The entity to add to cache.
        /// </param>
        /// <returns>
        /// The <see cref="IProductModule"/>.
        /// </returns>
        public IProductModule Add(IProductModule entity)
        {
            return this._cacheFactory.Add(entity);
        }

        /// <summary>
        /// Gets a <see cref="IProductModule">IProductModule</see> by its Id from the database
        /// </summary>
        /// <param name="id">
        /// The Id of the <see cref="IProductModule">IProductModule</see>
        /// </param>
        /// <returns>
        /// A <see cref="IProductModule">IProductModule</see>
        /// </returns>
        private IProductModule Get(int id)
        {
            IProductModule module;
            this.MetabaseDataConnection.Parameters.Add(new SqlParameter("@moduleID", SqlDbType.Int) { Value = id });

            using (var reader = this.MetabaseDataConnection.GetReader("SELECT moduleID, moduleName, description, brandName, brandNameHTML FROM dbo.moduleBase WHERE moduleID=@moduleID"))
            {
                reader.Read();

                int moduleId = reader.GetInt32(0);
                string moduleName = reader.GetString(1);
                string description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                string brandNameHtml = reader.GetString(3);
                
                switch (moduleId)
                {
                    case 2:
                        module = new ExpensesProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                    case 3:
                        module = new FrameworkProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                    case 5:
                    case 6:
                        module = new CorporateDilligenceProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                    case 7:
                        module = new GreenLightProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                    case 9:
                        module = new GreenLightWorkForceProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                    default:
                        module = new NullProductModule(moduleId, moduleName, description, brandNameHtml);
                        break;
                }
            }

            this.MetabaseDataConnection.Parameters.Clear();
            return module;
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<IProductModule> Get()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IProductModule> Get(Predicate<IProductModule> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
