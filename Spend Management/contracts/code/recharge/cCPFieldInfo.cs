using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
	#region cCPFieldInfoItem
	/// <summary>
	/// cCPFieldInfoItem class
	/// </summary>
	public class cCPFieldInfoItem
	{
		private int nCP_UF1;
		public int CP_UF1
		{
			get { return nCP_UF1; }
			set { nCP_UF1 = value; }
		}

		private int nCP_UF2;
		public int CP_UF2
		{
			get { return nCP_UF2; }
			set { nCP_UF2 = value; }
		}

		private UserFieldType uftCP_UF1_Type;
		public UserFieldType CP_UF1_Type
		{
			get { return uftCP_UF1_Type; }
			set { uftCP_UF1_Type = value; }
		}

		private UserFieldType uftCP_UF2_Type;
		public UserFieldType CP_UF2_Type
		{
			get { return uftCP_UF2_Type; }
			set { uftCP_UF2_Type = value; }
		}

		private int nCP_UF1_FId;
		public int CP_UF1_FId
		{
			get { return nCP_UF1_FId; }
			set { nCP_UF1_FId = value; }
		}

		private int nCP_UF2_FId;
		public int CP_UF2_FId
		{
			get { return nCP_UF2_FId; }
			set { nCP_UF2_FId = value; }
		}

		private SortedList slCP_UF1_Coll;
		public SortedList CP_UF1_Coll
		{
			get { return slCP_UF1_Coll; }
			set { slCP_UF1_Coll = value; }
		}

		private SortedList slCP_UF2_Coll;
		public SortedList CP_UF2_Coll
		{
			get { return slCP_UF2_Coll; }
			set { slCP_UF2_Coll = value; }
		}

		private string sCP_UF1_Title;
		public string CP_UF1_Title
		{
			get { return sCP_UF1_Title; }
			set { sCP_UF1_Title = value; }
		}

		private string sCP_UF2_Title;
		public string CP_UF2_Title
		{
			get { return sCP_UF2_Title; }
			set { sCP_UF2_Title = value; }
		}

		/// <summary>
		/// cCPFieldInfoItem constructor
		/// </summary>
		/// <param name="uf1"></param>
		/// <param name="uf1type"></param>
		/// <param name="uf1Id"></param>
		/// <param name="uf1_coll"></param>
		/// <param name="uf1title"></param>
		/// <param name="uf2"></param>
		/// <param name="uf2type"></param>
		/// <param name="uf2Id"></param>
		/// <param name="uf2_coll"></param>
		/// <param name="uf2title"></param>
		public cCPFieldInfoItem(int uf1, UserFieldType uf1type, int uf1Id, SortedList uf1_coll, string uf1title, int uf2, UserFieldType uf2type, int uf2Id, SortedList uf2_coll, string uf2title)
		{
			CP_UF1 = uf1;
			CP_UF1_Type = uf1type;
			CP_UF1_FId = uf1Id;
			CP_UF1_Title = uf1title;
			CP_UF1_Coll = uf1_coll;

			CP_UF2 = uf2;
			CP_UF2_Type = uf2type;
			CP_UF2_FId = uf2Id;
			CP_UF2_Title = uf2title;
			CP_UF2_Coll = uf2_coll;
		}

		public cCPFieldInfoItem()
		{
			CP_UF1 = 0;
			CP_UF1_Type = UserFieldType.Character;
			CP_UF1_FId = 0;
			CP_UF1_Title = "";
			CP_UF1_Coll = null;

			CP_UF2 = 0;
			CP_UF2_Type = UserFieldType.Character;
			CP_UF2_FId = 0;
			CP_UF2_Title = "";
			CP_UF2_Coll = null;
		}
	}
	#endregion

	#region cCPFieldInfo
	/// <summary>
	/// cCPFieldInfo class
	/// </summary>
	public class cCPFieldInfo
	{
		private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		private cCPFieldInfoItem curitem;
		//private UserInfo cUInfo;
		//private cFWSettings cFWS;
		private int nAccountId;
		private int? nSubAccountId;
		private int nEmployeeId;
		private string sConnectionString;
		private int curContractCategory;
		private cAccountProperties properties;


		#region properties
		public cCPFieldInfoItem CPFieldInfoItem
		{
			get { return curitem; }
		}
		public int AccountID
		{
			get { return nAccountId; }
		}
		public string ConnectionString
		{
			get { return sConnectionString; }
		}
		public int? SubAccountId
		{
			get { return nSubAccountId;}
		}
		public int EmployeeId
		{
			get { return nEmployeeId; }
		}
		private string cacheKey
		{
			get
			{
				string key = "CPFieldInfo" + "_" + curContractCategory.ToString() + "_" + EmployeeId.ToString();
				if (nSubAccountId.HasValue)
				{
					key += "_" + SubAccountId.Value.ToString();
				}
				return key;
			}
		}
		#endregion

		public cCPFieldInfo(int accountid, int? subaccountid, string connStr, int employeeid, int activeContract)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
			if (subaccountid.HasValue)
			{
				properties = subaccs.getSubAccountById(subaccountid.Value).SubAccountProperties;
			}
			else
			{
				properties = subaccs.getFirstSubAccount().SubAccountProperties;
			}
			nEmployeeId = employeeid;
			sConnectionString = connStr;

			DBConnection db = new DBConnection(connStr);
			db.sqlexecute.Parameters.AddWithValue("@conId", activeContract);
			curContractCategory = db.getcount("select isnull([categoryId],0) from contract_details where [contractId] = @conId");

			if (Cache[cacheKey] == null)
			{
				curitem = CacheItems();
			}
			else
			{
				curitem = (cCPFieldInfoItem)Cache[cacheKey];
			}
		}

		public cCPFieldInfo(int accountid, int? subaccountid, int employeeid, int ContractCategoryId)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			nEmployeeId = employeeid;
			curContractCategory = ContractCategoryId;

			if (Cache[cacheKey] == null)
			{
				curitem = CacheItems();
			}
			else
			{
				curitem = (cCPFieldInfoItem)Cache[cacheKey];
			}
		}

		private cCPFieldInfoItem CacheItems()
		{
			cCPFieldInfoItem newItem = GetCPUFInfo();
			
			return newItem;
		}

		public void RefreshItems()
		{
			cCPFieldInfoItem item = GetCPUFInfo();
		}

		private cRechargeSetting SetRechargeProperties()
		{
			return properties.RechargeSettings;
		}

		private string GetRechargeApportionType(int AppId)
		{
			string retStr = "";

			switch ((RechargeApportionType)(AppId))
			{
				case RechargeApportionType.Fixed:
					retStr = "Fixed";
					break;
				case RechargeApportionType.n_Units:
					retStr = "n_Units";
					break;
				case RechargeApportionType.Percentage:
					retStr = "Percentage";
					break;
				default:
					retStr = "Unknown";
					break;
			}

			return retStr;
		}

		private string GetSurchargeType(int AppId)
		{
			string retStr = "";

			switch ((SurchargeType)AppId)
			{
				case SurchargeType.Fixed:
					retStr = "Fixed";
					break;
				case SurchargeType.Percentage:
					retStr = "Percentage";
					break;
				default:
					retStr = "Unknown";
					break;
			}

			return retStr;
		}

		private cCPFieldInfoItem GetCPUFInfo()
		{
			DBConnection db = new DBConnection(ConnectionString);
			cEmployees employees = new cEmployees(AccountID);

			cCPFieldInfoItem cCOLL = new cCPFieldInfoItem();
			
			string tmpSQL = "";

			string sql = "select [UF1_Id], [UF2_Id] from user_preferences where [subaccountid] = @subaccId and [employeeId] = @employeeId and [categoryId] = @catId";
            db.sqlexecute.Parameters.AddWithValue("@subaccId", SubAccountId.Value);
			db.sqlexecute.Parameters.AddWithValue("@employeeId", EmployeeId);
			db.sqlexecute.Parameters.AddWithValue("@catId", curContractCategory);
			SortedList<string, object> fielddepparams = new SortedList<string, object>();
            fielddepparams.Add("@subaccId", SubAccountId.Value);
			fielddepparams.Add("@employeeId", EmployeeId);
			fielddepparams.Add("@catId", curContractCategory);

		    System.Web.Caching.SqlCacheDependency fielddep = null;
		    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
		    {
                fielddep = db.CreateSQLCacheDependency(sql, fielddepparams);
            }

			DataSet dset = db.GetDataSet(sql);
			System.Web.Caching.SqlCacheDependency uf1dep = null;
			System.Web.Caching.SqlCacheDependency uf2dep = null;
			SortedList<string, object> uf1params = new SortedList<string, object>();

			foreach (DataRow drow in dset.Tables[0].Rows)
			{
				//cCOLL.CP_UF1 = int.Parse(db.FWDbFindVal("Value", 1));
				cCOLL.CP_UF1 = (int)drow["UF1_Id"];
				if (cCOLL.CP_UF1 > 0)
				{
					string sqlinner = "select [Field Type], [Field Name] from user_fields where [User Field Id] = @UFId";
					db.sqlexecute.Parameters.Clear();
					db.sqlexecute.Parameters.AddWithValue("@UFId", cCOLL.CP_UF1);

					uf1params.Add("@UFId", cCOLL.CP_UF1);

				    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
				    {
				        uf1dep = db.CreateSQLCacheDependency(sqlinner, uf1params);
				    }

				    DataSet dset2 = db.GetDataSet(sqlinner);

					foreach (DataRow drow2 in dset2.Tables[0].Rows)
					{
						cCOLL.CP_UF1_Type = (UserFieldType)((int)drow2["Field Type"]);
						cCOLL.CP_UF1_Title = (string)drow2["Field Name"];

						SortedList tmpColl = new SortedList();

						switch (cCOLL.CP_UF1_Type)
						{
							case UserFieldType.StaffName_Ref:
								tmpSQL = "SELECT employeeid, firstname, surname from employees";
								DataSet emp_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in emp_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["employeeid"], (string)tmpRow["firstname"] + " " + (string)tmpRow["surname"]);
								}
								break;

							case UserFieldType.Site_Ref:
								tmpSQL = "SELECT [Site Id],[Site Code] FROM [codes_sites]";
								DataSet site_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in site_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Site Id"], tmpRow["Site Code"]);
								}
								break;

							case UserFieldType.RechargeClient_Ref:
								tmpSQL = "SELECT [Entity Id],[Name] FROM [codes_rechargeentity]";
								DataSet client_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in client_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Entity Id"], tmpRow["Name"]);
								}
								break;

							case UserFieldType.RechargeAcc_Code:
								tmpSQL = "SELECT [Code Id],[Account Code] FROM [codes_accountcodes]";
								DataSet acc_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in acc_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Code Id"], tmpRow["Account Code"]);
								}
								break;
						}

						cCOLL.CP_UF1_Coll = tmpColl;
					}
				}
				//}

				//db.FWDb("R", "fwparams", "Param", "ADD_CPINFO_UF_2", "", "", "", "", "", "", "", "", "", "");

				//if (db.FWDbFlag)
				//{
				//cCOLL.CP_UF2 = int.Parse(db.FWDbFindVal("Value", 1));
				cCOLL.CP_UF2 = (int)drow["UF2_Id"];
				SortedList<string, object> uf2params = new SortedList<string, object>();

				if (cCOLL.CP_UF2 > 0)
				{
					string sqlinner = "select [Field Type], [Field Name] from user_fields where [User Field Id] = @UFId";
					db.sqlexecute.Parameters.Clear();
					db.sqlexecute.Parameters.AddWithValue("@UFId", cCOLL.CP_UF2);

					uf2params.Add("@UFId", cCOLL.CP_UF2);

				    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
				    {
                        uf2dep = db.CreateSQLCacheDependency(sqlinner, uf2params);
                    }

					DataSet dset2 = db.GetDataSet(sqlinner);

					foreach (DataRow drow2 in dset2.Tables[0].Rows)
					{
						cCOLL.CP_UF2_Type = (UserFieldType)((int)drow2["Field Type"]);
						cCOLL.CP_UF2_Title = (string)drow2["Field Name"];

						SortedList tmpColl = new System.Collections.SortedList();

						switch (cCOLL.CP_UF2_Type)
						{
							case UserFieldType.StaffName_Ref:
								tmpSQL = "SELECT employeeid, firstname, surname from employees";
								DataSet emp_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in emp_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["employeeid"], (string)tmpRow["firstname"] + " " + (string)tmpRow["surname"]);
								}
								break;
							case UserFieldType.Site_Ref:
								tmpSQL = "SELECT [Site Id],[Site Code] FROM [codes_sites]";
								DataSet site_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in site_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Site Id"], tmpRow["Site Code"]);
								}
								break;

							case UserFieldType.RechargeClient_Ref:
								tmpSQL = "SELECT [Entity Id],[Name] FROM [codes_rechargeentity]";
								DataSet client_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in client_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Entity Id"], tmpRow["Name"]);
								}
								break;

							case UserFieldType.RechargeAcc_Code:
								tmpSQL = "SELECT [Code Id],[Account Code] FROM [codes_accountcodes]";
								DataSet acc_dset = db.GetDataSet(tmpSQL);
								foreach (DataRow tmpRow in acc_dset.Tables[0].Rows)
								{
									tmpColl.Add(tmpRow["Code Id"], tmpRow["Account Code"]);
								}
								break;
						}

						cCOLL.CP_UF2_Coll = tmpColl;
					}
				}
			}

		    if (fielddep != null)
		    {
                System.Web.Caching.AggregateCacheDependency aggdep = new System.Web.Caching.AggregateCacheDependency();

                if (uf1dep == null && uf2dep == null)
                {
                    aggdep.Add(new System.Web.Caching.CacheDependency[] { fielddep });
                }
                else if (uf1dep != null && uf2dep == null)
                {
                    aggdep.Add(new System.Web.Caching.CacheDependency[] { fielddep, uf1dep });
                }
                else if (uf1dep == null && uf2dep != null)
                {
                    aggdep.Add(new System.Web.Caching.CacheDependency[] { fielddep, uf2dep });
                }
                else
                {
                    aggdep.Add(new System.Web.Caching.CacheDependency[] { fielddep, uf1dep, uf2dep });
                }

                Cache.Add(cacheKey, cCOLL, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), System.Web.Caching.CacheItemPriority.Default, null);
            }

			return cCOLL;
		}
	}
	#endregion
}
