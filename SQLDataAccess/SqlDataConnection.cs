namespace SQLDataAccess
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.DataConnections;

    public abstract class SqlDataConnection : IDataConnection<SqlParameter>
    {
        protected SqlConnectionStringBuilder ConnectionString;

        private readonly string _initialCatalogue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        protected SqlDataConnection(SqlConnectionStringBuilder connectionString, DataParameters<SqlParameter> parameters)
        {
            Guard.ThrowIfNull(connectionString, nameof(connectionString));
            Guard.ThrowIfNull(parameters, nameof(parameters));

            this.Parameters = parameters;
            this._initialCatalogue = connectionString.InitialCatalog;
            connectionString.InitialCatalog = "expenses_temp";
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the parameters of a database command
        /// </summary>
        public DataParameters<SqlParameter> Parameters { get; set; }

        /// <summary>
        /// Executes the provided stored procedure
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to execute</param>
        public void ExecuteProc(string storedProcedureName)
        {
            using (var connection = new SqlConnection(this.ConnectionString.ToString()))
            {
                connection.Open();
                connection.ChangeDatabase(this._initialCatalogue);
                var sqlExecute = this.CreateSqlCommand();
                sqlExecute.Connection = connection;
                sqlExecute.CommandType = CommandType.StoredProcedure;
                sqlExecute.CommandText = storedProcedureName;
                sqlExecute.ExecuteNonQuery();

                if (this.Parameters.ReturnValue != null)
                {
                    this.Parameters.ReturnValue = sqlExecute.Parameters["@returnvalue"].Value as string;
                }
            }
        }

        /// <summary>
        /// Executes the provided stored procedure and returns the response.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to execute</param>
        public TReturnType ExecuteProc<TReturnType>(string storedProcedureName)
        {
            using (var connection = new SqlConnection(this.ConnectionString.ToString()))
            {
                connection.Open();
                connection.ChangeDatabase(this._initialCatalogue);
                var sqlExecute = this.CreateSqlCommand();
                sqlExecute.Connection = connection;
                sqlExecute.CommandType = CommandType.StoredProcedure;
                sqlExecute.CommandText = storedProcedureName;
                sqlExecute.ExecuteNonQuery();

                if (this.Parameters.ReturnValue != null)
                {
                    string returnParameter = null;

                    foreach (SqlParameter sqlParameter in sqlExecute.Parameters)
                    {
                        if (sqlParameter.Direction == ParameterDirection.ReturnValue)
                        {
                            returnParameter = sqlParameter.ParameterName;
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(returnParameter) == false)
                    {
                        this.Parameters.ReturnValue = (TReturnType) sqlExecute.Parameters[returnParameter].Value;
                    }
                }
            }

            return (TReturnType)this.Parameters.ReturnValue;
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>The number of rows affected by <paramref name="sql"/></returns>
        public int ExecuteNonQuery(string sql)
        {
            int count;

            using (var connection = new SqlConnection(this.ConnectionString.ToString()))
            {
                var sqlExecute = this.CreateSqlCommand();

                connection.Open();
                connection.ChangeDatabase(this._initialCatalogue);

                sqlExecute.Connection = connection;
                sqlExecute.CommandType = CommandType.Text;
                sqlExecute.CommandText = sql;
                count = sqlExecute.ExecuteNonQuery();
            }

            return count;
        }

        /// <summary>
        /// Executes a scalar and returns a defined type
        /// </summary>
        /// <typeparam name="TReturnType">The expected return type</typeparam>
        /// <param name="sql">The sql to execute</param>
        /// <param name="commandType">The <see cref="CommandType">CommandType</see></param>
        /// <returns>The value of type <see cref="TReturnType"/></returns>
        public TReturnType ExecuteScalar<TReturnType>(string sql, CommandType commandType)
        {
            TReturnType procedureReturnResult;
            
            using (var connection = new SqlConnection(this.ConnectionString.ToString()))
            {
                var sqlExecute = this.CreateSqlCommand();

                connection.Open();
                connection.ChangeDatabase(this._initialCatalogue);

                sqlExecute.Connection = connection;
                sqlExecute.CommandType = commandType;
                sqlExecute.CommandText = sql;
                procedureReturnResult = (TReturnType)sqlExecute.ExecuteScalar();
            }

            return procedureReturnResult;
        }

        public TReturnType ExecuteScalar<TReturnType>(string sql)
        {
            return this.ExecuteScalar<TReturnType>(sql, CommandType.Text);
        }

        public DbDataReader GetReader(string sql)
        {
            return this.GetReader(sql, CommandType.Text);
        }

        /// <summary>
        /// Gets a Reader
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="commandType">The <see cref="CommandType">CommandType</see></param>
        /// <returns>The reader</returns>
        public DbDataReader GetReader(string sql, CommandType commandType)
        {
            var connection = new SqlConnection(this.ConnectionString.ConnectionString);

            connection.Open();
            connection.ChangeDatabase(this._initialCatalogue);
            var sqlExecute = this.CreateSqlCommand();
            sqlExecute.Connection = connection;
            sqlExecute.CommandText = sql;
            sqlExecute.CommandType = commandType;

            var reader = sqlExecute.ExecuteReader(CommandBehavior.CloseConnection);
            
            return reader;
        }

        /// <summary>
        /// The create sql command.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        internal SqlCommand CreateSqlCommand()
        {
            var result = new SqlCommand();

            foreach (SqlParameter dataParameter in this.Parameters)
            {
                result.Parameters.Add(dataParameter);
            }

            if (this.Parameters.ReturnValue != null && this.Parameters.ReturnValue.ToString().Length > 0)
            {
                result.Parameters.Add(this.Parameters.ReturnValue.ToString(), SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            }

            return result;
        }
    }
}
