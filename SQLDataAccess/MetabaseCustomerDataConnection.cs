namespace SQLDataAccess
{
    using System.Data.SqlClient;

    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    /// <summary>
    /// The metabase customer data connection.
    /// </summary>
    public class MetabaseDataConnection : SqlDataConnection, IMetabaseDataConnection<SqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetabaseDataConnection"/> class. 
        /// </summary>
        /// <param name="metabaseCatalogue">An instance of <see cref="MetabaseCatalogue"/>
        /// <param name="parameters">An instance of <see cref="DataParameters{T}"/> for this <see cref="MetabaseDataConnection"/></param>
        /// </param>
        public MetabaseDataConnection(MetabaseCatalogue metabaseCatalogue, DataParameters<SqlParameter> parameters) : base(new SqlConnectionStringBuilder(metabaseCatalogue.ConnectionString), parameters)
        {
        }
    }
}