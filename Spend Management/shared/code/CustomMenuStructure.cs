namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using System.Data;
    using Spend_Management.shared.code.GreenLight;
    using Utilities.DistributedCaching;
    using System.Linq;
    /// <summary>
    /// Class includes Data access methods related to Custom menus.
    /// </summary>
    [Serializable]
    public class CustomMenuStructure
    {
        /// <summary>
        /// Account id
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// Audit log.
        /// </summary>
        private cAuditLog clsaudit;

        /// <summary>
        /// Cache object.
        /// </summary>
        private Cache cache = new Cache();
        /// <summary>
        /// Caching area.
        /// </summary>
        private const string CacheArea = "custommenu";

        /// <summary>
        /// Custom menu structure object.
        /// </summary>
        private Dictionary<int, CustomMenuStructureItem> customMenuList;

        /// <summary>
        /// Default constractor.
        /// </summary>
        public CustomMenuStructure(int accountId)
        {
            this.AccountId = accountId;
           this.clsaudit = new cAuditLog();
            this.InitialiseData();
        }

        /// <summary>
        /// This method returns a menu item from the cachelist  based on id.
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public CustomMenuStructureItem GetCustomMenuById(int menuId)
        {
            CustomMenuStructureItem customMenu;
            this.customMenuList.TryGetValue(menuId, out customMenu);
            return customMenu;
        }

        /// <summary>
        /// Get custommenu by menu name.
        /// </summary>
        /// <param name="menuName">custom menu name</param>
        /// <param name="isSystem">system menu</param>
        /// <returns></returns>
        public int GetCustomMenuIdByName(string menuName,bool isSystem)
        {
            var menuIdlist= this.customMenuList.Values.Where(item => item.CustomMenuName == menuName && item.SystemMenu ==isSystem).Select(a => a.CustomMenuId).ToList();
            return menuIdlist.Count > 0 ? menuIdlist[0] : 0;
        }

        /// <summary>
        /// Get custommenu name by menu id.
        /// </summary>
        /// <param name="menuId">custom menu id</param>
        /// <param name="isSystem">system menu</param>
        /// <returns></returns>
        public string GetCustomMenuNameById(int menuId, bool isSystem)
        {
            var menuName = this.customMenuList.Values.Where(item => item.CustomMenuId == menuId && item.SystemMenu == isSystem).Select(obj => obj.CustomMenuName).ToList();
            return menuName.Count > 0 ? menuName[0] : null;
        }


        /// <summary>
        /// Get child nodes for menu items.
        /// </summary>
        /// <param name="parentId"></param>
        public List<CustomMenuStructureItem> GetCustomMenusByParentId(int? parentId)
        {
            return this.customMenuList.Values.Where(x => x.CustomParentId == parentId).ToList();
        }

        /// <summary>
        /// Delete custom menu by custom menuid.
        /// </summary>
        /// <param name="customMenuId">custom menu id</param>
      
        public void DeleteCustomMenu(int customMenuId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@custommenuid", customMenuId);
                connection.ExecuteProc("DeleteCustomMenu");
                connection.sqlexecute.Parameters.Clear();
                this.InvalidateCache();
                var customMenu = this.GetCustomMenuById(customMenuId);
                this.clsaudit.deleteRecord(SpendManagementElement.GreenLightMenu, customMenuId, customMenu.CustomMenuName);
            }
        }

        /// <summary>
        /// Add or update custom menu.
        /// </summary>
        /// <param name="customMenuItems">Details of custom menu</param>
        public List<string> AddOrUpdateCustomMenu(List<CustomMenuStructureItem> customMenuItems)
        {
            var result = new List<string>();
            var newItemsList = new Dictionary<string, int>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                int referenceParentId;
                foreach (var item in customMenuItems)
                {
                    if (item.ReferenceDynamicParentId != null)
                    {
                        if (newItemsList.TryGetValue(item.ReferenceDynamicParentId, out referenceParentId))
                        {
                            connection.sqlexecute.Parameters.AddWithValue("@parentmenuid", referenceParentId);
                        }
                    }
                    else
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@parentmenuid", item.CustomParentId);
                    }
                        connection.sqlexecute.Parameters.AddWithValue("@custommenuid", item.CustomMenuId);
                        connection.sqlexecute.Parameters.AddWithValue("@name", item.CustomMenuName);
                        connection.sqlexecute.Parameters.AddWithValue("@description",item.CustomMenuDescription ?? (object)DBNull.Value);
                        connection.sqlexecute.Parameters.AddWithValue("@menuicon", item.CustomMenuIcon ?? (object)DBNull.Value);
                        connection.sqlexecute.Parameters.AddWithValue("@createdby", item.CreatedBy ?? (object)DBNull.Value);
                        connection.sqlexecute.Parameters.AddWithValue("@modifiedby", item.ModifiedBy ?? (object)DBNull.Value);
                        connection.sqlexecute.Parameters.AddWithValue("@orderby", item.OrderBy);
                        connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int).Direction =ParameterDirection.ReturnValue;
                        connection.ExecuteProc("AddOrUpdateCustomMenu");
                        int returnValue =(int)connection.sqlexecute.Parameters["@returnvalue"].Value;
                        if (item.ReferenceId != null)
                        {
                         newItemsList.Add(item.ReferenceId, returnValue);
                        }
                        connection.sqlexecute.Parameters.Clear();
                        if (returnValue == 0)
                        {
                            result.Add(item.CustomMenuName);
                        }
                    if (item.CustomMenuId == 0)
                    {
                        this.clsaudit.addRecord(SpendManagementElement.GreenLightMenu, item.CustomMenuName, returnValue);
                    }
                    else
                    {
                        var customMenu = this.GetCustomMenuById(item.CustomMenuId);
                        this.clsaudit.editRecord(
                            item.CustomMenuId,
                            customMenu.CustomMenuName,
                            SpendManagementElement.GreenLightMenu,
                            Guid.Empty,
                            customMenu.CustomMenuName,
                            item.CustomMenuName);
                    }
                }
                this.InvalidateCache();
                return result;
            }
        }
        /// <summary>
        /// Get all custom menu.
        /// </summary>
        public Dictionary<int,CustomMenuStructureItem> CacheList()
        {
            Dictionary<int,CustomMenuStructureItem> menuItemList = new Dictionary<int,CustomMenuStructureItem>();
            string menuName, menuDescription , menuIcon ;
             int parentId , orderBy;
           
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
                using (var reader = connection.GetReader("GetAllCustomMenu", CommandType.StoredProcedure))
                {
                    var menuIdOrd = reader.GetOrdinal("CustomMenuId");

                    var menuNameOrd = reader.GetOrdinal("Name");
                    var menuDescriptionOrd = reader.GetOrdinal("Description");
                    var parentIdOrd = reader.GetOrdinal("ParentMenuId");
                    var menuIconOrd = reader.GetOrdinal("MenuIcon");
                    var orderByOrd = reader.GetOrdinal("OrderBy");
                    var systemMenuOrd = reader.GetOrdinal("SystemMenu");

                    while (reader.Read())
                    {
                        int menuId =reader.GetInt32(menuIdOrd);
                        menuName = !reader.IsDBNull(menuNameOrd) ? reader.GetString(menuNameOrd) : null;
                        menuDescription = !reader.IsDBNull(menuDescriptionOrd) ? reader.GetString(menuDescriptionOrd) : null;
                        parentId = !reader.IsDBNull(parentIdOrd) ? reader.GetInt32(parentIdOrd) : 0;
                        menuIcon = !reader.IsDBNull(menuIconOrd) ? reader.GetString(menuIconOrd) : null;
                        orderBy = !reader.IsDBNull(orderByOrd) ? reader.GetInt32(orderByOrd) : 0;
                        bool systemMenu = reader.GetBoolean(systemMenuOrd);
                        menuItemList.Add(menuId,new CustomMenuStructureItem(menuId, menuName, parentId, menuDescription, menuIcon, orderBy, systemMenu));
                    }
                    reader.Close();
                    this.cache.Add(this.AccountId, CacheArea, "0", menuItemList);
                return menuItemList;
            }
        }


        /// <summary>
        /// Cache validation
        /// </summary>
        private void InvalidateCache()
        {
            this.cache.Delete(this.AccountId, CacheArea, "0");
        }
        /// <summary>
        /// Initialise data.
        /// </summary>
        private void InitialiseData()
        {
            this.customMenuList = this.cache.Get(this.AccountId, CacheArea, "0") as Dictionary<int, CustomMenuStructureItem>
                            ?? this.CacheList();
        }
        /// <summary>
        /// Check for the view
        /// </summary>
        /// <param name="customMenuId">Custom menu id</param>
        public bool HasView(int customMenuId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@CustomMenuId", customMenuId);
                connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("CheckIfCustomMenuHasView");
                int returnValue = (int)connection.sqlexecute.Parameters["@returnvalue"].Value;
                connection.sqlexecute.Parameters.Clear();
                return returnValue>0;
            }
        }

    }
}