using System;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using Spend_Management.expenses.code;

namespace Spend_Management
{
    /// <summary>
	/// Summary description for cAuditLog.
	/// </summary>
	public class cAuditLog : IAuditLog
	{
        private int nEmployeeid;
        private int nAccountid;
		private string sCompanyid;
		private string sUsername;

        cAccounts clsaccounts = new cAccounts();

		/// <summary>
		/// Audit Log constructor will obtain user and account info from User.Identity
		/// </summary>
        public cAuditLog()
        {
            var appInfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

		    if (appInfo.User.Identity.Name == "") return;

		    CurrentUser user = cMisc.GetCurrentUser();
		    nAccountid = user.AccountID;
		    nEmployeeid = user.EmployeeID;
		    var employees = new cEmployees(user.AccountID);
		    cAccount account = clsaccounts.GetAccountByID(user.AccountID);
		    sCompanyid = account.companyid;

            if (appInfo.Context.Session != null && appInfo.Session["myid"] != null)
		    {
		        if ((int)appInfo.Session["myid"] != 0)
		        {
		            Employee origUser = employees.GetEmployeeById((int)appInfo.Session["myid"]);
		            sUsername = origUser.Username + " (Logged on as " + user.Employee.Username + ")";
		        }
		        else
		        {
		            sUsername = user.Employee.Username;
		        }
		    }
		    else
		    {
		        if (user.Employee.Username == null)
		        {
		            CurrentUser fwUser = cMisc.GetCurrentUser();
		            sUsername = fwUser != null ? fwUser.Employee.FullName : "Unknown";
		        }
		        else
		        {
		            sUsername = user.Employee.Username;
		        }
		    }
        }

		/// <summary>
		/// Audit Log constructor passing in the account and employeeid (NOTE: Don't use this for Framework until employees merged)
		/// </summary>
		/// <param name="accountid">Account ID</param>
		/// <param name="employeeid">Employee ID of user</param>
        public cAuditLog(int accountid, int employeeid)
        {
            nEmployeeid = employeeid;
            nAccountid = accountid;
            //get the username and companyid;
            cEmployees clsemployees = new cEmployees(accountid);


		    Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            
            cAccount reqaccount = clsaccounts.GetAccountByID(accountid);
            sCompanyid = reqaccount.companyid;

            if (employeeid != 0)
            {
                Employee origuser = clsemployees.GetEmployeeById(employeeid);
                sUsername = origuser.Username + " (Logged on as " + reqemp.Username + ")";
            }
            else
            {
                if (reqemp != null)
                {
                    sUsername = reqemp.Username;
                }
            }

        }

		/// <summary>
		/// Enter into audit log a user logon entry
		/// </summary>
		/// <param name="accountid">Account ID</param>
		/// <param name="employeeid">Employee ID logging in</param>
        public void recordLogon(int accountid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)SpendManagementElement.Employees);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", nEmployeeid);

            if (sUsername == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", sUsername);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", subaccs.getFirstSubAccount().SubAccountID);
            expdata.ExecuteProc("addLoginEntryToAuditLog");
            expdata.sqlexecute.Parameters.Clear();

