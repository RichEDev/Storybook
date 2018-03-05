namespace SQLDataAccess.CustomEntities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using System.Linq;
    using BusinessLogic.CustomEntities;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    using Common.Logging;

    /// <summary>
    /// The sql custom entity fields factory.
    /// </summary>
    public class SqlCustomEntityFieldsFactory : CustomEntityFieldRepository
    {
        /// <summary>
        /// The base sql.
        /// </summary>
        private const string BaseSql = "SELECT dbo.[customentityattributes].fieldid, dbo.[customentities].tableid, CASE dbo.[customentityattributes].display_name WHEN 'GreenLight Currency' THEN 'GreenLightCurrency' WHEN 'Created On' THEN Replace([customentityattributes].display_name, ' ', '') WHEN 'Created By' THEN Replace([customentityattributes].display_name, ' ', '') WHEN 'Modified On' THEN Replace([customentityattributes].display_name, ' ', '') WHEN 'Modified By' THEN Replace([customentityattributes].display_name, ' ', '') ELSE 'att'  + Cast(dbo.[customentityattributes].attributeid AS NVARCHAR(10))  END AS field,  case dbo.[customEntityAttributes].fieldtype when '17' THEN N'CL' ELSE dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format)  END AS fieldtype ,  dbo.[customentityattributes].display_name AS description,  dbo.[customentityattributes].display_name AS comment,  Cast(0 AS BIT)                            AS normalview, dbo.[customentityattributes].is_key_field AS idfield,  dbo.[customentities].tableid AS viewgroupid,  Cast(0 AS BIT)                            AS genlist, Cast(50 AS INT)                           AS width, Cast(0 AS BIT)                            AS cantotal, Cast(0 AS BIT)                            AS printout, CASE dbo.[customentityattributes].fieldtype WHEN 4 THEN Cast(1 AS BIT) ELSE Cast(0 AS BIT) END AS valuelist,  Cast(1 AS BIT)                            AS allowimport, dbo.[customentityattributes].mandatory,  dbo.[customentityattributes].modifiedon AS amendedon,  dbo.[customentityattributes].relatedtable AS lookuptable,  dbo.[tables].keyfield AS lookupfield,  Cast(0 AS BIT)                            AS useforlookup, Cast(1 AS BIT)                            AS workflowUpdate, Cast(1 AS BIT)                            AS workflowSearch, dbo.[customentityattributes].maxlength AS length,  Cast(0 AS BIT)                            AS Expr1, NULL                                      AS Expr2, Cast(1 AS INT)                            AS fieldFrom, Cast(0 AS BIT)                            AS allowDuplicateChecking, NULL                                      AS classPropertyName, Cast(0 AS TINYINT)                        AS FieldCategory, customEntityAttributes.relatedtable AS RelatedTable,  CASE(dbo.[customentityattributes].fieldtype ) WHEN '22' THEN Cast(1 AS BIT) WHEN '9' THEN CASE dbo.[customentityattributes].relationshiptype WHEN 1 THEN Cast(1 AS BIT) ELSE Cast(0 AS BIT) END END                                       AS IsForeignKey, NULL                                      AS associatedFieldForDuplicateChecking, NULL                                      AS DuplicateCheckingSource, NULL                                      AS DuplicateCheckingCalculation FROM dbo.[customentityattributes] INNER JOIN dbo.[customentities] ON dbo.[customentities].entityid = dbo.[customentityattributes].entityid LEFT JOIN tables ON tables.tableid = dbo.[customentityattributes].relatedtable UNION SELECT     fieldid, tableid, field, fieldtype, description, description AS comment, CAST(0 AS BIT) AS normalview, idfield, NULL AS viewgroupid, CAST(0 AS bit) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS bit) AS Expr1, CAST(0 AS BIT) AS Expr2, CAST(0 AS BIT) AS Expr3, CAST(0 AS BIT) AS Expr4, NULL AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, 0 AS length, CAST(0 AS BIT) AS Expr5, NULL AS Expr6, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, NULL AS RelatedTable, CAST(0 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation FROM         dbo.customEntityAttachmentFields";

        /// <summary>
        /// The _customer data connection.
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// The _field factory.
        /// </summary>
        private readonly FieldFactory _fieldFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCustomEntityFieldsFactory"/> class.
        /// </summary>
        /// <param name="customerDataConnection">
        /// The customer data connection.
        /// </param>
        /// <param name="fieldFactory">
        /// The field factory.
        /// </param>
        /// <param name="customEntityFieldListValuesRepository">
        /// The custom entity field list values repository.
        /// </param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlCustomEntityFieldsFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, FieldFactory fieldFactory, CustomEntityFieldListValuesRepository customEntityFieldListValuesRepository, ILog logger) : base(logger)
        {
            this._customerDataConnection = customerDataConnection;
            this._fieldFactory = fieldFactory;
            this._fieldFactory.FieldListValuesRepository = customEntityFieldListValuesRepository;
        }

        /// <summary>
        /// Gets a Field from the database by its Name
        /// </summary>
        /// <param name="name">
        /// The field name to search on.
        /// </param>
        /// <returns>
        /// A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[string name]
        {
            get
            {
                this._customerDataConnection.Parameters.Clear();
                this._customerDataConnection.Parameters.Add(new SqlParameter("@desc", SqlDbType.NVarChar) { Value = name });
                var where = " WHERE description = @desc";

                var sql = $"{BaseSql} {where}";
                var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a Field from the database by its GUID
        /// </summary>
        /// <param name="id">
        /// The field name to search on.
        /// </param>
        /// <returns>
        /// A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[Guid id]
        {
            get
            {
                IField field = base[id];

                if (field == null)
                {
                    field = this.Get(id);
                    this.Save(field);
                }

                return field;
            }
        }

        /// <summary>
        /// Gets a List of <see cref="IField">IField</see> from a database
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="sql">
        /// The sql to execute.
        /// </param>
        /// <returns>
        /// A List of<see cref="IField"> IField</see>.
        /// </returns>
        private List<IField> GetFieldsFromDb(ICustomerDataConnection<SqlParameter> connection, string sql)
        {
            List<IField> result = new List<IField>();
            using (var reader = connection.GetReader(sql))
            {
                int fieldIdOrdId = reader.GetOrdinal("fieldid");
                int tableIdOrdId = reader.GetOrdinal("tableid");
                int fieldNameOrdId = reader.GetOrdinal("field");
                int fieldTypeOrdId = reader.GetOrdinal("fieldType");
                int descriptionOrdId = reader.GetOrdinal("description");
                int commentOrdId = reader.GetOrdinal("comment");
                int normalviewOrdId = reader.GetOrdinal("normalview");
                int identityFieldOrd = reader.GetOrdinal("idfield");
                int viewGroupIdOrdId = reader.GetOrdinal("viewgroupid");
                int genListOrdId = reader.GetOrdinal("genlist");
                int widthOrdId = reader.GetOrdinal("width");
                int canTotalOrdId = reader.GetOrdinal("cantotal");
                int printOutOrdId = reader.GetOrdinal("printout");
                int valueListOrdId = reader.GetOrdinal("valuelist");
                int allowImportOrdId = reader.GetOrdinal("allowimport");
                int mandatoryOrdId = reader.GetOrdinal("mandatory");
                int lookupTableOrdId = reader.GetOrdinal("lookuptable");
                int lookupFieldOrdId = reader.GetOrdinal("lookupfield");
                int useForLookupOrdId = reader.GetOrdinal("useforlookup");
                int workflowUpdateOrdId = reader.GetOrdinal("workflowUpdate");
                int workflowSearchableOrdId = reader.GetOrdinal("workflowSearch");
                int lengthOrdId = reader.GetOrdinal("length");
                int relabelOrd = reader.GetOrdinal("relabel");
                int relabelParamOrdId = reader.GetOrdinal("relabel_param");
                int classPropertyNameOrdId = reader.GetOrdinal("classPropertyName");
                int relatedTableIdOrdId = reader.GetOrdinal("relatedTable");
                int foreignKeyOrdId = reader.GetOrdinal("IsForeignKey");
                while (reader.Read())
                {
                    var fieldId = reader.GetGuid(fieldIdOrdId);
                    var tableId = reader.GetGuid(tableIdOrdId);
                    var fieldName = reader.GetString(fieldNameOrdId);
                    var fieldType = reader.GetString(fieldTypeOrdId);
                    var description = reader.IsDBNull(descriptionOrdId) ? string.Empty : reader.GetString(descriptionOrdId);
                    var comment = reader.IsDBNull(commentOrdId) ? string.Empty : reader.GetString(commentOrdId);
                    var normalView = reader.GetBoolean(normalviewOrdId);
                    var identifierField = reader.GetBoolean(identityFieldOrd);
                    var viewGroupId = reader.IsDBNull(viewGroupIdOrdId) ? Guid.Empty : reader.GetGuid(viewGroupIdOrdId);
                    var genList = reader.GetBoolean(genListOrdId);
                    var width = reader.GetInt32(widthOrdId);
                    var canTotal = reader.GetBoolean(canTotalOrdId);
                    var printOut = reader.GetBoolean(printOutOrdId);
                    var valueList = reader.GetBoolean(valueListOrdId);
                    var allowImport = reader.GetBoolean(allowImportOrdId);
                    var mandatory = reader.GetBoolean(mandatoryOrdId);
                    var lookupTableId = reader.IsDBNull(lookupTableOrdId) ? Guid.Empty : reader.GetGuid(lookupTableOrdId);
                    var lookUpFieldId = reader.IsDBNull(lookupFieldOrdId) ? Guid.Empty : reader.GetGuid(lookupFieldOrdId);
                    var useForLookup = reader.GetBoolean(useForLookupOrdId);
                    var workflowUpdate = reader.GetBoolean(workflowUpdateOrdId);
                    var workflowSearchable = reader.GetBoolean(workflowSearchableOrdId);
                    var length = reader.IsDBNull(lengthOrdId) ? 0 : reader.GetInt32(lengthOrdId);
                    var relabel = reader.GetBoolean(relabelOrd);
                    var relabelParam = reader.IsDBNull(relabelParamOrdId) ? string.Empty : reader.GetString(relabelParamOrdId);
                    var classPropertyName = reader.IsDBNull(classPropertyNameOrdId) ? string.Empty : reader.GetString(classPropertyNameOrdId);
                    var relatedTableId = reader.IsDBNull(relatedTableIdOrdId) ? Guid.Empty : reader.GetGuid(relatedTableIdOrdId);
                    var foreignKey = !reader.IsDBNull(foreignKeyOrdId) && reader.GetBoolean(foreignKeyOrdId);

                    var field = this._fieldFactory.New<Field>(
                        fieldType,
                        fieldId,
                        fieldName,
                        description,
                        comment,
                        tableId,
                        classPropertyName,
                        this._fieldFactory.PopulateFieldAttributes(normalView, identifierField, genList, canTotal, printOut, useForLookup, lookUpFieldId, lookupTableId, allowImport, foreignKey, relatedTableId, relabel, relabelParam, workflowSearchable, workflowUpdate, mandatory, FieldSource.CustomEntity),
                        viewGroupId,
                        width,
                        length,
                        valueList);

                    if (field != null)
                    {
                        result.Add(field);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a Field from the database by its Id
        /// </summary>
        /// <param name="id">
        /// The field name to search on.
        /// </param>
        /// <returns>
        /// A <see cref="IField">IField</see>.
        /// </returns>
        private IField Get(Guid id)
        {
            this._customerDataConnection.Parameters.Clear();
            var where = string.Empty;
            if (id != Guid.Empty)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@fieldId", SqlDbType.UniqueIdentifier) { Value = id });
                where = " WHERE fieldId = @fieldId";
            }

            var sql = $"{BaseSql} {where}";
            var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

            return result.FirstOrDefault();
        }
    }
}
