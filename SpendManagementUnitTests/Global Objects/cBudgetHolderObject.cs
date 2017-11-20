using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cBudgetHolderObject
    {
        public static cBudgetHolder FromTemplate()
        {
            cBudgetHolder budgetHolder = new cBudgetHolder(0, "UTBH_" + DateTime.UtcNow.ToString() + " " + DateTime.UtcNow.TimeOfDay.ToString(), "Unit Test budget holder", cGlobalVariables.EmployeeID, cGlobalVariables.EmployeeID, DateTime.UtcNow, null, null);
            return budgetHolder;
        }

        public static int CreateID()
        {
            cBudgetholders budgetHolders = new cBudgetholders(cGlobalVariables.AccountID);
            cBudgetHolder budgetHolder = FromTemplate();

            int budgetHolderID = budgetHolders.saveBudgetHolder(budgetHolder);

            return budgetHolderID;
        }

        public static cBudgetHolder CreateObject()
        {
            int budgetHolderID = CreateID();
            cBudgetholders budgetHolders = new cBudgetholders(cGlobalVariables.AccountID);
            cBudgetHolder budgetHolder = budgetHolders.getBudgetHolderById(budgetHolderID);

            return budgetHolder;
        }
    }
}
