namespace SQLDataAccess.UserDefinedFieldValues
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using CacheDataAccess.UserDefinedFields;

    using Common.Logging;

    /// <summary>
    ///     The sql user defined field factory.
    /// </summary>
    public class SqlUserDefinedFieldFactory : CacheUserDefinedFieldFactory
    {
        /// <summary>
        ///     The customer data connection.
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        ///     The base sql query.
        /// </summary>
        private readonly string baseSql =
            "SELECT  fieldid, tableid, 'udf' + CAST(userdefineid AS NVARCHAR(10)) AS field, dbo.getFieldType(fieldtype, format) AS fieldtype, display_name AS description, display_name AS comment, CAST(1 AS BIT) AS normalview, CAST(0 AS BIT) AS idfield, dbo.getUserdefinedViewGroup(tableid, fieldtype) AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, CASE userdefined.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, mandatory, ModifiedOn AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL AS Expr2, CAST(2 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, relatedTable AS RelatedTable, CASE fieldtype WHEN 9 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation, userdefineid, attribute_name, description, tooltip, [Order],[Format],defaultvalue, precision, hyperlinkPath, hyperlinkText, displayField, maxRows FROM dbo.userdefined";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUserDefinedFieldFactory"/> class.
        /// </summary>
        /// <param name="customerDataConnection">The customer data connection. <see cref="CustomerDatabaseConnection"/></param>
        /// <param name="account">The account. <see cref="IAccount"/></param>
        /// <param name="cache">The cache. <see cref="ICache{T, TK}"/></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlUserDefinedFieldFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, IAccount account, ICache<IField, Guid> cache, ILog logger)
            : base(account, cache, logger)
        {
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(account, nameof(account));
            Guard.ThrowIfNull(cache, nameof(cache));

            this._customerDataConnection = customerDataConnection;
        }

        /// <summary>
        /// Gets an instance of <see cref="IField"/> with a matching ID from memory if possible.
        /// </summary>
        /// <param name="id">
        /// The ID of the <see cref="IField"/> you want to retrieve
        /// </param>
        /// <returns>
        /// The required <see cref="IField"/> or null if it cannot be found
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
        /// Gets an instance of <see cref="IField"/> with a matching name from memory if possible.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="IField"/> you want to retrieve
        /// </param>
        /// <returns>
        /// The required <see cref="IField"/> or null if it cannot be found
        /// </returns>
        public override IField this[string name] => this.Get(name);

        /// <summary>
        /// Get a <see cref="List{IField}"/> for the given 
        /// <seealso cref="ITable"/>
        /// </summary>
        /// <param name="table">
        /// the <see cref="ITable"/> to get <seealso cref="IField"/> for
        /// </param>
        /// <returns>A <see cref="List{IField}"/> owned by this <seealso cref="ITable"/></returns>
        public override List<IField> this[ITable table] => this.GetFromDbByTableID(table.Id);

        /// <summary>
        /// Get a single <see cref="IField"/> from the Database by it's name.
        /// </summary>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        /// <returns>
        /// The <see cref="IField"/>.
        /// </returns>
        private IField Get(string name)
        {
            this._customerDataConnection.Parameters.Clear();
            var where = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@field", SqlDbType.NVarChar) { Value = name });
                where = " WHERE field = @field";
            }

            var sql = $"{this.baseSql} {where}";
            var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// The get a single <see cref="IField"/> by it's ID.
        /// </summary>
        /// <param name="id">
        /// The id. <see cref="Guid"/> of the 
        /// <seealso cref="IField"/>
        /// </param>
        /// <returns>
        /// The <see cref="IField"/>.
        /// </returns>
        private IField Get(Guid id)
        {
            return this.GetFromDb(id).FirstOrDefault();
        }

        /// <summary>
        /// The get fields from db.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private List<IField> GetFieldsFromDb(ICustomerDataConnection<SqlParameter> connection, string sql)
        {
            Field decoratedfield = null;
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
                var classPropertyNameOrdId = reader.GetOrdinal("classPropertyName");
                var relatedTableIdOrdId = reader.GetOrdinal("relatedTable");
                var isForeignKeyOrdId = reader.GetOrdinal("IsForeignKey");

                var userdefinedFieldIdOrd = reader.GetOrdinal("userdefineid");
                var labelOrd = reader.GetOrdinal("attribute_name");
                var userDefinedDescriptionOrd = reader.GetOrdinal("description");
                var tooltipOrd = reader.GetOrdinal("tooltip");
                var orderOrd = reader.GetOrdinal("order");
                var formatOrd = reader.GetOrdinal("format");
                var defaultValueOrd = reader.GetOrdinal("defaultvalue");
                var precisionOrd = reader.GetOrdinal("precision");
                var hyperLinkPathOrd = reader.GetOrdinal("hyperlinkpath");
                var hyperLinkTextOrd = reader.GetOrdinal("hyperlinktext");
                var displayFieldOrd = reader.GetOrdinal("displayfield");
                var maxRowsOrd = reader.GetOrdinal("maxrows");

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
                    var classPropertyName = reader.IsDBNull(classPropertyNameOrdId) ? string.Empty : reader.GetString(classPropertyNameOrdId);
                    var relatedTableId = reader.IsDBNull(relatedTableIdOrdId) ? Guid.Empty : reader.GetGuid(relatedTableIdOrdId);
                    var isForeignKey = reader.GetBoolean(isForeignKeyOrdId);
                    var userDefinedFieldId = reader.GetInt32(userdefinedFieldIdOrd);
                    var label = reader.GetString(labelOrd);
                    var userDefinedDescription = reader.GetString(userDefinedDescriptionOrd);
                    var tooltip = reader.GetString(tooltipOrd);
                    var order = reader.GetInt32(orderOrd);
                    var format = reader.IsDBNull(formatOrd) ? 0 : reader.GetByte(formatOrd);
                    var defaultValue = reader.GetValue(defaultValueOrd);
                    var precision = reader.IsDBNull(precisionOrd) ? 0 : reader.GetByte(precisionOrd);
                    var hyperLinkPath = reader.IsDBNull(hyperLinkPathOrd) ? string.Empty : reader.GetString(hyperLinkPathOrd);
                    var hyperLinkText = reader.IsDBNull(hyperLinkTextOrd) ? string.Empty : reader.GetString(hyperLinkTextOrd);
                    var displayField = reader.IsDBNull(displayFieldOrd) ? (Guid?)null : reader.GetGuid(displayFieldOrd);
                    var maxRows = reader.IsDBNull(maxRowsOrd) ? 0 : reader.GetInt32(maxRowsOrd);

                    var fieldAttributes = new FieldAttributes();

                    if (normalView)
                    {
                        fieldAttributes.Add(new NormalViewAttribute());
                    }

                    if (idField)
                    {
                        fieldAttributes.Add(new IdFieldAttribute());
                    }

                    if (genList)
                    {
                        fieldAttributes.Add(new GenListAttribute());
                    }

                    if (canTotal)
                    {
                        fieldAttributes.Add(new CanTotalAttribute());
                    }

                    if (printOut)
                    {
                        fieldAttributes.Add(new PrintOutAttribute());
                    }

                    if (useForLookup)
                    {
                        fieldAttributes.Add(new UseForLookupAttribute(lookUpFieldId, lookupTableId));
                    }

                    if (allowImport)
                    {
                        fieldAttributes.Add(new AllowImportAttribute());
                    }

                    if (isForeignKey)
                    {
                        fieldAttributes.Add(new ForeignKeyAttribute(relatedTableId));
                    }

                    if (workflowSearchable)
                    {
                        fieldAttributes.Add(new WorkflowSearchAttribute());
                    }

                    if (workflowUpdate)
                    {
                        fieldAttributes.Add(new WorkflowUpdateAttribute());
                    }

                    if (mandatory)
                    {
                        fieldAttributes.Add(new MandatoryAttribute());
                    }

                    Field field = new Field(fieldId, fieldName, description, comment, tableId, classPropertyName, fieldAttributes, viewGroupId, width, length);

                    switch (fieldType)
                    {
                        case "A":
                            decoratedfield = new AmountField(new NumericField(field));
                            break;
                        case "C":
                            decoratedfield = new CurrencyField(new NumericField(field));
                            break;
                        case "FC":
                            decoratedfield = new FunctionCurrency(new CurrencyField(new NumericField(field)));
                            break;
                        case "M":
                            decoratedfield = new MoneyField(new NumericField(field));
                            break;
                        case "D":
                            decoratedfield = new DateField(new DateTimeField(field));
                            break;
                        case "DT":
                            decoratedfield = new DateTimeField(new DateTimeField(field));
                            break;
                        case "T":
                            decoratedfield = new TimeField(new DateTimeField(new DateTimeField(field)));
                            break;
                        case "F":
                            decoratedfield = new FloatField(new DecimalField(field));
                            break;
                        case "FD":
                            decoratedfield = new FunctionDecimalField(new DecimalField(field));
                            break;
                        case "FI":
                            decoratedfield = new FunctionIntegerField(new IntegerField(field));
                            break;
                        case "I":
                            decoratedfield = new IntegerField(field);
                            break;
                        case "N":
                            decoratedfield = new NumberField(new IntegerField(field));
                            break;
                        case "S":
                            decoratedfield = new StringField(field);
                            break;
                        case "FS":
                            decoratedfield = new FunctionStringField(new StringField(field));
                            break;
                        case "LT":
                            decoratedfield = new LargeText(new StringField(field));
                            break;
                        case "FU":
                            decoratedfield = new FunctionUnique(new GuidField(field));
                            break;
                        case "G":
                            decoratedfield = new GuidField(field);
                            break;
                        case "U":
                            decoratedfield = new UniqueField(new GuidField(field));
                            break;
                        case "X":
                        case "Y":
                            decoratedfield = new BooleanField(field);
                            break;
                        case "B":
                            decoratedfield = new LongField(field);
                            break;
                        case "VB":
                            decoratedfield = new VarBinaryField(field);
                            break;
                    }
                   
                    if (valueList && decoratedfield != null)
                    {
                        switch (fieldType)
                        {
                            case "X":
                            case "Y":
                                decoratedfield = (BooleanFieldValueList)decoratedfield;
                                break;
                            case "U":
                            case "G":
                            case "FU":
                                decoratedfield = (GuidFieldValueList)decoratedfield;
                                break;
                            case "I":
                                decoratedfield = (IntegerFieldValueList)decoratedfield;
                                break;
                            case "S":
                                decoratedfield = (StringFieldValueList)decoratedfield;
                                break;
                        }
                    }

                    if (decoratedfield != null)
                    {
                        // Denotes this is a user defined field
                        field.FieldAttributes.Add(new UserdefinedAttribute(userDefinedFieldId, label, userDefinedDescription, tooltip, order, format, defaultValue, precision, hyperLinkText, hyperLinkPath, displayField, maxRows));

                        result.Add(decoratedfield);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get from db.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private List<IField> GetFromDb(Guid id)
        {
            this._customerDataConnection.Parameters.Clear();
            var where = string.Empty;
            if (id != Guid.Empty)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@fieldid", SqlDbType.UniqueIdentifier) { Value = id });
                where = " WHERE fieldid = @fieldid";
            }

            var sql = $"{this.baseSql} {where}";
            var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

            return result;
        }

        private List<IField> GetFromDbByTableID(Guid id)
        {
            this._customerDataConnection.Parameters.Clear();
            var where = string.Empty;
            if (id != Guid.Empty)
            {
                this._customerDataConnection.Parameters.Add(new SqlParameter("@tableId", SqlDbType.UniqueIdentifier) { Value = id });
                where = " WHERE tableId = @tableId";
            }

            var sql = $"{this.baseSql} {where}";
            var result = this.GetFieldsFromDb(this._customerDataConnection, sql);

            return result;
        }
    }
}