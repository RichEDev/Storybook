using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;
using System.Data;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    public class cAccountSubAccounts : cAccountSubAccountsBase
    {
        /// <summary>
        /// cAccountSubAccounts constructor
        /// </summary>
        /// <param name="accountId">Customer DB Account Id</param>
        public cAccountSubAccounts(int accountId, IDBConnection connection = null)
            : base(accountId, connection)
        {

        }

        /// <summary>
        /// Gets drop down list elements containing only sub-accounts currently associated with Employee Access roles for the employee
        /// </summary>
        /// <param name="employee">Employee to get assigned sub-accounts for</param>
        /// <param name="subaccountId">Current Sub-Account selection. NULL if none.</param>
        /// <returns></returns>
        public System.Web.UI.WebControls.ListItem[] CreateFilteredDropDown(Employee employee, int? subaccountId)
        {
            List<System.Web.UI.WebControls.ListItem> retList = new List<System.Web.UI.WebControls.ListItem>();
            SortedList<string, int> subaccs = new SortedList<string, int>();
            foreach (KeyValuePair<int, List<int>> subaccAssigned in employee.GetAccessRoles().AllAccessRoles)
            {
                int saId = subaccAssigned.Key;
                cAccountSubAccount sa = getSubAccountById(saId);

                if (!subaccs.ContainsKey(sa.Description))
                {
                    subaccs.Add(sa.Description, sa.SubAccountID);
                }
            }

            foreach (KeyValuePair<string, int> saDesc in subaccs)
            {
                System.Web.UI.WebControls.ListItem tempitem = new System.Web.UI.WebControls.ListItem();
                tempitem.Text = saDesc.Key;
                tempitem.Value = saDesc.Value.ToString();
                if (subaccountId.HasValue)
                {
                    if (saDesc.Value == subaccountId.Value)
                    {
                        tempitem.Selected = true;
                    }
                }
                retList.Add(tempitem);
            }

            return retList.ToArray();
        }

        /// <summary>
        /// Create the grid for the sub accounts pop up
        /// </summary>
        public string[] generateSubaccountGrid(ref int rowCount)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cTables clsTables = new cTables(currentUser.AccountID);
            cFields clsfields = new cFields(currentUser.AccountID);

            DataSet ds = getEmployeeSubAccountDataset(currentUser.EmployeeID, currentUser.CurrentSubAccountId);
            rowCount = ds.Tables[0].Rows.Count;
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("5AF302C6-40A7-425E-BF2E-D233D1131F57"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("2D43703F-7FF2-43E2-BC4D-2BAE727A25F8"))));

            cGridNew clsgrid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridSubaccounts", clsTables.GetTableByID(new Guid("be306290-d258-4295-b3f2-5b990396be20")), columns, ds);

            clsgrid.getColumnByName("subAccountID").hidden = true;
            clsgrid.enabledeleting = false;
            clsgrid.enableupdating = false;

            clsgrid.addEventColumn("switchsubacc", "/shared/images/buttons/check.png", "javascript:SwitchSubAccount({subAccountID});", "Switch sub account", "Switch sub account");
            clsgrid.KeyField = "subAccountID";
            clsgrid.CssClass = "datatbl";

            //clsgrid.EmptyText = "There are currently no forms defined for this entity.";
            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        #region Properties

        /// <summary>
        /// Gets or Sets the Default Cost Code Owner
        /// </summary>
        public IOwnership GetDefaultCostCodeOwner(int accountId, int? subAccountId)
        {
            IOwnership dcco = null;
            cAccountProperties properties = null;

            if (subAccountId.HasValue)
            {
                properties = this.getSubAccountById(subAccountId.Value).SubAccountProperties;
            }
            else
            {
                properties = this.getFirstSubAccount().SubAccountProperties;
            }

            if (!string.IsNullOrEmpty(properties.CostCodeOwnerBaseKey) && properties.CostCodeOwnerBaseKey.Contains(","))
            {
                dcco = Ownership.Parse(accountId, subAccountId, properties.CostCodeOwnerBaseKey);
            }

            return dcco;
        }

        #endregion
    }
}
