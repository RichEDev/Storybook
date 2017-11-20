namespace Spend_Management.shared.code.GreenLight
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.GreenLight;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// A class to represent the collection of GreenLight views that have been disabled within the menus of the modules available for the account.
    /// </summary>
    [Serializable]
    public class DisabledModuleMenuViews
    {
        /// <summary>
        /// The sql string used for caching the disabled views table
        /// </summary>
        private const string CachingSql = "SELECT ViewId, MenuId FROM dbo.CustomEntityDisabledModuleMenuView WHERE moduleId = {1} and {0} = {0}";

        /// <summary>
        /// The sql used as part of the caching dependency from GreenLight views.
        /// </summary>
        private const string ViewCachingSql = "SELECT CacheExpiry FROM dbo.customEntityViews WHERE {0} = {0}";

        /// <summary>
        /// The sql used as part of the caching dependency from the menu structure.
        /// </summary>
        private const string MenuCachingSql =
            "SELECT menuid, menu_name, parentid FROM dbo.customMenuStructure WHERE {0} = {0}";

        /// <summary>
        /// The cache object container.
        /// </summary>
        private readonly string _cacheKey;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int _accountId;

        /// <summary>
        /// The current module id.
        /// </summary>
        private readonly int _moduleId;

        /// <summary>
        /// Initialises a new instance of the <see cref="DisabledModuleMenuViews"/> class. Handles caching of all the disabled GreenLight views for menus within GreenLight modules.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="moduleId">
        /// The current module Id.
        /// </param>
        public DisabledModuleMenuViews(int accountId, int moduleId)
        {
            this._accountId = accountId;
            this._moduleId = moduleId;
            this._cacheKey = string.Format("DisabledModuleMenuViews_{0}_{1}", this._accountId, this._moduleId);
            this.RefreshCache();
        }

        #region properties

        /// <summary>
        /// Gets or Sets cached list of disabled views for the menus within the modules for the account.
        /// </summary>
        public List<MenuView> CachedList { get; private set; }

        #endregion

        #region public methods

        /// <summary>
        /// If the menu and view combination is in the cached list for this module, the view is currently disabled.
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <param name="viewId">View Id</param>
        /// <returns>Boolean indication disabled status of the view for the current module.</returns>
        public bool IsViewDisabled(int menuId, int viewId)
        {
            return this.CachedList.FirstOrDefault(x => x.MenuId == menuId && x.ViewId == viewId) != null;
        }
        
        /// <summary>
        /// Disables a view menu for any number of modules
        /// </summary>
        /// <param name="menuId">The menu id</param>
        /// <param name="viewId">The custom entity view id</param>
        /// <param name="modulesToDisable">The module ids to disable</param>
        /// <returns>A success indicator</returns>
        public int DisableForModules(int menuId, int viewId, List<int> modulesToDisable)
        {
            int result;
            using (var database = new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                database.AddWithValue("@menuId", menuId);
                database.AddWithValue("@viewId", viewId);
                database.AddWithValue("@moduleIds", modulesToDisable);

                database.AddReturn("@returnValue", SqlDbType.Int);
                database.ExecuteProc("SaveCustomEntityDisabledModuleMenuViews");

                result = database.GetReturnValue<int>("@returnValue");
                database.ClearParameters();
            }

            return result;
        }

        /// <summary>
        /// Gets a list of disabled module ids for a view menu
        /// </summary>
        /// <param name="viewId">The custom entity view id</param>
        /// <returns>A list of module ids</returns>
        public List<int> GetDisabledModuleIds(int viewId)
        {
            var result = new List<int>();
            using (var database = new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                database.AddWithValue("@viewId", viewId);
                using (IDataReader reader = database.GetReader("SELECT moduleid FROM CustomEntityDisabledModuleMenuView WHERE viewid = @viewId"))
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }
                }

                database.ClearParameters();
            }

            return result;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Cache the disabled views from database into collection
        /// </summary>
        /// <returns>List of Menu items.</returns>
        private List<MenuView> CacheList()
        {
            var moduleId = this._moduleId;
            string cacheSql = string.Format(CachingSql, this._accountId, moduleId);
            var disabledViews = new List<MenuView>();
            var connection = new DBConnection(cAccounts.getConnectionString(this._accountId))
                                 {
                                     sqlexecute =
                                         {
                                             CommandText
                                                 =
                                                 cacheSql
                                         }
                                 };

            connection.sqlexecute.Parameters.AddWithValue("@moduleId", moduleId);

            using (SqlDataReader reader = connection.GetReader(connection.sqlexecute.CommandText))
			{
                int viewIdPos = reader.GetOrdinal("ViewId");
                int menuIdPos = reader.GetOrdinal("MenuId");
                
                while (reader.Read())
                {
                    int viewId = reader.GetInt32(viewIdPos);
                    int menuId = reader.GetInt32(menuIdPos);
                    var disabledView = new MenuView(menuId, viewId);

                    disabledViews.Add(disabledView);
                }

                connection.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            var cache = new Caching();

            var sqlMonitors = new List<string>
                                  {
                                      cacheSql,
                                      string.Format(MenuCachingSql, this._accountId),
                                      string.Format(ViewCachingSql, this._accountId)
                                  };

            cache.Add(this._cacheKey, disabledViews, sqlMonitors, Caching.CacheTimeSpans.Permanent, Caching.CacheDatabaseType.Customer, this._accountId);

			return disabledViews;
        }

        /// <summary>
        /// Refresh the cache 
        /// </summary>
        private void RefreshCache()
        {
            var cache = new Caching();

            if (cache.Cache.Contains(this._cacheKey))
            {
                this.CachedList = (List<MenuView>)cache.Cache.Get(this._cacheKey);
            }
            else
            {
                //// if have to cache fields, ensure combined list doesn't remain in cache
                this.CachedList = this.CacheList();
            }
        }

        #endregion
    }
}