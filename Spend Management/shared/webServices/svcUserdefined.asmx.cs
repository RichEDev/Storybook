
namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for svcUserdefined
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcUserdefined : System.Web.Services.WebService
    {
        /// <summary>
        /// Gets the current selections for match fields when editing a relationship udf
        /// </summary>
        /// <param name="udfID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> getRelationshipMatchSelections(int udfID)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(curUser.AccountID);
            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
            cUserDefinedField ufield = ufields.GetUserDefinedById(udfID);

            List<ListItem> items = new List<ListItem>();
            foreach (Guid g in ((cManyToOneRelationship)ufield.attribute).AutoCompleteMatchFieldIDList)
            {
                cField matchField = clsFields.GetFieldByID(g);
                if (matchField != null)
                    items.Add(new ListItem(matchField.Description, g.ToString()));
            }

            return items;
        }

        /// <summary>
        /// The save user defined field.
        /// </summary>
        /// <param name="id">
        /// The user defined field ID.
        /// </param>
        /// <param name="udf_name">
        /// The user defined field name.
        /// </param>
        /// <param name="description">
        /// The user defined field description.
        /// </param>
        /// <param name="tooltip">
        /// The user defined field tooltip.
        /// </param>
        /// <param name="fieldtype">
        /// The user defined field type.
        /// </param>
        /// <param name="udfGroupId">
        /// The user defined field group ID.
        /// </param>
        /// <param name="order">
        /// The user defined field order.
        /// </param>
        /// <param name="format">
        /// The user defined field format.
        /// </param>
        /// <param name="itemSpecific">
        /// Is the data item specific?.
        /// </param>
        /// <param name="allowSearch">
        /// Allow searching on this field.
        /// </param>
        /// <param name="appliesToTableID">
        /// The ID of a table that this field applies to.
        /// </param>
        /// <param name="hyperlinkText">
        /// The hyperlink text.
        /// </param>
        /// <param name="hyperlinkPath">
        /// The hyperlink path.
        /// </param>
        /// <param name="relatedTableID">
        /// The ID of a related table (for relationship fields).
        /// </param>
        /// <param name="mandatory">
        /// Is this field mandatory.
        /// </param>
        /// <param name="acDisplayField">
        /// The auto complete display field.
        /// </param>
        /// <param name="acMatchFields">
        /// The auto complete match fields.
        /// </param>
        /// <param name="maxLength">
        /// The max length of a field.
        /// </param>
        /// <param name="precision">
        /// The precision (for decimal fields).
        /// </param>
        /// <param name="listitems">
        /// The list items (for list item types).
        /// </param>
        /// <param name="defaultVal">
        /// The default value.
        /// </param>
        /// <param name="maxRows">
        /// The max rows returned for a relationshop type.
        /// </param>
        /// <param name="allowEmployeeToPopulate">
        /// Allow the employee to populate the data.
        /// </param>
        /// <param name="encrypted">
        /// Is the data encrypted in the database.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> ID of the updated or nre record.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int saveUserDefinedField(int id, string udf_name, string description, string tooltip, byte fieldtype, int udfGroupId, int order, byte format, bool itemSpecific, bool allowSearch, string appliesToTableID, string hyperlinkText, string hyperlinkPath, string relatedTableID, bool mandatory, string acDisplayField, string[] acMatchFields, int maxLength, int precision, string[] listitems, string defaultVal, int maxRows, bool allowEmployeeToPopulate, bool encrypted)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cUserdefinedFields userdefinedFields = new cUserdefinedFields(currentUser.AccountID);
            cTables tables = new cTables(currentUser.AccountID);
            cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(currentUser.AccountID);
            return userdefinedFields.SaveUserDefinedField(id, udf_name, description, tooltip, fieldtype, udfGroupId, order, format, itemSpecific, allowSearch, appliesToTableID, hyperlinkText, hyperlinkPath, relatedTableID, mandatory, acDisplayField, acMatchFields, maxLength, precision, listitems, defaultVal, maxRows, allowEmployeeToPopulate, currentUser, tables, groupings, encrypted);
        }

    
        /// <summary>
        /// Checks if a specified List Fields' List Item is in use on a CustomEntityViewFilter
        /// </summary>
        /// <param name="listItemID"></param>
        /// <param name="userDefinedID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int CheckListItemIsNotUsedWithinFilter(int userDefinedID, int listItemID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities customEntities = new cCustomEntities(currentUser);
            cUserdefinedFields userdefinedFields = new cUserdefinedFields(currentUser.AccountID);
            cUserDefinedField userDefinedField = userdefinedFields.GetUserDefinedById(userDefinedID);

            return customEntities.CheckListItemIsNotUsedWithinFilter(userDefinedField.attribute.fieldid, listItemID);
        }

        [WebMethod(EnableSession = true)]
        public List<ListItem> getAvailableGroupings(string tableid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(user.AccountID);
            return groupings.CreateDropDown(new Guid(tableid));
        }

        /// <summary>
        /// Gets a grid of report data belonging UDF    
        /// <param name="userDefinedID">The user defined id for the UDF</param>
        /// <returns>HTML Grid</returns>
        /// </summary>
        [WebMethod(EnableSession = true)]
        public string[] GetUDFReports(int userDefinedId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFields userFields = new cUserdefinedFields(user.AccountID);
            return userFields.CreateUdfReportGrid(userDefinedId);
        }
    }
}
