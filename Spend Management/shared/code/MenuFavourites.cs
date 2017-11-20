using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Primary Menus class
    /// </summary>
    public class MenuFavourites
    {
        /// <summary>
        /// Cache key for this class
        /// </summary>
        public const string CacheKey  = "FavMenuItems";
		private readonly Utilities.DistributedCaching.Cache _cache = new Utilities.DistributedCaching.Cache();
        private int Accountid;
        private ICurrentUser CurUser;

        /// <summary>
        /// MenuFavourites constructor initialises the caching of all of the menu favourites in the database for the specified user.
        /// </summary>
        /// <param name="currentUser">The current user object</param>
        public MenuFavourites(ICurrentUser currentUser)
        {
            this.CurUser = currentUser;
            this.Accountid = currentUser.AccountID;
            this.InitialiseData();
        }

        #region properties

        /// <summary>
        /// Gets the Cached List of Menu Favourites for the Employee
        /// </summary>
        public SortedList<int, MenuFavourite> CachedList { get; private set; }

        #endregion

        /// <summary>
        /// Save favorite menu ino the database
        /// </summary>
        /// <param name="menuTitle">Menu Item Title</param>
        /// <param name="iconLocation">Icon file location</param>
        /// <param name="onclickUrl">URL for onClick event</param>
        /// <param name="order">order of menu item</param>
        /// <param name="employeeID">Employee ID of he favorite menu</param>
        /// <returns>Put something here 4</returns>
        public int SaveFavouriteMenuItem(string menuTitle, string iconLocation, string onclickUrl, byte order, int employeeID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.Accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@menuTitle", menuTitle);
            expdata.sqlexecute.Parameters.AddWithValue("@iconLocation", iconLocation);
            expdata.sqlexecute.Parameters.AddWithValue("@onclickUrl", onclickUrl);
            expdata.sqlexecute.Parameters.AddWithValue("@order", order);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveEmployeeMenuFavourite");
            int id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            this.RefreshCache();

            return id;
        }

        /// <summary>
        /// Remove the favorite menu item from the database
        /// </summary>
        /// <param name="menuFavouriteID">ID of the menu item to remove</param>
        /// <param name="employeeID">ID of the employee</param>
        /// <returns>Success integer from the stored proc. 1 For success</returns>
        public int DeleteFavouriteMenuItem(int menuFavouriteID, int employeeID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.Accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@menuFavouriteID", menuFavouriteID);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteEmployeeMenuFavourite");
            int id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            this.RefreshCache();

            return id;
        }

        /// <summary>
        /// Get a list of the favorite menu items
        /// </summary>
        /// <param name="appPath">application path or Icons</param>
        /// <returns>returns a string</returns>
        public string SetupFavouriteMenuItems(string appPath)
        {
            StringBuilder output = new StringBuilder("<div id='favouritesArea' style='display:none;'>");
			
            var documentLocation = "document.location='" + appPath;

            if (this.CurUser.Employee.AdminOverride && this.CurUser.isDelegate == false)
            {
            	output.Append("<span id=\"favouritesContainer\">");

                foreach (MenuFavourite menuFavourite in this.CachedList.Values)
                {
                    var title = menuFavourite.Title;
					var imgSource = GlobalVariables.StaticContentLibrary + "/icons/48/plain/" + menuFavourite.IconLocation;

                    // Ensure we use the correct Title text and Image source if the menu icon is from a GreenLight View
                    if (menuFavourite.OnClickUrl.Contains("?entityid=") && menuFavourite.OnClickUrl.Contains("&viewid="))
                    {
                        string[] greenLightIds = menuFavourite.OnClickUrl.Replace("/shared/viewentities.aspx?entityid=", string.Empty).Split(new string[] { "&viewid=" }, StringSplitOptions.RemoveEmptyEntries);

                        if (greenLightIds.Length == 2)
                        {
                            var entityID = Convert.ToInt32(greenLightIds[0]);
                            var viewID = Convert.ToInt32(greenLightIds[1].Replace("';", string.Empty));

                            cCustomEntities customEntities = new cCustomEntities(this.CurUser);
                            var customEntity = customEntities.getEntityById(entityID);

                            if (customEntity != null)
                            {
                                var customEntityView = customEntity.getViewById(viewID);

                                if (customEntityView != null)
                                {
                                    title = customEntityView.viewname;
									imgSource = GlobalVariables.StaticContentLibrary + "/icons/48/plain/" + customEntityView.MenuIcon;
                                }
                            }
                        }
                    }

                    output.Append("<span id=\"" + menuFavourite.MenuFavouriteID + "\" class=\"favMenuItem\" onclick=\"" + documentLocation + menuFavourite.OnClickUrl + "\">");

                    output.Append("<span class=\"favMenuImage\"><img src=\"" + imgSource + "\" title=\"" + menuFavourite.Title + "\"/></span>");

                    output.Append("<span class=\"favMenuTitle\">" + title + "</span>");

                    output.Append("</span>");
                }

				output.Append("</span>");
				output.Append("<span id=\"favouritesSaveIndicator\"></span>");
            }

            output.Append("</div>");            

            return output.ToString();
        }

        /// <summary>
        /// Initialises data collection into memory
        /// </summary>
		private void InitialiseData()
		{
			this.CachedList = (SortedList<int, MenuFavourite>)this._cache.Get(this.Accountid, CacheKey, CurUser.EmployeeID.ToString()) ?? this.CacheList();
		}

        /// <summary>
        /// Cache favourite menu items from database into collection
        /// </summary>
        /// <returns>SortedList of favourite menu items</returns>
        private SortedList<int, MenuFavourite> CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.Accountid));
            var menuFavourites = new SortedList<int, MenuFavourite>();
            var cachingSql =
                "SELECT MenuFavouriteID, Title, IconLocation, OnClickUrl, [Order] FROM dbo.employeeMenuFavourites WHERE EmployeeID = @employeeID";

            expdata.sqlexecute.CommandText = cachingSql;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", this.CurUser.EmployeeID);

            using (SqlDataReader reader = expdata.GetReader(expdata.sqlexecute.CommandText))
            {
                var menuFavIDOrdID = reader.GetOrdinal("MenuFavouriteID");
                var titleOrdID = reader.GetOrdinal("Title");
                var iconLocationOrdID = reader.GetOrdinal("IconLocation");
                var onclickUrlOrdID = reader.GetOrdinal("OnClickUrl");
                var orderOrdID = reader.GetOrdinal("Order");

                while (reader.Read())
                {
                    var menuFavouriteID = reader.GetInt32(menuFavIDOrdID);
                    var title = reader.GetString(titleOrdID);
                    var iconLocation = reader.GetString(iconLocationOrdID);
                    var onclickUrl = reader.GetString(onclickUrlOrdID);
                    var order = reader.GetByte(orderOrdID);

                    menuFavourites.Add(menuFavouriteID,
                        new MenuFavourite(menuFavouriteID, title, iconLocation, onclickUrl, order));
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }


            this._cache.Add(this.Accountid, CacheKey, this.CurUser.EmployeeID.ToString(), menuFavourites);

            return menuFavourites;
        }

        /// <summary>
        /// Refresh the cache 
        /// </summary>
        private void RefreshCache()
        {
            this._cache.Delete(this.Accountid, CacheKey, this.CurUser.EmployeeID.ToString());
            this.InitialiseData();
        }
    }
}
