using System.Data.Common;

namespace BusinessLogic.DataConnections
{
    using System.Data;

    /// <summary>
    /// Defines the common fields for a data connection
    /// </summary>
    public interface IDataConnection<T> where T : class
    {
        /// <summary>
        /// Gets or sets the parameters of a database command
        /// </summary>
        DataParameters<T> Parameters { get; set; }

        /// <summary>
        /// Executes the provided stored procedure
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute</param>
        void ExecuteProc(string procName);

        /// <summary>
        /// Executes the provided stored procedure
        /// </summary>
        /// <typeparam name="TReturnType">Specifies teh stored procedure return value data type.</typeparam>
        /// <param name="procName">
        /// The name of the stored procedure to execute
        /// </param>
        /// <returns>The value of type <see cref="TReturnType"/></returns>
        TReturnType ExecuteProc<TReturnType>(string procName);

        /// <summary>
        /// Executes a scalar and returns a defined type
        /// </summary>
        /// <typeparam name="TReturnType">The expected return type</typeparam>
        /// <param name="strsql">The sql to execute</param>
        /// <param name="commandType">The <see cref="CommandType">CommandType</see></param>
        /// <returns>The value of type <see cref="TReturnType"/></returns>
        TReturnType ExecuteScalar<TReturnType>(string strsql, CommandType commandType);

        /// <summary>
        /// Executes a scalar and returns a defined type
        /// </summary>
        /// <typeparam name="TReturnType">The expected return type</typeparam>
        /// <param name="strsql">The sql to execute</param>
        /// <returns>The value of type <see cref="TReturnType"/></returns>
        TReturnType ExecuteScalar<TReturnType>(string strsql);

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>The number of rows affected by <paramref name="sql"/></returns>
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// Gets a Reader
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <returns>The reader</returns>
        DbDataReader GetReader(string sql);

        /// <summary>
        /// Gets a Reader
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="commandType">The <see cref="CommandType">CommandType</see></param>
        /// <returns>The reader</returns>
        DbDataReader GetReader(string sql, CommandType commandType);

    }
}