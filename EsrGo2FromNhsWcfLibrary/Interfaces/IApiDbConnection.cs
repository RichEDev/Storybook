namespace EsrGo2FromNhsWcfLibrary.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The API database Connection interface.
    /// </summary>
    public interface IApiDbConnection : IDisposable
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
        Log.MessageLevel DebugLevel { get; set; }

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
        /// The get account details.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool GetAccountDetails(int accountId);

        List<Account> GetAccounts();
    }
}
