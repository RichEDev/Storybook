namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Caching;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Interfaces;
    using Common.Logging;

    /// <summary>
    ///     The Database connection.
    /// </summary>
    [Serializable]
    public class DatabaseConnection : IDBConnection, IDisposable
    {
        #region Fields

        /// <summary>
        /// The _database.
        /// </summary>
        [DataMember]
        private readonly string database;

        /// <summary>
        /// The _s db password.
        /// </summary>
        [DataMember]
        private readonly string sDbPassword;

        /// <summary>
        /// The _server.
        /// </summary>
        [DataMember]
        private readonly string server;

        /// <summary>
        ///     The db.
        /// </summary>
        [DataMember]
        private Database db = Database.Live;

        /// <summary>
        /// Private variable to hold disposed status.
        /// </summary>
        [DataMember]
        private bool disposed;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<DatabaseConnection>().GetLogger();

        #endregion
        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseConnection"/> class.
        /// </summary>
        /// <param name="connectionstring">
        /// The connection string.
        /// </param>
        /// <exception cref="Exception">
        /// Cannot pass empty connection string
        /// </exception>
        public DatabaseConnection(string connectionstring)
        {
            if (string.IsNullOrEmpty(connectionstring))
            {
                throw new Exception("Empty connection string passed.");
            }

            string password = ConfigurationManager.AppSettings["dbpassword"] ?? string.Empty;
            string[] temp = connectionstring.Split(';');
            this.server = temp[0].Replace("Data Source=", string.Empty);
            this.database = temp[1].Replace("Initial Catalog=", string.Empty);
            this.sDbPassword = Utilities.Cryptography.ExpensesCryptography.Decrypt(password);
            this.sqlexecute = new SqlCommand();
        }


        //restored 29/10/2013
        /// <summary>
        /// Decrypts the encrypted password in a connection string
        /// </summary>
        /// <param name="connectionStringWithEncryptedPassword"></param>
        /// <returns>The connection string with the password decrypted</returns>
        public static string GetConnectionStringWithDecryptedPassword(string connectionStringWithEncryptedPassword)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionStringWithEncryptedPassword)
            {
                Pooling = true,
                MinPoolSize = 5,
                MaxPoolSize = 1000
            };
            connectionStringBuilder.Password =
                Utilities.Cryptography.ExpensesCryptography.Decrypt(ConfigurationManager.AppSettings["dbpassword"] ??
                                                                    string.Empty);
            return connectionStringBuilder.ConnectionString;

        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the sql execute.
        /// </summary>
        public SqlCommand sqlexecute { get; set; }

        #endregion

        #region dispose methods

        /// <summary>
        /// Finalises an instance of the <see cref="DatabaseConnection"/> class. 
        /// Use C# destructor syntax for finalization code. 
        /// This destructor will run only if the Dispose method 
        /// does not get called. 
        /// It gives your base class the opportunity to finalize. 
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DatabaseConnection()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes of the database connection        
        /// </summary>
        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method. 
            // GC.SupressFinalize takes this object off the finalization queue 
            // and prevents finalization code for this object from executing a second time.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. 
        /// If disposing equals true, the method has been called directly 
        /// or indirectly by a user's code. Managed and unmanaged resources 
        /// can be disposed. 
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        /// </summary>
        /// <param name="disposing">
        /// The boolean value indicating disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (this.disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed 
            // and unmanaged resources. 
            if (disposing)
            {
                // Dispose managed resources.
                if (this.sqlexecute.Connection != null)
                {
                    if (this.sqlexecute.Connection.State == ConnectionState.Open)
                    {
                        this.sqlexecute.Connection.Close();
                    }

                    this.sqlexecute.Connection.Dispose();
                }
                this.sqlexecute.Dispose();
            }

            // disposing has been done.
            this.disposed = true;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add anything other than a nvarchar or decimal parameter to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="value">
        /// Value of the paramter
        /// </param>
        public void AddWithValue(string name, object value)
        {
            this.sqlexecute.Parameters.AddWithValue(name, value);
        }

        /// <summary>
        /// Add an input IntPK table type to the sqlexecute.Parameters collection, 
        /// note that values sent to this will be reduced to a distinct list
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="values">
        /// Value of the paramter
        /// </param>
        public void AddWithValue(string name, List<int> values)
        {
            List<SqlDataRecord> integers = new List<SqlDataRecord>();
            // Generate a sql IntPK table param and pass into the sql
            SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };

            foreach (int i in values.Distinct())
            {
                var row = new SqlDataRecord(rowMetaData);
                row.SetInt32(0, i);
                integers.Add(row);
            }

            this.sqlexecute.Parameters.Add(name, SqlDbType.Structured);
            this.sqlexecute.Parameters[name].TypeName = "dbo.IntPK";
            this.sqlexecute.Parameters[name].Direction = ParameterDirection.Input;
            this.sqlexecute.Parameters[name].Value = integers.Count == 0 ? null : integers;
        }

        /// <summary>
        /// Add an input GuidPK table type to the sqlexecute.Parameters collection, 
        /// note that values sent to this will be reduced to a distinct list
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="values">
        /// Value of the paramter
        /// </param>
        public void AddWithValue(string name, List<Guid> values)
        {
            List<SqlDataRecord> uniqueIdentifiers = new List<SqlDataRecord>();
            // Generate a sql GuidPK table param and pass into the sql
            SqlMetaData[] rowMetaData = { new SqlMetaData("ID", SqlDbType.UniqueIdentifier) };

            foreach (Guid i in values.Distinct())
            {
                var row = new SqlDataRecord(rowMetaData);
                row.SetGuid(0, i);
                uniqueIdentifiers.Add(row);
            }

            this.sqlexecute.Parameters.Add(name, SqlDbType.Structured);
            this.sqlexecute.Parameters[name].TypeName = "dbo.GuidPK";
            this.sqlexecute.Parameters[name].Direction = ParameterDirection.Input;
            this.sqlexecute.Parameters[name].Value = uniqueIdentifiers.Count == 0 ? null : uniqueIdentifiers;
        }

        /// <summary>
        /// Add an NVarChar to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="value">
        /// Value of the paramter
        /// </param>
        /// <param name="size">
        /// The length of the column
        /// </param>
        public void AddWithValue(string name, object value, int size)
        {
            this.sqlexecute.Parameters.Add(name, SqlDbType.NVarChar, size);
            this.sqlexecute.Parameters[name].Value = value;
        }

        /// <summary>
        /// Add a decimal parameter to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="value">
        /// Value of the paramter
        /// </param>
        /// <param name="precision">
        /// Precision of the decimal
        /// </param>
        /// <param name="scale">
        /// Scale of the decimal
        /// </param>
        public void AddWithValue(string name, decimal value, byte precision, byte scale)
        {
            this.sqlexecute.Parameters.Add(name, SqlDbType.Decimal);
            this.sqlexecute.Parameters[name].Value = value;
            this.sqlexecute.Parameters[name].Precision = precision;
            this.sqlexecute.Parameters[name].Scale = scale;
        }

        /// <summary>
        /// Add a collection of SqlDataRecords for an SQL User Defined Table Type to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">Name of the parameter, include the @</param>
        /// <param name="tableTypeName">The name of the SQL User Defined Table Type to use, include the schema name</param>
        /// <param name="records">The structured sql meta data records to represent the table of data</param>
        public void AddWithValue(string name, string tableTypeName, List<SqlDataRecord> records)
        {
            this.sqlexecute.Parameters.Add(name, SqlDbType.Structured);
            this.sqlexecute.Parameters[name].TypeName = tableTypeName;
            this.sqlexecute.Parameters[name].Direction = ParameterDirection.Input;
            this.sqlexecute.Parameters[name].Value = records == null || records.Count == 0 ? null : records;
        }

        /// <summary>
        /// Returns System.Web.Cache.SqlCacheDependency
        /// </summary>
        /// <param name="strsql">
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <returns>
        /// The <see cref="SqlCacheDependency"/>.
        /// </returns>
        public SqlCacheDependency CreateSQLCacheDependency(string strsql, SortedList<string, object> parameters)
        {
            SqlCacheDependency dep;
            using (var connection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.ChangeDatabase(this.database);
                    var comm = new SqlCommand(strsql) { CommandType = CommandType.Text, Connection = connection };

                    if (parameters != null)
                    {
                        foreach (var i in parameters)
                        {
                            comm.Parameters.AddWithValue(i.Key, i.Value);
                        }
                    }

                    dep = new SqlCacheDependency(comm);
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return dep;
        }

        /// <summary>
        /// Adds a return parameter to the parameters collection, with the return type set to <see cref="ParameterDirection.ReturnValue">ParameterDirection.ReturnValue</see>
        /// </summary>
        /// <param name="key">The Key name for the return parameter.</param>
        public void AddReturn(string key)
        {
            this.AddReturn(key, SqlDbType.Int);
        }

        /// <summary>
        /// Adds a return parameter to the parameters collection, with the return type set to <see cref="ParameterDirection.ReturnValue">ParameterDirection.ReturnValue</see>
        /// </summary>
        /// <param name="key">The Key name for the return parameter.</param>
        /// <param name="type">The Database Data-type of the parameter.</param>
        public void AddReturn(string key, SqlDbType type)
        {
            if (!key.StartsWith("@")) key = key.Insert(0, "@");
            sqlexecute.Parameters.Add(key, type);
            sqlexecute.Parameters[key].Direction = ParameterDirection.ReturnValue;
        }

        /// <summary>
        /// Gets the return parameter that was set using <see cref="AddReturn">AddReturn</see>.
        /// </summary>
        /// <param name="key">The key of the return parameter to get.</param>
        public T GetReturnValue<T>(string key)
        {
            if (!key.StartsWith("@")) key = key.Insert(0, "@");
            var toReturn = sqlexecute.Parameters[key];
            if (toReturn == null) throw new ArgumentException("Parameter doesn't exist by this key.");
            if (toReturn.Direction != ParameterDirection.ReturnValue) throw new ArgumentException("Key is not a return parameter.");
            return (T)sqlexecute.Parameters[key].Value;
        }

        /// <summary>
        /// The execute proc.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        public void ExecuteProc(string strsql)
        {
            var connection = new SqlConnection(this.GetConnectionString());

            // strsql = "use " + database + ";" + strsql;
            try
            {
                connection.Open();
                connection.ChangeDatabase(this.database);

                this.sqlexecute.Connection = connection;
                this.sqlexecute.CommandType = CommandType.StoredProcedure;
                this.sqlexecute.CommandText = strsql;
                this.sqlexecute.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\n\n" + ex.StackTrace);
                Debug.WriteLine(ex);
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// The execute sql.
        /// </summary>
        /// <param name="SQL">
        /// The strsql.
        /// </param>
        public int ExecuteSQL(string SQL)
        {
            int affectedRows;

            using (var connection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.ChangeDatabase(this.database);

                    this.sqlexecute.Connection = connection;
                    this.sqlexecute.CommandType = CommandType.Text;
                    this.sqlexecute.CommandText = SQL;
                    affectedRows = this.sqlexecute.ExecuteNonQuery();
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return affectedRows;
        }

        /// <summary>
        /// The execute sql.
        /// </summary>
        /// <param name="sql">
        /// The strsql.
        /// </param>
        public object ExecuteScalarSql(string sql)
        {
            object value;

            using (var connection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.ChangeDatabase(this.database);

                    this.sqlexecute.Connection = connection;
                    this.sqlexecute.CommandType = CommandType.Text;
                    this.sqlexecute.CommandText = sql;
                    value = this.sqlexecute.ExecuteScalar();
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// The get data set.
        /// </summary>
        /// <param name="sql">
        /// The strsql.
        /// </param>
        /// <param name="throwOnError">
        /// When set to false any errors will be silently logged and a null object or empty DataSet returned, if true the error is thrown up the stack
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GetDataSet(string sql, bool throwOnError = false)
        {
            DataSet dataSet = null;

            using (var dataconnection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    dataconnection.Open();
                    dataconnection.ChangeDatabase(this.database);

                    using (var sqlAdapter = new SqlDataAdapter())
                    {
                        this.sqlexecute.Connection = dataconnection;
                        dataSet = new DataSet();
                        this.sqlexecute.CommandText = sql;
                        this.sqlexecute.CommandType = CommandType.Text;
                        sqlAdapter.SelectCommand = this.sqlexecute;
                        sqlAdapter.Fill(dataSet);
                    }
                }
                catch
                {
                    if (throwOnError)
                        throw;
                }
            }
            return dataSet;
        }

        /// <summary>
        /// The get proc data set.
        /// </summary>
        /// <param name="sql">
        /// The strsql.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GetProcDataSet(string sql)
        {
            DataSet dataSet;
            using (var connection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.ChangeDatabase(this.database);

                    var sqladapter = new SqlDataAdapter();

                    this.sqlexecute.Connection = connection;
                    dataSet = new DataSet();
                    this.sqlexecute.CommandText = sql;
                    this.sqlexecute.CommandType = CommandType.StoredProcedure;
                    sqladapter.SelectCommand = this.sqlexecute;
                    sqladapter.Fill(dataSet);
                }
                finally
                {
                    if (this.sqlexecute.Connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return dataSet;
        }

        /// <summary>
        /// The get reader.
        /// </summary>
        /// <param name="sql">
        /// The strsql.
        /// </param>
        /// <param name="commandType">The <see cref="CommandType"/> for the reader</param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        public IDataReader GetReader(string sql, CommandType commandType = CommandType.Text)
        {
            var connection = new SqlConnection(this.GetConnectionString());
            SqlDataReader sqlreader = null;
            connection.Open();
            connection.ChangeDatabase(this.database);

            this.sqlexecute.Connection = connection;
            this.sqlexecute.CommandText = sql;
            this.sqlexecute.CommandType = commandType;
            try
            {
                sqlreader = this.sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error($"A SQL exception occured running {sql} with command type {commandType}", ex);
                }
            }
            catch (Exception ex)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error($"An exception occured running {sql} with command type {commandType}", ex);
                }
            }

            return sqlreader;
        }

        /// <summary>
        /// Return a single value of known type from a command string or stored procedure.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public T ExecuteScalar<T>(string strsql, CommandType commandType = CommandType.Text)
        {
            T count;
            using (var dataconnection = new SqlConnection(this.GetConnectionString()))
            {
                try
                {
                    dataconnection.Open();
                    dataconnection.ChangeDatabase(this.database);

                    this.sqlexecute.Connection = dataconnection;
                    this.sqlexecute.CommandText = strsql;
                    this.sqlexecute.CommandType = commandType;
                    count = (T)this.sqlexecute.ExecuteScalar();
                }
                finally
                {
                    if (this.sqlexecute.Connection.State != ConnectionState.Closed)
                    {
                        dataconnection.Close();
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Clear the parameters list of the underlying sqlexecute.s
        /// </summary>
        public void ClearParameters()
        {
            this.sqlexecute.Parameters.Clear();
        }

        #endregion

        /// <summary>
        ///     The get connection string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                Pooling = true,
                MinPoolSize = 5,
                MaxPoolSize = 1000,
                InitialCatalog = "expenses_temp",
                DataSource = this.server,
                ApplicationName = GlobalVariables.DefaultApplicationInstanceName
            };
            string user = "spenduser";
            string activeModule = ConfigurationManager.AppSettings["active_module"];
            if (!string.IsNullOrEmpty(activeModule) && (Modules)Convert.ToInt32(activeModule) == Modules.ESR)
            {
                // we know we are connecting from a service, as this setting is now removed from web configs
                user = "esruser";
            }

            builder.UserID = user;
            builder.Password = this.sDbPassword;
            return builder.ConnectionString;
        }

    }
}