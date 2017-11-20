namespace SQLDataAccess
{
    using System.Data.SqlClient;

    using BusinessLogic.Accounts;
    using BusinessLogic.DataConnections;

    /// <summary>
    /// The customer database connection.
    /// </summary>
    public class CustomerDatabaseConnection : SqlDataConnection, ICustomerDataConnection<SqlParameter>
    {
        public CustomerDatabaseConnection(IAccount account, DataParameters<SqlParameter> parameters) : base(new SqlConnectionStringBuilder(account.DatabaseCatalogue.ConnectionString), parameters)
        {
        }


    }
}
