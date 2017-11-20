
namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Tools;

    /// <summary>
    /// user defined fields repository 
    /// </summary>
    public class UserDefinedFieldsRepository
    {
        /// <summary>
        /// The populate user defined fields
        /// </summary>
        /// <returns></returns>
        public static List<UserDefinedFields> PopulateUserDefinedFields()
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            List<UserDefinedFields> UDFS = new List<UserDefinedFields>();
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(UserDefinedFields.SqlItems))
            {
                #region set database columns
                int userdefinrdidOrdinal = reader.GetOrdinal("userdefineid");
                int attributeNameOrdinal = reader.GetOrdinal("attribute_name");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int tooltipOrdinal = reader.GetOrdinal("tooltip");
                int mandatoryOrdinal = reader.GetOrdinal("mandatory");
                int fieldTypeOrdinal = reader.GetOrdinal("fieldtype");
                int specificOrdinal = reader.GetOrdinal("specific");
                int orderOrdinal = reader.GetOrdinal("order");
                int maxLengthOrdinal = reader.GetOrdinal("maxlength");
                int formatOrdinal = reader.GetOrdinal("format");
                int defaultValueOrdinal = reader.GetOrdinal("defaultvalue");
                int precisionOrdinal = reader.GetOrdinal("precision");
                int tableIDOrdinal = reader.GetOrdinal("tableid");
                int fieldIDOrdinal = reader.GetOrdinal("fieldid");
                int hyperlinkTextOrdinal = reader.GetOrdinal("hyperlinkText");
                int hyperlinkPathOrdinal = reader.GetOrdinal("hyperlinkPath");
                //int groupIDOrdinal = reader.GetOrdinal("groupID");
                //int relatedTableOrdinal = reader.GetOrdinal("relatedTable");
                //int allowSearchOrdinal = reader.GetOrdinal("allowSearch");
                #endregion

                while (reader.Read())
                {

                    FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                    int attributeid = reader.GetInt32(userdefinrdidOrdinal);

                    UserDefinedFields UDFField;
                    if (type == FieldType.List)
                    {
                        UserDefinedFieldTypeList userDefinedListAttributes = new UserDefinedFieldTypeList();
                        PopulateListItemsForUDF(ref userDefinedListAttributes, attributeid);
                        UDFField = userDefinedListAttributes;
                    }
                    else
                    {
                        UDFField = new UserDefinedFields();
                    }

                    #region values

                    UDFField.DisplayName = reader.GetString(attributeNameOrdinal);
                    UDFField._description = reader.GetString(descriptionOrdinal);
                    UDFField.Order = reader.GetInt32(orderOrdinal);
                    // UDFField._createdBy = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
                    UDFField._description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                    UDFField._tooltip = reader.IsDBNull(tooltipOrdinal) ? null : reader.GetString(tooltipOrdinal);
                    UDFField._mandatory = reader.GetBoolean(mandatoryOrdinal);
                    UDFField._fieldType = type;
                    UDFField._maxLength = reader.IsDBNull(maxLengthOrdinal) ? (int?)null : reader.GetInt32(maxLengthOrdinal);
                    UDFField._format = reader.IsDBNull(formatOrdinal) ? (short)Format.None : reader.GetByte(formatOrdinal);
                    UDFField._defaultValue = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal);
                    UDFField._precision = reader.IsDBNull(precisionOrdinal) ? (short?)null : reader.GetByte(precisionOrdinal);
                    UDFField.HyperLinkText = reader.IsDBNull(hyperlinkTextOrdinal) ? null : reader.GetString(hyperlinkTextOrdinal);
                    UDFField.HyperLinkPath = reader.IsDBNull(hyperlinkPathOrdinal) ? null : reader.GetString(hyperlinkPathOrdinal);
                    UDFField.FieldId = reader.GetGuid(fieldIDOrdinal);
                    UDFField.TableId = reader.GetGuid(tableIDOrdinal);
                    #endregion
                    UDFS.Add(UDFField);
                }
                reader.Close();
            }
            return UDFS;
        }

        /// <summary>
        /// the populate list items for udf
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="customEntityId"></param>
        public static void PopulateListItemsForUDF(ref UserDefinedFieldTypeList attribute, int customEntityId)
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT [userdefineid] ,[item], [order], [archived] FROM userdefined_list_items WHERE userdefineid = @attributeid";
            db.sqlexecute.Parameters.AddWithValue("@attributeid", customEntityId);

            List<CustomEntitiesUtilities.EntityListItem> customEntityListItems = new List<CustomEntitiesUtilities.EntityListItem>();
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                int attributeIdOrdinal = reader.GetOrdinal("userdefineid");
                int itemOrdinal = reader.GetOrdinal("item");
                int orderOrdinal = reader.GetOrdinal("order");
                int archivedOrdinal = reader.GetOrdinal("archived");

                while (reader.Read())
                {
                    int attributeId = reader.GetInt32(attributeIdOrdinal);
                    string text = reader.IsDBNull(itemOrdinal) ? null : reader.GetString(itemOrdinal);
                    int order = reader.GetInt32(orderOrdinal);
                    bool archived = reader.GetBoolean(archivedOrdinal);
                    customEntityListItems.Add(new CustomEntitiesUtilities.EntityListItem(attributeId, text, order, archived));
                }
            }
            attribute.UserDefinedFieldListItems = customEntityListItems;
        }

        /// <summary>
        /// The delete user defined fields
        /// </summary>
        /// <param name="UDFToDelete"></param>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        public static int DeleteUserDefinedField(UserDefinedFields UDFToDelete, ProductType executingProduct)
        {

            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            int result = 0;

            if (UDFToDelete._attributeid > 0)
            {
                UDFToDelete._createdBy = AutoTools.GetEmployeeIDByUsername(executingProduct);
                db.sqlexecute.Parameters.AddWithValue("@userdefineid", UDFToDelete._attributeid);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", UDFToDelete._createdBy);
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

                db.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Int);
                db.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;
                db.ExecuteProc("deleteUserdefined");
                result = (int)db.sqlexecute.Parameters["@returnVal"].Value;
                db.sqlexecute.Parameters.Clear();
            }

            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()");
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate);

            return result;
        }

        /// <summary>
        /// the create user defined fields
        /// </summary>
        /// <param name="UDFToSave"></param>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        public static int CreateUserDefinedField(UserDefinedFields UDFToSave, ProductType executingProduct)
        {
            UDFToSave._attributeid = 0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            UDFToSave._createdBy = AutoTools.GetEmployeeIDByUsername(executingProduct);
            db.sqlexecute.Parameters.AddWithValue("@userdefineid", UDFToSave._attributeid);
            db.sqlexecute.Parameters.AddWithValue("@attributename", UDFToSave.DisplayName);
            db.sqlexecute.Parameters.AddWithValue("@displayname", UDFToSave.DisplayName);
            db.sqlexecute.Parameters.AddWithValue("@defaultvalue", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@relatedTable", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@maxRows", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@precision", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@hyperlinkText", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@hyperlinkPath", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@groupID", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", UDFToSave._createdBy);
            db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@specificItem", "False");
            db.sqlexecute.Parameters.AddWithValue("@allowSearch", "False");
            db.sqlexecute.Parameters.AddWithValue("@acDisplayField", DBNull.Value);

            if (string.IsNullOrEmpty(UDFToSave._description))
            {
                db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@description", UDFToSave._description);
            }
            if (string.IsNullOrEmpty(UDFToSave._tooltip))
            {
                db.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@tooltip", UDFToSave._tooltip);
            }
            db.sqlexecute.Parameters.AddWithValue("@fieldtype", (byte)UDFToSave._fieldType);
            db.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(UDFToSave._mandatory));

            if (string.IsNullOrEmpty(UDFToSave._modifiedBy))
            {
                db.sqlexecute.Parameters.AddWithValue("@userid", UDFToSave._createdBy);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@userid", UDFToSave._modifiedBy);
            }
            db.sqlexecute.Parameters.AddWithValue("@maxlength", DBNull.Value);

            switch (UDFToSave._fieldType)
            {
                case FieldType.Text:
                    if (UDFToSave._maxLength != null)
                    {
                        db.sqlexecute.Parameters["@maxlength"].Value = UDFToSave._maxLength;
                    }
                    db.sqlexecute.Parameters.AddWithValue("@format", (byte)UDFToSave._format);

                    break;
                case FieldType.Integer:
                case FieldType.Number:
                    db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                    if (UDFToSave._precision == null)
                    {
                        db.sqlexecute.Parameters["@precision"].Value = DBNull.Value;
                    }
                    else
                    {
                        db.sqlexecute.Parameters["@precision"].Value = UDFToSave._precision;
                    }
                    break;
                //DateTime
                case FieldType.DateTime:
                    db.sqlexecute.Parameters.AddWithValue("@format", (byte)UDFToSave._format);
                    break;
                //List
                case FieldType.List:
                    if (UDFToSave._format != null)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@format", (byte)UDFToSave._format);

                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                    }
                    break;
                //Yes/No
                case FieldType.TickBox:
                    db.sqlexecute.Parameters["@defaultvalue"].Value = UDFToSave._defaultValue;
                    db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                    break;
                //Currency
                case FieldType.Currency:
                    db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                    db.sqlexecute.Parameters["@precision"].Value = DBNull.Value;
                    break;
                //LargeText
                case FieldType.LargeText:
                    db.sqlexecute.Parameters.AddWithValue("@format", (byte)UDFToSave._format);
                    break;
                case FieldType.Hyperlink:
                    db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                    db.sqlexecute.Parameters["@hyperlinkText"].Value = UDFToSave.HyperLinkText;
                    db.sqlexecute.Parameters["@hyperlinkPath"].Value = UDFToSave.HyperLinkPath;
                    break;
            }
            db.sqlexecute.Parameters.AddWithValue("@order", UDFToSave.Order);
            db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            db.sqlexecute.Parameters.AddWithValue("@tableid", UDFToSave.TableId);
            db.sqlexecute.Parameters.AddWithValue("@allowEmployeeToPopulate", UDFToSave.allowEmployeeToPopulate);
            db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveUserDefinedField");
            UDFToSave._attributeid = (int)db.sqlexecute.Parameters["@identity"].Value;
            UDFToSave.FieldId = GetFieldIDbyUserdefinedID(UDFToSave._attributeid, executingProduct);
            db.sqlexecute.Parameters.Clear();
            if (UDFToSave._fieldType == FieldType.List)
            {
                List<CustomEntitiesUtilities.EntityListItem> items = ((UserDefinedFieldTypeList)UDFToSave).UserDefinedFieldListItems;
                foreach (CustomEntitiesUtilities.EntityListItem item in items)
                {
                    item._attributeId = UDFToSave._attributeid;
                    int listItemId = InsertUDFListItems(db, item, (UserDefinedFieldTypeList)UDFToSave);
                    item._valueid = listItemId;
                }
            }

            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()");
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate);
            return UDFToSave._attributeid;
        }

        /// <summary>
        /// Inserts the list items for a List UDF
        /// </summary>
        /// <param name="db">Database connection</param>
        /// <param name="item">List item to insert</param>
        /// <param name="listAttributeInfo">UDF attribute</param>
        /// <returns>return id if successful</returns>
        public static int InsertUDFListItems(DBConnection db, CustomEntitiesUtilities.EntityListItem item, UserDefinedFieldTypeList listAttributeInfo)
        {
            item._valueid = 0;
            db.sqlexecute.Parameters.AddWithValue("@valueid", item._valueid);
            db.sqlexecute.Parameters.AddWithValue("@userdefineid", item._attributeId);
            db.sqlexecute.Parameters.AddWithValue("@order", item._order);
            db.sqlexecute.Parameters.AddWithValue("@item", item._textItem);
            db.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters.AddWithValue("@archived ", item._archived);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", listAttributeInfo._createdBy);
            if (listAttributeInfo._delegateId == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", listAttributeInfo._delegateId);
            }
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            db.ExecuteProc("addUserDefinedListItem");
            int result = (int)db.sqlexecute.Parameters["@identity"].Value;
            db.sqlexecute.Parameters.Clear();
            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()");
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate);
            return result;
        }   

        /// <summary>
        /// get field id by user defined id
        /// </summary>
        /// <param name="UserdefineID"></param>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        public static Guid GetFieldIDbyUserdefinedID(int UserdefineID, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select fieldid from userdefined where userdefineid =@userdefineId";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@userdefineId", UserdefineID);
            Guid displayfield = dbex_CodedUI.getGuidValue(strSQL2);

            return displayfield;
        }
    }
}
