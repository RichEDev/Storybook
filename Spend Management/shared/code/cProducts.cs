using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
	public class cProducts : cProductsBase
	{
		Cache cache = (Cache)System.Web.HttpRuntime.Cache;
        private int? nSubAccountId;

		public cProducts(int accountid, int? subaccountid) : base(accountid, subaccountid)
		{
			connectionstring = cAccounts.getConnectionString(accountid);
            nAccountId=accountid;
            nSubAccountId=subaccountid;

            if (cache[cacheKey] == null)
			{
				slProducts = CacheItems();
			}
			else
			{
				slProducts = (SortedList<int, cProduct>)cache[cacheKey];
			}
        }

        #region properties
        private string cacheKey
        {
            get
            {
                string key = "products_" + nAccountId.ToString();
                if (nSubAccountId.HasValue)
                {
                    key += "_" + nSubAccountId.ToString();
                }
                return key;
            }
        }
        #endregion

        public cProducts(int accountid)
            : base(accountid)
		{
			nSubAccountId = null;
			connectionstring = cAccounts.getConnectionString(accountid);

            if (cache[cacheKey] == null)
			{
				slProducts = CacheItems();
			}
			else
			{
                slProducts = (SortedList<int, cProduct>)cache[cacheKey];
			}
		}

		private SortedList<int, cProduct> CacheItems()
		{
			cEmployees emps = new cEmployees(AccountID);
            cBaseDefinitions clsBaseDefs;

            if (!SubAccountID.HasValue)
            {
                CurrentUser curUser = cMisc.GetCurrentUser();
                clsBaseDefs = new cBaseDefinitions(AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories);
            }
            else
            {
                clsBaseDefs = new cBaseDefinitions(AccountID, SubAccountID.Value, SpendManagementElement.ProductCategories);
            }

			SortedList<int, cProduct> products = new SortedList<int, cProduct>();
			System.Data.SqlClient.SqlDataReader reader;

			DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

			StringBuilder sql = new StringBuilder();
			sql.Append("SELECT [ProductId],ISNULL([ProductCode],'') AS [ProductCode], [ProductName], ");
			sql.Append("ISNULL([Description],'') AS [Description], ISNULL([ProductCategoryId],0) AS [ProductCategoryId],");
			sql.Append("ISNULL([InstalledVersionNumber],'') AS [InstalledVersionNumber], ISNULL([AvailableVersionNumber],'') AS [AvailableVersionNumber],");
			sql.Append("ISNULL([DateInstalled],CONVERT(datetime,'1900-01-01',120)) AS [DateInstalled], ISNULL([NumLicencedCopies],0) AS [NumLicencedCopies],");
			sql.Append("ISNULL([UserCode],'') AS [UserCode],");
			sql.Append("archived, createdon, createdby, modifiedon, modifiedby, subAccountId ");
			sql.Append("FROM productDetails");

			if (SubAccountID.HasValue)
			{
				sql.Append(" WHERE [subAccountId] = @subaccId");
                db.sqlexecute.Parameters.AddWithValue("@subaccId", SubAccountID.Value);
			}

			db.sqlexecute.CommandText = sql.ToString();

		    SqlCacheDependency dep = null;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                 dep = new SqlCacheDependency(db.sqlexecute);
            }

            using (reader = db.GetReader(sql.ToString()))
            {
                while (reader.Read())
                {
                    int prodid = reader.GetInt32(reader.GetOrdinal("ProductId"));
                    int? subAccountId = null;
                    if (reader.IsDBNull(reader.GetOrdinal("subAccountId")))
                    {
                        subAccountId = reader.GetInt32(reader.GetOrdinal("subAccountId"));
                    }
                    string prodcode = reader.GetString(reader.GetOrdinal("ProductCode"));
                    string prodname = reader.GetString(reader.GetOrdinal("ProductName"));
                    string proddesc = reader.GetString(reader.GetOrdinal("Description"));
                    int prodcatid = reader.GetInt32(reader.GetOrdinal("ProductCategoryId"));
                    string installedvers = reader.GetString(reader.GetOrdinal("InstalledVersionNumber"));
                    DateTime installed = reader.GetDateTime(reader.GetOrdinal("DateInstalled"));
                    string availablevers = reader.GetString(reader.GetOrdinal("AvailableVersionNumber"));
                    int nocopies = reader.GetInt16(reader.GetOrdinal("NumLicencedCopies"));
                    string usercode = reader.GetString(reader.GetOrdinal("UserCode"));
                    bool archived = false;
                    if (!reader.IsDBNull(reader.GetOrdinal("archived")))
                    {
                        archived = reader.GetBoolean(reader.GetOrdinal("archived"));
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

                    cProduct newprod = new cProduct(prodid, subAccountId, prodcode, prodname, proddesc, (cProductCategory)clsBaseDefs.GetDefinitionByID(prodcatid), archived, createdon, createdby, modifiedon, modifiedby);

                    products.Add(prodid, newprod);
                }
                reader.Close();
            }

			if (products.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
			{
                cache.Add(cacheKey, products, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), System.Web.Caching.CacheItemPriority.Default, null);
			}

			db.sqlexecute.Parameters.Clear();

			return products;
		}

        /// <summary>
        /// Returns the Product Grid HTML
        /// </summary>
        /// <returns></returns>
        public string[] getProductsGrid()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cProducts products = new cProducts(AccountID);
            cFields clsfields = new cFields(AccountID);

            cGridNew productgrid = new cGridNew(AccountID, curUser.EmployeeID, "productgrid", products.getGridSQL);
            productgrid.KeyField = "productid";
            productgrid.addFilter(clsfields.GetFieldByID(new Guid("F575902B-FB96-48DD-BDB2-3B7F486796DC")), ConditionType.Equals, new object[] { 0 }, new object[] { }, ConditionJoiner.None);
            if (curUser.CurrentSubAccountId >= 0)
            {
                productgrid.addFilter(clsfields.GetFieldByID(new Guid("6626D84D-AED3-491E-AB9A-A037C1092E64")), ConditionType.Equals, new object[] { curUser.CurrentSubAccountId }, new object[] { }, ConditionJoiner.And);
            }
            productgrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, false, false);
            productgrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, false, false);
            productgrid.enablepaging = true;
            productgrid.showheaders = true;
            productgrid.showfooters = false;

            // temporary until employees migrated
            productgrid.EnableSorting = false;
            productgrid.SortedColumn = productgrid.getColumnByName("productName");

            productgrid.getColumnByName("productId").hidden = true;
            productgrid.editlink = "javascript:editProduct({ProductId});";
            productgrid.deletelink = "javascript:deleteProduct({ProductId});";

            List<string> retVals = new List<string>();
            retVals.Add(productgrid.GridID);
            retVals.AddRange(productgrid.generateGrid());
            return retVals.ToArray();
        }
	}
}
