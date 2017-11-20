using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Caching;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
	#region cRechargeCollection Class
	[Serializable()]
	public class cRechargeCollection
	{
		private DBConnection activeDBConn;
		private int nAccountId;
		private int? nSubAccountId;
		private int nEmployeeId;
		//private cFWSettings cFWS;
		//private UserInfo cUInfo;
		private SortedList RechargeItemCollection;
		private cCPFieldInfo CP_UFInfoColl;
		private int curContractId;
		private int curContractCurrencyId;
		private string sConnectionString;
		private cAccountProperties accProperties;

		/// <summary>
		/// reference to cache
		/// </summary>
		private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

		#region properties
		/// <summary>
		/// Gets the current customer account ID
		/// </summary>
		public int AccountID
		{
			get { return nAccountId; }
		}
		/// <summary>
		/// Gets current sub account id (NULL if unused)
		/// </summary>
		public int? SubAccountID
		{
			get { return nSubAccountId; }
		}
		private string cacheKey
		{
			get
			{
				string key = "RT_" + nAccountId.ToString();
				if (nSubAccountId.HasValue)
				{
					key += "_" + nSubAccountId.ToString();
				}
				key += "_" + curContractId.ToString();

				return key;
			}
		}
		/// <summary>
		/// Gets the number of recharge items in the collection
		/// </summary>
		public int Count
		{
			get { return RechargeItemCollection.Count; }
		}
		#endregion

		public cRechargeCollection(int accountid, int? subaccountid, int employeeid, int contractId, string connStr, cAccountProperties properties)
		{
			//cFWS = fws;
			//cUInfo = uinfo;
			nAccountId = accountid;
			curContractId = contractId;
			nEmployeeId = employeeid;
			sConnectionString = connStr;
			accProperties = properties;

			CP_UFInfoColl = new cCPFieldInfo(accountid, subaccountid, connStr, employeeid, contractId);

			if (Cache[cacheKey] == null)
			{
				RechargeItemCollection = CacheItems(contractId);
			}
			else
			{
				RechargeItemCollection = (SortedList)Cache[cacheKey];
			}

			activeDBConn = new DBConnection(cAccounts.getConnectionString(AccountID));
			string sql = "select [ContractCurrency] from contract_details where [ContractId] = @conId";
			activeDBConn.sqlexecute.Parameters.AddWithValue("@conId", curContractId);

			curContractCurrencyId = activeDBConn.getcount(sql);
		}

		/// <summary>
		/// Cache the recharge items for particular contract
		/// </summary>
		/// <param name="contractId">ID of contract to obtain items for</param>
		/// <returns>Sorted list  of recharge items</returns>
		private SortedList CacheItems(int contractId)
		{
			SortedList list = new SortedList();

			activeDBConn = new DBConnection(cAccounts.getConnectionString(AccountID));

			System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.Append("SELECT [contract_details].[ContractId],[contract_details].[ContractKey],[contract_details].[ContractNumber],[contract_productdetails].[MaintenanceValue],[contract_productdetails].[Quantity],[productDetails].[ProductId],[productDetails].[ProductName],[recharge_associations].*,ISNULL(contract_productdetails.[CurrencyId],0) AS [CurrencyId],[codes_rechargeentity].[Name] ");
			sql.Append(", dbo.GetUnrecoveredRecharge(CONVERT(datetime,CAST(DATEPART(year,getdate()) AS nvarchar) + '-' + CAST(DATEPART(month,getdate()) AS nvarchar) + '-01',120),[contract_productdetails].[ContractProductId]) AS [Unrecovered] ");
			sql.Append("FROM [recharge_associations] ");
			sql.Append("INNER JOIN [contract_productdetails] ON [contract_productdetails].[ContractProductId] = [recharge_associations].[ContractProductId] ");
			sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId] ");
            sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] ");
			sql.Append("INNER JOIN [codes_rechargeentity] ON [recharge_associations].[RechargeEntityId] = [codes_rechargeentity].[EntityId] ");
			sql.Append("WHERE contract_productdetails.[contractId] = @conId ");
			sql.Append("ORDER BY [ProductName],[Name]");
			activeDBConn.sqlexecute.Parameters.AddWithValue("@conId", contractId);

			DataSet dset = activeDBConn.GetDataSet(sql.ToString());
			cEmployees emps = new cEmployees(AccountID);

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				cUserdefinedFields ufield = new cUserdefinedFields(AccountID);
				cUserdefinedFieldGroupings ufgrps = new cUserdefinedFieldGroupings(AccountID);
				Dictionary<int, cUserdefinedFieldGrouping> grps = ufgrps.GetGroupingByAssocTable(new Guid(ReportTable.RechargeAssociations));

				cRecharge rItem = new cRecharge(AccountID, SubAccountID, sConnectionString);
				cRechargeServiceDates sdates = new cRechargeServiceDates(AccountID, SubAccountID, sConnectionString, (int)drow["ContractProductId"], new DateTime(1900, 1, 1), new DateTime(2020, 12, 31));

				if (rItem.SetRechargeProperties(drow, sdates.GetServiceDateList, grps))
				{
					list.Add("RI" + drow["RechargeId"].ToString(), rItem);
				}
			}

            if (list.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
			{
				SortedList<string, object> dbParams = new SortedList<string, object>();
				dbParams.Add("@conId", contractId);
				SqlCacheDependency dep = activeDBConn.CreateSQLCacheDependency(sql.ToString(), dbParams);
				Cache.Add(cacheKey, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), System.Web.Caching.CacheItemPriority.Default, null);
			}
			return list;
		}

		private double RecalcUnrecovered(cRecharge rItem)
		{
			double newVal = 0;
			DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

			string sql = "SELECT dbo.GetUnrecoveredRecharge(CONVERT(datetime,CAST(DATEPART(year,getdate()) AS nvarchar) + '-' + CAST(DATEPART(month,getdate()) AS nvarchar) + '-01',120),@cpId) AS [Unrecovered]";
			db.sqlexecute.Parameters.AddWithValue("@cpId", rItem.ContractProductId);

			newVal = (double)db.getSum(sql);

			return newVal;
		}

		public void UpdateRechargeItem(cRecharge rItem)
		{
			if (rItem != null)
			{
				if (RechargeItemCollection.ContainsKey("RI" + rItem.RechargeId.ToString()))
				{
					rItem.UnrecoveredPercent = RecalcUnrecovered(rItem);
					RechargeItemCollection["RI" + rItem.RechargeId.ToString()] = rItem;
				}
			}
		}

		public void AddRechargeItem(cRecharge rItem)
		{
			if (rItem != null)
			{
				if (RechargeItemCollection.ContainsKey("RI" + rItem.RechargeId.ToString()) == false)
				{
					rItem.UnrecoveredPercent = RecalcUnrecovered(rItem);
					RechargeItemCollection.Add("RI" + rItem.RechargeId.ToString(), rItem);
				}
			}
		}

		public void DeleteRechargeItem(cRecharge rItem)
		{
			if (rItem != null)
			{
				if (RechargeItemCollection.ContainsKey("RI" + rItem.RechargeId.ToString()))
				{
					rItem.UnrecoveredPercent = RecalcUnrecovered(rItem);
					RechargeItemCollection.Remove("RI" + rItem.RechargeId.ToString());
				}
			}
		}

		public SortedList GetCollection
		{
			get { return RechargeItemCollection; }
		}

		public string[] GetIdList()
		{
			string[] items = new string[RechargeItemCollection.Count];

			for (int i = 0; i < RechargeItemCollection.Count; i++)
			{
				cRecharge rItem = (cRecharge)RechargeItemCollection.GetByIndex(i);

				items[i] = rItem.RechargeId.ToString();
			}

			return items;
		}

		public cRecharge GetRechargeItemByIndex(int index)
		{
			return (cRecharge)RechargeItemCollection.GetByIndex(index);
		}

		public cRecharge GetRechargeItemById(int rechargeId)
		{
			cRecharge retItem;
			if (RechargeItemCollection.ContainsKey("RI" + rechargeId.ToString()))
			{
				retItem = (cRecharge)RechargeItemCollection["RI" + rechargeId.ToString()];
			}
			else
			{
				retItem = null;
			}

			return retItem;
		}

		public ArrayList GetRechargeItemsByIdList(string csvList)
		{
			ArrayList retList = new ArrayList();

			string[] ids = csvList.Split(',');
			if (ids.Length > 0)
			{
				for (int x = 0; x < ids.Length; x++)
				{
					cRecharge tmpRecharge = GetRechargeItemById(int.Parse(ids[x]));
					if (tmpRecharge != null)
					{
						retList.Add(tmpRecharge);
					}
				}
			}

			return retList;
		}

		public double GetUnrecoveredPercent(int contract_productid)
		{
			double result = 0;

			/*
			cFWDBConnection db = new cFWDBConnection();
			db.DBOpen(cFWS,false);

			db.DBClose();
			*/
			return result;
		}

		public ArrayList GetRechargeItemsByCPId(int contract_productId)
		{
			ArrayList retList = new ArrayList();
			cRecharge rItem;

			System.Collections.IDictionaryEnumerator listEnum;
			listEnum = RechargeItemCollection.GetEnumerator();
			while (listEnum.MoveNext())
			{
				rItem = (cRecharge)listEnum.Value;
				if (rItem.ContractProductId == contract_productId)
				{
					retList.Add(rItem);
				}
			}

			return retList;
		}

		public ArrayList GetRechargeItemsByEntity(int entityId)
		{
			ArrayList retList = new ArrayList();
			cRecharge rItem;

			System.Collections.IDictionaryEnumerator listEnum;
			listEnum = RechargeItemCollection.GetEnumerator();
			while (listEnum.MoveNext())
			{
				rItem = (cRecharge)listEnum.Value;
				if (rItem.RechargeEntityId == entityId)
				{
					retList.Add(rItem);
				}
			}

			return retList;
		}

		public void UpdateCPAnnualCost(int CP_Id, double AnnualCost)
		{
			cRecharge rItem;

			System.Collections.IDictionaryEnumerator listEnum;
			listEnum = RechargeItemCollection.GetEnumerator();
			while (listEnum.MoveNext())
			{
				rItem = (cRecharge)listEnum.Value;
				if (rItem.ContractProductId == CP_Id)
				{
					rItem.Maintenance = AnnualCost;
				}
			}
		}
	}
	#endregion

	#region cRechargeClientList class
	[Serializable()]
	public class cRechargeClientList
	{
		private SortedList<int, cRechargeClient> ClientList;
		private int nAccountId;
		private int? nSubAccountId;
		private string sConnectionString;
		public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

		public cRechargeClientList(int accountid, int? subaccountid, string connStr)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			sConnectionString = connStr;

			if (Cache[cacheKey] == null)
			{
				ClientList = CacheList();
			}
			else
			{
				ClientList = (SortedList<int, cRechargeClient>)Cache[cacheKey];
			}
		}

		#region properties
		private string ConnectionString
		{
			get { return sConnectionString; }
		}
		public int AccountID
		{
			get { return nAccountId; }
		}
		public int? SubAccountId
		{
			get { return nSubAccountId; }
		}
		public int Count
		{
			get { return ClientList.Count; }
		}
		private string cacheKey
		{
			get
			{
				string key = "RechargeClients_" + AccountID.ToString();
				if (SubAccountId.HasValue)
				{
					key += "_" + SubAccountId.Value.ToString();
				}
				return key;
			}
		}
		#endregion

		private SortedList<int, cRechargeClient> CacheList()
		{
			SortedList<int, cRechargeClient> list = new SortedList<int, cRechargeClient>();
			DBConnection db = new DBConnection(ConnectionString);
			
			string sql;
			sql = "SELECT [Entity Id],[Location Id],[Name],[Shared],ISNULL([Staff Rep],0) AS [Staff Rep],ISNULL([Deputy Rep],0) AS [Deputy Rep],ISNULL([Account Mgr],0) AS [Account Mgr],ISNULL([Service Mgr],0) AS [Service Mgr],ISNULL([Notes],'') AS [Notes],[Closed],ISNULL([Date Closed],CONVERT(datetime,'1900-01-01 00:00:00',120)) AS [Date Closed],ISNULL([Date Ceased],CONVERT(datetime,'1900-01-01 00:00:00',120)) AS [Date Ceased],ISNULL([Code],'') AS [Code],ISNULL([Service Line],'') AS [Service Line],ISNULL([Sector],'') AS [Sector] FROM codes_rechargeentity ORDER BY [Name]";
			DataSet dset = db.GetDataSet(sql);

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				int loc = (int)drow["Location Id"];
				int entity = (int)drow["Entity Id"];
				string name = (string)drow["Name"];
				Int16 shared = (Int16)drow["Shared"];
				bool isshared = false;
				if (shared == 1)
				{
					isshared = true;
				}
				int staffrep = (int)drow["Staff Rep"];
				int deputy = (int)drow["Deputy Rep"];
				int accmgr = (int)drow["Account Mgr"];
				int servicemgr = (int)drow["Service Mgr"];
				string notes = (string)drow["Notes"];
				Int16 closed = (Int16)drow["Closed"];
				bool isclosed = false;
				if (closed == 1)
				{
					isclosed = true;
				}
				DateTime dclosed = (DateTime)drow["Date Closed"];
				DateTime dceased = (DateTime)drow["Date Ceased"];
				string code = (string)drow["Code"];
				string sline = (string)drow["Service Line"];
				string sector = (string)drow["Sector"];

				//cRechargeClient item = new cRechargeClient((int)drow["Location Id"], (int)drow["Entity Id"], (string)drow["Name"], (bool)drow["Shared"], (int)drow["Staff Rep"], (int)drow["Deputy Rep"], (int)drow["Account Mgr"], (int)drow["Service Mgr"], (string)drow["Notes"], (bool)drow["Closed"], (DateTime)drow["Date Closed"], (DateTime)drow["Date Ceased"], (string)drow["Code"], (string)drow["Service Line"], (string)drow["Sector"]);
				cRechargeClient item = new cRechargeClient(loc, entity, name, isshared, staffrep, deputy, accmgr, servicemgr, notes, isclosed, dclosed, dceased, code, sline, sector);
				list.Add((int)drow["Entity Id"], item);
			}

            if (list.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
			{
				SqlCacheDependency dep = db.CreateSQLCacheDependency(sql, new SortedList<string,object>());
				Cache.Insert(cacheKey, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), CacheItemPriority.Default, null);
			}
			return list;
		}

		public SortedList GetClientsByLocation(int locationId)
		{
			SortedList list = new SortedList();

			cRechargeClient rClient;
			foreach (KeyValuePair<int, cRechargeClient> i in ClientList)
			{
				rClient = (cRechargeClient)i.Value;
				if (rClient.LocationId == locationId)
				{
					if (!list.ContainsKey(rClient.ClientName))
					{
						list.Add(rClient.ClientName, rClient);
					}
				}
			}
			return list;
		}

		public cRechargeClient GetClientById(int entityid)
		{
			cRechargeClient client = (cRechargeClient)ClientList[entityid];
			if (client == null)
			{
				// NOT FOUND
				client = new cRechargeClient(0, 0, "<unknown>", false, 0, 0, 0, 0, "", false, DateTime.MinValue, DateTime.MinValue, "", "", "");
			}
			return client;
		}

		public cRechargeClient FindClientByCode(string code)
		{
			cRechargeClient client = new cRechargeClient(0, -1, "<not found>", false, 0, 0, 0, 0, "", false, DateTime.MinValue, DateTime.MinValue, "", "", "");

			cRechargeClient rClient;
			foreach (KeyValuePair<int, cRechargeClient> i in ClientList)
			{
				rClient = (cRechargeClient)i.Value;
				if (rClient.Code == code.Trim())
				{
					client = rClient;
					break;
				}
			}
			return client;
		}

		public cRechargeClient FindClientByName(string clientname)
		{
			cRechargeClient client = new cRechargeClient(0, -1, "<not found>", false, 0, 0, 0, 0, "", false, DateTime.MinValue, DateTime.MinValue, "", "", "");

			cRechargeClient rClient;
			foreach (KeyValuePair<int, cRechargeClient> i in ClientList)
			{
				rClient = (cRechargeClient)i.Value;
				if (rClient.ClientName == clientname.Trim())
				{
					client = rClient;
					break;
				}
			}
			return client;
		}

		public void AddClient(int entityid, cRechargeClient client)
		{
			if (ClientList.ContainsKey(entityid) == false)
			{
				ClientList.Add(entityid, client);
			}
		}

		public void DeleteClient(int entityid)
		{
			if (ClientList.ContainsKey(entityid))
			{
				ClientList.Remove(entityid);
			}
		}

		public void UpdateClient(int entityid, cRechargeClient client)
		{
			if (ClientList.ContainsKey(entityid))
			{
				ClientList[entityid] = client;
			}
		}

		public System.Web.UI.WebControls.ListItem[] GetListControlItems(bool addNonSelection, bool sorted)
		{
			List<System.Web.UI.WebControls.ListItem> newItem = new List<System.Web.UI.WebControls.ListItem>();
			SortedList<string, cRechargeClient> sortedItems = new SortedList<string, cRechargeClient>();

			if (sorted)
			{
				foreach (KeyValuePair<int, cRechargeClient> i in ClientList)
				{
					cRechargeClient rClient = (cRechargeClient)i.Value;
					sortedItems.Add(rClient.ClientName + "_" + rClient.EntityId.ToString(), rClient);
				}

				foreach (KeyValuePair<string, cRechargeClient> s in sortedItems)
				{
					cRechargeClient rClient = (cRechargeClient)s.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(rClient.ClientName, rClient.EntityId.ToString()));
				}
			}
			else
			{
				foreach (KeyValuePair<int, cRechargeClient> i in ClientList)
				{
					cRechargeClient rClient = (cRechargeClient)i.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(rClient.ClientName, rClient.EntityId.ToString()));
				}
			}

			if (addNonSelection)
			{
				newItem.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
			}

			return newItem.ToArray();
		}

		public SortedList<int, string> GetItems()
		{
			SortedList<int, string> items = new SortedList<int, string>();

			foreach (KeyValuePair<int, cRechargeClient> ritem in ClientList)
			{
				cRechargeClient rc = (cRechargeClient)ritem.Value;
				items.Add(rc.EntityId, rc.ClientName);
			}

			return items;
		}
	}
	#endregion

	#region Recharge Payment Collection
	[Serializable()]
	public class cRechargePaymentCollection
	{
		private int nAccountId;
		private int? nSubAccountId;
		private int nActiveContractId;
		private string sConnectionString;
		private cRechargeClientList clientList;
		private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		private SortedList<int, cRechargePayment> RechargePaymentCollection;
		private SortedList monthCollections;
		private cCPFieldInfo cColl;
		private cProductsBase productList;

		#region properties
		public int ActiveContractId
		{
			get { return nActiveContractId; }
		}
		public int AccountId
		{
			get { return nAccountId; }
		}
		public int? SubAccountId
		{
			get { return nSubAccountId; }
		}
		public int ContractId
		{
			get { return nActiveContractId; }
		}
		private string ConnectionString
		{
			get { return sConnectionString; }
		}
		public int Count
		{
			get { return RechargePaymentCollection.Count; }
		}
		private string cacheKey
		{
			get
			{
				string key = "RP_" + ContractId.ToString() + "_" + AccountId.ToString() + "_";
				if (SubAccountId.HasValue)
				{
					key += "_" + SubAccountId.ToString();
				}
				return key;
			}
		}
		#endregion

		public cRechargePaymentCollection(int accountid, int? subaccountid, string connStr, cCPFieldInfo CP_UF_Coll, int activeContract, cProductsBase products)
		{
			clientList = new cRechargeClientList(accountid, subaccountid, connStr);
			productList = products;
			sConnectionString = connStr;
			nAccountId = accountid;
			nSubAccountId = subaccountid;

			cColl = CP_UF_Coll;
			nActiveContractId = activeContract;

			if (Cache[cacheKey] == null)
			{
				monthCollections = new SortedList();
			}
			else
			{
				monthCollections = (SortedList)Cache[cacheKey];
			}
		}

		public cRechargePaymentCollection(int accountid, int? subaccountid, string connStr, cCPFieldInfo CP_UF_Coll, int activeContract, cProductsBase products, DateTime PaymentDate)
		{
			clientList = new cRechargeClientList(accountid, subaccountid, connStr);
			productList = products;
			sConnectionString = connStr;
			nAccountId = accountid;
			nSubAccountId = subaccountid; 
			cColl = CP_UF_Coll;
			nActiveContractId = activeContract;

			if (Cache[cacheKey] == null)
			{
				monthCollections = new SortedList();
			}
			else
			{
				monthCollections = (SortedList)Cache[cacheKey];
			}

			RechargePaymentCollection = GetMonthPayments(PaymentDate);
		}

		private SortedList<int, cRechargePayment> GetMonthPayments(DateTime paymentdate)
		{
			string monthCacheKey = "RP_" + AccountId.ToString();
			if (SubAccountId.HasValue)
			{
				monthCacheKey += "_" + SubAccountId.Value.ToString();
			}
			monthCacheKey += "_" + ActiveContractId.ToString() + "_" + paymentdate.Year.ToString() + "_" + paymentdate.Month.ToString();

			if (monthCollections.ContainsKey(monthCacheKey))
			{
				// payment for month exists in the collection
				RechargePaymentCollection = (SortedList<int, cRechargePayment>)monthCollections[monthCacheKey];
			}
			else
			{
				RechargePaymentCollection = CacheItems(ActiveContractId, paymentdate, monthCacheKey);
			}

			return RechargePaymentCollection;
		}

		private SortedList<int, cRechargePayment> CacheItems(int curContractId, DateTime newPaymentDate, string monthCacheKey)
		{
			SortedList<int, cRechargePayment> retList = new SortedList<int, cRechargePayment>();
			StringBuilder sql = new StringBuilder();
			cRechargeClientList rClients = new cRechargeClientList(AccountId, SubAccountId, ConnectionString);

			DBConnection db = new DBConnection(ConnectionString);

            sql.Append("SELECT [contract_productdetails_recharge].*,[contract_productdetails].[ContractId], [productDetails].[ProductId], ISNULL([contract_productdetails].[CurrencyId],0) AS [Currency Id],[productDetails].[ProductName] ");
			if (cColl.CPFieldInfoItem.CP_UF1 > 0)
			{
				sql.Append(",[contract_productdetails].[UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim() + "] ");
			}
			if (cColl.CPFieldInfoItem.CP_UF2 > 0)
			{
				sql.Append(",[contract_productdetails].[UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim() + "] ");
			}
			sql.Append("FROM [contract_productdetails_recharge] ");
			sql.Append("INNER JOIN [recharge_associations] ON [contract_productdetails_recharge].[RechargeId] = [recharge_associations].[RechargeId] ");
			sql.Append("INNER JOIN [contract_productdetails] ON [contract_productdetails].[ContractProductId] = [recharge_associations].[ContractProductId] ");
            sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] ");
			sql.Append("WHERE [contract_productdetails].[ContractId] = @conId AND contract_productdetails_recharge.[RechargePeriod] BETWEEN @startdate AND @enddate ");

			sql.Append(" ORDER BY [ProductName] ASC, contract_productdetails_recharge.[RechargePeriod] ASC");
			db.sqlexecute.Parameters.AddWithValue("@conId", ActiveContractId);
			db.sqlexecute.Parameters.AddWithValue("@startdate", DateTime.Parse("01/" + newPaymentDate.Month.ToString() + "/" + newPaymentDate.Year.ToString()));
			db.sqlexecute.Parameters.AddWithValue("@enddate", DateTime.Parse(DateTime.DaysInMonth(newPaymentDate.Year, newPaymentDate.Month).ToString() + "/" + newPaymentDate.Month.ToString() + "/" + newPaymentDate.Year.ToString()));
			
			SortedList<string, object> cacheParams = new SortedList<string,object>();
			cacheParams.Add("@conId", ActiveContractId);
			cacheParams.Add("@startdate", DateTime.Parse("01/" + newPaymentDate.Month.ToString() + "/" + newPaymentDate.Year.ToString()));
			cacheParams.Add("@enddate",DateTime.Parse(DateTime.DaysInMonth(newPaymentDate.Year, newPaymentDate.Month).ToString() + "/" + newPaymentDate.Month.ToString() + "/" + newPaymentDate.Year.ToString()));

			DataSet dset = db.GetDataSet(sql.ToString());

			RechargePaymentCollection = new SortedList<int, cRechargePayment>();

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				cRechargeClient rClient = rClients.GetClientById((int)drow["RechargeEntityId"]);
				string CPUF1Value = "";
				string CPUF2Value = "";

				if (cColl.CPFieldInfoItem.CP_UF1 > 0)
				{
					switch (cColl.CPFieldInfoItem.CP_UF1_Type)
					{
						case UserFieldType.RechargeAcc_Code:
						case UserFieldType.RechargeClient_Ref:
						case UserFieldType.Site_Ref:
						case UserFieldType.StaffName_Ref:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								CPUF1Value = (string)cColl.CPFieldInfoItem.CP_UF1_Coll[drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()]];
							}
							else
							{
								CPUF1Value = "";
							}
							break;
						case UserFieldType.Float:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								CPUF1Value = ((double)drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()]).ToString();
							}
							else
							{
								CPUF1Value = "0";
							}
							break;
						case UserFieldType.Number:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								CPUF1Value = ((int)drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()]).ToString();
							}
							else
							{
								CPUF1Value = "0";
							}
							break;
						case UserFieldType.DateField:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								CPUF1Value = ((DateTime)drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()]).ToShortDateString();
							}
							else
							{
								CPUF1Value = "";
							}
							break;
						case UserFieldType.Checkbox:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								int tmpChkVal = (int)drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()];
								if (tmpChkVal == 1)
								{
									CPUF1Value = "Yes";
								}
								else
								{
									CPUF1Value = "No";
								}
							}
							else
							{
								CPUF1Value = "No";
							}
							break;
						default:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()] != DBNull.Value)
							{
								CPUF1Value = (string)drow["UF" + cColl.CPFieldInfoItem.CP_UF1.ToString().Trim()];
							}
							else
							{
								CPUF1Value = "";
							}
							break;
					}
				}

				if (cColl.CPFieldInfoItem.CP_UF2 > 0)
				{
					switch (cColl.CPFieldInfoItem.CP_UF2_Type)
					{
						case UserFieldType.RechargeAcc_Code:
						case UserFieldType.RechargeClient_Ref:
						case UserFieldType.Site_Ref:
						case UserFieldType.StaffName_Ref:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								CPUF2Value = (string)cColl.CPFieldInfoItem.CP_UF2_Coll[drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()]];
							}
							else
							{
								CPUF2Value = "";
							}
							break;
						case UserFieldType.Float:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								CPUF2Value = ((double)drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()]).ToString();
							}
							else
							{
								CPUF2Value = "0";
							}
							break;
						case UserFieldType.Number:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								CPUF2Value = ((int)drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()]).ToString();
							}
							else
							{
								CPUF2Value = "0";
							}
							break;
						case UserFieldType.DateField:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								CPUF2Value = ((DateTime)drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()]).ToShortDateString();
							}
							else
							{
								CPUF2Value = "";
							}
							break;
						case UserFieldType.Checkbox:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								int tmpChkVal = (int)drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()];
								if (tmpChkVal == 1)
								{
									CPUF2Value = "Yes";
								}
								else
								{
									CPUF2Value = "No";
								}
							}
							else
							{
								CPUF2Value = "No";
							}
							break;
						default:
							if (drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()] != DBNull.Value)
							{
								CPUF2Value = (string)drow["UF" + cColl.CPFieldInfoItem.CP_UF2.ToString().Trim()];
							}
							else
							{
								CPUF2Value = "";
							}
							break;
					}
				}

				cRechargePayment rpItem = new cRechargePayment(ActiveContractId, (int)drow["CurrencyId"], (int)drow["RechargeItemId"], (int)drow["Recharge Id"], (DateTime)drow["RechargePeriod"], (int)drow["RechargeEntityId"], rClient.ClientName, (int)drow["ProductId"], (string)drow["ProductName"], (int)drow["ContractProductId"], (double)drow["RechargeAmount"], cColl.CPFieldInfoItem.CP_UF1, CPUF1Value, cColl.CPFieldInfoItem.CP_UF2, CPUF2Value);

				retList.Add((int)drow["RechargeItemId"], rpItem);
			}

			monthCollections.Add(monthCacheKey, RechargePaymentCollection);

		    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
		    {
		        SqlCacheDependency dep = db.CreateSQLCacheDependency(sql.ToString(), cacheParams);
		        Cache.Insert(monthCacheKey, monthCollections, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), CacheItemPriority.Default, null);
		    }

		    //Cache.Add("RP_" + fws.MetabaseAccountKey.Replace(" ", "_") + "_" + fws.glDatabase.Replace(" ", "_") + "_" + ActiveContractId.ToString() + "_" + curYear.ToString() + "_" + curMonth.ToString(), retList, getDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), System.Web.Caching.CacheItemPriority.Default, null);

			return retList;
		}

		public void AddRechargePayment(cRechargePayment rPayment)
		{
			RechargePaymentCollection = GetMonthPayments(rPayment.PaymentDate);

			if (RechargePaymentCollection.ContainsKey(rPayment.PaymentId) == false)
			{
				RechargePaymentCollection.Add(rPayment.PaymentId, rPayment);
			}
		}

		public void UpdateRechargePayment(cRechargePayment rPayment)
		{
			RechargePaymentCollection = GetMonthPayments(rPayment.PaymentDate);

			if (RechargePaymentCollection.ContainsKey(rPayment.PaymentId))
			{
				RechargePaymentCollection[rPayment.PaymentId] = rPayment;
			}
		}

		public void DeleteRechargePayment(cRechargePayment rPayment)
		{
			RechargePaymentCollection = GetMonthPayments(rPayment.PaymentDate);

			if (RechargePaymentCollection.ContainsKey(rPayment.PaymentId))
			{
				RechargePaymentCollection.Remove(rPayment.PaymentId);
			}
		}

		public void DeleteRechargePaymentForPeriod(int RechargeId, DateTime paymentstartdate, DateTime paymentenddate)
		{
			ArrayList arItems = GetPaymentsBetween(paymentstartdate, paymentenddate);
			for (int i = 0; i < arItems.Count; i++)
			{
				cRechargePayment rpItem = (cRechargePayment)arItems[i];
				if (rpItem.RechargeId == RechargeId && (rpItem.PaymentDate >= paymentstartdate && rpItem.PaymentDate <= paymentenddate))
				{
					DeleteRechargePayment(rpItem);
				}
			}
		}

		public void DeleteRechargePaymentByClient(int clientid, DateTime paymentstartdate, DateTime paymentenddate)
		{
			ArrayList arItems = GetPaymentsBetween(paymentstartdate, paymentenddate);
			for (int i = 0; i < arItems.Count; i++)
			{
				cRechargePayment rpItem = (cRechargePayment)arItems[i];
				if (rpItem.EntityId == clientid && (rpItem.PaymentDate >= paymentstartdate && rpItem.PaymentDate <= paymentenddate))
				{
					DeleteRechargePayment(rpItem);
				}
			}
		}

		public cRechargePayment GetPaymentById(int paymentId)
		{
			cRechargePayment rPayment = null;
			IDictionaryEnumerator rpEnum = monthCollections.GetEnumerator();

			while (rpEnum.MoveNext())
			{
				RechargePaymentCollection = (SortedList<int, cRechargePayment>)rpEnum.Value;

				if (RechargePaymentCollection.ContainsKey(paymentId))
				{
					rPayment = (cRechargePayment)RechargePaymentCollection[paymentId];
					break;
				}
			}
			return rPayment;
		}

		public ArrayList GetProductPaymentsBetween(int productId, DateTime fromDate, DateTime toDate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromDate;

			while (curdate <= toDate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.PaymentDate >= fromDate && rpItem.PaymentDate <= toDate && rpItem.ProductId == productId)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}

			return retList;
		}

		public ArrayList GetProductPayments(int productId, DateTime fromdate, DateTime todate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromdate;

			while (curdate <= todate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.ProductId == productId)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}
			return retList;
		}

		public ArrayList GetClientPaymentsBetween(int clientId, DateTime fromDate, DateTime toDate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromDate;

			while (curdate <= toDate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.PaymentDate >= fromDate && rpItem.PaymentDate <= toDate && rpItem.EntityId == clientId)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}

			return retList;
		}

		public ArrayList GetClientPayments(int clientId, DateTime fromdate, DateTime todate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromdate;

			while (curdate <= todate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.EntityId == clientId)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}
			return retList;
		}

		public ArrayList GetContractProductPaymentsBetween(int contractproductId, DateTime fromdate, DateTime todate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromdate;

			while (curdate <= todate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{

					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if ((rpItem.PaymentDate >= fromdate) && (rpItem.PaymentDate <= todate) && (rpItem.ContractProductId == contractproductId))
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}

			return retList;
		}

		public ArrayList GetContractProductPayments(int contractproductId, DateTime fromdate, DateTime todate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromdate;

			while (curdate <= todate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.ContractProductId == contractproductId)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}

			return retList;
		}

		public ArrayList GetPaymentsBetween(DateTime fromDate, DateTime toDate)
		{
			ArrayList retList = new ArrayList();
			DateTime curdate = fromDate;

			while (curdate <= toDate)
			{
				RechargePaymentCollection = GetMonthPayments(curdate);

				foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
				{
					cRechargePayment rpItem = (cRechargePayment)i.Value;

					if (rpItem.PaymentDate >= fromDate && rpItem.PaymentDate <= toDate)
					{
						retList.Add(rpItem);
					}
				}

				curdate = curdate.AddMonths(1);
			}
			return retList;
		}

		public ArrayList GetPayments(DateTime paymentdate)
		{
			ArrayList retList = new ArrayList();
			RechargePaymentCollection = GetMonthPayments(paymentdate);

			foreach (KeyValuePair<int, cRechargePayment> i in RechargePaymentCollection)
			{
				cRechargePayment rpItem = (cRechargePayment)i.Value;

				retList.Add(rpItem);
			}

			return retList;
		}
	}
	#endregion

	#region Recharge Account Code Collection class
	[Serializable()]
	public class cRechargeAccountCodes
	{
		private SortedList<int, cRechargeAccountCode> AccountCodeList;
		private int nAccountId;
		private int? nSubAccountId;
		private string sConnectionString;
		public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

		public cRechargeAccountCodes(int accountid, int? subaccountid, string connStr)
		{
			sConnectionString = connStr;
			nAccountId = accountid;
			nSubAccountId = subaccountid;
		

			if (Cache[cacheKey] == null)
			{
				AccountCodeList = CacheList();
			}
			else
			{
				AccountCodeList = (SortedList<int, cRechargeAccountCode>)Cache[cacheKey];
			}
		}

		#region properties
		private string ConnectionString
		{
			get { return sConnectionString; }
		}
		public int AccountID
		{
			get { return nAccountId; }
		}
		public int? SubAccountID
		{
			get { return nSubAccountId; }
		}
		private string cacheKey
		{
			get
			{
				string key = "RechargeAccCodes_" + AccountID.ToString();
				if (SubAccountID.HasValue)
				{
					key += "_" + SubAccountID.Value.ToString();
				}
				return key;
			}
		}
		public int Count
		{
			get { return AccountCodeList.Count; }
		}

		#endregion

		private SortedList<int, cRechargeAccountCode> CacheList()
		{
			SortedList<int, cRechargeAccountCode> list = new SortedList<int, cRechargeAccountCode>();
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(AccountID, SubAccountID.Value, SpendManagementElement.ContractCategories);
            //cContractCategories catlist = new cContractCategories(AccountID, SubAccountID);
			DBConnection db = new DBConnection(ConnectionString);
			
			string sql;
			sql = "SELECT [Code Id],[Location Id],[Account Code],[Description],ISNULL([Category Id],0) AS [Category Id] FROM codes_accountcodes";
			DataSet dset = db.GetDataSet(sql);
			
			cContractCategory cat;

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				int loc = (int)drow["Location Id"];
				int codeid = (int)drow["Code Id"];
				string acc_code = (string)drow["Account Code"];
				string desc = (string)drow["Description"];
				if (drow["Category Id"] != DBNull.Value)
				{
					int catid = (int)drow["Category Id"];
					cat = (cContractCategory)clsBaseDefs.GetDefinitionByID(catid);
				}
				else
				{
					cat = null;
				}

				cRechargeAccountCode acc = new cRechargeAccountCode(codeid, loc, acc_code, desc, cat);
				list.Add(codeid, acc);
			}

            if (list.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
			{
				SqlCacheDependency dep = db.CreateSQLCacheDependency(sql, new SortedList<string, object>());
				Cache.Insert(cacheKey, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), CacheItemPriority.Default, null);
			}

			return list;
		}

		public cRechargeAccountCode GetCodeById(int codeId)
		{
			cRechargeAccountCode acc = null;
			if (AccountCodeList.ContainsKey(codeId))
			{
				acc = AccountCodeList[codeId];
			}

			return acc;
		}

		public void AddCode(int codeid, cRechargeAccountCode code)
		{
			if (AccountCodeList.ContainsKey(codeid) == false)
			{
				AccountCodeList.Add(codeid, code);
			}
		}

		public void DeleteClient(int codeid)
		{
			if (AccountCodeList.ContainsKey(codeid))
			{
				AccountCodeList.Remove(codeid);
			}
		}

		public void UpdateClient(int codeid, cRechargeAccountCode code)
		{
			if (AccountCodeList.ContainsKey(codeid))
			{
				AccountCodeList[codeid] = code;
			}
		}

		public ListItem[] GetListControlItems(bool addNonSelection, bool sorted)
		{
			List<ListItem> newItem = new List<ListItem>();
			SortedList<string, cRechargeAccountCode> sortedItems = new SortedList<string, cRechargeAccountCode>();

			if (sorted)
			{
				foreach (KeyValuePair<int, cRechargeAccountCode> i in AccountCodeList)
				{
					cRechargeAccountCode rAccCode = (cRechargeAccountCode)i.Value;
					sortedItems.Add(rAccCode.AccountCode + "_" + rAccCode.CodeId.ToString(), rAccCode);
				}

				foreach (KeyValuePair<string, cRechargeAccountCode> s in sortedItems)
				{
					cRechargeAccountCode rAccCode = (cRechargeAccountCode)s.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(rAccCode.AccountCode, rAccCode.CodeId.ToString()));
				}
			}
			else
			{
				foreach (KeyValuePair<int, cRechargeAccountCode> i in AccountCodeList)
				{
					cRechargeAccountCode rAccCode = (cRechargeAccountCode)i.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(rAccCode.AccountCode, rAccCode.CodeId.ToString()));
				}
			}

			if (addNonSelection)
			{
				newItem.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
			}

			return newItem.ToArray();
		}

		public SortedList<int, string> GetItems()
		{
			SortedList<int, string> items = new SortedList<int, string>();

			foreach (KeyValuePair<int, cRechargeAccountCode> ritem in AccountCodeList)
			{
				cRechargeAccountCode acc = (cRechargeAccountCode)ritem.Value;
				items.Add(acc.CodeId, acc.AccountCode);
			}

			return items;
		}
	}
	#endregion

	#region cRechargeServiceDates

	[Serializable()]
	public class cRechargeServiceDates
	{
		private SortedList<int, cRechargeServiceDate> servicedatelist;
		private UserInfo uinfo;
		private int nAccountId;
		private int? nSubAccountId;
		private string sConnectionString;
		private int nContractProductId;

		public cRechargeServiceDates(int accountid, int? subaccountid, string connectionString, int CPid, DateTime fromdate, DateTime todate)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			sConnectionString = connectionString;

			nContractProductId = CPid;

			servicedatelist = GetServiceDates(fromdate, todate);

		}

		public cRechargeServiceDates(int accountid, int? subaccountid, string connectionString, int CPid, SortedList<int, cRechargeServiceDate> sdList)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			sConnectionString = connectionString;

			nContractProductId = CPid;

			servicedatelist = sdList;
		}

		#region properties
		public int AccountID
		{
			get { return nAccountId; }
		}
		public int? SubAccountId
		{
			get { return nSubAccountId; }
		}
		public SortedList<int, cRechargeServiceDate> GetServiceDateList
		{
			get { return servicedatelist; }
		}
				
		public int Count
		{
			get { return servicedatelist.Count; }
		}
		#endregion

		private SortedList<int, cRechargeServiceDate> GetServiceDates(DateTime fromdate, DateTime todate)
		{
			SortedList<int, cRechargeServiceDate> sdlist = new SortedList<int, cRechargeServiceDate>();

			string sql = "SELECT * FROM recharge_servicedates INNER JOIN recharge_associations ON recharge_associations.[Recharge Id] = recharge_servicedates.[Recharge Id] WHERE recharge_associations.[Contract-Product Id] = @CPid AND ([Offline from] >= @from OR [Online from] <= @to)";
			DBConnection db = new DBConnection(sConnectionString);

			db.sqlexecute.Parameters.AddWithValue("@CPid", nContractProductId);
			db.sqlexecute.Parameters.AddWithValue("@from", fromdate);
			db.sqlexecute.Parameters.AddWithValue("@to", todate);
			DataSet dset = db.GetDataSet(sql);

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				int serviceid = (int)drow["Service Date Id"];
				int RAid = (int)drow["Recharge Id"];
				DateTime from = (DateTime)drow["Offline from"];
				DateTime to = (DateTime)drow["Online from"];

				cRechargeServiceDate servicedate = new cRechargeServiceDate(serviceid, RAid, from, to);
				sdlist.Add(serviceid, servicedate);
			}

			return sdlist;
		}

		public cRechargeServiceDate GetServiceDateById(int SDid)
		{
			if (servicedatelist.ContainsKey(SDid))
			{
				return servicedatelist[SDid];
			}
			else
			{
				return null;
			}
		}

		public void UpdateServiceDate(int SDid, cRechargeServiceDate sdates)
		{
			if (servicedatelist.ContainsKey(SDid))
			{
				servicedatelist[SDid] = sdates;
			}
		}

		public void AddServiceDate(int SDid, cRechargeServiceDate sdates)
		{
			if (servicedatelist.ContainsKey(SDid) == false)
			{
				servicedatelist.Add(SDid, sdates);
			}
		}

		public bool DeleteServiceDate(int SDid)
		{
			bool success = false;
			if (servicedatelist.ContainsKey(SDid))
			{
				servicedatelist.Remove(SDid);
				success = true;
			}

			return success;
		}
	}
	#endregion

}
