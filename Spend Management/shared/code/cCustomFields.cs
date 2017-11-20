using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Collections.Generic;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Deals with the custom fields
    /// </summary>
    public class cCustomFields
    {
        private int nAccountID;

        /// <summary>
        /// Constructor for the custom fields
        /// </summary>
        /// <param name="AccountID"></param>
        public cCustomFields(int AccountID)
        {
            nAccountID = AccountID;
        }

        #region Properties

        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Creates the grid in the custon entity editor for the viewing of the alias and functional fields
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string[] CreateCustomFieldGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridFields", "select fieldid, description, tables.tablename, fieldtype, FieldCategory from customFields");
                
            cFields clsfields = new cFields(user.AccountID);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("73f832ec-3856-4772-9a21-9e7052e4ef5c")), ConditionType.DoesNotEqual, new object[] { FieldCategory.ViewField, FieldCategory.AliasTableField }, null, ConditionJoiner.None);
            
            clsgrid.getColumnByName("fieldid").hidden = true;
            clsgrid.enabledeleting = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:editCustomField('{fieldid}');";
            clsgrid.deletelink = "javascript:DeleteCustomField('{fieldid}');";

            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Currency, "Currency");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.DateTime, "Date/Time");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Hyperlink, "Hyperlink");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Integer, "Integer");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.LargeText, "Large Text");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.List, "List");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Number, "Number");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Relationship, "Relationship");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.Text, "Text");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.TickBox, "Tickbox");
            //clsgrid.getColumnByName("fieldtype").addValueListItem((byte)FieldType.OTMSummary, "Summary");

            ((cFieldColumn)clsgrid.getColumnByName("FieldCategory")).addValueListItem((byte)FieldCategory.AliasField, "Alias");
            ((cFieldColumn)clsgrid.getColumnByName("FieldCategory")).addValueListItem((byte)FieldCategory.FunctionField, "Function");
            ((cFieldColumn)clsgrid.getColumnByName("FieldCategory")).addValueListItem((byte)FieldCategory.ViewField, "View");

            clsgrid.CssClass = "datatbl";
            clsgrid.SortedColumn = clsgrid.getColumnByName("description");
            clsgrid.KeyField = "fieldid";

            clsgrid.EmptyText = "There are currently no fields defined for this GreenLight.";
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Save the custom field whether it be an Alias or Function field to the database
        /// </summary>
        /// <param name="custField"></param>
        /// <returns></returns>
        public Guid SaveCustomField(sCustomField custField)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cFields clsFields = new cFields(AccountID);
            Guid FieldID = Guid.Empty;
            Guid TableID = new Guid(custField.TableID);
            Guid RelatedFieldID = Guid.Empty;

            if (custField.RelatedFieldID != string.Empty)
            {
                RelatedFieldID = new Guid(custField.RelatedFieldID);
            }

            bool update = false;

            if (custField.FieldID == string.Empty)
            {
                FieldID = Guid.NewGuid();
            }
            else
            {
                FieldID = new Guid(custField.FieldID);
                update = true;
            }

            db.sqlexecute.Parameters.AddWithValue("@Update", update);
            db.sqlexecute.Parameters.AddWithValue("@FieldID", FieldID);
            db.sqlexecute.Parameters.AddWithValue("@TableID", TableID);

            if (custField.Description != string.Empty)
            {
                db.AddWithValue("@Description", custField.Description, clsFields.GetFieldSize("customFields", "description"));
            }
            else
            {
                db.AddWithValue("@Description", DBNull.Value, clsFields.GetFieldSize("customFields", "description"));
            }

            db.sqlexecute.Parameters.AddWithValue("@FieldCategory", custField.FieldCat);

            db.sqlexecute.Parameters.AddWithValue("@LookupTable", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@LookupField", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@Comment", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@ViewGroupID", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@ReLabel_Param", DBNull.Value);

            switch (custField.FieldCat) 
            {
                case FieldCategory.AliasField:
                case FieldCategory.AliasTableField:

                    #region Alias field values

                    cField relatedField = clsFields.GetFieldByID(RelatedFieldID);

                    if (relatedField != null)
                    {
                        db.AddWithValue("@Field", relatedField.FieldName, clsFields.GetFieldSize("customFields", "field"));
                        db.sqlexecute.Parameters.AddWithValue("@DataType", relatedField.FieldType);

                        if (custField.FieldCat == FieldCategory.AliasTableField)
                        {
                            if (relatedField.Description == string.Empty)
                            {
                                db.sqlexecute.Parameters["@Description"].Value = DBNull.Value;
                            }
                            else
                            {
                                db.sqlexecute.Parameters["@Description"].Value = relatedField.Description;
                            }
                        }

                        if (relatedField.Comment != string.Empty)
                        {
                            db.sqlexecute.Parameters["@Comment"].Value = relatedField.Comment;
                        }
                       
                        db.sqlexecute.Parameters.AddWithValue("@NormalView", relatedField.NormalView);
                        db.sqlexecute.Parameters.AddWithValue("@IdField", relatedField.IDField);
                        db.sqlexecute.Parameters.AddWithValue("@GenList", relatedField.GenList);
                        db.sqlexecute.Parameters.AddWithValue("@Width", relatedField.Width);
                        db.sqlexecute.Parameters.AddWithValue("@CanTotal", relatedField.CanTotal);
                        db.sqlexecute.Parameters.AddWithValue("@PrintOut", relatedField.PrintOut);
                        db.sqlexecute.Parameters.AddWithValue("@ValueList", relatedField.ValueList);
                        db.sqlexecute.Parameters.AddWithValue("@AllowImport", relatedField.AllowImport);
                        db.sqlexecute.Parameters.AddWithValue("@Mandatory", relatedField.Mandatory);

                        if (relatedField.GetLookupTable() != null)
                        {
                            db.sqlexecute.Parameters["@LookupTable"].Value = relatedField.GetLookupTable().TableID;
                        }

                        if (relatedField.GetLookupField() != null)
                        {
                            db.sqlexecute.Parameters["@LookupField"].Value = relatedField.LookupFieldID;
                        }

                        db.sqlexecute.Parameters.AddWithValue("@UseForLookup", relatedField.UseForLookup);
                        db.sqlexecute.Parameters.AddWithValue("@WorkflowUpdate", relatedField.WorkflowUpdate);
                        db.sqlexecute.Parameters.AddWithValue("@WorkflowSearch", relatedField.WorkflowSearch);
                        db.sqlexecute.Parameters.AddWithValue("@Length", relatedField.Length);

                        if (relatedField.ViewGroupID != Guid.Empty)
                        {
                            db.sqlexecute.Parameters["@ViewGroupID"].Value = relatedField.ViewGroupID;
                        }

                        db.sqlexecute.Parameters.AddWithValue("@ReLabel", relatedField.ReLabel);

                        if (relatedField.RelabelParam != string.Empty)
                        {
                            db.sqlexecute.Parameters["@ReLabel_Param"].Value = relatedField.RelabelParam;
                        }
                    }

                    #endregion

                    break;

                case FieldCategory.FunctionField:

                    #region Function field values

                    db.AddWithValue("@Field", custField.FieldName, clsFields.GetFieldSize("customFields", "field"));
                    db.sqlexecute.Parameters.AddWithValue("@DataType", custField.DataType);

                    db.sqlexecute.Parameters.AddWithValue("@NormalView", false);
                    db.sqlexecute.Parameters.AddWithValue("@IdField", false);
                    db.sqlexecute.Parameters.AddWithValue("@GenList", false);
                    db.sqlexecute.Parameters.AddWithValue("@Width", 0);
                    db.sqlexecute.Parameters.AddWithValue("@CanTotal", false);
                    db.sqlexecute.Parameters.AddWithValue("@PrintOut", false);
                    db.sqlexecute.Parameters.AddWithValue("@ValueList", false);
                    db.sqlexecute.Parameters.AddWithValue("@AllowImport", false);
                    db.sqlexecute.Parameters.AddWithValue("@Mandatory", false);
                    db.sqlexecute.Parameters.AddWithValue("@UseForLookup", false);
                    db.sqlexecute.Parameters.AddWithValue("@WorkflowUpdate", false);
                    db.sqlexecute.Parameters.AddWithValue("@WorkflowSearch", false);
                    db.sqlexecute.Parameters.AddWithValue("@Length", 0);
                    db.sqlexecute.Parameters.AddWithValue("@ReLabel", false);

                    #endregion

                    break;

                default:
                    break;
            }
            db.sqlexecute.Parameters.Add("@identity", SqlDbType.UniqueIdentifier);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
            db.ExecuteProc("dbo.SaveCustomField");
            FieldID = new Guid(db.sqlexecute.Parameters["@identity"].Value.ToString());
            db.sqlexecute.Parameters.Clear();

            removeFieldCache();

            return FieldID;
        }

        /// <summary>
        /// Delete the custom field from the databse. A check is made to make sure the field is not associated to 
        /// a workflow condition or a custom entity view, if it is then the field is not delete and the status returned
        /// </summary>
        /// <param name="sFieldID"></param>
        /// <returns></returns>
        public ReturnValues DeleteCustomField(string sFieldID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            Guid FieldID = new Guid(sFieldID);

            db.sqlexecute.Parameters.AddWithValue("@FieldID", FieldID);
            db.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
            db.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("DeleteCustomField");
            ReturnValues returnCode = (ReturnValues)(int)db.sqlexecute.Parameters["@returncode"].Value;

            db.sqlexecute.Parameters.Clear();

            removeFieldCache();

            return returnCode;
        }

        /// <summary>
        /// Remove the fields from cache to get the latest version   
        /// </summary>
        private void removeFieldCache()
        {
            System.Web.Caching.Cache cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            cache.Remove("fields" + AccountID);
            cFields fields = new cFields(AccountID); //Calls the initialise data method to update the cache
        }

        /// <summary>
        /// This method get the child field attribute id associated to the parent control
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="parentControlId">
        /// The parent attribute which has a child field configured as filter
        /// </param>
        /// <param name="formId">
        /// The Id of the form which has parent child filter configured
        /// </param>
        /// <returns>
        /// Child field attribute id associated to the selected parent
        /// </returns>
        public static int GetTriggerChildFieldValues(DatabaseConnection connection, string parentControlId, int formId)
        {
            int fieldToBuild = 0;
            using (connection)
            {
                connection.AddWithValue("@parentControlId", parentControlId);
                connection.AddWithValue("@formId", formId);
                
                    using (var reader = connection.GetReader("GetChildFieldValues", CommandType.StoredProcedure))
                    {
                        var fieldToBuildOrd = reader.GetOrdinal("FieldToBuild");

                        while (reader.Read())
                        {
                             fieldToBuild = !reader.IsDBNull(fieldToBuildOrd) ? reader.GetInt32(fieldToBuildOrd) : 0;
                        }

                    return fieldToBuild;
                    }
            }
        }

        /// <summary>
        /// The get field id of the Display field associated with the child control in parent child filter relation in greenlight forms
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="parentControl">
        /// The parent control.
        /// </param>
        /// <param name="childControl">
        /// The child control associated with the parent
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// GUID of the display field associated with the child control. This is to reset the display field associated with the child  on changing a parent control 
        /// </returns>
        public static Guid GetFilterAttributeForChildField(DatabaseConnection connection, string parentControl, string childControl)
        {
            Guid result = Guid.Empty;
            using (connection)
            {
                connection.AddWithValue("@parentControlId", parentControl);
                connection.AddWithValue("@ChildControlId", childControl);

                using (var reader = connection.GetReader("GetChildFilterFieldValues", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var displayField = reader.GetOrdinal("relationshipDisplayField");
                        result = !reader.IsDBNull(displayField) ? reader.GetGuid(displayField) : Guid.Empty;
                    }

                    return result;
                }
            }
        }
    }
}
