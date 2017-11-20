

namespace SpendManagementLibrary
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.Helpers;

    public class cElements
    {
        #region Global Variables
            private static List<cElement> lstCachedElements;
        #endregion Global Variables

        #region Public Methods
        /// <summary>
        /// Constructor for cElements
        /// </summary>
        public cElements()
        {
          
            if (lstCachedElements == null)
            {
                lstCachedElements = CacheList();
            }
        }

        /// <summary>
        /// Places a List of cElement into cache and returns the same list
        /// </summary>
        /// <returns></returns>
        public List<cElement> CacheList()
        {
           
            List<cElement> lstElements = new List<cElement>();
            DBConnection expdata = new DBConnection(System.Configuration.ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            //                          0           1           2           3               4                   5                   6                               7
            string strSQL = "SELECT elementID, categoryID, elementName, description, accessRolesCanEdit, accessRolesCanAdd, accessRolesCanDelete, elementFriendlyName, accessRolesApplicable FROM dbo.elementsBase";
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                int elementID, categoryID;
                string elementName, description, friendlyName;
                bool canEdit, canDelete, canAdd;
                cElement tmpElement;

                while (reader.Read())
                {
                    elementID = reader.GetInt32(0);
                    categoryID = reader.GetInt32(1);
                    elementName = reader.GetString(2);
                    if (reader.IsDBNull(3) == true)
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(3);
                    }

                    canEdit = reader.GetBoolean(4);
                    canAdd = reader.GetBoolean(5);
                    canDelete = reader.GetBoolean(6);
                    friendlyName = reader.GetString(7);
                    var accessRolesApplicable = reader.GetBoolean(8);
                    tmpElement = new cElement(elementID, categoryID, elementName, description, canAdd, canEdit, canDelete, friendlyName, accessRolesApplicable);
                    lstElements.Add(tmpElement);
                }
                reader.Close();
            }

            return lstElements;
        }

        /// <summary>
        /// Gets the element by elementID or null if the element is not found
        /// </summary>
        /// <param name="elementID"></param>
        /// <returns></returns>
        public cElement GetElementByID(int elementID)
        {
            cElement tmpElement = null;

            foreach (cElement element in lstCachedElements)
            {
                if (element.ElementID == elementID)
                {
                    tmpElement = element;
                    break;
                }
            }

            return tmpElement;
        }

        /// <summary>
        /// Gets the element by Name or null if the element is not found
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns>cElement or null</returns>
        public cElement GetElementByName(string elementName)
        {
            cElement tmpElement = null;

            foreach (cElement element in lstCachedElements)
            {
                if (element.Name == elementName)
                {
                    tmpElement = element;
                    break;
                }
            }

            return tmpElement;
        }

        /// <summary>
        /// Returns a list of module IDs that the current module is licenced and useable within
        /// </summary>
        /// <param name="accountID">
        /// The current account ID.
        /// </param>
        /// <param name="currentModule">
        /// The current Module.
        /// </param>
        /// <returns>
        /// List of licenced Module IDs
        /// </returns>
        public List<int> GetLicencedModuleIDs(int accountID, Modules currentModule)
        {
            var lstModules = new List<int>();
            
            using (var data = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                data.sqlexecute.Parameters.AddWithValue("@moduleID", currentModule);
                data.sqlexecute.Parameters.AddWithValue("@accountID", accountID);
                using (var reader = data.GetReader("dbo.getElementLicencedModuleIDs", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        lstModules.Add(reader.GetInt32(0));
                    }
                }
                data.sqlexecute.Parameters.Clear();
            }

            return lstModules;
        }

        /// <summary>
        /// Need to filter by the elements the module is licenced for
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateDropDown(int accountID, Modules activeModule)
        {
            SortedList<string, cElement> sorted = sortListByFriendlyName();

            List<int> lstLicencedElementIDs = this.GetLicencedModuleIDs(accountID, activeModule);

            return (from element in sorted.Values where lstLicencedElementIDs.Contains(element.ElementID) select new ListItem(element.FriendlyName, element.ElementID.ToString())).ToList();
        }

        private SortedList<string, cElement> sortListByFriendlyName()
        {
            var sorted = new SortedList<string, cElement>();
            foreach (cElement element in lstCachedElements)
            {
                sorted.Add(element.FriendlyName, element);
            }
            return sorted;
        }
        #endregion

        #region Properties
        /// <summary>
            /// Gets the list of elements that are in cache
            /// </summary>
            public List<cElement> Elements { get { return lstCachedElements; } }
        #endregion Properties
    }
}
