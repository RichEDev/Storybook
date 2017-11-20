using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Web.Caching;

namespace SpendManagementLibrary
{
    public class cElementCategories
    {
        private static List<cElementCategory> lstCachedElementCategories;

        /// <summary>
        /// Constructor for cElementCategories
        /// </summary>
        public cElementCategories()
        {
            if (lstCachedElementCategories == null)
            {
                lstCachedElementCategories = CacheList();
            } 
        }
        
        /// <summary>
        /// Places a List of cModule into cache and returns the same list
        /// </summary>
        /// <returns></returns>
        public List<cElementCategory> CacheList()
        {
            List<cElementCategory> lstCategories = new List<cElementCategory>();
            DBConnection expdata = new DBConnection(GlobalVariables.MetabaseConnectionString);

            string strSQL = "SELECT categoryID, categoryName, description FROM dbo.elementCategoryBase";
            
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                int categoryID;
                string categoryName;
                string description;

                cElementCategory tmpeElementCategory;

                while (reader.Read())
                {
                    categoryID = reader.GetInt32(0);
                    categoryName = reader.GetString(1);

                    if (reader.IsDBNull(2) == true)
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(2);
                    }

                    tmpeElementCategory = new cElementCategory(categoryID, categoryName, description);
                    lstCategories.Add(tmpeElementCategory);
                }

                reader.Close();
            }

            return lstCategories;
        }

        /// <summary>
        /// Returns the required Element Category or null if not found
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public cElementCategory GetElementCategoryByID(int categoryID)
        {
            cElementCategory tmpCat = null;
            foreach (cElementCategory cat in lstCachedElementCategories)
            {
                if (cat.ElementCategoryID == categoryID)
                {
                    tmpCat = cat;
                    break;
                }
            }

            return tmpCat;
        }

        /// <summary>
        /// Gets a list of available modules
        /// </summary>
        public static List<cElementCategory> Categories { get { return lstCachedElementCategories; } }
    }
}
