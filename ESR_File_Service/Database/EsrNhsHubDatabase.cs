using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ESR_File_Service.Database
{
    /// <summary>
    ///     Provides a class that has an empty constructor that specifies the default connectionstring from the config but decrypts the password
    /// </summary>
    public class EsrNhsHubDatabase : esrNhsHubEntities
    {
        private static readonly string EntityConnectionString;

        /// <summary>
        ///     Default empty constructor but that passes the decrypted connection string to the base class
        /// </summary>
        public EsrNhsHubDatabase() : base(EntityConnectionString)
        {
        }

        /// <summary>
        ///     A static constructor that will run before the first instance constructor to decrypt
        ///     the sql password contained in the entity connection string in the config
        ///     and create a new connectionstring from this
        /// </summary>
        static EsrNhsHubDatabase()
        {
            try
            {
                EntityConnectionStringBuilder connectionString = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["esrNhsHubEntities"].ConnectionString);
                SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder(connectionString.ProviderConnectionString);
                sqlConnectionString.Password = Utilities.Cryptography.ExpensesCryptography.Decrypt(sqlConnectionString.Password);
                connectionString.ProviderConnectionString = sqlConnectionString.ToString();
                EntityConnectionString = connectionString.ToString();
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("_File Transfer Service - ESR NHS Hub",
                    $"Exception whilst decrypting and creating a new entity connection string, check the esrNhsHubEntities connection string in the config has the correctly encrypted password.\n\nThe service cannot process files until this is corrected and the service is restarted.\n\n{exception.Message}", EventLogEntryType.Error);
                throw;
            }
        }
    }

    /// <summary>
    ///     Overrides for the automatically generated database edmx code
    /// </summary>
    public partial class esrNhsHubEntities
    {
        /// <summary>
        ///     Provide a constructor for the default edmx-generated class that can provide a connectionstring
        /// </summary>
        /// <param name="connectionString"></param>
        public esrNhsHubEntities(string connectionString) : base(connectionString)
        {
        }
    }
}
