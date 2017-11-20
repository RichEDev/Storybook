using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Data;
using SpendManagementLibrary;

namespace Spend_Management
{
	#region cRechargeSettings
	public class cRechargeSettings
	{
		private cRechargeSetting rslist;
		public Cache Cache = (Cache)System.Web.HttpRuntime.Cache;
		private int nAccountId;
		private int? nSubAccountId;
		private string sConnectionString;
		private cAccountProperties properties;

		#region properties
		public int AccountID
		{
			get { return nAccountId; }
		}
		public int? SubAccountID
		{
			get { return nSubAccountId; }
		}
		private string ConnectionString
		{
			get { return sConnectionString; }
		}
		public cRechargeSetting getSettings
		{
			get { return rslist; }
		}
		private string cacheKey
		{
			get
			{
				string key = "RechargeSettings_" + nAccountId.ToString();
				if (SubAccountID.HasValue)
				{
					key += "_" + SubAccountID.Value.ToString();
				}
				return key;
			}
		}
		#endregion

		public cRechargeSettings(int accountid, int? subaccountid, string connStr)
		{
			nAccountId = accountid;
			nSubAccountId = subaccountid;
			sConnectionString = connStr;

			cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
			if (subaccountid.HasValue)
			{
				properties = subaccs.getSubAccountById(subaccountid.Value).SubAccountProperties;
			}
			else
			{
				properties = subaccs.getFirstSubAccount().SubAccountProperties;
			}

			rslist = properties.RechargeSettings;
		}
	}
	#endregion
}
