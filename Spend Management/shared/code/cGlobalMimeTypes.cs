using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Data;

namespace Spend_Management
{
	/// <summary>
	/// cMimeTypes class definition
	/// </summary>
    public class cGlobalMimeTypes
    {
        private int nAccountID;
        private SortedList<Guid, cGlobalMimeType> list;
        private SortedList<Guid, cGlobalMimeType> customMimeTypeList;
        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        
		/// <summary>
		/// cMimeTypes constructor
		/// </summary>
        public cGlobalMimeTypes(int AccountID)
        {
            nAccountID = AccountID;
            initialiseData();
        }

        #region Properties

        /// <summary>
        /// Account ID of the company
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        /// <summary>
        /// The unique CacheKey for the custom mime types
        /// </summary>
        private string CacheKey
        {
            get
            {
                return "MimeTypes";
            }
        }

        /// <summary>
        /// The unique CacheKey for the custom mime types
        /// </summary>
        private string CustomCacheKey
        {
            get
            {
                return "customMimeTypes" + AccountID;
            }
        }

        #endregion

        /// <summary>
		/// Cache data if required or get from cache
		/// </summary>
        private void initialiseData()
        {
            list = (SortedList<Guid, cGlobalMimeType>)Cache[CacheKey];
            if (list == null || list.Count == 0)
            {
                list = CacheList();
            }

            customMimeTypeList = (SortedList<Guid, cGlobalMimeType>)Cache[CustomCacheKey];
            if (customMimeTypeList == null || customMimeTypeList.Count == 0)
            {
                customMimeTypeList = CacheCustomMimeTypeList();
            }
        }

		/// <summary>
		/// Caches the known MIME types from the metabase database
		/// </summary>
		/// <returns></returns>
        private SortedList<Guid, cGlobalMimeType> CacheList()
        {
            DBConnection metaData = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);

            SortedList<Guid, cGlobalMimeType> types = new SortedList<Guid, cGlobalMimeType>();
            Guid mimeid;
            string fileExtension, mimeHeader, description;

