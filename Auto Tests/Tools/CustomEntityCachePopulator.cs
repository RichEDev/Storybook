using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields;

namespace Auto_Tests.Tools
{
    public abstract class CachePopulator
    {
        protected cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
        public virtual string GetSQLStringForCustomEntity()
        {
            return "SELECT entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
        }
        public virtual string GetSqlstringForAttributes(int entityid)
        {
            return "SELECT createdby, modifiedby, display_name, description, tooltip, mandatory, fieldtype, relatedtable, relationshipdisplayfield, maxRows, createdon, maxlength, format, defaultvalue, precision, relationshiptype, related_entity, viewid, is_audit_identity, advicePanelText, is_unique, System_attribute, attributeid, TriggerAttributeId, TriggerJoinViaId, TriggerDisplayFieldId, boolAttribute, fieldid FROM customEntityAttributes where entityid = '" + entityid + "'";
        }

        public virtual string GetSqlStringForViews(int entityId)
        {
            return "SELECT viewid, entityid, view_name, description, createdon, createdby, modifiedon, modifiedby, menuid, MenuDescription, allowadd, add_formid, allowedit, edit_formid, allowdelete, allowapproval, SortColumn, SortOrder, SortColumnJoinViaID, MenuIcon FROM customEntityViews where entityid = '" + entityId + "'";
        }

        public virtual string GetSqlStringViewFields(int viewid)
        {
            return "SELECT viewFieldId, viewid, fieldid, [order], joinViaID FROM customEntityViewFields WHERE viewid = '" + viewid + "'";
        }

        public virtual string GetSqlStringViewFilters(int viewid)
        {
            return "SELECT viewid, fieldid, condition, value, [order], joinViaID, valueTwo, attributeid FROM fieldFilters WHERE viewid = '" + viewid + "'";
        }

        public virtual string GetSqlStringAttributeFilters(int attributeid)
        {
            return "SELECT viewid, fieldid, condition, value, [order], joinViaID, valueTwo, attributeid FROM fieldFilters WHERE attributeid = '" + attributeid + "'";
        }

        public virtual string GetSqlStringForForms(int entityId)
        {
            return "SELECT formid, form_name, description, createdon, createdby, modifiedby, showBreadCrumbs, showSaveAndNew, showSubMenus, SaveAndNewButtonText, SaveButtonText, CancelButtonText, showSave, showCancel, SaveAndStayButtonText, showSaveAndStay, SaveAndDuplicateButtonText, showSaveAndDuplicate, CheckDefaultValues FROM customEntityForms";
        }

        public virtual string GetSqlStringForTabs(int formid)
        {
            return "SELECT tabid, formid, header_caption, [order] from customEntityFormTabs where formid = '" + formid + "'";
        }

        public virtual string GetSqlStringForSections(int tabid)
        {
            return "SELECT sectionid, formid, header_caption, tabid, [order] from customEntityFormSections where tabid ='" + tabid + "'";
        }

        public virtual string GetSqlStringForFields(int sectionid)
        {
            return "SELECT  formid, attributeid, [readonly], sectionid, row, [column], labelText, DefaultValue from customEntityFormFields where sectionid = '" + sectionid + "'";
        }

        public virtual void PopulateAttributes(ref CustomEntity customEntity) { }
        public virtual void PopulateForms(ref CustomEntity customEntity) { }
        public virtual void PopulateViews(ref CustomEntity customEntity) { }
        public virtual void PopulateViewFields(ref CustomEntitiesUtilities.CustomEntityView view) { }
        public virtual void PopulateViewFilters(ref CustomEntitiesUtilities.CustomEntityView view) { }
        public virtual void PopulateAttributeFilters(ref CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute, int attributeId) { }
        
        /// <summary>
        /// Current product that test are executing
        /// </summary>
        protected ProductType ExecutingProduct;

        protected CachePopulator(ProductType executingProduct)
        {
            ExecutingProduct = executingProduct;
        }

