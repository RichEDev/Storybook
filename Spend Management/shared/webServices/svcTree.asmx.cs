using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using Spend_Management.shared.code.Helpers;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcTree
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcTree : System.Web.Services.WebService
    {
        /// <summary>
        /// Retrieves type information for the filter modal. It remaps the type into a more finite set. Type, list or precision is returned
        /// </summary>
        /// <param name="fieldId">Field ID Guid</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public object[] GetFieldInfoForFilter(Guid fieldId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities ces = new cCustomEntities(user);
            return ces.GetFieldInfoForFilter(fieldId, user.AccountID, user.CurrentSubAccountId);
        }

        /// <summary>
        /// A list of operators from ConditionType enum
        /// </summary>
        /// <param name="fieldType">field type e.g. "D" or "T" or "GL". This is a type derived from GetFieldInfoForViewFilter</param>
        /// <param name="filterFieldId">field ID of the field being filtered by</param>
        /// <param name="showInOption">Add the In option for text fields (torch)</param>
        /// <returns>List of ListItem made from ConditionType enum alongside the field type</returns>
        [WebMethod(EnableSession = true)]
        public FilterCriteria GetOperatorsListItemsByFilterFieldType(string fieldType, string filterFieldId, bool showInOption)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(user.AccountID);
            Guid gFieldID;
            Guid.TryParseExact(filterFieldId, "D", out gFieldID);

            var requestUrlReferrer = this.Context.Request.UrlReferrer;
            var allowHierachy = requestUrlReferrer != null && !requestUrlReferrer.LocalPath.ToLower().Contains("aereport");

            return FieldFilters.GetOperatorsListItemsByFilterFieldType(
                fieldType, allowHierachy && FieldFilters.CanUseMyHierarchy(clsFields.GetFieldByID(gFieldID)), showInOption);
        }

        /// <summary>
        /// Gets a list of values from a List field
        /// </summary>		
        /// <param name="fieldid"></param>
        /// <param name="commaSeperatedIDs"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public object[] GetListItemsTextForField(Guid fieldid, string commaSeperatedIDs)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cFields fields = new cFields(user.AccountID);
            cField field = fields.GetFieldByID(fieldid);
            List<string> list = new List<string>();

            List<string> listIDs = commaSeperatedIDs.Split(',').ToList();

            string type = field.FieldType;
            if (field.ValueList)
            {
                type = "L";
                list.AddRange(
                    from listItem in field.ListItems
                    where listIDs.Contains(listItem.Key.ToString())
                    select listItem.Value);
            }
            if (field.GenList && !field.ValueList)
            {
                type = "GL";
                List<ListItem> genList = FieldFilters.GetGenListItemsForField(fieldid, user);

                list.AddRange(
                    from genListItem in genList where listIDs.Contains(genListItem.Value) select genListItem.Text);
            }
            if (field.FieldType == "CL")
            {
                list.AddRange(
                    Currencies.GetCurrencyList(user.AccountID, user.CurrentSubAccountId).Select(listItem => listItem.Text));
            }

            object[] ret = new object[2];
            ret[0] = type;
            ret[1] = list;
            return ret;
        }
    }
}
