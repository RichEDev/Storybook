using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cDepItemObject
    {
        /// <summary>
        /// Create the department costcode breakdown global static object
        /// </summary>
        /// <returns></returns>
        public static cDepCostItem CreateDepCostItem()
        {
            cProjectCode projCode = cProjectCodeObject.CreateProjectCode();
            cCostCode costcode = cCostCodeObject.CreateCostCode();
            cDepartment department = cDepartmentObject.CreateDepartment();

            cDepCostItem depCostItem = new cDepCostItem(department.departmentid, costcode.costcodeid, projCode.projectcodeid, 100);

            return depCostItem;
        }

        /// <summary>
        /// Delete department costcode projectcode items
        /// </summary>
        public static void DeleteDepCostItem()
        {
            cProjectCodeObject.DeleteProjectcode();
            cCostCodeObject.DeleteCostcode();
            cDepartmentObject.DeleteDepartment();
        }
    }
}
