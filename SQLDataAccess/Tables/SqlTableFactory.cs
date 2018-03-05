namespace SQLDataAccess.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Tables.Type;

    using CacheDataAccess.Tables;

    using Common.Logging;

    /// <summary>
    /// The sql table factory.
    /// </summary>
    public class SqlTableFactory : CacheTableFactory
    {
        /// <summary>
        /// The customer data connection.
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _metabaseDataConnection;

        /// <summary>
        /// The base sql.
        /// </summary>
        private readonly string _baseSql = "SELECT     tableid, tablename, jointype, allowreporton, description, primarykey, allowimport, keyfield, amendedon, allowworkflow, allowentityrelationship, hasUserDefinedFields,  userdefined_table, NULL AS parentTableID, elementID, subAccountIDField, CAST(0 AS INT) tableFrom, CAST(0 AS BIT) AS isSystemView, relabel_param FROM dbo.tables_base";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableFactory"/> class.
        /// </summary>
        /// <param name="metabaseDataConnection">
        /// The customer data connection.
        /// </param>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlTableFactory(IMetabaseDataConnection<SqlParameter> metabaseDataConnection, IAccount account, ICache<ITable, Guid> cache, ILog logger)
            : base(account, cache, logger)
        {
            this._metabaseDataConnection = metabaseDataConnection;
        }

        /// <summary>
        /// Gets a <see cref="ITable">ITable</see> by its GUID
        /// </summary>
        /// <param name="id">
        /// The GUID 
        /// </param>
        /// <returns>
        /// The <see cref="ITable">ITable</see>
        /// </returns>
        public override ITable this[Guid id]
        {
            get
            {
                ITable field = base[id];

                if (field == null)
                {
                    field = this.Get(id);
                    this.Save(field);
                }

                return field;
            }
        }

        /// <summary>
        /// Gets a <see cref="ITable">ITable</see> by its Name
        /// </summary>
        /// <param name="name">
        /// The name 
        /// </param>
        /// <returns>
        /// The <see cref="ITable">ITable</see>
        /// </returns>
        public override ITable this[string name]
        {
            get
            {
                this._metabaseDataConnection.Parameters.Add(new SqlParameter("@tablename", SqlDbType.NVarChar) { Value = name });
                var sql = $"{this._baseSql} WHERE tablename = @tablename";
                var result = this.GetTablesFromDb(sql).FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// The parent <see cref="ITable">ITable</see> by its GUID
        /// </summary>
        /// <param name="id">
        /// The guid
        /// </param>
        /// <returns>
        /// The <see cref="ITable">ITable</see>
        /// </returns>
        public override ITable GetParentTable(Guid id)
        {
            this._metabaseDataConnection.Parameters.Add(new SqlParameter("@parentTableId", SqlDbType.UniqueIdentifier) { Value = id });
            var sql = $"{this._baseSql} WHERE parentTableId = @parentTableId";
            var result = this.GetTablesFromDb(sql).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Gets a <see cref="ITable">ITable</see> by its ID
        /// </summary>
        /// <param name="id">
        /// The GUID 
        /// </param>
        /// <returns>
        /// The <see cref="ITable">ITable</see>
        /// </returns>
        private ITable Get(Guid id)
        {
            string strSQL = "SELECT tableid, jointype, tablename, allowreporton, allowimport, allowworkflow, allowentityrelationship, hasuserdefinedfields, description, primarykey, keyfield, userdefined_table, amendedon, elementid, subaccountidfield, tableFrom, relabel_param WHERE tableid = @tableId";

            this._metabaseDataConnection.Parameters.Add(new SqlParameter("@tableId", SqlDbType.UniqueIdentifier) { Value = id });

            ITable result = this.GetTablesFromDb(strSQL).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Gets the tables from the database
        /// </summary>
        /// <param name="strSql">
        /// The SQL to execute.
        /// </param>
        /// <returns>
        /// The a List of <see cref="ITable">ITable</see>
        /// </returns>
        private List<ITable> GetTablesFromDb(string strSql)
        {
            var result = new List<ITable>();
            using (var reader = this._metabaseDataConnection.GetReader(strSql))
            {
                var nameOrd = reader.GetOrdinal("tablename");
                var descriptionOrd = reader.GetOrdinal("description");
                var idOrd = reader.GetOrdinal("tableid");
                var primaryKeyOrd = reader.GetOrdinal("primarykey");
                var keyfieldOrd = reader.GetOrdinal("keyfield");
                var userdefinedOrd = reader.GetOrdinal("userdefined_Table");
                var subaccountOrd = reader.GetOrdinal("subAccountIdField");
                var jointypeOrd = reader.GetOrdinal("jointype");
                var elementOrd = reader.GetOrdinal("elementId");
                var reportonOrd = reader.GetOrdinal("allowreporton");
                var importOrd = reader.GetOrdinal("allowimport");
                var workflowOrd = reader.GetOrdinal("allowworkflow");
                var entityrelationshipOrd = reader.GetOrdinal("allowentityrelationship");
                var systemViewOrd = reader.GetOrdinal("issystemview");
                var tableFromOrd = reader.GetOrdinal("tableFrom");
                var relabelOrd = reader.GetOrdinal("relabel_param");

                while (reader.Read())
                {
                    var tableName = reader.GetString(nameOrd);
                    var description = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
                    Guid tableId = reader.GetGuid(idOrd);
                    Guid primaryKey = reader.IsDBNull(primaryKeyOrd) ? Guid.Empty : reader.GetGuid(primaryKeyOrd);
                    Guid keyField = reader.IsDBNull(keyfieldOrd) ? Guid.Empty : reader.GetGuid(keyfieldOrd);

                    Guid userdefinedTable = reader.IsDBNull(userdefinedOrd) ? Guid.Empty : reader.GetGuid(userdefinedOrd);
                    Guid subAccountIdField = reader.IsDBNull(subaccountOrd) ? Guid.Empty : reader.GetGuid(subaccountOrd);
                    byte joinType = reader.IsDBNull(jointypeOrd) ? (byte)0 : reader.GetByte(jointypeOrd);
                    ModuleElements elementId = reader.IsDBNull(elementOrd) ? ModuleElements.None : (ModuleElements)reader.GetInt32(elementOrd);
                    bool allowReportOn = reader.GetBoolean(reportonOrd);
                    bool allowImport = reader.GetBoolean(importOrd);
                    bool allowWorkflow = reader.GetBoolean(workflowOrd);
                    bool allowEntityRelationship = reader.GetBoolean(entityrelationshipOrd);
                    bool isSystemView = this.Account.Id != 0 && reader.GetBoolean(systemViewOrd);
                    var tableFrom = reader.GetInt32(tableFromOrd);
                    string relabelParam = reader.IsDBNull(relabelOrd) ? string.Empty : reader.GetString(relabelOrd);

                    switch (tableFrom)
                    {
                        case 0:
                            result.Add(new MetabaseTable(tableName, joinType, description, allowReportOn, allowImport, allowWorkflow, allowEntityRelationship, tableId, primaryKey, keyField, userdefinedTable, subAccountIdField, elementId, isSystemView, relabelParam));
                            break;
                        case 1:
                            result.Add(new CustomEntityTable(tableName, joinType, description, allowReportOn, allowImport, allowWorkflow, allowEntityRelationship, tableId, primaryKey, keyField, userdefinedTable, subAccountIdField, elementId, isSystemView, relabelParam, this.Account.Id));
                            break;
                        case 2:
                            result.Add(new CustomTable(tableName, joinType, description, allowReportOn, allowImport, allowWorkflow, allowEntityRelationship, tableId, primaryKey, keyField, userdefinedTable, subAccountIdField, elementId, isSystemView, relabelParam, this.Account.Id));
                            break;
                    }
                }
            }
            return result;
        }
    }
}
