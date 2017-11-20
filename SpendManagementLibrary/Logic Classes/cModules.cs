namespace SpendManagementLibrary
{
	using System;
	using System.Collections.Generic;
    using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

    using Helpers;

	/// <summary>
    /// Modules from the metabase
    /// </summary>
    public class cModules
    {
		private string cacheSQL = "SELECT moduleID, moduleName, description, brandName, brandNameHTML FROM dbo.moduleBase";

        /// <summary>
        /// Constructor for cModules
        /// </summary>
        public cModules()
        {
            if (ModulesList == null)
            {
                ModulesList = CacheList();
            }
        }
        
        /// <summary>
        /// Places a List of cModule into cache and returns the same list
        /// </summary>
        /// <returns></returns>
        public List<cModule> CacheList()
        {
            int moduleID;

            #region Get module_base information            
            List<cModule> lstModules = new List<cModule>();
            DBConnection expdata = new DBConnection(GlobalVariables.MetabaseConnectionString);
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(this.cacheSQL))
            {
                string moduleName;
                string description;
                string brandNamePlainText;
                string brandNameHTML;
                cModule tmpModule;

                while (reader.Read())
                {
                    moduleID = reader.GetInt32(0);
                    moduleName = reader.GetString(1);

                    if (reader.IsDBNull(2) == true)
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(2);
                    }

                    brandNamePlainText = reader.GetString(3);
                    brandNameHTML = reader.GetString(4);

                    tmpModule = new cModule(moduleID, moduleName, description, brandNameHTML, brandNamePlainText);
                    lstModules.Add(tmpModule);
                }

                reader.Close();
            }
            #endregion


            return lstModules;
        }

        /// <summary>
        /// Returns the cModule object for the provided module enumerator
        /// </summary>
        /// <param name="moduleEnum">Module enumerator of the module to retrieve</param>
        /// <returns>cModule class object</returns>
        public cModule GetModuleByEnum(Modules moduleEnum)
        {
            cModule tmpModule = (from x in ModulesList
                                 where x.ModuleID == (int)moduleEnum
                                 select x).FirstOrDefault();

            return tmpModule;
        }

        /// <summary>
        /// Returns the required cModule or null if not found
        /// </summary>
        /// <param name="moduleID">Module ID to retrieve</param>
        /// <returns>cModule class object</returns>
        public cModule GetModuleByID(int moduleID)
        {
            cModule tmpModule = (from x in ModulesList
                                 where x.ModuleID == moduleID
                                 select x).FirstOrDefault();

            return tmpModule;
        }

        /// <summary>
        /// Get collection of elements assigned by category for a given module ID
        /// </summary>
        /// <param name="moduleID">Module to retrieve elements for</param>
        /// <returns></returns>
        public Dictionary<cElementCategory, SortedList<string,cElement>> GetCategoryElements(int accountID, int moduleID)
        {
            SortedList<int, SortedList<string, cElement>> retList = null;
            Dictionary<int, SortedList<int, SortedList<string, cElement>>> dicModuleCategoryElements = null;
            dicModuleCategoryElements = GetLicencedElementByCategory(accountID);

            if (dicModuleCategoryElements.ContainsKey(moduleID))
            {
                retList = dicModuleCategoryElements[moduleID];
            }

            if (retList == null)
            {
                retList = new SortedList<int, SortedList<string, cElement>>();
            }

            Dictionary<cElementCategory, SortedList<string, cElement>> sortedByCategoryList =
                new Dictionary<cElementCategory, SortedList<string, cElement>>();
            SortedList<string, int> sortedElementCategories = new SortedList<string, int>();
            cElementCategories cats = new cElementCategories();
            foreach (KeyValuePair<int, SortedList<string, cElement>> catKVP in retList)
            {
                int categoryId = (int) catKVP.Key;
                cElementCategory cat = cats.GetElementCategoryByID(categoryId);
                sortedElementCategories.Add(cat.ElementCategoryName, cat.ElementCategoryID);
            }

            foreach (KeyValuePair<string, int> kvp in sortedElementCategories)
            {
                cElementCategory cat = cats.GetElementCategoryByID((int) kvp.Value);
                if (retList.ContainsKey((int) kvp.Value))
                {
                    sortedByCategoryList.Add(cat, retList[(int) kvp.Value]);
                }
            }

            return sortedByCategoryList;
        }

	    private Dictionary<int, SortedList<int, SortedList<string, cElement>>> GetLicencedElementByCategory(int accountID)
        {
            #region collate module elements by category
            cElements elements = new cElements();
            SortedList<int, SortedList<string, cElement>> slCategoryElements;
            Dictionary<int, SortedList<int, SortedList<string, cElement>>> dicModuleCategoryElements = new Dictionary<int, SortedList<int, SortedList<string, cElement>>>();
            
            int elementID;
            int moduleID;

	        using (var metabase = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
	        {
	            metabase.AddWithValue("@accountId", accountID);
	            using (var reader = metabase.GetReader("GetLicencedElementsByAccountId", CommandType.StoredProcedure))
	            {
	                while (reader.Read())
	                {
	                    moduleID = reader.GetInt32(0);
	                    elementID = reader.GetInt32(1);

	                    cElement curElement = elements.GetElementByID(elementID);

	                    if (!dicModuleCategoryElements.ContainsKey(moduleID))
	                    {
	                        slCategoryElements = new SortedList<int, SortedList<string, cElement>>();
	                        dicModuleCategoryElements.Add(moduleID, slCategoryElements);
	                    }
	                    else
	                    {
	                        slCategoryElements = dicModuleCategoryElements[moduleID];
	                    }

	                    if (!slCategoryElements.ContainsKey(curElement.ElementCategoryID))
	                    {
	                        slCategoryElements.Add(curElement.ElementCategoryID, new SortedList<string, cElement>());
	                    }

	                    SortedList<string, cElement> categoryElements = slCategoryElements[curElement.ElementCategoryID];
	                    if (!categoryElements.ContainsKey(curElement.Name))
	                    {
	                        categoryElements.Add(curElement.Name, curElement);
	                    }
	                }
	                reader.Close();
	            }
	        }

            return dicModuleCategoryElements;
            #endregion
        }

        /// <summary>
        /// Returns a list of element IDs currently assigned to the module
        /// </summary>
        /// <param name="accountId">Account ID to retrieve element IDs for</param>
        /// <param name="module">Module to retrieve element IDs for</param>
        /// <returns>List of Element IDs</returns>
        public List<int> GetModuleElementIds(int accountId, Modules module)
        {
            if(GetModuleByEnum(module).Elements ==  null)
            {
                LoadElements(accountId, module);
            }

            List<int> rList = (from y in GetModuleByEnum(module).Elements
                               select y.ElementID
                              ).ToList();

            return rList;
        }

        /// <summary>
        /// Populates the Elements property by reference into the cached object
        /// </summary>
        /// <param name="accountId">Account ID to obtain element IDs for</param>
        /// <param name="module">Module to obtain element IDs for</param>
        private void LoadElements(int accountId, Modules module)
        {
            cElements elements = new cElements();
            DBConnection db = new DBConnection(GlobalVariables.MetabaseConnectionString);
            string sql = "select accountsLicencedElements.elementID from accountsLicencedElements inner join moduleElementBase on accountsLicencedElements.elementID = moduleElementBase.elementID where accountID = @accId and moduleID = @modId";

            cElement element = null;
            List<cElement> eList = new List<cElement>();
            db.sqlexecute.Parameters.AddWithValue("@accId", accountId);
            db.sqlexecute.Parameters.AddWithValue("@modId", (int) module);
            using (SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    element = elements.GetElementByID(reader.GetInt32(0));
                    if(element != null && !eList.Contains(element))
                    {
                        eList.Add(element);
                    }
                }
                reader.Close();
            }
            GetModuleByEnum(module).Elements = eList;
        }

		/// <summary>
		/// Gets a list of available modules
		/// </summary>
		public static List<cModule> ModulesList { get; private set; }

		/// <summary>
		/// The get default homepage for module.
		/// </summary>
		/// <param name="module">
		/// The module.
		/// </param>
		/// <returns>
		/// The default home page.
		/// </returns>
		public static string GetDefaultHomepageForModule(Modules module)
		{
			string defaultHomepage;
			switch (module)
			{
				case Modules.PurchaseOrders:
				case Modules.expenses:
				case Modules.contracts:
				case Modules.SpendManagement:
				case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
					defaultHomepage = "~/home.aspx";
					break;
				case Modules.SmartDiligence:
					defaultHomepage = "~/shared/viewentities.aspx?entityid=33&viewid=16";
					break;
                case Modules.CorporateDiligence:
                    defaultHomepage = "~/home.aspx";
                    break;
				default:
					throw new ArgumentOutOfRangeException("module");
			}

			return defaultHomepage;
		}
    }
}
