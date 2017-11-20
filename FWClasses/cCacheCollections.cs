using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SpendManagementLibrary;
using Spend_Management;

namespace FWClasses
{
    using SpendManagementLibrary.Employees;

    public class cCacheCollections
    {
        public delegate void CacheRTCollection(int contractId);
        public delegate void CacheRPCollection(cCPFieldInfo CP_UF_Coll, int activeContract);

        private CurrentUser curUser;
		private cAccountProperties accProperties;

        public cCacheCollections(CurrentUser currentuser, Employee curemployee)
        {
			curUser = currentuser;

			cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
			accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
        }

        public void CacheTemplateCollection(int contractId)
        {
			cRechargeCollection rtcoll = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, contractId, cAccounts.getConnectionString(curUser.AccountID), accProperties);
        }

        public void CachePaymentCollection(cCPFieldInfo CP_UF_Coll, int activeContract)
        {
            int accountid = curUser.AccountID;
			cProducts products = new cProducts(accountid, curUser.CurrentSubAccountId);
			
			cRechargePaymentCollection rpcoll = new cRechargePaymentCollection(accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(accountid), CP_UF_Coll, activeContract, products, DateTime.Today);
			cRechargePaymentCollection rpcoll2 = new cRechargePaymentCollection(accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(accountid), CP_UF_Coll, activeContract, products, DateTime.Today.AddMonths(-1));
			cRechargePaymentCollection rpcoll3 = new cRechargePaymentCollection(accountid, curUser.CurrentSubAccountId, cAccounts.getConnectionString(accountid), CP_UF_Coll, activeContract, products, DateTime.Today.AddMonths(1));

        }
    }
}
