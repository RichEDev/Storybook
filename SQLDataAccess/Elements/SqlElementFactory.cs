namespace SQLDataAccess.Elements
{
    using System.Data;
    using System.Data.SqlClient;

    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Elements;

    using CacheDataAccess.Caching;

    /// <summary>
    /// The sql element factory.
    /// </summary>
    public class SqlElementFactory
    {
        /// <summary>
        /// An instance of <see cref="IMetabaseDataConnection{T}"/> to use when retrieving data
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// Backing cache entity to retrieve objects from cache.
        /// </summary>
        private readonly IMetabaseCacheFactory<IElement, int> _cacheFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlElementFactory"/> class.
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> enabling caching on <see cref="SqlElementFactory"/></param>
        /// <param name="metabaseDataConnection">
        /// The customer data connection.
        /// </param>
        public SqlElementFactory(IMetabaseCacheFactory<IElement, int> cacheFactory, IMetabaseDataConnection<SqlParameter> metabaseDataConnection)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(metabaseDataConnection, nameof(metabaseDataConnection));

            this._metabaseDataConnection = metabaseDataConnection;
            this._cacheFactory = cacheFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="IElement"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally SQL
        /// </summary>
        /// <param name="id">The ID of the <see cref="IElement"/> you want to retrieve</param>
        /// <returns>The required <see cref="IElement"/> or null if it cannot be found</returns>
        public IElement this[int id]
        {
            get
            {
                IElement entity = this._cacheFactory[id];

                if (entity == null)
                {
                    entity = this.Get(id).FirstOrDefault();

                    if (entity != null)
                    {
                        this._cacheFactory.Save(entity); 
                    }
                }

                return entity;
            }
        }

        /// <summary>
        /// Gets a <see cref="IElement">IElement</see> based on its Name from the database
        /// </summary>
        /// <param name="name">
        /// The Element Name to lookup
        /// </param>
        /// <returns>
        /// The <see cref="IElement">IElement</see>
        /// </returns>
        public IElement this[string name]
        {
            get
            {
                var strSQL = "SELECT elementID, categoryID, elementName, description, accessRolesCanEdit, accessRolesCanAdd, accessRolesCanDelete, elementFriendlyName, accessRolesApplicable FROM dbo.elementsBase WHERE elementName = @elementName";
                this._metabaseDataConnection.Parameters.Clear();
                this._metabaseDataConnection.Parameters.Add(new SqlParameter("@elementName", SqlDbType.NVarChar) { Value = name });

                var lstElements = this.ReadFromDatabase(strSQL);
                return lstElements.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a List of <see cref="IElement">IElement</see> based on their Id from the database
        /// </summary>
        /// <param name="id">
        /// The Element Id to lookup
        /// </param>
        /// <returns>
        /// A List of <see cref="IElement">IElement</see>
        /// </returns>
        private List<IElement> Get(int id)
        {
            var strSQL = "SELECT elementID, categoryID, elementName, description, accessRolesCanEdit, accessRolesCanAdd, accessRolesCanDelete, elementFriendlyName, accessRolesApplicable FROM dbo.elementsBase WHERE @elementId = 0 OR elementID = @elementId";
            this._metabaseDataConnection.Parameters.Clear();
            this._metabaseDataConnection.Parameters.Add(new SqlParameter("@elementId", SqlDbType.Int) { Value = id });
            var lstElements = this.ReadFromDatabase(strSQL);

            return lstElements;
        }

        /// <summary>
        /// Gets a list of all available <see cref="IElement"/>
        /// </summary>
        /// <returns>The list of <see cref="IElement"/></returns>
        public List<IElement> Get()
        {
            var strSQL = "SELECT elementID, categoryID, elementName, description, accessRolesCanEdit, accessRolesCanAdd, accessRolesCanDelete, elementFriendlyName, accessRolesApplicable FROM dbo.elementsBase";
            this._metabaseDataConnection.Parameters.Clear();
            var lstElements = this.ReadFromDatabase(strSQL);

            return lstElements;
        }

        /// <summary>
        /// Populates a List of <see cref="IElement">IElement</see> from the database
        /// </summary>
        /// <param name="sql">
        /// The SQL to execute
        /// </param>
        private List<IElement> ReadFromDatabase(string sql)
        {
            var lstElements = new List<IElement>();
            using (var reader = this._metabaseDataConnection.GetReader(sql))
            {
                var elementIdOrd = reader.GetOrdinal("elementID");
                var categoryIdOrd = reader.GetOrdinal("categoryID");
                var elementNameOrd = reader.GetOrdinal("elementName");
                var descriptionOrd = reader.GetOrdinal("description");
                var accessRolesCanEditOrd = reader.GetOrdinal("accessRolesCanEdit");
                var accessRolesCanAddOrd = reader.GetOrdinal("accessRolesCanAdd");
                var accessRolesCanDeleteOrd = reader.GetOrdinal("accessRolesCanDelete");
                var elementFriendlyNameOrd = reader.GetOrdinal("elementFriendlyName");
                var accessRolesApplicableOrd = reader.GetOrdinal("accessRolesApplicable");
                while (reader.Read())
                {
                    var elementId = reader.GetInt32(elementIdOrd);
                    var categoryId = reader.GetInt32(categoryIdOrd);
                    var elementName = reader.GetString(elementNameOrd);
                    var description = reader.IsDBNull(descriptionOrd) ? "" : reader.GetString(descriptionOrd);
                    var canEdit = reader.GetBoolean(accessRolesCanEditOrd);
                    var canAdd = reader.GetBoolean(accessRolesCanAddOrd);
                    var canDelete = reader.GetBoolean(accessRolesCanDeleteOrd);
                    var friendlyName = reader.GetString(elementFriendlyNameOrd);
                    var accessRolesApplicable = reader.GetBoolean(accessRolesApplicableOrd);
                    IElement tmpElement = new Element(elementId, categoryId, elementName, description, canEdit, canAdd, canDelete, friendlyName, accessRolesApplicable);
                    lstElements.Add(tmpElement);
                }
            }

            return lstElements;
        }
    }
}
