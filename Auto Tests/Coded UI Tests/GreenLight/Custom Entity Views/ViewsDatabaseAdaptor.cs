using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Auto_Tests.Tools;
using System.Data.SqlClient;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Views;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.CustomVEntityViews.ViewsDatabaseAdaptor
{
    /// <summary>
    /// Classed user for interaction with the testing database.
    /// </summary>
    public class ViewsDatabaseAdaptor
    {
        /// <summary>
        /// Sql string for obtaining a list of viewnames and description altered by sorting
        /// </summary>
        private static readonly string ViewsSqlSortingOrder = "SELECT view_name, description FROM customEntityViews WHERE entityid = @entityid ORDER BY {0} {1}";

        /// <summary>
        /// Used to return the correct sorting order from the database 
        ///</summary>
        public static List<CustomEntitiesUtilities.CustomEntityView> GetCorrectSortingOrderFromDB(cDatabaseConnection databaseConnection, SortViewsByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid)
        {
            List<CustomEntitiesUtilities.CustomEntityView> views = new List<CustomEntitiesUtilities.CustomEntityView>();
            databaseConnection.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            SqlDataReader reader = databaseConnection.GetReader(string.Format(ViewsSqlSortingOrder, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(sortingOrder)));
            while (reader.Read())
            {
                views.Add(new CustomEntitiesUtilities.CustomEntityView()
                {
                    _viewName = reader.GetString(0), 
                    _description = reader.GetString(1)
                });
            }
            reader.Close();
            return views;
        }
    }
}
