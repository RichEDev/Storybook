using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cItemRoleObject
    {
        /// <summary>
        /// Create the item role global static object
        /// </summary>
        /// <returns></returns>
        public static cItemRole CreateItemRole()
        {
            cItemRoles clsItemRoles = new cItemRoles(cGlobalVariables.AccountID);

            int ItemRoleID = clsItemRoles.addRole("Unit test item role" + DateTime.Now.Ticks.ToString(), "Unit test item role description", new List<cRoleSubcat>(), cGlobalVariables.EmployeeID);

            cItemRole role = clsItemRoles.getItemRoleById(ItemRoleID);
            cGlobalVariables.ItemRoleID = ItemRoleID;

            CreateRoleSubcat(ref role);

            return role;
        }

        /// <summary>
        /// Create a role subcat that will be added to the item role
        /// </summary>
        /// <param name="itemRole"></param>
        public static void CreateRoleSubcat(ref cItemRole itemRole)
        {
            cSubcat subcat = cSubcatObject.CreateDummySubcat();
            cItemRoles clsItemRoles = new cItemRoles(cGlobalVariables.AccountID);
            clsItemRoles.saveRoleSubcat(new cRoleSubcat(0, itemRole.itemroleid, subcat, 0,0, true));

            itemRole = clsItemRoles.getItemRoleById(itemRole.itemroleid);
        }

        /// <summary>
        /// Delete the item role from the datbase and the associated subcat
        /// </summary>
        public static void DeleteItemRole()
        {
            cItemRoles clsItemRoles = new cItemRoles(cGlobalVariables.AccountID);
            clsItemRoles.deleteRole(cGlobalVariables.ItemRoleID);
            cSubcatObject.DeleteSubcat();
        }
    }

    
}