        public List<CustomEntity> PopulateCache()
        {
            List<CustomEntity> customEntities = new List<CustomEntity>();
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSQLStringForCustomEntity()))
            {
                while (reader.Read())
                {
                    #region Set variables

                    CustomEntity customEntity = new CustomEntity();
                    customEntity.entityId = reader.GetInt32(0);
                    if (!reader.IsDBNull(1))
                    {
                        customEntity.entityName = reader.GetString(1);
                    }
                    if (!reader.IsDBNull(2))
                    {
                        customEntity.pluralName = reader.GetString(2);
                    }
                    if (!reader.IsDBNull(3))
                    {
                        customEntity.description = reader.GetString(3);
                    }
                    customEntity.enableCurrencies = reader.GetBoolean(4);
                    if (!reader.IsDBNull(5))
                    {
                        customEntity.defaultCurrencyId = reader.GetString(5);
                    }
                    else
                    {
                        customEntity.defaultCurrencyId = null;
                    }
                    customEntity.date = reader.GetDateTime(6);
                    customEntity.enableAttachments = reader.GetBoolean(7);
                    customEntity.allowDocumentMerge = reader.GetBoolean(8);
                    customEntity.AudienceViewType = (AudienceViewType)reader.GetInt16(9);
                    customEntity.tableId = reader.GetGuid(12);
                    customEntity.userId = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);

                    PopulateAttributes(ref customEntity);
                    PopulateForms(ref customEntity);
                    PopulateViews(ref customEntity);
                    customEntities.Add(customEntity);
                    #endregion
                }
                reader.Close();
            }
            return customEntities;
        }

        public List<CustomEntitiesUtilities.JoinVia> PopulateJoinVias()
        {
            List<CustomEntitiesUtilities.JoinVia> joinVias = new List<CustomEntitiesUtilities.JoinVia>();
            var strSql = "SELECT joinViaID, joinViaDescription, joinViaAS, joinViaPathHash, CacheExpiry FROM joinVia";
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSql))
            {
                int joinViaIdOrdinal = reader.GetOrdinal("joinViaID");
                int joinViaDescriptionOrdinal = reader.GetOrdinal("joinViaDescription");
                int joinViaAsOrdinal = reader.GetOrdinal("joinViaAS");
                int joinViaHashPathOrdinal = reader.GetOrdinal("joinViaPathHash");
                int cashExpiry = reader.GetOrdinal("CacheExpiry");

                while (reader.Read())
                {
                    var joinVia = new CustomEntitiesUtilities.JoinVia
                    {
                        _joinViaID = reader.GetInt32(joinViaIdOrdinal),
                        _joinViaDescription = reader.GetString(joinViaDescriptionOrdinal),
                        _joinViaAS = reader.GetGuid(joinViaAsOrdinal),
                        JoinViaPathHash = reader.GetString(joinViaHashPathOrdinal),
                        JoinViaParts = new SortedList<int, CustomEntitiesUtilities.JoinViaPart>()
                    };

                    PopulateJoinViaParts(ref joinVia);

                    joinVias.Add(joinVia);
                }
                reader.Close();
            }
            return joinVias;
        }

        private void PopulateJoinViaParts(ref CustomEntitiesUtilities.JoinVia joinVia)
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            // List<CustomEntitiesUtilities.JoinViaPart> joinVias = new List<CustomEntitiesUtilities.JoinViaPart>();
            var strSql = "SELECT joinViaPartID, relatedID, relatedType, [order] FROM joinViaParts where joinViaID = @joinviaid";
            db.sqlexecute.Parameters.AddWithValue("@joinviaid", joinVia._joinViaID);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSql))
            {
                int joinViaPartIDOrdinal = reader.GetOrdinal("joinViaPartID");
                int relatedIDOrdinal = reader.GetOrdinal("relatedID");
                int relatedTypeOrdinal = reader.GetOrdinal("relatedType");
                int orderOrdinal = reader.GetOrdinal("order");

                while (reader.Read())
                {
                    var joinViaPart = new CustomEntitiesUtilities.JoinViaPart
                    {
                        JoinViaID = joinVia._joinViaID,
                        RelatedID = reader.GetGuid(relatedIDOrdinal),
                        Order = reader.GetInt32(orderOrdinal),
                        JoinViaPartID = reader.GetInt32(joinViaPartIDOrdinal),
                        _type = (CustomEntitiesUtilities.JoinViaPart.IDType)reader.GetByte(relatedTypeOrdinal)
                    };

                    joinVia.JoinViaParts.Add(joinViaPart.Order, joinViaPart);
                }
                reader.Close();
            }
        }


        internal static void PopulateListItems(ref CustomEntitiesUtilities.cCustomEntityListAttribute attribute, int customEntityId)
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT valueid, [attributeid] ,[item], [order], [archived] FROM customEntityAttributeListItems WHERE attributeid = @attributeid";
            db.sqlexecute.Parameters.AddWithValue("@attributeid", customEntityId);

            List<CustomEntitiesUtilities.EntityListItem> customEntityListItems = new List<CustomEntitiesUtilities.EntityListItem>();
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                int attributeIdOrdinal = reader.GetOrdinal("attributeid");
                int itemOrdinal = reader.GetOrdinal("item");
                int orderOrdinal = reader.GetOrdinal("order");
                int archivedOrdinal = reader.GetOrdinal("archived");
                int valueIdOrdinal = reader.GetOrdinal("valueid");

                while (reader.Read())
                {
                    int attributeId = reader.GetInt32(attributeIdOrdinal);
                    string text = reader.IsDBNull(itemOrdinal) ? null : reader.GetString(itemOrdinal);
                    int order = reader.GetInt32(orderOrdinal);
                    bool archived = reader.GetBoolean(archivedOrdinal);
                    int valueID = reader.GetInt32(valueIdOrdinal);
                    customEntityListItems.Add(new CustomEntitiesUtilities.EntityListItem(attributeId, text, order, archived, valueID));
                }
            }
            attribute._listItems = customEntityListItems;
        }
    }
}