            string strsql = "SELECT mimeID, fileExtension, mimeHeader, description FROM dbo.mime_headers";
            metaData.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(metaData.sqlexecute);
                Cache.Insert(CacheKey, types, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), System.Web.Caching.CacheItemPriority.Default, null);
            }

            using (SqlDataReader reader = metaData.GetStoredProcReader("GetMimeTypes"))
            {
                while (reader.Read())
                {
                    mimeid = reader.GetGuid(0);
                    fileExtension = reader.GetString(1);
                    mimeHeader = reader.GetString(2);

                    if (reader.IsDBNull(3))
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(3);
                    }
                    types.Add(mimeid, new cGlobalMimeType(mimeid, fileExtension, mimeHeader, description));
                }
                reader.Close();
            }

            return types;
        }

		/// <summary>
		/// getMimeTypeByExtension: gets a Content MIME type definition by its file extension type
		/// </summary>
		/// <param name="extension"></param>
		/// <returns></returns>
        public cGlobalMimeType getMimeTypeByExtension(string extension)
        {
            foreach (cGlobalMimeType mimeType in list.Values)
            {
                if (mimeType.FileExtension.Trim().ToLower() == extension.Trim().ToLower())
                {
                    return mimeType;
                }
            }

            //Custom Mime Types
            foreach (cGlobalMimeType custMimeType in customMimeTypeList.Values)
            {
                if (custMimeType.FileExtension.Trim().ToLower() == extension.Trim().ToLower())
                {
                    return custMimeType;
                }
            }

            return null;
        }

		/// <summary>
		/// getMimeTypeById: gets a Content MIME type definition by its ID
		/// </summary>
		/// <param name="TypeID"></param>
		/// <returns></returns>
		public cGlobalMimeType getMimeTypeById(Guid TypeID)
		{
			cGlobalMimeType retVal = null;

            list.TryGetValue(TypeID, out retVal);

            if (retVal == null)
            {
                customMimeTypeList.TryGetValue(TypeID, out retVal);
            }

			return retVal;
		}

        /// <summary>
        /// isPermittedFileType: Checks to see if file type is in the permitted extension list
        /// </summary>
        /// <param name="extension">File extension to check (e.g. PDF, TXT)</param>
        /// <returns>TRUE if file is permitted, otherwise FALSE</returns>
        public bool isPermittedFileType(string extension)
        {
            bool valid = false;
            if (getMimeTypeByExtension(extension) != null)
            {
                valid = true;
            }
            return valid;
        }

        /// <summary>
        /// Get the list of all the global and custom mime types
        /// </summary>
        /// <returns></returns>
        public SortedList<Guid, cGlobalMimeType> GetGlobalMimeTypes()
        {
            SortedList<Guid, cGlobalMimeType> globalList = new SortedList<Guid, cGlobalMimeType>();;

            foreach (cGlobalMimeType gType in list.Values)
            {
                globalList.Add(gType.GlobalMimeID, gType);
            }

            foreach (cGlobalMimeType type in customMimeTypeList.Values)
            {
                globalList.Add(type.GlobalMimeID, type);
            }

            return globalList;
        }

        #region Custom Mime Header Methods

        /// <summary>
        /// Store a seperate collection for the custom types in cache
        /// </summary>
        /// <returns></returns>
        private SortedList<Guid, cGlobalMimeType> CacheCustomMimeTypeList()
        {
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(AccountID));

            SortedList<Guid, cGlobalMimeType> types = new SortedList<Guid, cGlobalMimeType>();
            Guid mimeid;
            string fileExtension, mimeHeader, description;
            string strsql = "SELECT customMimeID, fileExtension, mimeHeader, description FROM dbo.customMimeHeaders";

            smData.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(smData.sqlexecute);
                Cache.Insert(CustomCacheKey, types, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Permanent), System.Web.Caching.CacheItemPriority.Default, null);
            }

            using (SqlDataReader reader = smData.GetStoredProcReader("GetCustomMimeHeaders"))
            {
                while (reader.Read())
                {
                    mimeid = reader.GetGuid(0);
                    fileExtension = reader.GetString(1);
                    mimeHeader = reader.GetString(2);

                    if (reader.IsDBNull(3))
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(3);
                    }
                    types.Add(mimeid, new cGlobalMimeType(mimeid, fileExtension, mimeHeader, description));
                }
                reader.Close();
            }

            return types;
        }

        /// <summary>
        /// Force an update of the cache for this base definition
        /// </summary>
        private void resetCache()
        {
            Cache.Remove(CustomCacheKey);
            customMimeTypeList = null;
            initialiseData();
        }

        /// <summary>
        /// Save the custom mime header to the database
        /// </summary>
        /// <param name="gType"></param>
        /// <returns></returns>
        public int SaveCustomMimeHeader(cGlobalMimeType gType)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cFields fields = new cFields(AccountID);
            Guid id;

            if (gType.GlobalMimeID == Guid.Empty)
            {
                id = Guid.NewGuid();
            }
            else
            {
                id = gType.GlobalMimeID;
            }

            db.sqlexecute.Parameters.AddWithValue("@mimeId", id);
            db.AddWithValue("@mimeHeader", gType.MimeHeader, fields.GetFieldSize("customMimeHeaders", "mimeHeader"));
            db.AddWithValue("@fileExtension", gType.FileExtension, fields.GetFieldSize("customMimeHeaders", "fileExtension"));
            db.AddWithValue("@description", gType.Description, fields.GetFieldSize("customMimeHeaders", "description"));

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@employeeId", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@employeeId", 0);
                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            }

            db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("SaveCustomMimeHeader");

            int retVal = (int)db.sqlexecute.Parameters["@identity"].Value;
            //int id = (int)db.sqlexecute.Parameters["@identity"].Value;
            db.sqlexecute.Parameters.Clear();

            resetCache();

            return retVal;
        }

        /// <summary>
        /// Delete the custom mime header from the database, checking if the mime header is not set as any of the mime types for the account
        /// </summary>
        /// <param name="MimeID"></param>
        /// <returns></returns>
        public int DeleteCustomMimeHeader(Guid MimeID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@mimeID", MimeID);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            }

            db.sqlexecute.Parameters.Add("@retVal", SqlDbType.Int);
            db.sqlexecute.Parameters["@retVal"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("DeleteCustomMimeHeader");
            int retVal = (int)db.sqlexecute.Parameters["@retVal"].Value;
            db.sqlexecute.Parameters.Clear();

            resetCache();

            return retVal;
        }

        /// <summary>
        /// Generate the html for the grid
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public string[] GenerateCustomAttachmentGrid(CurrentUser user)
        {
            string gridSQL = "SELECT customMimeID, fileExtension, description, mimeHeader FROM customMimeHeaders";

            cGridNew newgrid = new cGridNew(AccountID, user.EmployeeID, "gridCustomAttachmentTypes", gridSQL);

            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.CustomMimeHeaders, true);
            newgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CustomMimeHeaders, true);
            newgrid.SortedColumn = newgrid.getColumnByName("fileExtension");
            newgrid.SortDirection = SpendManagementLibrary.SortDirection.Ascending;
            newgrid.EmptyText = "There are currently no Custom Attachment Types set up";
            newgrid.deletelink = "javascript:SEL.AttachmentTypes.DeleteCustomAttachmentType('{customMimeID}');";
            newgrid.editlink = "javascript:SEL.AttachmentTypes.GetCustomAttachmentTypeData('{customMimeID}');";
            newgrid.getColumnByName("customMimeID").hidden = true;
            newgrid.KeyField = "customMimeID";
            List<string> retVals = new List<string>();
            retVals.Add(newgrid.GridID);
            retVals.AddRange(newgrid.generateGrid());
            return retVals.ToArray();
        }
        #endregion
    }
}
