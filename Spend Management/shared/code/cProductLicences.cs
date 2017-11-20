using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// cProductLicences class inherits cProductLicencesBase
    /// </summary>
	public class cProductLicences : cProductLicencesBase
	{
        /// <summary>
        /// Reference to cache
        /// </summary>
        private Cache cache = (Cache)System.Web.HttpRuntime.Cache;

        #region properties
        private string cacheKey
        {
            get { return "licences_" + nAccountId.ToString() + "_" + nProductId.ToString(); }
        }
        #endregion

        /// <summary>
        /// cProductLicences constructor
        /// </summary>
        /// <param name="accountid">Database account ID</param>
        /// <param name="subaccountid">Sub-Account ID</param>
        /// <param name="employeeid">Employee ID</param>
        /// <param name="productid">Product ID</param>
        public cProductLicences(int accountid, int subaccountid, int employeeid, int productid, string connectionString)
            : base(accountid, subaccountid, employeeid, productid, connectionString)
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            nAccountId = accountid;
            nSubAccountId = subaccountid;
            nProductId = productid;

            accProperties = subaccs.getSubAccountById(subaccountid).SubAccountProperties;

            initialiseData();
        }

        private void initialiseData()
        {
            if (cache[cacheKey] != null)
            {
                licences = (Dictionary<int, cProductLicence>)cache[cacheKey];
            }
            else
            {
                GetLicences();
            }
        }

		protected string getLicenceSQL()
		{
			StringBuilder sql = new StringBuilder();
			sql.Append("SELECT [LicenceId],[ProductId],ISNULL([LicenceKey],'') AS [Key],ISNULL([LicenceType],'') AS [Type],[Expiry],");
			sql.Append("ISNULL([RenewalType],0) AS [RenewalType],ISNULL([NotifyId],0) AS [NotifyId],[NotifyType],");
			sql.Append("ISNULL([Location],'') AS [Location],ISNULL([NotifyDays],0) AS [NotifyDays],");
			sql.Append("[SoftCopy],[HardCopy],[Unlimited],ISNULL([NumberCopiesHeld],0) AS [NumberCopiesHeld], ");
            sql.Append("dateInstalled, installedVersion, availableVersion, userCode, licenceType, ");
            sql.Append("createdon, createdby, modifiedon, modifiedby ");
			sql.Append("FROM dbo.productLicences WHERE [ProductId] = @prodId ");
			//sql.Append("ORDER BY [Expiry] DESC");

			return sql.ToString();
		}

        private string getGridSQL
        {
            get { return "select [LicenceId],[LicenceType],[Expiry],[NumberCopiesHeld] from productLicences"; }
        }

		private void GetLicences()
		{
			licences = new Dictionary<int, cProductLicence>();
			System.Data.SqlClient.SqlDataReader reader;
			cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(AccountID, SubAccountID, SpendManagementElement.LicenceRenewalTypes);
			DBConnection db = new DBConnection(connectionString);
		    AggregateCacheDependency aggdep = null;
		    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
		    {
                aggdep = new AggregateCacheDependency();
		    }

		    // set up cache dependencies
            const string sql = "SELECT [renewalType],description, archived, createdon, createdby, modifiedon, modifiedby FROM dbo.licenceRenewalTypes WHERE [subAccountId] = @subAccId";
            SortedList<string, object> depParams = new SortedList<string, object>();
            depParams.Add("@subAccId", nSubAccountId);

            if (aggdep != null)
		    {
		        var lrDep = db.CreateSQLCacheDependency(sql + " AND " + AccountID.ToString() + " = " + AccountID.ToString(), depParams);
		        aggdep.Add(lrDep);
		    }
		    db.sqlexecute.CommandText = getLicenceSQL();
            db.sqlexecute.Parameters.AddWithValue("@prodId", nProductId);
		    if (aggdep != null)
		    {
		        var dep = new SqlCacheDependency(db.sqlexecute);
		        aggdep.Add(dep);
		    }

		    using (reader = db.GetReader(getLicenceSQL()))
            {
                while (reader.Read())
                {
                    int licId = reader.GetInt32(reader.GetOrdinal("LicenceId"));
                    int prodId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                    string key = reader.GetString(reader.GetOrdinal("Key"));
                    //string lictype = reader.GetString(reader.GetOrdinal("Type"));
                    DateTime expires = DateTime.MinValue;
                    if (!reader.IsDBNull(reader.GetOrdinal("Expiry")))
                    {
                        expires = reader.GetDateTime(reader.GetOrdinal("Expiry"));
                    }
                    int rentype = reader.GetInt32(reader.GetOrdinal("RenewalType"));
                    cLicenceRenewalType renewaltype = null;
                    if (rentype > 0)
                    {
                        renewaltype = (cLicenceRenewalType)clsBaseDefinitions.GetDefinitionByID(rentype);
                    }
                    AudienceType notifytype = (AudienceType)reader.GetInt32(reader.GetOrdinal("NotifyType"));
                    string loc = reader.GetString(reader.GetOrdinal("Location"));
                    int notifydays = reader.GetInt32(reader.GetOrdinal("NotifyDays"));
                    int notifyid = reader.GetInt32(reader.GetOrdinal("NotifyId"));
                    bool ec = false;
                    if (!reader.IsDBNull(reader.GetOrdinal("SoftCopy")))
                    {
                        ec = reader.GetBoolean(reader.GetOrdinal("SoftCopy"));
                    }
                    bool hc = false;
                    if (!reader.IsDBNull(reader.GetOrdinal("HardCopy")))
                    {
                        hc = reader.GetBoolean(reader.GetOrdinal("HardCopy"));
                    }
                    bool unlimited = false;
                    if (!reader.IsDBNull(reader.GetOrdinal("Unlimited")))
                    {
                        unlimited = reader.GetBoolean(reader.GetOrdinal("Unlimited"));
                    }
                    int numheld = reader.GetInt32(reader.GetOrdinal("NumberCopiesHeld"));
                    string availableVersion = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("availableVersion")))
                    {
                        availableVersion = reader.GetString(reader.GetOrdinal("availableVersion"));
                    }
                    string installedVersion = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("installedVersion")))
                    {
                        installedVersion = reader.GetString(reader.GetOrdinal("installedVersion"));
                    }
                    DateTime? dateInstalled = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("dateInstalled")))
                    {
                        dateInstalled = reader.GetDateTime(reader.GetOrdinal("dateInstalled"));
                    }
                    string userCode = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("userCode")))
                    {
                        userCode = reader.GetString(reader.GetOrdinal("userCode"));
                    }
                    int licType = 0;
                    if (!reader.IsDBNull(reader.GetOrdinal("licenceType")))
                    {
                        licType = reader.GetInt32(reader.GetOrdinal("licenceType"));
                    }
                    DateTime createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    int createdby = 0;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    DateTime? modifiedon = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    int? modifiedby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    licences.Add(licId, new cProductLicence(licId, prodId, key, licType, expires, renewaltype, notifyid, notifytype, notifydays, loc, hc, ec, unlimited, numheld, availableVersion, installedVersion, userCode, dateInstalled, createdon, createdby, modifiedon, modifiedby));
                }
                reader.Close();
            }

		    if (aggdep != null)
		    {
		        cache.Insert(cacheKey, licences, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
		    }

		    db.sqlexecute.Parameters.Clear();

			return;
		}

        public string[] getLicencesGrid(int productid)
        {
            cGridNew licgrid = new cGridNew(AccountID, EmployeeID, "licencegrid", getGridSQL);
            CurrentUser curUser = cMisc.GetCurrentUser();

            licgrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ProductLicences, false, false);
            licgrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ProductLicences, false, false);
            licgrid.editlink = "javascript:editLicence({productid});";
            licgrid.deletelink = "javascript:deleteLicence({productid});";
            licgrid.EmptyText = "There are not currently any licences for the product.";
            licgrid.enabledeleting = true;
            licgrid.enableupdating = true;
            licgrid.getColumnByName("licenceid").hidden = true;
            licgrid.getColumnByName("productid").hidden = true;
            licgrid.KeyField = "licence id";
            List<string> retVals = new List<string>();
            retVals.Add(licgrid.GridID);
            retVals.AddRange(licgrid.generateGrid());
            return retVals.ToArray();
        }

        public int saveLicence(cProductLicence lic)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            
            int retLicenceId = 0;

            db.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@licenceId", lic.LicenceId);
            db.sqlexecute.Parameters.AddWithValue("@productId", lic.ProductId);
            db.sqlexecute.Parameters.AddWithValue("@elecCopyHeld", lic.IsElectronicCopyHeld);
            db.sqlexecute.Parameters.AddWithValue("@hardCopyHeld", lic.IsHardCopyHeld);
            db.sqlexecute.Parameters.AddWithValue("@unlimited", lic.IsUnlimitedLicence);
            db.sqlexecute.Parameters.AddWithValue("@licExpiry", lic.LicenceExpiry);
            db.sqlexecute.Parameters.AddWithValue("@licKey", lic.LicenceKey);
            db.sqlexecute.Parameters.AddWithValue("@licLocation", lic.LicenceLocation);
            db.sqlexecute.Parameters.AddWithValue("@licRenewalType", lic.LicenceRenewalType);
            db.sqlexecute.Parameters.AddWithValue("@numCopiesHeld", lic.NumberCopiesHeld);
            db.sqlexecute.Parameters.AddWithValue("@userCode", lic.UserCode);
            db.sqlexecute.Parameters.AddWithValue("@availableVersion", lic.AvailableVersion);
            db.sqlexecute.Parameters.AddWithValue("@installedVersion", lic.InstalledVersion);
            db.sqlexecute.Parameters.AddWithValue("@licType", lic.LicenceType);
            
            if (lic.DateInstalled == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@dateInstalled", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@dateInstalled", lic.DateInstalled);
            }
            db.sqlexecute.Parameters.AddWithValue("@employeeId", EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@updateCount", accProperties.AutoUpdateLicenceTotal);

            db.ExecuteProc("saveProductLicence");

            retLicenceId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

            return retLicenceId;
        }

        /// <summary>
        /// Reset the cache
        /// </summary>
        private void resetCache()
        {
            cache.Remove(cacheKey);
            licences = null;
            initialiseData();
        }

        /// <summary>
        /// Override to reset the cache
        /// </summary>
        /// <param name="LicenceID"></param>
        /// <param name="userID"></param>
        public override void DeleteLicence(int LicenceID, int userID)
        {
            base.DeleteLicence(LicenceID, userID);

            resetCache();
        }

        public override int UpdateLicence(cProductLicence newlicence)
        {
            int ID = base.UpdateLicence(newlicence);

            resetCache();

            return ID;
        }
	}
}
