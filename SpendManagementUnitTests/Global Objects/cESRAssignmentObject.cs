using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cESRAssignmentObject
    {
        /// <summary>
        /// Create the global object for the ESR Assignment
        /// </summary>
        /// <returns></returns>
        public static cESRAssignment CreateESRAssignment()
        {
            cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            int tempAssignmentID = clsAssign.saveESRAssignment(new cESRAssignment(0, 0, "12345678" + DateTime.Now.Ticks.ToString(), new DateTime(2010,01,01), null, ESRAssignmentStatus.ActiveAssignment, "", "", "", "Unit Test House", "Unit Test Lane", "Town", "County", "LN6 3JY", "GB", false, "87654321", "87654321", "Unit test supervisor", "", "", "", true, 0,"", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, true, DateTime.Now, cGlobalVariables.EmployeeID, null, null));
            cGlobalVariables.ESRAssignmentID = tempAssignmentID;
            
            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);

            //clsAssign = new cESRAssignments(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            cESRAssignment ESRAssignment = clsAssign.getAssignmentById(tempAssignmentID);
            return ESRAssignment;
        }

        /// <summary>
        /// Delete the global ESR Assignment object from the database 
        /// </summary>
        public static void DeleteESRAssignment()
        {
            cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            clsAssign.deleteESRAssignment(cGlobalVariables.ESRAssignmentID);
        }
    }
}
