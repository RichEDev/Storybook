using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;


namespace SpendManagementUnitTests.Global_Objects
{
    public class cProjectCodeObject
    {
        /// <summary>
        /// Create global static project code object
        /// </summary>
        /// <returns></returns>
        public static cProjectCode CreateProjectCode()
        {
            cProjectCodes clsProjectCodes = new cProjectCodes(cGlobalVariables.AccountID);
            int tempProjectCodes = clsProjectCodes.saveProjectCode(new cProjectCode(0, "Unit Test Project Code " + DateTime.Now.ToString(), "Unit Test Project Code Description", false, false, new DateTime(), cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>()));
            clsProjectCodes = new cProjectCodes(cGlobalVariables.AccountID);
            cProjectCode tempProjectCode = clsProjectCodes.getProjectCodeById(tempProjectCodes);
            cGlobalVariables.ProjectcodeID = tempProjectCodes;

            return tempProjectCode;
        }

        /// <summary>
        /// Delete project code from the database
        /// </summary>
        public static void DeleteProjectcode()
        {
            cProjectCodes clsProjectCodes = new cProjectCodes(cGlobalVariables.AccountID);
            clsProjectCodes.deleteProjectCode(cGlobalVariables.ProjectcodeID, cGlobalVariables.EmployeeID);
        }
    }
}
