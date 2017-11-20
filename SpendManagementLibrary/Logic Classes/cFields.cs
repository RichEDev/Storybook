using System.Collections.Concurrent;
using System.Data;

using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementLibrary.Logic_Classes.Fields;

    public class cFields : IFields
    {
        #region Global Varaiables

        private readonly int nAccountID;

        private ConcurrentDictionary<Guid, cField> lstCachedMetabaseFields;

        private ConcurrentDictionary<Guid, cField> lstCachedCustomerFields;

        private static ConcurrentDictionary<int, ConcurrentDictionary<Guid, cField>> allFields = new ConcurrentDictionary<int, ConcurrentDictionary<Guid, cField>>();

        private static ConcurrentDictionary<int, long> lastReadFromDatabaseTicks = new ConcurrentDictionary<int, long>();

        #endregion Global Variables

        #region Properties

        public int Count
        {
            get
            {
                return this.lstCachedMetabaseFields.Count + this.lstCachedCustomerFields.Count;
            }
        }

        #endregion Properties

        #region Enumerators

        [Flags]
        private enum CacheType
        {
            Metabase = 1 << 0,
            Customer = 1 << 1
        }

        #endregion Enumerators

        #region Methods

        public cFields(int accountID, IDBConnection connection = null)
        {
            if (accountID == 0) throw new Exception("Invalid accountID passed to cFields");
            nAccountID = accountID;
            Initialise(connection);
        }

        public cFields(IDBConnection connection = null)
        {
            nAccountID = 0;
            Initialise(connection);
        }

        private void Initialise(IDBConnection connection = null)
        {
            if (nAccountID > 0)
            {
                long lastUpdatedAllServers = cUserDefinedFieldsBase.GetLastUpdatedFromCache(nAccountID);
                long lastReadFromDatabaseThisServer = lastReadFromDatabaseTicks.GetOrAdd(nAccountID, 0);
                var forceUpdateFromDatabase = lastUpdatedAllServers > lastReadFromDatabaseThisServer;
                if (forceUpdateFromDatabase)
                {
                    ConcurrentDictionary<Guid, cField> oldValue;
                    allFields.TryRemove(nAccountID, out oldValue);
                }
            }

            lstCachedMetabaseFields = allFields.GetOrAdd(0, i => CacheLists(i, connection));
            if (nAccountID != 0)
            {
                lstCachedCustomerFields = allFields.GetOrAdd(nAccountID, i => CacheLists(i, connection));
            }
        }

        private static ConcurrentDictionary<Guid, cField> CacheLists(int nAccountID, IDBConnection connection = null)
        {
            var lstCachedFields = new ConcurrentDictionary<Guid, cField>();
            SortedList<Guid, SortedList<object, string>> listGenItems = new SortedList<Guid, SortedList<object, string>>();
            SortedList<Guid, SortedList<object, string>> listCustomEntityItems = new SortedList<Guid, SortedList<object, string>>();
            SortedList<Guid, SortedList<object, string>> listUserdfinedItems = new SortedList<Guid, SortedList<object, string>>();

            cField tmpField;

            SortedList<object, string> items;


            #region local variables

            Guid fieldID;
            Guid tableID;

            Guid lookUpFieldID;
            Guid viewGroupID;
            Guid lookupTableID;
            Guid relatedTableID;


            string fieldName;
            string fieldType;
            string description;
            string comment;

            string reLabelParam;
            string classPropertyName;
            bool workflowUpdate;
            bool workflowSearchable;
            bool useForLookup;
            bool normalView;
            bool idField;
            bool genList;
            bool canTotal;
            bool reLabel;
            bool valueList;
            bool printOut;
            bool allowImport;
            bool mandatory;
            bool isForeignKey;
            int width;
            int length;
            FieldCategory fieldCat;

            DateTime amendedOn;
            cField.FieldSourceType fieldFrom;

            string friendlyNameTo;
            string friendlyNameFrom;

            #endregion local variables

            IDBConnection db;
            string strSQL = "select getdate() ";

            if (nAccountID == 0)
            {
                listGenItems = GetGenListItems(nAccountID);
                db = connection ?? new DatabaseConnection(GlobalVariables.MetabaseConnectionString);
                strSQL +=
                    " SELECT fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(0 AS INT) AS fieldFrom, classPropertyName, relatedTable, NULL as fieldCategory, IsForeignKey, friendlyNameTo, friendlyNameFrom, treeGroup from [dbo].[fields_base]";
            }
            else
            {
                listCustomEntityItems = GetCustomEntityListItems(nAccountID);
                listUserdfinedItems = GetUserdefinedListItems(nAccountID);
                db = connection ?? new DatabaseConnection(cAccounts.getConnectionString(nAccountID));
                strSQL +=
                    " SELECT fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, CAST(0 as Bit) as relabel, '' as relabel_param, fieldFrom, CAST(NULL as nvarchar(100)) AS classPropertyName, relatedTable, fieldCategory, IsForeignKey, null as friendlyNameTo, null as friendlyNameFrom from [dbo].[Customerfields]";
            }

            const string recordsetError = "The SQL should return the date, then the fields.";
            DateTime lastReadFromDatabase;
            using (IDataReader reader = db.GetReader(strSQL))
            {
                if (!reader.Read()) throw new Exception(recordsetError);
                lastReadFromDatabase = reader.GetDateTime(0);

                if (!reader.NextResult()) throw new Exception(recordsetError);

                #region Ordinals

                int fieldID_ordID = reader.GetOrdinal("fieldid");
                int tableID_ordID = reader.GetOrdinal("tableid");
                int fieldName_ordID = reader.GetOrdinal("field");
                int fieldType_ordID = reader.GetOrdinal("fieldType");
                int description_ordID = reader.GetOrdinal("description");
                int comment_ordID = reader.GetOrdinal("comment");
                int normalview_ordID = reader.GetOrdinal("normalview");
                int idField_ordID = reader.GetOrdinal("idfield");
                int viewGroupID_ordID = reader.GetOrdinal("viewgroupid");
                int genList_ordID = reader.GetOrdinal("genlist");
                int width_ordID = reader.GetOrdinal("width");
                int canTotal_ordID = reader.GetOrdinal("cantotal");
                int printOut_ordID = reader.GetOrdinal("printout");
                int valueList_ordID = reader.GetOrdinal("valuelist");
                int allowImport_ordID = reader.GetOrdinal("allowimport");
                int mandatory_ordID = reader.GetOrdinal("mandatory");
                int amendedOn_ordID = reader.GetOrdinal("amendedon");
                int lookupTable_ordID = reader.GetOrdinal("lookuptable");
                int lookupField_ordID = reader.GetOrdinal("lookupfield");
                int useForLookup_ordID = reader.GetOrdinal("useforlookup");
                int workflowUpdate_ordID = reader.GetOrdinal("workflowUpdate");
                int workflowSearchable_ordID = reader.GetOrdinal("workflowSearch");
                int length_ordID = reader.GetOrdinal("length");
                int reLabel_ordID = reader.GetOrdinal("relabel");
                int reLabelParam_ordID = reader.GetOrdinal("relabel_param");
                int fieldFrom_ordID = reader.GetOrdinal("fieldFrom");
                int classPropertyName_ordID = reader.GetOrdinal("classPropertyName");
                int relatedTableID_ordID = reader.GetOrdinal("relatedTable");
                int fieldCategory_ordID = reader.GetOrdinal("fieldCategory");
                int isForeignKey_ordID = reader.GetOrdinal("IsForeignKey");
                int friendlyNameTo_ordID = reader.GetOrdinal("friendlyNameTo");
                int friendlyNameFrom_ordID = reader.GetOrdinal("friendlyNameFrom");
                var treeGroupOrd = 0;
                if (nAccountID == 0)
                {
                    treeGroupOrd = reader.GetOrdinal("TreeGroup");
                }
                #endregion Ordinals

                while (reader.Read())
                {
                    #region Field data

                    fieldID = reader.GetGuid(fieldID_ordID);
                    tableID = reader.GetGuid(tableID_ordID);
                    fieldName = reader.GetString(fieldName_ordID);
                    fieldType = reader.GetString(fieldType_ordID);
                    description = reader.IsDBNull(description_ordID) ? string.Empty : reader.GetString(description_ordID);
                    comment = reader.IsDBNull(comment_ordID) ? string.Empty : reader.GetString(comment_ordID);
                    normalView = reader.GetBoolean(normalview_ordID);
                    idField = reader.GetBoolean(idField_ordID);
                    viewGroupID = reader.IsDBNull(viewGroupID_ordID) ? Guid.Empty : reader.GetGuid(viewGroupID_ordID);
                    genList = reader.GetBoolean(genList_ordID);
                    width = reader.GetInt32(width_ordID);
                    canTotal = reader.GetBoolean(canTotal_ordID);
                    printOut = reader.GetBoolean(printOut_ordID);
                    valueList = reader.GetBoolean(valueList_ordID);
                    allowImport = reader.GetBoolean(allowImport_ordID);
                    mandatory = reader.GetBoolean(mandatory_ordID);
                    amendedOn = reader.IsDBNull(amendedOn_ordID) ? new DateTime(1900, 1, 1) : reader.GetDateTime(amendedOn_ordID);
                    lookupTableID = reader.IsDBNull(lookupTable_ordID) ? Guid.Empty : reader.GetGuid(lookupTable_ordID);
                    lookUpFieldID = reader.IsDBNull(lookupField_ordID) ? Guid.Empty : reader.GetGuid(lookupField_ordID);
                    useForLookup = reader.GetBoolean(useForLookup_ordID);
                    workflowUpdate = reader.GetBoolean(workflowUpdate_ordID);
                    workflowSearchable = reader.GetBoolean(workflowSearchable_ordID);
                    length = reader.IsDBNull(length_ordID) ? 0 : reader.GetInt32(length_ordID);
                    reLabel = reader.GetBoolean(reLabel_ordID);
                    reLabelParam = reader.IsDBNull(reLabelParam_ordID) ? string.Empty : reader.GetString(reLabelParam_ordID);
                    fieldFrom = (cField.FieldSourceType)reader.GetInt32(fieldFrom_ordID);
                    classPropertyName = reader.IsDBNull(classPropertyName_ordID) ? string.Empty : reader.GetString(classPropertyName_ordID);
                    relatedTableID = reader.IsDBNull(relatedTableID_ordID) ? Guid.Empty : reader.GetGuid(relatedTableID_ordID);
                    fieldCat = reader.IsDBNull(fieldCategory_ordID) ? FieldCategory.ViewField : (FieldCategory)reader.GetByte(fieldCategory_ordID);
                    isForeignKey = reader.GetBoolean(isForeignKey_ordID);
                    friendlyNameTo = nAccountID > 0 || reader.IsDBNull(friendlyNameTo_ordID) ? string.Empty : reader.GetString(friendlyNameTo_ordID);
                    friendlyNameFrom = nAccountID > 0 || reader.IsDBNull(friendlyNameFrom_ordID) ? string.Empty : reader.GetString(friendlyNameFrom_ordID);
                    Guid? treeGroup = null;
                    if (nAccountID == 0 && !reader.IsDBNull(treeGroupOrd))
                    {
                        treeGroup =  reader.GetGuid(treeGroupOrd);
                    }
                    

                    #region Value List

                    if (valueList)
                    {
                        switch (fieldFrom)
                        {
                            case cField.FieldSourceType.Metabase:
                                if (listGenItems.ContainsKey(fieldID))
                                {
                                    listGenItems.TryGetValue(fieldID, out items);
                                }
                                else
                                {
                                    items = new SortedList<object, string>();
                                }
                                break;
                            case cField.FieldSourceType.CustomEntity:
                                if (listCustomEntityItems.ContainsKey(fieldID))
                                {
                                    listCustomEntityItems.TryGetValue(fieldID, out items);
                                }
                                else
                                {
                                    items = new SortedList<object, string>();
                                }
                                break;
                            case cField.FieldSourceType.Userdefined:
                                if (listUserdfinedItems.ContainsKey(fieldID))
                                {
                                    listUserdfinedItems.TryGetValue(fieldID, out items);
                                }
                                else
                                {
                                    items = new SortedList<object, string>();
                                }
                                break;
                            default:
                                items = new SortedList<object, string>();
                                break;
                        }
                    }
                    else
                    {
                        items = new SortedList<object, string>();
                    }

                    #endregion Value List

                    #endregion

                    tmpField = new cField(
                        nAccountID,
                        fieldID,
                        tableID,
                        viewGroupID,
                        lookUpFieldID,
                        relatedTableID,
                        lookupTableID,
                        fieldName,
                        fieldType,
                        description,
                        comment,
                        normalView,
                        idField,
                        genList,
                        canTotal,
                        width,
                        valueList,
                        allowImport,
                        mandatory,
                        printOut,
                        length,
                        workflowUpdate,
                        workflowSearchable,
                        reLabelParam,
                        classPropertyName,
                        items,
                        useForLookup,
                        fieldFrom,
                        fieldCat,
                        isForeignKey,
                        friendlyNameTo,
                        friendlyNameFrom,
                        treeGroup);
                    lstCachedFields.GetOrAdd(tmpField.FieldID, tmpField);
                }
                reader.Close();
            }
            lastReadFromDatabaseTicks.AddOrUpdate(nAccountID, addValueFactory: accId => lastReadFromDatabase.Ticks,
                                                  updateValueFactory: (accId, old) => lastReadFromDatabase.Ticks);

            return lstCachedFields;
        }

        private Dictionary<Guid, cField> CombineCachedDictionaries()
        {
            Dictionary<Guid, cField> lstCombinedList = this.lstCachedMetabaseFields.Concat(this.lstCachedCustomerFields).DistinctBy(y => y.Key).ToDictionary(x => x.Key, x => x.Value);
            return lstCombinedList;
        }

        private static SortedList<Guid, SortedList<object, string>> GetGenListItems(int nAccountId)
        {
            DBConnection expdata = new DBConnection(GlobalVariables.MetabaseConnectionString);
            System.Data.SqlClient.SqlDataReader reader;

            SortedList<Guid, SortedList<object, string>> lstAllGenItems = new SortedList<Guid, SortedList<object, string>>();
            SortedList<object, string> lstGenItems = new SortedList<object, string>();

            // :                        0        1          2       3
            string strSQL = "SELECT listitem, fieldid, listvalue, valuetype FROM rptlistitems";

            string listItem;
            string listValue;
            string valueType;
            Guid gFieldID;

            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    gFieldID = reader.GetGuid(1); //reader.GetOrdinal("fieldid"));
                    listValue = reader.GetString(2); //reader.GetOrdinal("listvalue"));
                    listItem = reader.GetString(0); //reader.GetOrdinal("listitem"));
                    valueType = reader.GetString(3); //reader.GetOrdinal("valuetype"));

                    if (lstAllGenItems.ContainsKey(gFieldID) == false)
                    {
                        lstAllGenItems.Add(gFieldID, new SortedList<object, string>());
                    }

                    switch (valueType)
                    {
                        case "byte":
                            lstAllGenItems[gFieldID].Add(Convert.ToByte(listValue), listItem);
                            break;
                        case "int":
                            lstAllGenItems[gFieldID].Add(Convert.ToInt32(listValue), listItem);
                            break;
                        case "smallint":
                            lstAllGenItems[gFieldID].Add(Convert.ToInt16(listValue), listItem);
                            break;
                        case "char":
                            lstAllGenItems[gFieldID].Add(listValue, listItem);
                            break;
                        default:
                            break;
                    }

                }

                reader.Close();
            }

            return lstAllGenItems;
        }

        private static SortedList<Guid, SortedList<object, string>> GetUserdefinedListItems(int nAccountID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            System.Data.SqlClient.SqlDataReader reader;

            SortedList<Guid, SortedList<object, string>> lstAllUserdefinedValues = new SortedList<Guid, SortedList<object, string>>();

            string strSQL =
                "SELECT userdefined.fieldid, userdefined_list_items.userdefineid, userdefined_list_items.item, userdefined_list_items.[order], userdefined_list_items.valueid FROM userdefined_list_items INNER JOIN userdefined ON (userdefined.userdefineid = userdefined_list_items.userdefineid) ORDER BY userdefineid, [order]";


            string listItem;
            Guid gFieldID;
            int valueID;

            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    gFieldID = reader.GetGuid(0);
                    listItem = reader.GetString(2);
                    valueID = reader.GetInt32(4);

                    if (lstAllUserdefinedValues.ContainsKey(gFieldID) == false)
                    {
                        lstAllUserdefinedValues.Add(gFieldID, new SortedList<object, string>());
                    }

                    lstAllUserdefinedValues[gFieldID].Add(valueID, listItem);
                }
                reader.Close();
            }
            return lstAllUserdefinedValues;
        }

        private static SortedList<Guid, SortedList<object, string>> GetCustomEntityListItems(int nAccountID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            System.Data.SqlClient.SqlDataReader reader;

            SortedList<Guid, SortedList<object, string>> lstAllCustomEntityItems = new SortedList<Guid, SortedList<object, string>>();

            // :                                0                                       1                               2
            string strSQL =
                "SELECT customEntityAttributes.fieldid, customEntityAttributeListItems.item, customEntityAttributeListItems.valueid FROM customEntityAttributeListItems INNER JOIN customEntityAttributes ON (customEntityAttributes.attributeid = customEntityAttributeListItems.attributeid)	ORDER BY customEntityAttributeListItems.[order]";

            string listItem;
            Guid gFieldID;
            int valueID;

            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    gFieldID = reader.GetGuid(0);
                    listItem = reader.GetString(1);
                    valueID = reader.GetInt32(2);

                    if (lstAllCustomEntityItems.ContainsKey(gFieldID) == false)
                    {
                        lstAllCustomEntityItems.Add(gFieldID, new SortedList<object, string>());
                    }

                    lstAllCustomEntityItems[gFieldID].Add(valueID, listItem);
                }
                reader.Close();
            }

            strSQL =
                "SELECT fieldid, field, (select fieldid from customEntityAttributes where 'att' + cast(customEntityAttributes.attributeid as nvarchar) = field) as donorFieldID from customFields where valuelist = 1 and fieldtype = 'N'";
            Guid gDonorFieldID;

            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    gFieldID = reader.GetGuid(0);
                    string field = reader.GetString(1);
                    gDonorFieldID = reader.GetGuid(2);

                    if (lstAllCustomEntityItems.ContainsKey(gDonorFieldID))
                    {
                        if (!lstAllCustomEntityItems.ContainsKey(gFieldID))
                        {
                            lstAllCustomEntityItems.Add(gFieldID, new SortedList<object, string>());
                        }

                        lstAllCustomEntityItems[gFieldID] = lstAllCustomEntityItems[gDonorFieldID];
                    }
                }
                reader.Close();
            }

            return lstAllCustomEntityItems;
        }

        /// <summary>
        /// Get a field definition by its Field ID
        /// </summary>
        /// <param name="fieldID"></param>
        /// <returns></returns>
        public cField GetFieldByID(Guid fieldID)
        {
            cField reqField = null;
            if (fieldID != Guid.Empty)
            {
                if (!this.lstCachedMetabaseFields.TryGetValue(fieldID, out reqField))
                {
                    this.lstCachedCustomerFields.TryGetValue(fieldID, out reqField);
                }
            }

            return reqField;
        }

        /// <summary>
        /// Gets the database field length of a field
        /// </summary>
        /// <param name="fieldID">field ID to retrieve length for</param>
        /// <returns>Length of field in characters</returns>
        public int GetFieldSize(Guid fieldID)
        {
            cField reqField = this.GetFieldByID(fieldID);

            int length = 50;
            if (reqField != null)
            {
                length = reqField.Length;
            }

            return length;
        }

        /// <summary>
        /// Gets the database field length of a field
        /// </summary>
        /// <param name="tableName">Table name containing field</param>
        /// <param name="fieldName">Field name to retrieve length for</param>
        /// <returns>Length of field in characters</returns>
        public int GetFieldSize(string tableName, string fieldName)
        {
            cField reqField = GetFieldByTableAndFieldName(tableName, fieldName);
            return this.GetFieldSize(reqField.FieldID);
        }

        public cField getFieldByFieldName(string fieldName)
        {
            cField reqField = (from x in this.lstCachedMetabaseFields.Values where x.FieldName.ToLower() == fieldName.ToLower() select x).FirstOrDefault();

            if (reqField == null)
            {
                reqField = (from x in this.lstCachedCustomerFields.Values where x.FieldName.ToLower() == fieldName.ToLower() select x).FirstOrDefault();
            }

            return reqField;
        }

        /// <summary>
        /// Gets all fields for a given Table ID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public List<cField> GetFieldsByTableID(Guid tableID)
        {
            Guid userdefinedtableid = Guid.Empty;
            switch (tableID.ToString())
            {
                case "618db425-f430-4660-9525-ebab444ed754":
                    userdefinedtableid = new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62");
                    break;
            }

            List<cField> lstTableFields =
                (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && (x.GetParentTable().TableID == tableID || x.GetParentTable().TableID == userdefinedtableid) select x).ToList();

            lstTableFields.AddRange((from x in this.lstCachedCustomerFields.Values where x.GetParentTable() != null && (x.GetParentTable().TableID == tableID || x.GetParentTable().TableID == userdefinedtableid) select x).ToList());

            return lstTableFields;
        }

        /// <summary>
        /// Gets all fields for a given Table ID
        /// </summary>
        /// <param name="tableID">The table id</param>
        /// <returns>list of fields for the table</returns>
        public List<cField> GetFieldsByTableIDForViews(Guid tableID)
        {
            List<cField> lstTableFields = this.lstCachedMetabaseFields.Values.Where(
                x =>
                    x.GetParentTable() != null
                    && (x.GetParentTable().TableID == tableID)
                    && !x.FieldName.ToLower().EndsWith("password")
                    && x.FieldType != "L").ToList();

            List<cField> list =
                this.lstCachedCustomerFields.Values.Where(
                    x => (x.GetParentTable() != null && (x.GetParentTable().TableID == tableID)) && ((x.GetRelatedTable() == null) || (x.GetRelatedTable() != null && x.IsForeignKey)) && x.FieldType != "L").ToList();
            lstTableFields.AddRange(list);

            return lstTableFields;
        }

        /// <summary>
        /// Gets a field by its table name and field name
        /// </summary>
        /// <param name="tableName">Table name to search</param>
        /// <param name="fieldName">field name to search for</param>
        /// <returns>a specific field that matches the name</returns>
        public cField GetFieldByTableAndFieldName(string tableName, string fieldName)
        {
            cField reqField =
                (from x in this.lstCachedMetabaseFields.Values
                 where x.GetParentTable() != null && x.GetParentTable().TableName.ToLower() == tableName.ToLower() && x.FieldName.ToLower() == fieldName.ToLower() && x.FieldType != "L"
                 select x).FirstOrDefault();

            if (reqField == null)
            {
                reqField =
                    (from x in this.lstCachedCustomerFields.Values
                     where x.GetParentTable() != null && x.GetParentTable().TableName.ToLower() == tableName.ToLower() && x.FieldName.ToLower() == fieldName.ToLower() && x.FieldType != "L"
                     select x).FirstOrDefault();
            }

            return reqField;
        }

        /// <summary>
        /// Get field details by table and description
        /// </summary>
        /// <param name="tableId">table id</param>
        /// <param name="description">table description</param>
        /// <returns></returns>
        public cField GetFieldByTableAndDescription(Guid tableId, string description)
        {
            cField reqField =
                (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.FieldName != null && x.GetParentTable().TableID == tableId && x.Description.ToLower() == description.ToLower() select x).
                    FirstOrDefault();

            if (reqField == null)
            {
                reqField =
                 (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.FieldName != null && x.GetParentTable().TableID == tableId && x.Comment.ToLower() == description.ToLower() select x).
                        FirstOrDefault();
            }
            if (reqField == null)
            {
                reqField =
                    (from x in this.lstCachedCustomerFields.Values
                     where x.GetParentTable() != null && x.GetParentTable().TableID == tableId && x.Description.ToLower() == description.ToLower() && x.FieldType != "L"
                     select x).FirstOrDefault();
            }
            if (reqField == null)
            {
                reqField =
                 (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.FieldName != null && x.Description.ToLower() == description.ToLower() select x).FirstOrDefault();
            }

            return reqField;
        }

        
       /// <summary>
        /// The get by.
        /// </summary>
        /// <param name="tableID">
        /// The table id.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="cField"/>.
        /// </returns>
        public cField GetBy(Guid tableID, string fieldName)
        {
            return this.GetFieldByTableAndFieldName(tableID, fieldName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public cField GetFieldByTableAndFieldName(Guid tableID, string fieldName)
        {
            cField reqField =
                (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.FieldName != null && x.GetParentTable().TableID == tableID && x.FieldName.ToLower() == fieldName.ToLower() select x).
                    FirstOrDefault();

            if (reqField == null && this.lstCachedCustomerFields != null)
            {
                reqField =
                    (from x in this.lstCachedCustomerFields.Values where x.GetParentTable() != null && x.FieldName != null && x.GetParentTable().TableID == tableID && x.FieldName.ToLower() == fieldName.ToLower() select x).
                        FirstOrDefault();
            }

            return reqField;
        }

        /// <summary>
        /// Get a <see cref="cField"/> using the tableId and field Name
        /// </summary>
        /// <param name="tableID">The <see cref="Guid"/>Id to match against the tableid</param>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first instance of <see cref="cField"/>That matches or null</returns>
        public cField GetCustomFieldByTableAndFieldName(Guid tableID, string fieldName)
        {
            return this.GetFieldByTableAndFieldName(tableID, fieldName);
        }

        public cField GetFieldByTableAndFieldDescription(Guid tableID, string description)
        {
            cField reqField =
                (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.GetParentTable().TableID == tableID && x.Description.ToLower() == description.ToLower() select x).FirstOrDefault();

            if (reqField == null)
            {
                reqField =
                    (from x in this.lstCachedCustomerFields.Values where x.GetParentTable() != null && x.GetParentTable().TableID == tableID && x.Description.ToLower() == description.ToLower() select x).FirstOrDefault();
            }

            return reqField;
        }

        /// <summary>
        /// Returns a list of fields for a table that are either updatable or searchable via workflows
        /// </summary>
        /// <param name="tableID">The tableID the fields belong to</param>
        /// <param name="workflowUpdatable">true if you want to update the field values or false if you just want to search them</param>
        /// <returns></returns>
        public List<sFieldBasics> GetFieldBasicsByTableID(Guid tableID, bool workflowUpdatable)
        {
            Guid userdefinedtableid = Guid.Empty;
            switch (tableID.ToString())
            {
                case "618db425-f430-4660-9525-ebab444ed754": //employees
                    userdefinedtableid = new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62");
                    break;
            }

            List<sFieldBasics> lst = (from x in this.CombineCachedDictionaries().Values
                                      orderby x.Description
                                      where
                                          x.GetParentTable() != null && (x.GetParentTable().TableID == tableID || x.GetParentTable().TableID == userdefinedtableid)
                                          && ((workflowUpdatable && x.WorkflowUpdate) || (!workflowUpdatable && x.WorkflowSearch))
                                      select new sFieldBasics(x.FieldID, x.Description, x.FieldType, x.FieldName)).ToList();

            return lst;
        }

        /// <summary>
        /// Returns a list of fields for a table that are either updatable or searchable and the same data type via workflows
        /// </summary>
        /// <param name="tableID">The tableID the fields belong to</param>
        /// <param name="workflowUpdatable">true if you want to update the field values or false if you just want to search them</param>
        /// <returns></returns>
        public List<sFieldBasics> GetFieldBasicsByTableIDAndDatatype(Guid tableID, bool workflowUpdatable, DataType dataType)
        {
            Guid userdefinedtableid = Guid.Empty;
            switch (tableID.ToString())
            {
                case "618db425-f430-4660-9525-ebab444ed754": //employees
                    userdefinedtableid = new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62");
                    break;
            }

            string sDataType = ConvertDataTypeToFieldType(dataType);

            List<sFieldBasics> lst = (from x in this.CombineCachedDictionaries().Values
                                      orderby x.Description
                                      where
                                          x.GetParentTable() != null && (x.GetParentTable().TableID == tableID || x.GetParentTable().TableID == userdefinedtableid)
                                          && (((workflowUpdatable && x.WorkflowUpdate) || (!workflowUpdatable && x.WorkflowSearch)) && x.FieldType == sDataType)
                                      select new sFieldBasics(x.FieldID, x.Description, x.FieldType, x.FieldName)).ToList();

            return lst;
        }

        public string[] SearchFieldByFieldID(Guid fieldID, ConditionType searchType, string searchValue, int maxResults)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.nAccountID));
            List<string> lstMatches = new List<string>();

            cField reqField = GetFieldByID(fieldID);

            if (reqField.FieldName.Contains("dbo.") == true)
            {
                return new string[] { };
            }


            string strSQL = "SELECT TOP " + maxResults + " CAST([" + reqField.GetParentTable().TableName + "].[" + reqField.FieldName + "] AS NVARCHAR(70)) AS match FROM [" + reqField.GetParentTable().TableName + "] WHERE ["
                            + reqField.FieldName + "] ";

            switch (searchType)
            {
                case ConditionType.Equals:
                    strSQL += "= @searchValue";
                    expdata.sqlexecute.Parameters.AddWithValue("@searchValue", searchValue);
                    break;

                case ConditionType.Like:
                default:
                    strSQL += "LIKE @searchValue";
                    expdata.sqlexecute.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                    break;

            }

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                string result;

                while (reader.Read())
                {
                    result = reader.GetString(reader.GetOrdinal("match"));

                    if (lstMatches.Contains(result) == false)
                    {
                        lstMatches.Add(result);
                    }
                }

                reader.Close();
            }

            return lstMatches.ToArray();
        }

        public List<cField> getLookupFields(Guid tableid)
        {
            List<cField> fields = GetFieldsByTableID(tableid).Where(x => x.UseForLookup).ToList();

            return fields;
        }

        /// <summary>
        /// Get a field by its class property name
        /// </summary>
        /// <param name="tableid"></param>
        /// <param name="classPropertyName"></param>
        /// <returns></returns>
        public cField getFieldByTableAndClassPropertyName(Guid tableid, string classPropertyName)
        {
            cField fld =
                (from x in this.lstCachedMetabaseFields.Values where x.GetParentTable() != null && x.GetParentTable().TableID == tableid && x.ClassPropertyName.ToLower() == classPropertyName.ToLower() select x).FirstOrDefault
                    ();

            if (fld == null)
            {
                fld =
                    (from x in this.lstCachedCustomerFields.Values where x.GetParentTable() != null && x.GetParentTable().TableID == tableid && x.ClassPropertyName.ToLower() == classPropertyName.ToLower() select x).
                        FirstOrDefault();
            }

            return fld;
        }

        public SortedList<string, cField> getFieldsByViewGroup(Guid id)
        {
            Dictionary<string, cField> lst = (from x in this.CombineCachedDictionaries().Values
                                              orderby x.Description
                                              where
                                                  x.ViewGroupID == id && x.Width > 0
                                                  // Strip out savedexpensesGrid from being returned.
                                                  && x.TableID != new Guid("64421BB4-88EE-40C4-B87B-C37548F2A780")
                                                  &&
                                                  (x.FieldSource != cField.FieldSourceType.CustomEntity
                                                   || (x.FieldSource == cField.FieldSourceType.CustomEntity && (!x.IsForeignKey && x.GetRelatedTable() == null) || x.IsForeignKey))
                                              // final clause excludes 1:n fields for custom entities
                                              select x).DistinctBy(y => y.Description).ToDictionary(a => a.Description);

            SortedList<string, cField> tmp = new SortedList<string, cField>(lst);


            return tmp;
        }

        /// <inheritdoc />
        public SortedList<Guid, cField> getAllFieldsByViewGroup(Guid id)
        {
            Dictionary<Guid, cField> lst = (from x in this.CombineCachedDictionaries().Values
                                            orderby x.Description
                                            where
                                                x.ViewGroupID == id && x.Width > 0
                                                &&
                                                (x.FieldSource != cField.FieldSourceType.CustomEntity
                                                 || (x.FieldSource == cField.FieldSourceType.CustomEntity && (!x.IsForeignKey && x.GetRelatedTable() == null) || x.IsForeignKey))
                                            // final clause excludes 1:n fields for custom entities
                                            select x).DistinctBy(y => y.FieldID).ToDictionary(a => a.FieldID);

            SortedList<Guid, cField> tmp = new SortedList<Guid, cField>(lst);


            return tmp;
        }

        /// <summary>
        /// Get all the fields that relate to the target table.
        /// </summary>
        /// <param name="relatedTableId"></param>
        /// <returns></returns>
        public List<cField> GetAllRelatedFields(Guid relatedTableId)
        {
            return this.CombineCachedDictionaries().Values.Where(field => field.RelatedTableID == relatedTableId && field.TableID != relatedTableId && field.FieldName.ToLower().EndsWith("id")).ToList();
        }

        public List<cField> getPrintoutFields()
        {
            List<cField> lst =
                (from x in this.lstCachedMetabaseFields.Values where x.PrintOut && (!x.IDField || (!x.IDField && x.GetParentTable() != null && x.GetParentTable().TableName == "userdefined_employees")) select x).ToList();

            lst.AddRange(
                (from x in this.lstCachedCustomerFields.Values where x.PrintOut && (!x.IDField || (!x.IDField && x.GetParentTable() != null && x.GetParentTable().TableName == "userdefined_employees")) select x).ToList());

            return lst;
        }

        public List<cField> getFieldsForImport(Guid tableid)
        {
            List<cField> lst = (from x in this.lstCachedMetabaseFields.Values where x.TableID == tableid && x.AllowImport && x.GetLookupTable() == null select x).ToList();

            lst.AddRange((from x in this.lstCachedCustomerFields.Values where x.TableID == tableid && x.AllowImport && x.GetLookupTable() == null select x).ToList());

            return lst;
        }

        public SortedList<string, string> getFieldsForImport(cTable table, bool withLookup)
        {
            List<cField> lookupfields;
            SortedList<string, string> lst = new SortedList<string, string>();
            foreach (cField field in this.CombineCachedDictionaries().Values)
            {
                if ((field.TableID == table.TableID || field.TableID == table.UserDefinedTableID) && field.AllowImport)
                {
                    if (field.GetLookupTable() != null)
                    {
                        if (withLookup)
                        {
                            //add lookup fields
                            lookupfields = getLookupFields(field.GetLookupTable().TableID);
                            foreach (cField lookupfield in lookupfields)
                            {
                                if (!lst.ContainsKey(field.Description))
                                {
                                    lst.Add(field.Description + "<lookup using " + lookupfield.Description + ">", field.FieldID + "," + lookupfield.FieldID);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!lst.ContainsKey(field.Description))
                        {
                            lst.Add(field.Description, field.FieldID.ToString());
                        }
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Field Description property
        /// </summary>
        /// <param name="name">the <see cref="string"/>field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        public cField getFieldByName(string name)
        {
            return this.GetFieldByName(name, false);
        }

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Field Description property and optionally the fieldname properrty
        /// </summary>
        /// <param name="name">the <see cref="string"/>field name to match</param>
        /// <param name="includeCheckForFieldName">If true, also attempt to match the fieldname to the "name" parameter</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        public cField GetFieldByName(string name, bool includeCheckForFieldName)
        {
            cField fld = (from x in this.lstCachedMetabaseFields.Values where x.Description == name select x).FirstOrDefault();

            if (fld == null)
            {
                fld = (from x in this.lstCachedCustomerFields.Values where x.Description == name || (includeCheckForFieldName && x.FieldName == name) select x).FirstOrDefault();
            }
            return fld;
        }

        /// <summary>
        /// Get fields in the form of sFieldBasics structure for a given table
        /// </summary>
        /// <param name="tableId">Table ID to retreive fields for</param>
        /// <param name="dataType">Data type defined in XML template of field type to match</param>
        /// <param name="includeIdField">Does the table's ID field need to be included in the returned field collection</param>
        /// <returns>List of basic field info</returns>
        public List<sFieldBasics> GetFieldBasicsByTableIDWithUserdefined(Guid tableId, DataType dataType, bool includeIdField = false)
        {
            Dictionary<Guid, cField> allFields = this.CombineCachedDictionaries();

            string sDataType = ConvertDataTypeToFieldType(dataType);

            List<sFieldBasics> fields = (from x in allFields.Values
                                         orderby x.Description
                                         where ((!includeIdField && !x.IDField) || includeIdField) && x.TableID == tableId && x.FieldType == sDataType
                                         select new sFieldBasics(x.FieldID, x.Description, x.FieldType, x.FieldName)).ToList();

            return fields;
        }

        /// <summary>
        /// Switches from XML template data type definition to cFields fieldtype
        /// </summary>
        /// <param name="dataType">XML Template Field Type</param>
        /// <returns>cField field type</returns>
        public static string ConvertDataTypeToFieldType(DataType dataType)
        {
            string sDataType = string.Empty;

            switch (dataType)
            {
                case DataType.booleanVal:
                    sDataType = "X";
                    break;
                case DataType.currencyVal:
                    sDataType = "C";
                    break;
                case DataType.dateVal:
                    sDataType = "D";
                    break;
                case DataType.decimalVal:
                    sDataType = "M";
                    break;
                case DataType.floatVal:
                    sDataType = "F";
                    break;
                case DataType.functionDecimalVal:
                    sDataType = "FD";
                    break;
                case DataType.functionStringVal:
                    sDataType = "FS";
                    break;
                case DataType.intVal:
                    sDataType = "N";
                    break;
                case DataType.stringVal:
                    sDataType = "S";
                    break;
                case DataType.longVal:
                    sDataType = "B";
                    break;
                default:
                    break;
            }

            return sDataType;
        }

        /// <summary>
        /// Translates the fieldType to an actual database type
        /// </summary>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> that represents the database type.
        /// </returns>
        public static string TranslateFieldTypeToSqlType(string fieldType)
        {
            string sqlType = string.Empty;
            switch (fieldType)
            {
                case "A":
                case "C":
                case "FC":
                case "M":
                    sqlType = "money";
                    break;
                case "D":
                case "DT":
                case "T":
                    sqlType = "datetime";
                    break;
                case "F":
                case "FD":
                    sqlType = "decimal";
                    break;
                case "FI":
                case "I":
                case "N":
                    sqlType = "int";
                    break;
                case "S":
                case "FS":
                case "LT":
                    sqlType = "nvarchar(max)";
                    break;
                case "FU":
                case "G":
                case "U":
                    sqlType = "uniqueidentifier";
                    break;
                case "​FX":
                case "X":
                case "Y":
                    sqlType = "bit";
                    break;
                case "B":
                    sqlType = "bigint";
                    break;
                case "VB":
                    sqlType = "varbinary";
                    break;
                default:
                    sqlType = "UNKNOWN!!!!!!";
                    break;
            }

            return sqlType;
        }

        /// <summary>
        /// Converts fields required for accessrole to restrict/allow fields to be reported on.
        /// </summary>
        /// <param name="list">
        /// The list of fields which can be reportable.
        /// </param>
        /// <param name="selectedFields">
        /// The selected fields which needs to be restricted/allowed only.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/> of the reportable fields for the accessrole grid.
        /// </returns>
        public DataSet ListFieldsToDataSet(IEnumerable<cField> list, ICollection<Guid> selectedFields)
        {
            var table = new DataTable();
            var dataSet = new DataSet();
            table.Columns.Add("fieldid", typeof(Guid));
            table.Columns.Add("Field Name");
            table.Columns.Add("Description");
            table.Columns.Add("exclusiontype", typeof(int));
            foreach (var array in list)
            {
                table.Rows.Add(array.FieldID, array.Description, array.Comment, selectedFields.Contains(array.FieldID) ? 1 : 0);
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        #endregion Methods
    }
}
