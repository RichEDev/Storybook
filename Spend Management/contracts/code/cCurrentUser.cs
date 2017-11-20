using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    [Obsolete("DUPLICATE ALERT", true)]
    public class cCurrentUser
	{
		private int nEmployeeId;
		public int EmployeeID
		{
            get { return nEmployeeId; }
		}

		private int nAccountId;
		public int AccountID
		{
            get { return nAccountId; }
		}
		private cFWSettings cFWS;
		public cFWSettings UserFWS
		{
			get { return cFWS; }
		}
		private Employee curUser;
		public Employee currentUser
		{
			get { return curUser; }
			set { curUser = value; }
		}

		public cCurrentUser(string identity, string meta_connectionstring)
		{
			// identity should contain <customer id>,<user id>
			string[] ids = identity.Split(',');
			nAccountId = int.Parse(ids[0]);
			nEmployeeId = int.Parse(ids[1]);

            cAccounts accs = new cAccounts();
            cAccount acc = accs.GetAccountByID(nAccountId);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);

            cFWS = cMigration.ConvertToFWSettings(acc, subaccs.getSubAccountsCollection(), null);

            cFWS.MetabaseConnectionString = cAccounts.getConnectionString(AccountID);

            cEmployees emps = new cEmployees(AccountID);
            curUser = emps.GetEmployeeById(EmployeeID);
		}
	}
}
