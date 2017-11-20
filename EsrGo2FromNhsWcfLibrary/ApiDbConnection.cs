namespace EsrGo2FromNhsWcfLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Caching;
    using System.Text;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Utilities.Cryptography;

    /// <summary>
    /// The API Database connection.
    /// </summary>
    public class ApiDbConnection : IApiDbConnection
    {
        /// <summary>
        /// The METABASE connection string.
        /// </summary>
        private readonly string metabaseConnectionString;

        /// <summary>
        /// The SQL command.
        /// </summary>
        public SqlCommand SqlCommand;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log logger;

        private readonly MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiDbConnection"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="loggerDb">
        /// The logger DataBase.
        /// </param>
        public ApiDbConnection(string metabase, int accountId, IApiDbConnection loggerDb = null)
        {
            this.metabaseConnectionString = ConfigurationManager.ConnectionStrings[metabase].ConnectionString;
            if (this.metabaseConnectionString.Contains(ConfigurationManager.AppSettings["dbpassword"]))
            {
                this.metabaseConnectionString = this.metabaseConnectionString.Replace(ConfigurationManager.AppSettings["dbpassword"], ExpensesCryptography.Decrypt(ConfigurationManager.AppSettings["dbpassword"]));
            }

            this.ConnectionStringValid = this.GetAccountDetails(accountId);
            this.MetaBase = metabase;
            this.AccountId = accountId;
            this.ErrorMessage = string.Empty;
            this.logger = loggerDb == null ? new Log() : new Log(loggerDb);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiDbConnection"/> class.
        /// Used to populate the list of account details.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        public ApiDbConnection(string metaBase)
        {
            this.metabaseConnectionString = ConfigurationManager.ConnectionStrings[metaBase].ConnectionString;
            if (this.metabaseConnectionString.Contains(ConfigurationManager.AppSettings["dbpassword"]))
            {
                this.metabaseConnectionString = this.metabaseConnectionString.Replace(ConfigurationManager.AppSettings["dbpassword"], ExpensesCryptography.Decrypt(ConfigurationManager.AppSettings["dbpassword"]));
            }

            this.ConnectionStringValid = this.GetAccountDetails(0);
            this.MetaBase = metaBase;
            this.AccountId = 0;
            this.ErrorMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether connection string valid.
        /// </summary>
        public bool ConnectionStringValid { get; set; }

        /// <summary>
        /// Gets or sets the SQL execute.
        /// </summary>
        public SqlCommand Sqlexecute
        {
            get
            {
                return this.SqlCommand ?? (this.SqlCommand = new SqlCommand());
            }

            set
            {
                this.SqlCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets the meta base.
        /// </summary>
        public string MetaBase { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        public Log.MessageLevel DebugLevel { get; set; }

        /// <summary>
        /// Execute a stored procedure.
        /// </summary>
        /// <param name="strsql">
        /// The SQL string.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// the number of affected rows
        /// </returns>
        public int ExecuteProc(string strsql)
        {
            using (var dataconnection = new SqlConnection(this.ConnectionString))
            {
                int result;
                try
                {
                    dataconnection.Open();
                    this.Sqlexecute.Connection = dataconnection;
                    this.Sqlexecute.CommandType = CommandType.StoredProcedure;
                    this.Sqlexecute.CommandText = strsql;
                    this.Sqlexecute.CommandTimeout = 120;
                    result = this.Sqlexecute.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    this.ErrorMessage = ex.Message;
                    var sb = new StringBuilder();

                    if (this.Sqlexecute.Parameters.Count == 1
                        && this.Sqlexecute.Parameters[0].SqlDbType == SqlDbType.Structured)
                    {
                        sb.AppendFormat("Batch list parameter - too large for log entry.");
                    }
                    else
                    {
                        foreach (SqlParameter parameter in this.Sqlexecute.Parameters)
                        {
                            sb.AppendFormat("{0} = {1}", parameter.ParameterName, parameter.Value);
                        }                        
                    }

                    this.logger.Write(this.MetaBase, "0", this.AccountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, ex.Message, string.Format("ApiDbConnection.{0} ", strsql));
                    this.logger.Write(this.MetaBase, "0", this.AccountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, sb.ToString(), string.Format("ApiDbConnection.{0} ", strsql));
                    result = -1;
                }
                finally
                {
                    dataconnection.Close();
                }

                return result;
            }
        }

        /// <summary>
        /// Read data from a stored procedure.
        /// </summary>
        /// <param name="strSql">Name of the stored procedure.</param>
        /// <returns>SQL Reader object</returns>
        public IDataReader GetStoredProcReader(string strSql)
        {
            var dataconnection = new SqlConnection(this.ConnectionString);
            dataconnection.Open();
            SqlDataReader sqlreader = null;
            this.Sqlexecute.Connection = dataconnection;
            this.Sqlexecute.CommandText = strSql;
            this.Sqlexecute.CommandType = CommandType.StoredProcedure;
            this.Sqlexecute.CommandTimeout = 120;
            try
            {
                sqlreader = this.Sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                this.logger.Write(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    ex.Message,
                    "ApiDbConnection");
            }

            return sqlreader;
        }

        /// <summary>
        /// Get an SQL reader to read data from an SQL string (stored procedure).
        /// </summary>
        /// <param name="connectionstring">
        /// The connection string.
        /// </param>
        /// <param name="strsql">
        /// The SQL string.
        /// </param>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="SqlDataReader"/>.
        /// </returns>
        public IDataReader GetReader(string connectionstring, string strsql, string database = "")
        {
            var dataconnection = new SqlConnection(connectionstring);
            SqlDataReader sqlreader = null;
            dataconnection.Open();
            if (database != string.Empty)
            {
                dataconnection.ChangeDatabase(database);
            }

            this.Sqlexecute.Connection = dataconnection;
            this.Sqlexecute.CommandText = strsql;
            this.Sqlexecute.CommandType = CommandType.Text;
            try
            {
                sqlreader = this.Sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                this.logger.Write(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    ex.Message,
                    "ApiDbConnection");
            }

            return sqlreader;
        }

        /// <summary>
        /// The get account details.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool GetAccountDetails(int accountId)
        {
            bool result = false;
            string applicationName = "EsrApi " + this.MetaBase;
            if (this.cache.Contains(this.CacheKey(accountId)))
            {
                var currentAccount = this.cache.Get(this.CacheKey(accountId)) as Account;
                if (currentAccount != null)
                {
                    var connectionStringBuilder = new SqlConnectionStringBuilder { DataSource = currentAccount.DatabaseServer, InitialCatalog = currentAccount.DatabaseName, UserID = currentAccount.DatabaseUserName, Password = ExpensesCryptography.Decrypt(currentAccount.DatabasePassword), MaxPoolSize = 10000, ApplicationName = applicationName };
                    this.ConnectionString = connectionStringBuilder.ConnectionString;
                    result = true;
                }
            }

            if (!result)
            {
                var account = this.GetAccounts();

                foreach (Account account1 in account)
                {
                    if (account1.AccountId == accountId)
                    {
                        var connectionStringBuilder = new SqlConnectionStringBuilder { DataSource = account1.DatabaseServer, InitialCatalog = account1.DatabaseName, UserID = account1.DatabaseUserName, Password = ExpensesCryptography.Decrypt(account1.DatabasePassword), MaxPoolSize = 10000, ApplicationName = applicationName };
                        this.ConnectionString = connectionStringBuilder.ConnectionString;
                        result = true;
                    }
                }
            }

            return result;
        }

        public List<Account> GetAccounts()
        {
            var result = new List<Account>();
            const string Strsql = "SELECT registeredusers.accountid, registeredusers.dbserver, registeredusers.dbname, registeredusers.dbusername, registeredusers.dbpassword, databases.hostname FROM registeredusers INNER JOIN databases ON registeredusers.dbserver = databases.databaseID WHERE registeredusers.archived = 0";
            this.ConnectionString = string.Empty;

            using (IDataReader reader = this.GetReader(this.metabaseConnectionString, Strsql))
            {
                int accountOrdId = reader.GetOrdinal("accountid");
                int nameOrdId = reader.GetOrdinal("dbname");
                int usernameOrdId = reader.GetOrdinal("dbusername");
                int passwordOrdId = reader.GetOrdinal("dbpassword");
                int serverOrdId = reader.GetOrdinal("hostname");

                while (reader.Read())
                {
                    int accountid = reader.GetInt32(accountOrdId);
                    string databaseServer = reader.GetString(serverOrdId);
                    string databaseName = reader.GetString(nameOrdId);
                    string databaseUserName = reader.GetString(usernameOrdId);
                    string databasePassword = reader.GetString(passwordOrdId);
                    var newAccount = new Account(accountid, databaseServer, databaseName, databaseUserName, databasePassword);
                    result.Add(newAccount);
                    this.cache.Add(this.CacheKey(accountid), newAccount, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                }
            }

            return result;
        }

        /// <summary>
        /// The cache key.
        /// </summary>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CacheKey(int accountid)
        {
            return string.Format("Account_{0}", accountid);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.SqlCommand != null)
            {
                this.SqlCommand.Dispose();
                this.SqlCommand = null;
            }
        }
    }
}
