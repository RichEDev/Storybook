namespace SQLDataAccess.Fields
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    using CacheDataAccess.Fields;

    using Common.Logging;

    /// <summary>
    /// The sql field factory.
    /// </summary>
    public class SqlFieldFactory : CacheFieldFactory
    {
        /// <summary>
        /// The base sql.
        /// </summary>
        private const string BaseSql =
            "SELECT fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, allowDuplicateChecking,classPropertyName, RelatedTable, IsForeignKey, associatedFieldForDuplicateChecking, DuplicateCheckingSource, DuplicateCheckingCalculation FROM fields_base";

        /// <summary>
        /// The _customer data connection.
        /// </summary>
        private readonly IMetabaseDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// The _field factory.
        /// </summary>
        private readonly FieldFactory _fieldFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFieldFactory"/> class.
        /// </summary>
        /// <param name="customerDataConnection">The customer data connection.</param>
        /// <param name="account">The account.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="fieldFactory">An instance of <see cref="FieldFactory"/>.</param>
        /// <param name="fieldListValuesRepository">An instance of <see cref="FieldListValuesRepository"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlFieldFactory(IMetabaseDataConnection<SqlParameter> customerDataConnection, IAccount account, ICache<IField, Guid> cache, FieldFactory fieldFactory, FieldListValuesRepository fieldListValuesRepository, ILog logger)
            : base(account, cache, logger)
        {
            this._customerDataConnection = customerDataConnection;
            
            this._fieldFactory = fieldFactory;
            this._fieldFactory.FieldListValuesRepository = fieldListValuesRepository;
        }

        /// <summary>
        /// Gets an instance of <see cref="IField"/> with a matching ID from memory if possible, if not it will search cache for an entry, and add to memory
        /// </summary>
        /// <param name="id">The ID of the <see cref="IField"/> you want to retrieve</param>
        /// <returns>The required <see cref="IField"/> or null if it cannot be found</returns>
        public override IField this[Guid id]
        {
            get
            {
                IField field = base[id];

                if (field == null)
                {
                    field = this.Get(id).FirstOrDefault();
                    this.Save(field);
                }

                return field;
            }
        }

        /// <summary>
        /// Gets a List of <see cref="IField">IField</see> based on its Id
        /// </summary>
        /// <param name="id">
        /// The field Id
        /// </param>
        /// <returns>
        /// The List of <see cref="IField">IField</see>
        /// </returns>
        private List<IField> Get(Guid id)
        {
            this._customerDataConnection.Parameters.Clear();
            var where = string.Empty;
            if (id != Guid.Empty)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@fieldid", SqlDbType.UniqueIdentifier) { Value = id });
                where = " WHERE fieldid = @fieldid";
            }
            var sql = $"{BaseSql} {where}";
            var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

            return result;
        }

        /// <summary>
        /// Gets a List of <see cref="IField">IField</see> from the database
        /// </summary>
        /// <param name="connection">
        /// The metabase connection.
        /// </param>
        /// <param name="sql">
        /// The SQL to execute
        /// </param>
        /// <returns>
        /// The List of <see cref="IField">IField</see>
        /// </returns>
        private List<IField> GetFieldsFromDb(IMetabaseDataConnection<SqlParameter> connection, string sql)
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
                int idFieldOrdId = reader.GetOrdinal("idfield");
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
                int reLabelOrdId = reader.GetOrdinal("relabel");
                int reLabelParamOrdId = reader.GetOrdinal("relabel_param");
                int classPropertyNameOrdId = reader.GetOrdinal("classPropertyName");
                int relatedTableIdOrdId = reader.GetOrdinal("relatedTable");
                int isForeignKeyOrdId = reader.GetOrdinal("IsForeignKey");
                while (reader.Read())
                {
                    var fieldId = reader.GetGuid(fieldIdOrdId);
                    var tableId = reader.GetGuid(tableIdOrdId);
                    var fieldName = reader.GetString(fieldNameOrdId);
                    var fieldType = reader.GetString(fieldTypeOrdId);
                    var description = reader.IsDBNull(descriptionOrdId) ? string.Empty : reader.GetString(descriptionOrdId);
                    var comment = reader.IsDBNull(commentOrdId) ? string.Empty : reader.GetString(commentOrdId);
                    var normalView = reader.GetBoolean(normalviewOrdId);
                    var idField = reader.GetBoolean(idFieldOrdId);
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
                    var reLabel = reader.GetBoolean(reLabelOrdId);
                    var reLabelParam = reader.IsDBNull(reLabelParamOrdId) ? string.Empty : reader.GetString(reLabelParamOrdId);
                    var classPropertyName = reader.IsDBNull(classPropertyNameOrdId) ? string.Empty : reader.GetString(classPropertyNameOrdId);
                    var relatedTableId = reader.IsDBNull(relatedTableIdOrdId) ? Guid.Empty : reader.GetGuid(relatedTableIdOrdId);
                    var isForeignKey = reader.GetBoolean(isForeignKeyOrdId);

                    var field = this._fieldFactory.New<Field>(
                        fieldType,
                        fieldId,
                        fieldName,
                        description,
                        comment,
                        tableId,
                        classPropertyName,
                        this._fieldFactory.PopulateFieldAttributes(normalView, idField, genList, canTotal, printOut, useForLookup, lookUpFieldId, lookupTableId, allowImport, isForeignKey, relatedTableId, reLabel, reLabelParam, workflowSearchable, workflowUpdate, mandatory, FieldSource.Field),
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
