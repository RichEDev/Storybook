using System;
using System.Collections;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Configuration;
using System.Web.Caching;
using System.Text;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for employees.
	/// </summary>
	public class cEmployees
	{
		
		string strsql;
		string strrole;
		int accesstype = 0;
		int nAccountid;

		
        public System.Web.Caching.Cache Cache;
        
        public cEmployees(int accountid)
        {
            nAccountid = accountid;
        
            Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

            
        }
        private void createConnection()
        {

            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            if (appinfo.User.Identity.Name != "")
            {
                CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
                cEmployee reqemp = GetEmployeeById(user.employeeid);
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
                expdata = new DBConnection(cAccounts.getConnectionString(reqemp.accountid));
                nAccountid = accountid;
            }
        }
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        public bool firstlogon(string username, int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int count = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@username", username);
            
			strsql = "select count(*) from employees where username = @username and active = 1 and password is null";
			count = expdata.getcount(strsql);

			expdata.sqlexecute.Parameters.Clear();
			if (count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

        public static bool checkEmailIsUnique(string email)
        {
            DBConnection expdata;
            cAccounts clsaccounts = new cAccounts();
            string strsql;
            DBConnection conn;

            int count = 0;

            foreach (cAccount account in clsaccounts.accounts.Values)
            {
                if (!account.archived)
                {
                    conn = new DBConnection(cAccounts.getConnectionString(account.accountid));
                    strsql = "select count(*) from employees where email = @email";
                    conn.sqlexecute.Parameters.AddWithValue("@email", email);
                    count = count + conn.getcount(strsql);
                    conn.sqlexecute.Parameters.Clear();

                    if (count > 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static CurrentUser getEmployeeidByEmail(string email)
        {
            DBConnection expdata;
            DBConnection conn;
            System.Data.SqlClient.SqlDataReader reader;
            int employeeid = 0;
            CurrentUser user = new CurrentUser();
            user.accountid = 0;
            user.employeeid = 0;
            string strsql;
            cAccounts clsaccounts = new cAccounts();
            foreach (cAccount account in clsaccounts.accounts.Values)
            {
                if (!account.archived)
                {
                    conn = new DBConnection(cAccounts.getConnectionString(account.accountid));
                    conn.sqlexecute.Parameters.AddWithValue("@email", email);

                    strsql = "select employeeid from employees where email = @email";
                    reader = conn.GetReader(strsql);
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) == false)
                        {
                            employeeid = reader.GetInt32(0);
                        }
                    }
                    reader.Close();

                    conn.sqlexecute.Parameters.Clear();
                    
                    if (employeeid > 0)
                    {
                        user.accountid = account.accountid;
                        user.employeeid = employeeid;
                        return user;
                    }
                }
            }
            return user;
        }

        public void sendNewPassword(cEmployee emp)
        {

            byte checkpwd = 1;
            string password = "";

            int accountid = emp.accountid;
            while (checkpwd != 0)
            {
                password = generateRandomPassword();
                checkpwd = checkpassword(password, accountid, emp.employeeid);
            }

            //string hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
            changePassword(accountid, emp.employeeid, emp.password, password, false, checkpwd);
            //changePassword(emp.accountid, emp.employeeid, emp.password, hashpwd, false, checkpwd);
            sendPasswordEmail(emp, password);
        }

        public void sendLogonDetails(cEmployee reqEmployee)
        {
            cMisc clsMisc = new cMisc(accountid);
            cAccounts clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.getAccountById(accountid);
            System.Text.StringBuilder sbEmailContent = new StringBuilder();
            cEmails clsEmails = new cEmails(accountid);

            sbEmailContent.Append("A request for your logon details was made on expenses. Please find your company ID and username below, your password will follow in a separate email:\n\n");
            sbEmailContent.Append("CompanyID: " + reqAccount.companyid + "\n");
            sbEmailContent.Append("Username: " + reqEmployee.username + "\n");
            clsEmails.sendMail("admin@sel-expenses.com", reqEmployee.email, "Expenses - Logon Details", sbEmailContent.ToString());
            sbEmailContent.Remove(0, sbEmailContent.Length - 1);

            // md5 = 32 characters long and c#'s md5 is entirely in upper case.
            // If it is md5'd password or null password (new user) then generate a random new password else decrypt and send.
            if ((reqEmployee.password.Length == 32 && reqEmployee.password.ToUpper() == reqEmployee.password) || reqEmployee.password.Length == 0)
            {
                byte checkPwd = 1;
                string randomPassword = "";

                while (checkPwd != 0)
                {
                    randomPassword = generateRandomPassword();
                    checkPwd = checkpassword(randomPassword, accountid, reqEmployee.employeeid);
                }

                changePassword(accountid, reqEmployee.employeeid, reqEmployee.password, randomPassword, false, checkPwd);

                sbEmailContent.Append("Your password to logon to expenses has been reset successfully. Please find your logon details below. Your new password is:\n\n");
                sbEmailContent.Append(randomPassword);
                clsEmails.sendMail("admin@sel-expenses.com", reqEmployee.email, "Expenses - New Password", sbEmailContent.ToString());
            }
            else
            {
                cSecureData clsSecure = new cSecureData();
                string decryptedPassword = clsSecure.Decrypt(reqEmployee.password);

                sbEmailContent.Append("Your password to logon to expenses is as follows:\n\n");
                sbEmailContent.Append(decryptedPassword);
                clsEmails.sendMail("admin@sel-expenses.com", reqEmployee.email, "Expenses - Password", sbEmailContent.ToString());
            }
        }
                
        private void sendPasswordEmail(cEmployee reqemp, string password)
        {
            cMisc clsmisc = new cMisc(accountid);
            cAccounts clsaccounts = new cAccounts();
            cAccount account = clsaccounts.getAccountById(accountid);
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cEmails clsemails = new cEmails(accountid);

            output.Append("Your password to logon to expenses has been reset successfully. Please find your logon details below. Your new password will follow in a separate email:\n\n");
            output.Append("Company ID: " + account.companyid + "\n");
            output.Append("Username: " + reqemp.username + "\n");
            clsemails.sendMail("admin@sel-expenses.com", reqemp.email, "Expenses - Logon Details", output.ToString());

            output.Remove(0, output.Length - 1);
            output.Append("Your password to logon to expenses has been reset successfully. Please find your logon details below. Your new password is:\n\n");
            output.Append(password);
            output.Append("\n\nPlease note your password is case sensitive.");
            clsemails.sendMail("admin@sel-expenses.com", reqemp.email, "Expenses - New Password", output.ToString());
            
        }
        private string generateRandomPassword()
        {
            int letter = 60;
            int reqpwlength = 8;
            int i;
            string password = "";
            Random rnd = new Random();
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            switch (clsproperties.plength)
            {
                case PasswordLength.AnyLength:
                    reqpwlength = 8;
                    break;
                case PasswordLength.GreaterThan:
                case PasswordLength.Between:
                    reqpwlength = clsproperties.length1 + 1;
                    break;
                case PasswordLength.LessThan:
                    reqpwlength = clsproperties.length1 - 1;
                    break;
                case PasswordLength.EqualTo:
                    reqpwlength = clsproperties.length1;
                    break;
            }

            while (password.Length < reqpwlength)
            {
                letter = 47;
                while (letter < 48 || letter > 122)
                {
                    letter = rnd.Next(47, 122);
                }

                if ((letter >= 48 && letter <= 57) || (letter >= 65 && letter <= 90) || (letter >= 97 && letter <= 122))
                {
                    password += System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)letter });
                }
                else
                {
                    letter = 47;
                }
            }

            if (clsproperties.pupper == true)
            {
                password = password.Remove(0,1);
                letter = rnd.Next(65, 90);
                password += System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)letter });
            }

            if (clsproperties.pnumbers == true)
            {
                password = password.Remove(0,1);
                letter = rnd.Next(48, 57);
                password += System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)letter });
            }

            return password;
        }

        // simon's for adminemployee's
        public System.Data.DataSet generateGrid(string surname, int accountid, int roleid, int groupid, int costcodeid, int departmentid, byte filter, string username, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet rcdstemployees = new System.Data.DataSet();

            surname += "%";
            username += "%";

            strsql = "SELECT employees.employeeid, employees.username, employees.archived, employees.surname + ',' + employees.title + ' ' + employees.firstname as Name, groups.groupname, roles.rolename, dbo.getEmpDepartmentSplit(employees.employeeid) as department, dbo.getEmpCostcodeSplit(employees.employeeid) as costcode " +
                "FROM employees " +
                "left join groups on groups.groupid = employees.groupid " +
                "left join roles on roles.roleid = employees.roleid " +


                "where employees.username not like 'admin%'";

            if (username != "%")
            {
                strsql += " and employees.username LIKE @username";
                expdata.sqlexecute.Parameters.AddWithValue("@username", username);
            }

            if (surname != "%")
            {
                strsql += " and employees.surname LIKE @surname";
                expdata.sqlexecute.Parameters.AddWithValue("@surname", surname);
            }

            if (employeeid != 0)
            {
                strsql += " and employees.employeeid = @employeeid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            }
            if (roleid != 0)
            {
                strsql += " and employees.roleid = @roleid";
                expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
            }
            if (groupid != 0)
            {
                strsql += " and employees.groupid = @groupid";
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            }
            if (costcodeid != 0)
            {
                strsql += " and employeeid in (select employeeid from employee_costcodes where employee_costcodes.costcodeid = @costcodeid)";
                expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
            }
            if (departmentid != 0)
            {
                strsql += " and employeeid in (select employeeid from employee_costcodes where employee_costcodes.departmentid = @departmentid)";
                expdata.sqlexecute.Parameters.AddWithValue("@departmentid", departmentid);
            }

            switch (filter)
            {
                case 2:
                    strsql += " and employees.archived = @archived";
                    expdata.sqlexecute.Parameters.AddWithValue("@archived", (byte)1);
                    break;
                case 3:
                    strsql += " and employees.archived = @archived";
                    expdata.sqlexecute.Parameters.AddWithValue("@archived", (byte)0);
                    break;
            }

            if (accesstype == 2)
            {
                strsql = strsql + " and " + strrole;
            }
            strsql = strsql + " ORDER BY employees.username";


            rcdstemployees = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return rcdstemployees;       
        }

		public System.Data.DataSet getGrid(string surname, int accountid, int roleid, int groupid, int costcodeid, int departmentid, byte filter)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet rcdstemployees = new System.Data.DataSet();

            surname += "%";
            strsql = "SELECT employees.employeeid, employees.username, employees.archived, employees.surname + ',' + employees.title + ' ' + employees.firstname as Name, groups.groupname, roles.rolename, dbo.getEmpDepartmentSplit(employees.employeeid) as department, dbo.getEmpCostcodeSplit(employees.employeeid) as costcode " +
                "FROM employees " +
                "left join groups on groups.groupid = employees.groupid " +
                "inner join roles on roles.roleid = employees.roleid " +


                "where employees.username not like 'admin%' ";
            if (surname != "%")
            {
                strsql += " and employees.surname LIKE @surname";
                expdata.sqlexecute.Parameters.AddWithValue("@surname", surname);
            }
            if (roleid != 0)
            {
                strsql += " and employees.roleid = @roleid";
                expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
            }
            if (groupid != 0)
            {
                strsql += " and employees.groupid = @groupid";
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            }
            if (costcodeid != 0)
            {
                strsql += " and employeeid in (select employeeid from employee_costcodes where employee_costcodes.costcodeid = @costcodeid)";
                expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
            }
            if (departmentid != 0)
            {
                strsql += " and employeeid in (select employeeid from employee_costcodes where employee_costcodes.departmentid = @departmentid)";
                expdata.sqlexecute.Parameters.AddWithValue("@departmentid", departmentid);
            }

            switch (filter)
            {
                case 2:
                    strsql += " and employees.archived = @archived";
                    expdata.sqlexecute.Parameters.AddWithValue("@archived", (byte)1);
                    break;
                case 3:
                    strsql += " and employees.archived = @archived";
                    expdata.sqlexecute.Parameters.AddWithValue("@archived", (byte)0);
                    break;
            }

            if (accesstype == 2)
            {
                strsql = strsql + " and " + strrole;
            }
            strsql = strsql + " ORDER BY employees.username";


            rcdstemployees = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return rcdstemployees;
		}

		public void changeStatus(int employeeid, bool archive)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());

			if (archive == true)
			{
                strsql = "update employees set archived = 1, modifiedon = @modifiedon where employeeid = @employeeid";
			}
			else
			{
                strsql = "update employees set archived = 0, modifiedon = @modifiedon where employeeid = @employeeid";
			}
			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}
		public int getCount(int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			int count = 0;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

			strrole = (string)appinfo.Application["strrole" + accountid];
			strsql = "select count(*) from employees";
            
			if (accesstype == 2)
			{
				strsql = strsql + " and " + strrole;
			}

			count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();
			return count;

		}


        public int Authenticate(string username, string password)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int employeeID = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@username", username);

            strsql = "SELECT employeeid, password FROM employees WHERE username=@username AND ";

            if (password == null || password == "")
            {
                strsql += " password IS NULL";
            }
            else
            {
                cSecureData secureData = new cSecureData();
                expdata.sqlexecute.Parameters.AddWithValue("@encryptedPassword", secureData.Encrypt(password));
                expdata.sqlexecute.Parameters.AddWithValue("@hashedPassword", System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString()));

                strsql += " (password=@hashedPassword OR password=@encryptedPassword)";
            }

            System.Data.SqlClient.SqlDataReader reader;
            reader = expdata.GetReader(strsql);

            string passwordInDB = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                        employeeID = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                        {
                            passwordInDB = reader.GetString(1);
                        }
                }
            }

            reader.Close();

            if (password != null && password != "")
            {
                if (passwordInDB.Length == 32 && passwordInDB.ToUpper() == passwordInDB)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);

                    strsql = "UPDATE employees SET password=@encryptedPassword WHERE employeeid=@employeeid";

                    expdata.ExecuteSQL(strsql);
                }
            }

            expdata.sqlexecute.Parameters.Clear();

            return employeeID;
        }


        public void archiveEmployee(string username, int accountid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "UPDATE employees SET archived = 1, modifiedon = @modifiedon WHERE username = @username";
            expdata.sqlexecute.Parameters.Add("@username", username);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        
		public int Logon(string username, string password, int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int employeeid;
			int empcount = 0;
			string hashpwd = "";
			if (password != null)
			{
				hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password,"md5");
			}
			if (hashpwd == "")
			{
				strsql = "select count(*) as empcount from employees where  archived = 0 and active = 1 and username = @username and password is null";
				
				expdata.sqlexecute.Parameters.AddWithValue("@username",username);
			}
			else
			{
				strsql = "select count(*) as empcount from employees where  archived = 0 and active = 1 and username = @username and password = @password";
				
				expdata.sqlexecute.Parameters.AddWithValue("@username",username);
				expdata.sqlexecute.Parameters.AddWithValue("@password",hashpwd);
			}
			empcount = expdata.getcount(strsql);
			if (empcount == 0)
			{
				expdata.sqlexecute.Parameters.Clear();
				return 0;
			}

			if (hashpwd == "")
			{
				strsql = "select employeeid from employees where active = 1 and username = @username and password is null";
			}
			else
			{
				strsql = "select employeeid from employees where active = 1 and username = @username and password = @password";
			}
			employeeid = expdata.getcount(strsql);
			
			expdata.sqlexecute.Parameters.Clear();
			//InitialiseEmployee(employeeid);

			

			return employeeid;
		}

        public static int AdminLogon(string username, string password)
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            string strsql;
            int employeeid;
            int empcount = 0;
            string hashpwd = "";
            if (password != null)
            {
                hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
            }
            if (hashpwd == "")
            {
                strsql = "select count(*) as empcount from administrators where username = @username and password is null";
                
                expdata.sqlexecute.Parameters.AddWithValue("@username", username);
            }
            else
            {
                strsql = "select count(*) as empcount from administrators where username = @username and password = @password";
                
                expdata.sqlexecute.Parameters.AddWithValue("@username", username);
                expdata.sqlexecute.Parameters.AddWithValue("@password", hashpwd);
            }
            empcount = expdata.getcount(strsql);
            if (empcount == 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return 0;
            }

            if (hashpwd == "")
            {
                strsql = "select administratorid from administrators where username = @username and password is null";
            }
            else
            {
                strsql = "select administratorid from administrators where username = @username and password = @password";
            }
            employeeid = expdata.getcount(strsql);

            expdata.sqlexecute.Parameters.Clear();
            //InitialiseEmployee(employeeid);



            return employeeid / -1;
        }

		

		public System.Data.DataSet getDirectoryGrid(int accountid, string letter)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet ds = new System.Data.DataSet();
			if (letter != "X")
			{
				strsql = "select employeeid, username, title, firstname, surname, extension from employees where archived = 0 and surname like @surname order by surname, firstname";
			}
			else
			{
				strsql = "select employeeid, username, title, firstname, surname, extension from employees where archived = 0 and (surname like @surname or surname like @surname1 or surname like @surname2) order by surname, firstname";
				expdata.sqlexecute.Parameters.AddWithValue("@surname1","y%");
				expdata.sqlexecute.Parameters.AddWithValue("@surname2","z%");
			}
			
			expdata.sqlexecute.Parameters.AddWithValue("@surname",letter + "%");
			ds = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();

			return ds;
		}
		
		public int getEmployeeidByUsername(int accountid, string username)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int employeeid = 0;
			System.Data.SqlClient.SqlDataReader reader;
			expdata.sqlexecute.Parameters.AddWithValue("@username",username);
			
			strsql = "select employeeid from employees where username = @username";
			reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				if (reader.IsDBNull(0) == false)
				{
					employeeid = reader.GetInt32(0);
				}
			}
			reader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			
			return employeeid;
		}

        
		public bool alreadyExists(string username, int action, int employeeid, int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int count = 0;
			if (action == 0)
			{
				strsql = "select count(*) from employees where username = @username";
			}
			else
			{
				strsql = "select count(*) from employees where username = @username and employeeid <> @employeeid";
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			}
			expdata.sqlexecute.Parameters.AddWithValue("@username",username);
			

			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();
			if (count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}

		}
        public int addEmployee(int accountid, string username, string title, string firstname, string surname, string address1, string address2, string city, string county, string postcode, string telno, string email, string creditor, string payroll, string position, int groupid, int roleid, string accountname, string accountnumber, string accounttype, string sortcode, string reference, long mileagetotal, Dictionary<int, object> userdefined, string extension, string faxno, string homeemail, string pagerno, string mobileno, int linemanager, int advancegroup, int mileage, int mileageprev, cDepCostItem[] depcostbreakdown, int primarycurrency, int primarycountry, bool verified, bool active, DateTime licenceexpiry, DateTime licencelastchecked, int licencecheckedby, string licencenumber, int groupidcc, int groupidpc, List<int> itemroles, string ninumber, string middlenames, string maidenname, string gender, DateTime dateofbirth, DateTime hiredate, DateTime terminationdate, string country, int homelocationid, int officelocationid, string applicantnumber, bool applicantactivestatusflag)
		{
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            int currentUserEmployeeID = 0;

            //if (appinfo.User.Identity.Name != null && appinfo.User.Identity.Name != "")
            //{
            //    CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            //}
            //else
            //{

            //}

			int employeeid = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			if (alreadyExists(username,0,0,accountid) == true)
			{
				return 1;
			}
			
			
			expdata.sqlexecute.Parameters.AddWithValue("@username",username);
			expdata.sqlexecute.Parameters.AddWithValue("@title",title);
			
			expdata.sqlexecute.Parameters.AddWithValue("@firstname",firstname);
			expdata.sqlexecute.Parameters.AddWithValue("@surname",surname);
			expdata.sqlexecute.Parameters.AddWithValue("@address1",address1);
			expdata.sqlexecute.Parameters.AddWithValue("@address2",address2);
			expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			expdata.sqlexecute.Parameters.AddWithValue("@postcode",postcode);
			expdata.sqlexecute.Parameters.AddWithValue("@telno",telno);
			expdata.sqlexecute.Parameters.AddWithValue("@email",email);
			expdata.sqlexecute.Parameters.AddWithValue("@creditor",creditor);
			expdata.sqlexecute.Parameters.AddWithValue("@payroll",payroll);
			expdata.sqlexecute.Parameters.AddWithValue("@position",position);
			
			if (groupid == 0)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@groupid",DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@groupid",groupid);
			}
			
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			
			expdata.sqlexecute.Parameters.AddWithValue("@mileagetotal",mileagetotal);
			expdata.sqlexecute.Parameters.AddWithValue("@homeemail",homeemail);
			expdata.sqlexecute.Parameters.AddWithValue("@faxno",faxno);
			expdata.sqlexecute.Parameters.AddWithValue("@pagerno",pagerno);
			expdata.sqlexecute.Parameters.AddWithValue("@mobileno",mobileno);
			expdata.sqlexecute.Parameters.AddWithValue("@extension",extension);
			if (linemanager == 0)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@linemanager",DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@linemanager",linemanager);
			}
			expdata.sqlexecute.Parameters.AddWithValue("@advancegroup",advancegroup);
			expdata.sqlexecute.Parameters.AddWithValue("@mileage",mileage);
			expdata.sqlexecute.Parameters.AddWithValue("@mileageprev",mileageprev);
            

            if (primarycurrency == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", primarycurrency);
            }
            if (primarycountry == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", primarycountry);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@verified", Convert.ToByte(verified));
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(active));
            if (licencecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", licencecheckedby);
            }
            if (licencelastchecked == new DateTime(1900,01,01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", licencecheckedby);
            }
            if (licenceexpiry == new DateTime(1900,01,01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", licenceexpiry);
            }
            if (licencenumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", licencenumber);
            }
            if (groupidcc == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", groupidcc);
            }
            if (groupidpc == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", groupidpc);
            }
            if (ninumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", ninumber);
            }
            if (middlenames == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", middlenames);
            }
            if (maidenname == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", maidenname);
            }
            if (gender == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", gender);
            }
            if (dateofbirth == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", dateofbirth);
            }
            if (hiredate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", hiredate);
            }
            if (terminationdate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", terminationdate);
            }
            if (country == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", country);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@homelocationid", homelocationid);
            expdata.sqlexecute.Parameters.AddWithValue("@officelocationid", officelocationid);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", currentUserEmployeeID);
            if (applicantnumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@applicantnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@applicantnumber", applicantnumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@applicantactivestatusflag", Convert.ToByte(applicantactivestatusflag));

            strsql = "insert into  employees (username, title, firstname, surname, address1, address2, city, county, postcode, telno, email, creditor, payroll, position, groupid, roleid, extension, pagerno, faxno, mobileno, homeemail, linemanager, advancegroupid, mileage, mileageprev, primarycountry, primarycurrency, verified, active, licencelastchecked, licenceexpiry, licencecheckedby, licencenumber, groupidcc, groupidpc, ninumber, middlenames, maidenname, gender, dateofbirth, hiredate, terminationdate, country, homelocationid, officelocationid, createdon, createdby, applicantnumber, applicantactivestatusflag) " +
                "values (@username,@title,@firstname,@surname,@address1,@address2,@city,@county,@postcode,@telno,@email,@creditor,@payroll,@position,@groupid,@roleid,@extension, @pagerno, @faxno, @mobileno, @homeemail, @linemanager, @advancegroup, @mileage, @mileageprev, @primarycountry, @primarycurrency, @verified, @active, @licencelastchecked, @licenceexpiry, @licencecheckedby, @licencenumber, @groupidcc, @groupidpc, @ninumber, @middlenames, @maidenname, @gender, @dateofbirth, @hiredate, @terminationdate, @country, @homelocationid, @officelocationid, @createdon, @createdby, @applicantnumber, @applicantactivestatusflag);select @identity = @@identity";

            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
			
			
			
			expdata.ExecuteSQL(strsql);
            employeeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            

            addItemRoles(employeeid, itemroles);
			addBankDetails(employeeid,accountname,accountnumber,accounttype,sortcode,reference);
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Employee, employeeid, userdefined);
			//createDefaultViews(employeeid);
			//createHomePage(employeeid);
			cEmployee reqemp = GetEmployeeById(employeeid);
			reqemp.breakdown = depcostbreakdown;
            InsertCostCodeBreakdown(reqemp.employeeid, 0);

            //if (mileagetotal != 0)
            //{
            //    strsql = "insert into employee_mileagetotals (employeeid, financial_year, mileagetotal) " +
            //        "values (@employeeid, @financialyear, @mileagetotal)";
            //    expdata.sqlexecute.Parameters.AddWithValue("@financialyear",DateTime.Today.Year);
            //    expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
            //    expdata.sqlexecute.Parameters.AddWithValue("@mileagetotal",mileagetotal);
            //    expdata.ExecuteSQL(strsql);
            //}
			expdata.sqlexecute.Parameters.Clear();

			cEmails clsemails = new cEmails(accountid);
			int[] empid = new int[1];
			empid[0] = employeeid;
            if (currentUserEmployeeID > 0)
            {
                clsemails.sendMessage(1, currentUserEmployeeID, empid);

                cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
                clsaudit.addRecord("Employees", username);
            }
			return employeeid;
		}

        private void addItemRoles(int employeeid, List<int> itemroles)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int order = 1;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteProc("deleteEmployeeRoles");
            expdata.sqlexecute.Parameters.Clear();
            foreach (int itemroleid in itemroles)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
                expdata.sqlexecute.Parameters.AddWithValue("@order", order);
                expdata.ExecuteProc("addEmployeeRole");
                expdata.sqlexecute.Parameters.Clear();
                order++;
            }
        }
		

        public void verifyAccount(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update employees set verified = 1 where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }
        public void activateAccount(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update employees set active = 1 where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            sendActivatedEmail(employeeid);

            
        }

        private void sendActivatedEmail(int employeeid)
        {
           

            cEmployee reqemp = GetEmployeeById(employeeid);

            cAccounts clsaccounts = new cAccounts();
            cMisc clsmisc = new cMisc(reqemp.accountid);
            cAccount account = clsaccounts.getAccountById(reqemp.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);

            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            
            cEmployee administrator = GetEmployeeById(user.employeeid);
            string subject = "Your account has been activated";
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            output.Append("<html>\n");
            output.Append("<head>\n");
            output.Append("<style>\n");
            output.Append("*\n");
            output.Append("{\n");
            output.Append("font: 12px Arial, Helvetica, sans-serif;\n");
            output.Append("}\n");
            output.Append(".labeltd\n");
            output.Append("{\n");
            output.Append("padding: 2px 3px 2px 7px;\n");
            output.Append("width: 130px;\n");
            output.Append("background-color: #cae8f4;\n");
            output.Append("border: 1px solid #fff;\n");
            output.Append("color: #000;\n");
            output.Append("}\n");
            output.Append(".inputtd\n");
            output.Append("{\n");

            output.Append("padding: 1px 3px 1px 7px;\n");

            output.Append("background-color: #ffffff;\n");
            output.Append("}\n");
            output.Append(".inputpaneltitle\n");
            output.Append("{\n");
            output.Append("border: 1px solid white;\n");
            output.Append("font-size: 1.2em;\n");
            output.Append("font-weight: bold;\n");
            output.Append("background-color: #6280a7;\n");
            output.Append("color: white;\n");
            output.Append("padding: 1px 2px 2px 5px;\n");
            output.Append("}\n");
            output.Append("</style>\n");
            output.Append("</head>\n");
            output.Append("<body>\n");
            output.Append("Dear " + reqemp.firstname + "<br><br>\n");
            output.Append("Welcome to expenses!<bR><br>\n");

            output.Append("Your account has been activated and you are now able to start submitting your expense claims using the following details:<br><Br>\n");

            output.Append("Company ID: " + account.companyid + "<br>\n");
            output.Append("Username: " + reqemp.username + "<br>\n");
            output.Append("Password: The password you entered when you registered<br><Br>\n");

            output.Append("To log on visit: https://www.sel-expenses.com/<br><br>\n");

            output.Append("Please ensure that your username and password remain secure at all times.<br><Br>\n");

            output.Append("The expenses helpdesk is available to you from 9am until 5pm Monday to Friday by calling 01522 881300 or by emailing support@software-europe.co.uk<br><br>\n");

            output.Append("</body>");
            output.Append("</html>");


            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(administrator.email, reqemp.email, subject, output.ToString());
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "127.0.0.1";

            msg.IsBodyHtml = true;
            client.Send(msg);
        }
		private void createDefaultViews (int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet rcdstfields = new System.Data.DataSet();
			System.Data.DataRow[] rows;
			int order = 1;
			int fieldid;
			strsql = "select fieldid, [field] from fields where [field] = 'date' or [field] = 'subcat' or [field] = 'company' or [field] = 'normalreceipt' or [field] = 'net' or [field] = 'vat' or [field] = 'total'";
			rcdstfields = expdata.GetDataSet(strsql);

			//date
			rows = rcdstfields.Tables[0].Select("[field] = 'date'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//subcat
			rows = rcdstfields.Tables[0].Select("[field] = 'subcat'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//company
			rows = rcdstfields.Tables[0].Select("[field] = 'company'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//receipt
			rows = rcdstfields.Tables[0].Select("[field] = 'normalreceipt'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//net
			rows = rcdstfields.Tables[0].Select("[field] = 'net'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//vat
			rows = rcdstfields.Tables[0].Select("[field] = 'vat'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;
			//total
			rows = rcdstfields.Tables[0].Select("[field] = 'total'");
			fieldid = (int)rows[0]["fieldid"];
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (1," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (2," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (3," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			strsql = "insert into views(viewid, employeeid, fieldid, [order]) values (4," + employeeid + "," + fieldid + "," + order + ")";
			expdata.ExecuteSQL(strsql);
			order++;

		}

		
		public bool checkExpiry(int employeeid)
		{
			int expiry = 0;
			cEmployee reqemployee;
			DateTime lastdate;
			reqemployee = GetEmployeeById(employeeid);
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            cMisc clsmisc = new cMisc(reqemployee.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemployee.accountid);

            expiry = clsproperties.expiry;
			if (expiry == 0)
			{
				return false;
			}

			
			lastdate = reqemployee.lastchange;

			lastdate = lastdate.AddDays(expiry);
			if (lastdate < DateTime.Today)
			{
				return true;
			}




			return false;


		}
		private void insertPrevious(string password, int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			int previous = 0;
            cEmployee reqemp = GetEmployeeById(employeeid);
            cMisc clsmisc = new cMisc(reqemp.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
			string hashpwd = "";
		
			if (clsproperties.previous == 0)
			{
				return;
			}

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
            previous = clsproperties.previous;
			strsql = "update previouspasswords set [order] = [order] + 1 where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);

			strsql = "delete from previouspasswords where employeeid = @employeeid and [order] = " + (previous+1);
			expdata.ExecuteSQL(strsql);

			

			hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password,"md5");
			expdata.sqlexecute.Parameters.AddWithValue("@password",hashpwd);
			strsql = "insert into previouspasswords (employeeid, password, [order]) values (@employeeid,@password,1)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

		}

		public void updateMyDetails(int employeeid, string title, string firstname, string surname, string address1, string address2, string city, string county, string postcode, string telno, string email, string homeemail, string pagerno, string extension, string mobileno)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			cEmployee oldemp = GetEmployeeById(employeeid);

			string username = oldemp.username;
			expdata.sqlexecute.Parameters.AddWithValue("@title",title);
			
			expdata.sqlexecute.Parameters.AddWithValue("@firstname",firstname);
			expdata.sqlexecute.Parameters.AddWithValue("@surname",surname);
			expdata.sqlexecute.Parameters.AddWithValue("@address1",address1);
			expdata.sqlexecute.Parameters.AddWithValue("@address2",address2);
			expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			expdata.sqlexecute.Parameters.AddWithValue("@postcode",postcode);
			expdata.sqlexecute.Parameters.AddWithValue("@telno",telno);
			expdata.sqlexecute.Parameters.AddWithValue("@email",email);
			expdata.sqlexecute.Parameters.AddWithValue("@homeemail",homeemail);
			expdata.sqlexecute.Parameters.AddWithValue("@extension",extension);
			expdata.sqlexecute.Parameters.AddWithValue("@mobileno",mobileno);
			expdata.sqlexecute.Parameters.AddWithValue("@pagerno",pagerno);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			strsql = "update employees set title = @title, firstname = @firstname, surname = @surname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, telno = @telno, email = @email, homeemail = @homeemail, pagerno = @pagerno, extension = @extension, mobileno = @mobileno where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			
			
			#region auditlog
			cAuditLog clsaudit = new cAuditLog();
			cGroups clsgroups = new cGroups(accountid);

			if (oldemp.address1 != address1)
			{
				clsaudit.editRecord(username, "Address Line 1","Employees",oldemp.address1,address1);
			}
			if (oldemp.address2 != address2)
			{
				clsaudit.editRecord(username, "Address Line 2","Employees",oldemp.address2,address2);
			}
			if (oldemp.city != city)
			{
				clsaudit.editRecord(username,"City","Employees",oldemp.city,city);
			}
			if (oldemp.county != county)
			{
				clsaudit.editRecord(username, "County","Employees",oldemp.county,county);
			}
			if (oldemp.email != email)
			{
				clsaudit.editRecord(username, "E-mail Address","Employees",oldemp.email,email);
			}
			if (oldemp.extension != extension)
			{
				clsaudit.editRecord(username,"Extension","Employees",oldemp.extension,extension);
			}
			if (oldemp.firstname != firstname)
			{
				clsaudit.editRecord(username,"First Name","Employees",oldemp.firstname,firstname);
			}
			if (oldemp.homeemail != homeemail)
			{
				clsaudit.editRecord(username,"Home E-mail Address","Employees",oldemp.homeemail,homeemail);
			}
			if (oldemp.mobileno != mobileno)
			{
				clsaudit.editRecord(username,"Mobile No","Employees",oldemp.mobileno,mobileno);
			}
			if (oldemp.pagerno != pagerno)
			{
				clsaudit.editRecord(username,"Pager No","Employees",oldemp.pagerno,pagerno);
			}
			if (oldemp.postcode != postcode)
			{
				clsaudit.editRecord(username,"Postcode","Employees",oldemp.postcode,postcode);
			}
			
			if (oldemp.surname != surname)
			{
				clsaudit.editRecord(username,"Surname","Employees",oldemp.surname,surname);
			}
			if (oldemp.telno != telno)
			{
				clsaudit.editRecord(username,"Tel No","Employees",oldemp.telno,telno);
			}
			if (oldemp.title != title)
			{
				clsaudit.editRecord(username,"Title","Employees",oldemp.title,title);
			}
			
			#endregion

            
		}

        public byte changePassword(int accountID, int employeeID, string currentPassword, string newPassword, bool checkOldPassword, byte checkNewPassword)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            cSecureData secureData = new cSecureData();

            string OldPasswordHashed = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(currentPassword, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
            string OldPasswordEncrypted = secureData.Encrypt(currentPassword);
            string NewPasswordEncryped = secureData.Encrypt(newPassword);

            cEmployees clsEmployees = new cEmployees(accountID);
            cEmployee reqEmployee = clsEmployees.GetEmployeeById(employeeID);

            if(checkOldPassword == true) {
                if (reqEmployee.password != OldPasswordEncrypted && reqEmployee.password != OldPasswordHashed && reqEmployee.password != currentPassword)
                {
                    return 1;
                }
            }

            if(checkNewPassword != 0) {
                return 2;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@newPassword", NewPasswordEncryped);
            expdata.sqlexecute.Parameters.AddWithValue("@dateChanged", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());

            strsql = "UPDATE employees SET password = @newPassword, lastchange = @dateChanged, modifiedon = @modifiedon WHERE employeeid=@employeeID";
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();

            addPreviousPassword(OldPasswordHashed, employeeID);

            cAuditLog clsaudit = new cAuditLog(accountID, employeeID);
            clsaudit.addRecord("Employees", "Password Changed");

            return 0;
        }

        public void uploadCarDocument(int employeeid, int carid, CarDocumentType documentType, string filename)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "insert into car_documents (employeeid, carid, documenttype, filename) " +
                "values (@employeeid, @carid, @documenttype, @filename)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            if (carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@documenttype", (byte)documentType);
            expdata.sqlexecute.Parameters.AddWithValue("@filename", filename);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();


        }

        public void deleteCarDocument(int employeeid, int carid, CarDocumentType documentType)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "delete from car_documents where employeeid = @employeeid and documenttype = @documenttype";
            if (carid != 0)
            {
                strsql += " and carid = @carid";
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            
            expdata.sqlexecute.Parameters.AddWithValue("@documenttype", (byte)documentType);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
		private void addPreviousPassword(string password, int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			//System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			System.Data.SqlClient.SqlDataReader reader;
			cEmployee reqemp = GetEmployeeById(employeeid);
			int previous = 0;
			int order = 0;
            cMisc clsmisc = new cMisc(reqemp.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
			if (clsproperties.previous != 0)
			{
                previous = clsproperties.previous;
			}

			if (previous == 0)
			{
				return;
			}

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);

			strsql = "select max([order]) from previouspasswords where employeeid = @employeeid";
			reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				if (reader.IsDBNull(0) == false)
				{
					order = reader.GetInt32(0);
				}
			}
			reader.Close();

			order++;

			//insert the password
			strsql = "insert into previouspasswords (employeeid, password, [order]) " +
				"values (@employeeid, @password, @order)";
			expdata.sqlexecute.Parameters.AddWithValue("@password",password);
			expdata.sqlexecute.Parameters.AddWithValue("@order",order);
			expdata.ExecuteSQL(strsql);

			if (order > previous) //we are storing to many passwords to delete oldest one
			{
				strsql = "delete from previouspasswords where employeeid = @employeeid and [order] = 1";
				expdata.ExecuteSQL(strsql);
				//renumber
				strsql = "update previouspasswords set [order] = [order] - 1 where employeeid = @employeeid";
				expdata.ExecuteSQL(strsql);
			}

			expdata.sqlexecute.Parameters.Clear();
		}
		public byte checkpassword (string password, int accountid, int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			//return codes
			//0 = password fine
			//1 = password is incorrect length
			//2 = capital letter
			//3 = number
			//4 = previous

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            PasswordLength plength = PasswordLength.AnyLength;
			int length1 = 0;
			int length2 = 0;
            string hashpwd = "";

			char[] nums = "0123456789".ToCharArray();

			bool isPrevious = false;
            plength = clsproperties.plength;
			if (plength != PasswordLength.AnyLength)
			{
                length1 = clsproperties.length1;
                length2 = clsproperties.length2;
				switch (plength)
				{
					case PasswordLength.EqualTo:
						if (password.Length != length1)
						{
							return 1;
						}
						break;
					case PasswordLength.GreaterThan:
						if (password.Length <= length1)
						{
							return 1;
						}
						break;
					case PasswordLength.LessThan:
						if (password.Length >= length1)
						{
							return 1;
						}
						break;
					case PasswordLength.Between:
						if (password.Length < length1 || password.Length > length2)
						{
							return 1;
						}
						break;

				}
			}

			if (clsproperties.pupper == true)
			{
				if (password.ToLower() == password)
				{
					return 2;
				}
			}

			if (clsproperties.pnumbers == true)
			{
				if (password.IndexOfAny(nums,0,password.Length) == -1)
				{
					return 3;
				}
			}
			if (clsproperties.previous != 0)
			{
				System.Data.SqlClient.SqlDataReader prevreader;
                if (employeeid != 0)
                {
                    cSecureData clsSecure = new cSecureData();
                    string encryptedPassword;
                    string prevPassword;
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                    strsql = "select password from previouspasswords where employeeid = @employeeid";
                    prevreader = expdata.GetReader(strsql);
                    while (prevreader.Read())
                    {
                        hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
                        prevPassword = prevreader.GetString(prevreader.GetOrdinal("password"));
                        if (prevPassword == hashpwd)
                        {
                            isPrevious = true;
                            break;
                        }
                        else
                        {
                            encryptedPassword = clsSecure.Encrypt(password);

                            if (prevPassword == encryptedPassword)
                            {
                                isPrevious = true;
                                break;
                            }

                        }
                    }
                    prevreader.Close();

                    strsql = "select password from employees where employeeid = @employeeid";
                    prevreader = expdata.GetReader(strsql);
                    while (prevreader.Read())
                    {
                        hashpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");

                        if (prevreader.IsDBNull(prevreader.GetOrdinal("password")) == false)
                        {
                            prevPassword = prevreader.GetString(prevreader.GetOrdinal("password"));
                            if (prevPassword == hashpwd)
                            {
                                isPrevious = true;
                                break;
                            }
                            else
                            {
                                encryptedPassword = clsSecure.Encrypt(password);

                                if (prevPassword == encryptedPassword)
                                {
                                    isPrevious = true;
                                    break;
                                }

                            }
                        }
                    }
                    prevreader.Close();

                    if (isPrevious == true)
                    {
                        return 4;
                    }
                }
			}

			expdata.sqlexecute.Parameters.Clear();
			return 0;
		}

        private void deleteItemFromAddItems(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "delete from additems where employeeid = @employeeid and subcatid not in (select subcatid from rolesubcats where roleid in (select itemroleid from employee_roles where employeeid = @employeeid))";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public int updateEmployee(int accountid, int employeeid, string username, string title, string firstname, string surname, string address1, string address2, string city, string county, string postcode, string telno, string email, string creditor, string payroll, string position, int groupid, int roleid, string accountname, string accountnumber, string accounttype, string sortcode, string reference, long mileagetotal, Dictionary<int, object> userdefined, string extension, string faxno, string homeemail, string pagerno, string mobileno, int linemanager, int advancegroup, int mileage, int mileageprev, cDepCostItem[] depcostbreakdown, int primarycurrency, int primarycountry, DateTime licenceexpiry, DateTime licencelastchecked, int licencecheckedby, string licencenumber, int groupidcc, int groupidpc, List<int> itemroles, string ninumber, string middlenames, string maidenname, string gender, DateTime dateofbirth, DateTime hiredate, DateTime terminationdate, string country, int homelocationid, int officelocationid, string applicantnumber, bool applicantactivestatusflag)
		{
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            //CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cEmployee oldemp = GetEmployeeById(employeeid);
			
            Spend_Management.cAccessRoles clsAccessRoles = new Spend_Management.cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
			if (alreadyExists(username,2,employeeid, accountid) == true)
			{
				return 1;
			}

			if (oldemp.groupid != groupid) //see if employee has any claims
			{
				cClaims clsclaims = new cClaims(accountid);
				if (clsclaims.getCount(employeeid, ClaimStage.Submitted) > 0)
				{
					return 1;
				}
			}


            #region Check if the employee had access to check and pay and now doesnt if the true, abort the update

            bool stopUpdateDueToRoleChange = false;
            bool oldRolesAllowedCheckAndPay = false;

            #region check if previous roles allowed access to check and pay
            foreach (cAccessRole oldAccessRole in oldemp.AccessRoles)
            {
                if (oldAccessRole.ElementAccess[SpendManagementElement.CheckAndPay].CanView == true)
                {
                    oldRolesAllowedCheckAndPay = true;
                    break;
                }
            }
            #endregion check if previous roles allowed access to check and pay

            #region if oldRolesAllowedCheckAndPay equals true, check if new ones do, if they do not and  the employee is part of a team abort the update
            if (oldRolesAllowedCheckAndPay == true)
            {
                bool newRolesAllowCheckAndPay = false;

                // check new roles and see if the allow access
                cAccessRole tmpAccessRole;
                foreach (int newAccessRoleID in lstRoles)
                {
                    tmpAccessRole = clsAccessRoles.GetAccessRoleByID(newAccessRoleID);
                    if (tmpAccessRole.ElementAccess[SpendManagementElement.CheckAndPay].CanView == true)
                    {
                        newRolesAllowCheckAndPay = true;
                        break;
                    }
                }

                if (newRolesAllowCheckAndPay == false)
                {
                    strsql = "select count(*) from teamemps where employeeid = @employeeid";
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                    int count = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    if (count != 0)
                    {
                        return 2;
                    }
                }
            }
            #endregion

            #endregion Check if the employee had access to check and pay and now doesnt if the true, abort the update
			if (oldemp.roleid != roleid) //see if this employees belongs to any teams. May not be able to switch role
			{
				
				cRole reqrole = clsroles.getRoleById(oldemp.roleid);
				cRole newrole = clsroles.getRoleById(roleid);
				if (reqrole != null && (reqrole.checkandpay == true &&  newrole.checkandpay == false))
				{
					//does emp belong to any teams
					strsql = "select count(*) from teamemps where employeeid = @employeeid";
					expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
					int count = expdata.getcount(strsql);
					expdata.sqlexecute.Parameters.Clear();
					if (count != 0)
					{
						return 2;
					}
				}
			}
			
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@title",title);
			
			expdata.sqlexecute.Parameters.AddWithValue("@firstname",firstname);
			expdata.sqlexecute.Parameters.AddWithValue("@surname",surname);
			expdata.sqlexecute.Parameters.AddWithValue("@address1",address1);
			expdata.sqlexecute.Parameters.AddWithValue("@address2",address2);
			expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			expdata.sqlexecute.Parameters.AddWithValue("@postcode",postcode);
			expdata.sqlexecute.Parameters.AddWithValue("@telno",telno);
			expdata.sqlexecute.Parameters.AddWithValue("@email",email);
			expdata.sqlexecute.Parameters.AddWithValue("@creditor",creditor);
			expdata.sqlexecute.Parameters.AddWithValue("@payroll",payroll);
			expdata.sqlexecute.Parameters.AddWithValue("@position",position);
			
			if (groupid == 0)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@groupid",DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@groupid",groupid);
			}
			
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			
			
			if (linemanager == 0)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@linemanager",DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@linemanager",linemanager);
			}
			expdata.sqlexecute.Parameters.AddWithValue("@mileagetotal",mileagetotal);
			expdata.sqlexecute.Parameters.AddWithValue("@homeemail",homeemail);
			expdata.sqlexecute.Parameters.AddWithValue("@faxno",faxno);
			expdata.sqlexecute.Parameters.AddWithValue("@pagerno",pagerno);
			expdata.sqlexecute.Parameters.AddWithValue("@mobileno",mobileno);
			expdata.sqlexecute.Parameters.AddWithValue("@extension",extension);
			expdata.sqlexecute.Parameters.AddWithValue("@advancegroup",advancegroup);
			expdata.sqlexecute.Parameters.AddWithValue("@mileage",mileage);
			expdata.sqlexecute.Parameters.AddWithValue("@mileageprev",mileageprev);
           
			
            if (primarycurrency == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", primarycurrency);
            }
            if (primarycountry == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", primarycountry);
            }
            if (licencecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", licencecheckedby);
            }
            if (licencelastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", licencelastchecked);
            }
            if (licenceexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", licenceexpiry);
            }
            if (licencenumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", licencenumber);
            }
            if (groupidcc == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", groupidcc);
            }
            if (groupidpc == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", groupidpc);
            }
            if (ninumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", ninumber);
            }
            
            
            if (middlenames == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", middlenames);
            }
            if (maidenname == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", maidenname);
            }
            if (gender == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", gender);
            }
            if (dateofbirth == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", dateofbirth);
            }
            if (hiredate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", hiredate);
            }
            if (terminationdate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", terminationdate);
            }
            if (country == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", country);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@username", username);
            if (homelocationid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@homelocationid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@homelocationid", homelocationid);
            }
            if (officelocationid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@officelocationid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@officelocationid", officelocationid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", employeeid);
            if (applicantnumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@applicantnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@applicantnumber", applicantnumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@applicantactivestatusflag", Convert.ToByte(applicantactivestatusflag));
            strsql = "update employees set title = @title, firstname = @firstname, surname = @surname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, telno = @telno, email = @email, creditor = @creditor, payroll = @payroll, position = @position, groupid = @groupid, roleid = @roleid, mileagetotal = @mileagetotal, homeemail = @homeemail, pagerno = @pagerno, mobileno = @mobileno, faxno = @faxno, extension = @extension, linemanager = @linemanager, advancegroupid = @advancegroup, mileage = @mileage, mileageprev = @mileageprev, primarycountry = @primarycountry, primarycurrency = @primarycurrency, licenceexpiry = @licenceexpiry, licencelastchecked = @licencelastchecked, licencecheckedby = @licencecheckedby, licencenumber = @licencenumber, groupidcc = @groupidcc, groupidpc = @groupidpc, ninumber = @ninumber, middlenames = @middlenames, maidenname = @maidenname, gender = @gender, dateofbirth = @dateofbirth, hiredate = @hiredate, terminationdate = @terminationdate, country = @country, homelocationid = @homelocationid, officelocationid = @officelocationid, username = @username, modifiedon = @modifiedon, modifiedby = @modifiedby, applicantnumber = @applicantnumber, applicantactivestatusflag = @applicantactivestatusflag where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
            //if (getMileageTotal(oldemp.employeeid,DateTime.Today) == 0 && mileagetotal != 0)
            //{
            //    strsql = "insert into employee_mileagetotals (employeeid, financial_year, mileagetotal) " +
            //        "values (@employeeid, @financialyear, @mileagetotal)";
            //    expdata.sqlexecute.Parameters.AddWithValue("@financialyear",DateTime.Today.Year);
            ////	expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
            ////	expdata.sqlexecute.Parameters.AddWithValue("@mileagetotal",mileagetotal);
            //    expdata.ExecuteSQL(strsql);
            //}
			expdata.sqlexecute.Parameters.Clear();
			
			#region auditlog
			cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
			cGroups clsgroups = new cGroups(accountid);
			
			string oldval, newval;
			if (oldemp.address1 != address1)
			{
				clsaudit.editRecord(username, "Address Line 1","Employees",oldemp.address1,address1);
			}
			if (oldemp.address2 != address2)
			{
				clsaudit.editRecord(username, "Address Line 2","Employees",oldemp.address2,address2);
			}
			if (oldemp.advancegroup != advancegroup)
			{
				
				if (oldemp.advancegroup == 0)
				{
					oldval = "";
				}
				else
				{
					oldval = clsgroups.GetGroupById(oldemp.advancegroup).groupname;
				}
				if (advancegroup == 0)
				{
					newval = "";
				}
				else
				{
					newval = clsgroups.GetGroupById(advancegroup).groupname;
				}
				clsaudit.editRecord(username,"Advance Group","Employees",oldval,newval);
			}
            //if (oldemp.cardnum != cardnum)
            //{
            //    clsaudit.editRecord(username,"Card Number","Employees",oldemp.cardnum,cardnum);
            //}
			if (oldemp.city != city)
			{
				clsaudit.editRecord(username,"City","Employees",oldemp.city,city);
			}
			if (oldemp.county != county)
			{
				clsaudit.editRecord(username, "County","Employees",oldemp.county,county);
			}
            //if (oldemp.creditcard != creditcard)
            //{
            //    clsaudit.editRecord(username,"Has Credit Card","Employees",oldemp.creditcard.ToString(),creditcard.ToString());
            //}
			if (oldemp.creditor != creditor)
			{
				clsaudit.editRecord(username, "Credit Account","Employees",oldemp.creditor,creditor);
			}
			if (oldemp.email != email)
			{
				clsaudit.editRecord(username, "E-mail Address","Employees",oldemp.email,email);
			}
			if (oldemp.extension != extension)
			{
				clsaudit.editRecord(username,"Extension","Employees",oldemp.extension,extension);
			}
			if (oldemp.fax != faxno)
			{
				clsaudit.editRecord(username,"Fax No","Employees",oldemp.fax,faxno);
			}
			if (oldemp.firstname != firstname)
			{
				clsaudit.editRecord(username,"First Name","Employees",oldemp.firstname,firstname);
			}
			if (oldemp.groupid != groupid)
			{
				if (oldemp.groupid == 0)
				{
					oldval = "";
				}
				else
				{
					oldval = clsgroups.GetGroupById(oldemp.groupid).groupname;
				}
				if (groupid == 0)
				{
					newval = "";
				}
				else
				{
					newval = clsgroups.GetGroupById(groupid).groupname;
				}
				clsaudit.editRecord(username,"Signoff Group","Employees",oldval,newval);
			}
			if (oldemp.homeemail != homeemail)
			{
				clsaudit.editRecord(username,"Home E-mail Address","Employees",oldemp.homeemail,homeemail);
			}
			if (oldemp.linemanager != linemanager)
			{
				if (oldemp.linemanager == 0)
				{
					oldval = "";
				}
				else
				{
					oldval = GetEmployeeById(oldemp.linemanager).username;
				}
				if (linemanager == 0)
				{
					newval = "";
				}
				else
				{
					newval = GetEmployeeById(linemanager).username;
				}
				clsaudit.editRecord(username,"Line Manger","Employees",oldval,newval);
			}
			if (oldemp.mileage != mileage)
			{
				clsaudit.editRecord(username,"Mileage","Employees",oldemp.mileage.ToString(),mileage.ToString());
			}
			if (oldemp.mileageprev != mileageprev)
			{
				clsaudit.editRecord(username,"Mileage Previous","Employees",oldemp.mileageprev.ToString(),mileageprev.ToString());
			}
			if (oldemp.mobileno != mobileno)
			{
				clsaudit.editRecord(username,"Mobile No","Employees",oldemp.mobileno,mobileno);
			}
			if (oldemp.pagerno != pagerno)
			{
				clsaudit.editRecord(username,"Pager No","Employees",oldemp.pagerno,pagerno);
			}
			if (oldemp.payroll != payroll)
			{
				clsaudit.editRecord(username,"Payroll No","Employees",oldemp.payroll,payroll);
			}
			if (oldemp.position != position)
			{
				clsaudit.editRecord(username,"Position","Employees",oldemp.position,position);
			}
			if (oldemp.postcode != postcode)
			{
				clsaudit.editRecord(username,"Postcode","Employees",oldemp.postcode,postcode);
			}
			if (oldemp.roleid != roleid)
			{
				if (oldemp.roleid == 0)
				{
					oldval = "";
				}
				else
				{
					oldval = clsroles.getRoleById(oldemp.roleid).rolename;
				}
				if (roleid == 0)
				{
					newval = "";
				}
				else
				{
					newval = clsroles.getRoleById(roleid).rolename;
				}
				clsaudit.editRecord(username,"Role","Employees",oldval,newval);
			}
			if (oldemp.surname != surname)
			{
				clsaudit.editRecord(username,"Surname","Employees",oldemp.surname,surname);
			}
			if (oldemp.telno != telno)
			{
				clsaudit.editRecord(username,"Tel No","Employees",oldemp.telno,telno);
			}
			if (oldemp.title != title)
			{
				clsaudit.editRecord(username,"Title","Employees",oldemp.title,title);
			}
			
			#endregion

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Employee, employeeid, userdefined);

			updateBankDetails(employeeid,accountname,accountnumber,accounttype,sortcode,reference);

            
			cEmployee reqemp = GetEmployeeById(employeeid);
			reqemp.breakdown = depcostbreakdown;
			InsertCostCodeBreakdown(reqemp.employeeid,2);
            addItemRoles(employeeid, itemroles);
            deleteItemFromAddItems(employeeid);
            
			return 0;
		}

        public void updateEmployeeLineManager(int employeeid, int linemanager)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            if (linemanager == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@linemanagerid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@linemanagerid", linemanager);
            }

            strsql = "UPDATE employees SET linemanager = @linemanagerid WHERE employeeid = @employeeid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void updateEmployeeSignoffs(int employeeid, int signoffID, int ccSignoffID, int pcSignoffID, int advSignoffID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            if (signoffID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", signoffID);
            }

            if (ccSignoffID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", ccSignoffID);
            }

            if (pcSignoffID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", pcSignoffID);
            }

            if (advSignoffID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@advancegroupid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@advancegroupid", advSignoffID);
            }

            strsql = "UPDATE employees SET groupid = @groupid, groupidcc = @groupidcc, groupidpc = @groupidpc, advancegroupid = @advancegroupid WHERE employeeid = @employeeid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

		private int addBankDetails(int employeeid, string accountname, string accountnumber, string accounttype, string sortcode, string reference)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@accountname",accountname);
			expdata.sqlexecute.Parameters.AddWithValue("@accountnumber",accountnumber);
			expdata.sqlexecute.Parameters.AddWithValue("@accounttype",accounttype);
			expdata.sqlexecute.Parameters.AddWithValue("@sortcode",sortcode);
			expdata.sqlexecute.Parameters.AddWithValue("@reference",reference);
			strsql = "insert into [employees_bankdetails] (employeeid, name, accountnumber, accounttype, sortcode, reference) " +
				"values (@employeeid,@accountname,@accountnumber,@accounttype,@sortcode,@reference)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return 0;
		}

		
		public int deleteEmployee(int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int mainadmin;
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			System.Data.SqlClient.SqlDataReader reader;

            cEmployee reqemp = GetEmployeeById(employeeid);
            cMisc clsmisc = new cMisc(reqemp.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);

            mainadmin = clsproperties.mainadministrator;
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			
			//see if the user is assigned to any groups
			int count;
			strsql = "select count(*) from signoffs where (signofftype = 2 and relid = @employeeid) or (holidaytype = 2 and relid = @employeeid)";
			count = expdata.getcount(strsql);
			if (count != 0)
			{
				expdata.sqlexecute.Parameters.Clear();
				return 1;
			}
			strsql = "select floats.floatid,[float], (select sum(amount) from float_allocations where floatid = floats.floatid) as floatused from  [floats] where employeeid = @employeeid and paid = 1";
			reader = expdata.GetReader(strsql);
			bool hasAdvance = false;
			while (reader.Read())
			{
				if (reader.IsDBNull(reader.GetOrdinal("floatused")) == true) //hasn't used advance
				{
					hasAdvance = true;
					break;
				}
				else
				{
					if (reader.GetDecimal(reader.GetOrdinal("float")) - reader.GetDecimal(reader.GetOrdinal("floatused")) > 0) //still advance left
					{
						hasAdvance = true;
						break;
					}
				}
			}
			reader.Close();
			if (hasAdvance == true)
			{
				expdata.sqlexecute.Parameters.Clear();
				//see if there is any cash left on them
				
				return 2;
			}
			else
			{
				//delete advances
				strsql = "delete from float_allocations where floatid in (select floatid from [floats] where employeeid = @employeeid)";
				expdata.ExecuteSQL(strsql);
				strsql = "delete from [floats] where employeeid = @employeeid";
				expdata.ExecuteSQL(strsql);
			}
			//reports
			strsql = "update reports set employeeid = @mainadmin where employeeid = @employeeid";
			expdata.sqlexecute.Parameters.AddWithValue("@mainadmin",mainadmin);
			expdata.ExecuteSQL(strsql);

			
			//savedexpenses_current
			strsql = "delete from savedexpenses_current where claimid in (select claimid from claims where employeeid = @employeeid)";
			expdata.ExecuteSQL(strsql);
            //savedexpenses_current
            strsql = "delete from savedexpenses_previous where claimid in (select claimid from claims where employeeid = @employeeid)";
            expdata.ExecuteSQL(strsql);
			//claims
			strsql = "delete from claims_base where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
			//bank details
			strsql = "delete from [employees_bankdetails] where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
			//employee
			strsql = "delete from employees where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			cAuditLog clsaudit = new cAuditLog();
			clsaudit.deleteRecord("Employees",GetEmployeeById(employeeid).username);

			return 0;
		}

		private int updateBankDetails(int employeeid, string accountname, string accountnumber, string accounttype, string sortcode, string reference)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			

			int count = 0;
			//does record exists
			strsql = "select count(*) from [employees_bankdetails] where employeeid = @employeeid";
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();
			if (count == 0)
			{
				addBankDetails(employeeid,accountname,accountnumber,accounttype,sortcode,reference);
				return 0;
			}
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@accountname",accountname);
			expdata.sqlexecute.Parameters.AddWithValue("@accountnumber",accountnumber);
			expdata.sqlexecute.Parameters.AddWithValue("@accounttype",accounttype);
			expdata.sqlexecute.Parameters.AddWithValue("@sortcode",sortcode);
			expdata.sqlexecute.Parameters.AddWithValue("@reference",reference);
			strsql = "update [employees_bankdetails] set name = @accountname, accountnumber = @accountnumber, accounttype = @accounttype, sortcode = @sortcode, reference = @reference where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			#region auditlog
			cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
			cEmployee oldemp = GetEmployeeById(employeeid);
			if (oldemp.bankdetails.name != accountname)
			{
				clsaudit.editRecord(oldemp.username,"Account Name","Employees",oldemp.bankdetails.name,accountname);
			}
			if (oldemp.bankdetails.accountnumber != accountnumber)
			{
				clsaudit.editRecord(oldemp.username,"Account Number","Employees",oldemp.bankdetails.accountnumber,accountnumber);
			}
			if (oldemp.bankdetails.accounttype != accounttype)
			{
				clsaudit.editRecord(oldemp.username,"Account Type","Employees",oldemp.bankdetails.accounttype,accounttype);
			}
			if (oldemp.bankdetails.sortcode != sortcode)
			{
				clsaudit.editRecord(oldemp.username,"Sortcode","Employees",oldemp.bankdetails.sortcode,sortcode);
			}
			if (oldemp.bankdetails.reference != reference)
			{
				clsaudit.editRecord(oldemp.username,"Reference","Employees",oldemp.bankdetails.reference,reference);
			}
			#endregion
			return 0;
		}
		
		public Infragistics.WebUI.UltraWebGrid.ValueList CreateVList(int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			Infragistics.WebUI.UltraWebGrid.ValueList list = new Infragistics.WebUI.UltraWebGrid.ValueList();
			System.Data.SqlClient.SqlDataReader empreader;
			string empname;
			int employeeid;

			
			strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees ";
			if (accesstype == 2)
			{
				strsql = strsql + " where " + strrole;
			}
			strsql = strsql + " order by empname";

			empreader = expdata.GetReader(strsql);
			while (empreader.Read())
			{
				empname = empreader.GetString(empreader.GetOrdinal("empname"));
				employeeid = empreader.GetInt32(empreader.GetOrdinal("employeeid"));
				list.ValueListItems.Add(employeeid,empname);
			}
			empreader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			return list;
		}

        public bool isCheckAndPayer(int accountid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int count = 0;
            strsql = "select count(*) from signoffs where groupid in (select groupid from groups) and ((signofftype = 2 and relid = @employeeid) or (holidaytype = 2 and holidayid = @employeeid))";
            
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
		public cColumnList CreateColumnList(int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cColumnList list = new cColumnList();
			System.Data.SqlClient.SqlDataReader empreader;
			string empname;
			int employeeid;

			
			strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees";
			if (accesstype == 2)
			{
				strsql = strsql + " and " + strrole;
			}
			strsql = strsql + " order by empname";

			empreader = expdata.GetReader(strsql);
			while (empreader.Read())
			{
				empname = empreader.GetString(empreader.GetOrdinal("empname"));
				employeeid = empreader.GetInt32(empreader.GetOrdinal("employeeid"));
				
				list.addItem(employeeid,empname);
			}
			empreader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			return list;
		}

        public ListItem[] getEmployeesByRole(string rolePermission, int accountid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            
            System.Data.SqlClient.SqlDataReader reader;
            List<ListItem> lstitems = new List<ListItem>();
            SortedList<string,int> tempitems = new SortedList<string,int>();
            
            strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees inner join roles on roles.roleid = employees.roleid where archived = 0 and username not like 'admin%' and  " + rolePermission;
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (!tempitems.ContainsKey(reader.GetString(reader.GetOrdinal("empname"))))
                {
                    tempitems.Add(reader.GetString(reader.GetOrdinal("empname")), reader.GetInt32(reader.GetOrdinal("employeeid")));
                }
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();
            

            foreach (KeyValuePair<string,int> i in tempitems)
            {
                lstitems.Add(new ListItem(i.Key,i.Value.ToString()));
            }
            return lstitems.ToArray();
        }

        public System.Web.UI.WebControls.ListItem[] CreateCheckPayDropDown(int employeeid, int accountid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            cEmployee reqemp = GetEmployeeById(user.employeeid);
            cRoles clsroles = new cRoles(accountid);
            cRole reqrole = clsroles.getRoleById(reqemp.roleid);

            int itemcount = 0;
            int i = 0;
            System.Data.SqlClient.SqlDataReader empreader;

            
            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
            strsql = "select count(*) from employees where archived = 0 and roleid in (select roleid from roles where checkandpay = 1)";
            
            itemcount = expdata.getcount(strsql);
            System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[itemcount];


            strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees where archived = 0 and roleid in (select roleid from roles where checkandpay = 1)";
            
            strsql = strsql + " order by empname";

            empreader = expdata.GetReader(strsql);
            while (empreader.Read())
            {
                tempitems[i] = new System.Web.UI.WebControls.ListItem();
                tempitems[i].Text = empreader.GetString(empreader.GetOrdinal("empname"));
                tempitems[i].Value = empreader.GetInt32(empreader.GetOrdinal("employeeid")).ToString();
                if (empreader.GetInt32(empreader.GetOrdinal("employeeid")) == employeeid)
                {
                    tempitems[i].Selected = true;
                }
                i++;
            }
            empreader.Close();
            
            expdata.sqlexecute.Parameters.Clear();
            return tempitems;
        }

        public System.Web.UI.WebControls.ListItem[] CreateDropDown(int employeeid, int accountid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            CurrentUser currentUser = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);
            List<ListItem> items = new List<ListItem>();

            //cRoles clsroles = new cRoles(accountid);
            //cRole reqrole = clsroles.getRoleById(reqemp.roleid);

            System.Data.SqlClient.SqlDataReader empreader;


            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);



            strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees where archived = 0 and username not like 'admin%' ";

            if (currentUser.Employee.AccessRoles != null)
            {
                if (currentUser.HighestAccessLevel != AccessRoleLevel.EmployeesResponsibleFor)
                {
                    strsql = strsql + " and " + currentUser.GetAccessRoleWhereClause();
                }
            }
            strsql += " or employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";
            strsql = strsql + " order by empname";

            empreader = expdata.GetReader(strsql);
            while (empreader.Read())
            {
                items.Add(new ListItem(empreader.GetString(1), empreader.GetInt32(0).ToString()));
            }
            empreader.Close();

            expdata.sqlexecute.Parameters.Clear();
            return items.ToArray();

        }

		private cEmployee getEmployeeFromDB(int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			string username, password;
			int additems;
			string title, firstname, surname, address1, address2, city, county, postcode;
            bool archived;

            cNewUserDefined clsuserdefined = null;
            Dictionary<int, object> lstuserdefined;
			string creditor;
			int curclaimno, currefnum;
			int groupid;
			string email, hint;
			DateTime lastchange;
			int mileagetotal;
			string payroll, position;
			int roleid;
			string telno;
			int linemanager;
			int advancegroup;
			int mileage, mileageprev;
            int groupidpc, groupidcc;
			string faxno, homeemail, pagerno, mobileno, extension, country;
            string middlenames, maidenname, gender;
            DateTime dateofbirth, hiredate, terminationdate;
            DateTime createdOn, modifiedOn;
            int createdBy, modifiedBy;
			bool userole, customiseditems;
            int primarycurrency, primarycountry;
            string applicantnumber;
            bool applicantactivestatusflag;
            bool expensesConnectUser;
            
			System.Data.SqlClient.SqlDataReader employeereader;
            bool active, verified;

            DateTime licenceexpiry, licencelastchecked;
            string licencenumber, ninumber;
            
            int licencecheckedby;
            int homelocationid, officelocationid;
			cEmployee tempemployee = new cEmployee();
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            SortedList<string, object> parameters = new SortedList<string, object>();
            AggregateCacheDependency aggdep = new AggregateCacheDependency();

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);

            DBConnection itemdata = new DBConnection(cAccounts.getConnectionString(accountid));
            parameters.Add("@employeeid", employeeid);
            strsql = "select employeeid, subcatid from dbo.additems where employeeid = @employeeid";
            itemdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency itemdep = itemdata.CreateSQLCacheDependency(strsql, parameters);

            DBConnection docdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "select employeeid, carid, filename from dbo.car_documents where employeeid = @employeeid";
            SqlCacheDependency docdep = docdata.CreateSQLCacheDependency(strsql, parameters);

            DBConnection cardata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT     carid, employeeid, startdate, enddate, make, model, registration, mileageid, cartypeid, active, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motlastchecked, motcheckedby, motexpiry, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, enginesize FROM dbo.cars where employeeid = @employeeid";
            cardata.sqlexecute.CommandText = strsql;
            SqlCacheDependency cardep = cardata.CreateSQLCacheDependency(strsql, parameters);

            DBConnection poolcardata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT     poolcaruserid, carid, employeeid, CreatedOn, CreatedBy FROM dbo.pool_car_users";
            poolcardata.sqlexecute.CommandText = strsql;
            SqlCacheDependency poolcardep = poolcardata.CreateSQLCacheDependency(strsql, parameters);

            DBConnection carddata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT     corporatecardid, employeeid, cardnumber, cardproviderid, active FROM dbo.employee_corporate_cards where employeeid = @employeeid";
            carddata.sqlexecute.CommandText = strsql;
            SqlCacheDependency carddep = carddata.CreateSQLCacheDependency(strsql, parameters);

            DBConnection esrdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT AssignmentNumber FROM dbo.esr_assignments WHERE employeeid = @employeeid;";
            esrdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency esrdep = expdata.CreateSQLCacheDependency(strsql, parameters);

            strsql = "select employeeid, username, password, title, firstname, surname, mileagetotal, email, currefnum, curclaimno, speedo, address1, address2, city, county, postcode, payroll, position, telno, creditor, archived, groupid, roleid, hint, lastchange, additems, cardnum, userole, costcodeid, departmentid, extension, pagerno, mobileno, faxno, homeemail, linemanager, advancegroupid, mileage, mileageprev, customiseditems, active, primarycountry, primarycurrency, verified, licenceexpiry, licencelastchecked, licencecheckedby, licencenumber, groupidcc, groupidpc, CreatedOn, CreatedBy, ModifiedOn, modifiedby, ninumber,maidenname, middlenames, gender, dateofbirth, hiredate, terminationdate, country, homelocationid, officelocationid, applicantnumber, applicantactivestatusflag, expensesConnectUser  from dbo.employees where employeeid = @employeeid";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency empdep = expdata.CreateSQLCacheDependency(strsql, parameters);

            

            aggdep.Add(new CacheDependency[] { itemdep, cardep, empdep, carddep, poolcardep, docdep, esrdep });
           

			employeereader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (employeereader.Read())
			{
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
				username = employeereader.GetString(employeereader.GetOrdinal("username"));
				if (employeereader.IsDBNull(employeereader.GetOrdinal("password")) == false)
				{
					password = employeereader.GetString(employeereader.GetOrdinal("password"));
				}
				else
				{
					password = "";
				}
				additems = employeereader.GetInt32(employeereader.GetOrdinal("additems"));
				if (employeereader.IsDBNull(employeereader.GetOrdinal("title")) == false)
				{
					title = employeereader.GetString(employeereader.GetOrdinal("title"));
				}
				else
				{
					title = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("firstname")) == false)
				{
					firstname = employeereader.GetString(employeereader.GetOrdinal("firstname"));
				}
				else
				{
					firstname = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("surname")) == false)
				{
					surname = employeereader.GetString(employeereader.GetOrdinal("surname"));
				}
				else
				{
					surname = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("address1")) == false)
				{
					address1 = employeereader.GetString(employeereader.GetOrdinal("address1"));
				}
				else
				{
					address1 = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("address2")) == false)
				{
					address2 = employeereader.GetString(employeereader.GetOrdinal("address2"));
				}
				else
				{
					address2 = "";
				}

				if (employeereader.IsDBNull(employeereader.GetOrdinal("city")) == false)
				{
					city = employeereader.GetString(employeereader.GetOrdinal("city"));
				}
				else
				{
					city = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("county")) == false)
				{
					county = employeereader.GetString(employeereader.GetOrdinal("county"));
				}
				else
				{
					county = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("postcode")) == false)
				{
					postcode = employeereader.GetString(employeereader.GetOrdinal("postcode"));
				}
				else
				{
					postcode = "";
				}
				archived = employeereader.GetBoolean(employeereader.GetOrdinal("archived"));
				
				
				
				if (employeereader.IsDBNull(employeereader.GetOrdinal("creditor")) == false)
				{
					creditor = employeereader.GetString(employeereader.GetOrdinal("creditor"));
				}
				else
				{
					creditor = "";
				}
				curclaimno = employeereader.GetInt32(employeereader.GetOrdinal("curclaimno"));
				currefnum = employeereader.GetInt32(employeereader.GetOrdinal("currefnum"));
				
				if (employeereader.IsDBNull(employeereader.GetOrdinal("email")) == false)
				{
					email = employeereader.GetString(employeereader.GetOrdinal("email"));
				}
				else
				{
					email = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("groupid")) == false)
				{
					groupid = employeereader.GetInt32(employeereader.GetOrdinal("groupid"));
				}
				else
				{
					groupid = 0;
				}
				
				if (employeereader.IsDBNull(employeereader.GetOrdinal("lastchange")) == false)
				{
					lastchange = employeereader.GetDateTime(employeereader.GetOrdinal("lastchange"));
				}
				else
				{
					lastchange = DateTime.Today;
				}
				
				if (employeereader.IsDBNull(employeereader.GetOrdinal("mileagetotal")) == false)
				{
					mileagetotal = employeereader.GetInt32(employeereader.GetOrdinal("mileagetotal"));
				}
				else
				{
					mileagetotal = 0;
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("payroll")) == false)
				{
					payroll = employeereader.GetString(employeereader.GetOrdinal("payroll"));
				}
				else
				{
					payroll = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("position")) == false)
				{
					position = employeereader.GetString(employeereader.GetOrdinal("position"));
				}
				else
				{
					position = "";
				}
				roleid = employeereader.GetInt32(employeereader.GetOrdinal("roleid"));
				
				if (employeereader.IsDBNull(employeereader.GetOrdinal("telno")) == false)
				{
					telno = employeereader.GetString(employeereader.GetOrdinal("telno"));
				}
				else
				{
					telno = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("extension")) == false)
				{
					extension = employeereader.GetString(employeereader.GetOrdinal("extension"));
				}
				else
				{
					extension = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("homeemail")) == false)
				{
					homeemail = employeereader.GetString(employeereader.GetOrdinal("homeemail"));
				}
				else
				{
					homeemail = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("pagerno")) == false)
				{
					pagerno = employeereader.GetString(employeereader.GetOrdinal("pagerno"));
				}
				else
				{
					pagerno = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("mobileno")) == false)
				{
					mobileno = employeereader.GetString(employeereader.GetOrdinal("mobileno"));
				}
				else
				{
					mobileno = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("faxno")) == false)
				{
					faxno = employeereader.GetString(employeereader.GetOrdinal("faxno"));
				}
				else
				{
					faxno = "";
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("linemanager")) == false)
				{
					linemanager = employeereader.GetInt32(employeereader.GetOrdinal("linemanager"));
				}
				else
				{
					linemanager = 0;
				}
				if (employeereader.IsDBNull(employeereader.GetOrdinal("advancegroupid")) == false)
				{
					advancegroup = employeereader.GetInt32(employeereader.GetOrdinal("advancegroupid"));
				}
				else
				{
					advancegroup = 0;
				}
				userole = employeereader.GetBoolean(employeereader.GetOrdinal("userole"));
				mileage = employeereader.GetInt32(employeereader.GetOrdinal("mileage"));
				mileageprev = employeereader.GetInt32(employeereader.GetOrdinal("mileageprev"));
				customiseditems = employeereader.GetBoolean(employeereader.GetOrdinal("customiseditems"));
                if (employeereader.IsDBNull(employeereader.GetOrdinal("primarycountry")) == true)
                {
                    primarycountry = 0;
                }
                else
                {
                    primarycountry = employeereader.GetInt32(employeereader.GetOrdinal("primarycountry"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("primarycurrency")) == true)
                {
                    primarycurrency = 0;
                }
                else
                {
                    primarycurrency = employeereader.GetInt32(employeereader.GetOrdinal("primarycurrency"));
                }
                
                active = employeereader.GetBoolean(employeereader.GetOrdinal("active"));
                verified = employeereader.GetBoolean(employeereader.GetOrdinal("verified"));
                if (employeereader.IsDBNull(employeereader.GetOrdinal("licenceexpiry")) == true)
                {
                    licenceexpiry = new DateTime(1900, 01, 01);
                }
                else
                {
                    licenceexpiry = employeereader.GetDateTime(employeereader.GetOrdinal("licenceexpiry"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("licencelastchecked")) == true)
                {
                    licencelastchecked = new DateTime(1900, 01, 01);
                }
                else
                {
                    licencelastchecked = employeereader.GetDateTime(employeereader.GetOrdinal("licencelastchecked"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("licencecheckedby")) == true)
                {
                    licencecheckedby = 0;
                }
                else
                {
                    licencecheckedby = employeereader.GetInt32(employeereader.GetOrdinal("licencecheckedby"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("licencenumber")) == true)
                {
                    licencenumber = "";
                }
                else
                {
                    licencenumber = employeereader.GetString(employeereader.GetOrdinal("licencenumber"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("groupidcc")) == true)
                {
                    groupidcc = 0;
                }
                else
                {
                    groupidcc = employeereader.GetInt32(employeereader.GetOrdinal("groupidcc"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("groupidpc")) == true)
                {
                    groupidpc = 0;
                }
                else
                {
                    groupidpc = employeereader.GetInt32(employeereader.GetOrdinal("groupidpc"));
                }
                lstuserdefined = clsuserdefined.getValues(AppliesTo.Employee, employeeid);
                if (employeereader.IsDBNull(employeereader.GetOrdinal("ninumber")) == true)
                {
                    ninumber = "";
                }
                else
                {
                    ninumber = employeereader.GetString(employeereader.GetOrdinal("ninumber"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("middlenames")) == true)
                {
                    middlenames = "";
                }
                else
                {
                    middlenames = employeereader.GetString(employeereader.GetOrdinal("middlenames"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("maidenname")) == true)
                {
                    maidenname = "";
                }
                else
                {
                    maidenname = employeereader.GetString(employeereader.GetOrdinal("maidenname"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("gender")) == true)
                {
                    gender = "";
                }
                else
                {
                    gender = employeereader.GetString(employeereader.GetOrdinal("gender"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("dateofbirth")) == true)
                {
                    dateofbirth = new DateTime(1900, 01, 01);
                }
                else
                {
                    dateofbirth = employeereader.GetDateTime(employeereader.GetOrdinal("dateofbirth"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("hiredate")) == true)
                {
                    hiredate = new DateTime(1900, 01, 01);
                }
                else
                {
                    hiredate = employeereader.GetDateTime(employeereader.GetOrdinal("hiredate"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("terminationdate")) == true)
                {
                    terminationdate = new DateTime(1900, 01, 01);
                }
                else
                {
                    terminationdate = employeereader.GetDateTime(employeereader.GetOrdinal("terminationdate"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("country")) == true)
                {
                    country = "";
                }
                else
                {
                    country = employeereader.GetString(employeereader.GetOrdinal("country"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("homelocationid")) == true)
                {
                    homelocationid = 0;
                }
                else
                {
                    homelocationid = employeereader.GetInt32(employeereader.GetOrdinal("homelocationid"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("officelocationid")) == true)
                {
                    officelocationid = 0;
                }
                else
                {
                    officelocationid = employeereader.GetInt32(employeereader.GetOrdinal("officelocationid"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("createdon")) == true)
                {
                    createdOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdOn = employeereader.GetDateTime(employeereader.GetOrdinal("createdon"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("createdby")) == true)
                {
                    createdBy = 0;
                }
                else
                {
                    createdBy = employeereader.GetInt32(employeereader.GetOrdinal("createdby"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedOn = employeereader.GetDateTime(employeereader.GetOrdinal("modifiedon"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedBy = 0;
                }
                else
                {
                    modifiedBy = employeereader.GetInt32(employeereader.GetOrdinal("modifiedby"));
                }
                if (employeereader.IsDBNull(employeereader.GetOrdinal("applicantnumber")) == true)
                {
                    applicantnumber = "";
                }
                else
                {
                    applicantnumber = employeereader.GetString(employeereader.GetOrdinal("applicantnumber"));
                }
                applicantactivestatusflag = employeereader.GetBoolean(employeereader.GetOrdinal("applicantactivestatusflag"));
                expensesConnectUser = employeereader.GetBoolean(employeereader.GetOrdinal("expensesConnectUser"));
                tempemployee = new cEmployee(accountid, employeeid, username, password, title, firstname, surname, mileagetotal, email, currefnum, curclaimno, address1, address2, city, county, postcode, payroll, position, telno, creditor, archived, roleid, groupid, lastchange, userole, faxno, homeemail, extension, pagerno, mobileno, linemanager, advancegroup, mileage, mileageprev, customiseditems, primarycountry, primarycurrency, verified, active, licenceexpiry, licencelastchecked, licencecheckedby, licencenumber, groupidcc, groupidpc, lstuserdefined, ninumber, middlenames, maidenname, gender, dateofbirth, hiredate, terminationdate, country, getGridSorts(employeeid), getRoles(employeeid), getBankDetails(employeeid), getReadBroadcasts(employeeid), getCars(employeeid), getLicencePath(employeeid), getMultipleItems(employeeid, customiseditems), getCorporateCards(employeeid), getCostCodeBreakdown(employeeid),createdOn,createdBy, modifiedOn, modifiedBy,  homelocationid, officelocationid, applicantnumber, applicantactivestatusflag, getESRAssignmentNumbers(employeeid), expensesConnectUser);
			}
			employeereader.Close();
			
			if (tempemployee != null)
			{
                Cache.Insert("employee" + accountid + employeeid, tempemployee, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
				return tempemployee;
			}
			else
			{
				return null;
			}
			
		}
		public cEmployee GetEmployeeById(int employeeid)
		{
            cEmployee reqemp;
            if (employeeid < 0) //admin logon
            {
                reqemp = null;// new cEmployee(accountid, employeeid, "admin", "", "", "SEL", "Administrator", 0, "", 0, 0, "", "", "", "", "", "", "", "", "", false, -1, 0, new DateTime(1900, 01, 01), false, "", "", "", "", "", 0, 0, 0, 0, false, 0, 0, true, true, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, "", 0, 0, new Dictionary<int, object>());
            }
            else
            {
                

                reqemp = (cEmployee)Cache["employee" + accountid + employeeid];

                if (reqemp == null)
                {
                    reqemp = getEmployeeFromDB(employeeid);

                }
            }
			return reqemp;
        }

        #region get functions
        private SortedList<Grid, cGridSort> getGridSorts(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            string strsql;
            Grid grid;
            cGridSort clssort;
            string columnname;
            byte sortorder;

            SortedList<Grid, cGridSort> lst = new SortedList<Grid, cGridSort>();

            strsql = "select * from default_sorts where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                grid = (Grid)reader.GetInt32(reader.GetOrdinal("gridid"));
                columnname = reader.GetString(reader.GetOrdinal("columnname"));
                sortorder = reader.GetByte(reader.GetOrdinal("defaultorder"));
                clssort = new cGridSort(grid, columnname, sortorder);
                lst.Add(grid, clssort);
            }
            reader.Close();

            return lst;
        }
        private List<cItemRole> getRoles(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cItemRoles clsroles = new cItemRoles(accountid);
            cItemRole role;
            System.Data.SqlClient.SqlDataReader reader;
            string strsql;
            List<cItemRole> roles = new List<cItemRole>();
            strsql = "select item_roles.itemroleid from item_roles inner join employee_roles on employee_roles.itemroleid = item_roles.itemroleid where employee_roles.employeeid = @employeeid order by employee_roles.[order]";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                role = clsroles.getItemRoleById(reader.GetInt32(0));
                if (role != null)
                {
                    roles.Add(role);
                }
            }
            reader.Close();
            return roles;
        }

        public List<string> getESRAssignmentNumbers(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            List<string> lstAssignments = new List<string>();
            string assignmentnumber;

            string strsql = "SELECT AssignmentNumber FROM esr_assignments WHERE employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                assignmentnumber = reader.GetString(reader.GetOrdinal("AssignmentNumber"));
                lstAssignments.Add(assignmentnumber);
            }

            reader.Close();

            return lstAssignments;
        }

        public string saveESRAssignment(int employeeid, string assignmentNum, expenses.Action action, bool checkedit, string oldvalue)
        {
            int recordcount;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            strsql = "select count(*) from esr_assignments where AssignmentNumber = @AssignmentNumber and employeeid = @employeeid";
            
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);    
            expdata.sqlexecute.Parameters.AddWithValue("@AssignmentNumber", assignmentNum);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.UtcNow);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.UtcNow);
            recordcount = expdata.getcount(strsql);

            if (action == Action.Add)
            {
                if (recordcount > 0) //update existing record
                {
                    return "Cannot add this Assignment Number as it already exists.";
                }
                else
                {
                    strsql = "INSERT INTO esr_assignments (employeeid, AssignmentNumber, createdon) VALUES (@employeeid, @AssignmentNumber, @createdon)";
                }

                
            }
            else if (action == Action.Edit)
            {
                if (checkedit)
                {
                    if (recordcount > 0)
                    {
                        return "Cannot edit this Assignment Number as another instance already has this value.";
                    }
                }
                expdata.sqlexecute.Parameters.AddWithValue("@oldAssignmentNumber", oldvalue);
                strsql = "UPDATE esr_assignments SET AssignmentNumber = @AssignmentNumber, modifiedon = @modifiedon WHERE employeeid = @employeeid AND AssignmentNumber = @oldAssignmentNumber;";
            }
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return "";
        }

        public void deleteESRAssignment(int employeeid, string assignmentNum)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            strsql = "DELETE FROM esr_assignments WHERE AssignmentNumber = @AssignmentNumber and employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@AssignmentNumber", assignmentNum);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public System.Data.DataSet getEmployeeESRAssignmentsGrid(int employeeid)
        {
            object[] values;

            cEmployee reqemp = GetEmployeeById(employeeid);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("AssignmentNumber", System.Type.GetType("System.String"));
            

            foreach (string ass in reqemp.esrAssignNums)
            {
                values = new object[1];
                values[0] = ass;
                tbl.Rows.Add(values); 
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        private cEmpBankDetails getBankDetails(int employeeid)
        {
            cEmpBankDetails clsbank;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            string name = "";
            string accountnumber = "";
            string accounttype = "";
            string sortcode = "";
            string reference = "";
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select * from employees_bankdetails where employeeid = " + employeeid;
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(reader.GetOrdinal("name")) == false)
                {
                    name = reader.GetString(reader.GetOrdinal("name"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("accountnumber")) == false)
                {
                    accountnumber = reader.GetString(reader.GetOrdinal("accountnumber"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("accounttype")) == false)
                {
                    accounttype = reader.GetString(reader.GetOrdinal("accounttype"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("sortcode")) == false)
                {
                    sortcode = reader.GetString(reader.GetOrdinal("sortcode"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("reference")) == false)
                {
                    reference = reader.GetString(reader.GetOrdinal("reference"));
                }
            }
            reader.Close();
            

            clsbank = new cEmpBankDetails(name, accountnumber, accounttype, sortcode, reference);
            return clsbank;
        }
        private System.Collections.ArrayList getReadBroadcasts(int employeeid)
        {
            System.Collections.ArrayList temp = new System.Collections.ArrayList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select broadcastid from employee_readbroadcasts where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                temp.Add(reader.GetInt32(0));
            }
            reader.Close();
            

            return temp;
        }

        public cCar getCarByRegistration(string registration, int employeeID)
        {
            cEmployee employee = GetEmployeeById(employeeID);
            

            

            foreach (cCar car in employee.cars)
            {
                if (car.registration.ToLower().Replace(" ","") == registration.ToLower().Trim().Replace(" ",""))
                {
                    return car;
                }
            }

            
            return null;
        }


        public List<cCar> getCars(int employeeid)
        {
            int i = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;
            System.Data.SqlClient.SqlDataReader reader;
            Dictionary<int, object> userdefined;
            int carid;
            string make, model, registration;
            DateTime startdate, enddate;
            bool active;
            int endodometer;
            Int64 odometer;
            byte cartypeid;
            bool fuelcard;
            DateTime taxexpiry, taxlastchecked, motexpiry, motlastchecked, insuranceexpiry, insurancelastchecked, serviceexpiry, servicelastchecked;
            int taxcheckedby, motcheckedby, insurancecheckedby, servicecheckedby;
            string mottestnumber, insurancenumber;
            string insurancepath = "";
            string motpath = "";
            string servicepath = "";
            string taxpath = "";
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            int caremployeeid;
            cNewUserDefined clsuserdefined = null;
            List<cCar> cars = new List<cCar>();
            int engineSize = 0;
            bool approved;

                
                
            strsql = "select * from cars where employeeid = @employeeid or carid in (select carid from pool_car_users where employeeid = @employeeid)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            
            SortedList<CarDocumentType, string> doctypes;
             
            MileageUOM defaultuom;
            
            
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                doctypes = new SortedList<CarDocumentType, string>();
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
                if (reader.IsDBNull(reader.GetOrdinal("employeeid")) == true)
                {
                    caremployeeid = 0;
                }
                else
                {
                    caremployeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                }
                carid = reader.GetInt32(reader.GetOrdinal("carid"));

                make = reader.GetString(reader.GetOrdinal("make"));
                model = reader.GetString(reader.GetOrdinal("model"));
                registration = reader.GetString(reader.GetOrdinal("registration"));
                if (reader.IsDBNull(reader.GetOrdinal("startdate")) == false)
                {
                    startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                }
                else
                {
                    startdate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("enddate")) == false)
                {
                    enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                }
                else
                {
                    enddate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("cartypeid")) == false)
                {
                    cartypeid = reader.GetByte(reader.GetOrdinal("cartypeid"));
                }
                else
                {
                    cartypeid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("odometer")) == false)
                {
                    odometer = reader.GetInt64(reader.GetOrdinal("odometer"));
                }
                else
                {
                    odometer = 0;
                }
                active = reader.GetBoolean(reader.GetOrdinal("active"));
                fuelcard = reader.GetBoolean(reader.GetOrdinal("fuelcard"));
                if (reader.IsDBNull(reader.GetOrdinal("endodometer")) == false)
                {
                    endodometer = reader.GetInt32(reader.GetOrdinal("endodometer"));
                }
                else
                {
                    endodometer = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxexpiry")) == false)
                {
                    taxexpiry = reader.GetDateTime(reader.GetOrdinal("taxexpiry"));
                }
                else
                {
                    taxexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxlastchecked")) == false)
                {
                    taxlastchecked = reader.GetDateTime(reader.GetOrdinal("taxlastchecked"));
                }
                else
                {
                    taxlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motexpiry")) == false)
                {
                    motexpiry = reader.GetDateTime(reader.GetOrdinal("motexpiry"));
                }
                else
                {
                    motexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motlastchecked")) == false)
                {
                    motlastchecked = reader.GetDateTime(reader.GetOrdinal("motlastchecked"));
                }
                else
                {
                    motlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insuranceexpiry")) == false)
                {
                    insuranceexpiry = reader.GetDateTime(reader.GetOrdinal("insuranceexpiry"));
                }
                else
                {
                    insuranceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancelastchecked")) == false)
                {
                    insurancelastchecked = reader.GetDateTime(reader.GetOrdinal("insurancelastchecked"));
                }
                else
                {
                    insurancelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("serviceexpiry")) == false)
                {
                    serviceexpiry = reader.GetDateTime(reader.GetOrdinal("serviceexpiry"));
                }
                else
                {
                    serviceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicelastchecked")) == false)
                {
                    servicelastchecked = reader.GetDateTime(reader.GetOrdinal("servicelastchecked"));
                }
                else
                {
                    servicelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxcheckedby")) == false)
                {
                    taxcheckedby = reader.GetInt32(reader.GetOrdinal("taxcheckedby"));
                }
                else
                {
                    taxcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("motcheckedby")) == false)
                {
                    motcheckedby = reader.GetInt32(reader.GetOrdinal("motcheckedby"));
                }
                else
                {
                    motcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancecheckedby")) == false)
                {
                    insurancecheckedby = reader.GetInt32(reader.GetOrdinal("insurancecheckedby"));
                }
                else
                {
                    insurancecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicecheckedby")) == false)
                {
                    servicecheckedby = reader.GetInt32(reader.GetOrdinal("servicecheckedby"));
                }
                else
                {
                    servicecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("mottestnumber")) == false)
                {
                    mottestnumber = reader.GetString(reader.GetOrdinal("mottestnumber"));
                }
                else
                {
                    mottestnumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancenumber")) == false)
                {
                    insurancenumber = reader.GetString(reader.GetOrdinal("insurancenumber"));
                }
                else
                {
                    insurancenumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                doctypes = getDocumentPaths(carid);
                insurancepath = "";
                if (doctypes.ContainsKey(CarDocumentType.Insurance))
                {
                    doctypes.TryGetValue(CarDocumentType.Insurance, out insurancepath);
                }
                motpath = "";
                if (doctypes.ContainsKey(CarDocumentType.MOT))
                {
                    doctypes.TryGetValue(CarDocumentType.MOT, out motpath);
                }
                servicepath = "";
                if (doctypes.ContainsKey(CarDocumentType.Service))
                {
                    doctypes.TryGetValue(CarDocumentType.Service, out servicepath);
                }
                taxpath = "";
                if (doctypes.ContainsKey(CarDocumentType.Tax))
                {
                    doctypes.TryGetValue(CarDocumentType.Tax, out taxpath);
                }
                userdefined = clsuserdefined.getValues(AppliesTo.Car, carid);
                defaultuom = (MileageUOM)reader.GetByte(reader.GetOrdinal("default_unit"));

                if (!reader.IsDBNull(reader.GetOrdinal("enginesize")))
                {
                    engineSize = reader.GetInt32(reader.GetOrdinal("enginesize"));
                }

                if (!reader.IsDBNull(reader.GetOrdinal("approved")))
                {
                    approved = reader.GetBoolean(reader.GetOrdinal("approved"));
                }
                else
                {
                    approved = true;
                }

                cars.Add(new cCar(accountid, caremployeeid, carid, make, model, registration, startdate, enddate, active, getCarMileageCats(carid), cartypeid, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motexpiry, motlastchecked, motcheckedby, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, userdefined, getOdometerReadings(carid), insurancepath, motpath, servicepath, taxpath, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved));
            }
            reader.Close();
            

            return cars;

        }

        public List<cCar> getPoolCars()
        {
            int i = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;
            System.Data.SqlClient.SqlDataReader reader;
            Dictionary<int, object> userdefined;
            int carid;
            string make, model, registration;
            DateTime startdate, enddate;
            bool active;
            int endodometer;
            Int64 odometer;
            byte cartypeid;
            bool fuelcard;
            DateTime taxexpiry, taxlastchecked, motexpiry, motlastchecked, insuranceexpiry, insurancelastchecked, serviceexpiry, servicelastchecked;
            int taxcheckedby, motcheckedby, insurancecheckedby, servicecheckedby;
            string mottestnumber, insurancenumber;
            string insurancepath = "";
            string motpath = "";
            string servicepath = "";
            string taxpath = "";
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            cNewUserDefined clsuserdefined = null;
            List<cCar> cars = new List<cCar>();
            int engineSize = 0;
            bool approved;


            
            strsql = "select * from cars where employeeid is null";
            


            SortedList<CarDocumentType, string> doctypes;

            MileageUOM defaultuom;


            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
                carid = reader.GetInt32(reader.GetOrdinal("carid"));

                make = reader.GetString(reader.GetOrdinal("make"));
                model = reader.GetString(reader.GetOrdinal("model"));
                registration = reader.GetString(reader.GetOrdinal("registration"));
                if (reader.IsDBNull(reader.GetOrdinal("startdate")) == false)
                {
                    startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                }
                else
                {
                    startdate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("enddate")) == false)
                {
                    enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                }
                else
                {
                    enddate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("cartypeid")) == false)
                {
                    cartypeid = reader.GetByte(reader.GetOrdinal("cartypeid"));
                }
                else
                {
                    cartypeid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("odometer")) == false)
                {
                    odometer = reader.GetInt64(reader.GetOrdinal("odometer"));
                }
                else
                {
                    odometer = 0;
                }
                active = reader.GetBoolean(reader.GetOrdinal("active"));
                fuelcard = reader.GetBoolean(reader.GetOrdinal("fuelcard"));
                if (reader.IsDBNull(reader.GetOrdinal("endodometer")) == false)
                {
                    endodometer = reader.GetInt32(reader.GetOrdinal("endodometer"));
                }
                else
                {
                    endodometer = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxexpiry")) == false)
                {
                    taxexpiry = reader.GetDateTime(reader.GetOrdinal("taxexpiry"));
                }
                else
                {
                    taxexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxlastchecked")) == false)
                {
                    taxlastchecked = reader.GetDateTime(reader.GetOrdinal("taxlastchecked"));
                }
                else
                {
                    taxlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motexpiry")) == false)
                {
                    motexpiry = reader.GetDateTime(reader.GetOrdinal("motexpiry"));
                }
                else
                {
                    motexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motlastchecked")) == false)
                {
                    motlastchecked = reader.GetDateTime(reader.GetOrdinal("motlastchecked"));
                }
                else
                {
                    motlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insuranceexpiry")) == false)
                {
                    insuranceexpiry = reader.GetDateTime(reader.GetOrdinal("insuranceexpiry"));
                }
                else
                {
                    insuranceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancelastchecked")) == false)
                {
                    insurancelastchecked = reader.GetDateTime(reader.GetOrdinal("insurancelastchecked"));
                }
                else
                {
                    insurancelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("serviceexpiry")) == false)
                {
                    serviceexpiry = reader.GetDateTime(reader.GetOrdinal("serviceexpiry"));
                }
                else
                {
                    serviceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicelastchecked")) == false)
                {
                    servicelastchecked = reader.GetDateTime(reader.GetOrdinal("servicelastchecked"));
                }
                else
                {
                    servicelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxcheckedby")) == false)
                {
                    taxcheckedby = reader.GetInt32(reader.GetOrdinal("taxcheckedby"));
                }
                else
                {
                    taxcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("motcheckedby")) == false)
                {
                    motcheckedby = reader.GetInt32(reader.GetOrdinal("motcheckedby"));
                }
                else
                {
                    motcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancecheckedby")) == false)
                {
                    insurancecheckedby = reader.GetInt32(reader.GetOrdinal("insurancecheckedby"));
                }
                else
                {
                    insurancecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicecheckedby")) == false)
                {
                    servicecheckedby = reader.GetInt32(reader.GetOrdinal("servicecheckedby"));
                }
                else
                {
                    servicecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("mottestnumber")) == false)
                {
                    mottestnumber = reader.GetString(reader.GetOrdinal("mottestnumber"));
                }
                else
                {
                    mottestnumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancenumber")) == false)
                {
                    insurancenumber = reader.GetString(reader.GetOrdinal("insurancenumber"));
                }
                else
                {
                    insurancenumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                doctypes = getDocumentPaths(carid);
                if (doctypes.ContainsKey(CarDocumentType.Insurance))
                {
                    doctypes.TryGetValue(CarDocumentType.Insurance, out insurancepath);
                }
                if (doctypes.ContainsKey(CarDocumentType.MOT))
                {
                    doctypes.TryGetValue(CarDocumentType.MOT, out motpath);
                }
                if (doctypes.ContainsKey(CarDocumentType.Service))
                {
                    doctypes.TryGetValue(CarDocumentType.Service, out servicepath);
                }
                if (doctypes.ContainsKey(CarDocumentType.Tax))
                {
                    doctypes.TryGetValue(CarDocumentType.Tax, out taxpath);
                }
                userdefined = clsuserdefined.getValues(AppliesTo.Car, carid);
                defaultuom = (MileageUOM)reader.GetByte(reader.GetOrdinal("default_unit"));

                if (!reader.IsDBNull(reader.GetOrdinal("enginesize")))
                {
                    engineSize = reader.GetInt32(reader.GetOrdinal("enginesize"));
                }

                if (!reader.IsDBNull(reader.GetOrdinal("approved")))
                {
                    approved = reader.GetBoolean(reader.GetOrdinal("approved"));
                }
                else
                {
                    approved = true;
                }

                cars.Add(new cCar(accountid, 0, carid, make, model, registration, startdate, enddate, active, getCarMileageCats(carid), cartypeid, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motexpiry, motlastchecked, motcheckedby, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, userdefined, getOdometerReadings(carid), insurancepath, motpath, servicepath, taxpath, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved));
            }
            reader.Close();


            return cars;

        }
        public List<int> GetUnapprovedCars(int employeeID)
        {
            cEmployee employee = GetEmployeeById(employeeID);
            
            List<int> lstUnapprovedCars = new List<int>(); ;

            foreach (cCar car in employee.cars)
            {
                if (car.Approved == false)
                {
                    lstUnapprovedCars.Add(car.carid);
                }
            }

            return lstUnapprovedCars;   
        }

        public cCar getCarById(int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            Dictionary<int, object> userdefined;
            
            string make, model, registration;
            DateTime startdate, enddate;
            bool active;
            int endodometer;
            Int64 odometer;
            byte cartypeid;
            bool fuelcard;
            DateTime taxexpiry, taxlastchecked, motexpiry, motlastchecked, insuranceexpiry, insurancelastchecked, serviceexpiry, servicelastchecked;
            int taxcheckedby, motcheckedby, insurancecheckedby, servicecheckedby;
            string mottestnumber, insurancenumber;
            string insurancepath = "";
            string motpath = "";
            string servicepath = "";
            string taxpath = "";
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            cNewUserDefined clsuserdefined = null;
            cCar car = null;
            SortedList<CarDocumentType, string> doctypes;
            MileageUOM defaultuom;
            int enginesize = 0;
            bool approved;

            strsql = "select * from cars where carid = @carid;";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);

            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }

                make = reader.GetString(reader.GetOrdinal("make"));
                model = reader.GetString(reader.GetOrdinal("model"));
                registration = reader.GetString(reader.GetOrdinal("registration"));
                if (reader.IsDBNull(reader.GetOrdinal("startdate")) == false)
                {
                    startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                }
                else
                {
                    startdate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("enddate")) == false)
                {
                    enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                }
                else
                {
                    enddate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("cartypeid")) == false)
                {
                    cartypeid = reader.GetByte(reader.GetOrdinal("cartypeid"));
                }
                else
                {
                    cartypeid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("odometer")) == false)
                {
                    odometer = reader.GetInt64(reader.GetOrdinal("odometer"));
                }
                else
                {
                    odometer = 0;
                }
                active = reader.GetBoolean(reader.GetOrdinal("active"));
                fuelcard = reader.GetBoolean(reader.GetOrdinal("fuelcard"));
                if (reader.IsDBNull(reader.GetOrdinal("endodometer")) == false)
                {
                    endodometer = reader.GetInt32(reader.GetOrdinal("endodometer"));
                }
                else
                {
                    endodometer = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxexpiry")) == false)
                {
                    taxexpiry = reader.GetDateTime(reader.GetOrdinal("taxexpiry"));
                }
                else
                {
                    taxexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxlastchecked")) == false)
                {
                    taxlastchecked = reader.GetDateTime(reader.GetOrdinal("taxlastchecked"));
                }
                else
                {
                    taxlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motexpiry")) == false)
                {
                    motexpiry = reader.GetDateTime(reader.GetOrdinal("motexpiry"));
                }
                else
                {
                    motexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("motlastchecked")) == false)
                {
                    motlastchecked = reader.GetDateTime(reader.GetOrdinal("motlastchecked"));
                }
                else
                {
                    motlastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insuranceexpiry")) == false)
                {
                    insuranceexpiry = reader.GetDateTime(reader.GetOrdinal("insuranceexpiry"));
                }
                else
                {
                    insuranceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancelastchecked")) == false)
                {
                    insurancelastchecked = reader.GetDateTime(reader.GetOrdinal("insurancelastchecked"));
                }
                else
                {
                    insurancelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("serviceexpiry")) == false)
                {
                    serviceexpiry = reader.GetDateTime(reader.GetOrdinal("serviceexpiry"));
                }
                else
                {
                    serviceexpiry = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicelastchecked")) == false)
                {
                    servicelastchecked = reader.GetDateTime(reader.GetOrdinal("servicelastchecked"));
                }
                else
                {
                    servicelastchecked = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("taxcheckedby")) == false)
                {
                    taxcheckedby = reader.GetInt32(reader.GetOrdinal("taxcheckedby"));
                }
                else
                {
                    taxcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("motcheckedby")) == false)
                {
                    motcheckedby = reader.GetInt32(reader.GetOrdinal("motcheckedby"));
                }
                else
                {
                    motcheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancecheckedby")) == false)
                {
                    insurancecheckedby = reader.GetInt32(reader.GetOrdinal("insurancecheckedby"));
                }
                else
                {
                    insurancecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("servicecheckedby")) == false)
                {
                    servicecheckedby = reader.GetInt32(reader.GetOrdinal("servicecheckedby"));
                }
                else
                {
                    servicecheckedby = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("mottestnumber")) == false)
                {
                    mottestnumber = reader.GetString(reader.GetOrdinal("mottestnumber"));
                }
                else
                {
                    mottestnumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("insurancenumber")) == false)
                {
                    insurancenumber = reader.GetString(reader.GetOrdinal("insurancenumber"));
                }
                else
                {
                    insurancenumber = "";
                }
                doctypes = getDocumentPaths(carid);
                if (doctypes.ContainsKey(CarDocumentType.Insurance))
                {
                    doctypes.TryGetValue(CarDocumentType.Insurance, out insurancepath);
                }
                if (doctypes.ContainsKey(CarDocumentType.MOT))
                {
                    doctypes.TryGetValue(CarDocumentType.MOT, out motpath);
                }
                if (doctypes.ContainsKey(CarDocumentType.Service))
                {
                    doctypes.TryGetValue(CarDocumentType.Service, out servicepath);
                }
                if (doctypes.ContainsKey(CarDocumentType.Tax))
                {
                    doctypes.TryGetValue(CarDocumentType.Tax, out taxpath);
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                userdefined = clsuserdefined.getValues(AppliesTo.Car, carid);
                defaultuom = (MileageUOM)reader.GetByte(reader.GetOrdinal("default_unit"));
                if (!reader.IsDBNull(reader.GetOrdinal("enginesize")))
                {
                    enginesize = reader.GetInt32(reader.GetOrdinal("enginesize"));
                }

                if(!reader.IsDBNull(reader.GetOrdinal("approved")))
                {
                    approved = reader.GetBoolean(reader.GetOrdinal("approved"));
                } 
                else 
                {
                    approved = true;
                }
                car = new cCar(accountid, 0, carid, make, model, registration, startdate, enddate, active, getCarMileageCats(carid), cartypeid, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motexpiry, motlastchecked, motcheckedby, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, userdefined, getOdometerReadings(carid), insurancepath, motpath, servicepath, taxpath, defaultuom, enginesize, createdon, createdby, modifiedon, modifiedby, approved);
                
            }
            reader.Close();

            return car;

        }


        public System.Data.DataSet getPoolCarGrid()
        {
            object[] values;

            List<cCar> cars = getPoolCars();
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("carid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("make", System.Type.GetType("System.String"));
            tbl.Columns.Add("model", System.Type.GetType("System.String"));
            tbl.Columns.Add("registration", System.Type.GetType("System.String"));
            tbl.Columns.Add("startdate", System.Type.GetType("System.String"));
            tbl.Columns.Add("enddate", System.Type.GetType("System.String"));

            foreach (cCar car in cars)
            {
                values = new object[6];
                values[0] = car.carid;
                values[1] = car.make;
                values[2] = car.model;
                values[3] = car.registration;
                values[4] = car.startdate.ToShortDateString();
                values[5] = car.enddate.ToShortDateString();
                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        public System.Data.DataSet getEmployeePoolCarGrid(int employeeid)
        {
            object[] values;

            cEmployee reqemp = GetEmployeeById(employeeid);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("carid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("make", System.Type.GetType("System.String"));
            tbl.Columns.Add("model", System.Type.GetType("System.String"));
            tbl.Columns.Add("registration", System.Type.GetType("System.String"));
            tbl.Columns.Add("startdate", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("enddate", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("active", System.Type.GetType("System.Boolean"));

            foreach (cCar car in reqemp.cars)
            {
                if (car.employeeid == 0)
                {
                    values = new object[7];
                    values[0] = car.carid;
                    values[1] = car.make;
                    values[2] = car.model;
                    values[3] = car.registration;
                    values[4] = car.startdate.ToShortDateString();
                    values[5] = car.enddate.ToShortDateString();
                    values[6] = car.active;
                    tbl.Rows.Add(values);
                }
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        public Dictionary<int, int> getModifiedEmployeePoolCars(int employeeid, DateTime offGlobalDate)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            Dictionary<int, int> lstPoolCars = new Dictionary<int, int>();
            System.Data.SqlClient.SqlDataReader reader;
            DateTime createdOn, modifiedOn;
            int carid, poolcaruserid;
            bool update = false;

            strsql = "SELECT carid, poolcaruserid, createdon, modifiedon FROM pool_car_users WHERE employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                carid = reader.GetInt32(reader.GetOrdinal("carid"));
                poolcaruserid = reader.GetInt32(reader.GetOrdinal("poolcaruserid"));

                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdOn = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }

                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }

                if (createdOn > offGlobalDate || modifiedOn > offGlobalDate)
                {
                    update = true;
                }

                lstPoolCars.Add(poolcaruserid, carid);
            }
            reader.Close();

            if (update)
            {
                return lstPoolCars;
            }
            else
            {
                return null;
            }
            
        }

        public List<int> getUsersPerPoolCar(int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<int> lstUsers = new List<int>();
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "SELECT employeeid FROM pool_car_users WHERE carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                lstUsers.Add(reader.GetInt32(reader.GetOrdinal("employeeid")));
            }
            reader.Close();

            return lstUsers;
        }

        public int addPoolCarUser(int carid, int employeeid)
        {
            int poolcaruserid;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            strsql = "INSERT INTO pool_car_users (carid, employeeid, createdon, createdby) VALUES (@carid, @employeeid, @createdon, @createdby);select @identity = @@identity;";
            
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            poolcaruserid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return poolcaruserid;
        }

        public void deletePoolCarUsers(int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            strsql = "DELETE FROM pool_car_users WHERE carid = @carid";

            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void deleteUserFromPoolCar(int employeeid, int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            strsql = "DELETE FROM pool_car_users WHERE carid = @carid AND employeeid = @employeeid";

            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private List<int> getCarMileageCats(int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));            
            List<int> mileagecats = new List<int>();
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select mileageid from car_mileagecats where carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                mileagecats.Add(reader.GetInt32(0));
            }
            reader.Close();
            return mileagecats;
        }
        private string getLicencePath(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            string path = "";
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select filename from car_documents where employeeid = @employeeid and documenttype = 1";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                path = reader.GetString(0);
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();

            return path;
        }
        public int getPersonalMiles(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            int x = 0;
            DateTime taxyearstart;
            int year;
            int startreading, endreading;
            int totalmiles = 0;
            int businessmiles = 0;
            int personalmiles = 0;
            if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 3)
            {
                year = DateTime.Today.Year - 1;
            }
            else
            {
                year = DateTime.Today.Year;
            }
            taxyearstart = new DateTime(year, 04, 06);

            cEmployee reqemp = GetEmployeeById(employeeid);

            foreach (cCar car in reqemp.cars)
            {
                
                if (car.fuelcard == true)
                {
                    x = 0;
                    while (x < car.odometerreadings.Length)
                    {
                        if (car.odometerreadings[x].datestamp >= taxyearstart)
                        {
                            break;
                        }
                        x++;
                    }
                    if (x < car.odometerreadings.Length)
                    {
                        startreading = car.odometerreadings[x].oldreading;
                        endreading = car.odometerreadings[car.odometerreadings.Length - 1].newreading;
                        totalmiles += (endreading - startreading);

                        strsql = "select cast(sum(num_miles) as decimal) from savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid where carid = @carid and (date between @startdate and getDate())";
                        expdata.sqlexecute.Parameters.AddWithValue("@carid", car.carid);
                        expdata.sqlexecute.Parameters.AddWithValue("@startdate", taxyearstart);
                        businessmiles += Convert.ToInt32(expdata.getSum(strsql));
                        expdata.sqlexecute.Parameters.Clear();
                    }
                }
            }

            personalmiles = totalmiles - businessmiles;
            return personalmiles;
        }

        public decimal getBusinessMiles(int employeeid, DateTime startdate, DateTime enddate)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
            string strsql;
            decimal miles = 0;
            strsql = "select sum(num_miles) from savedexpenses_journey_steps where expenseid in (select expenseid from savedexpenses where claimid in (select claimid from claims where employeeid = @employeeid) and (date between @startdate and @enddate))";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
            expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    miles = reader.GetDecimal(0);
                }
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();
            return miles;
        }
        public List<int> getMultipleItems(int employeeid, bool customiseditems)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            List<int> list = new List<int>();
            System.Data.SqlClient.SqlDataReader reader;
            int count = 0;
            
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "select count(*) from additems where employeeid = @employeeid";
            count = expdata.getcount(strsql);
            if (count == 0 && customiseditems == false) // get role items
            {
                strsql = "select distinct subcatid from rolesubcats inner join employee_roles on employee_roles.itemroleid = rolesubcats.roleid where employee_roles.employeeid = @employeeid and isadditem = 1";
            }
            else
            {
                strsql = "select subcatid from additems where employeeid = @employeeid order by [order]";
            }
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            reader.Close();
            
            expdata.sqlexecute.Parameters.Clear();
            return list;
        }
        private cDepCostItem[] getCostCodeBreakdown(int employeeid)
        {
            cDepCostItem[] clsbreakdown;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count, i;
            int departmentid, costcodeid, percentused, projectcodeid;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select count(*) from employee_costcodes where employeeid = @employeeid";
            count = expdata.getcount(strsql);
            clsbreakdown = new cDepCostItem[count];

            if (count != 0)
            {
                i = 0;
                strsql = "select departmentid, costcodeid, percentused, projectcodeid from [employee_costcodes] where employeeid = @employeeid";

                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    if (reader.IsDBNull(reader.GetOrdinal("departmentid")) == false)
                    {
                        departmentid = reader.GetInt32(reader.GetOrdinal("departmentid"));
                    }
                    else
                    {
                        departmentid = 0;
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("costcodeid")) == false)
                    {
                        costcodeid = reader.GetInt32(reader.GetOrdinal("costcodeid"));
                    }
                    else
                    {
                        costcodeid = 0;
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("projectcodeid")) == false)
                    {
                        projectcodeid = reader.GetInt32(reader.GetOrdinal("projectcodeid"));
                    }
                    else
                    {
                        projectcodeid = 0;
                    }
                    percentused = reader.GetInt32(reader.GetOrdinal("percentused"));
                    clsbreakdown[i] = new cDepCostItem(departmentid, costcodeid, projectcodeid, percentused);
                    i++;
                }
                reader.Close();

            }
            expdata.sqlexecute.Parameters.Clear();
            return clsbreakdown;
        }
        private cOdometerReading[] getOdometerReadings(int carid)
        {
            cOdometerReading[] readings;
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;
            int odometerid, oldodometer, newodometer, createdby;
            int i;
            DateTime datestamp, createdon;
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);

            strsql = "select count(*) from odometer_readings where carid = @carid";
            count = expdata.getcount(strsql);

            readings = new cOdometerReading[count];

            if (count != 0)
            {
                i = 0;
                strsql = "select * from odometer_readings where carid = @carid";
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    odometerid = reader.GetInt32(reader.GetOrdinal("odometerid"));
                    datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
                    oldodometer = reader.GetInt32(reader.GetOrdinal("oldreading"));
                    if (reader.IsDBNull(reader.GetOrdinal("newreading")) == true)
                    {
                        newodometer = 0;
                    }
                    else
                    {
                        newodometer = reader.GetInt32(reader.GetOrdinal("newreading"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    readings[i] = new cOdometerReading(odometerid, carid, datestamp, oldodometer, newodometer, createdon, createdby);
                    i++;
                }
                reader.Close();
                
            }

            expdata.sqlexecute.Parameters.Clear();

            return readings;
        }
        #endregion

        public string getHolidayApprovers(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            string strsql;
            string rowclass = "row1";
            cBudgetholders clsholders = new cBudgetholders(accountid);
            cBudgetHolder holder;
            cTeams clsteams = new cTeams(accountid);
            cTeam reqteam;

            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp;
            System.Data.SqlClient.SqlDataReader reader;

            output.Append("<table class=datatbl>");
            output.Append("<tr><th>Group Name</th><th>Temporary Approver</th></tr>");
            strsql = "select groupname, holidaytype, holidayid from signoffs inner join groups on groups.groupid = signoffs.groupid where " +
                "(signofftype = 1 and (select count(*) from budgetholders where employeeid = @employeeid and budgetholderid = signoffs.relid) <> 0) or " +
                "(signofftype = 2 and relid = @employeeid) or " +
                "(signofftype = 3 and (select count(*) from teamemps where teamid = signoffs.relid and employeeid = @employeeid) <> 0)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                output.Append("<tr>");
                output.Append("<td class=\"" + rowclass + "\">" + reader.GetString(reader.GetOrdinal("groupname")) + "</td>");
                output.Append("<td class=\"" + rowclass + "\">");
                switch (reader.GetInt32(reader.GetOrdinal("holidaytype")))
                {
                    case 0:
                        output.Append("-");
                        break;
                    case 1:
                        holder = clsholders.getBudgetHolderById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                        reqemp = clsemployees.GetEmployeeById(holder.employeeid);
                        output.Append(reqemp.title + " " + reqemp.firstname + " " + reqemp.surname);
                        break;
                    case 2:
                        reqemp = clsemployees.GetEmployeeById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                        output.Append(reqemp.title + " " + reqemp.firstname + " " + reqemp.surname);
                        break;
                    case 3:
                        reqteam = clsteams.GetTeamById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                        output.Append(reqteam.teamname + " team");
                        break;
                }


                output.Append("</td>");
                output.Append("</tr>");
                if (rowclass == "row1")
                {
                    rowclass = "row2";
                }
                else
                {
                    rowclass = "row1";
                }

            }
            reader.Close();

            output.Append("</table>");
            return output.ToString();

        }

        //public string createClientCarArray(int employeeid, int mileagetotal)
        //{
        //    System.Text.StringBuilder output = new System.Text.StringBuilder();
        //    cCar reqcar;
        //    cMileagecats clsmileage = new cMileagecats(accountid);
        //    cMileageCat reqmileage;
        //    int i;
        //    decimal price = 0;
        //    decimal before = 0;
        //    decimal after = 0;
        //    System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

        //    cMisc clsmisc = new cMisc(accountid);
        //    cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

        //    cEmployee reqemp = GetEmployeeById(employeeid);

        //    output.Append("<script language=javascript>\n");
        //    output.Append("var mileageType = " + clsproperties.thresholdtype + ";\n");
        //    output.Append("var threshold = " + clsproperties.mileage + ";\n");
        //    output.Append("var cars = new Array();\n");

        //    for (i = 0; i < reqemp.cars.Length; i++)
        //    {
        //        reqcar = (cCar)reqemp.cars[i];
        //        reqmileage = clsmileage.GetMileageCatById(reqcar.mileageid);

        //        if (reqmileage != null)
        //        {
        //            switch (reqcar.cartypeid)
        //            {
        //                case 1:
        //                    if (reqemp.mileagetotal < mileagetotal)
        //                    {
        //                        price = reqmileage.before;

        //                    }
        //                    else
        //                    {
        //                        price = reqmileage.after;

        //                    }
        //                    before = reqmileage.before;
        //                    after = reqmileage.after;
        //                    break;
        //                case 2:
        //                    if (reqemp.mileagetotal < mileagetotal)
        //                    {
        //                        price = reqmileage.befored;

        //                    }
        //                    else
        //                    {
        //                        price = reqmileage.afterd;

        //                    }
        //                    before = reqmileage.befored;
        //                    after = reqmileage.afterd;
        //                    break;
        //                case 3:
        //                    if (reqemp.mileagetotal < mileagetotal)
        //                    {
        //                        price = reqmileage.beforelpg;

        //                    }
        //                    else
        //                    {
        //                        price = reqmileage.afterlpg;

        //                    }
        //                    before = reqmileage.beforelpg;
        //                    after = reqmileage.afterlpg;
        //                    break;
        //            }
        //            output.Append("cars[" + i + "] = new Array(" + reqcar.carid + ",'" + price.ToString("£###,###,##0.0000") + "','" + before.ToString("£###,###,##0.0000") + "','" + after.ToString("£###,###,##0.0000") + "','" + reqmileage.carsize + "');\n");
        //        }


        //    }
        //    output.Append("</script>");

        //    return output.ToString();
        //}
        public void incrementRefnum(int employeeid)
        {
            string strsql = "update employees set currefnum = currefnum + 1 where employeeid = @employeeid";
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();

            cEmployee reqemp = GetEmployeeById(employeeid);
            reqemp.incrementRefnum();
        }
        public void markBroadcastAsRead(int employeeid, int broadcastid)
        {
            ArrayList arrBroadcasts = getReadBroadcasts(employeeid);

            if (!arrBroadcasts.Contains(broadcastid))
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql;

                strsql = "insert into employee_readbroadcasts (employeeid, broadcastid, createdon, createdby) " +
                    "values (@employeeid, @broadcastid, @createdon, @userid)";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.sqlexecute.Parameters.AddWithValue("@broadcastid", broadcastid);
                expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();

                cEmployee reqemp = GetEmployeeById(employeeid);
                reqemp.broadcasts.Add(broadcastid);
            }
            
        }
        public void addSubcatToTemplate(int employeeid, int subcatid)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);
            int order = reqemp.useritems.Count + 1;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "insert into additems (employeeid, subcatid, [order]) values (@employeeid, @subcatid, @order)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@order", order);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
            reqemp.addSubcatToTemplate(subcatid);
        }

        public void removeSubcatFromTemplate(int employeeid, int subcatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "delete from additems where employeeid = @employeeid and subcatid = @subcatid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            cEmployee reqemp = GetEmployeeById(employeeid);
            reqemp.removeSubcatFromTemplate(subcatid);

            //update the orders;
            int order = 1;

            foreach (int i in reqemp.useritems)
            {
                strsql = "update additems set [order] = " + (reqemp.useritems.IndexOf(i) + 1) + " where employeeid = @employeeid and subcatid = @subcatid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.sqlexecute.Parameters.AddWithValue("@subcatid", i);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            


        }
        private void saveReadings(int carid, object[,] readings)
        {
            DateTime date;
            string strsql = "";
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int i;

            deleteOdometerReadings(carid);

            for (i = 0; i < readings.GetLength(0); i++)
            {
                date = (DateTime)readings[i, 0];
                strsql += "insert into odometer_readings (carid, datestamp, oldreading, newreading) " +
                    "values (@carid, '" + date.Year + "/" + date.Month + "/" + date.Day + " " + date.Hour + ":" + date.Minute + "." + date.Second + "'," + readings[i, 1] + "," + readings[i, 2] + ");";
            }
            if (strsql != "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        private void deleteOdometerReadings(int carid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "delete from odometer_readings where carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }



        public int addCar(int employeeid, string make, string model, string regno, DateTime startdate, DateTime enddate, List<int> mileagecats, MileageUOM defaultuom, byte cartypeid, bool active, Int64 odometer, bool fuelcard, int endodometer, object[,] readings, DateTime taxexpiry, DateTime taxlastchecked, int taxcheckedby, DateTime motexpiry, DateTime motlastchecked, int motcheckedby, string mottestnumber, DateTime insuranceexpiry, DateTime insurancelastchecked, int insurancecheckedby, string insurancenumber, DateTime serviceexpiry, DateTime servicelastchecked, int servicecheckedby, Dictionary<int, object> userdefinedfields, int engineSize, bool approved, int userid)
        {
            int carid;
           
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            if (employeeid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@make", make);
            expdata.sqlexecute.Parameters.AddWithValue("@model", model);
            expdata.sqlexecute.Parameters.AddWithValue("@regno", regno);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultuom", Convert.ToByte(defaultuom)); 
            expdata.sqlexecute.Parameters.AddWithValue("@cartypeid", cartypeid);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(active));
            expdata.sqlexecute.Parameters.AddWithValue("@odometer", odometer);
            expdata.sqlexecute.Parameters.AddWithValue("@fuelcard", Convert.ToByte(fuelcard));
            expdata.sqlexecute.Parameters.AddWithValue("@endodometer", endodometer);
            expdata.sqlexecute.Parameters.AddWithValue("@engineSize", engineSize);
            expdata.sqlexecute.Parameters.AddWithValue("@approved", approved);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);

            strsql = "insert into cars (employeeid, make, model, registration, startdate, enddate, cartypeid, active, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, motexpiry, motlastchecked, motcheckedby, mottestnumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, insurancenumber, serviceexpiry, servicelastchecked, servicecheckedby, default_unit, enginesize, approved, createdon, createdby) " +
                "values (@employeeid,@make,@model,@regno,@startdate,@enddate,@cartypeid,@active,@odometer,@fuelcard,@endodometer,@taxexpiry,@taxlastchecked,@taxcheckedby,@motexpiry,@motlastchecked,@motcheckedby, @mottestnumber, @insuranceexpiry, @insurancelastchecked, @insurancecheckedby, @insurancenumber, @serviceexpiry, @servicelastchecked, @servicecheckedby, @defaultuom, @engineSize, @approved, @createdon, @createdby);select @identity = @@identity";
            if (startdate == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);

            }

            if (enddate == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate.Year + "/" + enddate.Month + "/" + enddate.Day);

            }

            
            if (taxexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", taxexpiry);
            }
            if (taxlastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", taxlastchecked);
            }
            if (motexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", motexpiry);
            }
            if (motlastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", motlastchecked);
            }
            if (insuranceexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", insuranceexpiry);
            }
            if (insurancelastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", insurancelastchecked);
            }
            if (serviceexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", serviceexpiry);
            }
            if (servicelastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", servicelastchecked);
            }
            if (taxcheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", taxcheckedby);
            }
            if (motcheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", motcheckedby);
            }
            if (insurancecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", insurancecheckedby);
            }
            if (servicecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", servicecheckedby);
            }
            if (mottestnumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", mottestnumber);
            }
            if (insurancenumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", insurancenumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            carid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            updateMileageCats(carid, mileagecats);
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Car, carid, userdefinedfields);

            saveReadings(carid, readings);

            cAuditLog clsaudit;

            if (userid > 0)
            {
                clsaudit = new cAuditLog(accountid, userid);
                clsaudit.addRecord("Cars", make + " " + model + " " + regno);
            }
            
            
            return carid;

        }

        private void updateMileageCats(int carid, List<int> mileagecats)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "delete from car_mileagecats where carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            foreach (int i in mileagecats)
            {
                strsql = "insert into car_mileagecats (carid, mileageid) values (@carid, @mileageid)";
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", i);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }
        public void updateCar(int employeeid, int carid, string make, string model, string regno, DateTime startdate, DateTime enddate, List<int> mileagecats, MileageUOM defaultuom, byte cartypeid, bool active, Int64 odometer, bool fuelcard, int endodometer, object[,] readings, DateTime taxexpiry, DateTime taxlastchecked, int taxcheckedby, DateTime motexpiry, DateTime motlastchecked, int motcheckedby, string mottestnumber, DateTime insuranceexpiry, DateTime insurancelastchecked, int insurancecheckedby, string insurancenumber, DateTime serviceexpiry, DateTime servicelastchecked, int servicecheckedby, Dictionary<int, object> userdefinedfields, int engineSize, bool approved, int userid)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            cCar oldcar = null;

            if (reqemp != null && employeeid != 0)
            {
                oldcar = reqemp.getCarById(carid);
            }
            else
            {
                List<cCar> cars = getPoolCars();

                foreach (cCar car in cars)
                {
                    if (car.carid == carid)
                    {
                        oldcar = car;
                        break;
                    }
                }
            }

            string item = make + " " + model;
            
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            expdata.sqlexecute.Parameters.AddWithValue("@make", make);
            expdata.sqlexecute.Parameters.AddWithValue("@model", model);
            expdata.sqlexecute.Parameters.AddWithValue("@regno", regno);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultuom", Convert.ToByte(defaultuom)); 
            expdata.sqlexecute.Parameters.AddWithValue("@cartypeid", cartypeid);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(active));
            expdata.sqlexecute.Parameters.AddWithValue("@odometer", odometer);
            expdata.sqlexecute.Parameters.AddWithValue("@fuelcard", fuelcard);
            expdata.sqlexecute.Parameters.AddWithValue("@endodometer", endodometer);
            expdata.sqlexecute.Parameters.AddWithValue("@engineSize", engineSize);
            expdata.sqlexecute.Parameters.AddWithValue("@approved", approved);

            if (startdate == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);

            }



            if (enddate == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate.Year + "/" + enddate.Month + "/" + enddate.Day);

            }

           
            if (taxexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", taxexpiry);
            }
            if (taxlastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", taxlastchecked);
            }
            if (motexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", motexpiry);
            }
            if (motlastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", motlastchecked);
            }
            if (insuranceexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", insuranceexpiry);
            }
            if (insurancelastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", insurancelastchecked);
            }
            if (serviceexpiry == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", serviceexpiry);
            }
            if (servicelastchecked == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", servicelastchecked);
            }
            if (taxcheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", taxcheckedby);
            }
            if (motcheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", motcheckedby);
            }
            if (insurancecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", insurancecheckedby);
            }
            if (servicecheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", servicecheckedby);
            }
            if (mottestnumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", mottestnumber);
            }
            if (insurancenumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", insurancenumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", employeeid);
            strsql = "update cars set make = @make, model = @model, registration = @regno, odometer = @odometer, startdate = @startdate, enddate = @enddate, cartypeid = @cartypeid, active = @active, fuelcard = @fuelcard, endodometer = @endodometer, taxexpiry = @taxexpiry, taxlastchecked = @taxlastchecked, taxcheckedby = @taxcheckedby, motexpiry = @motexpiry, motlastchecked = @motlastchecked, motcheckedby = @motcheckedby, mottestnumber = @mottestnumber, insuranceexpiry = @insuranceexpiry, insurancelastchecked = @insurancelastchecked, insurancecheckedby = @insurancecheckedby, insurancenumber = @insurancenumber, serviceexpiry = @serviceexpiry, servicelastchecked = @servicelastchecked, servicecheckedby = @servicecheckedby, default_unit = @defaultuom, enginesize=@engineSize, approved=@approved, modifiedon = @modifiedon, modifiedby = @modifiedby";


            strsql = strsql + " where carid = @carid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Car, carid, userdefinedfields);
            updateMileageCats(carid, mileagecats);
            saveReadings(carid, readings);
            //audit log
            cAuditLog clsaudit;

            if (userid > 0)
            {
                clsaudit = new cAuditLog(accountid, userid);

                if (oldcar.make != make)
                {
                    clsaudit.editRecord(item, "Make", "Car Details", oldcar.make, make);
                }
                if (oldcar.model != model)
                {
                    clsaudit.editRecord(item, "Model", "Car Details", oldcar.model, model);
                }
                if (oldcar.registration != regno)
                {
                    clsaudit.editRecord(item, "Registration No", "Car Details", oldcar.registration, regno);
                }
                if (oldcar.defaultuom != defaultuom)
                {
                    clsaudit.editRecord(item, "Unit of Measure", "Car Details", oldcar.defaultuom.ToString(), defaultuom.ToString());
                }
                if (oldcar.fuelcard != fuelcard)
                {
                    clsaudit.editRecord(item, "Fuel Card", "Car Details", oldcar.fuelcard.ToString(), fuelcard.ToString());
                }
                if (oldcar.cartypeid != cartypeid)
                {
                    string oldcartype, newcartype;
                    switch (oldcar.cartypeid)
                    {
                        case 1:
                            oldcartype = "Petrol";
                            break;
                        case 2:
                            oldcartype = "Diesel";
                            break;
                        case 3:
                            oldcartype = "LPG";
                            break;
                        default:
                            oldcartype = "";
                            break;
                    }
                    switch (cartypeid)
                    {
                        case 1:
                            newcartype = "Petrol";
                            break;
                        case 2:
                            newcartype = "Diesel";
                            break;
                        case 3:
                            newcartype = "LPG";
                            break;
                        default:
                            newcartype = "";
                            break;
                    }
                    clsaudit.editRecord(item, "Engine Type", "Car Details", oldcartype, newcartype);
                }
                if (oldcar.active != active)
                {
                    clsaudit.editRecord(item, "Car Active", "Car Details", oldcar.active.ToString(), active.ToString());
                }
                if (oldcar.startdate != startdate)
                {
                    clsaudit.editRecord(item, "Start Date", "Car Details", oldcar.startdate.ToShortDateString(), startdate.ToShortDateString());
                }
                if (oldcar.enddate != enddate)
                {
                    clsaudit.editRecord(item, "End Date", "Car Details", oldcar.enddate.ToShortDateString(), enddate.ToShortDateString());
                }
                if (oldcar.odometer != odometer)
                {
                    clsaudit.editRecord(item, "Odometer", "Car Details", oldcar.odometer.ToString(), odometer.ToString());
                }
            }
        }

        public void deleteCar(int employeeid, int carid)
        {
            cEmployee reqemp = null;

            if (employeeid != 0)
            {
                reqemp = GetEmployeeById(employeeid);
            }

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            cCar oldcar = null;
            string item;

            if (reqemp != null)
            {
                oldcar = reqemp.getCarById(carid);
            }
            else
            {
                List<cCar> cars = getPoolCars();

                foreach (cCar car in cars)
                {
                    if (car.carid == carid)
                    {
                        oldcar = car;
                        break;
                    }
                }
            }

            item = oldcar.make + " " + oldcar.model;

            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            strsql = "delete from cars where carid = @carid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cAuditLog clsaudit = new cAuditLog();
            clsaudit.deleteRecord("Car Details", item);
        }

        public void insertFuelCardReading(int employeeid, bool recordReading, int claimid, int carid, Int64 newodo)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);
            System.Data.SqlClient.SqlDataReader reader;
            cCar reqcar = reqemp.getCarById(carid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int business;
            int personal;
            int nummiles;
            string strsql;

            int fuelcardid = 0;

            //get the business miles
            strsql = "select sum(bmiles) from savedexpenses where claimid = " + claimid + " and carid = " + carid;
            business = expdata.getcount(strsql);

            if (recordReading == false) //just update the reading with this claims mileage
            {
                //does a record already exist?
                strsql = "select fuelcardid from fuel_cards where carid = " + carid + " and newodometer is null";
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        fuelcardid = reader.GetInt32(0);
                    }
                }
                reader.Close();

                if (fuelcardid == 0) //insert a new record
                {
                    strsql = "insert into fuel_cards (carid, oldodometer, business) " +
                        "values (" + carid + "," + reqcar.odometer + "," + business + ")";


                }
                else //update the current record
                {
                    strsql = "update fuel_cards set business = business + " + business + " where fuelcardid = " + fuelcardid;
                }
                expdata.ExecuteSQL(strsql);

                if (fuelcardid == 0) //get the fuelcardid to update claims used
                {
                    strsql = "select fuelcardid from fuel_cards where carid = " + carid + " and oldodometer = " + reqcar.odometer + " and business = " + business;
                    fuelcardid = expdata.getcount(strsql);
                }

                strsql = "insert into fuel_cards_claims (fuelcardid, claimid) " +
                    "values (" + fuelcardid + "," + claimid + ")";
                expdata.ExecuteSQL(strsql);
            }
            else
            {
                strsql = "select fuelcardid from fuel_cards where carid = " + carid + " and oldodometer = " + reqcar.odometer;
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        fuelcardid = reader.GetInt32(0);
                    }
                }
                reader.Close();
                

                nummiles = (int)(newodo - reqcar.odometer);
                personal = nummiles - business;

                if (fuelcardid == 0)
                {
                    //single record


                    strsql = "insert into fuel_cards (datestamp, carid, oldodometer, newodometer, business, personal) " +
                        "values (@datestamp, @carid, @oldodometer, @newodometer, @business, @personal)";
                    expdata.sqlexecute.Parameters.AddWithValue("@datestamp", DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
                    expdata.sqlexecute.Parameters.AddWithValue("@newodometer", newodo);
                    expdata.sqlexecute.Parameters.AddWithValue("@oldodometer", reqcar.odometer);
                    expdata.sqlexecute.Parameters.AddWithValue("@business", business);
                    expdata.sqlexecute.Parameters.AddWithValue("@personal", personal);
                    expdata.ExecuteSQL(strsql);


                    strsql = "select fuelcardid from fuel_cards where carid = @carid and newodometer = @newodometer and oldodometer = @oldodometer";
                    fuelcardid = expdata.getcount(strsql);
                    strsql = "insert into fuel_cards_claims (fuelcardid, claimid) " +
                        "values (" + fuelcardid + "," + claimid + ")";
                    expdata.ExecuteSQL(strsql);


                    //update the car record
                    strsql = "update cars set odometer = @newodometer where carid = @carid";

                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
                else
                {
                    //update the existing record
                    strsql = "update fuel_cards set business = business + " + business + " where fuelcardid = " + fuelcardid;
                    expdata.ExecuteSQL(strsql);
                    strsql = "insert into fuel_cards_claims (fuelcardid, claimid) " +
                        "values (" + fuelcardid + "," + claimid + ")";
                    expdata.ExecuteSQL(strsql);

                    //get the new business
                    strsql = "select business from fuel_cards where fuelcardid = " + fuelcardid;
                    business = expdata.getcount(strsql);
                    personal = nummiles - business;

                    strsql = "update fuel_cards set datestamp = @datestamp, newodometer = @newodometer, personal = @personal where fuelcardid = @fuelcardid";

                    expdata.sqlexecute.Parameters.AddWithValue("@newodometer", newodo);
                    expdata.sqlexecute.Parameters.AddWithValue("@personal", personal);
                    expdata.sqlexecute.Parameters.AddWithValue("@datestamp", DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    expdata.sqlexecute.Parameters.AddWithValue("@fuelcardid", fuelcardid);
                    expdata.ExecuteSQL(strsql);

                    //update the car record
                    strsql = "update cars set odometer = @newodometer where carid = @carid";
                    expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }

                reqcar.odometer = newodo;
            }


        }

        public void removeFuelCardReading(int claimid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int i;
            int carid;
            int fuelcardid;
            int business;
            int businesstotal = 0;
            Int64 newodometer;
            int x;
            bool useoldreading;
            int newfuelcardid;

            System.Data.DataSet rcdstclaims = new System.Data.DataSet();
            DateTime datestamp;
            System.Data.SqlClient.SqlDataReader reader;
            System.Data.DataSet rcdstfuel = new System.Data.DataSet();

            strsql = "select * from fuel_cards inner join fuel_cards_claims on fuel_cards_claims.fuelcardid = fuel_cards.fuelcardid where claimid = " + claimid;
            rcdstfuel = expdata.GetDataSet(strsql);

            for (i = 0; i < rcdstfuel.Tables[0].Rows.Count; i++)
            {
                useoldreading = false;
                newodometer = 0;
                fuelcardid = (int)rcdstfuel.Tables[0].Rows[i]["fuelcardid"];
                carid = (int)rcdstfuel.Tables[0].Rows[i]["carid"];
                if (rcdstfuel.Tables[0].Rows[i]["datestamp"] == DBNull.Value) //odometer reading hasn't been set yet so we can just remove business miles
                {
                    strsql = "select sum(bmiles) from savedexpenses where claimid = " + claimid + " and carid = " + carid;
                    business = expdata.getcount(strsql);
                    strsql = "update fuel_cards set business = business - " + business + " where fuelcardid = " + fuelcardid;
                    expdata.ExecuteSQL(strsql);
                    strsql = "delete from fuel_cards_claims where fuelcardid = " + fuelcardid + " and claimid = " + claimid;
                    expdata.ExecuteSQL(strsql);
                }
                else //we need to remove all readings later than this one
                {
                    datestamp = (DateTime)rcdstfuel.Tables[0].Rows[i]["datestamp"];
                    //select the latest odo reading before the one we're deleting
                    strsql = "select newodometer from fuel_cards where carid = " + carid + " and datestamp = (select max(datestamp) from fuel_cards where carid = " + carid + " and datestamp < @datestamp)";
                    expdata.sqlexecute.Parameters.AddWithValue("@datestamp", datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second);
                    reader = expdata.GetReader(strsql);
                    useoldreading = true;
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) == true)
                        {
                            useoldreading = true;
                        }
                        else
                        {
                            newodometer = reader.GetInt64(0);
                            useoldreading = false;
                        }
                    }
                    reader.Close();
                    expdata.sqlexecute.Parameters.Clear();

                    if (useoldreading == true)
                    {
                        strsql = "select oldodometer from fuel_cards where carid = " + carid + " and datestamp = @datestamp";
                        expdata.sqlexecute.Parameters.AddWithValue("@datestamp", datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second);
                        reader = expdata.GetReader(strsql);
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(0) == true)
                            {
                                useoldreading = true;
                            }
                            else
                            {
                                newodometer = reader.GetInt64(0);

                            }
                        }
                        reader.Close();
                        expdata.sqlexecute.Parameters.Clear();
                    }
                    //insert new record to transfer deleted claims to
                    strsql = "insert into fuel_cards (oldodometer, carid) values (" + newodometer + "," + carid + ")";
                    expdata.ExecuteSQL(strsql);

                    strsql = "select fuelcardid from fuel_cards where carid = " + carid + " and fuelcardid <> " + fuelcardid + " and oldodometer = " + newodometer;
                    newfuelcardid = expdata.getcount(strsql);

                    //loop through claims
                    strsql = "select claimid from fuel_cards_claims where claimid <> " + claimid + " and fuelcardid <> " + newfuelcardid + " and fuelcardid in (select fuelcardid from fuel_cards where carid = " + carid + " and datestamp >= @datestamp)";
                    expdata.sqlexecute.Parameters.AddWithValue("@datestamp", datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second);
                    rcdstclaims = expdata.GetDataSet(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    business = 0;

                    for (x = 0; x < rcdstclaims.Tables[0].Rows.Count; x++)
                    {
                        strsql = "select sum(bmiles) from savedexpenses where carid = " + carid + " and claimid = " + claimid;
                        business = expdata.getcount(strsql);
                        businesstotal += business;
                        strsql = "insert into fuel_cards_claims (fuelcardid, claimid) values (" + newfuelcardid + "," + (int)rcdstclaims.Tables[0].Rows[i]["claimid"] + ")";
                        expdata.ExecuteSQL(strsql);
                    }

                    strsql = "update fuel_cards set business = " + business + " where fuelcardid = " + newfuelcardid;
                    expdata.ExecuteSQL(strsql);
                    //delete the claims >= this one
                    strsql = "delete from fuel_cards_claims where fuelcardid in (select fuelcardid from fuel_cards where carid = " + carid + " and datestamp >= @datestamp)";
                    expdata.sqlexecute.Parameters.AddWithValue("@datestamp", datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    //delete the fuel card
                    strsql = "delete from fuel_cards where fuelcardid = @fuelcardid";
                    expdata.sqlexecute.Parameters.AddWithValue("@fuelcardid", fuelcardid);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
            }

        }

        public List<ListItem> CreateMileageCatDropdown(int employeeid, int carid)
        {
            List<ListItem> cats = new List<ListItem>();
            cEmployee emp = GetEmployeeById(employeeid);
            cCar car = emp.getCarById(carid);

            cMileagecats clsmileagecats = new cMileagecats(accountid);
            cMileageCat mileagecat;

            foreach (int i in car.mileagecats)
            {
                mileagecat = clsmileagecats.GetMileageCatById(i);
                if (mileagecat != null && mileagecat.catvalid && car.defaultuom == mileagecat.mileUom)
                {
                    cats.Add(new ListItem(mileagecat.carsize, mileagecat.mileageid.ToString()));
                }
            }
            return cats;
        }
        public List<System.Web.UI.WebControls.ListItem> CreateCarDropDown(int employeeid)
        {
            List<System.Web.UI.WebControls.ListItem> cars = new List<System.Web.UI.WebControls.ListItem>();

            cEmployee reqemp = GetEmployeeById(employeeid);
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            string desc = "";
            foreach (cCar car in reqemp.cars)
            {
                if (car.active && car.cartypeid > 0 && car.mileagecats.Count > 0 && car.passesDutyOfCare(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry, clsproperties.blockinsuranceexpiry))
                {
                    desc = car.make + " " + car.model;
                    if (car.registration != "")
                    {
                        desc += " (" + car.registration + ")";
                    }
                    cars.Add(new System.Web.UI.WebControls.ListItem(desc, car.carid.ToString()));

                }
            }
           

            return cars;
        }
        

        

        

        
        public void markNoteAsRead(int noteid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "update notes set [read] = 1 where noteid = @noteid";
            expdata.sqlexecute.Parameters.AddWithValue("@noteid", noteid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        #region proxies
        public System.Data.DataSet getProxies(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.DataSet ds;

            strsql = "select employees.employeeid, username, title + ' ' + firstname + ' ' + surname as [empname] from employees inner join employee_proxies on employees.employeeid = employee_proxies.proxyid where employee_proxies.employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }
        public void assignProxy(int assigningid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", assigningid);
            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

            strsql = "select count(*) from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
            count = expdata.getcount(strsql);
            if (count != 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return;
            }
            strsql = "insert into employee_proxies (employeeid, proxyid) " +
                "values (@employeeid, @proxyid)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

        }

        public void removeProxy(int removingid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "delete from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", removingid);
            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        public bool isProxy(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;

            strsql = "select count(*) from employee_proxies where proxyid = @proxyid";
            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public System.Web.UI.WebControls.ListItem[] createProxyDropDown(int employeeid)
        {
            int count;
            int i;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.SqlClient.SqlDataReader empreader;

            expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

            strsql = "select count(*) from employees where archived = 0 and " +
                " employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";
            count = expdata.getcount(strsql);

            System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[count];
            strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees where archived = 0 and " +
                " employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";

            empreader = expdata.GetReader(strsql);
            i = 0;
            while (empreader.Read())
            {
                tempitems[i] = new System.Web.UI.WebControls.ListItem();
                tempitems[i].Text = empreader.GetString(empreader.GetOrdinal("empname"));
                tempitems[i].Value = empreader.GetInt32(empreader.GetOrdinal("employeeid")).ToString();
                if (empreader.GetInt32(empreader.GetOrdinal("employeeid")) == employeeid)
                {
                    tempitems[i].Selected = true;
                }
                i++;
            }
            empreader.Close();
            expdata.sqlexecute.Parameters.Clear();

            return tempitems;
        }
        #endregion

        public System.Data.DataTable getNotes(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet ds;
            string strsql;



            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("datestamp", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("note", System.Type.GetType("System.String"));

            strsql = "select * from notes where [read] = 0 and employeeid = " + employeeid + " order by datestamp desc";
            ds = expdata.GetDataSet(strsql);
            tbl = ds.Tables[0];
            

            return tbl;
        }

        #region corporate cards
        public ReturnCode addCorporateCard(cEmployeeCorporateCard card)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int corporatecardid;
            strsql = "insert into employee_corporate_cards (employeeid, cardproviderid, cardnumber, active, createdon, createdby) " +
                "values (@employeeid, @cardprovider, @cardnumber, @active, @date, @employeeid); select @identity = @@identity";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", card.employeeid);
            
            expdata.sqlexecute.Parameters.AddWithValue("@cardprovider", (byte)card.cardprovider.cardproviderid);
            expdata.sqlexecute.Parameters.AddWithValue("@cardnumber", card.cardnumber);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(card.active));
            expdata.sqlexecute.Parameters.AddWithValue("@date", card.createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            corporatecardid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();
            
            card.updateCorporateCardId(corporatecardid);

            
            
            return ReturnCode.OK;
        }

        public ReturnCode updateCorporateCard(cEmployeeCorporateCard card)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "update employee_corporate_cards set cardnumber = @cardnumber, active = @active, modifiedon = @date, modifiedby = @userid where corporatecardid = @corporatecardid";

            expdata.sqlexecute.Parameters.AddWithValue("@corporatecardid", card.corporatecardid);
            
            expdata.sqlexecute.Parameters.AddWithValue("@cardprovider", (byte)card.cardprovider.cardproviderid);
            expdata.sqlexecute.Parameters.AddWithValue("@cardnumber", card.cardnumber);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(card.active));
            expdata.sqlexecute.Parameters.AddWithValue("@date", card.modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", card.modifiedby);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
            

            return ReturnCode.OK;
        }
        public void deleteCorporateCard(int employeeid, int corporatecardid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "delete from employee_corporate_cards where corporatecardid = @corporatecardid";
            expdata.sqlexecute.Parameters.AddWithValue("@corporatecardid", corporatecardid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            //cEmployee reqemp = GetEmployeeById(employeeid);
            //reqemp.deleteCorporateCard(corporatecardid);
            
        }
        public SortedList getCorporateCards(int employeeid)
        {
            SortedList cards = new SortedList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            cCardProviders clscardproviders = new cCardProviders();
            cCardProvider provider;
            string cardnumber;
            int corporatecardid;
            bool active;
            string strsql;
            cEmployeeCorporateCard card;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select * from employee_corporate_cards where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                corporatecardid = reader.GetInt32(reader.GetOrdinal("corporatecardid"));
                
                provider = clscardproviders.getProviderByID(reader.GetInt32(reader.GetOrdinal("cardproviderid")));
                cardnumber = reader.GetString(reader.GetOrdinal("cardnumber"));
                active = reader.GetBoolean(reader.GetOrdinal("active"));
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                card = new cEmployeeCorporateCard(corporatecardid, employeeid, provider, cardnumber, active, createdon, createdby, modifiedon, modifiedby);
                cards.Add(corporatecardid, card);
            }
            reader.Close();
            

            return cards;
        }
        #endregion


        public void clearNotes(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "delete from notes where employeeid = " + employeeid;
            expdata.ExecuteSQL(strsql);

        }

        public void hasCustomisedItems(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "update employees set customiseditems = 1 where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cEmployee reqemp = GetEmployeeById(employeeid);
            reqemp.hasCustomisedItems();
        }
        public int incrementClaimNo(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int claimno;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "update employees set curclaimno = curclaimno + 1 where employeeid = @employeeid";
            expdata.ExecuteSQL(strsql);
            claimno = expdata.getcount("select curclaimno from employees where employeeid = @employeeid");
            expdata.sqlexecute.Parameters.Clear();

            cEmployee reqemp = GetEmployeeById(employeeid);
            reqemp.incrementClaimNo();
            return claimno;
        }


        public decimal getMileageTotal(int employeeid, DateTime date)
        {
            int year = date.Year;

            decimal total = 0;

            if (date < new DateTime(year, 04, 06))
            {
                year--;
            }
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int financialyear;
            decimal mileagetotal = 0;
            SortedList<int, decimal> totals = new SortedList<int, decimal>();
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select financial_year, mileagetotal from employee_mileagetotals where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    financialyear = reader.GetInt32(0);
                    if (reader.IsDBNull(1))
                    {
                        mileagetotal = 0;
                    }
                    else
                    {
                        mileagetotal = reader.GetDecimal(1);
                    }
                    totals.Add(financialyear, mileagetotal);
                }
            }
            reader.Close();
            

            if (totals.ContainsKey(year))
            {
                total = totals[year];
                if (totals.IndexOfKey(year) == 0)
                {
                    cEmployee reqemp = GetEmployeeById(employeeid);
                    total += reqemp.mileagetotal;
                }
            }

            
            return total;
        }
        public sCarInfo getModifiedCars(int employeeid, DateTime date)
        {
            sCarInfo carInfo = new sCarInfo();
            Dictionary<int, cCar> lstCars = new Dictionary<int, cCar>();
            List<int> lstcarids = new List<int>();
            Dictionary<int, cOdometerReading> lstodoreadings = new Dictionary<int, cOdometerReading>();
            SortedList<int, int> lstodoids = new SortedList<int, int>();

            cEmployee employee = GetEmployeeById(employeeid);
            List<cCar> cars = employee.cars;

            foreach (cCar car in cars)
            {
                if (car.createdon > date || car.modifiedon > date)
                {
                    lstCars.Add(car.carid, car);
                }
                lstcarids.Add(car.carid);

                foreach (cOdometerReading odo in car.odometerreadings)
                {
                    if (odo.createdon > date)
                    {
                        lstodoreadings.Add(odo.odometerid, odo);
                    }
                    lstodoids.Add(odo.odometerid, car.carid);
                }
            }

            carInfo.lstonlinecars = lstCars;
            carInfo.lstcarids = lstcarids;
            carInfo.lstonlineodoreadings = lstodoreadings;
            carInfo.lstodoids = lstodoids;

            return carInfo;
        }
        public void InsertCostCodeBreakdown(int employeeid, int action)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            if (reqemp.breakdown.Length == 0) //no items
            {
                return;
            }

            if (action == 2) //delete current breakdown
            {
                deleteCostCodeBreakdown(employeeid);
            }

            cDepCostItem[] items;

            int i = 0;

            //expenseid = cItems[curi].expenseid;

            items = reqemp.breakdown;

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            for (i = 0; i < items.Length; i++)
            {
                if (items[i].departmentid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@departmentid" + i, DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@departmentid" + i, items[i].departmentid);
                }
                if (items[i].costcodeid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@costcodeid" + i, DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@costcodeid" + i, items[i].costcodeid);
                }
                if (items[i].projectcodeid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid" + i, DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid" + i, items[i].projectcodeid);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@percentused" + i, items[i].percentused);
                strsql = "insert into [employee_costcodes] (employeeid, departmentid, costcodeid, percentused, projectcodeid) " +
                    "values (@employeeid,@departmentid" + i + ",@costcodeid" + i + ",@percentused" + i + ",@projectcodeid" + i + ")";


                expdata.ExecuteSQL(strsql);
            }

            expdata.sqlexecute.Parameters.Clear();




        }

        private void deleteCostCodeBreakdown(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "delete from [employee_costcodes] where employeeid = @employeeid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void addCostcodeBreakdownRow(int employeeid, int costcodeid, int departmentid, int projectcodeid, int percentage)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            
                if (departmentid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@departmentid", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@departmentid", departmentid);
                }
                if (costcodeid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
                }
                if (projectcodeid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", projectcodeid);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@percentused", percentage);

                strsql = "insert into [employee_costcodes] (employeeid, departmentid, costcodeid, percentused, projectcodeid) " +
                    "values (@employeeid,@departmentid, @costcodeid, @percentused, @projectcodeid)";


                expdata.ExecuteSQL(strsql);
            

            expdata.sqlexecute.Parameters.Clear();
        }

        public void deleteOdometerReading(int employeeid, int carid, int odometerid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "delete from odometer_readings where odometerid = @odometerid";
            expdata.sqlexecute.Parameters.AddWithValue("@odometerid", odometerid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cEmployee reqemp = GetEmployeeById(employeeid);
            cCar car = reqemp.getCarById(carid);
            car.odometerreadings = getOdometerReadings(carid);
            
        }

        public int updateOdometerReading(int employeeid, int carid, int odometerid, int newodometer, byte businessmiles)
        {
            int newodometerid;
            cEmployee reqemp = GetEmployeeById(employeeid);
            cCar car = reqemp.getCarById(carid);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            cOdometerReading reading = car.getLastOdometerReading();
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            int oldodo = 0;
            if (odometerid != 0)
            {
                oldodo = reading.newreading;
            }
            else
            {
                oldodo = 0;
            }


            strsql = "insert into odometer_readings (carid, oldreading, newreading, businessmileage, createdon, createdby) " +
                "values (@carid, @oldodometer, @newodometer, @businessmiles, @createdon, @createdby);select @identity = @@identity";
            expdata.sqlexecute.Parameters.AddWithValue("@oldodometer", oldodo);

            expdata.sqlexecute.Parameters.AddWithValue("@newodometer", newodometer);
            if (businessmiles == 2)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@businessmiles", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@businessmiles", businessmiles);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);

            newodometerid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            expdata.sqlexecute.Parameters.Clear();
            car.odometerreadings = getOdometerReadings(carid);

            return newodometerid;
        }
        public Dictionary<int, cEmployee> getModifiedEmployees(List<int> empList, DateTime date)
        {
            Dictionary<int, cEmployee> lstupdatedemps = new Dictionary<int, cEmployee>();

            foreach (int val in empList)
            {
                cEmployee emp = GetEmployeeById(val);

                if (emp.modifiedon > date)
                {
                    lstupdatedemps.Add(emp.employeeid, emp);
                }
            }

            return lstupdatedemps;
        }

        public SortedList<CarDocumentType, string> getDocumentPaths(int carid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            SortedList<CarDocumentType, string> doctypes = new SortedList<CarDocumentType, string>();
            CarDocumentType doctype;
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select documenttype, filename from car_documents where carid = @carid ORDER BY documentid DESC";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                doctype = (CarDocumentType)reader.GetByte(reader.GetOrdinal("documenttype"));
                switch (doctype)
                {
                    case CarDocumentType.Insurance:
                        if (!doctypes.ContainsKey(CarDocumentType.Insurance))
                        {
                            doctypes.Add(CarDocumentType.Insurance, reader.GetString(1));
                        }
                        break;
                    case CarDocumentType.MOT:
                        if (!doctypes.ContainsKey(CarDocumentType.MOT))
                        {
                            doctypes.Add(CarDocumentType.MOT, reader.GetString(1));
                        }
                        break;
                    case CarDocumentType.Service:
                        if (!doctypes.ContainsKey(CarDocumentType.Service))
                        {
                            doctypes.Add(CarDocumentType.Service, reader.GetString(1));
                        }
                        break;
                    case CarDocumentType.Tax:
                        if (!doctypes.ContainsKey(CarDocumentType.Tax))
                        {
                            doctypes.Add(CarDocumentType.Tax, reader.GetString(1));
                        }
                        break;
                }

            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();

            return doctypes;
        }

        public Dictionary<int, cRoleSubcat> getResultantRoleSet(int employeeid)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);

            Dictionary<int, cRoleSubcat> roleset = new Dictionary<int, cRoleSubcat>();
            cItemRoles clsroles = new cItemRoles(accountid);
            cItemRole reqrole;
            foreach (cItemRole role in reqemp.roles)
            {
                reqrole = clsroles.getItemRoleById(role.itemroleid);

                foreach (cRoleSubcat rolesubcat in reqrole.items.Values)
                {
                    if (roleset.ContainsKey(rolesubcat.subcat.subcatid) == false) //doesn't exist so add it
                    {
                        roleset.Add(rolesubcat.subcat.subcatid, rolesubcat);
                    }
                    else
                    {

                    }
                }
            }

            return roleset;
        }


        #region views
        public cUserView getUserView(int employeeid, UserView viewtype, bool printview)
        {
            cEmployee reqemp = GetEmployeeById(employeeid);

            cUserView view;
            reqemp.views.TryGetValue(viewtype, out view);
            bool defaultview;
            if (view == null)
            {
                SortedList<int, cField> fieldsforview = getFieldsForView(printview, employeeid, viewtype, out defaultview);

                view = new cUserView(accountid, employeeid, viewtype, printview, fieldsforview, generateViewSQL(viewtype, fieldsforview, printview, defaultview));

                if (!reqemp.views.ContainsKey(viewtype))
                {
                    reqemp.views.Add(viewtype, view);
                }
            }

            return view;
        }

        private SortedList<int, cField> getFieldsForView(bool printview, int employeeid, UserView viewtype, out bool defaultview)
        {
            defaultview = false;
            expenses.cFields clsfields = new expenses.cFields(accountid);
            cField reqfield;

            SortedList<int, cField> list = new SortedList<int, cField>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            int order;
            Guid fieldid;
            string strsql;
            bool useprintout = false;
            if (printview)
            {
                int count;
                strsql = "select count(*) from print_views";
                

                count = expdata.getcount(strsql);
                expdata.sqlexecute.Parameters.Clear();
                if (count != 0)
                {
                    useprintout = true;
                }
            }

            if (useprintout)
            {
                strsql = "select fieldid from [print_views] order by [order]";
                
            }
            else
            {
                strsql = "select fieldid from [views] where employeeid = @employeeid and viewid = @viewid order by [order]";
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@viewid", (int)viewtype);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            order = 1;
            while (reader.Read())
            {
                fieldid = reader.GetGuid(0);

                reqfield = clsfields.getFieldById(fieldid);


                list.Add(order, reqfield);
                order++;
            }
            reader.Close();
            if (list.Count == 0)
            {
                defaultview = true;
                strsql = "select fieldid, [order] from [default_views]";
                
                reader = expdata.GetReader(strsql);
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    fieldid = reader.GetGuid(0);
                    order = reader.GetInt32(1);
                    reqfield = clsfields.getFieldById(fieldid);
                    list.Add(order, reqfield);
                }
                reader.Close();
            }
            return list;
        }
        private string generateViewSQL(UserView viewtype,SortedList<int, cField> lstfields, bool printview, bool defaultview)
        {
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            if (clsproperties.allowmultipledestinations) //remove from, to
            {
                for (int x = lstfields.Count - 1; x >= 0; x--)
                {
                    if (lstfields.Values[x].fieldid == new Guid("c75064ec-be87-4dd3-8299-d0d81ea3f819") || lstfields.Values[x].fieldid == new Guid("3d8c699e-9e0e-4484-b821-b49b5cb4c098"))
                    {
                        lstfields.RemoveAt(x);
                    }
                }
            }
            StringBuilder output = new StringBuilder();
            List<cField> tmpfields = new List<cField>();
            string table, expenseid;
            if (viewtype == UserView.Previous || viewtype == UserView.PreviousPrint)
            {
                table = "savedexpenses_previous";
                expenseid = "savedexpenses_previous.expenseid";
            }
            else
            {
                table = "savedexpenses_current";
                expenseid = "savedexpenses_current.expenseid";
            }
            output.Append("select ");
            output.Append(table + ".expenseid, " + table + ".basecurrency, " + table + ".currencyid as originalcurrency, globalbasecurrency, " + table + ".returned, " + table + ".transactionid, ");
            foreach (cField fld in lstfields.Values)
            {
                if (fld != null)
                {
                    tmpfields.Add(fld);

                    switch (fld.fieldid.ToString())
                    {
                        case "359dfac9-74e6-4be5-949f-3fb224b1cbfc":
                            output.Append("dbo.getCostcodeSplit(" + expenseid + ") as [88], ");
                            break;
                        case "9617a83e-6621-4b73-b787-193110511c17":
                            output.Append("dbo.getDepartmentSplit(" + expenseid + ") as [90], ");
                            break;
                        case "6d06b15e-a157-4f56-9ff2-e488d7647219":
                            output.Append("dbo.getProjectcodeSplit(" + expenseid + ") as [247], ");
                            break;
                        case "ec527561-dfee-48c7-a126-0910f8e031b0":
                            output.Append("countries.countryid as [31], ");
                            break;
                        case "1ee53ae2-2cdf-41b4-9081-1789adf03459":
                            output.Append("currencies.currencyid as [27], ");
                            break;
                        default:
                            if (fld.table.tableid == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8") || fld.table.tableid == new Guid("fa59baa6-99c8-484d-a206-d4ecec4874f0") || fld.table.tableid == new Guid("9c69deba-b2de-4836-8767-091da9cb7c79"))
                            {
                                switch (fld.fieldtype)
                                {
                                    case "C":
                                        output.Append("round([" + table + "].[" + fld.field + "],2) as [" + fld.fieldid + "], ");
                                        break;
                                    case "FD":
                                        if (table != "savedexpenses" && fld.field.Contains("savedexpenses."))
                                        {
                                            output.Append(fld.field.Replace("savedexpenses.", table + ".") + " as [" + fld.fieldid + "], ");
                                        }
                                        else
                                        {
                                            output.Append(fld.field + " as [" + fld.fieldid + "], ");
                                        }
                                        break;
                                    default:
                                        output.Append("[" + table + "].[" + fld.field + "] as [" + fld.fieldid + "], ");
                                        break;
                                }
                            }
                            else
                            {
                                switch (fld.fieldtype)
                                {
                                    case "C":
                                        output.Append("round([" + fld.table.tablename + "].[" + fld.field + "],2) as [" + fld.fieldid + "], ");
                                        break;
                                    case "FC":
                                        output.Append("round(" + fld.table.tablename + "." + fld.field + ",2) as [" + fld.fieldid + "], ");
                                        break;
                                    default:
                                        output.Append("[" + fld.table.tablename + "].[" + fld.field + "] as [" + fld.fieldid + "], ");
                                        break;
                                }
                            }
                            break;
                    }
                }

            }

            output.Remove(output.Length - 2, 2);


            output.Append(" FROM [" + table + "]");

            cJoins clsjoins = new cJoins(accountid, cAccounts.getConnectionString(accountid), ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString, new cTables(accountid), new cFields(accountid));

            bool customview = false;
            if (defaultview || printview)
            {
                customview = true;
            }
            output.Append(clsjoins.createJoinSQL(tmpfields, viewtype, customview));
            if (defaultview || printview)
            {
                if (viewtype == UserView.Previous || viewtype == UserView.PreviousPrint)
                {
                    output = output.Replace("[savedexpenses]", "[savedexpenses_previous]");
                    output = output.Replace("[savedexpenses_current]", "[savedexpenses_previous]");
                }
                else
                {
                    output = output.Replace("[savedexpenses]", "[savedexpenses_current]");
                    output = output.Replace("[savedexpenses_previous]", "[savedexpenses_current]");
                }
            }
            return output.ToString();

        }
        #endregion

        #region expensesConnect Users

        public void saveExpensesConnectUser(int employeeid, bool isUser)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@isUser", isUser);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
            expdata.ExecuteProc("saveExpensesConnectUser");
            expdata.sqlexecute.Parameters.Clear();
        }

        public List<cEmployee> getExpensesConnectUsers()
        {
            List<int> lstEmpIds = new List<int>();
            List<cEmployee> lstExpensesConnectUsers = new List<cEmployee>();
            int employeeid;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "SELECT employeeid from employees WHERE expensesConnectUser = 1 ORDER BY surname";
            SqlDataReader reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                lstEmpIds.Add(employeeid);
            }
            reader.Close();

            cEmployee emp;

            foreach (int i in lstEmpIds)
            {
                emp = GetEmployeeById(i);
                lstExpensesConnectUsers.Add(emp);
            }

            return lstExpensesConnectUsers;
        }

        public int getExpensesConnectUserCount()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "SELECT count(*) from employees WHERE expensesConnectUser = 1";
            int count = expdata.getcount(strsql);

            return count;
        }

        #endregion
    }



    

    

}
