namespace EsrGo2FromNhs.Base
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.Caching;

    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The log DB connection.
    /// </summary>
    public class LogDbConnection : IApiDbConnection
    {
        /// <summary>
        /// The SQL command.
        /// </summary>
        private SqlCommand sqlCommand;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogDbConnection"/> class.
        /// </summary>
        public LogDbConnection()
        {
            try
            {
                this.ConnectionString = ConfigurationManager.ConnectionStrings["ApiLog"].ConnectionString;
                if (this.ConnectionString.Contains(ConfigurationManager.AppSettings["dbpassword"]))
                {
                    this.ConnectionString = this.ConnectionString.Replace(ConfigurationManager.AppSettings["dbpassword"], this.Decrypt(ConfigurationManager.AppSettings["dbpassword"]));
                }

                this.DebugLevel = this.GetEsrDiagnostics();
                this.ConnectionStringValid = true;
                this.ErrorMessage = string.Empty;
            }
            catch (Exception)
            {
                this.ConnectionStringValid = false;
            }
        }

        /// <summary>
        /// Gets or sets the debug level.
        /// </summary>
        public Log.MessageLevel DebugLevel { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether connection string valid.
        /// </summary>
        public bool ConnectionStringValid { get; set; }

        /// <summary>
        /// Gets or sets the message level.
        /// </summary>
        public int MessageLevel { get; set; }

        /// <summary>
        /// Gets or sets the SQLEXECUTE object.
        /// </summary>
        /// <summary>
        /// Gets or sets the SQL execute.
        /// </summary>
        public SqlCommand Sqlexecute
        {
            get
            {
                return this.sqlCommand ?? (this.sqlCommand = new SqlCommand());
            }

            set
            {
                this.sqlCommand = value;
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

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public MemoryCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// Gets the Key.
        /// </summary>
        private static byte[] Key
        {
            get
            {
                var thekey = new byte[]
                                 {
                                     201, 34, 61, 177, 73, 61, 42, 198, 179, 115, 39, 113, 42, 80, 255, 104, 185, 137, 89,
                                     174, 45, 65, 172, 144, 206, 102, 201, 71, 178, 0, 11, 4
                                 };
                return thekey;
            }
        }

        /// <summary>
        /// Gets the Iv.
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                var theiv = new byte[] { 148, 93, 123, 24, 109, 9, 122, 147, 64, 112, 218, 217, 11, 116, 235, 55 };
                return theiv;
            }
        }

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
            var dataconnection = new SqlConnection(this.ConnectionString);
            int result;
            try
            {
                dataconnection.Open();
                this.Sqlexecute.Connection = dataconnection;
                this.Sqlexecute.CommandType = CommandType.StoredProcedure;
                this.Sqlexecute.CommandText = strsql;
                result = this.Sqlexecute.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(
                    "Application",
                    string.Format("Execute Proc error \n {0} \n {1} \n SQL = {2}", ex.Message, ex.StackTrace, strsql));
                Debug.WriteLine(ex);
                this.ErrorMessage = ex.Message;
                result = -1;
            }
            finally
            {
                dataconnection.Close();
            }

            return result;
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
            try
            {
                sqlreader = this.Sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
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
            }

            return sqlreader;
        }

        public bool GetAccountDetails(int accountId)
        {
            throw new NotImplementedException();
        }

        public List<Account> GetAccounts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decrypt password method.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Decrypt(string data)
        {
            using (var decryptor = new System.Security.Cryptography.RijndaelManaged())
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    byte[] inputByteArray = Convert.FromBase64String(data);
                    using (var cs = new System.Security.Cryptography.CryptoStream(stream, decryptor.CreateDecryptor(Key, Iv), System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// The get ESR diagnostics level.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageLevel"/>.
        /// </returns>
        private Log.MessageLevel GetEsrDiagnostics()
        {
            const string Strsql = "select stringValue from apiProperties where stringKey = 'messageLevel'";
            int stringValue = 0;
            if (this.Cache.Contains("messageLevel"))
            {
                var value = this.Cache.Get("messageLevel") as int?;
                stringValue = value ?? 0;
            }
            else
            {
                using (IDataReader reader = this.GetReader(this.ConnectionString, Strsql))
                {
                    int stringValueOrd = reader.GetOrdinal("stringValue");

                    while (reader.Read())
                    {
                        var value = reader.GetString(stringValueOrd);
                        int.TryParse(value, out stringValue);
                    }

                    this.Cache.Add("messageLevel", stringValue, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                }
            }

            return (Log.MessageLevel)stringValue;
        }

        public void Dispose()
        {
            this.Sqlexecute.Dispose();
            this.Sqlexecute = null;
        }
    }
}