using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cConcurrentUserObject
    {
        public static void CreateConcurrentUser()
        {
            Guid newManageID = Guid.Empty;
            cConcurrentUsers cusers = new cConcurrentUsers(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            newManageID = cusers.LogonUser();
            cGlobalVariables.ConcurrentUserManageID = newManageID;

            return;
        }

        public static void RemoveConcurrentUser()
        {
            cConcurrentUsers cusers = new cConcurrentUsers(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);

            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string sql = "delete from accessManagement where manageID = @manageID";
            db.sqlexecute.Parameters.AddWithValue("@manageID", cGlobalVariables.ConcurrentUserManageID);
            db.ExecuteSQL(sql);

            cGlobalVariables.ConcurrentUserManageID = Guid.Empty;
            return;
        }
    }
}
