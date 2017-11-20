using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    /// <summary>
    /// Class for the local mime types
    /// </summary>
    public class cMimeTypes
    {
        private SortedList<int, cMimeType> list;
        private int nAccountID;
        private int nSubAccountID;
        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        
		/// <summary>
		/// cMimeTypes constructor
		/// </summary>
        public cMimeTypes(int AccountID, int SubAccountID)
        {
            nAccountID = AccountID;
            nSubAccountID = SubAccountID;
            initialiseData();
        }

        #region Properties

        /// <summary>
        /// ID of the logged in account
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        /// <summary>
        /// ID of the sub account 
        /// </summary>
        private int SubAccountID
        {
            get { return nSubAccountID; }
        }

        /// <summary>
        /// The unique CacheKey
        /// </summary>
        private string CacheKey
        {
            get
            {
                return "MimeTypes" + AccountID;
            }
        }

        #endregion

        /// <summary>
		/// Cache data if required or get from cache
		/// </summary>
        private void initialiseData()
        {
            list = (SortedList<int, cMimeType>)Cache[CacheKey];
            if (list == null || list.Count == 0)
            {
                list = CacheList();
            }
        }

        /// <summary>
        /// Caches the locally selected MIME types from the database
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cMimeType> CacheList()
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));
            SortedList<int, cMimeType> types = new SortedList<int, cMimeType>();
            int MimeID;
            Guid GlobalMimeID;
            bool archived;
            DateTime? CreatedOn;
            int? CreatedBy;
            DateTime? ModifiedOn;
            int? ModifiedBy;

            string strsql = "select mimeID, globalMimeID, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.mimeTypes where SubAccountID = @SubAccountID";
            data.sqlexecute.Parameters.AddWithValue("@SubAccountID", SubAccountID);
            data.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(data.sqlexecute);
                Cache.Insert(CacheKey, types, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), System.Web.Caching.CacheItemPriority.Default, null);
            }

            using (SqlDataReader reader = data.GetStoredProcReader("GetMimeTypes"))
            {
                while (reader.Read())
                {
                    MimeID = reader.GetInt32(0);
                    GlobalMimeID = reader.GetGuid(1);
                    archived = reader.GetBoolean(2);

                    if (reader.IsDBNull(3))
                    {
                        CreatedOn = null;
                    }
                    else
                    {
                        CreatedOn = reader.GetDateTime(3);
                    }

                    if (reader.IsDBNull(4))
                    {
                        CreatedBy = null;
                    }
                    else
                    {
                        CreatedBy = reader.GetInt32(4);
                    }

                    if (reader.IsDBNull(5))
                    {
                        ModifiedOn = null;
                    }
                    else
                    {
                        ModifiedOn = reader.GetDateTime(5);
                    }

                    if (reader.IsDBNull(6))
                    {
                        ModifiedBy = null;
                    }
                    else
                    {
                        ModifiedBy = reader.GetInt32(6);
                    }
                    types.Add(MimeID, new cMimeType(MimeID, GlobalMimeID, archived, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy));
                }
                reader.Close();
            }

            data.sqlexecute.Parameters.Clear();

            return types;
        }

        /// <summary>
        /// Force an update of the cache
        /// </summary>
        private void resetCache()
        {
            Cache.Remove(CacheKey);
            list = null;
            initialiseData();
        }

        /// <summary>
        /// Get the mime type object by its ID
        /// </summary>
        /// <param name="MimeID"></param>
        /// <returns></returns>
        public cMimeType GetMimeTypeByID(int MimeID)
        {
            cMimeType mimeType = null;
            list.TryGetValue(MimeID, out mimeType);
            return mimeType;
        }

        /// <summary>
        /// Get the mime type object by its Global ID
        /// </summary>
        /// <param name="GlobalMimeID"></param>
        /// <returns></returns>
        public cMimeType GetMimeTypeByGlobalID(Guid GlobalMimeID)
        {
            foreach (cMimeType type in list.Values)
            {
                if (type.GlobalMimeID == GlobalMimeID)
                {
                    return type;
                }
            }

            return null;
        }


        /// <summary>
        /// Create the list items for the mimetypes for the admin page for saving mime types, filtering out the global mime types not already used
        /// </summary>
        /// <returns></returns>
        public ListItem[] CreateMimeTypeDropdown()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(AccountID);
            SortedList<Guid, cGlobalMimeType> lstGlobalTypes = clsGlobalMimeTypes.GetGlobalMimeTypes();
            cMimeType mimeType = null;
            List<ListItem> lstMimeTypes = new List<ListItem>();

            foreach (cGlobalMimeType gMime in lstGlobalTypes.Values)
            {
                mimeType = GetMimeTypeByGlobalID(gMime.GlobalMimeID);

                //Not already used
                if (mimeType == null)
                {
                    if (gMime.Description == string.Empty)
                    {
                        lstMimeTypes.Add(new ListItem(gMime.FileExtension, gMime.GlobalMimeID.ToString()));
                    }
                    else
                    {
                        lstMimeTypes.Add(new ListItem(gMime.FileExtension + " - " + gMime.Description, gMime.GlobalMimeID.ToString()));
                    }
                }
            }

            lstMimeTypes.Sort(delegate(ListItem lt1, ListItem lt2)
            {
                return lt1.Text.CompareTo(lt2.Text);
            });

            return lstMimeTypes.ToArray();
        }

        /// <summary>
        /// Save the mime Type to the database
        /// </summary>
        /// <param name="globalMimeType"></param>
        /// <returns></returns>
        public int SaveMimeType(Guid globalMimeID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            db.sqlexecute.Parameters.AddWithValue("@GlobalMimeID",globalMimeID);
            db.sqlexecute.Parameters.AddWithValue("@SubAccountID", SubAccountID);

            cGlobalMimeTypes clsMimeTypes = new cGlobalMimeTypes(AccountID);

            cGlobalMimeType gMime = clsMimeTypes.getMimeTypeById(globalMimeID);

            db.sqlexecute.Parameters.AddWithValue("@FileExtension", gMime.FileExtension);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", 0);
                db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

            }

            db.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            db.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("SaveMimeType");
            int id = (int)db.sqlexecute.Parameters["@id"].Value;
            db.sqlexecute.Parameters.Clear();

            resetCache();

            return id;
        }

        /// <summary>
        /// Delete the mime type from the database, checking if the mimetype is not associated to an attachment
        /// </summary>
        /// <param name="MimeID"></param>
        /// <returns></returns>
        public MimeTypeReturnValue DeleteMimeType(int MimeID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@MimeID", MimeID);
            db.sqlexecute.Parameters.AddWithValue("@SubAccountID", SubAccountID);

            //Get the global mimetype to use the extension value for the audit log in the stored procedure
            cMimeType mimeType = GetMimeTypeByID(MimeID);
            if (mimeType == null)
            {
                return MimeTypeReturnValue.Success;
            }
            
            cGlobalMimeTypes clsMimeTypes = new cGlobalMimeTypes(AccountID);

            cGlobalMimeType gMime = clsMimeTypes.getMimeTypeById(mimeType.GlobalMimeID);

            db.sqlexecute.Parameters.AddWithValue("@FileExtension", gMime.FileExtension);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", 0);
                db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

            }

            db.sqlexecute.Parameters.Add("@retVal", SqlDbType.Int);
            db.sqlexecute.Parameters["@retVal"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("DeleteMimeType");
            MimeTypeReturnValue retVal = (MimeTypeReturnValue)db.sqlexecute.Parameters["@retVal"].Value;
            db.sqlexecute.Parameters.Clear();

            resetCache();

            return retVal;
        }

        /// <summary>
        /// Archive the mime type in the database
        /// </summary>
        /// <param name="MimeID"></param>
        public void ArchiveMimeType(int MimeID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@MimeID", MimeID);
            db.sqlexecute.Parameters.AddWithValue("@SubAccountID", SubAccountID);

            //Get the global mimetype to use the extension value for the audit log in the stored procedure
            cMimeType mimeType = GetMimeTypeByID(MimeID);
            cGlobalMimeTypes clsMimeTypes = new cGlobalMimeTypes(AccountID);

            cGlobalMimeType gMime = clsMimeTypes.getMimeTypeById(mimeType.GlobalMimeID);

            db.sqlexecute.Parameters.AddWithValue("@FileExtension", gMime.FileExtension);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", 0);
                db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

            }

            db.ExecuteProc("ArchiveMimeType");
            db.sqlexecute.Parameters.Clear();

            resetCache();
        }

    }
}