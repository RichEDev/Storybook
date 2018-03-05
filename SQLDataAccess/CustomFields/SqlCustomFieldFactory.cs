namespace SQLDataAccess.CustomFields
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic.CustomFields;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    using Common.Logging;

    /// <summary>
    ///     The sql custom field factory.
    /// </summary>
    public class SqlCustomFieldFactory : CustomFieldRepository
    {
        /// <summary>
        ///     The base sql.
        /// </summary>
        private const string BaseSql =
            "SELECT     fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, FieldCategory, RelatedTable, IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation FROM         dbo.[customFields] ";

        /// <summary>
        ///     The _customer data connection.
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        ///     The _field factory.
        /// </summary>
        private readonly FieldFactory _fieldFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCustomFieldFactory" /> class.
        /// </summary>
        /// <param name="customerDataConnection">The customer data connection.</param>
        /// <param name="fieldFactory">An instance of <see cref="FieldFactory">FieldFactory</see></param>
        /// <param name="customFieldListValuesRepository">An instance of <see cref="CustomFieldListValuesRepository">CustomFieldListValuesRepository</see></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlCustomFieldFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, FieldFactory fieldFactory, CustomFieldListValuesRepository customFieldListValuesRepository, ILog logger) : base(logger)
        {
            this._customerDataConnection = customerDataConnection;
            this._fieldFactory = fieldFactory;
            this._fieldFactory.FieldListValuesRepository = customFieldListValuesRepository;
        }

        /// <summary>
        ///     Gets a Field from the database by its Name
        /// </summary>
        /// <param name="name">
        ///     The field name to lookup.
        /// </param>
        /// <returns>
        ///     A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[string name]
        {
            get
            {
                this._customerDataConnection.Parameters.Clear();
                var where = string.Empty;
                if (!string.IsNullOrEmpty(name))
                {
                    this._customerDataConnection.Parameters.Add(new SqlParameter("@field", SqlDbType.NVarChar) { Value = name });
                    where = " WHERE field = @field";
                }

                var sql = $"{BaseSql} {where}";
                var result = this.Get(this._customerDataConnection, sql);

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        ///     Gets a Field from the database by its Id
        /// </summary>
        /// <param name="id">
        ///     The field name to lookup.
        /// </param>
        /// <returns>
        ///     A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[Guid id]
        {
            get
            {
                var field = base[id];

                if (field == null)
                {
                    field = this.Get(id);
                    this.Save(field);
                }

                return field;
            }
        }

        /// <summary>
        ///     Gets a Field from the database by its GUID
        /// </summary>
        /// <returns>
        ///     A <see cref="IField">IField</see>.
        /// </returns>
        private IField Get(Guid id)
        {
            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@field", SqlDbType.UniqueIdentifier) { Value = id });
            var where = " WHERE fieldid = @field";

            var sql = $"{BaseSql} {where}";
            var result = this.Get(this._customerDataConnection, sql);

            return result.FirstOrDefault();
        }

        /// <summary>
        ///     Gets a List of <see cref="IField">IField</see> from a database
        /// </summary>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        /// <param name="sql">
        ///     The sql to execute.
        /// </param>
        /// <returns>
        ///     A List of<see cref="IField"> IField</see>.
        /// </returns>
        private List<IField> Get(ICustomerDataConnection<SqlParameter> connection, string sql)
        {
            var result = new List<IField>();
            using (var reader = connection.GetReader(sql))
            {
                var fieldIdOrdId = reader.GetOrdinal("fieldid");
                var tableIdOrdId = reader.GetOrdinal("tableid");
                var fieldNameOrdId = reader.GetOrdinal("field");
                var fieldTypeOrdId = reader.GetOrdinal("fieldType");
                var descriptionOrdId = reader.GetOrdinal("description");
                var commentOrdId = reader.GetOrdinal("comment");
                var normalviewOrdId = reader.GetOrdinal("normalview");
                var idFieldOrdId = reader.GetOrdinal("idfield");
                var viewGroupIdOrdId = reader.GetOrdinal("viewgroupid");
                var genListOrdId = reader.GetOrdinal("genlist");
                var widthOrdId = reader.GetOrdinal("width");
                var canTotalOrdId = reader.GetOrdinal("cantotal");
                var printOutOrdId = reader.GetOrdinal("printout");
                var valueListOrdId = reader.GetOrdinal("valuelist");
                var allowImportOrdId = reader.GetOrdinal("allowimport");
                var mandatoryOrdId = reader.GetOrdinal("mandatory");
                var lookupTableOrdId = reader.GetOrdinal("lookuptable");
                var lookupFieldOrdId = reader.GetOrdinal("lookupfield");
                var useForLookupOrdId = reader.GetOrdinal("useforlookup");
                var workflowUpdateOrdId = reader.GetOrdinal("workflowUpdate");
                var workflowSearchableOrdId = reader.GetOrdinal("workflowSearch");
                var lengthOrdId = reader.GetOrdinal("length");
                var reLabelOrdId = reader.GetOrdinal("relabel");
                var reLabelParamOrdId = reader.GetOrdinal("relabel_param");
                var classPropertyNameOrdId = reader.GetOrdinal("classPropertyName");
                var relatedTableIdOrdId = reader.GetOrdinal("relatedTable");
                var isForeignKeyOrdId = reader.GetOrdinal("IsForeignKey");
                while (reader.Read())
                {
                    var fieldId = reader.GetGuid(fieldIdOrdId);
                    var tableId = reader.GetGuid(tableIdOrdId);
                    var fieldName = reader.GetString(fieldNameOrdId);
                    var fieldType = reader.GetString(fieldTypeOrdId);
                    var description = reader.IsDBNull(descriptionOrdId)
                                          ? string.Empty
                                          : reader.GetString(descriptionOrdId);
                    var comment = reader.IsDBNull(commentOrdId) ? string.Empty : reader.GetString(commentOrdId);
                    var normalView = reader.GetBoolean(normalviewOrdId);
                    var idField = reader.GetBoolean(idFieldOrdId);
                    var viewGroupId = reader.IsDBNull(viewGroupIdOrdId)
                                          ? Guid.Empty
                                          : reader.GetGuid(viewGroupIdOrdId);
                    var genList = reader.GetBoolean(genListOrdId);
                    var width = reader.GetInt32(widthOrdId);
                    var canTotal = reader.GetBoolean(canTotalOrdId);
                    var printOut = reader.GetBoolean(printOutOrdId);
                    var valueList = reader.GetBoolean(valueListOrdId);
                    var allowImport = reader.GetBoolean(allowImportOrdId);
                    var mandatory = reader.GetBoolean(mandatoryOrdId);
                    var lookupTableId = reader.IsDBNull(lookupTableOrdId)
                                            ? Guid.Empty
                                            : reader.GetGuid(lookupTableOrdId);
                    var lookUpFieldId = reader.IsDBNull(lookupFieldOrdId)
                                            ? Guid.Empty
                                            : reader.GetGuid(lookupFieldOrdId);
                    var useForLookup = reader.GetBoolean(useForLookupOrdId);
                    var workflowUpdate = reader.GetBoolean(workflowUpdateOrdId);
                    var workflowSearchable = reader.GetBoolean(workflowSearchableOrdId);
                    var length = reader.IsDBNull(lengthOrdId) ? 0 : reader.GetInt32(lengthOrdId);
                    var reLabel = reader.GetBoolean(reLabelOrdId);
                    var reLabelParam = reader.IsDBNull(reLabelParamOrdId)
                                           ? string.Empty
                                           : reader.GetString(reLabelParamOrdId);
                    var classPropertyName = reader.IsDBNull(classPropertyNameOrdId)
                                                ? string.Empty
                                                : reader.GetString(classPropertyNameOrdId);
                    var relatedTableId = reader.IsDBNull(relatedTableIdOrdId)
                                             ? Guid.Empty
                                             : reader.GetGuid(relatedTableIdOrdId);
                    var isForeignKey = reader.GetBoolean(isForeignKeyOrdId);

                    var field = this._fieldFactory.New<Field>(
                        fieldType, 
                        fieldId, 
                        fieldName, 
                        description, 
                        comment, 
                        tableId, 
                        classPropertyName, 
                        this._fieldFactory.PopulateFieldAttributes(
                            normalView, 
                            idField, 
                            genList, 
                            canTotal, 
                            printOut, 
                            useForLookup, 
                            lookUpFieldId, 
                            lookupTableId, 
                            allowImport, 
                            isForeignKey, 
                            relatedTableId, 
                            reLabel, 
                            reLabelParam, 
                            workflowSearchable, 
                            workflowUpdate, 
                            mandatory, 
                            FieldSource.Custom), 
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
    }
}