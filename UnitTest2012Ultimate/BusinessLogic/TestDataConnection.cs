namespace UnitTest2012Ultimate.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

    using global::BusinessLogic.DataConnections;

    /// <summary>
    /// The test customer data connection.
    /// </summary>
    public class TestDataConnection : ICustomerDataConnection, IMetabaseDataConnection
    {
   
        private readonly DataSet dataSet;

        private Dictionary<string, object> scalarResponses;

        /// <summary>
        /// Initialises the <see cref="TestDataConnection"/>
        /// </summary>
        /// <param name="dataParameters">
        /// The <see cref="DataParameters"/>DataParameters
        /// </param>
        /// <param name="dataTable">
        /// The <see cref="DataTable"/>DataSet
        /// </param>
        public TestDataConnection(DataParameters dataParameters, DataTable dataTable)
        {
            this.dataSet = new DataSet();
            this.dataSet.Tables.Add(dataTable);
            this.Parameters = dataParameters;
            this.scalarResponses = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initialises the <see cref="TestDataConnection"/>
        /// </summary>
        /// <param name="dataParameters">
        /// The <see cref="DataParameters"/>DataParameters
        /// </param>
        /// <param name="dataSet">
        /// The <see cref="DataSet"/>DataSet
        /// </param>
        public TestDataConnection(DataParameters dataParameters, DataSet dataSet)
        {
            this.dataSet = dataSet;
            this.Parameters = dataParameters;
            this.scalarResponses = new Dictionary<string, object>();
        }

        /// <summary>
        /// The add response.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddResponse(string sql, object value)
        {
            this.scalarResponses.Add(sql, value);
        }

        /// <summary>
        /// The execute proc.
        /// </summary>
        /// <param name="processName">
        /// The process name.
        /// </param>
        public  void ExecuteProc(string processName)
        {
            // Do Nothing
        }

        /// <summary>
        /// Executes a scalar.
        /// </summary>
        /// <param name="strsql">
        /// The strsql to run.
        /// </param>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public  T ExecuteScalar<T>(string strsql, CommandType commandType = CommandType.Text)
        {
            return (T)this.scalarResponses[strsql];
        }

        /// <summary>
        /// Gets a reader using the SQL provided
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        public  IDataReader GetReader(string sql, CommandType commandType = CommandType.Text)
        {
            return this.FilterTable(sql);
        }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public DataParameters Parameters { get; set; }

        /// <summary>
        /// Filters a datatable by the SQL query provided
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        private IDataReader FilterTable(string sql)
        {
            DataTable dataTable = new DataTable();

            if (this.dataSet.Tables.Count == 1)
            {
                dataTable = this.dataSet.Tables[0];
            }
            else
            {
                List<string> sqlTableNames = SqlParser.GetTableNamesFromQueryString(sql);
                dataTable = this.dataSet.Tables[sqlTableNames[0]];
            }
      
            var whereClause = SqlParser.GetWhereClaseFromQueryString(sql, this.Parameters, dataTable);

            if (whereClause == string.Empty)
            {
                return dataTable.CreateDataReader();
            }
        
            var view = new DataView(dataTable);
            view.RowFilter = whereClause;

            return view.ToTable("filtered").CreateDataReader();

        }
    }
}