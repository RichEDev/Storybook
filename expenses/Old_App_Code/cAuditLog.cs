using System;
using ExpensesLibrary;
using expenses.Old_App_Code;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for cAuditLog.
	/// </summary>
	public class cAuditLog
	{
        private int nEmployeeid;
		private string strsql;
		private DBConnection expdata = null;
		private string sCompanyid;
		private string sUsername;

        cAccounts clsaccounts = new cAccounts();


        

        public cAuditLog()
        {
            //get the username and companyid;
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            cAccount reqaccount;
            if (appinfo.User.Identity.Name != "")
            {
                CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

                cEmployees clsemployees = new cEmployees(user.accountid);
                cEmployee reqemp = clsemployees.GetEmployeeById(user.employeeid);
                cMisc clsmisc = new cMisc(reqemp.accountid);
                reqaccount = clsaccounts.getAccountById(reqemp.accountid);
                expdata = new DBConnection(cAccounts.getConnectionString(reqemp.accountid));
                sCompanyid = reqaccount.companyid;
                if (appinfo.Session["myid"] != null)
                {

                    if ((int)appinfo.Session["myid"] != 0)
                    {
                        cEmployee origuser = clsemployees.GetEmployeeById((int)appinfo.Session["myid"]);
                        sUsername = origuser.username + " (Logged on as " + reqemp.username + ")";
                    }
                    else
                    {
                        sUsername = reqemp.username;
                    }
                }
                else
                {
                    sUsername = reqemp.username;
                }
            }
        }

        public cAuditLog(int accountid, int employeeid)
        {
            nEmployeeid = employeeid;
            //get the username and companyid;
            cEmployees clsemployees = new cEmployees(accountid);
            cAccount reqaccount;

            //if (appinfo.User.Identity.Name != "")
            //{
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            
            reqaccount = clsaccounts.getAccountById(accountid);
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            sCompanyid = reqaccount.companyid;
            //if (appinfo.Session["myid"] != null)
            //{

            if (employeeid != 0)
            {
                cEmployee origuser = clsemployees.GetEmployeeById(employeeid);
                sUsername = origuser.username + " (Logged on as " + reqemp.username + ")";
            }
            else
            {
                sUsername = reqemp.username;
            }
            //}
            //else
            //{
            //    sUsername = reqemp.username;
            //}
            //}
        }

		public void recordLogon(int accountid, int employeeid)
		{
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			
				cEmployees clsemployees = new cEmployees(accountid);
				cAccount reqaccount;
				cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);

				cMisc clsmisc = new cMisc(reqemp.accountid);
				reqaccount = clsaccounts.getAccountById(reqemp.accountid);
				sCompanyid = reqaccount.companyid;
			sUsername = reqemp.username;
			
			strsql = "insert into [audit_log] (companyid, username, action, category) " + 
				"values (@companyid,@username,@action, @category)";
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",sCompanyid);
			expdata.sqlexecute.Parameters.AddWithValue("@username",sUsername);
			expdata.sqlexecute.Parameters.AddWithValue("@action","I");
			expdata.sqlexecute.Parameters.AddWithValue("@category","Logged In");
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void recordLogout()
		{
		
			strsql = "insert into [audit_log] (companyid, username, action, category) " + 
				"values (@companyid,@username,@action, @category)";
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",sCompanyid);
			expdata.sqlexecute.Parameters.AddWithValue("@username",sUsername);
			expdata.sqlexecute.Parameters.AddWithValue("@action","O");
			expdata.sqlexecute.Parameters.AddWithValue("@category","Logged Out");
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void addRecord(string category, string newvalue)
		{
            if (sCompanyid == null)
            {
                return;
            }
			strsql = "insert into audit_log (companyid, username, action, category, newvalue) " +
				"values (@companyid, @username, @action, @category, @newvalue)";
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",sCompanyid);
			expdata.sqlexecute.Parameters.AddWithValue("@username",sUsername);
			expdata.sqlexecute.Parameters.AddWithValue("@action","A");
			expdata.sqlexecute.Parameters.AddWithValue("@category",category);
			expdata.sqlexecute.Parameters.AddWithValue("@newvalue",newvalue);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void editRecord(string item, string field, string category, string oldvalue, string newvalue)
		{
			strsql = "insert into audit_log (companyid, username, action, category, field, oldvalue, newvalue) " +
				"values (@companyid, @username, @action, @category, @field, @oldvalue, @newvalue)";
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",sCompanyid);
			expdata.sqlexecute.Parameters.AddWithValue("@username",sUsername);
			expdata.sqlexecute.Parameters.AddWithValue("@action","E");
			expdata.sqlexecute.Parameters.AddWithValue("@category",category);
			if (item != newvalue)
			{
				field += " (" + item + ")";
			}
			expdata.sqlexecute.Parameters.AddWithValue("@field",field);
			
			expdata.sqlexecute.Parameters.AddWithValue("@oldvalue",oldvalue);
			expdata.sqlexecute.Parameters.AddWithValue("@newvalue",newvalue);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void deleteRecord(string category, string newvalue)
		{
			strsql = "insert into audit_log (companyid, username, action, category, newvalue) " +
				"values (@companyid, @username, @action, @category, @newvalue)";
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",sCompanyid);
			expdata.sqlexecute.Parameters.AddWithValue("@username",sUsername);
			expdata.sqlexecute.Parameters.AddWithValue("@action","D");
			expdata.sqlexecute.Parameters.AddWithValue("@category",category);
			expdata.sqlexecute.Parameters.AddWithValue("@newvalue",newvalue);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

        public System.Collections.ArrayList getCategoryList()
        {
            System.Collections.ArrayList lst = new System.Collections.ArrayList();
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select distinct category from audit_log order by category";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                lst.Add(reader.GetString(0));
            }
            reader.Close();
            
            return lst;
        }

        public System.Data.DataSet getLog(string category, string action, DateTime startdate, DateTime enddate, string employee)
        {
            System.Data.DataSet ds;
            
            strsql = "select * from audit_log where companyid = @companyid and (datestamp between @startdate and @enddate)";
            expdata.sqlexecute.Parameters.AddWithValue("@companyid", sCompanyid);
            expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
            expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);
            if (category.ToLower() != "** any **")
            {
                strsql += " and category = @category";
                expdata.sqlexecute.Parameters.AddWithValue("@category", category);
            }
            if (action.ToLower() != "** any **") 
            {
                strsql += " and action = @action";
                expdata.sqlexecute.Parameters.AddWithValue("@action", action);
            }
            if (employee != "")
            {
                strsql += " and username like @username";
                expdata.sqlexecute.Parameters.AddWithValue("@username","%" + employee + "%");
            }
            ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return ds;
        }
	}

	public class cAuditEntry
	{
		private int nAuditlogid;
		private string sCompanyid;
		private string sUsername;
		private DateTime dtDatestamp;
		private char cAction;
		private string sCategory;
		private string sOldvalue;
		private string sNewvalue;

		public cAuditEntry(int auditlogid, string companyid, string username, DateTime datestamp, char action, string category, string oldvalue, string newvalue)
		{
			nAuditlogid = auditlogid;
			sCompanyid = companyid;
			sUsername = username;
			dtDatestamp = datestamp;
			cAction = action;
			sCategory = category;
			sOldvalue = oldvalue;
			sNewvalue = newvalue;
		}
	}
}
