using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cDepartmentObject
    {
        /// <summary>
        /// Create global static department object
        /// </summary>
        /// <returns></returns>
        public static cDepartment CreateDepartment()
        {
            cDepartments clsDepartments = new cDepartments(cGlobalVariables.AccountID);
            int tempDepartmentId = clsDepartments.saveDepartment(new cDepartment(0, "Unit Test Department " + DateTime.Now.ToString(), "Unit Test Department Description", false, new DateTime(), cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>()));
            clsDepartments = new cDepartments(cGlobalVariables.AccountID);
            cDepartment tempDepartment = clsDepartments.GetDepartmentById(tempDepartmentId);
            cGlobalVariables.DepartmentID = tempDepartmentId;
            return tempDepartment;
        }

        /// <summary>
        /// Delete the department from the database
        /// </summary>
        public static void DeleteDepartment()
        {
            cDepartments clsDepartments = new cDepartments(cGlobalVariables.AccountID);
            clsDepartments.deleteDepartment(cGlobalVariables.DepartmentID, cGlobalVariables.EmployeeID);
        }
    }
}
