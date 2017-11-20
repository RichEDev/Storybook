using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using Microsoft.SqlServer.Server;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
using System.Runtime.Serialization;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields;
using System.Web.Security;


namespace Auto_Tests.Tools
{
    public class CustomEntitiesUtilities
    {

        public static int CreateCustomEntityAttribute(CustomEntity parentEntity, CustomEntityAttribute attributeToSave, ProductType executingProduct)
        {
            attributeToSave.OldAttributeId = attributeToSave._attributeid;
            attributeToSave._attributeid = 0;
            DBConnection db = null;
          
            if (parentEntity != null && attributeToSave != null)
            {

                db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
                db.sqlexecute.Parameters.AddWithValue("@entityid", parentEntity.entityId);
                db.sqlexecute.Parameters.AddWithValue("@attributeid", attributeToSave._attributeid);
                db.sqlexecute.Parameters.AddWithValue("@displayname", attributeToSave.DisplayName);
                db.sqlexecute.Parameters.AddWithValue("@defaultvalue", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@precision", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@workflowid", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@boolAttribute", false);
                db.sqlexecute.Parameters.AddWithValue("@displayInMobile", attributeToSave.EnableForMobile);

                if (string.IsNullOrEmpty(attributeToSave._description))
                {
                    db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@description", attributeToSave._description);
                }
                if (string.IsNullOrEmpty(attributeToSave._tooltip))
                {
                    db.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@tooltip", attributeToSave._tooltip);
                }
                db.sqlexecute.Parameters.AddWithValue("@fieldtype", (byte)attributeToSave._fieldType);
                db.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(attributeToSave._mandatory));

                if (string.IsNullOrEmpty(attributeToSave._modifiedBy))
                {
                    db.sqlexecute.Parameters.AddWithValue("@userid", attributeToSave._createdBy);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@userid", attributeToSave._modifiedBy);
                }
                db.sqlexecute.Parameters.AddWithValue("@maxlength", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@commentText", DBNull.Value);

                db.sqlexecute.Parameters.AddWithValue("@isauditidentity", attributeToSave._isAuditIdenity);
                db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                db.sqlexecute.Parameters.AddWithValue("@related_entityid", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@isunique", attributeToSave._isUnique);
                db.sqlexecute.Parameters.AddWithValue("@populateExistingWithDefault", false);
                db.sqlexecute.Parameters.AddWithValue("@triggerAttributeId", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@triggerJoinViaId", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@triggerDisplayFieldId", DBNull.Value);
                db.sqlexecute.Parameters.Add("@relatedTable", DBNull.Value);

                db.sqlexecute.Parameters.AddWithValue("@relationshipType", 0);
                switch (attributeToSave._fieldType)
                {
                    case FieldType.Text:
                        if (attributeToSave._maxLength != null)
                        {
                            db.sqlexecute.Parameters["@maxlength"].Value = attributeToSave._maxLength;
                        }
                        db.sqlexecute.Parameters.AddWithValue("@format", (byte)attributeToSave._format);

                        break;
                    case FieldType.Integer:
                    case FieldType.Number:
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        if (attributeToSave._precision == null)
                        {
                            db.sqlexecute.Parameters["@precision"].Value = DBNull.Value;
                        }
                        else
                        {
                            db.sqlexecute.Parameters["@precision"].Value = attributeToSave._precision;
                        }
                        break;
                    //DateTime
                    case FieldType.DateTime:
                        db.sqlexecute.Parameters.AddWithValue("@format", (byte)attributeToSave._format);
                        break;
                    //List
                    case FieldType.List:
                        if (attributeToSave._format != null)
                        {
                            db.sqlexecute.Parameters.AddWithValue("@format", (byte)attributeToSave._format);

                        }
                        else
                        {
                            db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        }
                        break;
                    //Yes/No
                    case FieldType.TickBox:
                        db.sqlexecute.Parameters["@defaultvalue"].Value = attributeToSave._defaultValue;
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        break;
                    //Currency
                    case FieldType.Currency:
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        db.sqlexecute.Parameters["@precision"].Value = DBNull.Value;
                        break;
                    //LargeText
                    case FieldType.LargeText:
                        db.sqlexecute.Parameters.AddWithValue("@format", (byte)attributeToSave._format);
                        break;
                    //comment
                    case FieldType.Comment:
                        db.sqlexecute.Parameters["@commentText"].Value = attributeToSave._commentText;
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        break;
                    case FieldType.Attachment:
                        db.sqlexecute.Parameters["@boolAttribute"].Value = attributeToSave.EnableImageLibrary;
                        break;
                    case FieldType.LookupDisplayField:
                        db.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
                        db.sqlexecute.Parameters["@triggerAttributeId"].Value = attributeToSave.TriggerAttributeId;
                        db.sqlexecute.Parameters["@triggerJoinViaId"].Value = attributeToSave.TriggerJoinViaId;
                        db.sqlexecute.Parameters["@triggerDisplayFieldId"].Value = attributeToSave.TriggerDisplayFieldID;
                        break;
                }

                db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                //db.sqlexecute.Parameters.Add("@fieldid", SqlDbType.UniqueIdentifier);
                //db.sqlexecute.Parameters["@fieldid"].Direction = ParameterDirection.Output;
                db.ExecuteProc("saveCustomEntityAttribute");
                attributeToSave._attributeid = (int)db.sqlexecute.Parameters["@identity"].Value;
                attributeToSave.OldFieldId = attributeToSave.FieldId;
                attributeToSave.FieldId = GetFieldIDbyFieldNameandTableID(parentEntity.tableId, attributeToSave.DisplayName, executingProduct);
                ///attributeToSave._fieldid = (Guid)db.sqlexecute.Parameters["@fieldid"].Value;
                db.sqlexecute.Parameters.Clear();
            }
            if (attributeToSave._fieldType == FieldType.List)
            {
                List<EntityListItem> items = ((cCustomEntityListAttribute)attributeToSave)._listItems;
                foreach (EntityListItem item in items)
                {
                    item._attributeId = attributeToSave._attributeid;
                    int listItemId = InsertListItems(db, item, (cCustomEntityListAttribute)attributeToSave);
                    //item._valueid = listItemId;
                }
            }
            return attributeToSave._attributeid;
        }

        public static Guid GetFieldIDbyFieldNameandTableID(Guid tableid, string displayname, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select fieldid from fields where description =@displayName and tableid = @tableId";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@displayName", displayname);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@tableId", tableid);
            Guid displayfield = dbex_CodedUI.getGuidValue(strSQL2);

            return displayfield;
        }

        public static Guid GetFieldIDbyDisplayNameandTableID(int tableid, string displayname, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select * from customEntityAttributes where display_name = @displayName and entityid = @tableId";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@displayName", displayname);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@tableId", tableid);
            Guid displayfield = dbex_CodedUI.getGuidValue(strSQL2);

            return displayfield;
        }

        public static int GetItemValueId(int attributeId, string item, DBConnection db)
        {
            string strSQL2 = "select valueid from customEntityAttributeListItems where attributeid = @attid and item = @itemValue";
            db.sqlexecute.Parameters.AddWithValue("@attid", attributeId);
            db.sqlexecute.Parameters.AddWithValue("@itemValue", item);
            int valueId = db.ExecuteScalar<int>(strSQL2);

            return valueId;
        }

