namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Net;

    using Utilities.DistributedCaching;

    /// <summary>
	/// Summary description for IP Filters
	/// </summary>
	public class cIPFilters
	{
        #region Fields

        private readonly int accountId;

        #endregion

        #region Constructors and Destructors

        public cIPFilters(int accountID)
		{
            this.accountId = accountID;
            this.CachedIpFilters = new Cache().Get(this.accountId, CacheArea, "0") as SortedList<int, cIPFilter> ?? this.CacheList();
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a SortedList of IP Filters that are currently in cache 
        /// </summary>
        public SortedList<int, cIPFilter> CachedIpFilters { get; private set; }

        /// <summary>
        /// The name used in the couchbase keys
        /// </summary>
        public const string CacheArea = "ipFilter";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Ensures the given IP Address is valid for use with the current account
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>True if the IP Address is permitted, False otherwise</returns>
        public bool CheckValidIPForAccesss(string ipAddress)
        {
            bool hasActiveIpFilters = false;
            bool hasMatchingAddress = false;
            bool permitAccess = true;

            if (this.CachedIpFilters.Count > 0)
            {
                foreach (cIPFilter ipFilter in this.CachedIpFilters.Values)
                {
                    if (ipFilter.Active)
                    {
                        hasActiveIpFilters = true;
                    }

                    if (ipAddress == ipFilter.IPAddress && ipFilter.Active)
                    {
                        hasMatchingAddress = true;
                        break;
                    }
                }

                if (hasActiveIpFilters && hasMatchingAddress == false)
                {
                    permitAccess = false;
                }
            }

            return permitAccess;
        }

        /// <summary>
        /// Get an IP Filter using its ID
        /// </summary>
        /// <param name="ipfilterid"></param>
        /// <returns>The desired cIPFilter</returns>
        public cIPFilter GetIPFilterById(int ipfilterid)
        {
            cIPFilter ipfilter;            
            this.CachedIpFilters.TryGetValue(ipfilterid, out ipfilter);
            return ipfilter;			
        }

        /// <summary>
        /// Delete the given IP Filter from the database and invalidate cache
        /// </summary>
        /// <param name="IPFilterID">The IP Filter to be deleted</param>
        /// <param name="employeeID">The employee ID of the employee performing the delete</param>
        /// <returns>The number of rows that have been removed after performing the delete</returns>
        public int deleteIPFilter(int IPFilterID, int employeeID, int? delegateID)
        {            
            DBConnection data = new DBConnection(cAccounts.getConnectionString(this.accountId));
            int nReturnValue = 0;
            
            data.sqlexecute.Parameters.AddWithValue("@ipfilterid", IPFilterID);
            data.sqlexecute.Parameters.AddWithValue("@userid", employeeID);
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            if (delegateID.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Int);
            data.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;
            data.ExecuteProc("deleteIPFilter");
            nReturnValue = (int)data.sqlexecute.Parameters["@returnVal"].Value;
            data.sqlexecute.Parameters.Clear();
            this.InvalidateCache();

            return nReturnValue;
        }

        /// <summary>
        /// Save the given IP Filter to the database and invalidate the cache for the current account
        /// </summary>
        /// <param name="ipFilter">The IP Filter to be saved</param>
        /// <param name="employeeID">The employee ID of the employee performing the save</param>
        /// <param name="delegateID">The delegate ID of the employee performing the save</param>
        /// <returns>The ID of the IP Fitler after it has been saved</returns>
        public int saveIPFilter(cIPFilter ipFilter, int employeeID, int? delegateID)
        {
            int nReturnValue;

            IPAddress tempIpAddress;

            if (IPAddress.TryParse(ipFilter.IPAddress, out tempIpAddress))
            {               
                DBConnection data = new DBConnection(cAccounts.getConnectionString(this.accountId));
                data.sqlexecute.Parameters.AddWithValue("@ipfilterid", ipFilter.IPFilterID);                
                data.AddWithValue("@ipaddress", ipFilter.IPAddress, 50);
                data.AddWithValue("@description", ipFilter.Description, 4000);
                data.sqlexecute.Parameters.AddWithValue("@active", ipFilter.Active);
                if (ipFilter.ModifiedBy.HasValue && ipFilter.ModifiedOn.HasValue)
                {
                    data.sqlexecute.Parameters.AddWithValue("@userid", ipFilter.ModifiedBy.Value);
                    data.sqlexecute.Parameters.AddWithValue("@date", ipFilter.ModifiedOn.Value);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@userid", ipFilter.CreatedBy);
                    data.sqlexecute.Parameters.AddWithValue("@date", ipFilter.CreatedOn);
                }
                
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);

                if (delegateID.HasValue)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;

                data.ExecuteProc("saveIPFilter");
                nReturnValue = (int)data.sqlexecute.Parameters["@returnvalue"].Value;

                this.InvalidateCache();
            }
            else
            {
                nReturnValue = -2;
            }

            return nReturnValue;
        }

        #endregion

        #region Methods

        private SortedList<int, cIPFilter> CacheList()
		{            
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountId));
            SortedList<int, cIPFilter> ipFilters = new SortedList<int, cIPFilter>();

            using (SqlDataReader reader = expdata.GetReader("SELECT ipFilterID, ipAddress, description, active, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.ipfilters"))
            {
                int ipFilterIDOrdID = reader.GetOrdinal("ipfilterid");
                int ipFilterAddressOrdID = reader.GetOrdinal("ipAddress");
                int ipFilterDescOrdID = reader.GetOrdinal("description");
                int ipFilterActiveOrdID = reader.GetOrdinal("active");
                int ipFilterCreatedOnOrdID = reader.GetOrdinal("createdOn");
                int ipFilterCreatedByOrdID = reader.GetOrdinal("createdBy");
                int ipFilterModifiedOnOrdID = reader.GetOrdinal("modifiedOn");
                int ipFilterModifiedByOrdID = reader.GetOrdinal("modifiedBy");

                while (reader.Read())
                {
                    int ipFilterId = reader.GetInt32(ipFilterIDOrdID);
                    string ipAddress = reader.GetString(ipFilterAddressOrdID);
                    string description = reader.IsDBNull(ipFilterDescOrdID) ? string.Empty : reader.GetString(ipFilterDescOrdID);
                    bool active = reader.GetBoolean(ipFilterActiveOrdID);
                    DateTime createdOn = reader.IsDBNull(ipFilterCreatedOnOrdID) ? new DateTime(1900, 01, 01) : reader.GetDateTime(ipFilterCreatedOnOrdID);
                    int createdBy = reader.IsDBNull(ipFilterCreatedByOrdID) ? 0 : reader.GetInt32(ipFilterCreatedByOrdID);
                    DateTime? modifiedOn = reader.IsDBNull(ipFilterModifiedOnOrdID) ? null : reader.GetDateTime(ipFilterModifiedOnOrdID) as DateTime?;
                    int? modifiedBy = reader.IsDBNull(ipFilterModifiedByOrdID) ? null : reader.GetInt32(ipFilterModifiedByOrdID) as int?;

                    ipFilters.Add(ipFilterId, new cIPFilter(ipFilterId, ipAddress, description, active, createdOn, createdBy, modifiedOn, modifiedBy));
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            new Cache().Add(this.accountId, CacheArea, "0", ipFilters);

			return ipFilters;
		}

        private void InvalidateCache()
        {
            this.CachedIpFilters = null;
            new Cache().Delete(this.accountId, CacheArea, "0");
        }

        #endregion
	}

}
