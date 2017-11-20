using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cCostCodeObject
    {
        /// <summary>
        /// Create a global static costcode object
        /// </summary>
        /// <returns></returns>
        public static cCostCode CreateCostCode()
        {
            cCostcodes clsCostCodes = new cCostcodes(cGlobalVariables.AccountID);
            int tempCostCodesId = clsCostCodes.saveCostcode(new cCostCode(0, "Unit Test Cost Code " + DateTime.Now.Ticks.ToString(), "Unit Test Cost Code Description", false, new DateTime(), cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>()));
            clsCostCodes = new cCostcodes(cGlobalVariables.AccountID);
            cCostCode tempCostCode = clsCostCodes.GetCostCodeById(tempCostCodesId);
            cGlobalVariables.CostcodeID = tempCostCodesId;
            return tempCostCode;
        }

        /// <summary>
        /// Delete the costcode from the database
        /// </summary>
        public static void DeleteCostcode()
        {
            cCostcodes clsCostCodes = new cCostcodes(cGlobalVariables.AccountID);
            clsCostCodes.deleteCostCode(cGlobalVariables.CostcodeID, cGlobalVariables.EmployeeID);
        }
    }
}
