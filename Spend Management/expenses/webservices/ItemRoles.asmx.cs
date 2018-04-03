namespace Spend_Management.expenses.webservices
{
    using System;
    using System.Collections.Generic;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for List
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ItemRoles : WebService
    {
        /// <summary>
        /// Deletes an item role
        /// </summary>
        /// <param name="id">The id of the item role to delete</param>
        /// <returns>0 if deletion was successful and -99 if the user does not have permission to delete the item role</returns>
        [WebMethod]
        public int DeleteItemRole(int id)
        {
            var user = cMisc.GetCurrentUser();
            if (!user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ItemRoles, true, false))
            {
                return -99;
            }

            global::ItemRoles roles = new global::ItemRoles(user.AccountID);
            return roles.DeleteRole(id);
        }

        /// <summary>
        /// Adds or updates an item role
        /// </summary>
        /// <param name="id">The id of the item role</param>
        /// <param name="roleName">The name of the item role</param>
        /// <param name="description">A description for the item role</param>
        /// <returns>A positive item role id or a negative failure code</returns>
        [WebMethod]
        public int SaveItemRole(int id, string roleName, string description)
        {
            var user = cMisc.GetCurrentUser();

            if (id > 0)
            {
                if (!user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ItemRoles, true, false))
                {
                    return -99;
                }
            }
            else
            {
                if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ItemRoles, true, false))
                {
                    return -99;
                }
            }

            global::ItemRoles roles = new global::ItemRoles(user.AccountID);
            ItemRole itemRole = new ItemRole(id, roleName, description, null, DateTime.Now, 0, DateTime.Now, 0);
            return roles.SaveRole(itemRole, user);
        }

        /// <summary>
        /// Gets a list of expense items
        /// </summary>
        /// <returns>A List of expense items</returns>
        [WebMethod]
        public List<ListItem> GetExpenseItems()
        {
            var user = cMisc.GetCurrentUser();
            cSubcats subcats = new cSubcats(user.AccountID);
            return subcats.CreateDropDown();
        }

        /// <summary>
        /// Saves an expense item to item role association (rolesubcat)
        /// </summary>
        /// <param name="roleSubcatId">
        /// The id of the rolesubcat
        /// </param>
        /// <param name="itemRoleId">
        /// The item role id
        /// </param>
        /// <param name="subcatId">
        /// The expense item id
        /// </param>
        /// <param name="maximumLimitWithoutReceipt">
        /// The maximum Limit Without Receipt.
        /// </param>
        /// <param name="maximumLimitWithReceipt">
        /// The maximum Limit With Receipt.
        /// </param>
        /// <param name="addToTemplate">
        /// Whether to include this associated on the add expense "seleted items" template
        /// </param>
        /// <returns>
        /// 0 if saving was successful and -99 if the user does not have permission to save the item role
        /// </returns>
        [WebMethod]
        public int SaveExpenseItemToItemRoleAssociation (int roleSubcatId, int itemRoleId, int subcatId, decimal maximumLimitWithoutReceipt, decimal maximumLimitWithReceipt, bool addToTemplate)
        {
            var user = cMisc.GetCurrentUser();
            if (roleSubcatId > 0)
            {
                if (!user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ItemRoles, true, false))
                {
                    return -99;
                }
            }
            else
            {
                if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ItemRoles, true, false))
                {
                    return -99;
                }
            }

            global::ItemRoles itemRoles = new global::ItemRoles(user.AccountID);
            RoleSubcat roleSubcat = new RoleSubcat(roleSubcatId, itemRoleId, subcatId, maximumLimitWithoutReceipt, maximumLimitWithReceipt, addToTemplate);
            itemRoles.SaveRoleSubcat(roleSubcat);
            return 0;
        }

        /// <summary>
        /// Deletes an expense item to item role association (rolesubcat)
        /// </summary>
        /// <param name="roleSubcatId">
        /// The id of the rolesubcat to delete
        /// </param>
        /// <returns>
        /// 0 if deletion was successful and -99 if the user does not have permission to delete the associated expense
        /// </returns>
        [WebMethod]
        public int DeleteExpenseItemToItemRoleAssociation(int roleSubcatId)
        {
            var user = cMisc.GetCurrentUser();
            if (!user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ItemRoles, true, false))
            {
                return -99;
            }

            global::ItemRoles itemRoles = new global::ItemRoles(user.AccountID);
            itemRoles.DeleteExpenseItemToItemRoleAssociation(roleSubcatId, user);
            return 0;
        }

        /// <summary>
        /// Gets an instance of a role subcat
        /// </summary>
        /// <param name="itemRoleId">The id of the item role</param>
        /// <param name="roleSubcatId">The id of the rolesubcat</param>
        /// <returns>An instance of the associated expense item or null if the employee does not have permission to view</returns>
        [WebMethod]
        public RoleSubcat GetRoleSubcat(int itemRoleId, int roleSubcatId)
        {
            var user = cMisc.GetCurrentUser();
            if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true, false))
            {
                return null;
            }

            global::ItemRoles itemRoles = new global::ItemRoles(user.AccountID);
            return itemRoles.GetRoleSubcatById(itemRoleId, roleSubcatId);
        }
    }
}
