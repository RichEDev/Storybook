namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Web;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.UserDefinedFields;

    /// <summary>
    /// The user defined fields base handling class.  This reads from the database returning a list of <seealso cref="cUserDefinedField"/>.
    /// </summary>
    public abstract class cUserDefinedFieldsBase
    {
        /// <summary>
        /// An instance of <see cref="ConcurrentDictionary{TKey,TValue}"/> containing a, <seealso cref="int"/> (the User defined field ID)
        /// </summary>
        protected static ConcurrentDictionary<int, SortedList<int, cUserDefinedField>> AllUserDefinedFields = new ConcurrentDictionary<int, SortedList<int, cUserDefinedField>>();

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Gets or sets the metabase connection string.
        /// </summary>
        public string MetabaseConnectionString { get; set; }

        /// <summary>
        /// An instance of <see cref="cUserDefinedFieldsBase"/>.
        /// </summary>
        private cUserDefinedFieldsBase _clsUserDefinedFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="cUserDefinedFieldsBase"/> class.
        /// </summary>
        /// <param name="accountId">
        /// The current account id.
        /// </param>
        protected cUserDefinedFieldsBase(int accountId)
        {
            this._connectionString = cAccounts.getConnectionString(accountId);
        }

        #region properties

        /// <summary>
        /// Gets or sets the Account ID used in class
        /// </summary>
        public int AccountID { get; protected set; }

        /// <summary>
        /// Gets or sets the validation group currently in use
        /// </summary>
        public string CurrentValidationGroup { get; set; }

        /// <summary>
        /// Gets or sets a complete list of all userdefined fields for this account
        /// </summary>
        public SortedList<int, cUserDefinedField> UserdefinedFields { get; protected set; }

        #endregion

        /// <summary>
        /// Get the collection of <see cref="cUserDefinedField"/> for the current Account.
        /// </summary>
        /// <param name="clsTables">
        /// An instance of <see cref="cTables"/>.
        /// </param>
        /// <param name="clsGroupings">
        /// An instance of <see cref="cUserDefinedFieldGroupingsBase"/>
        /// </param>
        /// <returns>
        /// A <see cref="SortedList{TKey,TValue}"/> of <seealso cref="int"/> which is the Userdefined Field ID and an instance of <seealso cref="cUserDefinedField"/>.
        /// </returns>
        /// <exception cref="Exception">Invalid SQL exception.
        /// </exception>
        protected SortedList<int, cUserDefinedField> GetCollection(cTables clsTables, cUserDefinedFieldGroupingsBase clsGroupings)
        {
            SortedList<int, cUserDefinedField> list;
            DateTime lastUpdated;
            using (var sqlConnection = new SqlConnection(DatabaseConnection.GetConnectionStringWithDecryptedPassword(this._connectionString)))
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    SortedList<int, SortedList<int, cListAttributeElement>> lstitems = this.GetAttributeListItems(transaction);
                    int maxorder = 1;
                    list = new SortedList<int, cUserDefinedField>();

                    SortedList<int, List<int>> lstSubcats = GetSubcats(transaction);
                    Dictionary<int, List<Guid>> lstMatchFields = this.GetMatchFields(transaction);

                    cTable relationshipTextBoxRelatedtable = null;
                    int maxRows = 15;

                    const string Sql = "SELECT GETDATE() SELECT userdefineid, attribute_name, display_name, fieldtype, tableid, specific, mandatory, description, [order], CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, tooltip, maxlength, format, defaultvalue, fieldid, groupID, archived, [precision], allowSearch, hyperlinkText, hyperlinkPath, relatedTable, displayField, maxRows, allowEmployeeToPopulate, encrypted FROM dbo.userdefined ORDER BY tableid, [order]";
                    const string RecordsetError = "The SQL should return the date before the user defined fields.";

                    using (var getUdfsCommand = new SqlCommand(Sql, sqlConnection, transaction))
                    using (var reader = getUdfsCommand.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception(RecordsetError);
                        }

                        lastUpdated = reader.GetDateTime(0);
                        if (!reader.NextResult())
                        {
                            throw new Exception(RecordsetError);
                        }

                        var tableIdOrd = reader.GetOrdinal("tableid");
                        var userdefineIdOrd = reader.GetOrdinal("userdefineid");
                        var attributeNameOrd = reader.GetOrdinal("attribute_name");
                        var displayNameOrd = reader.GetOrdinal("display_name");
                        var descriptionOrd = reader.GetOrdinal("description");
                        var fieldTypeOrd = reader.GetOrdinal("fieldtype");
                        var fieldIdOrd = reader.GetOrdinal("fieldid");
                        var specificOrd = reader.GetOrdinal("specific");
                        var mandatoryOrd = reader.GetOrdinal("mandatory");
                        var createdOnOrd = reader.GetOrdinal("createdon");
                        var createdByOrd = reader.GetOrdinal("createdby");
                        var modifiedOnOrd = reader.GetOrdinal("modifiedon");
                        var modifiedByOrd = reader.GetOrdinal("modifiedby");
                        var tooltipOrd = reader.GetOrdinal("tooltip");
                        var archivedOrd = reader.GetOrdinal("archived");
                        var encryptedOrd = reader.GetOrdinal("encrypted");
                        var groupIdOrd = reader.GetOrdinal("groupID");
                        var maxLengthOrd = reader.GetOrdinal("maxlength");
                        var formatOrd = reader.GetOrdinal("format");
                        var precisionOrd = reader.GetOrdinal("precision");
                        var defaultValueOrd = reader.GetOrdinal("defaultvalue");
                        var relatedTableOrd = reader.GetOrdinal("relatedtable");
                        var displayFieldOrd = reader.GetOrdinal("displayField");
                        var maxRowsOrd = reader.GetOrdinal("maxRows");
                        var hyperLinkTextOrd = reader.GetOrdinal("hyperlinkText");
                        var hyperLinkPathOrd = reader.GetOrdinal("hyperlinkPath");
                        var allowSearchOrd = reader.GetOrdinal("allowsearch");
                        var allowEmployeeToPopulateOrd = reader.GetOrdinal("allowEmployeeToPopulate");

                        while (reader.Read())
                        {
                            cAttribute attribute = null;
                            cTable table = clsTables.GetTableByID(reader.GetGuid(tableIdOrd));
                            int userdefineid = reader.GetInt32(userdefineIdOrd);
                            string attributename = reader.GetString(attributeNameOrd);
                            string displayname = reader.GetString(displayNameOrd);
                            string description = reader.IsDBNull(descriptionOrd) == false ? reader.GetString(descriptionOrd) : string.Empty;
                            FieldType fieldtype = (FieldType)reader.GetByte(fieldTypeOrd);
                            Guid fieldid = reader.GetGuid(fieldIdOrd);
                            bool specific = reader.GetBoolean(specificOrd);
                            bool mandatory = reader.GetBoolean(mandatoryOrd);
                            int order = maxorder;
                            DateTime createdon = reader.IsDBNull(createdOnOrd) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdOnOrd);
                            int createdby = reader.IsDBNull(createdByOrd) == true ? 0 : reader.GetInt32(createdByOrd);
                            DateTime modifiedon = reader.IsDBNull(modifiedOnOrd) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(modifiedOnOrd);
                            int modifiedby = reader.IsDBNull(modifiedByOrd) == true ? 0 : reader.GetInt32(modifiedByOrd);
                            string tooltip = reader.IsDBNull(tooltipOrd) == true ? string.Empty : reader.GetString(tooltipOrd).Replace("'", "\\'").Replace("\\\\'", "\\'").Replace("\"", "&quot;");
                            bool archived = reader.GetBoolean(archivedOrd);
                            var encrypted = reader.GetBoolean(encryptedOrd);
                            cUserdefinedFieldGrouping grouping = reader.IsDBNull(groupIdOrd) == true ? null : clsGroupings.GetGroupingByID(reader.GetInt32(groupIdOrd));

                            AttributeFormat format;
                            switch (fieldtype)
                            {
                                case FieldType.Text:
                                case FieldType.LargeText:
                                    int? maxlength;
                                    if (reader.IsDBNull(maxLengthOrd))
                                    {
                                        maxlength = null;
                                    }
                                    else
                                    {
                                        maxlength = reader.GetInt32(maxLengthOrd);
                                    }

                                    format = (AttributeFormat)reader.GetByte(formatOrd);
                                    attribute = new cTextAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, maxlength, format, fieldid, false, false, false, false, false, false, false, false, encrypted);
                                    break;
                                case FieldType.Integer:
                                    attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 0, fieldid, false, false, false, false, false, false, false, false);
                                    break;
                                case FieldType.Number:
                                    byte precision = reader.IsDBNull(precisionOrd) ? (byte)0 : reader.GetByte(precisionOrd);
                                    attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, precision, fieldid, false, false, false, false, false, false, false, false);
                                    break;
                                case FieldType.Currency:
                                    attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 2, fieldid, false, false, false, false, false, false, false, false);
                                    break;
                                case FieldType.DateTime:
                                    format = (AttributeFormat)reader.GetByte(formatOrd);
                                    attribute = new cDateTimeAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, format, fieldid, false, false, false, false, false, false, false);
                                    break;
                                case FieldType.TickBox:
                                    string defaultvalue = reader.IsDBNull(defaultValueOrd) == true ? string.Empty : reader.GetString(defaultValueOrd);
                                    attribute = new cTickboxAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, defaultvalue, fieldid, false, false, false, false, false, false, false);
                                    break;
                                case FieldType.Relationship:
                                    var relatedtableid = reader.GetGuid(relatedTableOrd);
                                    cTable relatedtable = clsTables.GetTableByID(relatedtableid);
                                    Guid displayField = Guid.Empty;
                                    if (!reader.IsDBNull(displayFieldOrd))
                                    {
                                        displayField = reader.GetGuid(displayFieldOrd);
                                    }

                                    if (!reader.IsDBNull(maxRowsOrd))
                                    {
                                        maxRows = reader.GetInt32(maxRowsOrd);
                                    }

                                    List<Guid> matchFields = null;
                                    if (lstMatchFields.ContainsKey(userdefineid))
                                    {
                                        matchFields = lstMatchFields[userdefineid];
                                    }

                                    attribute = new cManyToOneRelationship(userdefineid, attributename, displayname, description, tooltip, mandatory, false, createdon, createdby, modifiedon, modifiedby, relatedtable, fieldid, false, false, false, null, displayField, matchFields, maxRows, new List<Guid>(), new SortedList<int, FieldFilter>(), false, null);
                                    break;
                                case FieldType.List:
                                    SortedList<int, cListAttributeElement> items;
                                    lstitems.TryGetValue(userdefineid, out items);
                                    if (items == null)
                                    {
                                        items = new SortedList<int, cListAttributeElement>();
                                    }

                                    format = AttributeFormat.ListStandard; // list wide / standard format not implemented in udfs yet
                                    attribute = new cListAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, items, fieldid, false, false, format, false, false, false, false, false);
                                    break;
                                case FieldType.DynamicHyperlink:
                                case FieldType.Hyperlink:
                                    string hyperlinkText;
                                    hyperlinkText = reader.IsDBNull(hyperLinkTextOrd) == true ? string.Empty : reader.GetString(hyperLinkTextOrd);
                                    string hyperlinkPath;
                                    hyperlinkPath = reader.IsDBNull(hyperLinkPathOrd) == true ? string.Empty : reader.GetString(hyperLinkPathOrd);
                                    attribute = new cHyperlinkAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, hyperlinkText, hyperlinkPath, false, false, false, false, false);
                                    break;
                                case FieldType.RelationshipTextbox:
                                    if (reader.IsDBNull(relatedTableOrd) == false)
                                    {
                                        Guid relationshipTextBoxRelatedtableId = reader.GetGuid(relatedTableOrd);
                                        relationshipTextBoxRelatedtable = clsTables.GetTableByID(relationshipTextBoxRelatedtableId);
                                    }

                                    attribute = new cRelationshipTextBoxAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, relationshipTextBoxRelatedtable, fieldid, false, false, false, false, false, false);
                                    break;
                            }

                            List<int> subcats;
                            lstSubcats.TryGetValue(userdefineid, out subcats);
                            if (subcats == null)
                            {
                                subcats = new List<int>();
                            }
                            bool allowSearch = reader.GetBoolean(allowSearchOrd);
                            bool allowEmployeeToPopulate = reader.GetBoolean(allowEmployeeToPopulateOrd);

                            cUserDefinedField requserdef = new cUserDefinedField(userdefineid, table, order, subcats, createdon, createdby, modifiedon, modifiedby, attribute, grouping, archived, specific, allowSearch, allowEmployeeToPopulate, encrypted);

                            if (this.AlreadyExists(requserdef) == false)
                            {
                                list.Add(userdefineid, requserdef);
                            }

                            maxorder++;
                        }
                    }

                    transaction.Commit();
                }
            }

            lastReadFromDatabaseTicks.AddOrUpdate(
                this.AccountID,
                addValueFactory: accId => lastUpdated.Ticks,
                updateValueFactory: (accId, old) => lastUpdated.Ticks);
            return list;
        }

        /// <summary>
        /// Get a List of Field ID's that are used for match fields in a type-ahead control..
        /// </summary>
        /// <param name="transaction">
        /// the current <see cref="SqlTransaction"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> of <seealso cref="int"/> (the user defined field ID) and <seealso cref="Guid"/> (the field id).
        /// </returns>
        private Dictionary<int, List<Guid>> GetMatchFields(SqlTransaction transaction)
        {
            Dictionary<int, List<Guid>> retVals = new Dictionary<int, List<Guid>>();
            List<Guid> tmp = null;
            int udfID;
            Guid fieldID;

            const string Sql = "select userdefineid, fieldId from userdefinedMatchFields";
            using (var getMatchFieldsCommand = new SqlCommand(Sql, transaction.Connection, transaction))
            using (SqlDataReader reader = getMatchFieldsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    udfID = reader.GetInt32(0);
                    fieldID = reader.GetGuid(1);

                    if (!retVals.ContainsKey(udfID))
                    {
                        retVals.Add(udfID, new List<Guid>());
                    }

                    tmp = retVals[udfID];

                    if (!tmp.Contains(fieldID))
                    {
                        tmp.Add(fieldID);
                    }

                    retVals[udfID] = tmp;
                }

                reader.Close();
            }

            return retVals;
        }

        /// <summary>
        /// Gets the list items for all fields of type "List"
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>a <see cref="SortedList{TKey,TValue}"/></returns>
        protected SortedList<int, SortedList<int, cListAttributeElement>> GetAttributeListItems(SqlTransaction transaction)
        {
            const string Strsql = "select valueid, userdefineid, item, [order], archived from userdefined_list_items";
            var lst = new SortedList<int, SortedList<int, cListAttributeElement>>();
            using (var getAttributeListItemsCommand = new SqlCommand(Strsql, transaction.Connection, transaction))
            using (SqlDataReader reader = getAttributeListItemsCommand.ExecuteReader())
            {
                var valueidOrd = reader.GetOrdinal("valueid");
                var userdefineidOrd = reader.GetOrdinal("userdefineid");
                var itemOrd = reader.GetOrdinal("item");
                var orderOrd = reader.GetOrdinal("order");
                var archivedOrd = reader.GetOrdinal("archived");
                while (reader.Read())
                {
                    int valueid = reader.GetInt32(valueidOrd);
                    int userdefineid = reader.GetInt32(userdefineidOrd);
                    string item = reader.GetString(itemOrd);
                    int order = reader.GetInt32(orderOrd);
                    var archived = reader.GetBoolean(archivedOrd);

                    SortedList<int, cListAttributeElement> items;
                    lst.TryGetValue(userdefineid, out items);
                    if (items == null)
                    {
                        items = new SortedList<int, cListAttributeElement>();
                        lst.Add(userdefineid, items);
                    }

                    var listelement = new cListAttributeElement(valueid, item, order, archived);
                    if (!items.Keys.Contains(valueid))
                    {
                        items.Add(valueid, listelement);
                    }
                }
                reader.Close();
            }
            return lst;
        }

        protected static SortedList<int, List<int>> GetSubcats(SqlTransaction transaction)
        {
            SortedList<int, List<int>> lst = new SortedList<int, List<int>>();

            SqlDataReader reader;

            const string strsql = "select subcatid, userdefineid from subcats_userdefined";
            using (var getSubCatsCommand = new SqlCommand(strsql, transaction.Connection, transaction))
            using (reader = getSubCatsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int subcatid = reader.GetInt32(0);
                    int userdefineid = reader.GetInt32(1);
                    List<int> subcats;
                    lst.TryGetValue(userdefineid, out subcats);
                    if (subcats == null)
                    {
                        subcats = new List<int>();
                        lst.Add(userdefineid, subcats);
                    }
                    subcats.Add(subcatid);
                }
                reader.Close();
            }

            return lst;
        }

        public string GetGrid()
        {
            return "select userdefineid, display_name, description, fieldtype, mandatory, userdefinedInformation.AppliesTo from userdefined"; // dbo.getTableDescription(tableid)
        }

        public const string LatestUpdateKey = "userdefinedfieldslatest";

        /// <summary>
        /// The ticks representing the last time the user defined fields were updated from the database.
        /// </summary>
        protected static ConcurrentDictionary<int, long> lastReadFromDatabaseTicks = new ConcurrentDictionary<int, long>();

        /// <summary>
        /// Gets the the latest update by any server from the cache. See also SetLastUpdatedToCache.
        /// </summary>
        /// <returns>The ticks value of the date/time of the most recent update.</returns>
        public static long GetLastUpdatedFromCache(int accountId)
        {
            long lastUpdatedFromCacheTicks = 0;
            var cache = new Utilities.DistributedCaching.Cache();
            var lastUpdatedFromCacheDateTime = cache.Get(accountId, string.Empty, LatestUpdateKey) as long?;
            if (lastUpdatedFromCacheDateTime.HasValue)
            {
                lastUpdatedFromCacheTicks = lastUpdatedFromCacheDateTime.Value;
            }
            else
            {
                lastUpdatedFromCacheTicks = long.MaxValue;
                //possibly null if the cache wasn't available - so this is a failsafe, which will force 
                //a refresh from database.

                SaveLastUpdatedToCache(accountId, DateTime.Now);
                //...this has the effect that if a UDF has never been saved since the cache was created,
                //then it will not always load from the database.
                //it should still reload if another server saves a UDF subsequently, as if the cache is available
                //then the time saved by this call will be superseded with a later time, 
                //but if the cache is not available, then this call will fail and have no effect.
            }
            return lastUpdatedFromCacheTicks;
        }

        /// <summary>
        /// Saves the latest update to the cache so other servers will know to refresh their in-memory lists from the database.
        /// Called when e.g. a user defined field is saved
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="lastUpdated">The ticks value of the date time at which the update occurred.</param>
        public static void SaveLastUpdatedToCache(int accountId, DateTime lastUpdated)
        {
            var cache = new Utilities.DistributedCaching.Cache();
            cache.Set(accountId, string.Empty, LatestUpdateKey, lastUpdated.Ticks);
        }

        /// <summary>
        /// Find a duplicate
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool AlreadyExists(cUserDefinedField field)
        {
            if (this.UserdefinedFields == null || this.UserdefinedFields.Count == 0)
            {
                return false;
            }
            foreach (cUserDefinedField tmpField in this.UserdefinedFields.Values)
            {
                if (tmpField.label == field.label && tmpField.table.TableID == field.table.TableID && tmpField.userdefineid != field.userdefineid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the user defined field definition
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="field"></param>
        /// <returns></returns>
        protected int SaveUserDefinedFieldToDB(cCurrentUserBase currentUser, cUserDefinedField field, out DateTime lastModified)
        {
            using (var sqlConnection = new SqlConnection(DatabaseConnection.GetConnectionStringWithDecryptedPassword(this._connectionString)))
            {
                sqlConnection.Open();
                using (var sqlTransaction = sqlConnection.BeginTransaction())
                {
                    var sqlCommand = new SqlCommand("saveUserDefinedField", sqlConnection, sqlTransaction) { CommandType = CommandType.StoredProcedure };

                    sqlCommand.Parameters.AddWithValue("@userdefineid", field.userdefineid);
                    sqlCommand.Parameters.AddWithValue("@tableid", field.table.TableID);
                    sqlCommand.Parameters.AddWithValue("@attributename", field.attribute.attributename);
                    sqlCommand.Parameters.AddWithValue("@displayname", field.attribute.displayname);
                    sqlCommand.Parameters.AddWithValue("@description", field.attribute.description);
                    sqlCommand.Parameters.AddWithValue("@tooltip", field.attribute.tooltip);
                    sqlCommand.Parameters.AddWithValue("@mandatory", Convert.ToByte(field.attribute.mandatory));
                    sqlCommand.Parameters.AddWithValue("@fieldtype", (byte)field.attribute.fieldtype);
                    if (field.modifiedby == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@userid", field.createdby);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@userid", field.modifiedby);
                    }
                    if (field.modifiedon == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@date", field.createdon);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@date", field.modifiedon);
                    }
                    sqlCommand.Parameters.AddWithValue("@maxlength", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@format", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@defaultvalue", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@precision", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@order", field.order);
                    sqlCommand.Parameters.AddWithValue("@hyperlinkText", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@hyperlinkPath", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@relatedTable", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@acDisplayField", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@maxRows", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@encrypted", field.Encrypted);

                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.Text:
                        case FieldType.LargeText:
                            cTextAttribute textatt = (cTextAttribute)field.attribute;
                            if (textatt.maxlength != null)
                            {
                                sqlCommand.Parameters["@maxlength"].Value = (int)textatt.maxlength;
                            }
                            sqlCommand.Parameters["@format"].Value = (byte)textatt.format;
                            break;
                        case FieldType.DateTime:
                            cDateTimeAttribute dateatt = (cDateTimeAttribute)field.attribute;
                            sqlCommand.Parameters["@format"].Value = (byte)dateatt.format;
                            break;
                        case FieldType.TickBox:
                            cTickboxAttribute tickboxatt = (cTickboxAttribute)field.attribute;
                            if (tickboxatt.defaultvalue != string.Empty)
                            {
                                sqlCommand.Parameters["@defaultvalue"].Value = tickboxatt.defaultvalue;
                            }
                            break;
                        case FieldType.Number:
                        case FieldType.Currency:
                        case FieldType.Integer:
                            cNumberAttribute numberatt = (cNumberAttribute)field.attribute;
                            sqlCommand.Parameters["@precision"].Value = numberatt.precision;
                            break;
                        case FieldType.Hyperlink:
                            cHyperlinkAttribute hyperlinkAtt = (cHyperlinkAttribute)field.attribute;
                            sqlCommand.Parameters["@hyperlinkText"].Value = hyperlinkAtt.hyperlinkText;
                            sqlCommand.Parameters["@hyperlinkPath"].Value = hyperlinkAtt.hyperlinkPath;
                            break;
                        case FieldType.Relationship:
                            cManyToOneRelationship relationshipAtt = (cManyToOneRelationship)field.attribute;
                            sqlCommand.Parameters["@relatedTable"].Value = relationshipAtt.relatedtable.TableID;
                            sqlCommand.Parameters["@acDisplayField"].Value = relationshipAtt.AutoCompleteDisplayField;
                            sqlCommand.Parameters["@maxRows"].Value = (relationshipAtt.AutoCompleteMatchRows == 0) ? 15 : relationshipAtt.AutoCompleteMatchRows;
                            break;
                    }
                    if (field.Grouping == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@groupID", DBNull.Value);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@groupID", field.Grouping.UserdefinedGroupID);
                    }

                    sqlCommand.Parameters.AddWithValue("@specificItem", field.Specific);
                    sqlCommand.Parameters.AddWithValue("@allowSearch", field.AllowSearch);
                    sqlCommand.Parameters.AddWithValue("@allowEmployeeToPopulate", field.AllowEmployeeToPopulate);
                    sqlCommand.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate == true)
                    {
                        sqlCommand.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }

                    sqlCommand.Parameters.Add("@identity", SqlDbType.Int);
                    sqlCommand.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                    sqlCommand.ExecuteNonQuery();

                    int id = (int)sqlCommand.Parameters["@identity"].Value;

                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.List:
                            SaveListItemsToDB(sqlConnection, sqlTransaction, currentUser, id, (cListAttribute)field.attribute);
                            break;
                        case FieldType.Relationship:
                            SaveMatchFieldsToDB(sqlConnection, sqlTransaction, currentUser, id, (cManyToOneRelationship)field.attribute);
                            break;
                    }

                    using (var getModifiedCommand = new SqlCommand("select getdate()", sqlConnection, sqlTransaction))
                    {
                        lastModified = (DateTime)getModifiedCommand.ExecuteScalar();
                    }
                    sqlTransaction.Commit();

                    return id;
                }
            }

        }

        private void SaveMatchFieldsToDB(SqlConnection sqlConnection, SqlTransaction sqlTransaction, cCurrentUserBase currentUser, int id, cManyToOneRelationship relationship)
        {
            using (var sqlCommand = new SqlCommand("saveUserdefinedMatchFields", sqlConnection, sqlTransaction) { CommandType = CommandType.StoredProcedure })
            {
                // save the match field ids
                List<SqlDataRecord> lstMatchData = null;
                // Generate a sql GuidPK table param and pass into the stored proc
                SqlMetaData[] tvpItems = { new SqlMetaData("ID", System.Data.SqlDbType.UniqueIdentifier) };

                if (relationship.AutoCompleteMatchFieldIDList != null && relationship.AutoCompleteMatchFieldIDList.Count > 0)
                {
                    lstMatchData = new List<SqlDataRecord>();
                    foreach (Guid fieldID in relationship.AutoCompleteMatchFieldIDList.Distinct())
                    {
                        SqlDataRecord row = new SqlDataRecord(tvpItems);
                        row.SetGuid(0, fieldID);
                        lstMatchData.Add(row);
                    }
                }

                sqlCommand.Parameters.AddWithValue("@userdefineid", id);
                sqlCommand.Parameters.Add("@fieldIdList", System.Data.SqlDbType.Structured);
                sqlCommand.Parameters["@fieldIdList"].Direction = System.Data.ParameterDirection.Input;
                sqlCommand.Parameters["@fieldIdList"].Value = lstMatchData;

                sqlCommand.ExecuteNonQuery();
            }
        }

        private void SaveListItemsToDB(SqlConnection sqlConnection, SqlTransaction sqlTransaction, ICurrentUserBase currentUser, int userdefineid, cListAttribute attribute)
        {
            using (var deleteCommand = new SqlCommand("deleteUserDefinedListItems", sqlConnection, sqlTransaction) { CommandType = CommandType.StoredProcedure })
            {
                cUserDefinedField curFieldDef = GetUserDefinedById(userdefineid);

                if (curFieldDef != null)
                {
                    List<int> deleteValueIDs = new List<int>();

                    foreach (KeyValuePair<int, cListAttributeElement> i in curFieldDef.items)
                    {
                        cListAttributeElement item = (cListAttributeElement)i.Value;

                        bool itemExists = false;

                        foreach (KeyValuePair<int, cListAttributeElement> newItems in attribute.items)
                        {
                            cListAttributeElement newItem = (cListAttributeElement)newItems.Value;

                            if (newItem.elementValue == item.elementValue)
                            {
                                itemExists = true;
                                break;
                            }
                        }

                        if (!itemExists)
                        {
                            deleteValueIDs.Add(item.elementValue);
                        }
                    }

                    foreach (int deleteID in deleteValueIDs)
                    {
                        deleteCommand.Parameters.Clear();

                        deleteCommand.Parameters.Add("@valueid", SqlDbType.Int);
                        deleteCommand.Parameters["@valueid"].Value = deleteID;
                        deleteCommand.Parameters.Add("@userdefineid", SqlDbType.Int);
                        deleteCommand.Parameters["@userdefineid"].Value = userdefineid;
                        deleteCommand.Parameters.Add("@CUemployeeID", SqlDbType.Int);
                        deleteCommand.Parameters["@CUemployeeID"].Value = currentUser.EmployeeID;
                        deleteCommand.Parameters.Add("@CUdelegateID", SqlDbType.Int);
                        if (currentUser.Delegate != null)
                        {
                            deleteCommand.Parameters["@CUdelegateID"].Value = currentUser.Delegate.EmployeeID;
                        }
                        else
                        {
                            deleteCommand.Parameters["@CUdelegateID"].Value = DBNull.Value;
                        }
                        deleteCommand.ExecuteNonQuery();
                    }

                }
            }

            using (var addCommand = new SqlCommand("addUserDefinedListItem", sqlConnection, sqlTransaction) { CommandType = CommandType.StoredProcedure })
            {
                foreach (KeyValuePair<int, cListAttributeElement> i in attribute.items)
                {
                    addCommand.Parameters.Clear();
                    var element = i.Value;

                    addCommand.Parameters.AddWithValue("@userdefineid", userdefineid);
                    addCommand.Parameters.AddWithValue("@valueid", element.elementValue);
                    addCommand.Parameters.AddWithValue("@item", element.elementText);
                    addCommand.Parameters.AddWithValue("@order", element.elementOrder);
                    addCommand.Parameters.AddWithValue("@archived", element.Archived);
                    addCommand.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        addCommand.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        addCommand.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                    addCommand.ExecuteNonQuery();

                }
            }
        }

        public cUserDefinedField GetUserdefinedFieldByFieldID(Guid id)
        {
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.attribute.fieldid == id)
                {
                    return field;
                }
            }
            return null;
        }
        public List<cUserDefinedField> GetFieldsByTable(cTable table)
        {
            List<cUserDefinedField> fields = new List<cUserDefinedField>();
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.table != null && field.table.TableID == table.TableID)
                {
                    fields.Add(field);
                }
            }
            return fields;
        }

        public SortedList<int, cUserDefinedField> GetFieldsByTableAndGrouping(cTable table, cUserdefinedFieldGrouping grouping, bool excludeAdminFields = false)
        {
            SortedList<int, cUserDefinedField> fields = new SortedList<int, cUserDefinedField>();
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.table.TableID == table.TableID && field.Grouping == grouping)
                {
                    // only add fields for userdefinedCars that are for claimants to add if not an administrator screen
                    if (field.table.TableName == "userdefinedCars" && !field.AllowEmployeeToPopulate && excludeAdminFields) continue;

                    fields.Add(field.order, field);
                }
            }
            return fields;
        }
        public List<cUserDefinedField> GetFieldsByTableAndType(cTable table, FieldType type)
        {
            List<cUserDefinedField> fields = new List<cUserDefinedField>();
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.table.TableID == table.TableID && field.fieldtype == type)
                {
                    fields.Add(field);
                }
            }
            return fields;
        }

        public List<cUserDefinedField> GetSpecificUserDefinedFields()
        {
            List<cUserDefinedField> fields = new List<cUserDefinedField>();
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.Specific)
                {
                    fields.Add(field);
                }
            }
            return fields;
        }

        public cUserDefinedField GetUserDefinedById(int userdefineid)
        {
            cUserDefinedField field = null;
            this.UserdefinedFields.TryGetValue(userdefineid, out field);
            return field;
        }

        protected int DeleteUserDefinedToDB(cCurrentUserBase currentUser, int userdefineid, out DateTime lastUpdated)
        {
            try
            {
                using (var data = new SqlConnection(DatabaseConnection.GetConnectionStringWithDecryptedPassword(this._connectionString)))
                {
                    data.Open();
                    using (var transaction = data.BeginTransaction())
                    {
                        using (var deleteUdfCommand = new SqlCommand("deleteUserdefined", data, transaction) { CommandType = CommandType.StoredProcedure })
                        using (var getDateCommand = new SqlCommand("select getdate()", data, transaction))
                        {
                            lastUpdated = (DateTime)getDateCommand.ExecuteScalar();

                            deleteUdfCommand.Parameters.AddWithValue("@userdefineid", userdefineid);
                            deleteUdfCommand.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                            if (currentUser.isDelegate == true)
                            {
                                deleteUdfCommand.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                            }
                            else
                            {
                                deleteUdfCommand.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                            }

                            deleteUdfCommand.Parameters.Add("@udfDeleted", SqlDbType.Int);
                            deleteUdfCommand.Parameters["@udfDeleted"].Direction = ParameterDirection.ReturnValue;
                            deleteUdfCommand.ExecuteNonQuery();
                            transaction.Commit();
                            return (int)deleteUdfCommand.Parameters["@udfDeleted"].Value;
                        }
                    }
                }
            }
            catch
            {
                lastUpdated = DateTime.MinValue;
                return -1;
            }
        }

        public SortedList<int, object> GetRecord(cTable tbl, int id, cTables clsTables, cFields clsFields)
        {
            SortedList<int, object> record = new SortedList<int, object>();
            cQueryBuilder clsquery = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, tbl, clsTables, clsFields);

            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            if (fields.Count == 0)
            {
                return new SortedList<int, object>();
            }

            foreach (cUserDefinedField field in fields)
            {
                if (field.attribute.GetType() != typeof(cOneToManyRelationship))
                {
                    cField cField = clsFields.GetFieldByID(field.attribute.fieldid);
                    clsquery.addColumn(cField, field.attribute.attributeid.ToString());
                }
            }

            clsquery.addFilter(tbl.GetPrimaryKey(), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.None, null); // null as udf? !!!!!!!!

            using (SqlDataReader reader = clsquery.getReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Contains("_text") == false)
                        {
                            string tmp = reader.GetName(i).Replace("udf", string.Empty);
                            record.Add(Convert.ToInt32(tmp), reader.GetValue(i));
                        }
                    }
                }
                reader.Close();
            }

            return record;
        }

        public SortedList<int, SortedList<int, object>> GetRecords(cTable tbl, List<int> ids, cTables clsTables, cFields clsFields)
        {
            SortedList<int, SortedList<int, object>> records = new SortedList<int, SortedList<int, object>>();
            cQueryBuilder clsquery = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, tbl, clsTables, clsFields);

            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            if (fields.Count == 0)
            {
                return new SortedList<int, SortedList<int, object>>();
            }

            clsquery.addColumn(tbl.GetPrimaryKey());

            foreach (cUserDefinedField field in fields)
            {
                if (field.attribute.GetType() != typeof(cOneToManyRelationship))
                {
                    clsquery.addColumn(clsFields.GetFieldByID(field.attribute.fieldid), field.attribute.attributeid.ToString());
                }
            }

            clsquery.addFilter(tbl.GetPrimaryKey(), ConditionType.Equals, ids.Cast<object>().ToArray(), null, ConditionJoiner.None, null);

            using (SqlDataReader reader = clsquery.getReader())
            {
                while (reader.Read())
                {
                    SortedList<int, object> record = new SortedList<int, object>();
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Contains("_text") == false)
                        {
                            record.Add(Convert.ToInt32(reader.GetName(i)), reader.GetValue(i));
                        }
                    }

                    records.Add(reader.GetInt32(0), record);
                }

                reader.Close();
            }

            return records;
        }


        public SortedList<int, SortedList<int, object>> GetAllRecords(cTable tbl, cTables clsTables, cFields clsFields)
        {
            SortedList<int, SortedList<int, object>> records = new SortedList<int, SortedList<int, object>>();
            SortedList<int, object> record;
            cQueryBuilder clsquery = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, tbl, clsTables, clsFields);

            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            if (fields.Count == 0)
            {
                return new SortedList<int, SortedList<int, object>>();
            }
            clsquery.addColumn(tbl.GetPrimaryKey());
            foreach (cUserDefinedField field in fields)
            {
                if (field.attribute.GetType() != typeof(cOneToManyRelationship))
                {
                    clsquery.addColumn(clsFields.GetFieldByID(field.attribute.fieldid), field.attribute.attributeid.ToString());
                }
            }

            using (SqlDataReader reader = clsquery.getReader())
            {
                while (reader.Read())
                {
                    record = new SortedList<int, object>();
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Contains("_text") == false)
                        {
                            record.Add(Convert.ToInt32(reader.GetName(i)), reader.GetValue(i));
                        }
                    }
                    records.Add(reader.GetInt32(0), record);
                }
                reader.Close();
            }
            return records;
        }

        public SortedList<int, SortedList<int, object>> GetAllRecords(cTable tbl, cField filterField, object filterValue, cTables clsTables, cFields clsFields)
        {
            SortedList<int, SortedList<int, object>> records = new SortedList<int, SortedList<int, object>>();
            SortedList<int, object> record;
            cQueryBuilder clsquery = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, tbl, clsTables, clsFields);

            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            if (fields.Count == 0)
            {
                return new SortedList<int, SortedList<int, object>>();
            }
            clsquery.addColumn(tbl.GetPrimaryKey());
            foreach (cUserDefinedField field in fields)
            {
                if (field.attribute.GetType() != typeof(cOneToManyRelationship))
                {
                    clsquery.addColumn(clsFields.GetFieldByID(field.attribute.fieldid), field.attribute.attributeid.ToString());
                }
            }
            clsquery.addFilter(filterField, ConditionType.Equals, new object[] { filterValue }, null, ConditionJoiner.None, null); // null as genlist/udf? !!!!!!!

            DBConnection expdata = new DBConnection(this._connectionString);
            using (SqlDataReader reader = clsquery.getReader())
            {
                while (reader.Read())
                {
                    record = new SortedList<int, object>();
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Contains("_text") == false)
                        {
                            record.Add(Convert.ToInt32(reader.GetName(i)), reader.GetValue(i));
                        }
                    }
                    records.Add(reader.GetInt32(0), record);
                }
                reader.Close();
            }
            return records;
        }

        public void SaveValues(cTable table, int id, SortedList<int, object> values, cTables tables, cFields fields, ICurrentUserBase currentUser, GroupingOutputType groupType = GroupingOutputType.All, bool skipFieldsNotOnPage = false, int elementId = 0, string record = "")
        {
            if (values == null || values.Count == 0)
            {
                return;
            }

            List<cUserDefinedField> udfFields = GetFieldsByTable(table);
            //does the record already exist
            var builder = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, table, tables, fields);
            builder.addColumn(table.GetPrimaryKey(), SelectType.Count);
            builder.addFilter(table.GetPrimaryKey(), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.None, null);  // null as on bt? !!!!!!!!

            var updateQuery = new cUpdateQuery(AccountID, this._connectionString, this.MetabaseConnectionString, table, tables, fields);
            var udfValues = new SortedList<int, object>();

            foreach (cUserDefinedField udf in udfFields)
            {
                if (skipFieldsNotOnPage && values.ContainsKey(udf.userdefineid) == false)
                {
                    // if field does not have a value passed for it, don't bother to update that field
                    continue;
                }

                if (groupType == GroupingOutputType.All || (groupType == GroupingOutputType.GroupedOnly && udf.Grouping != null) || (groupType == GroupingOutputType.UnGroupedOnly && udf.Grouping == null))
                {
                    cField field = fields.GetBy(table.TableID, "udf" + udf.userdefineid);

                    if (values.ContainsKey(udf.userdefineid))
                    {
                        object value = values[udf.userdefineid];

                        switch (udf.attribute.fieldtype)
                        {
                            case FieldType.Currency:
                            case FieldType.Number:
                            case FieldType.Integer:
                            case FieldType.DateTime:
                                if (value == null || value.ToString() == string.Empty)
                                {
                                    value = DBNull.Value;
                                }

                                break;
                            case FieldType.Relationship:
                                if (value == null || value.ToString() == string.Empty || value.ToString() == "0")
                                {
                                    value = DBNull.Value;
                                }

                                break;
                            case FieldType.RelationshipTextbox:
                                var relatedText = (cRelationshipTextBoxAttribute)udf.attribute;
                                var relatedQuery = new cQueryBuilder(AccountID, this._connectionString, this.MetabaseConnectionString, relatedText.relatedtable, tables, fields);
                                relatedQuery.addColumn(relatedText.relatedtable.GetPrimaryKey());
                                relatedQuery.addFilter(relatedText.relatedtable.GetKeyField(), ConditionType.Equals, new[] { value }, null, ConditionJoiner.None, null); // null as genlist? !!!!!!
                                if (relatedText.relatedtable.GetSubAccountIDField() != null)
                                {
                                    relatedQuery.addFilter(relatedText.relatedtable.GetSubAccountIDField(), ConditionType.Equals, new object[] { currentUser.CurrentSubAccountId }, null, ConditionJoiner.And, null);
                                    // null as genlist? !!!!!!
                                }

                                using (SqlDataReader reader = relatedQuery.getReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (!reader.IsDBNull(0))
                                        {
                                            value = reader.GetInt32(0);
                                            break;
                                        }
                                    }
                                    reader.Close();
                                }
                                break;
                        }
                        updateQuery.addColumn(field, value ?? DBNull.Value);
                        udfValues.Add(udf.userdefineid, value);
                    }
                    else
                    {
                        updateQuery.addColumn(field, DBNull.Value);
                        udfValues.Add(udf.userdefineid, DBNull.Value);
                    }
                }
            }

            if (updateQuery.lstColumns.Count > 0)
            {
                int count = builder.GetCount();
                if (count == 0)
                {
                    updateQuery.addColumn(table.GetPrimaryKey(), id);
                    updateQuery.executeInsertStatement();
                }
                else
                {
                    updateQuery.addFilter(table.GetPrimaryKey(), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.None, null);  // null as always on bt? !!!!!!

                    SortedList<int, SortedList<int, object>> userDefinedFields = this.GetRecords(table, new List<int> { id }, tables, fields);
                    SortedList<int, object> previousUdfValuesFromDB = userDefinedFields[id];

                    List<UdfAuditingDetails> previousUdfValues = ProcessUdfValues(fields, table, previousUdfValuesFromDB);
                    List<UdfAuditingDetails> newUdfValues = ProcessUdfValues(fields, table, udfValues);

                    List<UdfRecordForAudit> udfRecordsForAudit = GetDifferences(previousUdfValues, newUdfValues, table, fields);

                    //update UDF values.
                    updateQuery.executeUpdateStatement();

                    if (udfRecordsForAudit.Count > 0)
                    {
                        //update audit log
                        this.UpdateAuditLog(udfRecordsForAudit, elementId, record, currentUser, id);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the UDF values to ensure the data types and values are correct and consistent
        /// </summary>
        /// <param name="fields">The field class</param>
        /// <param name="table">The tables class</param>
        /// <param name="udfValues"></param>
        /// <returns>A list of <see cref="UdfAuditingDetails"/>.</returns>
        internal List<UdfAuditingDetails> ProcessUdfValues(cFields fields, cTable table, SortedList<int, object> udfValues)
        {
            var processedUdfValues = new List<UdfAuditingDetails>();
            int count = udfValues.Count;
            int i = 0;

            while (i < count)
            {
                cField field = fields.GetBy(table.TableID, "udf" + udfValues.Keys[i]);
                object value = DBNull.Value;

                switch (field.FieldType)
                {
                    //Yes/No
                    case "X":
                        {
                            string yesNoValue = udfValues.Values[i]?.ToString();

                            if (yesNoValue == "True" || yesNoValue == "1")
                            {
                                value = 1;
                            }
                            else
                            {
                                value = 0;
                            }
                        }
                        break;
                    //Integer
                    case "I":
                    //Relationship, Numeric & List (They share the same field.fieldType)
                    case "N":
                        value = udfValues.Values[i] == DBNull.Value ? 0 : Convert.ToInt32(udfValues.Values[i]);
                        break;
                    //Currency
                    case "C":
                        value = Math.Round(udfValues.Values[i] == DBNull.Value ? Convert.ToDecimal(0) : Convert.ToDecimal(udfValues.Values[i]), 2);
                        break;
                    //Decimal
                    case "M":
                        value = udfValues.Values[i] == DBNull.Value ? Convert.ToDecimal(0) : Convert.ToDecimal(udfValues.Values[i]);
                        break;
                    //Date Time
                    case "DT":
                    //Time
                    case "T":
                    //Date
                    case "D":

                        if (udfValues.Values[i] == DBNull.Value)
                        {
                            value = string.Empty;
                        }
                        else
                        {
                            value = Convert.ToDateTime(udfValues.Values[i]);
                        }

                        break;
                    default:

                        if (udfValues.Values[i] == DBNull.Value || udfValues.Values[i] == null)
                        {
                            value = string.Empty;
                        }
                        else
                        {
                            value = HttpUtility.HtmlDecode(udfValues.Values[i].ToString());
                        }

                        break;
                }
                processedUdfValues.Add(new UdfAuditingDetails(udfValues.Keys[i], value, field));
                i++;
            }

            return processedUdfValues;
        }

        /// <summary>
        ///  Compares the values from the new and previous UDF values. 
        ///  Builds a list of Udf's and their new/previous values.
        /// </summary>
        /// <param name="previousUdfValues">A list of <see cref="UdfAuditingDetails"/>.</param>
        /// <param name="newUdfValues">A list of new udf values.></param>
        /// <returns>A list of <see cref="UdfRecordForAudit"/>.</returns>
        internal List<UdfRecordForAudit> GetDifferences(IEnumerable<UdfAuditingDetails> previousUdfValues, List<UdfAuditingDetails> newUdfValues, cTable table, cFields fields)
        {
            var udfRecordsForAudit = new List<UdfRecordForAudit>();

            foreach (var udfValue in previousUdfValues)
            {
                var previousValue = udfValue.Value;
                var item = newUdfValues.Find(x => x.UdfId == udfValue.UdfId);

                if (item != null)
                {
                    var newValue = item.Value;

                    if (!previousValue.Equals(newValue))
                    {
                        string processedPreviousValue = string.Empty;
                        string processedNewValue = string.Empty;

                        switch (udfValue.Field.FieldType)
                        {
                                //time
                            case "T":
                                if (previousValue == DBNull.Value || previousValue == string.Empty)
                                {
                                    processedPreviousValue = string.Empty;
                                }
                                else
                                {
                                    processedPreviousValue = Convert.ToDateTime(previousValue).ToString(("H:mm"));
                                }

                                if (newValue == DBNull.Value || newValue == string.Empty)
                                {
                                    processedNewValue = string.Empty;
                                }
                                else
                                {
                                    processedNewValue = Convert.ToDateTime(newValue).ToString(("H:mm"));
                                }

                                break;
                                //date
                            case "D":
                                if (previousValue == DBNull.Value || previousValue == string.Empty)
                                {
                                    processedPreviousValue = string.Empty;
                                }
                                else
                                {
                                    processedPreviousValue = Convert.ToDateTime(previousValue).ToString(("dd/MM/yyyy"));
                                }

                                if (newValue == DBNull.Value || newValue == string.Empty)
                                {
                                    processedNewValue = string.Empty;
                                }
                                else
                                {
                                    processedNewValue = Convert.ToDateTime(newValue).ToString(("dd/MM/yyyy"));
                                }
                             
                                break;
                                //True/False
                            case "X":

                                if (previousValue == DBNull.Value)
                                {
                                    processedPreviousValue = "No";
                                }
                                else
                                {
                                    processedPreviousValue = Convert.ToInt32(previousValue) == 1 ? "Yes" : "No";
                                }

                                if (newValue == DBNull.Value)
                                {
                                    processedPreviousValue = "No";
                                }
                                else
                                {
                                    processedNewValue = Convert.ToInt32(newValue) == 1 ? "Yes" : "No";

                                }
                    
                                break;
                                //Relationship, Numeric & List (They share the same field.fieldType)
                            case "N":

                                List<cUserDefinedField> udfDFields = GetFieldsByTable(table);

                                foreach (cUserDefinedField udfField in udfDFields.Where(field => field.userdefineid == udfValue.UdfId))
                                {

                                    switch (udfField.attribute.fieldtype)
                                    {
                                        case FieldType.Integer:
                                        processedPreviousValue = (previousValue == DBNull.Value ? string.Empty : previousValue.ToString());
                                        processedNewValue = newValue.ToString();
                                            break;
                                        case FieldType.Relationship:
                                            processedPreviousValue = previousValue == DBNull.Value ? string.Empty : GetOtmActualText(fields, udfField, previousValue);
                                            processedNewValue = GetOtmActualText(fields, udfField, newValue);
                                            break;
                                        case FieldType.List:
                                        
                                            var listatt = udfValue.Field.ListItems;
                                            const string noneListItemText = "[None]";
                                            
                                            if (previousValue == DBNull.Value || (int)previousValue == 0)
                                            {
                                                processedPreviousValue = noneListItemText;
                                    }
                                    else
                                    {
                                                processedPreviousValue = listatt[Convert.ToInt32(previousValue)];
                                            }

                                            processedNewValue = newValue.ToString() == "0" ? noneListItemText : listatt[Convert.ToInt32(newValue)];
                               
                                            break;
                                        default:
                                            processedPreviousValue = previousValue.ToString();
                                            processedNewValue = newValue.ToString();

                                            break;
                                    }
                                }

                                break;
                            default:

                                //Decode HTML here to ensure any HTML code is rendered in the Audit log
                                processedPreviousValue = HttpUtility.HtmlDecode(previousValue.ToString());
                                processedNewValue = HttpUtility.HtmlDecode(newValue.ToString());

                                break;
                        }

                        udfRecordsForAudit.Add(new UdfRecordForAudit(udfValue.Field.FieldID, processedPreviousValue, processedNewValue, "udf" + udfValue.UdfId));
                    }
                }
            }

            return udfRecordsForAudit;
        }

        /// <summary>
        /// Gets the actual text of the one to many relationship based on the lookup value. 
        /// I.e. the text the user sees on the form.
        /// </summary>
        /// <param name="fields">A collection of fields</param>
        /// <param name="udfField">The user defined field</param>
        /// <param name="lookupValue">The lookup value for one to many field</param>
        /// <returns>The text of the one to many relationship</returns>
        public string GetOtmActualText(cFields fields, cUserDefinedField udfField, object lookupValue)
        {
            cManyToOneRelationship relationship = (cManyToOneRelationship)udfField.attribute;
            cQueryBuilder query = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID),
                ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, relationship.relatedtable,
                new cTables(AccountID), fields);

            cField acDisplayField = fields.GetFieldByID(relationship.AutoCompleteDisplayField);
            query.addColumn(acDisplayField);
            query.addFilter(relationship.relatedtable.GetPrimaryKey(), ConditionType.Equals, new object[] { lookupValue }, null,
                ConditionJoiner.None, null);

            string otmValue = string.Empty;

            using (SqlDataReader reader = query.getReader())
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        switch (acDisplayField.FieldType)
                        {
                            case "D":
                                otmValue = reader.GetDateTime(0).ToShortDateString();
                                break;
                            case "DT":
                                otmValue = reader.GetDateTime(0).ToString();
                                break;
                            case "N":
                            case "FI":
                                otmValue = reader.GetInt32(0).ToString();
                                break;
                            case "C":
                            case "FD":
                            case "M":
                                otmValue = reader.GetDecimal(0).ToString();
                                break;
                            default:
                                otmValue = reader.GetString(0);
                                break;
                        }
                        break;
                    }
                }
                reader.Close();
            }
            return otmValue;
        }

        /// <summary>
        /// Updates the audit log with the changes made to UDF's
        /// </summary>
        /// <param name="udfRecordsForAudit"></param>
        /// <param name="elementId">The elementId</param>
        /// <param name="record">The record</param>
        /// <param name="currentUser">The current user</param>
        /// <param name="id">The id of the record the udfs belong to</param>
        private void UpdateAuditLog(IEnumerable<UdfRecordForAudit> udfRecordsForAudit, int elementId, string record, ICurrentUserBase currentUser, int id)
        {
            foreach (var udfRecord in udfRecordsForAudit)
            {
                using (IDBConnection expdata = new DatabaseConnection(this._connectionString))
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@elementid", elementId);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", id);
                    expdata.sqlexecute.Parameters.AddWithValue("@field", udfRecord.FieldId);
                    expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", record);
                    expdata.sqlexecute.Parameters.AddWithValue("@oldvalue", udfRecord.PreviousValue);
                    expdata.sqlexecute.Parameters.AddWithValue("@newvalue", udfRecord.NewValue);
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);
                    expdata.ExecuteProc("addUpdateEntryToAuditLog");
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
        }

        public int GetNextOrder()
        {
            int order = 0;
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                if (field.order > order)
                {
                    order++;
                }
            }

            order++;
            return order;
        }

        public sOnlineUserdefinedInfo GetModifiedUserdefinedFields(DateTime date)
        {
            sOnlineUserdefinedInfo onlineUserdefined = new sOnlineUserdefinedInfo();
            Dictionary<int, cUserDefinedField> lstUserdefined = new Dictionary<int, cUserDefinedField>();
            List<int> lstUserdefinedids = new List<int>();
            Dictionary<int, cListItem> lstListitems = new Dictionary<int, cListItem>();
            List<int> lstListitemids = new List<int>();

            foreach (cUserDefinedField val in this.UserdefinedFields.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lstUserdefined.Add(val.userdefineid, val);
                }

                lstUserdefinedids.Add(val.userdefineid);
                //**todo**
                //foreach (cListItem item in val.items.Values)
                //{
                //    if (item.createdon > date || item.modifiedon > date)
                //    {
                //        lstListitems.Add(item.itemid, item);
                //    }

                //    lstListitemids.Add(item.itemid);
                //}
            }

            onlineUserdefined.lstUserdefined = lstUserdefined;
            onlineUserdefined.lstUserdefinedids = lstUserdefinedids;
            onlineUserdefined.lstListitems = lstListitems;
            onlineUserdefined.lstListitemids = lstListitemids;

            return onlineUserdefined;
        }

        protected string CreateJavaScriptArray()
        {

            StringBuilder output = new StringBuilder();
            output.Append("var lstUserdefined;");
            output.Append("if(lstUserdefined == null || lstUserdefined == undefined) { lstUserdefined = new Array(); }\n");
            //output.Append("var curSMHost = '" + sHost + "';\n");
            output.Append("var userdefinedField;\n");

            return output.ToString();
        }


        /// <summary>
        /// Gets a collection of active user defined fields that have been tagged as searchable.
        /// </summary>
        /// <returns>Sorted List of User Defined Field class objects</returns>
        public SortedList<int, cUserDefinedField> GetSearchableFields()
        {
            SortedList<int, cUserDefinedField> retFields = new SortedList<int, cUserDefinedField>();

            foreach (KeyValuePair<int, cUserDefinedField> uf in this.UserdefinedFields)
            {
                cUserDefinedField curUF = (cUserDefinedField)uf.Value;

                if (curUF.AllowSearch && !curUF.Archived)
                {
                    retFields.Add(curUF.userdefineid, curUF);
                }
            }

            return retFields;
        }

        /// <summary>
        /// Saves the order of userdefined fields
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="appliesTo"></param>
        /// <param name="lastUpdated"></param>
        /// <returns></returns>
        protected void SaveOrdersToDB(Dictionary<int, int> orders, cTable appliesTo, out DateTime lastUpdated)
        {
            List<SqlDataRecord> lstFieldOrderData = new List<SqlDataRecord>();

            //-- Create the data type - required when passing in the sql table param below
            //CREATE TYPE dbo.UserdefinedFieldOrdering AS TABLE 
            //(
            // userdefinedFieldID int NOT NULL, 
            // displayOrder int NOT NULL, 
            // PRIMARY KEY (userdefinedFieldID)
            //)

            // Generate a sql table param and pass into the stored proc
            SqlMetaData[] tvpGroupOrder = { new SqlMetaData("userdefinedFieldID", System.Data.SqlDbType.Int), new SqlMetaData("displayOrder", System.Data.SqlDbType.Int) };

            SqlDataRecord row;
            foreach (KeyValuePair<int, int> kvp in orders)
            {
                row = new SqlDataRecord(tvpGroupOrder);
                row.SetInt32(0, kvp.Key);
                row.SetInt32(1, kvp.Value);
                lstFieldOrderData.Add(row);
            }

            DBConnection smData = new DBConnection(this._connectionString);

            smData.sqlexecute.Parameters.Add("@fieldOrder", System.Data.SqlDbType.Structured);
            smData.sqlexecute.Parameters["@fieldOrder"].Direction = System.Data.ParameterDirection.Input;
            smData.sqlexecute.Parameters["@fieldOrder"].Value = lstFieldOrderData;
            SqlParameter lastUpdatedParam = smData.sqlexecute.Parameters.Add("@lastUpdated", SqlDbType.DateTime);
            lastUpdatedParam.Direction = ParameterDirection.Output;
            smData.ExecuteProc("dbo.[SaveUserdefinedFieldsOrder]");
            lastUpdated = (DateTime)lastUpdatedParam.Value;
        }

    }
}