        public static List<string> GetEntitiesFromDatabase(ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select distinct customEntities.plural_name from customEntityViews join customEntities on customEntities.entityid = customEntityViews.entityid order by plural_name Asc";
            List<string> EntityList = dbex_CodedUI.getStringList(strSQL2);
            return EntityList;
        }

        public static void DeleteCustomEntityAttribute(CustomEntity parentEntity, CustomEntityAttribute attributeToDelete, ProductType executingProduct)
        {
            if (parentEntity != null && attributeToDelete != null)
            {
                DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
                db.sqlexecute.Parameters.AddWithValue("@attributeid", attributeToDelete._attributeid);
                if (!string.IsNullOrEmpty(attributeToDelete._modifiedBy))
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", attributeToDelete._modifiedBy);
                else
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", attributeToDelete._createdBy);
                if (attributeToDelete._delegateId == null)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value); 
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", attributeToDelete._delegateId);
                }
                db.ExecuteProc("deleteCustomAttribute");
                db.sqlexecute.Parameters.Clear();
            }
        }

        public static int CreateCustomEntityRelationship(CustomEntity parentEntity, CustomEntityAttribute attributeToSave, CustomEntity relatedEntity, ProductType executingProduct)
        {
            if (attributeToSave.GetType() == typeof(CustomEntityAttribute))
            {
                throw new ArgumentException("CreateCustomEntityRelationship called with incorrect attribute type!!");
            }
            attributeToSave.OldAttributeId = attributeToSave._attributeid;
            int attributeid = 0;
            DBConnection db = null;

            if (parentEntity != null && attributeToSave != null)
            {
                db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
                db.sqlexecute.Parameters.AddWithValue("@entityid", parentEntity.entityId);
                db.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
                db.sqlexecute.Parameters.AddWithValue("@displayname", attributeToSave.DisplayName);
                db.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(attributeToSave._mandatory));

                if (string.IsNullOrEmpty(attributeToSave._description))
                {
                    db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@description", attributeToSave._description);
                }
                if (string.IsNullOrEmpty(attributeToSave._tooltip))
                {
                    db.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@tooltip", attributeToSave._tooltip);
                }
                if (string.IsNullOrEmpty(attributeToSave._modifiedBy))
                {
                    db.sqlexecute.Parameters.AddWithValue("@userid", attributeToSave._createdBy);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@userid", attributeToSave._modifiedBy);
                }

                db.sqlexecute.Parameters.AddWithValue("@viewid", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@relatedentity", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@maxRows", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@displayFieldID", DBNull.Value);
                db.sqlexecute.Parameters.AddWithValue("@aliasTableID", DBNull.Value);

                if (attributeToSave.GetType() == typeof(CustomEntityNtoOneAttribute))
                {
                    CustomEntityNtoOneAttribute manytoOneToSave = (CustomEntityNtoOneAttribute)attributeToSave;
                    
                    db.sqlexecute.Parameters["@maxRows"].Value = (manytoOneToSave._maxRows == null) ? 15 : manytoOneToSave._maxRows;
                    Guid displayFieldId = GetFieldId(manytoOneToSave._relationshipdisplayfield, executingProduct);
                    if (displayFieldId != Guid.Empty)
                    {
                        db.sqlexecute.Parameters["@displayFieldID"].Value = manytoOneToSave._relationshipdisplayfield;
                    }
                    else
                    {
                        manytoOneToSave._relationshipdisplayfield = manytoOneToSave._matchFieldListItems.FirstOrDefault()._fieldid;
                        db.sqlexecute.Parameters["@displayFieldID"].Value = manytoOneToSave._relationshipdisplayfield;
                    }
                    
                    db.sqlexecute.Parameters.AddWithValue("@relationshiptype", RelationshipType.ManyToOne);
                    Guid newRelatedTableId = GetTableId(manytoOneToSave._relatedTable, executingProduct);
                    if (newRelatedTableId != Guid.Empty)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@relatedtableid", manytoOneToSave._relatedTable);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@relatedtableid", relatedEntity.tableId);
                    }
                   
                    if (!string.IsNullOrEmpty(manytoOneToSave._aliastableid))
                    {
                       db.sqlexecute.Parameters["@aliasTableID"].Value = manytoOneToSave._aliastableid;
                    }
                }
                else if (attributeToSave.GetType() == typeof(CustomEntityOnetoNAttribute))
                {
                    CustomEntityOnetoNAttribute onetomanyToSave = (CustomEntityOnetoNAttribute)attributeToSave;
                    onetomanyToSave._relatedEntity = relatedEntity;
                    onetomanyToSave._viewId = relatedEntity.view[0]._viewid;
                    onetomanyToSave._relatedEntityId = relatedEntity.entityId;
                    db.sqlexecute.Parameters.AddWithValue("@relatedtableid", onetomanyToSave._relatedEntity.tableId);
                    db.sqlexecute.Parameters["@viewid"].Value = onetomanyToSave._viewId;
                    db.sqlexecute.Parameters.AddWithValue("@relationshiptype", RelationshipType.OneToMany);
                    db.sqlexecute.Parameters["@relatedentity"].Value = onetomanyToSave._relatedEntity.entityId;
                }
                db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                db.ExecuteProc("saveCustomEntityRelationship");
                attributeid = (int)db.sqlexecute.Parameters["@identity"].Value;
                attributeToSave.OldFieldId = attributeToSave.FieldId;
                attributeToSave.FieldId = GetFieldIDbyFieldNameandTableID(parentEntity.tableId, attributeToSave.DisplayName, executingProduct);
                db.sqlexecute.Parameters.Clear();
                attributeToSave._attributeid = attributeid;
                if (attributeToSave.GetType() == typeof(CustomEntityNtoOneAttribute))
                {
                    // save the match field ids
                    List<SqlDataRecord> lstMatchData = new List<SqlDataRecord>();
                    // Generate a sql GuidPK table param and pass into the stored proc
                    SqlMetaData[] tvpItems = { new SqlMetaData("ID", System.Data.SqlDbType.UniqueIdentifier) };

                    List<Guid> matchGuid = new List<Guid>();
                    int index = 0;
                    foreach (RelationshipMatchFieldListItem iterator in ((CustomEntityNtoOneAttribute)attributeToSave)._matchFieldListItems)
                    {
                        Guid fieldId = GetFieldId(iterator._fieldid, executingProduct);
                        if (fieldId == Guid.Empty)
                        {
                            matchGuid.Add(relatedEntity.attribute.FirstOrDefault().FieldId);
                            index++;
                        }
                        else
                        {
                            matchGuid.Add(iterator._fieldid);
                        }
                    }

                    foreach (Guid id in matchGuid)
                    {
                        SqlDataRecord row = new SqlDataRecord(tvpItems);
                        row.SetGuid(0, id);
                        lstMatchData.Add(row);
                    }

                    db.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
                    db.sqlexecute.Parameters.Add("@fieldIdList", System.Data.SqlDbType.Structured);
                    db.sqlexecute.Parameters["@fieldIdList"].Direction = System.Data.ParameterDirection.Input;
                    db.sqlexecute.Parameters["@fieldIdList"].Value = lstMatchData;

                    db.ExecuteProc("saveCustomEntityRelationshipMatchFields");
                }
            }
                return attributeid;
        }

        /// <summary>
        /// Refreshes the relationship for the attribute
        /// </summary>
        /// <param name="oldTableId">old table id to lookup - if there return it else returns null</param>
        /// <returns>return guid or null if not found</returns>
        public static Guid GetTableId(Guid oldTableId, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@tableid", oldTableId);
            return dbex_CodedUI.getGuidValue("SELECT tableid from tables WHERE tableid = @tableid"); 
        }

        public static Guid GetFieldId(Guid oldFieldID, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@oldFieldID", oldFieldID);
            return dbex_CodedUI.getGuidValue("SELECT fieldid from fields WHERE fieldid = @oldFieldID");
        }

        public static int CreateCustomEntityForm(CustomEntity parentEntity, CustomEntityForm formToSave, ProductType executingProduct)
        {
            formToSave.FormOldId = formToSave._formid;
            formToSave._formid = 0;
          
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@formid", formToSave._formid);
            db.sqlexecute.Parameters.AddWithValue("@entityid", parentEntity.entityId);
            db.sqlexecute.Parameters.AddWithValue("@formname", formToSave._formName);
            if (formToSave._description == "" || formToSave._description == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@description", formToSave._description);
            }
            db.sqlexecute.Parameters.AddWithValue("@showSave", formToSave._showSave);
            if (formToSave._saveButtonText == "")
            {
                db.sqlexecute.Parameters.AddWithValue("@saveButtonText", String.Empty);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@saveButtonText", formToSave._saveButtonText);
            }
            db.sqlexecute.Parameters.AddWithValue("@showSaveAndNew", formToSave._showSaveAndNew);
            if (formToSave._saveAndNewButtonText == "")
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndNewButtonText", String.Empty);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndNewButtonText", formToSave._saveAndNewButtonText);
            }
            db.sqlexecute.Parameters.AddWithValue("@showSaveAndStay", formToSave._showSaveAndStay);
            if (formToSave._saveAndStayButtonText == "")
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndStayButtonText", String.Empty);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndStayButtonText", formToSave._saveAndStayButtonText);
            }
            db.sqlexecute.Parameters.AddWithValue("@showCancel", formToSave._showCancel);
            if (formToSave._saveAndDuplicateButtonText == string.Empty || formToSave._saveAndDuplicateButtonText == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndDuplicateButtonText", String.Empty);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@saveAndDuplicateButtonText", formToSave._saveAndDuplicateButtonText);
            }
            db.sqlexecute.Parameters.AddWithValue("@showSaveAndDuplicate", formToSave._showSaveAndStay);
            if (formToSave._cancelButtonText == "")
            {
                db.sqlexecute.Parameters.AddWithValue("@cancelButtonText", String.Empty);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@cancelButtonText", formToSave._cancelButtonText);
            }
            db.sqlexecute.Parameters.AddWithValue("@showSubMenus", formToSave._showSubMenus);
            db.sqlexecute.Parameters.AddWithValue("@showBreadcrumbs", formToSave._showBreadcrumbs);
            if (formToSave._modifiedBy == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", formToSave._createdBy);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", formToSave._modifiedBy);
            }
            if (formToSave._delegateid == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", formToSave._delegateid);
            }
            db.sqlexecute.Parameters.AddWithValue("@checkDefaultVales", formToSave._checkDefaultValues);
            db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveCustomEntityForm");
            
            formToSave._formid = (int)db.sqlexecute.Parameters["@identity"].Value;
            db.sqlexecute.Parameters.Clear();

            int? delegateid = 0;
            if (formToSave._delegateid != null)
            {
                delegateid = formToSave._delegateid;
            }
            db.sqlexecute.Parameters.AddWithValue("@formid", formToSave._formid);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", formToSave._employeeid);
            db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);

            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate);
            return formToSave._formid;
        }

        public static int CreateFormTabs(int formid, CustomEntityFormTab tabToSave, ProductType executingProduct)
        {
            tabToSave._tabid = 0;
            int tabid;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            if (tabToSave != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@formid", formid);
                db.sqlexecute.Parameters.AddWithValue("@header", tabToSave._headercaption);
                db.sqlexecute.Parameters.AddWithValue("@order", tabToSave._order);
                db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                db.ExecuteProc("saveCustomEntityFormTab");
                tabid = (int)db.sqlexecute.Parameters["@identity"].Value;
                db.sqlexecute.Parameters.Clear();
                tabToSave._formid = formid;
                tabToSave._tabid = tabid;
            }
            return formid;
        }

        public static int CreateFormSection(int formid, int tabId, CustomEntityFormSection sectionToSave, ProductType executingProduct)
        {
            sectionToSave._sectionid = 0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            if (sectionToSave != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@formid", formid);
                db.sqlexecute.Parameters.AddWithValue("@header", sectionToSave._headercaption);
                db.sqlexecute.Parameters.AddWithValue("@order", sectionToSave._order);
                db.sqlexecute.Parameters.AddWithValue("@tabid", tabId);
                db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                db.ExecuteProc("saveCustomEntityFormSection");
                sectionToSave._formid = formid;
                sectionToSave._tabid = tabId;
                sectionToSave._sectionid = (int)db.sqlexecute.Parameters["@identity"].Value;
                db.sqlexecute.Parameters.Clear();
            }
            return formid;
        }

        public static int CreateFormFields(int formid, int sectionid, CustomEntityFormField fieldToSave, ProductType executingProduct)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            if (fieldToSave != null && fieldToSave.AttributeId != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@formid", formid);
                db.sqlexecute.Parameters.AddWithValue("@attributeid", fieldToSave.AttributeId);
                db.sqlexecute.Parameters.AddWithValue("@sectionid", sectionid);
                db.sqlexecute.Parameters.AddWithValue("@readonly", fieldToSave._readOnly);
                db.sqlexecute.Parameters.AddWithValue("@row", fieldToSave._row);
                db.sqlexecute.Parameters.AddWithValue("@column", fieldToSave._column);
                if (String.IsNullOrWhiteSpace(fieldToSave._labelText))
                {
                    db.sqlexecute.Parameters.AddWithValue("@labeltext", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@labeltext", fieldToSave._labelText);
                }
                if (string.IsNullOrEmpty(fieldToSave._defaultValue)) 
                {
                    db.sqlexecute.Parameters.AddWithValue("@defaultValue", DBNull.Value);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@defaultValue ", fieldToSave._defaultValue);
                }
                db.ExecuteProc("saveCustomEntityFormField");
                fieldToSave._formid = formid;
                fieldToSave._sectionid = sectionid;
                db.sqlexecute.Parameters.Clear();
            }
            return formid;
        }

        public static string GetCustomEntityNameById(int ID, ProductType executingProduct)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@entityID", ID);
            string sqlQuery = "select entity_name from customentities where entityid = @entityID";
            string entityname = db.getStringValue(sqlQuery);
            return entityname;
        }
        //Used to update the list items
        //To delete all items, ensure an empty list is passed.
        public static void UpdateListItems(DBConnection db, cCustomEntityListAttribute listAttributeInfo)
        {
            List<EntityListItem> listOfAttributes = listAttributeInfo._listItems;
            
            //Remove existing list items
            //Get all values....
            List<int> deleteValueIDs = new List<int>();
            db.sqlexecute.Parameters.AddWithValue("@attributeid", listAttributeInfo._attributeid);
            db.ExecuteSQL("delete from customEntityAttributeListItems where attributeid = @attributeid");

            //Insert new list items
            foreach (EntityListItem it in listOfAttributes)
            {
                InsertListItems(db, it, listAttributeInfo);
            }
        }

        private static int InsertListItems(DBConnection db, EntityListItem item, cCustomEntityListAttribute listAttributeInfo)
        {
            item.OldValueId = item._valueid;
            item._valueid = 0;
            db.sqlexecute.Parameters.AddWithValue("@valueid", item._valueid);
            db.sqlexecute.Parameters.AddWithValue("@attributeid", item._attributeId);
            db.sqlexecute.Parameters.AddWithValue("@order", item._order);
            db.sqlexecute.Parameters.AddWithValue("@item", item._textItem);
            db.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", listAttributeInfo._createdBy);
            db.sqlexecute.Parameters.AddWithValue("@archived ", item._archived);

            if (listAttributeInfo._delegateId == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", listAttributeInfo._delegateId);
            }
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            db.ExecuteProc("addCustomEntityListAttributeItem");
            //item._valueid = (int)db.sqlexecute.Parameters["@identity"].Value;
            item._valueid = GetItemValueId(item._attributeId, item._textItem, db);
            db.sqlexecute.Parameters.Clear();
            return item._valueid;
        }

        public static int CreateCustomEntityView(CustomEntity parentEntity, CustomEntityView viewToSave, ProductType executingProduct)
        {
            viewToSave.OldViewId = viewToSave._viewid;
            viewToSave._viewid = 0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@viewid", viewToSave._viewid);
            db.sqlexecute.Parameters.AddWithValue("@entityid", parentEntity.entityId);
            db.sqlexecute.Parameters.AddWithValue("@viewname", viewToSave._viewName);
            if (viewToSave._menuid.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@menuid", viewToSave._menuid);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@menuid", DBNull.Value);
            }
            if (viewToSave._description == "" || viewToSave._description == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@description", viewToSave._description);
            }
            if (viewToSave._menudescription == "" || viewToSave._menudescription == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@MenuDescription", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@MenuDescription", viewToSave._menudescription);
            }
            if (viewToSave._modifiedBy == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@date", viewToSave._createdOn);
                db.sqlexecute.Parameters.AddWithValue("@userid", viewToSave._createdBy);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@date", viewToSave._modifiedOn);
                db.sqlexecute.Parameters.AddWithValue("@userid", viewToSave._modifiedBy);
            }
            db.sqlexecute.Parameters.AddWithValue("@allowadd", Convert.ToByte(viewToSave._allowAdd));
            if (viewToSave._allowAdd)
            {
                db.sqlexecute.Parameters.AddWithValue("@addformid", viewToSave.addform._formid);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@addformid", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@allowedit", Convert.ToByte(viewToSave._allowEdit));
            if (viewToSave._allowEdit)
            {
                db.sqlexecute.Parameters.AddWithValue("@editformid", viewToSave.editform._formid);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@editformid", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@allowdelete", Convert.ToByte(viewToSave._allowDelete));

            if (viewToSave._viewid == 0)
            {
                db.sqlexecute.Parameters.AddWithValue("@SortColumn", DBNull.Value);

                db.sqlexecute.Parameters.AddWithValue("@SortOrder", 0);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@SortColumn", viewToSave.sortColumn._fieldID);

                db.sqlexecute.Parameters.AddWithValue("@SortOrder", viewToSave.sortColumn._sortDirection);
            }

            //int? jViaID = null;
            //if (viewToSave.sortColumn.JoinVia != null && viewToSave.sortColumn.JoinVia._joinViaID > 0)
            //{
            //    jViaID = viewToSave.sortColumn.JoinVia._joinViaID;
            //}
            //else if (viewToSave.sortColumn.JoinVia != null && viewToSave.sortColumn.JoinVia._joinViaID ==   &&
            //         viewToSave.sortColumn.JoinVia._joinViaList.Count > 0)
            //{
            //    //JoinVias clsJoinVias = new JoinVias(oCurrentUser);
            //    //jViaID = clsJoinVias.SaveJoinVia(viewToSave.sortColumn.JoinVia);
            //}
            //if (jViaID.HasValue)
            //{
            //        db.sqlexecute.Parameters.AddWithValue("@SortJoinViaID", jViaID);
            //}
            //else
            //{
                    db.sqlexecute.Parameters.AddWithValue("@SortJoinViaID", DBNull.Value);
            //}


            //--------------MenuIcon

            if (viewToSave._menuIcon == string.Empty)
            {
                db.sqlexecute.Parameters.AddWithValue("@MenuIcon", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@MenuIcon", viewToSave._menuIcon);
            }

            db.sqlexecute.Parameters.AddWithValue("@AllowApproval", Convert.ToByte(viewToSave._allowApproval));

            db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveCustomEntityView");
            viewToSave._viewid = (int)db.sqlexecute.Parameters["@identity"].Value;
            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate); db.sqlexecute.Parameters.Clear();
            return viewToSave._viewid;
        }

        public static int CreateViewFields(CustomEntityView view, CustomEntityViewField fieldToSave, ProductType executingProduct)
        {
            //fieldToSave._viewFieldId = 0;
            int? jViaID = 0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@viewid", view._viewid);
            db.sqlexecute.Parameters.AddWithValue("@fieldid", fieldToSave._fieldid);
            db.sqlexecute.Parameters.AddWithValue("@order", fieldToSave._order);
            if (fieldToSave._joinViaId.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@joinViaID", fieldToSave._joinViaId.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", view._createdBy);
            db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            db.ExecuteProc("addCustomEntityViewField");
            //fieldToSave._viewFieldId = (int)db.sqlexecute.Parameters["@identity"].Value;
            db.sqlexecute.Parameters.Clear();
            return fieldToSave._viewFieldId;
        }

        public static void CreateViewFilters(CustomEntityViewFilter filtertoSave, ProductType executingProduct, CustomEntityView view = null, CustomEntityAttribute attribute = null)
        {
            int? jViaID = 0;
            //JoinVias clsJoinVias;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            if (view != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@viewID", view._viewid);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", view._createdBy);
            }
            else 
            {
                db.sqlexecute.Parameters.AddWithValue("@viewID", DBNull.Value);
            }

            db.sqlexecute.Parameters.AddWithValue("@fieldID", filtertoSave._fieldid);
            db.sqlexecute.Parameters.AddWithValue("@order", filtertoSave._order);
            db.sqlexecute.Parameters.AddWithValue("@operator", filtertoSave._conditionType);
            db.sqlexecute.Parameters.AddWithValue("@valueOne", filtertoSave._valueOne);
            db.sqlexecute.Parameters.AddWithValue("@valueTwo", filtertoSave._valueTwo);
            if (attribute != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@attributeID", attribute._attributeid);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", attribute._createdBy);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@attributeID", DBNull.Value);
            }

            db.sqlexecute.Parameters.AddWithValue("@userdefineID", DBNull.Value);
            if (filtertoSave._joinViaId.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@joinViaID", filtertoSave._joinViaId.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
            }
            
            db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            db.ExecuteProc("addFieldFilter");
            db.sqlexecute.Parameters.Clear();
        }

        public static int SaveJoinVia(JoinVia joinVia, ProductType executingProduct)
        {
            //int savedJoinViaID;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            var lstViaIDs = new List<SqlDataRecord>();
            var pathForHashing = new StringBuilder();

            // Generate a sql Int_UniqueIdentifier_TinyInt table param and pass into the stored proc
            SqlMetaData[] viaPart = { new SqlMetaData("c1", System.Data.SqlDbType.Int), new SqlMetaData("c2", System.Data.SqlDbType.UniqueIdentifier), new SqlMetaData("c3", System.Data.SqlDbType.TinyInt) };

            foreach (KeyValuePair<int, JoinViaPart> kvp in joinVia.JoinViaParts)
            {
                // Int_UniqueIdentifier_TinyInt
                var row = new SqlDataRecord(viaPart);
                row.SetInt32(0, kvp.Key);
                row.SetGuid(1, kvp.Value.RelatedID);
                row.SetByte(2, (byte)kvp.Value._type);

                lstViaIDs.Add(row);

                // build up the comparison string for hashing
                pathForHashing.Append(kvp.Value.RelatedID);
            }

            // this allows us to compare all parts of a via path at once
            string pathHash = FormsAuthentication.HashPasswordForStoringInConfigFile(pathForHashing.ToString(), "MD5");

            expdata.sqlexecute.Parameters.Clear();
            int joinViaId = 0;
            joinVia.OLDjoinViaID = joinVia._joinViaID;
            expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", joinViaId);
            expdata.sqlexecute.Parameters.AddWithValue("@joinViaDescription", joinVia._joinViaDescription);
            expdata.sqlexecute.Parameters.AddWithValue("@joinViaPathHash", pathHash);
            expdata.sqlexecute.Parameters.Add("@joinViaParts_Int_Unique_TinyInt", System.Data.SqlDbType.Structured);
            expdata.sqlexecute.Parameters["@joinViaParts_Int_Unique_TinyInt"].Direction = System.Data.ParameterDirection.Input;
            expdata.sqlexecute.Parameters["@joinViaParts_Int_Unique_TinyInt"].Value = lstViaIDs;

            expdata.sqlexecute.Parameters.Add("@savedJoinViaID", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@savedJoinViaID"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("SaveJoinVia");

            joinViaId = (int)expdata.sqlexecute.Parameters["@savedJoinViaID"].Value;
            joinVia._joinViaID = joinViaId;
            expdata.sqlexecute.Parameters.Clear();

            // if savedJoinViaID = -1 there were MORE than one joinvia with that ID (thats not good)
            // so it couldn't decide which one to choose and won't have saved the joinvia

            return joinVia._joinViaID;
        }

        public class cCustomEntityListAttribute : CustomEntityAttribute
        {
            public List<EntityListItem> _listItems { get; set; }

            public cCustomEntityListAttribute(List<EntityListItem> listItems, int createdby, string modifiedBy, string DisplayName, string Description, string Tooltip, bool Mandatory, FieldType FieldType, DateTime Date, int MaxLength,
               Format Format, string DefaultValue, short? Precision, Guid fieldid, bool systemAttribute, int? DelegateId = null, bool IsAuditId = false, string CommentText = "", bool IsUnique = false)
                : base(createdby, modifiedBy, DisplayName, Description, Tooltip, Mandatory, FieldType, Date, MaxLength,
               Format, DefaultValue, Precision, fieldid, systemAttribute, DelegateId, IsAuditId, CommentText, IsUnique)
            {
                _listItems = listItems;
            }

            public cCustomEntityListAttribute() { }
        }
      
        public class EntityListItem 
        {
            public int OldValueId { get; set; }
            public int _valueid { get; set; }
            public int _attributeId { get; set; }
            public int _order { get; set; }
            public string _textItem { get; set; }
            public bool _archived { get; set; }


            public EntityListItem(int attributeId, string textItem, int order, bool archived, int valueid = 0)
            {
                _attributeId = attributeId;
                _textItem = textItem;
                _order = order;
                _valueid = valueid;
                _archived = archived;
            }

            public EntityListItem()
            { }

        }

        public class CustomEntityOnetoNAttribute : CustomEntityAttribute
        {
            public short? _relationshipType { get; set; }
            public CustomEntity _relatedEntity { get; set; }
            public Guid _relatedTable { get; set; }
            public int _viewId { get; set; }
            public int _entityId { get; set; }
            public int _relatedEntityId { get; set; }

            public CustomEntityOnetoNAttribute() { }

            public CustomEntityOnetoNAttribute(int createdby, string modifiedBy, string DisplayName, string Description, string Tooltip, bool Mandatory, FieldType FieldType, DateTime Date, int MaxLength,
               Format Format, string DefaultValue, short? Precision, Guid fieldid, bool systemAttribute, short? RelationshipType, int EntityId, Guid RelatedTable, CustomEntity RelatedEntity, int? DelegateId = null, bool IsAuditId = false, string CommentText = "", bool IsUnique = false)
                : base(createdby, modifiedBy, DisplayName, Description, Tooltip, Mandatory, FieldType, Date, MaxLength,
               Format, DefaultValue, Precision, fieldid, systemAttribute, DelegateId, IsAuditId, CommentText, IsUnique)
            {
                _relationshipType = RelationshipType;
                _entityId = EntityId;
                _relatedTable = RelatedTable;
                _relatedEntity = RelatedEntity;
            }
        }

        public class CustomEntityNtoOneAttribute : CustomEntityAttribute
        {

            public List<RelationshipMatchFieldListItem> _matchFieldListItems { get; set; }
            public List<Field> _baseTableFields { get; set; }
            public List<Field> _UDFFields { get; set; }
            public List<UserDefinedFields> _UDF { get; set; }
            public List<CustomEntityViewFilter> Filters { get; set; }
            public short? _relationshipType { get; set; }
            public Guid _relatedTable { get; set; }
            public Guid _relationshipdisplayfield { get; set; }
            public string _aliastableid { get; set; }
            public int? _maxRows { get; set; }
            public int? _relatedEntityId { get; set; }
            public bool _isExpanded { get; set; }
            public Field _udfFolder { get; set; }

            public CustomEntityNtoOneAttribute() { }

            public CustomEntityNtoOneAttribute(List<UserDefinedFields> UDF, int createdby, string modifiedBy, string DisplayName, string Description, string Tooltip, bool Mandatory, FieldType FieldType, DateTime Date, int MaxLength,
               Format Format, string DefaultValue, short? Precision, Guid fieldid, bool systemAttribute, short? RelationshipType, int? RelatedEntityId, Guid RelatedTable, Guid RelationshipDisplayField, string aliastableid, int? maxRows,
               List<RelationshipMatchFieldListItem> matchFieldListItems, int? DelegateId = null, bool IsAuditId = false, string CommentText = "", bool IsUnique = false, List<Field> baseTableFields = null, List<Field> UDFFields = null, Field udfFolder = null, bool isExpanded = false)
                : base(createdby, modifiedBy, DisplayName, Description, Tooltip, Mandatory, FieldType, Date, MaxLength,
               Format, DefaultValue, Precision, fieldid, systemAttribute, DelegateId, IsAuditId, CommentText, IsUnique)
            {
                _relationshipType = RelationshipType;
                _relatedEntityId = RelatedEntityId;
                _relatedTable = RelatedTable;
                _relationshipdisplayfield = RelationshipDisplayField;
                _matchFieldListItems = matchFieldListItems;
                _aliastableid = aliastableid;
                _maxRows = maxRows;
                _baseTableFields = baseTableFields;
                _isExpanded = isExpanded;
                _UDFFields = UDFFields;
                _udfFolder = udfFolder;
                _UDF = UDF;
            }

            public CustomEntityNtoOneAttribute(string displayName, Guid fieldID, short relationshipType, FieldType fieldType, Guid relatedTable, bool systemAttribute, List<Field> baseTableFields = null, List<Field> UDFFields = null, bool isExpanded = false)
            {
                DisplayName = displayName;
                FieldId = fieldID;
                _relationshipType = relationshipType;
                _fieldType = fieldType;
                _relatedTable = relatedTable;
                _baseTableFields = baseTableFields;
                _isExpanded = isExpanded;
                _UDFFields = UDFFields;
                SystemAttribute = systemAttribute;
            }
        }

        public class RelationshipMatchFieldListItem
        {
            public int _matchfieldid { get; set; }
            public int _attributeId { get; set; }
            public Guid _fieldid { get; set; }

            public RelationshipMatchFieldListItem(int attributeId, Guid fieldid, int matchfieldid)
            {
                _attributeId = attributeId;
                _matchfieldid = matchfieldid;
                _fieldid = fieldid;
            }

            public RelationshipMatchFieldListItem()
            { }
        }

        public class CustomEntityForm
        {
            public int FormOldId { get; set; }
            public int _formid { get; set; }
            public string _formName { get; set; }
            public string _description { get; set; }
            public int _employeeid { get; set; }
            public int? _delegateid { get; set; }
            public DateTime _date { get; set; }
            public bool _showBreadcrumbs { get; set; }
            public bool _showSaveAndNew { get; set; }
            public bool _showSaveAndStay { get; set; }
            public bool _showSubMenus { get; set; }
            public string _saveAndNewButtonText { get; set; }
            public string _saveButtonText { get; set; }
            public string _cancelButtonText { get; set; }
            public bool _showSave { get; set; }
            public bool _showCancel { get; set; }
            public string _saveAndStayButtonText { get; set; }
            public string _modifiedBy { get; set; }
            public string _createdBy { get; set; }
            public List<CustomEntityFormTab> tabs { get; set; }
            public bool _checkDefaultValues { get; set; }
            public string _saveAndDuplicateButtonText { get; set; }
            public bool _showSaveAndDuplicate { get; set; }
            
            public CustomEntityForm(int formID, string formName, string description, bool showSave, string saveButtonText, bool showSaveAndNew, bool showSaveAndStay, string saveAndNewButtonText, 
                string saveAndStayButtonText, bool showCancel, string cancelButtonText, bool showSubMenus, bool showBreadcrumbs, DateTime date, int employeeID, int? delegateID, string modifiedBy, string createdBy, bool checkDefaultValues)
            {
                _formid = formID;
                _formName = formName;
                _description = description;
                _employeeid = employeeID;
                _date = date;
                _showBreadcrumbs = showBreadcrumbs;
                _showSaveAndNew = showSaveAndNew;
                _showSaveAndStay = showSaveAndStay;
                _showSubMenus = showSubMenus;
                _saveAndNewButtonText = saveAndNewButtonText;
                _saveButtonText = saveButtonText;
                _cancelButtonText = cancelButtonText;
                _showSave = showSave;
                _showCancel = showCancel;
                _saveAndStayButtonText = saveAndStayButtonText;
                _modifiedBy = modifiedBy;
                _createdBy = createdBy;
                _delegateid = delegateID;
                _checkDefaultValues = checkDefaultValues;
            }

            public CustomEntityForm() { }

            public CustomEntityForm(string formName, string description)
            {
                _formName = formName;
                _description = description;
            }
        }

        public class Field : CustomEntityNtoOneAttribute
        {
            public Guid _tabledId { get; set; }
            public bool _isForeignKey { get; set; }
            //public FieldFrom _fieldFrom { get; set; }  Never used, WHY? 
            //public List<Field> _fieldsList {get; set;}

            public Field(Guid fieldID, string displayName, bool isForeignKey, FieldFrom fieldFrom, Guid tableID, Guid relatedTable)
            {
                FieldId = fieldID;
                DisplayName = displayName;
                _isForeignKey = isForeignKey;
                //_fieldFrom = fieldFrom;
                _tabledId = tableID;
                _relatedTable = relatedTable;
            }

            public Field() { }
        }

        public class CustomEntityFormTab
        {
            public int _tabid { get; set; }
            public int _formid { get; set; }
            public string _headercaption { get; set; }
            public byte _order { get; set; }
            public List<CustomEntityFormSection> sections { get; set; }


            public CustomEntityFormTab() { }

            public CustomEntityFormTab(int tabid, int formid, string headercaption, byte order)
            {
                _tabid = tabid;
                _formid = formid;
                _headercaption = headercaption;
                _order = order;
            }
        }

        public class CustomEntityFormSection
        {
            public int _sectionid { get; set; }
            public int _formid { get; set; }
            public string _headercaption { get; set; }
            public byte _order { get; set; }
            public int _tabid { get; set; }
            public List<CustomEntityFormField> fields { get; set; }

            public CustomEntityFormSection() { }

            public CustomEntityFormSection(int sectionid, int formid, string headercaption, byte order, int tabid)
            {
                _sectionid = sectionid;
                _formid = formid;
                _headercaption = headercaption;
                _order = order;
                _tabid = tabid;
            }
        }

        public class CustomEntityFormField
        {
            public int _formid { get; set; }
            public CustomEntityAttribute attribute { get; set; }
            public bool _readOnly { get; set; }
            public int _sectionid { get; set; }
            public int AttributeId { get; set; }
            public byte _column { get; set; }
            public byte _row { get; set; }
            public string _labelText { get; set; }
            public byte _columnSpan { get; set; }
            public string _defaultValue { get; set; }

            public CustomEntityFormField() { }

            public CustomEntityFormField(int formid, CustomEntityAttribute _attribute, bool breadonly, int sectionid, byte column, byte row, string labeltext = "", byte columnSpan = 1, string defaultValue = "")
            {
                _formid = formid;
                attribute = _attribute;
                _readOnly = breadonly;
                _sectionid = sectionid;
                _column = column;
                _row = row;
                _labelText = labeltext;
                _columnSpan = columnSpan;
                _defaultValue = defaultValue;
            }

            #region properties
            public byte columnSpan
            {
                get
                {
                    byte columnSpan = 1;

                    if (attribute._fieldType == FieldType.Comment || attribute._fieldType == FieldType.OTMSummary || (attribute._fieldType == FieldType.Relationship && attribute._format == (short)RelationshipType.OneToMany))
                    {
                        columnSpan = 2;
                    }

                    if (attribute._fieldType == FieldType.Text || attribute._fieldType == FieldType.LargeText)
                    {
                        switch (((CustomEntityAttribute)attribute)._format)
                        {
                            case (short)Format.FormattedTextBox:
                                columnSpan = 2;
                                break;
                            case (short)Format.Multi_line:
                                columnSpan = 2;
                                break;
                            case (short)Format.SingleLineWide:
                                columnSpan = 2;
                                break;
                            default:
                                break;
                        }
                    }

                    if (attribute._fieldType == FieldType.List)
                    {
                        if (((CustomEntityAttribute)this.attribute)._format == (short)Format.ListWide)
                        {
                            columnSpan = 2;
                        }
                    }

                    return columnSpan;
                }
            }
            #endregion
        }

        public class CustomEntityView
        {
            public int OldViewId { get; set; }
            public int _viewid { get; set; }
            public string _viewName { get; set; }
            public string _description { get; set; }
            public DateTime _createdOn { get; set; }
            public string _createdBy { get; set; }
            public DateTime? _modifiedOn { get; set; }
            public string _modifiedBy { get; set; }
            public int? _menuid { get; set; }
            public string _menudescription { get; set; }
            public bool _allowAdd { get; set; }
            public int AddFormId { get; set; }
            public CustomEntityForm addform { get; set; }
            public bool _allowEdit { get; set; }
            public int EditFormId { get; set; }
            public CustomEntityForm editform { get; set; }
            public bool _allowDelete { get; set; }
            public bool _allowApproval { get; set; }
            public GreenLightSortColumn sortColumn { get; set; }
            public List<CustomEntityViewField> fields { get; set; }
            public List<CustomEntityViewFilter> filters { get; set; }
            public string _menuIcon { get; set; }

            public CustomEntityView() { }

            public CustomEntityView(int viewid, string viewname, string description, DateTime createdon, string createdby, DateTime? modifiedon, string modifiedby, int? menuid, string menudescription,
                GreenLightSortColumn SortColumn, CustomEntityForm _addform, CustomEntityForm _editform, string menuIcon,
                bool allowedit = false, bool allowdelete = false, bool allowapproval = false, bool allowadd = false)
            {
                _viewid = viewid;
                _viewName = viewname;
                _description = description;
                _createdOn = createdon;
                _createdBy = createdby;
                _modifiedOn = modifiedon;
                _modifiedBy = modifiedby;
                _menuid = menuid;
                _menudescription = menudescription;
                _allowAdd = allowadd;
                addform = _addform;
                _allowEdit = allowedit;
                editform = _editform;
                _allowDelete = allowdelete;
                _allowApproval = allowapproval;
                _menuIcon = menuIcon;
            }
        }

        public class CustomEntityViewField
        {
            //public cField Field { get; set; }
            public int _viewFieldId { get; set; }
            public int _viewId { get; set; }
            public int _order { get; set; }
            public Guid _fieldid { get; set; }
            ///public JoinVia JoinVia { get; set; }
            public int? _joinViaId { get; set; }

            public CustomEntityViewField() { }

            public CustomEntityViewField(/*cField field,*/ int viewFieldId, int viewId, int order, Guid fieldid, int joinViaId/*JoinVia joinVia = null*/)
            {
                _viewFieldId = viewFieldId;
                _viewId = viewId;
                _order = order;
                _fieldid = fieldid;
                _joinViaId = joinViaId;
                //JoinVia = joinVia;
            }
        }

        public class CustomEntityViewFilter
        {
            //public cField _field { get; set; }
            public int? _viewId { get; set; }
            public int? AttributeId { get; set; }
            public byte _conditionType { get; set; }
            public string _valueOne { get; set; }
            public string _valueTwo { get; set; }
            public byte _order { get; set; }
            public Guid _fieldid { get; set; }
            //public JoinVia _joinVia { get; set; }
            public int? _joinViaId { get; set; }

            public int? OldJoinViaId { get; set; }

            public CustomEntityViewFilter() { }

            public CustomEntityViewFilter(/*cField field,*/int viewId, Guid fieldid, byte conditiontype, string valueOne, string valueTwo, byte order, int joinViaId/*JoinVia joinVia = null*/)
            {
                //_field = field;
                _viewId = viewId;
                _fieldid = fieldid;
                _conditionType = conditiontype;
                _valueOne = valueOne;
                _valueTwo = valueTwo;
                _order = order;
                //JoinVia = joinVia;
                _joinViaId = joinViaId;
            }
        }

        public class GreenLightSortColumn
        {
        
            public JoinVia JoinVia { get; set; }
            public int _joinviaID { get; set; }
            public int _sortDirection { get; set; }
            public Guid _fieldID { get; set; }

            public GreenLightSortColumn() { }

            public GreenLightSortColumn(Guid fieldID, int joinviaID, int sortDirection, JoinVia joinVia)
            {
                _fieldID = fieldID;
                _sortDirection = sortDirection;
                JoinVia = joinVia;
            }
        }

        public class JoinVia
       {
            public int OLDjoinViaID { get; set; }
            public int _joinViaID { get; set; }
            public string _joinViaDescription { get; set; }
            public Guid _joinViaAS { get; set; }
            public SortedList<int, JoinViaPart> JoinViaParts { get; set; }
            public string JoinViaPathHash { get; set; }
            public JoinVia() { }

            public JoinVia(int id, string description, Guid viaAS, SortedList<int, JoinViaPart> viaList = null)
            {
                _joinViaID = id;
                _joinViaDescription = description;
                _joinViaAS = viaAS;
                JoinViaParts = viaList == null ? new SortedList<int, JoinViaPart>() : viaList;
            }
        }

        public class JoinViaPart
        {
            public Guid RelatedID { get; set; }
            public IDType _type { get; set; }
            public JoinType _join { get; set; }
            public int Order { get; set; }

            public int JoinViaPartID { get; set; }
            public int JoinViaID { get; set; }

            public enum IDType
            {
                Field = 0, // IsForeignKey / Many To One / Genlist
                Table = 1 // JoinTable link / One To Many
            }

            public enum JoinType
            {
                LEFT = 0,
                INNER = 1
            }
            public JoinViaPart()
            {
            }

            public JoinViaPart(Guid relatedID, IDType idType, JoinType joinType = JoinType.LEFT)
            {
                RelatedID = relatedID;
                _type = idType;
                _join = joinType;
            }
        }

        [Serializable]
        public class CustomEntityAttribute 
        {
            public int OldAttributeId { get; set; }
            public int _attributeid { get; set; }
            /// <summary>
            /// Display name relating to attribute
            /// </summary>
            public string DisplayName { get; set; }
            public string _description { get; set; }
            public string _tooltip { get; set; }
            public bool _mandatory { get; set; }
            public FieldType _fieldType { get; set; }
            public DateTime _date { get; set; }
            public int? _maxLength { get; set; }
            public short _format { get; set; }
            public string _defaultValue { get; set; }
            public short? _precision { get; set; }
            public bool _isAuditIdenity { get; set; }
            public string _commentText { get; set; }
            public bool _isUnique { get; set; }
            public string _modifiedBy { get; set; }
            public int _createdBy { get; set; }
            public int? _delegateId { get; set; }
            public Guid OldFieldId { get; set; }
            public Guid FieldId { get; set; }
            public bool SystemAttribute { get; set; }
            public int? TriggerAttributeId { get; set; }

            public int? OldTriggerJoinViaId { get; set; }

            public int? TriggerJoinViaId { get; set; }

            public Guid? TriggerDisplayFieldID { get; set; }
            public CustomEntity ParentEntity { get; set; }
            public AttributeData attributeData { get; set; }
            public bool EnableImageLibrary { get; set; }
            public bool EnableForMobile { get; set; }

            public CustomEntityAttribute(int createdby, string modifiedBy, string Displayname, string Description, string Tooltip, bool Mandatory, FieldType FieldType, DateTime Date, int MaxLength,
               Format Format, string DefaultValue, short? Precision, Guid fieldid, bool systemAttribute, int? DelegateId = null, bool IsAuditId = false, string CommentText = "", bool IsUnique = false)
            {
                _modifiedBy = modifiedBy;
                _createdBy = createdby;
                DisplayName = Displayname;
                _description = Description;
                _tooltip = Tooltip;
                _mandatory = Mandatory;
                _fieldType = FieldType;
                _date = Date;
                _maxLength = MaxLength;
                _format = (short)Format;
                _defaultValue = DefaultValue;
                _precision = Precision;
                _isAuditIdenity = IsAuditId;
                _commentText = CommentText;
                _delegateId = DelegateId;
                _isUnique = IsUnique;
                FieldId = fieldid;
                SystemAttribute = systemAttribute;
            }

            public CustomEntityAttribute() { }

            public CustomEntityAttribute(string Displayname, string Description, FieldType FieldType, bool IsAuditId = false)
            {
                DisplayName = Displayname;
                _description = Description;
                _fieldType = FieldType;
                _isAuditIdenity = IsAuditId;
            }

            public CustomEntityAttribute(string Displayname, Guid fieldID, FieldType fieldType, bool systemAttribute)
            {
                DisplayName = Displayname;
                FieldId = fieldID;
                _fieldType = fieldType;
                SystemAttribute = systemAttribute;
            }
        }

        [Serializable]
        public class AttributeData
        {
            public string clientData1 { get; set; }
            public string clientData2 { get; set; }

            public AttributeData() { }

            public AttributeData(string _clientData1, string _clientData2)
            {
                clientData1 = _clientData1;
                clientData2 = _clientData2;
            }
        }
    }

    public enum ConditionType
    {
        [Description("[None]")]
        None = 0,
        [Description("Equals")]
        Equals = 1,
        [Description("Does Not Equal")]
        DoesNotEqual = 2,
        [Description("Greater Than")]
        GreaterThan = 3,
        [Description("Less Than")]
        LessThan = 4,
        [Description("Greater Than or Equal To")]
        GreaterThanEqualTo = 5,
        [Description("Less Than or Equal To")]
        LessThanEqualTo = 6,
        [Description("Like")]
        Like = 7,
        [Description("Between")]
        Between = 8,
        [Description("Contains Data")]
        ContainsData = 9,
        [Description("Does Not Contain Data")]
        DoesNotContainData = 10,
        [Description("Yesterday")]
        Yesterday = 11,
        [Description("Today")]
        Today,
        [Description("Tomorrow")]
        Tomorrow,
        [Description("Next 7 Days")]
        Next7Days,
        [Description("Last 7 Days")]
        Last7Days,
        [Description("Next Week")]
        NextWeek,
        [Description("Last Week")]
        LastWeek,
        [Description("This Week")]
        ThisWeek,
        [Description("Next Month")]
        NextMonth,
        [Description("Last Month")]
        LastMonth,
        [Description("This Month")]
        ThisMonth,
        [Description("Next Year")]
        NextYear,
        [Description("Last Year")]
        LastYear,
        [Description("This Year")]
        ThisYear,
        [Description("Next Financial Year")]
        NextFinancialYear,
        [Description("Last Financial Year")]
        LastFinancialYear,
        [Description("This Financial Year")]
        ThisFinancialYear,
        [Description("Last X Days")]
        LastXDays,
        [Description("Next X Days")]
        NextXDays,
        [Description("Last X Weeks")]
        LastXWeeks,
        [Description("Next X Weeks")]
        NextXWeeks,
        [Description("Last X Months")]
        LastXMonths,
        [Description("Next X Months")]
        NextXMonths,
        [Description("Last X Years")]
        LastXYears,
        [Description("Next X Years")]
        NextXYears,
        [Description("Any Time")]
        AnyTime,
        [Description("On")]
        On,
        [Description("Not On")]
        NotOn,
        [Description("After")]
        After,
        [Description("Before")]
        Before,
        [Description("On or After")]
        OnOrAfter,
        [Description("On or Before")]
        OnOrBefore,
        [Description("Next Tax Year")]
        NextTaxYear,
        [Description("Last Tax Year")]
        LastTaxYear,
        [Description("This Tax Year")]
        ThisTaxYear,
        [Description("Not Like")]
        NotLike
    }

    public enum SortDirection
    {
        [Description("[None]")]
        None = 0,
        [Description("Ascending")]
        Ascending = 1,
        [Description("Descending")]
        Descending = 2
    }

    public enum Format
    {
        [Description("[None]")]
        None = -1,
        [Description("Single Line")]
        Single_line = 1,
        [Description("Multiple Line")]
        Multi_line = 2,
        [Description("Date and Time")]
        Date_And_Time = 3,
        [Description("Date Only")]
        Date_Only = 4,
        [Description("Time Only")]
        Time_Only = 5,
        [Description("Formatted Text Box")]
        FormattedTextBox = 6,
        [Description("SingleLineWide")]
        SingleLineWide = 7,
        [Description("Standard")]
        ListStandard = 8,
        [Description("ListWide")]
        ListWide = 9,
        [Description("E-Mail")]
        EMail = 10,
        Phone = 11,
        SMS = 12
    }

    public enum RelationshipType
    {
        [Description("[None]")]
        None = -1,
        [Description("NtoOneRelationship")]
        ManyToOne = 1,
        [Description("OnetoNRelationship")]
        OneToMany = 2
    }

    public enum MenuItems
    {
        [Description("[None]")]
        None = 0,
        [Description("Home")]
        Home = 1,
        [Description("Change Administrative Settings")]
        ChangeAdministrativeSettings = 2,
        [Description("Base Information")]
        BaseInformation = 3,
        [Description("Tailoring")]
        Tailoring = 4,
        [Description("Policy Information")]
        PolicyInformation = 5,
        [Description("User Management")]
        UserManagement = 6,
        [Description("System Options")]
        SystemOptions = 7,
    }

    public enum FieldType
    {
        [Description("[None]")]
        NotSet = 0,
        [Description("Text")]
        Text = 1,
        [Description("Integer")]
        Integer = 2,
        [Description("Date")]
        DateTime = 3,
        [Description("List")]
        List = 4,
        [Description("Yes/No")]
        TickBox = 5,
        [Description("Currency")]
        Currency = 6,
        [Description("Number")]
        Number = 7,
        [Description("Hyperlink")]
        Hyperlink = 8,
        [Description("Relationship")]
        Relationship = 9,
        [Description("Large Text")]
        LargeText = 10,
        RunWorkflow = 11,
        RelationshipTextbox = 12,
        AutoCompleteTextbox = 13,
        /*Message = 14, */
        [Description("Summary")]
        OTMSummary = 15,
        DynamicHyperlink = 16,
        CurrencyList = 17,
        [Description("Comment")]
        Comment = 19,

        LookupDisplayField = 21,
        [Description("Attachment")]
        Attachment = 22,
        Contact = 23
    }

    public enum TabName
    {
        [Description("General Details")]
        GeneralDetails = 1,
        [Description("Columns")]
        Columns = 2,
        [Description("Filters")]
        Filters = 3,
        [Description("Sorting")]
        Sorting = 4,
        [Description("Icon")]
        Icon = 5
    }

    public enum YesOrNo
    {
        [Description("[None]")]
        None = 0,
        [Description("Yes")]
        Yes = 1,
        [Description("No")]
        No = 2,
    }

    public enum FieldFrom
    {
        [Description("Base Table")]
        BaseTable = 0,
        [Description("Greenlight Attribute")]
        GreenlightAttribute = 1,
        [Description("UDF")]
        UDF = 2
    }
}
