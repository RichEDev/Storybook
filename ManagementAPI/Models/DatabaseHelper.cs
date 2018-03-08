namespace ManagementAPI.Models
{
    using System.Configuration;
    using System.Data.SqlClient;

    public static class DatabaseHelper
    {
        /// <summary>
        /// Connects to a database according to connection string in the web.config file.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetConnection()
        {
            SecureData crypt = new SecureData();
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            connectionString.Password = crypt.Decrypt(connectionString.Password);

            return new SqlConnection(connectionString.ConnectionString);
        }
    }
}