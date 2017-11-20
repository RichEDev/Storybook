using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cSubAccountObject
    {
        public static cAccountSubAccount CreateSubAccount()
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);

            cAccountSubAccount subacc = new cAccountSubAccount(-1, "Test " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), false, new cAccountProperties(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
            cGlobalVariables.SubAccountID = subaccs.UpdateSubAccount(subacc, cGlobalVariables.EmployeeID, cGlobalVariables.DefaultSubAccountID, cGlobalVariables.AccountID, cGlobalVariables.DefaultSubAccountID);

            subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
            subacc = subaccs.getSubAccountById(cGlobalVariables.SubAccountID);
            return subacc;
        }

        //public static void CreateSubAccount()
        //{
        //    cGlobalVariables.BaseDefinitionTableID = new Guid("be306290-d258-4295-b3f2-5b990396be20");
        //    cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
        //    cBaseDefinitionObject.CreateBaseDefinition(cGlobalVariables.BaseDefinitionTableID, SpendManagementElement.SubAccounts);
        //    cGlobalVariables.SubAccountID = cGlobalVariables.BaseDefinitionID;
        //}

        public static void DeleteSubAccount()
        {
            if (cGlobalVariables.SubAccountID != cGlobalVariables.DefaultSubAccountID)
            {
                cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
                subaccs.DeleteSubAccount(cGlobalVariables.SubAccountID, cGlobalVariables.EmployeeID);
                cGlobalVariables.SubAccountID = cGlobalVariables.DefaultSubAccountID;
            }
        }

        public static void GrantAccessRole(int subaccountid)
        {
            cAccessRoles roles = new cAccessRoles(cGlobalVariables.AccountID, cAccounts.getConnectionString(cGlobalVariables.AccountID));
            cAccessRole role = roles.AccessRoles[roles.AccessRoles.Keys[0]];
            cGlobalVariables.RoleID = role.RoleID;

            cEmployees emps = new cEmployees(cGlobalVariables.AccountID);

            emps.saveAccessRoles(cGlobalVariables.EmployeeID, new List<int>() { cGlobalVariables.RoleID }, cGlobalVariables.SubAccountID);

            return;
        }

        public static void CleanupAccessRoles()
        {
            cEmployees emps = new cEmployees(cGlobalVariables.AccountID);

            emps.deleteEmployeeAccessRole(cGlobalVariables.EmployeeID, cGlobalVariables.RoleID, cGlobalVariables.SubAccountID);
        }
    }
}
