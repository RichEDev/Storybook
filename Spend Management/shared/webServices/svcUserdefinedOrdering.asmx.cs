using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcUserdefinedOrdering
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcUserdefinedOrdering : System.Web.Services.WebService
    {
        /// <summary>
        ///  Saves the order of groups and fields (dependant on access roles)
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="tableID"></param>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public void SaveOrder(object[] orders, Guid tableID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            bool canUpdateGroups = false;
            bool canUpdateFields = false;

            if(currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserDefinedFields, false) == true || currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserDefinedFields, false) == true)
            {
                canUpdateFields = true;
            }

            if(currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserdefinedGroupings, false) == true || currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserdefinedGroupings, false) == true)
            {
                canUpdateGroups = true;
            }

            if (canUpdateFields == true || canUpdateGroups == true)
            {
                /// Get the applies to table
                cTables clsTables = new cTables(currentUser.Account.accountid);
                cTable reqTable = clsTables.GetTableByID(tableID);
                
                Dictionary<int, int> lstGroupingOrder = new Dictionary<int, int>();
                Dictionary<int, int> lstFieldOrdering = new Dictionary<int, int>();

                #region Sort the serialization mess out
                int groupID;
                int udfID;

                int groupOrderCounter = 0;
                int fieldOrderCounter = 0;

                for (int i = 0; i < orders.Length; i++)
                {
                    groupID = int.Parse(((Dictionary<string, object>)orders[i])["Group"].ToString());

                    /// Remove groups with groupID as Int32.MaxValue due to them not being real
                    if (groupID < Int32.MaxValue)
                    {
                        lstGroupingOrder.Add(groupID, groupOrderCounter);
                    }

                    for (int j = 0; j < ((object[])((Dictionary<string, object>)orders[i])["UserdefinedFields"]).Length; j++)
                    {
                        udfID = int.Parse(((object[])((Dictionary<string, object>)orders[i])["UserdefinedFields"])[j].ToString());
                        lstFieldOrdering.Add(udfID, fieldOrderCounter);
                        fieldOrderCounter++;
                    }
                    groupOrderCounter++;
                }
                #endregion Sort the serialization mess out

                if (canUpdateGroups == true)
                {
                    /// Save the group orders
                    cUserdefinedFieldGroupings clsUDFGroupings = new cUserdefinedFieldGroupings(currentUser.Account.accountid);
                    clsUDFGroupings.SaveOrders(lstGroupingOrder, reqTable);
                }

                if (canUpdateFields == true)
                {
                    /// Save the field orders
                    cUserdefinedFields clsUDFFields = new cUserdefinedFields(currentUser.Account.accountid);
                    clsUDFFields.SaveOrders(lstFieldOrdering, reqTable);
                }
            }
        }
    }
}