            // log visit in the logon trace table
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteProc("RegisterLogon");
        }

		/// <summary>
		/// Enter a user logout into the audit log
		/// </summary>
		public void recordLogout()
		{
            
            CurrentUser user = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            if (user != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                if (user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)SpendManagementElement.Employees);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", nEmployeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", sUsername);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", user.CurrentSubAccountId);
            expdata.ExecuteProc("addLogoutEntryToAuditLog");
            expdata.sqlexecute.Parameters.Clear();
		}

		/// <summary>
		/// Audit log a record insertion
		/// </summary>
		/// <param name="category">Description of where record add has occurred</param>
		/// <param name="newvalue">Record description to identify record</param>
        public void addRecord(SpendManagementElement element, string newvalue, int id, bool fromReportsOrScheduler = false)
		{
            if (sCompanyid == null)
            {
                return;
            }

            CurrentUser user = fromReportsOrScheduler ? cMisc.GetCurrentUser(this.nAccountid + "," + this.nEmployeeid, true) : cMisc.GetCurrentUser();

            if (user != null)
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                if (user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)element);
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", id);
                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", newvalue);
                if (fromReportsOrScheduler)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", user.CurrentSubAccountId);
                }
                
                expdata.ExecuteProc("addInsertEntryToAuditLog");
                expdata.sqlexecute.Parameters.Clear();
            }
		}

		/// <summary>
		/// editRecord: Audit log a database update action
		/// </summary>
		/// <param name="item">Item being updated</param>
		/// <param name="field">Field description</param>
		/// <param name="category">Application area description</param>
		/// <param name="oldvalue">Pre-update value</param>
		/// <param name="newvalue">Post-update value</param>
		public void editRecord(int id, string item, SpendManagementElement element, Guid fieldID, string oldvalue, string newvalue)
		{
            
            CurrentUser user = cMisc.GetCurrentUser();

            if (user != null)
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                if (user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)element);
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", id);
                expdata.sqlexecute.Parameters.AddWithValue("@field", fieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", item);
                expdata.sqlexecute.Parameters.AddWithValue("@oldvalue", oldvalue);
                expdata.sqlexecute.Parameters.AddWithValue("@newvalue", newvalue);
                expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", user.CurrentSubAccountId);
                expdata.ExecuteProc("addUpdateEntryToAuditLog");

                expdata.sqlexecute.Parameters.Clear();
            }
		}

        /// <summary>
        /// deleteRecord: Audit log a database delete action
        /// </summary>
        /// <param name="category">Application area description</param>
        /// <param name="newvalue">Record description to identify record</param>
        /// <param name="currentUser">Pass a CurentUser if you don't want the session to be looked at.</param>
        public void deleteRecord(SpendManagementElement element, int id, string newvalue, ICurrentUser currentUser = null)
        {

            CurrentUser user = (currentUser ?? cMisc.GetCurrentUser()) as CurrentUser;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            if (user != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                if (user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)element);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", id);
            expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", newvalue);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", user.CurrentSubAccountId);
            expdata.ExecuteProc("addDeleteEntryToAuditLog");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// viewRecord: Audit log a database view action
        /// </summary>
        /// <param name="element">Application area description</param>
        /// <param name="value">Record description to identify record</param>
        /// <param name="currentUser">Pass a CurentUser if you don't want the session to be looked at.</param>
        public void ViewRecord(SpendManagementElement element, string value, ICurrentUser currentUser)
        {
            CurrentUser user = (currentUser ?? cMisc.GetCurrentUser()) as CurrentUser;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            if (user != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                if (user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)element);
            expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", value);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", user.CurrentSubAccountId);
            expdata.ExecuteProc("AddViewEntryToAuditLog");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Notify when user clearing the audit log.
        /// </summary>
        /// <param name="user">current user</param>
        public void NotificationOnAuditLogCleared(CurrentUser user)
        {
            try
            {
                var templateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenAnEmployeeClearAuditLog);
                var senderId =user.EmployeeID;
                int[] recipientIds = new cEmployees(user.AccountID).GetEmployeeIdByNotificationType((int)EmailNotificationType.AuditLogCleared).ToArray();
                new NotificationTemplates(user).SendMessage(templateName, senderId, recipientIds, defaultSender: "admin@sel-expenses.com");
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Failed to send audit log cleared email\nAccountID: " + user.AccountID + "\nReason: " + ex);
            }
        }

        public bool clearAuditLog()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (user != null)
            {
                if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AuditLog, true, false))
                {
                    DBConnection data = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                    data.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                    if (user.isDelegate)
                    {
                        data.sqlexecute.Parameters.AddWithValue("@delegateid", user.Delegate.EmployeeID);
                    }
                    else
                    {
                        data.sqlexecute.Parameters.AddWithValue("@delegateid", DBNull.Value);
                    }

                    data.ExecuteProc("clearAuditLog");
                    data.sqlexecute.Parameters.Clear();
                    return true;
                }
            }

            return false;
        }

        #region properties
        public int AccountID
        {
            get { return nAccountid; }
        }
        #endregion
        /// <summary>
		/// TEMPORARY OVERRIDE FOR USE BY FRAMEWORK UNTIL EMPLOYEES MERGED!
		/// </summary>
		public string UserName
		{
			set { sUsername = value; }
		}
	}
}
