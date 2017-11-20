namespace Spend_Management
{
using System;
using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;

using Utilities.DistributedCaching;

    /// <summary>
	/// Summary description for cReasons.
	/// Caches
	/// [accountid_cReasons_0] = SortedList collection of reasons
	/// </summary>
	public class cReasons
	{
        #region Fields

        readonly Cache cache = new Cache();
			
        readonly int accountId;

        SortedList<int, cReason> reasons;

        /// <summary>
        /// The cache key area name
        /// </summary>
        public const string CacheArea = "cReasons";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// The constructor for cReasons, will attempt to cache the list of reasons
        /// </summary>
        /// <param name="accountid">The account id</param>
        public cReasons(int accountid)
        {
			this.accountId = accountid;

			this.InitialiseData();
        }

        #endregion

        #region Public Properties

        public int accountid
        {
            get { return this.accountId; }
                }

        public int count
        {
            get { return this.reasons.Count; }
        }
        
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieve the reasons for this account from the database and store them in the cache
        /// </summary>
        /// <returns>A sorted list of <see cref="cReason">cReason</see> indexed by their id</returns>
        public SortedList<int,cReason> CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            SortedList<int, cReason> list = new SortedList<int, cReason>();

            using (SqlDataReader reader = expdata.GetReader("SELECT reasonid, reason, description, accountcodevat, accountcodenovat, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived FROM dbo.reasons"))
            {
                while (reader.Read())
                {
                    int reasonid = reader.GetInt32(reader.GetOrdinal("reasonid"));
                    string reason = reader.GetString(reader.GetOrdinal("reason"));
                    string description = reader.IsDBNull(reader.GetOrdinal("description")) == false ? reader.GetString(reader.GetOrdinal("description")) : string.Empty;
                    string accountcodevat = reader.IsDBNull(reader.GetOrdinal("accountcodevat")) == false ? reader.GetString(reader.GetOrdinal("accountcodevat")) : string.Empty;
                    string accountcodenovat = reader.IsDBNull(reader.GetOrdinal("accountcodenovat")) == false ? reader.GetString(reader.GetOrdinal("accountcodenovat")) : string.Empty;
                    bool isArchived = reader.GetBoolean(reader.GetOrdinal("Archived"));
                    DateTime createdon = reader.IsDBNull(reader.GetOrdinal("createdon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                    int createdby = reader.IsDBNull(reader.GetOrdinal("createdby")) ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                    DateTime? modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    int? modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("modifiedby"));

                    list.Add(reasonid, new cReason(this.accountid, reasonid, reason, description, accountcodevat, accountcodenovat, createdon, createdby, modifiedon, modifiedby, isArchived));
                    }

                reader.Close();
                    }

            expdata.sqlexecute.Parameters.Clear();
            this.cache.Add(this.accountid, CacheArea, "0", list);

			return list;
                    }

        /// <summary>
        /// Get the cached reasons as a non-sorted list of cReason
        /// </summary>
        /// <returns></returns>
        public List<cReason> CachedList()
                    {
            List<cReason> lstReasons = new List<cReason>();
            foreach (cReason reqReason in this.reasons.Values.Where(reqReason => lstReasons.Contains(reqReason) == false))
                    {
                lstReasons.Add(reqReason);
                    }

            return lstReasons;
                    }

        /// <summary>
        /// Get the cached reasons as a list of <see cref="ListItem">ListItem</see>
        /// </summary>
        /// <returns>List of <see cref="ListItem">ListItem</see> with reason name as the text and reasonId as the value</returns>
        public List<ListItem> CreateDropDown()
                    {
            List<ListItem> items = new List<ListItem> { new ListItem(string.Empty, "0") };
            items.AddRange(this.reasons.Values.OrderBy(x => x.reason).Select(reason => new ListItem(reason.reason, reason.reasonid.ToString(CultureInfo.InvariantCulture))));

            return items;
                    }

        public void InitialiseData()
                    {
            this.reasons = this.cache.Get(this.accountId, CacheArea, "0") as SortedList<int, cReason> ?? this.CacheList();
                    }

        public int deleteReason(int reasonid)
                    {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate)
                    {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
            expdata.sqlexecute.Parameters.AddWithValue("@reasonid", reasonid);
            expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteReason");

            this.ClearCache();

            return (int)expdata.sqlexecute.Parameters["@retCode"].Value; 
            
            //cAuditLog clsaudit = new cAuditLog();
            //cReason clsreason = getReasonById(reasonid);
            //clsaudit.deleteRecord("Reason Categories", clsreason.reason);
                    }

        public string[] getArray()
                    {
			return this.reasons.Values.Select(x => x.reason).ToArray();
                    }

        public string getGrid()
        {
            return "select reasonid, reason, description, archived from reasons";
		}

        public ArrayList getModifiedReasons(DateTime date)
        {
            ArrayList lst = new ArrayList();
            foreach (cReason val in this.reasons.Values.Where(val => val.createdon > date || val.modifiedon > date))
            {
                lst.Add(val);
            }            
            
            return lst;
        }

        public cReason getReasonById(int reasonid)
        {
            cReason reason;
            this.reasons.TryGetValue(reasonid, out reason);
            return reason;
        }

        public cReason getReasonByString(string reason)
            {
            return this.reasons.Values.FirstOrDefault(clsreason => clsreason.reason == reason);
        }
                
        public ArrayList getReasonIds()
		{
            ArrayList ids = new ArrayList();
            foreach (cReason val in this.reasons.Values)
			{
                ids.Add(val.reasonid);
		}

            return ids;
		}

        public int saveReason(cReason reason)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@reasonid", reason.reasonid);
            expdata.sqlexecute.Parameters.AddWithValue("@reason", reason.reason);
            expdata.sqlexecute.Parameters.AddWithValue("@description", reason.description);
            expdata.sqlexecute.Parameters.AddWithValue("@accountcodevat", reason.accountcodevat);
            expdata.sqlexecute.Parameters.AddWithValue("@accountcodenovat", reason.accountcodenovat);
            if (reason.modifiedby == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", reason.createdon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", reason.createdby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", reason.modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", reason.modifiedby);
            }

            if (currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveReason");
            int reasonid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();
			
            this.ClearCache();

            return reasonid;
		}

        /// <summary>
        /// Updates the archive status of the reason.
        /// </summary>
        /// <param name="reasonId">
        /// Reason ID of the reason.
        /// </param>
        /// <param name="archive">
        /// Archive status of the reason.
        /// </param>
        public int ChangeStatus(int reasonId, bool archive)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", reasonId);
                expdata.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                CurrentUser currentUser = cMisc.GetCurrentUser();
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }

                expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("changeReasonStatus");
                int returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;
                expdata.sqlexecute.Parameters.Clear();
                this.ClearCache();
                return returncode;
            }
        }

        /// <summary>
        /// Get the cached un-archived reasons as a list of <see cref="ListItem">ListItem</see>
        /// </summary>
        /// <returns>List of <see cref="ListItem">ListItem</see> with reason name as the text and reasonId as the value</returns>
        public List<ListItem> CreateActiveReasonsDropDown()
        {
            List<ListItem> items = new List<ListItem> { new ListItem(string.Empty, "0") };
            items.AddRange(this.reasons.Values.OrderBy(x => x.reason).Where(e => e.Archive != true).Select(reason => new ListItem(reason.reason, reason.reasonid.ToString(CultureInfo.InvariantCulture))));
            return items;
        }

        #endregion

        #region Methods

        private void ClearCache()
        {
            this.cache.Delete(this.accountId, CacheArea, "0");
            this.reasons = null;
        }

        #endregion
	}
}
