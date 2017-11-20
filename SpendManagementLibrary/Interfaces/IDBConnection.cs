// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDBConnection.cs" company="Software (Europe) Ltd">
//   Copyright (c) Software (Europe) Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SpendManagementLibrary.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web.Caching;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// The DBConnection interface.
    /// </summary>
    public interface IDBConnection : IDisposable
    {
        #region Public Methods and Operators
        /// <summary>
        /// Gets or sets the SQL execute.
        /// </summary>
        SqlCommand sqlexecute { get; set; }

        /// <summary>
        /// Add anything other than a nvarchar or decimal parameter to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="value">
        /// Value of the paramter
        /// </param>
        void AddWithValue(string name, object value);

        /// <summary>
        /// Add an IntPK table type to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="values">
        /// Value of the paramter
        /// </param>
        void AddWithValue(string name, List<int> values);

        /// <summary>
        /// Add an GuidPK table type to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">
        /// Name of the parameter
        /// </param>
        /// <param name="values">
        /// Value of the paramter
        /// </param>
        void AddWithValue(string name, List<Guid> values);

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
        void AddWithValue(string name, object value, int size);

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
        void AddWithValue(string name, decimal value, byte precision, byte scale);

        /// <summary>
        /// Add a collection of SqlDataRecords for an SQL User Defined Table Type to the sqlexecute.Parameters collection
        /// </summary>
        /// <param name="name">Name of the parameter, include the @</param>
        /// <param name="tableTypeName">The name of the SQL User Defined Table Type to use, include the schema name</param>
        /// <param name="records">The structured sql meta data records to represent the table of data</param>
        void AddWithValue(string name, string tableTypeName, List<SqlDataRecord> records);

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
        SqlCacheDependency CreateSQLCacheDependency(string strsql, SortedList<string, object> parameters);

        /// <summary>
        /// Adds a return parameter to the parameters collection, with the return type set to <see cref="ParameterDirection.ReturnValue">ParameterDirection.ReturnValue</see>
        /// </summary>
        /// <param name="name">The Key name for the return parameter.</param>
        /// <param name="type">The Database Data-type of the parameter.</param>
        void AddReturn(string name, SqlDbType type);

        /// <summary>
        /// Gets the return parameter that was set using <see cref="AddReturn">AddReturn</see>.
        /// </summary>
        /// <param name="key">The key of the return parameter to get.</param>
        T GetReturnValue<T>(string key);

        /// <summary>
        /// The execute proc.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        void ExecuteProc(string strsql);

        /// <summary>
        /// The execute sql.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        int ExecuteSQL(string strsql);

        /// <summary>
        /// The get data set.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        /// <param name="throwOnError">
        /// When set to false any errors will be silently logged and a null object or empty DataSet returned, if true the error is thrown up the stack
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        DataSet GetDataSet(string strsql, bool throwOnError = false);

        /// <summary>
        /// The get proc data set.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        DataSet GetProcDataSet(string strsql);

        /// <summary>
        /// The get reader.
        /// </summary>
        /// <param name="strsql">
        ///     The strsql.
        /// </param>
        /// <returns>
        /// The <see cref="SqlDataReader"/>.
        /// </returns>
        IDataReader GetReader(string strsql, CommandType commandType = CommandType.Text);

        /// <summary>
        /// The getcount.
        /// </summary>
        /// <param name="strsql">
        /// The strsql.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        T ExecuteScalar<T>(string strsql, CommandType commandType = CommandType.Text);

        #endregion
    }
}