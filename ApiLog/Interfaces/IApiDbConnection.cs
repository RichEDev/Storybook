namespace ApiLog.Interfaces
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// The API database Connection interface.
    /// </summary>
    public interface IApiDbConnection
    {
        /// <summary>
        /// Gets or sets a value indicating whether connection string valid.
        /// </summary>
        bool ConnectionStringValid { get; set; }

        /// <summary>
        /// Gets or sets the SQL execute.
        /// </summary>
        SqlCommand Sqlexecute { get; set; }

        /// <summary>
        /// Gets or sets the meta base.
        /// </summary>
        string MetaBase { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the debug level.
        /// </summary>
        ApiLog.MessageLevel DebugLevel { get; set; }

        /// <summary>
        /// The execute procedure method.
        /// </summary>
        /// <param name="strsql">
        /// The SQL string
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int ExecuteProc(string strsql);

        /// <summary>
        /// The get stored procedure reader.
        /// </summary>
        /// <param name="strSql">
        /// The SQL string.
        /// </param>
        /// <returns>
        /// The <see cref="SqlDataReader"/>.
        /// </returns>
        IDataReader GetStoredProcReader(string strSql);

        /// <summary>
        /// The get reader.
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
        IDataReader GetReader(string connectionstring, string strsql, string database = "");

        /// <summary>
        /// The decrypt.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Decrypt(string data);
    }
}
