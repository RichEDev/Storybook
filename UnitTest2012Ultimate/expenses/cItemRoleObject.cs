using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate
{
    internal class cItemRoleObject
    {
        public static cItemRole New(cItemRole itemRole, cSubcat subcat, int employeeId = 0)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cItemRoles clsItemRoles = new cItemRoles(currentUser.AccountID);

            int ItemRoleID = clsItemRoles.addRole(itemRole.rolename, itemRole.description, new List<cRoleSubcat>(), currentUser.EmployeeID);

            cItemRole newItemRole = clsItemRoles.getItemRoleById(ItemRoleID);

            clsItemRoles.saveRoleSubcat(new cRoleSubcat(0, newItemRole.itemroleid, subcat.subcatid, 0, 0, true));

            if (employeeId > 0)
            {
                // save the item role against the employee
                DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
                const string sql = "insert into employee_roles (employeeid, itemroleid, [order]) values (@employeeid, @itemroleid, 1)";
                db.sqlexecute.Parameters.AddWithValue("@employeeid", currentUser.EmployeeID);
                db.sqlexecute.Parameters.AddWithValue("@itemroleid", ItemRoleID);
                db.ExecuteSQL(sql);
            }
            return clsItemRoles.getItemRoleById(newItemRole.itemroleid);
        }

        public static cItemRole Template(int itemroleid = 0, string rolename = default(string), string description = default(string), Dictionary<int, cRoleSubcat> subcats = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
            rolename = (rolename == default(string) ? "Unit test item role" + DateTime.Now.Ticks.ToString() : rolename);
            description = (description == default(string) ? "Unit test item role description" : description);

            return new cItemRole(itemroleid, rolename, description, subcats, DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID);
        }

        public static void TearDown(int employeeId, int itemRoleId)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cItemRoles clsItemRoles = new cItemRoles(currentUser.AccountID);
            clsItemRoles.deleteRole(itemRoleId);
        }
    }
}
