using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Data;

namespace SpendManagementLibrary
{
	public class cSite
	{
		private int nSiteId;
		private string sSiteCode;
		private string sSiteDesc;
		private int nREId;
		private string sAddr1;
		private string sAddr2;
		private string sTown;
		private string sPostCode;

		#region properties

		public int SiteId
		{
			get { return nSiteId; }
		}
		public string SiteCode
		{
			get { return sSiteCode; }
		}
		public string SiteDescription
		{
			get { return sSiteDesc; }
		}
		public int RechargeEntityId
		{
			get { return nREId; }
		}
		public string AddressLine1
		{
			get { return sAddr1; }
		}
		public string AddressLine2
		{
			get { return sAddr2; }
		}
		public string Town
		{
			get { return sTown; }
		}
		public string PostCode
		{
			get { return sPostCode; }
		}
		#endregion

		public cSite(int siteid, string sitecode, string sitedesc, int rechargeentityid, string addr1, string addr2, string town, string postcode)
		{
			nSiteId = siteid;
			sSiteCode = sitecode;
			sSiteDesc = sitedesc;
			nREId = rechargeentityid;
			sAddr1 = addr1;
			sAddr2 = addr2;
			sTown = town;
			sPostCode = postcode;
		}
	}

	public class cSites
	{
		private int nAccountId;
		private int? nSubAccountId;
		private int nEmployeeId;
		private string sConnectionString;
		private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		private static SortedList<int, cSite> slSites;

		#region properties
		public int Count
		{
			get { return slSites.Count; }
		}
		private string cacheKey
		{
			get
			{
				string key = "sites_" + nAccountId.ToString();
				if (nSubAccountId.HasValue)
				{
					key += "_" + nSubAccountId.Value.ToString();
				}
				return key;		
			}
		}
		#endregion

		public cSites(int accountid, int? subaccountid, int employeeid, string connStr)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			nEmployeeId = employeeid;
            sConnectionString = connStr;
			if (Cache[cacheKey] == null)
			{
				slSites = CacheSites();
			}
			else
			{
				slSites = (SortedList<int, cSite>)Cache[cacheKey];
			}
		}

		private SortedList<int, cSite> CacheSites()
		{
			string sql = "";
			SortedList<int, cSite> list = new SortedList<int, cSite>();
			System.Data.SqlClient.SqlDataReader reader;
			cSite curSite;
			DBConnection db = new DBConnection(sConnectionString);
			SortedList<string, object> depParams = new SortedList<string, object>();

			sql = "SELECT [Site Id], ISNULL([Site Code],'') AS [Site Code], ISNULL([Site Description],'') AS [Site Description], ISNULL([Recharge Entity Id],0) AS [Recharge Entity Id], ISNULL([Address Line 1],'') AS [Address Line 1], ISNULL([Address Line 2],'') AS [Address Line 2], ISNULL([Town],'') AS [Town], ISNULL([Post Code],'') AS [Post Code] FROM [codes_sites]";
			if (nSubAccountId.HasValue)
			{
				sql += "WHERE [Location Id] = @locId";
				db.sqlexecute.Parameters.AddWithValue("@locId", nSubAccountId.Value);
				depParams.Add("@locId", nSubAccountId.Value);
			}
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = db.CreateSQLCacheDependency(sql, depParams);
                Cache.Insert(cacheKey, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            }
		    using (reader = db.GetReader(sql))
		    {
		        while (reader.Read())
		        {
		            int siteId = reader.GetInt32(reader.GetOrdinal("Site Id"));
		            string sitecode = reader.GetString(reader.GetOrdinal("Site Code"));
		            string desc = reader.GetString(reader.GetOrdinal("Site Description"));
		            int rechargeentityid = reader.GetInt32(reader.GetOrdinal("Recharge Entity Id"));
		            string addr1 = reader.GetString(reader.GetOrdinal("Address Line 1"));
		            string addr2 = reader.GetString(reader.GetOrdinal("Address Line 2"));
		            string town = reader.GetString(reader.GetOrdinal("Town"));
		            string pcode = reader.GetString(reader.GetOrdinal("Post Code"));

		            curSite = new cSite(siteId, sitecode, desc, rechargeentityid, addr1, addr2, town, pcode);
		            list.Add(siteId, curSite);
		        }

		        reader.Close();
		    }

			return list;
		}

		public cSite GetSiteById(int siteId)
		{
			cSite site = null;
			if (slSites.ContainsKey(siteId))
			{
				site = (cSite)slSites[siteId];
			}
			return site;
		}

		public cSite GetSiteByCode(string sitecode)
		{
			cSite retSite = null;
			foreach (KeyValuePair<int, cSite> curSite in slSites)
			{
				cSite site = (cSite)curSite.Value;
				if (site.SiteCode == sitecode)
				{
					retSite = site;
					break;
				}
			}
			return retSite;
		}

		public System.Web.UI.WebControls.ListItem[] GetListItems(bool addNoneSelection)
		{
			List<System.Web.UI.WebControls.ListItem> newItem = new List<System.Web.UI.WebControls.ListItem>();
			SortedList<string, cSite> sortedSites = new SortedList<string, cSite>();

			foreach (KeyValuePair<int, cSite> i in slSites)
			{
				cSite curSite = (cSite)i.Value;
				sortedSites.Add(curSite.SiteCode + ":" + curSite.SiteDescription, curSite);
			}

			foreach (KeyValuePair<string, cSite> i in sortedSites)
			{
				cSite curSite = (cSite)i.Value;
				newItem.Add(new System.Web.UI.WebControls.ListItem(curSite.SiteCode + ":" + curSite.SiteDescription, curSite.SiteId.ToString()));
			}

			if (addNoneSelection)
			{
				newItem.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
			}

			return newItem.ToArray();
		}

		public void AddSite(int siteid, cSite site)
		{
			if (slSites.ContainsKey(siteid) == false)
			{
				slSites.Add(siteid, site);
			}
		}

		public void DeleteSite(int siteid)
		{
			if (slSites.ContainsKey(siteid))
			{
				slSites.Remove(siteid);
			}
		}

		public void UpdateSite(int siteid, cSite site)
		{
			if (slSites.ContainsKey(siteid))
			{
				slSites[siteid] = site;
			}
		}

		public SortedList<int, string> GetItems()
		{
			SortedList<int, string> items = new SortedList<int, string>();
			foreach (KeyValuePair<int, cSite> i in slSites)
			{
				cSite site = (cSite)i.Value;
				items.Add(site.SiteId, site.SiteCode + ":" + site.SiteDescription);
			}

			return items;
		}
	}
}
