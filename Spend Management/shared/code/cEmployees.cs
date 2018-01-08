using SpendManagementLibrary.Addresses;


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;

using SpendManagementLibrary;
using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary.FinancialYears;
using AjaxControlToolkit;
using System.Data;
using System.Web;

    namespace Spend_Management
    {
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;

    using Infragistics.WebUI.UltraWebGrid;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using Utilities.DistributedCaching;
    using shared.code;
    using expenses.code;
    using SpendManagementLibrary.DVLA;

        /// <summary>
    /// Summary description for employees.
    /// </summary>
    public class cEmployees
    {
        //string strrole;
        //int accesstype = 0;
        private readonly int _nAccountid;
        private readonly cFields _fields;

        public Cache Cache;

        /// <summary>
        /// Initialises a new instance of the <see cref="cEmployees"/> class. 
        /// </summary>
        /// <param name="accountid">
        /// </param>
        public cEmployees(int accountid)
        {
            this._fields = new cFields(accountid);
            this.Cache = new Cache();
            this._nAccountid = accountid;
        }

        #region properties

        /// <summary>
        /// Customer account ID
        /// </summary>
        public int accountid
        {
            get { return _nAccountid; }
        }

        #endregion

        /// <summary>
        /// Sends a Forgotten Email to the given email address
        /// </summary>
        /// <param name="emailAddress">
        /// The requesting email address
        /// </param>
        /// <param name="currentModule">
        /// The current module
        /// </param>
        /// <param name="fromMobile">
        /// Whether the request came from a mobile device or not - this changes the URL in the content of the email.
        /// </param>
        /// <param name="brandName">
        /// The branding text to use within messages that will be sent, if not specified this will be determined from the module.
        /// </param>
        /// <param name="requestSource">
        /// The source of the password reset request, i.e. CorpD, Expenses360 or web
        /// </param>
        /// <returns>
        /// The <see cref="ForgottenDetailsResponse"/> ForgottenDetailsResponse 
        /// </returns>
        public static ForgottenDetailsResponse RequestForgottenDetails(string emailAddress, Modules currentModule, bool fromMobile = false, string brandName = null, string requestSource = "")
        {
            emailAddress = emailAddress.Trim();

            UniqueEmployeeCheck uniqueCheck = CheckEmailIsUnique(emailAddress);

            ForgottenDetailsResponse response = uniqueCheck.IsUnique;

            if (response == ForgottenDetailsResponse.EmployeeDetailsSent)
            {
                var employees = new cEmployees(uniqueCheck.AccountID);
                var reqEmployee = employees.GetEmployeeById(uniqueCheck.EmployeeID);
                if (reqEmployee.Archived)
                {
                    response = ForgottenDetailsResponse.ArchivedEmployee;
                }
                else if (reqEmployee.Active == false)
                {
                    response = ForgottenDetailsResponse.InactiveEmployee;
                }
                else if (reqEmployee.Locked)
                {
                    response = ForgottenDetailsResponse.EmployeeLocked;
                }
                else
                {
                    employees.SendPasswordKey(uniqueCheck.EmployeeID, PasswordKeyType.ForgottenPassword, null, currentModule,fromMobile: fromMobile, requestSource: requestSource);
                }
            }
            
            return response;
        }

        public void RemovePasswordKey(string uniqueKey, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.AddWithValue("@uniqueKey", uniqueKey, 50);
            const string SQL = "DELETE FROM employeePasswordKeys WHERE uniqueKey=@uniqueKey";
                expdata.ExecuteSQL(SQL);
        }
        }

        public string AddPasswordKey(int employeeID, PasswordKeyType passwordKeyType, DateTime? sendOnDate, IDBConnection connection = null)
        {
            string uniqueKey;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
            string currentPasswordHash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(employeeID.ToString(), "MD5");
                uniqueKey = this.accountid.ToString().Substring(0, 1) + currentPasswordHash.Substring(0, 20) + this.accountid.ToString().Substring(1, this.accountid.ToString().Length - 1);
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);

            const string SQL = "DELETE FROM employeePasswordKeys WHERE employeeID=@employeeID";

                expdata.ExecuteSQL(SQL);
            
                expdata.AddWithValue("@uniqueKey", uniqueKey, 50);

            if (sendOnDate.HasValue)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendOnDate", sendOnDate.Value);
            }
            else
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendOnDate", DBNull.Value);
            }

                expdata.sqlexecute.Parameters.AddWithValue("@passwordKeyType", (Int16)passwordKeyType);
                expdata.ExecuteProc("saveEmployeePasswordKey");
                expdata.sqlexecute.Parameters.Clear();
            }

            return uniqueKey;
        }

        /// <summary>
        /// Send the two emails for password reset
        /// </summary>
        /// <param name="employeeID">
        /// EmployeeID
        /// </param>
        /// <param name="passwordKeyType">
        /// encryption type
        /// </param>
        /// <param name="sendOnDate">
        /// when to send the emails
        /// </param>
        /// <param name="currentModule">
        /// Module enum indicating the current module to send the email from
        /// </param>
        /// <param name="employeeIsLockedEmail">
        /// Send employee is locked email
        /// </param>
        /// <param name="fromMobile">
        /// Whether the request came from a mobile device or not - this changes the URL in the content of the email.
        /// </param>
        /// <param name="requestSource">
        /// The source of the password reset request, i.e. CorpD, Expenses360 or web
        /// </param>
        public void SendPasswordKey(int employeeID, PasswordKeyType passwordKeyType, DateTime? sendOnDate, Modules currentModule, bool employeeIsLockedEmail = false, bool fromMobile = false, string requestSource = "")
        {
            string passwordKey = this.AddPasswordKey(employeeID, passwordKeyType, sendOnDate);

            Employee reqemp = this.GetEmployeeById(employeeID);

            cAccounts clsaccounts = new cAccounts();
            cAccount account = clsaccounts.GetAccountByID(accountid);
            cMisc clsMisc = new cMisc(this.accountid);
            var reqGlobalProperties = clsMisc.GetGlobalProperties(this.accountid);
            cEmailTemplates clsemails = new cEmailTemplates(this.accountid, employeeID, passwordKey, reqGlobalProperties.attempts, currentModule);

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.accountid);
            cAccountProperties clsAccountProperties = (reqemp.DefaultSubAccount >= 0) ? clsSubAccounts.getSubAccountById(reqemp.DefaultSubAccount).SubAccountProperties : clsSubAccounts.getFirstSubAccount().SubAccountProperties;

            string emailFrom;
            if (string.IsNullOrEmpty(clsAccountProperties.EmailAdministrator) == false && string.IsNullOrWhiteSpace(clsAccountProperties.EmailAdministrator) == false && clsAccountProperties.SourceAddress == (byte)1) // coming from server address
            {
                emailFrom = clsAccountProperties.EmailAdministrator;
            }
            else
            {
                emailFrom = reqemp.EmailAddress;
            }

            try
            {                        
                Guid templateId;
                var clsemps = new cEmployees(account.accountid);
                var senderId = clsemps.getEmployeeidByEmailAddress(account.accountid, emailFrom);
                int[] recipients = { employeeID };

                if (employeeIsLockedEmail)
                {
                    templateId = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenAnEmployeeGetsLockedOut);
                    clsemails.SendMessage(templateId, senderId, recipients);
                    templateId = SendMessageEnum.GetEnumDescription(fromMobile ? SendMessageDescription.SentWhenAnEmployeeLocksAccountFromExpensesMobile : SendMessageDescription.SentWhenAnEmployeeLocksAccountFromAMethodOtherThanAMobileDevice);
                    clsemails.SendMessage(templateId, senderId, recipients);
                }
                else
                {
                    templateId = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenAnEmployeeRequestsAPasswordReset);
                    clsemails.SendMessage(templateId, senderId, recipients);

                    if (fromMobile)
                    {
                        templateId = requestSource == "Expenses360"
                                        // Forgotten password Email template for Expenses Mobile
                                         ? SendMessageEnum.GetEnumDescription(
                                             SendMessageDescription
                                               .SentWhenAnEmployeeRequestsAPasswordResetFromExpensesMobile)
                                         : SendMessageEnum.GetEnumDescription(
                                             SendMessageDescription
                                               .SentWhenAnEmployeeRequestsAPasswordResetFromAMobileDevice);
                    }
                    else
                    {
                        templateId = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenAnEmployeeRequestsAPasswordResetFromAMethodOtherThanAMobileDevice);
                    }

                    clsemails.SendMessage(templateId, senderId, recipients);
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + "----" + ex.InnerException);
            }
        }

        /// <summary>
        /// Send a welcome email to a new user.
        /// </summary>
        /// <param name="senderId">
        /// The sender id.
        /// </param>
        /// <param name="recipientId">
        /// The recipient id.
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="fromActivation">
        /// True if from activation.
        /// </param>
        public void SendWelcomeEmail(int senderId, int recipientId, ICurrentUser user, bool fromActivation = false)
        {
            var emails = new cEmailTemplates(user);
            try
            {
                var empId = new int[1];
                empId[0] = recipientId;
                    emails.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToANewEmployeeAfterTheyHaveBeenAdded), senderId, empId, recipientId, fromActivation);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " -- " + ex.InnerException);
            }
        }

        /// <summary>
        /// checkEmailIsUnique
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <returns>UniqueEmployeeCheck with information for the specified email address</returns>
        internal static UniqueEmployeeCheck CheckEmailIsUnique(string email)
        {
			var result = new UniqueEmployeeCheck(ForgottenDetailsResponse.EmailNotFound, 0, 0);
			
        	bool employeeFound = false;
			const string strsql = "select employeeid from employees where email = @email";
            var dbsChecked = new List<string>();

			var fields = new cFields();

			var emailFieldSize = fields.GetFieldSize("employees", "email");

            foreach (cAccount account in cAccounts.CachedAccounts.Values.Where(account => !dbsChecked.Contains(account.dbname)))
            {
                dbsChecked.Add(account.dbname);
                if (employeeFound && result.IsUnique == ForgottenDetailsResponse.EmailNotFound)
                {
                    break;
                }

                if (account.archived == false)
                {
                    var conn = new DBConnection(cAccounts.getConnectionString(account.accountid));
					conn.AddWithValue("@email", email, emailFieldSize);

                    using (IDataReader reader = conn.GetReader(strsql))
                    {
                        while (reader.Read())
                        {
                            if (employeeFound)
                            {
                                result.IsUnique = ForgottenDetailsResponse.EmailNotUnique;
                                break;
                            }

                            result.IsUnique = ForgottenDetailsResponse.EmployeeDetailsSent;
                            result.AccountID = account.accountid;
                            result.EmployeeID = reader.GetInt32(0);

                            employeeFound = true;
                        }

                        reader.Close();
                    }
                }
            }
        	
            return result;
        }

		internal struct UniqueEmployeeCheck
		{
            private ForgottenDetailsResponse _isUnique;
			private int _accountID;
			private int _employeeID;

            internal UniqueEmployeeCheck(ForgottenDetailsResponse isUnique, int accountID, int employeeID)
			{
				_isUnique = isUnique;
				_accountID = accountID;
				_employeeID = employeeID;
			}

			#region Properties

			internal ForgottenDetailsResponse IsUnique
			{
				get { return _isUnique; }
				set { _isUnique = value; }
			}

			internal int AccountID
			{
				get { return _accountID; }
				set { _accountID = value; }
			}

			internal int EmployeeID
			{
				get { return _employeeID; }
				set { _employeeID = value; }
			}

			#endregion Properties
		}

        /// <summary>
        /// Get an employee record by their email address
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <returns>CurrentUser collection of required employee</returns>
        public static CurrentUser getEmployeeidByEmail(string email)
        {
            int employeeid = 0;
            CurrentUser user = new CurrentUser { AccountID = 0, EmployeeID = 0 };

            // don't proceed any further if the email address provided is blank
            if (email.Trim() == string.Empty)
            {
                return user;
            }

            foreach (cAccount account in cAccounts.CachedAccounts.Values)
            {
                if (account.archived)
                {
                    continue;
                }

                cFields fields = new cFields(account.accountid);

                DBConnection conn = new DBConnection(cAccounts.getConnectionString(account.accountid));
                conn.AddWithValue("@email", email, fields.GetFieldSize("employees", "email"));

                const string strsql = "select employeeid from employees where email = @email";

                using (IDataReader reader = conn.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) == false)
                        {
                            employeeid = reader.GetInt32(0);
                        }
                    }

                    reader.Close();
                }

                conn.sqlexecute.Parameters.Clear();

                if (employeeid <= 0) continue;

                user.AccountID = account.accountid;
                user.EmployeeID = employeeid;
                return user;
            }
            return user;
        }
 
        public string createGrid(bool includeSubAccountDescField = false)
        {
            string strsql = "SELECT employees.employeeid, employees.username, employees.archived, employees.title, employees.firstname, employees.surname, groups.groupname, dbo.getEmpDepartmentSplit(employees.employeeid) as department, dbo.getEmpCostcodeSplit(employees.employeeid) as costcode, employees.locked, employees.active  ";

            if (includeSubAccountDescField)
            {
                strsql += ", accessRoles.roleName, accountsSubAccounts.Description ";
            }
            strsql += "FROM employees";
            return strsql;
        }

        public DataSet generateUnallocatedCardGrid(string surname, int accountID, int roleid, int groupid, int costcodeid, int departmentid, byte filter, string username, int employeeid, IDBConnection connection = null)
        {
            DataSet rcdstemployees;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                rcdstemployees = new DataSet();

            surname += "%";
            username += "%";

            string strsql = "SELECT employees.employeeid, employees.username, employees.archived, employees.surname + ',' + employees.title + ' ' + employees.firstname as Name, groups.groupname, dbo.getEmpDepartmentSplit(employees.employeeid) as department, dbo.getEmpCostcodeSplit(employees.employeeid) as costcode FROM employees left join groups on groups.groupid = employees.groupid where employees.username not like 'admin%'";

            if (username != "%")
            {
                strsql += " and employees.username LIKE @username";
                    expdata.AddWithValue("@username", username, this._fields.GetFieldSize("employees", "username"));
            }

            if (surname != "%")
            {
                strsql += " and employees.surname LIKE @surname";
                    expdata.AddWithValue("@surname", surname, this._fields.GetFieldSize("employees", "surname"));
            }

            if (employeeid != 0)
            {
                strsql += " and employees.employeeid = @employeeid";
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
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

            strsql = strsql + " ORDER BY employees.username";
                rcdstemployees = expdata.GetDataSet(strsql);
            }

            return rcdstemployees;
        }

        public string generateGrid(string surname, int accountID, int roleid, int groupid, int costcodeid, int departmentid, byte filter, string username, int employeeid, IDBConnection connection = null)
        {
            string strsql;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

            surname += "%";
            username += "%";

                strsql = "SELECT employees.employeeid, employees.username, employees.archived, employees.surname + ',' + employees.title + ' ' + employees.firstname as Name, groups.groupname, dbo.getEmpDepartmentSplit(employees.employeeid) as department, dbo.getEmpCostcodeSplit(employees.employeeid) as costcode FROM employees left join groups on groups.groupid = employees.groupid where employees.username not like 'admin%'";

            if (username != "%")
            {
                strsql += " and employees.username LIKE @username";
                    expdata.AddWithValue("@username", username, this._fields.GetFieldSize("employees", "username"));
            }

            if (surname != "%")
            {
                strsql += " and employees.surname LIKE @surname";
                    expdata.AddWithValue("@surname", surname, this._fields.GetFieldSize("employees", "surname"));
            }

            if (employeeid != 0)
            {
                strsql += " and employees.employeeid = @employeeid";
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
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
            }

            strsql = strsql + " ORDER BY employees.username";

            return strsql;
        }

        public int getCount(int accountID, IDBConnection connection = null)
        {
            int? count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                count = (int?)this.Cache.Get(accountID, "employee", "count");

			if (count == null)
			{
                    const string strsql = "select count(*) from employees";
                    count = expdata.ExecuteScalar<int>(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                this.Cache.Add(accountID, "employee", "count", count);
			}
            }

			return (int)count;
        }


            public AuthenicationOutcome Authenticate(string username, string password, AccessRequestType accessRequestType, bool fromSso = false, IDBConnection connection = null)
        {
            int employeeId;
                var authOutcome = new AuthenicationOutcome();

            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                employeeId = 0;
                var active = false;
                var archived = false;
                var allowWebsiteAccess = 0;
                var allowMobileAccess = 0;
                var allowApiAccess = 0;

                expdata.AddWithValue("@username", username, this._fields.GetFieldSize("employees", "username"));
                const string Sql = "SELECT employees.employeeID, active, archived, password, passwordMethod, max(CAST(accessRoles.allowApiAccess as int)) as allowApiAccess, Max(CAST(accessRoles.allowMobileAccess as INT)) as allowMobileAccess, MAX(CAST(accessRoles.allowWebsiteAccess as INT))  as allowWebsiteAccess FROM dbo.employees  left join employeeAccessRoles on employeeAccessRoles.employeeID = employees.employeeid left join accessRoles on accessRoles.roleID = employeeAccessRoles.accessRoleID WHERE username=@username group by employees.employeeid, employees.active, employees.archived, employees.password, employees.passwordMethod";

                using (IDataReader reader = expdata.GetReader(Sql))
                {
                    var pwdMethod = PasswordEncryptionMethod.RijndaelManaged;
                    var passwordInDB = string.Empty;

                while (reader.Read())
                {
                        employeeId = reader.GetInt32(reader.GetOrdinal("employeeID"));
                        active = reader.GetBoolean(reader.GetOrdinal("active"));
                        if (!active)
                        {
                            continue;
                        }

                        archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                        if (archived)
                        {
                            continue;
                        }

                    passwordInDB = reader.IsDBNull(reader.GetOrdinal("password"))
                                       ? string.Empty
                                       : reader.GetString(reader.GetOrdinal("password"));
                    pwdMethod = PasswordEncryptionMethod.RijndaelManaged;

                    if (!reader.IsDBNull(reader.GetOrdinal("passwordMethod")))
                    {
                        pwdMethod = (PasswordEncryptionMethod)reader.GetByte(reader.GetOrdinal("passwordMethod"));
                    }

                    allowApiAccess = reader.IsDBNull(reader.GetOrdinal("AllowApiAccess")) ? 0 : reader.GetInt32(reader.GetOrdinal("AllowApiAccess"));
                    allowMobileAccess = reader.IsDBNull(reader.GetOrdinal("AllowMobileAccess")) ? 0 : reader.GetInt32(reader.GetOrdinal("AllowMobileAccess"));
                    allowWebsiteAccess = reader.IsDBNull(reader.GetOrdinal("AllowWebsiteAccess")) ? 0 : reader.GetInt32(reader.GetOrdinal("AllowWebsiteAccess"));

                }

                reader.Close();

                    expdata.sqlexecute.Parameters.Clear();

                    if (active == false || archived == true)
                    {
                            authOutcome.employeeId = 0 - employeeId;
                            authOutcome.LoginResult = LoginResult.InactiveAccount;
                            return authOutcome;
                    }

                bool passwordMatch = false;
                bool updatePassword = false;

                    if (employeeId > 0 && fromSso)
                {
                            authOutcome.employeeId = employeeId;
                            authOutcome.LoginResult = LoginResult.Success;

                            return authOutcome;
                }

                if (string.IsNullOrEmpty(passwordInDB) && string.IsNullOrEmpty(password))
                {
                    passwordMatch = true;
                }
                else
                {
                    switch (pwdMethod)
                    {
                        case PasswordEncryptionMethod.FWBasic:
                            if (cPassword.Crypt(password, "2") == passwordInDB)
                            {
                                passwordMatch = true;
                                updatePassword = true;
                            }

                            break;
                        case PasswordEncryptionMethod.Hash:
                        case PasswordEncryptionMethod.MD5:
                                if (
                                    System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(
                                password, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString())
                                == passwordInDB)
                            {
                                passwordMatch = true;
                                updatePassword = true;
                            }

                            break;
                        case PasswordEncryptionMethod.RijndaelManaged:
                                var clsSecureData = new cSecureData();
                            if (clsSecureData.Encrypt(password) == passwordInDB)
                            {
                                passwordMatch = true;
                            }


                            break;
                        case PasswordEncryptionMethod.ShaHash:
                            if (cPassword.SHA_HashPassword(password) == passwordInDB)
                            {
                                passwordMatch = true;
                                updatePassword = true;
                            }

                            break;
                    }

                    Employee employee = this.GetEmployeeById(employeeId);

                    if (!this.ValidateAccessRequestType(accessRequestType, allowWebsiteAccess, allowMobileAccess,
                            allowApiAccess) && !employee.AdminOverride)
                    {
                                authOutcome.employeeId = 0 - employeeId;
                                authOutcome.LoginResult = LoginResult.EmployeeHasInsufficientPermissions;

                                return authOutcome;
                    }

                    if (updatePassword)
                    {
                                var clsSecureData = new cSecureData();
                            employee.Password = clsSecureData.Encrypt(password);
                            employee.PasswordMethod = PasswordEncryptionMethod.RijndaelManaged;
                        employee.Save(null);
                    }
                    else if (employeeId > 0 && !passwordMatch)
                    {
                        employee.LogonRetryCount++;
                        employee.Save(null);
                                authOutcome.employeeId = 0 - employeeId;
                                authOutcome.LoginResult = LoginResult.InvalidUsernamePasswordCombo;

                                return authOutcome;
                    }

                    if (employeeId > 0 && passwordMatch)
                    {
                        // set retry count to zero and register logon in the logon-stats compilation table
                        employee.LogonRetryCount = 0;
                        employee.Save(null);
                            expdata.sqlexecute.Parameters.Clear();
                            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                            expdata.ExecuteProc("RegisterLogon");
                    }
                }
            }
            }
                authOutcome.employeeId = employeeId;

                return authOutcome;
        }

        private bool ValidateAccessRequestType(AccessRequestType accessRequestType, int allowWebsiteAccess, int allowMobileAccess, int allowApiAccess)
        {
            switch (accessRequestType)
            {
                case AccessRequestType.Website:
                    if (allowWebsiteAccess == 0)
                    {
                        return false;
                    }
                    break;
                case AccessRequestType.Mobile:
                    if (allowMobileAccess == 0)
                    {
                        return false;
                    }
                    break;
                case AccessRequestType.Api:
                    if (allowApiAccess == 0)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// GetEmployeeRetryCount : Gets the current logon attempt count for an employee
        /// </summary>
        /// <param name="employeeID">ID of employee to get retry count for</param>
        /// <returns>Current number of attempts made</returns>
        public int GetEmployeeRetryCount(int employeeID)
        {
            Employee emp = GetEmployeeById(employeeID);
            return emp != null ? emp.LogonRetryCount : 0;
        }

        /// <summary>
        /// Lock an employee by their username
        /// </summary>
        /// <param name="username">
        /// Username of employee to lock
        /// </param>
        /// <param name="accountID">
        /// The account id of employee
        /// </param>
        /// <param name="activeModule">
        /// An instance of <see cref="Modules"/>
        /// </param>
        /// <param name="connection">
        /// An instance of <see cref="IDBConnection"/>
        /// </param>
        /// <param name="fromMobile">
        /// Whether the request is from a mobile device.
        /// </param>
        public void lockEmployee(string username, int accountID, Modules activeModule, IDBConnection connection = null, bool fromMobile = false)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            }
            int employeeID = getEmployeeidByUsername(accountID, username);

            Employee employee = this.GetEmployeeById(employeeID);
       
            employee.ChangeLockedStatus(true, null); 

            //prevents archived user(s) from initiating the password reset procedure. 
              if (employee.Archived == false)
            {  
                SendPasswordKey(employeeID, PasswordKeyType.ForgottenPassword, null, activeModule, true, fromMobile);
            }
        }

        public DataSet getDirectoryGrid(int accountID, string letter, IDBConnection connection = null)
        {
            DataSet ds;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                int surnameLength = this._fields.GetFieldSize("employees", "surname");
            string strsql;
            if (letter != "X")
            {
                strsql = "select employeeid, username, title, firstname, surname, extension from employees where archived = 0 and surname like @surname order by surname, firstname";
            }
            else
            {
                strsql = "select employeeid, username, title, firstname, surname, extension from employees where archived = 0 and (surname like @surname or surname like @surname1 or surname like @surname2) order by surname, firstname";
                    expdata.AddWithValue("@surname1", "y%", surnameLength);
                    expdata.AddWithValue("@surname2", "z%", surnameLength);
            }

                expdata.AddWithValue("@surname", letter + "%", surnameLength);
                ds = expdata.GetDataSet(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }

            return ds;
        }

        public int getEmployeeidByEmailAddress(int accountID, string email, IDBConnection connection = null)
        {
            int employeeid;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            if (email.Trim() == string.Empty)
            {
                return 0;
            }

                employeeid = 0;
                expdata.AddWithValue("@email", email, this._fields.GetFieldSize("employees", "email"));

            const string SQL = "select employeeid from employees where email = @email";
                using (IDataReader reader = expdata.GetReader(SQL))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(reader.GetOrdinal("employeeid")) == false)
                    {
                        employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    }
                }
                reader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return employeeid;
        }

        public virtual int getEmployeeidByUsername(int accountID, string username, IDBConnection connection = null)
        {
            int employeeid;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                employeeid = 0;
                expdata.AddWithValue("@username", username, this._fields.GetFieldSize("employees", "username"));

            const string strsql = "select employeeid from employees where username = @username";
                using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        employeeid = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return employeeid;
        }

        /// <summary>
        /// THIS METHOD RETRIEVE EMPLOYEE ID BY MATCHING assignmentNumber AGAINST employees.payroll
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="assignmentNum"></param>
        /// <returns></returns>
        public int getEmployeeidByAssignment(int accountid, string assignmentNum, IDBConnection connection = null)
        {
            int employeeid;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                employeeid = 0;
            
                expdata.AddWithValue("@assignmentNum", assignmentNum, this._fields.GetFieldSize("employees", "payroll"));

            const string strsql = "select employeeid from employees where payroll = LEFT(@assignmentNum, 8)";
                using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        employeeid = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return employeeid;
        }

        /// <summary>
        /// This method retrieves the employeeid by matching the esr assignment number against the esr_assignments table
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="assignmentNum"></param>
        /// <returns></returns>
        public int getEmployeeidByAssignmentNumber(int accountid, string assignmentNum, IDBConnection connection = null)
        {
            int employeeid;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                employeeid = 0;

                expdata.AddWithValue("@assignmentNum", assignmentNum, 30);

                using (IDataReader reader = expdata.GetReader("getEsrAssignmentsEmployeeId", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        employeeid = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return employeeid;
        }

        public bool alreadyExists(string username, int action, int employeeid, int accountid, IDBConnection connection = null)
        {
            int count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            string strsql;
            if (action == 0)
            {
                strsql = "select count(*) from employees where username = @username";
            }
            else
            {
                strsql = "select count(*) from employees where username = @username and employeeid <> @employeeid";
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            }
                expdata.AddWithValue("@username", username, this._fields.GetFieldSize("employees", "username"));

                count = expdata.ExecuteScalar<int>(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            return count != 0;
        }

        /// <summary>
        /// This method send the email to the employees whose account is activated through web/mobile/ESR
        /// </summary>
        /// <param name="employeeId">ID of the employee whose account is activated</param>
        /// <param name="fromMobile">Boolean value which determine if the activation is done through Mobile</param>
        private void SendActivatedEmail(int employeeId)
        {
            var employee = this.GetEmployeeById(employeeId);
            var currentUser = cMisc.GetCurrentUser();
            var emails = HttpContext.Current != null ? new cEmailTemplates(currentUser) : new cEmailTemplates(employee.AccountID, 0, string.Empty, 0, GlobalVariables.DefaultModule, true);
            var templateId =SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnEmployeeAfterTheirAccountHaveBeenActivated);
            var subAccounts = currentUser != null ? new cAccountSubAccounts(currentUser.AccountID): new cAccountSubAccounts(employee.AccountID);
            var properties = currentUser != null ? subAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties:subAccounts.getSubAccountById(subAccounts.getFirstSubAccount().SubAccountID).SubAccountProperties;
            var senderId = properties.MainAdministrator > 0 ? properties.MainAdministrator : currentUser.EmployeeID;

            try
            {
                int[] recipients = { employeeId };
                emails.SendMessage(templateId, senderId, recipients);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " -- " + ex.InnerException);
            }
        }

        
        public byte checkpassword(string password, int accountID, int employeeid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            //return codes
            //0 = password fine
            //1 = password is incorrect length
            //2 = capital letter
            //3 = number
            //4 = previous
            //5 = symbol

            Employee reqEmployee = this.GetEmployeeById(employeeid);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountID);
            cAccountProperties clsproperties;
            if (reqEmployee != null && reqEmployee.EmployeeID > 0 && reqEmployee.DefaultSubAccount >= 0)
            {
                clsproperties = subaccs.getSubAccountById(reqEmployee.DefaultSubAccount).SubAccountProperties;
            }
            else
            {
                clsproperties = subaccs.getFirstSubAccount().SubAccountProperties;
            }

            bool isPrevious = false;
            PasswordLength plength = clsproperties.PwdConstraint;
            if (plength != PasswordLength.AnyLength)
            {
                int length1 = clsproperties.PwdLength1;
                int length2 = clsproperties.PwdLength2;
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

            if (clsproperties.PwdMustContainUpperCase)
            {
                if (password.ToLower() == password)
                {
                    return 2;
                }
            }

            if (clsproperties.PwdMustContainNumbers)
            {
                var containsNumber = Regex.Match(password, "[0-9]", RegexOptions.IgnoreCase).Success;

                if (containsNumber == false)
                {
                    return 3;
                }
            }
            if (clsproperties.PwdMustContainSymbol)
            {
                var containsSymbol = Regex.Match(password, "[^a-z0-9 ]", RegexOptions.IgnoreCase).Success;

                if (containsSymbol == false)
                {
                    return 5;
                }
            }
            if (clsproperties.PwdHistoryNum != 0)
            {
                if (employeeid != 0)
                {
                        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                    const string strsql = "select password from previouspasswords where employeeid = @employeeid";
                    string oldPassword;

                        using (IDataReader prevreader = expdata.GetReader(strsql))
                    {
                        switch (reqEmployee.PasswordMethod)
                        {
                            case PasswordEncryptionMethod.FWBasic:
                                oldPassword = cPassword.Crypt(password, "2");
                                break;
                            case PasswordEncryptionMethod.Hash:
                            case PasswordEncryptionMethod.MD5:
                                oldPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
                                break;
                            case PasswordEncryptionMethod.RijndaelManaged:
                                cSecureData clsSecureData = new cSecureData();
                                oldPassword = clsSecureData.Encrypt(password);
                                break;
                            case PasswordEncryptionMethod.ShaHash:
                                oldPassword = cPassword.SHA_HashPassword(password);
                                break;
                            default:
                                oldPassword = string.Empty;
                                break;
                        }

                        while (prevreader.Read())
                        {
                            if (prevreader.GetString(0) == oldPassword)
                            {
                                isPrevious = true;
                                break;
                            }
                        }
                        prevreader.Close();
                    }

                    //Check against current password
                    if (reqEmployee.Password == oldPassword)
                    {
                        isPrevious = true;
                    }

                    if (isPrevious)
                    {
                        return 4;
                    }
                }
            }

                expdata.sqlexecute.Parameters.Clear();
            }
            return 0;
        }

        public ListItem[] createProxyDropDown(int employeeid, IDBConnection connection = null)
        {
            List<ListItem> tempItems;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

                tempItems = new List<ListItem>();

            const string SQL = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees where archived = 0 and employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";

                using (IDataReader empreader = expdata.GetReader(SQL))
            {
                while (empreader.Read())
                {
                    tempItems.Add(new ListItem
                                      {
                                          Text = empreader.GetString(empreader.GetOrdinal("empname")),
                                          Value = empreader.GetInt32(empreader.GetOrdinal("employeeid")).ToString()
                                      });

                    if (empreader.GetInt32(empreader.GetOrdinal("employeeid")) == employeeid)
                    {
                        tempItems[tempItems.Count - 1].Selected = true;
                    }
                }

                empreader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return tempItems.ToArray();
        }

        //<summary>
        //Populate authoriser levels to fill drop-down list.
        //</summary>
        public List<ListItem> PopulateDropDownListAuthoriserLevel()
        {
            List<ListItem> tempItems;
            tempItems = new List<ListItem>();
            var user = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                using (var reader = expdata.GetReader("dbo.GetAllAuthoriserLevelDetails"))
                {
                    if (reader != null && ((System.Data.SqlClient.SqlDataReader)reader).HasRows)
                    {
                        while (reader.Read())
                        {
                            decimal amount = reader.GetDecimal(reader.GetOrdinal("Amount"));
                            int authoriserLevelId = reader.GetInt32(reader.GetOrdinal("AuthoriserLevelDetailId"));
                                var currency = GetCurrencySymbol(user, true);
                            var cmbBoxItem = string.Format("{0} {1}", currency, Math.Round(amount, 2).ToString());
                            tempItems.Add((new ListItem(cmbBoxItem, authoriserLevelId.ToString())));
                        }
                    }
                }
            }
            return tempItems;
        }
        /// <summary>
        /// Get Currency Symbol
        /// </summary>
        /// <param name="user">current user</param>
        /// <param name="shouldShowSymbol">shouldShowSymbol is boolean</param>
        /// <returns></returns>
            public string GetCurrencySymbol(CurrentUser user, bool shouldShowSymbol)
        {
          
            var subAccounts = new cAccountSubAccounts(user.AccountID);
            var properties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var currencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            var globalCurrencies = new cGlobalCurrencies();
            string currency = string.Empty;
            if (properties.BaseCurrency.HasValue && properties.BaseCurrency.Value != 0)
            {
                int baseCurrency = properties.BaseCurrency.Value;
                var currencyList = currencies.CreateDropDown(baseCurrency);

                foreach (var item in currencyList.Where(item => item.Selected))
                {
                    var currencySymbol = globalCurrencies.getGlobalCurrencyByLabel(item.Text);
                    if (shouldShowSymbol == true)
                    {
                        currency = currencySymbol.symbol.ToString();
                    }
                    else
                    {
                        currency = currencySymbol.label.ToString();
                    }
                    break;
                }
            }
            
            return currency;
        }

        /// <summary>
        /// Check Default Approver.
        /// </summary>
        /// <param name="authoriserLevelDetailId">Id of authoriser level to search for</param>
        /// <returns></returns>
        public bool CheckDefaultApprover(int authoriserLevelDetailId)
        {
            bool result = false;
            var user = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", authoriserLevelDetailId);
                using (var reader = expdata.GetReader("dbo.GetAuthoriserLimitByAuthoriserLevelId", CommandType.StoredProcedure))
                {
                    if (reader != null && ((System.Data.SqlClient.SqlDataReader)reader).HasRows)
                    {
                        while (reader.Read())
                        {
                            var amount = reader.GetDecimal(reader.GetOrdinal("Amount"));
                                if (amount < 0)
                            {
                                result = true;
                                
                            }
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Get authoriser level of employee
        /// </summary>
        /// <param name="employeeId">Employee id</param>
        /// <returns></returns>
        public int? GetAuthoriserLevelIdByEmployee(int employeeId)
        {
            int? authoriserLevelId = null;
            var user = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@EmployeeId", employeeId);
                using (var reader = expdata.GetReader("dbo.GetAuthoriserLevelIdByEmployee", CommandType.StoredProcedure))
                {
                    if (reader != null && ((System.Data.SqlClient.SqlDataReader)reader).HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("AuthoriserLevelDetailId")))
                            {
                                authoriserLevelId = reader.GetInt32(reader.GetOrdinal("AuthoriserLevelDetailId"));
                            }
                            
                        }
                    }
                }
            }
            return authoriserLevelId;
        }

        public DataSet getProxies(int employeeid, IDBConnection connection = null)
        {
            DataSet ds;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

            const string strsql = "select employees.employeeid, username, title + ' ' + firstname + ' ' + surname as [empname] from employees inner join employee_proxies on employees.employeeid = employee_proxies.proxyid where employee_proxies.employeeid = @employeeid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                ds = expdata.GetDataSet(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            return ds;
        }

        /// <summary>
        /// Gets a list of email addresses of delegates for the given employee
        /// </summary>
        /// <param name="lst">list of email recipients</param>
        /// <param name="employeeid">the id of the employee who's delegates are read for</param>
        /// <param name="connection">database connection</param>
        public void GetProxiesEmailAddress(ref EmailRecipients lst, int employeeid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                const string Sql = "select proxyid, email from employees inner join employee_proxies on employees.employeeid = employee_proxies.proxyid where employee_proxies.employeeid = @employeeid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

                using (IDataReader reader = expdata.GetReader(Sql))
                {
                    int emailAddressOrd = reader.GetOrdinal("email");
                    int employeeIdOrd = reader.GetOrdinal("proxyid");
                    while (reader.Read())
                    {
                       lst.Add(reader.GetInt32(employeeIdOrd), reader.GetString(emailAddressOrd));
                    }

                    reader.Close();
                }
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        public void assignProxy(int assigningid, int employeeid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", assigningid);
                expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

            string strsql = "select count(*) from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
                int count = expdata.ExecuteScalar<int>(strsql);
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
        }

        public void removeProxy(int removingid, int employeeid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            const string strsql = "delete from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", removingid);
                expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
        }
        }

        public bool CheckExpiry(Employee employee)
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            cAccountProperties clsproperties = employee.DefaultSubAccount >= 0 ? subaccs.getSubAccountById(employee.DefaultSubAccount).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

            return employee.PasswordExpired(clsproperties.PwdExpires, clsproperties.PwdExpiryDays);
        }
        
        public ValueList CreateVList(int accountID, IDBConnection connection = null)
        {
            ValueList list;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                list = new ValueList();
            
            const string strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees order by empname";

                using (IDataReader empreader = expdata.GetReader(strsql))
            {
                while (empreader.Read())
                {
                    string empname = empreader.GetString(empreader.GetOrdinal("empname"));
                    int employeeid = empreader.GetInt32(empreader.GetOrdinal("employeeid"));
                    list.ValueListItems.Add(employeeid, empname);
                }
                empreader.Close();
            }
                expdata.sqlexecute.Parameters.Clear();
            }
            return list;
        }

        public bool isCheckAndPayer(int accountID, int employeeid, IDBConnection connection = null)
        {
            int count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            const string strsql = "select count(*) from signoffs where groupid in (select groupid from groups) and ((signofftype = 2 and relid = @employeeid) or (holidaytype = 2 and holidayid = @employeeid))";

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                count = expdata.ExecuteScalar<int>(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }

            return count != 0;
        }

        public cColumnList CreateColumnList(int accountID, IDBConnection connection = null)
        {
            cColumnList list;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                list = new cColumnList();
            
            const string strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees order by empname";

                using (IDataReader empreader = expdata.GetReader(strsql))
            {
                while (empreader.Read())
                {
                    string empname = empreader.GetString(empreader.GetOrdinal("empname"));
                    int employeeid = empreader.GetInt32(empreader.GetOrdinal("employeeid"));

                    list.addItem(employeeid, empname);
                }
                empreader.Close();
            }
                expdata.sqlexecute.Parameters.Clear();
            }
            return list;
        }

        public ListItem[] getEmployeesBySpendManagementElement(SpendManagementElement element, int accountID, IDBConnection connection = null)
        {
            SortedList<string, int> tempitems;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

                tempitems = new SortedList<string, int>();
                cAccessRoles allAccessRoles = new cAccessRoles(this.accountid, cAccounts.getConnectionString(accountID));
            string sAllowedRoles = string.Empty;

            foreach (cAccessRole currentRole in allAccessRoles.AccessRoles.Values)
            {
                if (currentRole.ElementAccess.Keys.Contains((element)))
                {
                    sAllowedRoles += "accessRoleID = " + currentRole.RoleID + " OR ";
                }
            }

            if (sAllowedRoles != string.Empty)
            {
                sAllowedRoles = sAllowedRoles.Remove(sAllowedRoles.Length - 4, 4);
            }

            string strsql = @"SELECT [employees].employeeid, [surname] + ' ' + [title] + ', ' + firstname + ' [' + username + ']' AS empname 
                        FROM employees INNER JOIN employeeAccessRoles ON employeeAccessRoles.employeeID = employees.employeeid 
                        WHERE archived = 0 AND username NOT LIKE 'admin%' AND (" + sAllowedRoles + ")";

                using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (!tempitems.ContainsKey(reader.GetString(reader.GetOrdinal("empname"))))
                    {
                        tempitems.Add(reader.GetString(reader.GetOrdinal("empname")), reader.GetInt32(reader.GetOrdinal("employeeid")));
                    }
                }
                reader.Close();
            }
                expdata.sqlexecute.Parameters.Clear();
            }

            return tempitems.Select(i => new ListItem(i.Key, i.Value.ToString())).ToArray();
        }

        public ListItem[] CreateCheckPayDropDown(int employeeid, int accountID, IDBConnection connection = null)
        {
            List<ListItem> tempItems;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();

            int i = 0;
            
                tempItems = new List<ListItem>();
            
            const string SQL = "select DISTINCT(employees.employeeid), employees.[surname] + ', ' + employees.[title] + ' ' + employees.firstname as empname from  employees INNER JOIN employeeAccessRoles ON (employeeAccessRoles.employeeID = employees.employeeid) INNER JOIN accessRoleElementDetails ON (accessRoleElementDetails.roleID = employeeAccessRoles.accessRoleID AND accessRoleElementDetails.elementID = 14) AND employees.archived = 0  order by empname";

                using (IDataReader empreader = expdata.GetReader(SQL))
            {
                while (empreader.Read())
                {
                    tempItems.Add(new ListItem
                        {
                            Text = empreader.GetString(empreader.GetOrdinal("empname")),
                            Value = empreader.GetInt32(empreader.GetOrdinal("employeeid")).ToString()
                        });

                    if (empreader.GetInt32(empreader.GetOrdinal("employeeid")) == employeeid)
                    {
                        tempItems[tempItems.Count - 1].Selected = true;
                    }

                    i++;
                }
                empreader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }
            return tempItems.ToArray();
        }

        public ListItem[] CreateDropDown(int employeeid, bool checkRoles, IDBConnection connection = null)
        {
            List<ListItem> items;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
            CurrentUser currentUser = null;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                currentUser = cMisc.GetCurrentUser();
            }

                items = new List<ListItem>();
            string strsql = "select  distinct username, employees.employeeid, employees.[surname] + ', ' + employees.[title] + ' ' + employees.firstname as empname from employees, employeeAccessRoles where employees.archived = 0 and employees.username not like 'admin%' ";

            if (checkRoles && currentUser != null)
            {
                List<int> lstAccessRoles = currentUser.Employee.GetAccessRoles().GetBy(currentUser.CurrentSubAccountId);
                    expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
                if (lstAccessRoles != null)
                {
                    if (currentUser.HighestAccessLevel != AccessRoleLevel.EmployeesResponsibleFor)
                    {
                        string additionalClause = currentUser.GetAccessRoleWhereClause();
                        if (string.IsNullOrEmpty(additionalClause) == false)
                        {
                            strsql = strsql + " and " + additionalClause;
                        }
                    }
                }
                strsql += " or employees.employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";
            }

            strsql = strsql + " order by empname";

                using (IDataReader empreader = expdata.GetReader(strsql))
            {
                while (empreader.Read())
                {
                    items.Add(new ListItem(empreader.GetString(2) + " (" + empreader.GetString(0) + ")", empreader.GetInt32(1).ToString()));
                }
                empreader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }
            return items.ToArray();

        }

        public string createEmployeeControl(ref PlaceHolder placeHolder, string checkerIDCtl, string checkerByCtl, EmployeeAreaType empAreaType, bool labelRequired)
        {
            string script;
            int employeeCount = getCount(accountid);
            Label spanInput = new Label();
            Label lblCheckedBy = null;

            if (labelRequired)
            {
                lblCheckedBy = new Label();
                lblCheckedBy.Text = "Checked By";
                placeHolder.Controls.Add(lblCheckedBy);
            }

            TextBox txtCheckID = null;
            TextBox txtCheckBy = null;
            DropDownList cmbcheckedby = null;

            if (employeeCount > 50)
            {
                if (labelRequired)
                {
                    lblCheckedBy.AssociatedControlID = "txt" + checkerByCtl;
                }

                txtCheckID = new TextBox { ID = "txt" + checkerIDCtl };
                txtCheckID.Style.Add("display", "none");

                txtCheckBy = new TextBox { ID = "txt" + checkerByCtl, CssClass = "fillspan" };
                txtCheckBy.Attributes.Add("onblur", "checkEmployee(" + (byte)empAreaType + ");");
                txtCheckBy.Attributes.Add("onkeyup", "checkEmployee(" + (byte)empAreaType + ");");


                spanInput.CssClass = "inputs";
                spanInput.Controls.Add(txtCheckID);
                spanInput.Controls.Add(txtCheckBy);

                AutoCompleteExtender autocomp = new AutoCompleteExtender 
                { 
                    CompletionSetCount = 10, 
                    MinimumPrefixLength = 1, 
                    EnableCaching = true, 
                    ServicePath = "~/shared/webServices/svcAutocomplete.asmx", 
                    ServiceMethod = "getEmployeeNameAndUsername", 
                    TargetControlID = "txt" + checkerByCtl 
                };

                spanInput.Controls.Add(autocomp);
            }
            else
            {
                if (labelRequired)
                {
                    lblCheckedBy.AssociatedControlID = "cmb" + checkerByCtl;
                }

                cmbcheckedby = new DropDownList { ID = "cmb" + checkerByCtl, CssClass = "fillspan" };
                cmbcheckedby.Items.Add(new ListItem(string.Empty, "0"));
                cmbcheckedby.Items.AddRange(CreateDropDown(0, false));

                spanInput.CssClass = "inputs";
                spanInput.Controls.Add(cmbcheckedby);
            }

            placeHolder.Controls.Add(spanInput);

            if (employeeCount > 50)
            {
                script = "var txt" + checkerByCtl + " ='" + txtCheckBy.ClientID + "'; var txt" + checkerIDCtl + " ='" + txtCheckID.ClientID + "';";
                script += "var cmb" + checkerByCtl + " = null;";
            }
            else
            {
                script = "var txt" + checkerByCtl + " = null; var txt" + checkerIDCtl + " = null;";
                script += "var cmb" + checkerByCtl + " = '" + cmbcheckedby.ClientID + "';";
            }

            return script;
        }

        [Obsolete]
        public bool CheckAccessRole(AccessRoleType type, SpendManagementElement element, int employeeID, int subaccountID)
        {
            bool result = false;
            cAccessRoles clsAccessRoles = new cAccessRoles(accountid, cAccounts.getConnectionString(accountid));

            Employee employee = GetEmployeeById(employeeID);
            List<int> lstAccessRoles = employee.GetAccessRoles().GetBy(subaccountID);

            if (lstAccessRoles != null)
            {
                foreach (int val in lstAccessRoles)
                {
                    cAccessRole accessRole = clsAccessRoles.GetAccessRoleByID(val);

                    if ((accessRole.ElementAccess != null && accessRole.ElementAccess.ContainsKey(element) == true && ((type == AccessRoleType.Delete && accessRole.ElementAccess[element].CanDelete == true) || (type == AccessRoleType.Add && accessRole.ElementAccess[element].CanAdd == true) || (type == AccessRoleType.Edit && accessRole.ElementAccess[element].CanEdit == true) || (type == AccessRoleType.View && (accessRole.ElementAccess[element].CanView == true || accessRole.ElementAccess[element].CanAdd == true || accessRole.ElementAccess[element].CanDelete == true || accessRole.ElementAccess[element].CanEdit == true)))))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get a cEmployee record by its employee ID
        /// </summary>
        /// <param name="employeeid">
        /// Employee ID to obtain a record for
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// cEmployee class object
        /// </returns>
        public virtual Employee GetEmployeeById(int employeeid, IDBConnection connection = null)
        {
            Employee employee;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                employee = null;
            if (employeeid > 0)
            {
                    employee = Employee.Get(employeeid, this.accountid, expdata);
                }
                }
                return employee;
            }

            /// <summary>
            /// The get employee cost code.
            /// </summary>
            /// <param name="employeeID">
            /// The employee id.
            /// </param>
            /// <param name="accountID">
            /// The account id.
            /// </param>
            /// <param name="connection">
            /// The connection.
            /// </param>
            /// <returns>
            /// The <see cref="cCostCentreBreakdown"/>.
            /// </returns>
            public List<cCostCentreBreakdown> GetEmployeeCostCodeFromDatabase(int employeeID, int accountID, IDBConnection connection = null)
            {
                var costCentreBreakdowns = new List<cCostCentreBreakdown>();
                using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountID)))
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "GetCostCentreBreakdown";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@empId", employeeID);

                    using (IDataReader reader = databaseConnection.GetReader(SQL, CommandType.StoredProcedure))
                    {
                        while (reader.Read())
                        {
                            string department = reader.IsDBNull(reader.GetOrdinal("department")) == false ? reader.GetString(reader.GetOrdinal("department")) : string.Empty;
                            string costcode = reader.IsDBNull(reader.GetOrdinal("costcode")) == false ? reader.GetString(reader.GetOrdinal("costcode")) : string.Empty;
                            string projectcode = reader.IsDBNull(reader.GetOrdinal("Projectcode")) == false ? reader.GetString(reader.GetOrdinal("Projectcode")) : string.Empty;
                            var percentused = reader.IsDBNull(reader.GetOrdinal("percentused")) == false ? reader.GetInt32(reader.GetOrdinal("percentused")) : 0;
                            costCentreBreakdowns.Add(new cCostCentreBreakdown(department, costcode, projectcode, percentused));
                        }
                        reader.Close();
                    }
                }
                return costCentreBreakdowns;
            }

            /// <summary>
            /// The get employee address.
            /// </summary>
            /// <param name="employeeID">
            /// The employee id.
            /// </param>
            /// <param name="accountID">
            /// The account id.
            /// </param>
            /// <param name="queryFor">
            /// The query for.
            /// </param>
            /// <param name="connection">
            /// The connection.
            /// </param>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            public List<cEmployeeHomeAddress> GetEmployeeHomeAddressFromDatabase(int employeeID, int accountID, IDBConnection connection = null)
            {
                var employeeHomeAddresses = new EmployeeHomeAddresses(accountID, employeeID);
                return (from homelocation in employeeHomeAddresses.HomeLocations select homelocation into value let endDate = value.EndDate let startDate = value.StartDate let address = Address.Get(accountID, value.LocationID) let addressLine1 = address.Line1 let city = address.City let postCode = address.Postcode select new cEmployeeHomeAddress(startDate, endDate, addressLine1, city, postCode)).ToList();
            }

            /// <summary>
            /// The get employee work address.
            /// </summary>
            /// <param name="employeeID">
            /// The employee id.
            /// </param>
            /// <param name="accountID">
            /// The account id.
            /// </param>
            /// <param name="connection">
            /// The connection.
            /// </param>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            public List<cEmployeeHomeAddress> GetEmployeeWorkAddressFromDatabase(int employeeID, int accountID, IDBConnection connection = null)
            {
                var employeeWorkAddresses = new EmployeeWorkAddresses(accountID, employeeID);
                return (from worklocation in employeeWorkAddresses.WorkLocations select worklocation.Value into value let endDate = value.EndDate let startDate = value.StartDate let address = Address.Get(accountID, value.LocationID) let addressLine1 = address.Line1 let city = address.City let postCode = address.Postcode select new cEmployeeHomeAddress(startDate, endDate, addressLine1, city, postCode)).ToList();
            }

        /// <summary>
        /// Gets employee details which will be used while populating driving licence from portal
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="connection">Database connection</param>
        /// <returns>Employee details</returns>
        public EmployeesToPopulateDrivingLicence GetEmployeesToPopulateDrivingLicence(int accountId, IDBConnection connection = null)
        {
            var employeeDetails =new EmployeeDetailsToPopulateDrivingLicence().GetEmployeesDetailsToPopulateDrivingLicence(accountId,connection);
            return employeeDetails;
        }

        /// <summary>
        /// The employee cars.
        /// </summary>
        /// <param name="EmployeeID">
        /// The employee id.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="cCar"/>.
        /// </returns>
        public List<cEmployeeCar> GetEmployeeCarsFromDataBase(int employeeID, int accountID, IDBConnection connection = null)
            {
                var cars = new List<cEmployeeCar>();
                var employeeCars = new cEmployeeCars(accountID, employeeID);
                var currentUser = cMisc.GetCurrentUser();
                var vehicleEngineType = VehicleEngineType.GetAll(currentUser);

                foreach (var value in employeeCars.Cars)
                {
                    var currentvehicleEngineType = vehicleEngineType.FirstOrDefault(x => x.VehicleEngineTypeId.Value == value.VehicleEngineTypeId);
                    var vehicleType = currentvehicleEngineType == null ? "" : currentvehicleEngineType.Name;
                    var defaultuom = value.defaultuom;
                    if (value.employeeid > 0)
                    {
                        cars.Add(new cEmployeeCar(value.make, value.model, value.registration, vehicleType, value.EngineSize, defaultuom.ToString(), value.Approved, value.active));
                    }
                }
                return cars;
            }

            /// <summary>
            /// Gets the list of Employee Cars by it's id.
            /// </summary>
            /// <param name="employeeId">
            /// The employee id to get cars.
            /// </param>
            /// <param name="accountId">
            /// The account id.
            /// </param>
            /// <returns>
            /// The list of employee cars.
            /// </returns>
            public List<cCar> GetEmployeeCars(int employeeId, int accountId)
            {
                var employeeCars = new cEmployeeCars(accountId, employeeId);
                return employeeCars.Cars;
        }

        #region get functions


        #region Item Roles

        public Dictionary<int, cRoleSubcat> getResultantRoleSet(Employee employee)
        {
            //cEmployee reqemp = GetEmployeeById(employeeid);
            List<EmployeeItemRole> lstItemRoles = employee.GetItemRoles().ItemRoles;

            Dictionary<int, cRoleSubcat> roleset = new Dictionary<int, cRoleSubcat>();
            cItemRoles clsroles = new cItemRoles(accountid);

            foreach (EmployeeItemRole role in lstItemRoles)
            {
                cItemRole reqrole = clsroles.getItemRoleById(role.ItemRoleId);

                foreach (cRoleSubcat rolesubcat in reqrole.items.Values)
                {
                    if (roleset.ContainsKey(rolesubcat.SubcatId) == false) //doesn't exist so add it
                    {
                        roleset.Add(rolesubcat.SubcatId, rolesubcat);
                    }
                    else
                    {
                        if (rolesubcat.maximum > roleset[rolesubcat.SubcatId].maximum)
                        {
                            roleset[rolesubcat.SubcatId].maximum = rolesubcat.maximum;
                }

                        if (rolesubcat.receiptmaximum > roleset[rolesubcat.SubcatId].receiptmaximum)
                        {
                            roleset[rolesubcat.SubcatId].receiptmaximum = rolesubcat.receiptmaximum;
            }
                    }
                }
            }

            return roleset;
        }
        #endregion Item Roles


        #region ESR Assignments

        public Dictionary<int, cESRAssignment> GetESRAssignments(int employeeID)
        {
            cESRAssignments clsassignments = new cESRAssignments(_nAccountid, employeeID);
            return clsassignments.getAssignmentsAssociated();
        }

        #endregion ESR Assignments

        public int getPersonalMiles(int employeeid, IDBConnection connection = null)
        {
            int totalmiles;
            int businessmiles;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();

            int year;
            int month = 4;
            int day = 6;
            totalmiles = 0;
            businessmiles = 0;
            var cars = new svcCars();
            var years = cars.GetFinancialYears(employeeid);
            FinancialYear financialYear = null;
            foreach (ListItem item in years)
            {
                if (item.Selected)
                {
                    var allYears = FinancialYears.ActiveYears(cMisc.GetCurrentUser());
                    foreach (FinancialYear thisYear in allYears)
                    {
                        if (thisYear.FinancialYearID.ToString() == item.Value)
                        {
                            financialYear = thisYear;
                        }
                    }

                }
            }
            if (financialYear != null)
            {
                year = DateTime.Today.Year;
                var compareDate = new DateTime(1900, DateTime.Today.Month, DateTime.Today.Day);
                if (compareDate < financialYear.YearStart)
                {
                    year = year - 1;
                }
            }
            else
            {
                if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 3)
                {
                    year = DateTime.Today.Year - 1;
                }
                else
                {
                    year = DateTime.Today.Year;
                }
            }
            
            DateTime taxyearstart = new DateTime(year, month, day);
            cEmployeeCars clsEmpCars = new cEmployeeCars(this.accountid, employeeid);
            foreach (cCar car in clsEmpCars.Cars)
            {
                if (car.fuelcard)
                {
                    int x = 0;
                    var odoReadings = car.GetAllOdometerReadings();
                    while (x < odoReadings.Count)
                    {
                        if (odoReadings[x].datestamp >= taxyearstart)
                        {
                            break;
                        }

                        x++;
                    }
                    if (x < odoReadings.Count)
                    {
                        int startreading = odoReadings[x].oldreading;
                                int endreading = odoReadings[odoReadings.Count - 1].newreading;
                        totalmiles += endreading - startreading;

                        const string strsql = "select cast(isnull(sum(num_miles), 0) as decimal) from savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid where carid = @carid and (date between @startdate and getDate())";
                        expdata.sqlexecute.Parameters.AddWithValue("@carid", car.carid);
                        expdata.sqlexecute.Parameters.AddWithValue("@startdate", taxyearstart);
                        businessmiles += Convert.ToInt32(expdata.ExecuteScalar<decimal>(strsql));
                        expdata.sqlexecute.Parameters.Clear();
                        }
                    }
                }
            }

            int personalmiles = totalmiles - businessmiles;
            return personalmiles;
        }

        public decimal getBusinessMiles(int employeeid, DateTime startdate, DateTime enddate, IDBConnection connection = null)
        {
            decimal miles;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                miles = 0;
            const string strsql = "select sum(num_miles) from savedexpenses_journey_steps where expenseid in (select expenseid from savedexpenses where claimid in (select claimid from claims where employeeid = @employeeid) and (date between @startdate and @enddate))";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);
                using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        miles = reader.GetDecimal(0);
                    }
                }
                reader.Close();
            }
                expdata.sqlexecute.Parameters.Clear();
            }
            return miles;
        }


        #region odometer readings

        public cOdometerReading getOdometerReadingByID(int odometerID, IDBConnection connection = null)
        {
            cOdometerReading reading;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                reading = null;

                expdata.sqlexecute.Parameters.Clear();
            const string SQL = "SELECT * FROM odometer_readings WHERE odometerid = @odometerID";

                expdata.sqlexecute.Parameters.AddWithValue("@odometerID", odometerID);

                using (IDataReader reader = expdata.GetReader(SQL))
            {
                while (reader.Read())
                {
                    int carID = reader.GetInt32(reader.GetOrdinal("carid"));
                    DateTime dateStamp = reader.IsDBNull(reader.GetOrdinal("datestamp")) ? new DateTime(1900, 1, 1) : reader.GetDateTime(reader.GetOrdinal("dateStamp"));
                    int oldReading = reader.GetInt32(reader.GetOrdinal("oldreading"));
                    int newReading = reader.IsDBNull(reader.GetOrdinal("newreading")) ? 0 : reader.GetInt32(reader.GetOrdinal("newreading"));
                    DateTime createdOn = reader.IsDBNull(reader.GetOrdinal("createdon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                    int createdBy = reader.IsDBNull(reader.GetOrdinal("createdby")) ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));

                    reading = new cOdometerReading(odometerID, carID, dateStamp, oldReading, newReading, createdOn, createdBy);
                }
                reader.Close();
            }
            }

            return reading;
        }

        #endregion odometer readings

        #endregion

        public string getHolidayApprovers(int employeeid, IDBConnection connection = null)
        {
            StringBuilder output;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                output = new StringBuilder();
            string rowclass = "row1";
                cBudgetholders clsholders = new cBudgetholders(this.accountid);
                cTeams clsteams = new cTeams(this.accountid);

            output.Append("<table class=datatbl>");
            output.Append("<tr><th>Group Name</th><th>Temporary Approver</th></tr>");
            const string SQL = "select groupname, holidaytype, holidayid from signoffs inner join groups on groups.groupid = signoffs.groupid where " +
                                  "(signofftype = 1 and (select count(*) from budgetholders where employeeid = @employeeid and budgetholderid = signoffs.relid) <> 0) or " +
                                  "(signofftype = 2 and relid = @employeeid) or " +
                                  "(signofftype = 3 and (select count(*) from teamemps where teamid = signoffs.relid and employeeid = @employeeid) <> 0)";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

                using (IDataReader reader = expdata.GetReader(SQL))
            {
                while (reader.Read())
                {
                    output.Append("<tr>");
                    output.Append("<td class=\"" + rowclass + "\">" + reader.GetString(reader.GetOrdinal("groupname")) + "</td>");
                    output.Append("<td class=\"" + rowclass + "\">");
                    Employee reqemp;
                    switch (reader.GetInt32(reader.GetOrdinal("holidaytype")))
                    {
                        case 0:
                            output.Append("-");
                            break;
                        case 1:
                            cBudgetHolder holder = clsholders.getBudgetHolderById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                            reqemp = this.GetEmployeeById(holder.employeeid);
                            output.Append(reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname);
                            break;
                        case 2:
                            reqemp = this.GetEmployeeById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                            output.Append(reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname);
                            break;
                        case 3:
                            cTeam reqteam = clsteams.GetTeamById(reader.GetInt32(reader.GetOrdinal("holidayid")));
                            output.Append(reqteam.teamname + " team");
                            break;
                    }


                    output.Append("</td>");
                    output.Append("</tr>");

                    rowclass = rowclass == "row1" ? "row2" : "row1";

                }
                reader.Close();
            }
            }

            output.Append("</table>");
            return output.ToString();
        }

        #region User Defined Fields

        /// <summary>
        /// Obtain the list of user defined field values for the employee
        /// </summary>
        /// <param name="employeeID">Employee ID</param>
        /// <returns>List of udf values as "objects"</returns>
        public SortedList<int, object> GetUserDefinedFields(int employeeID)
        {
            SortedList<int, object> list = new SortedList<int, object>();

            cUserdefinedFields clsUserDefinedFields = new cUserdefinedFields(_nAccountid);
            cTables clsTables = new cTables(_nAccountid);
            cFields clsFields = new cFields(_nAccountid);

            cTable tblEmployees = clsTables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754")); // employees table GUID
            cTable tblEmployeesUDFs = tblEmployees.GetUserdefinedTable();

            list = clsUserDefinedFields.GetRecord(tblEmployeesUDFs, employeeID, clsTables, clsFields);

            return list;
        }

        #endregion User Defined Fields

        #region cars

        public string getCarGrid()
        {
            return "select carid,vehicletypeid, make, model, registration, startdate, enddate,cartypeid, active from cars";
        }

        public string getOdometerGrid()
        {
            return "SELECT odometerid, carid, datestamp, oldreading, newreading FROM odometer_readings";
        }

        #endregion

        #region notes

        public DataTable getNotes(int employeeid, IDBConnection connection = null)
        {
            DataTable tbl;
            DataSet ds;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();

                tbl = new DataTable();

            tbl.Columns.Add("datestamp", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("note", System.Type.GetType("System.String"));
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            const string strsql = "select * from notes where [read] = 0 and employeeid = @employeeID order by datestamp desc";
                ds = expdata.GetDataSet(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            tbl = ds.Tables[0];

            return tbl;
        }

        public void markNoteAsRead(int noteid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();

            const string strsql = "update notes set [read] = 1 where noteid = @noteid";
                expdata.sqlexecute.Parameters.AddWithValue("@noteid", noteid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        public void clearNotes(int employeeid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
            const string strsql = "delete from notes where employeeid = @employeeid";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.ExecuteSQL(strsql);
            }
        }

        #endregion notes

        /// <summary>
        /// Get the current Mileage for the Employee in the "current" year as defined by the date parameter.  
        /// If the earliest date expense date (year) matches then the "MileageTotal" field from employees will be included in the total
        /// </summary>
        /// <param name="employeeid">The empoyee to enquire on</param>
        /// <param name="date">The date of the expense</param>
        /// <param name="connection">Optional: Database connection.</param>
        /// <returns>The mileage total for the current year for the employee id entered.</returns>
        public decimal getMileageTotal(int employeeid, DateTime date, IDBConnection connection = null)
        {
            decimal total;
            using (var dbConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                dbConnection.sqlexecute.Parameters.Clear();

                dbConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                dbConnection.sqlexecute.Parameters.AddWithValue("@expenseDate", date);
                dbConnection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                dbConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                dbConnection.ExecuteProc("getEmployeeMileageTotal");

                total = (int)dbConnection.sqlexecute.Parameters["@identity"].Value;
                dbConnection.sqlexecute.Parameters.Clear();
            }

            return total;
        }

        /// <summary>
        /// Save DVLA Lookup date and response code after the driving licence lookup is completed.
        /// </summary>
        /// <param name="employeeId">Employee Id of user for which the dvla lookup is performed</param>
        /// <param name="responseCode">Response code returned by DVLA. Repsone code will be blank on successfull lookup</param>
        /// <param name="lookupDate">Lookup date </param>
        /// <param name="connection">Db connection</param>
        public void SaveDvlaLookupInformation(int employeeId,string responseCode, DateTime? lookupDate, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@dvlaLookupDate", lookupDate);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@responseCode", responseCode);
                databaseConnection.ExecuteProc("SaveEmployeeDvlaLookUpInformation");
                User.CacheRemove(employeeId, this.accountid);
            }
        }

        /// <summary>
        /// Get the latest driving licence record updated through the Auto lookup previously 
        /// </summary>
        /// <param name="employeeId">Employee Id of user for which the dvla lookup is performed</param>
        /// <param name="drivingLicenceNumber">Driving licence number for which the lookup dvla lookup has made</param>
        /// <param name="connection">Db connection</param>
        public int GetLatestDrivingLicenceRecordForEmployee(int employeeId, string drivingLicenceNumber)
        {
            int recordid = 0;
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@drivingLicenceNumber", drivingLicenceNumber);
                using (var reader = databaseConnection.GetReader("GetLatestDrivingLicenceRecordForEmployee", CommandType.StoredProcedure))
                {
                    int recordIdOrd = reader.GetOrdinal("recordId");
                    while (reader.Read())
                    {
                        recordid = reader.GetInt32(recordIdOrd);
                    }
                }
                return recordid;
            }
        }

        /// <summary>
        /// Get employees based on notification type
        /// </summary>
        /// <param name="emailNotificationType">Email notification type</param>
        /// <param name="connection">Database connection</param>
        /// <returns>List of employeeid</returns>
        public List<int> GetEmployeeIdByNotificationType(int emailNotificationType,IDBConnection connection = null)
        {
            List<int> employeeid = new List<int>();
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@emailNotificationType", emailNotificationType);
                using (IDataReader reader = expdata.GetReader("GetEmployeeIdByNotificationType",CommandType.StoredProcedure))
                {
                    int employeeidOrd = reader.GetOrdinal("employeeid");
                    while (reader.Read())
                    {
                        employeeid.Add(reader.GetInt32(employeeidOrd));
                    }
                }
                expdata.sqlexecute.Parameters.Clear();
            }
            return employeeid;
        }

        /// <summary>
        /// Establish whether the start mileage and start mileage date fields are editable
        /// </summary>
        /// <param name="employeeid"></param>
        /// <returns></returns>
        public bool enableStartMileage(int employeeid, IDBConnection connection = null)
        {
            bool bEnable;
            int count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                bEnable = true;
                expdata.sqlexecute.Parameters.Clear();
                count = 0;

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                using (IDataReader reader = expdata.GetReader("select dbo.enableEmployeeMileageTotal(@employeeid)"))
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
            }
            }

            if (count > 0)
                bEnable = false;

            return bEnable;
        }

        public bool userIsOnHoliday(int employeeID, IDBConnection connection = null)
        {
            int count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
            const string sql = "select count(*) from holidays where employeeid = @employeeid and startdate < getDate() and enddate > getDate()";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
                count = expdata.ExecuteScalar<int>(sql);
                expdata.sqlexecute.Parameters.Clear();
            }

            return count != 0;
        }

        public void deleteOdometerReading(int employeeid, int carid, int odometerid, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@odometerID", odometerid);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser != null)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }

                expdata.ExecuteProc("dbo.deleteOdometerReading");
                expdata.sqlexecute.Parameters.Clear();
            }

            cEmployeeCars clsEmpCars = new cEmployeeCars(this.accountid, employeeid);
            cCar car = clsEmpCars.GetCarByID(carid);
        }

        public int saveOdometerReading(int odometerID, int employeeid, int carID, DateTime? dateStamp, int oldOdometer, int newOdometer, byte businessmiles, IDBConnection connection = null)
        {
            cEmployeeCars clsEmployeeCars;
            cCar car;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                clsEmployeeCars = new cEmployeeCars(this.accountid, employeeid);
                car = clsEmployeeCars.GetCarByID(carID);

            if (oldOdometer == 0)
            {
                cOdometerReading reading = car.getLastOdometerReading();
                if (reading != null)
                {
                    oldOdometer = reading.newreading;
                }
            }

                expdata.sqlexecute.Parameters.AddWithValue("@odometerID", odometerID);
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);

                expdata.sqlexecute.Parameters.AddWithValue("@dateStamp", dateStamp ?? DateTime.Now);

                expdata.sqlexecute.Parameters.AddWithValue("@oldReading", oldOdometer);
                expdata.sqlexecute.Parameters.AddWithValue("@newReading", newOdometer);
            if (businessmiles == 2)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@businessMileage", DBNull.Value);
            }
            else
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@businessMileage", businessmiles);
            }

                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser != null)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.saveOdometerReading");
                odometerID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

                expdata.sqlexecute.Parameters.Clear();
            }

            clsEmployeeCars = new cEmployeeCars(this.accountid, employeeid);
            car = clsEmployeeCars.GetCarByID(carID);

            return odometerID;
        }

        #region views

        public cUserView getUserView(int employeeid, UserView viewtype, bool printview)
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            Employee reqemp = GetEmployeeById(employeeid);
            cAccountProperties clsproperties = (reqemp.DefaultSubAccount >= 0) ? subaccs.getSubAccountById(reqemp.DefaultSubAccount).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

            cUserView view = reqemp.GetViews().GetBy(viewtype);

            if (view != null && view.fields.Count > 0)
            {
            	return view;
            }
			
			bool defaultview, checkForUserViews = !(view != null && view.fields.Count == 0);
			
            SortedList<int, cField> fieldsforview = getFieldsForView(printview, employeeid, viewtype, out defaultview, checkForUserViews);

            if (defaultview || viewtype == UserView.CurrentPrint || viewtype == UserView.PreviousPrint || viewtype == UserView.SubmittedPrint || viewtype == UserView.CheckAndPayPrint)
            {
            	view = new cUserView(accountid, employeeid, viewtype, printview, fieldsforview, generateViewSQL(viewtype, fieldsforview, printview, defaultview, clsproperties));
				if (checkForUserViews)
				{
                    reqemp.GetViews().Add(viewtype, new cUserView(accountid, employeeid, viewtype, printview, new SortedList<int, cField>(), string.Empty));
				}
            }
            else if (view == null)
            {
                view = new cUserView(accountid, employeeid, viewtype, printview, fieldsforview, generateViewSQL(viewtype, fieldsforview, printview, defaultview, clsproperties));
                reqemp.GetViews().Add(viewtype, view);
            }

            return view;
        }

        internal SortedList<int, cField> getFieldsForView(bool printview, int employeeid, UserView viewtype, out bool defaultview, bool checkUserViews, IDBConnection connection = null)
        {
            cViews clsViews;
            SortedList<int, cField> list;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
            defaultview = false;
                cFields clsfields = new cFields(this.accountid);
                clsViews = new cViews(this.accountid, employeeid);

                list = new SortedList<int, cField>();
                expdata.sqlexecute.Parameters.Clear();

            string strsql;
            bool useprintout = false;
            if (printview)
            {
                strsql = "select count(*) from print_views";

                    int count = expdata.ExecuteScalar<int>(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                if (count != 0)
                {
                    useprintout = true;
                }
            }

            if (useprintout)
            {
                SortedList<int, cField> lstPrintout = clsViews.getDefaultPrintView();

                foreach (KeyValuePair<int, cField> kvp in lstPrintout)
                {
                    list.Add(kvp.Key, kvp.Value);
                }
            }
            else if (checkUserViews)
            {
                strsql = "select fieldid from [views] where employeeid = @employeeid and viewid = @viewid order by [order]";

                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                    expdata.sqlexecute.Parameters.AddWithValue("@viewid", (int)viewtype);
                    using (IDataReader reader = expdata.GetReader(strsql))
                {
                        expdata.sqlexecute.Parameters.Clear();

                    int order = 1;
                    while (reader.Read())
                    {
                        Guid fieldid = reader.GetGuid(0);

                        cField reqfield = clsfields.GetFieldByID(fieldid);

                        if (reqfield != null)
                        {
                            list.Add(order, reqfield);
                        }
                        order++;
                    }
                    reader.Close();
                }
            }
            }

            if (list.Count == 0)
            {
                defaultview = true;

                SortedList<int, cField> lstDefault = clsViews.getDefaultView();

                foreach (KeyValuePair<int, cField> kvp in lstDefault)
                {
                    list.Add(kvp.Key, kvp.Value);
                }
            }
            return list;
        }

        private string generateViewSQL(UserView viewtype, SortedList<int, cField> lstfields, bool printview, bool defaultview, cAccountProperties clsproperties)
        {
            if (clsproperties.AllowMultipleDestinations) //remove from, to
            {
                for (int x = lstfields.Count - 1; x >= 0; x--)
                {
                    if (lstfields.Values[x].FieldID == new Guid("c75064ec-be87-4dd3-8299-d0d81ea3f819") || lstfields.Values[x].FieldID == new Guid("3d8c699e-9e0e-4484-b821-b49b5cb4c098"))
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
                table = "savedexpenses";
                expenseid = "savedexpenses.expenseid";
            }
            else
            {
                table = "savedexpenses";
                expenseid = "savedexpenses.expenseid";
            }
            output.Append("select ");
            output.Append(table + ".expenseid, " + table + ".basecurrency, " + table + ".currencyid as originalcurrency, globalbasecurrency, " + table + ".returned, " + table + ".transactionid, ");
            foreach (cField fld in lstfields.Values)
            {
                if (fld != null)
                {
                    tmpfields.Add(fld);

                    switch (fld.FieldID.ToString())
                    {
                        case "41340289-22f5-4efc-bf52-0755ff875ea7":
                            output.Append("dbo.getCostcodeSplit(" + expenseid + ") as [359dfac9-74e6-4be5-949f-3fb224b1cbfc], ");
                            break;
                        case "98193d41-34af-482e-b924-5f15ac34c48f":
                            output.Append("dbo.getDepartmentSplit(" + expenseid + ") as [9617a83e-6621-4b73-b787-193110511c17], ");
                            break;
                        case "ac887be1-536a-4076-a07d-08c621c5ff06":
                            output.Append("dbo.getProjectcodeSplit(" + expenseid + ") as [6d06b15e-a157-4f56-9ff2-e488d7647219], ");
                            break;
                        case "ec527561-dfee-48c7-a126-0910f8e031b0":
                            output.Append("countries.countryid as [ec527561-dfee-48c7-a126-0910f8e031b0], ");
                            break;
                        case "1ee53ae2-2cdf-41b4-9081-1789adf03459":
                            output.Append("currencies.currencyid as [1ee53ae2-2cdf-41b4-9081-1789adf03459], ");
                            break;
                        default:
                            if (fld.GetParentTable().TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8") || fld.GetParentTable().TableID == new Guid("fa59baa6-99c8-484d-a206-d4ecec4874f0") || fld.GetParentTable().TableID == new Guid("9c69deba-b2de-4836-8767-091da9cb7c79"))
                            {
                                switch (fld.FieldType)
                                {
                                    case "C":
                                        output.Append("round([" + table + "].[" + fld.FieldName + "],2) as [" + fld.FieldID + "], ");
                                        break;
                                    case "FD":
                                        if (table != "savedexpenses" && fld.FieldName.Contains("savedexpenses."))
                                        {
                                            output.Append(fld.FieldName.Replace("savedexpenses.", table + ".") + " as [" + fld.FieldID + "], ");
                                        }
                                        else
                                        {
                                            output.Append(fld.FieldName + " as [" + fld.FieldID + "], ");
                                        }
                                        break;
                                    default:
                                        output.Append("[" + table + "].[" + fld.FieldName + "] as [" + fld.FieldID + "], ");
                                        break;
                                }
                            }
                            else
                            {
                                if (fld.ValueList)
                                {
                                    if (fld.ListItems.Count > 0)
                                    {
                                        output.Append("[" + fld.FieldID + "] = CASE ");

                                        foreach (KeyValuePair<object, string> kvp in fld.ListItems)
                                        {
                                            output.Append(" WHEN [" + fld.GetParentTable().TableName + "].[" + fld.FieldName + "] = " + kvp.Key + " THEN '" + kvp.Value.Replace("'", "''") + "' ");
                                        }

                                        output.Append("END, ");
                                    }
                                    else
                                    {
                                        output.Append("''[" + fld.FieldID + "] AS '', ");
                                    }
                                }
                                else
                                {
                                    switch (fld.FieldType)
                                    {
                                        case "C":
                                            output.Append("round([" + fld.GetParentTable().TableName + "].[" + fld.FieldName + "],2) as [" + fld.FieldID + "], ");
                                            break;
                                        case "FC":
                                            output.Append("round(" + fld.GetParentTable().TableName + "." + fld.FieldName + ",2) as [" + fld.FieldID + "], ");
                                            break;
                                        default:
                                            output.Append("[" + fld.GetParentTable().TableName + "].[" + fld.FieldName + "] as [" + fld.FieldID + "], ");
                                            break;
                                    }
                                }
                            }
                            break;
                    }

                }
            }

            output.Remove(output.Length - 2, 2);

            output.Append(" FROM [" + table + "]");

            cJoins clsjoins = new cJoins(accountid);

            bool customview = defaultview || printview;
            output.Append(clsjoins.createJoinSQL(tmpfields, viewtype, customview));
            if (defaultview || printview)
            {
                
            }
            return output.ToString();
        }

        #endregion
        
        public string[] CreateMyDetailsCarGrid(CurrentUser currentUser)
        {
            cAccountSubAccounts clsAccountSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties clsProperties = clsAccountSubAccounts.getFirstSubAccount().SubAccountProperties;


            string sSQL = "SELECT cars.carid, cars.employeeid, cars.vehicletypeid, cars.make, cars.model, cars.registration, dbo.GetVehicleEngineType(cars.cartypeid), cars.enginesize, cars.default_unit, cars.active";
            if (clsProperties.ActivateCarOnUserAdd == false)
            {
                sSQL += ", cars.approved";
            }
           
            sSQL += " FROM dbo.cars";

            cGridNew gridEmployeeCars = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridEmployeeCars", sSQL);
            cFields clsFields = new cFields(currentUser.AccountID);

            gridEmployeeCars.KeyField = "carid";
            gridEmployeeCars.getColumnByName("carid").hidden = true;
            gridEmployeeCars.getColumnByName("employeeid").hidden = true;
            gridEmployeeCars.EmptyText = "You have no vehicles set up on your account.";

            gridEmployeeCars.enabledeleting = false;
            gridEmployeeCars.enableupdating = false;
            svcCars.AddVehicleTypes(ref gridEmployeeCars);
            
            ((cFieldColumn)gridEmployeeCars.getColumnByName("default_unit")).addValueListItem(0, "Miles");
            ((cFieldColumn)gridEmployeeCars.getColumnByName("default_unit")).addValueListItem(1, "Kilometres");

            gridEmployeeCars.addFilter(clsFields.GetFieldByID(new Guid("5DDBF0EF-FA06-4E7C-A45A-54E50E33307E")), ConditionType.Equals, new object[] { currentUser.EmployeeID }, null, ConditionJoiner.None);

            List<string> retVals = new List<string> { gridEmployeeCars.GridID };
            retVals.AddRange(gridEmployeeCars.generateGrid());
            return retVals.ToArray();
        }

        public bool isProxy(int employeeid, IDBConnection connection = null)
        {
            int count;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();

            const string strsql = "select count(*) from employee_proxies where proxyid = @proxyid";
                expdata.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
                count = expdata.ExecuteScalar<int>(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }

            return count != 0;
        }

        public void UpdateEmployeeModifiedOn(int editedEmployeeId, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", currentUser != null ? currentUser.EmployeeID : 0);
                expdata.sqlexecute.Parameters.AddWithValue("@editedEmployeeId", editedEmployeeId);
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                expdata.ExecuteProc("updateEmployeeModifiedOn");
                expdata.sqlexecute.Parameters.Clear();
            }

            //ResetCache(editedEmployeeId);
        }

        /// <summary>
        /// Sends an email to the account admin with details of an employee's requested changes to their details
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="requestedChanges">The requested changes</param>
        /// <param name="modules">The <see cref="cModules">cModules</see></param>
        /// <param name="reqProperties">The <see cref="cAccountProperties">cAccountProperties</see></param>
        /// <param name="emailSender">The <see cref="EmailSender">EmailSender</see></param>
        /// <returns></returns>
        public NotifyAdminOfChangesOutcome NotifyAdminOfChanges(ICurrentUser user, string requestedChanges, cModules modules, cAccountProperties reqProperties, EmailSender emailSender)
        {
            var employee = this.GetEmployeeById(user.EmployeeID);

            string changes =
                $"{employee.Title} {employee.Forename} {employee.Surname} with username {employee.Username}, has requested a change of their details in expenses.\n\n{requestedChanges}";

            string fromAddress;

            if (reqProperties.SourceAddress == 1)
            {
                fromAddress = (reqProperties.EmailAdministrator.Trim() == string.Empty ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
            }
            else
            {
                if (employee.EmailAddress != string.Empty)
                {
                    fromAddress = employee.EmailAddress;
                }
                else
                {
                    //If no email address set then send from admin
                    fromAddress = (reqProperties.EmailAdministrator.Trim() == string.Empty ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
                }
            }

            cModule module = modules.GetModuleByID((int)user.CurrentActiveModule);
            string brandName = (module != null) ? module.BrandNamePlainText : "Expenses";

            var adminEmployee = this.GetEmployeeById(reqProperties.MainAdministrator);
            MailMessage msg = new MailMessage(fromAddress, adminEmployee.EmailAddress, "Some of my details are incorrect in " + brandName + ".", changes);

            return emailSender.SendEmail(msg) ? NotifyAdminOfChangesOutcome.EmailSentSuccessfully : NotifyAdminOfChangesOutcome.ErrorDuringSending;
        }

        #region AutoActivate / AutoArchive

        /// <summary>
        /// Get all employees who need activating
        /// </summary>
        /// <param name="grace_period"></param>
        /// <returns></returns>
        public Dictionary<int, DateTime?> getPendingArchives(int grace_period, IDBConnection connection = null)
        {
            Dictionary<int, DateTime?> retList;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                retList = new Dictionary<int, DateTime?>();
            const string sql = "select employeeid, terminationdate from dbo.employees where active = 1 and archived = 0 and dateadd(d, @grace_period, terminationdate) <= getdate() and CreationMethod = @empCreationMethod";

                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@grace_period", grace_period);
                expdata.sqlexecute.Parameters.AddWithValue("@empCreationMethod", (int)CreationMethod.ESROutbound);
                using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    int empid = reader.GetInt32(0);
                    if (empid <= 0) continue;

                    DateTime? terminationdate;
                    if (reader.IsDBNull(1))
                    {
                        terminationdate = null;
                    }
                    else
                    {
                        terminationdate = reader.GetDateTime(1);
                    }
                    retList.Add(empid, terminationdate);
                }
                reader.Close();
            }
            }

            return retList;
        }

        /// <summary>
        /// Get all employees unique ID's and the their hire date's awaiting activation
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, DateTime?> getPendingActivations(IDBConnection connection = null)
        {
            Dictionary<int, DateTime?> retList;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                retList = new Dictionary<int, DateTime?>();
            const string sql = "select employeeid, hiredate from dbo.employees where active = 0 and archived = 0 and (hiredate <= getdate() or hiredate is null) and CreationMethod = @empCreationMethod";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@empCreationMethod", (int)CreationMethod.ESROutbound);
                using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    int empid = reader.GetInt32(0);
                    if (empid <= 0) continue;

                    DateTime? hiredate;
                    if (reader.IsDBNull(1))
                    {
                        hiredate = null;
                    }
                    else
                    {
                        hiredate = reader.GetDateTime(1);
                    }

                    retList.Add(empid, hiredate);
                }
                reader.Close();
            }
            }

            return retList;
        }

    #endregion


        public enum PasswordKeyType
        {
            Imported,
            NewEmployee,
            ForgottenPassword,
            AdminRequest
        }

        public List<int> GetEmployeeIdList(IDBConnection connection = null)
        {
            List<int> employeeIds;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                employeeIds = new List<int>();

            const string SQL = "select employeeid from employees";
                using (IDataReader reader = expdata.GetReader(SQL))
            {
                while (reader.Read())
                {
                    int eId = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    employeeIds.Add(eId);
                }
                reader.Close();
            }
            }

            return employeeIds;
        }

        /// <summary>
        /// Get a list of all the employee IDs and their usernames from the employees table
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllEmployeeIDsAndUsernamesList(IDBConnection connection = null)
        {
            Dictionary<int, string> lstDetails;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                expdata.sqlexecute.Parameters.Clear();
                lstDetails = new Dictionary<int, string>();

                using (IDataReader reader = expdata.GetReader("dbo.GetAllEmployeeIDsAndUsernames", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    int empID = reader.GetInt32(0);
                    string username = reader.GetString(1);
                    lstDetails.Add(empID, username);
                }
                reader.Close();
            }
            }

            return lstDetails;
        }

        /// <summary>
        /// Get a list of all the employee ID, username, title, forename and surname from the employees table.
        /// </summary>
        /// <returns>A List of dynamic in this format: { empId, username, title, forename, surname }</returns>
        public List<RequiredEmployeeInfo> GetAllEmployeeRequiredInfoAsList(IDBConnection connection = null)
        {
            List<RequiredEmployeeInfo> lstDetails;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                lstDetails = new List<RequiredEmployeeInfo>();

                using (IDataReader reader = expdata.GetReader("dbo.GetAllEmployeeRequiredInfo", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(reader.GetOrdinal("employeeid"));
                        var username = reader.GetString(reader.GetOrdinal("username"));
                        var title = reader.GetString(reader.GetOrdinal("title"));
                        var forename = reader.GetString(reader.GetOrdinal("firstname"));
                        var surname = reader.GetString(reader.GetOrdinal("surname"));
                            var archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                        lstDetails.Add(new RequiredEmployeeInfo
                        {
                            Id = id,
                            Username = username,
                            Title = title,
                            Forename = forename,
                                Surname = surname,
                                Archived = archived
                        });
                    }
                    reader.Close();
                }
            }

            return lstDetails;
        }

        /// <summary>
        /// This Method Activate the employee account and send email after activation with the information how to access the expenses 
        /// </summary>
        /// <param name="employeeID">ID of the employee whose account is activated</param>
        /// <param name="connection">Database connection</param>
        public void Activate(int employeeID,IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                Employee employee = Employee.Get(employeeID, this.accountid, expdata);
                employee.Active = true;
                employee.LogonRetryCount = 0;
                employee.Save(null);

                if (string.IsNullOrWhiteSpace(employee.EmailAddress) == false)
                {
                    this.SendActivatedEmail(employeeID);
                }

                expdata.sqlexecute.Parameters.Clear();
                this.Cache.Delete(this._nAccountid, Employee.CacheArea, employeeID.ToString());

            // if ESR Assignment record is present, activate that also
                Dictionary<int, cESRAssignment> lstAssignments = this.GetESRAssignments(employeeID);
            if (lstAssignments != null)
            {
                foreach (KeyValuePair<int, cESRAssignment> kvp in lstAssignments)
                {
                    cESRAssignment esrass = kvp.Value;
                    if (esrass.active)
                    {
                        continue;
                    }

                    // check that date is valid for activation according to start and end date of the assignment number
                    DateTime now = DateTime.Now;
                    bool activateAssignment = false;

                    if (esrass.earliestassignmentstartdate.Date >= now.Date)
                    {
                        activateAssignment = true;

                        if (esrass.finalassignmentenddate.HasValue && esrass.finalassignmentenddate.Value.Date < now.Date)
                        {
                            activateAssignment = false;
                        }
                    }

                    if (!activateAssignment)
                    {
                        continue;
                    }

                    // activate their assignment number
                        expdata.sqlexecute.Parameters.Clear();
                        const string SQL = "setEsrAssignmentActive";
                        expdata.sqlexecute.Parameters.AddWithValue("@active", 1);
                        expdata.sqlexecute.Parameters.AddWithValue("@assignmentID", esrass.assignmentid);
                        expdata.ExecuteProc(SQL);
                    }

                cESRAssignments.ResetCache(accountid, employeeID);
                }
            }
        }

        public int SaveEmployee(Employee employee, cDepCostItem[] costCodeBreakdown, List<int> emailNotificationIDs, SortedList<int, object> userDefinedFields, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
            bool newEmployee = employee.EmployeeID == 0;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            employee.EmployeeID = employee.Save(currentUser);

            if (employee.EmployeeID < 1)
            {
                return employee.EmployeeID;
            }

            if (newEmployee && currentUser != null)
            {
                cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
                    cAccountProperties properties =
                        subaccs.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                Guid newPassword = Guid.NewGuid();
                    employee.ChangePassword(
                        newPassword.ToString(),
                        newPassword.ToString(),
                        false,
                        0,
                        properties.PwdHistoryNum,
                        currentUser);
            }

            employee.GetCostBreakdown().Add(newEmployee, costCodeBreakdown);

            cEmailNotifications clsEmailNotifications = new cEmailNotifications(this.accountid);
            clsEmailNotifications.SaveNotificationLink(emailNotificationIDs, employee.EmployeeID, null);

            cFields clsFields = new cFields(this.accountid);
            cTables clstables = new cTables(this.accountid);
            cTable tbl = clstables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(this.accountid);

                    clsuserdefined.SaveValues(tbl.GetUserdefinedTable(), employee.EmployeeID, userDefinedFields, clstables, clsFields, currentUser, elementId: (int)SpendManagementElement.Employees, record: employee.Username);

            if (employee.EmployeeID > 0)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employee.EmployeeID);
                    expdata.ExecuteProc("ResetClaimCurrency");
                            expdata.sqlexecute.Parameters.Clear();
                        }
                    }

            return employee.EmployeeID;
        }


        /// <summary>
        /// Get cars to activate.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<int, cCar> GetCarsToActivate(IDBConnection connection = null)
        {
            Dictionary<int, cCar> retList;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
                retList = new Dictionary<int, cCar>();
            const string sql = "SELECT DISTINCT cars.carid, cars.startdate, cars.employeeid FROM cars, CarAssignmentNumberAllocations WHERE cars.carid = CarAssignmentNumberAllocations.CarId AND CARS.active = 0 AND cars.startdate <= GETDATE()";

                expdata.sqlexecute.Parameters.Clear();
                using (IDataReader reader = expdata.GetReader(sql))
            {
                var caridOrd = reader.GetOrdinal("carid");
                var startdateidOrd = reader.GetOrdinal("startdate");
                var employeeidOrd = reader.GetOrdinal("employeeid");
                while (reader.Read())
                {
                    int carid = reader.GetInt32(caridOrd);
                    if (carid <= 0)
                    {
                        continue;
                    }

                    DateTime? startdate;
                    if (reader.IsDBNull(startdateidOrd))
                    {
                        startdate = null;
                    }
                    else
                    {
                        startdate = reader.GetDateTime(startdateidOrd);
                    }

                    int employeeid = reader.GetInt32(employeeidOrd);

                        var tempCar = new cCar(
                          this.accountid,
                          employeeid,
                          carid,
                          string.Empty,
                          string.Empty,
                          string.Empty,
                          startdate,
                          null,
                          false,
                          new List<int>(),
                          0,
                          0,
                          false,
                          0,
                          MileageUOM.KM,
                          0,
                          null,
                          0,
                          null,
                          null,
                          false,
                          false,
                          0);

                        retList.Add(carid, tempCar);
                }

                reader.Close();
            }
            }

            return retList;
        }

        /// <summary>
        /// The get employee id from car id.
        /// </summary>
        /// <param name="carID">
        /// The car id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetEmployeeIdFromCarId(int carID, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
        {
            const string SQL = "SELECT employeeid from cars where carid = @carid";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
                return expdata.ExecuteScalar<int>(SQL);
            }
        }

        /// <summary>
        /// Save user level of employee into database.
        /// </summary>
        /// <param name="employeeId">
        ///  employee Id.
        /// </param>
        /// <param name="level">
        ///  User level value to set
        /// </param>
        /// <returns>
        /// SaveUserLevel Result <see cref="int"/>.
        /// </returns>
        public int SaveUserLevel(int employeeId, int level)
        {
            int identity = 0;
            var currentUser = cMisc.GetCurrentUser();
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                expdata.sqlexecute.Parameters.AddWithValue("@employeeLevel", level);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("SaveEmployeeLevelBasedOnEmployee");
                identity = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            return identity;
        }
        /// <summary>
        /// Delete user level of employee into database.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public int DeleteUserLevel(int employeeId)
        {
            int identity = 0;
            var currentUser = cMisc.GetCurrentUser();
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("DeleteEmployeeLevelBasedOnEmployee");
                identity = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            return identity;
        }

        /// <summary>
        /// Update lookup date of employees with latest manual driving licence review date 
        /// </summary>
        /// <param name="connection">database connection </param>
        /// <returns>List IDs of employees whose lookupdate is updated.</returns>
        public List<int> UpdateEmployeesDrivingLicenceLookupDate(IDBConnection connection)
        {
            var employeeIdList = new List<int>();
            using (connection)
            {
                using (var reader = connection.GetReader("UpdateEmployeesDrivingLicenceLookupDate", CommandType.StoredProcedure))
                {
                    var employeeidOrd = reader.GetOrdinal("employeeid");
                    while (reader.Read())
                    {
                        employeeIdList.Add(reader.GetInt32(employeeidOrd));
                    }
                }

                connection.sqlexecute.Parameters.Clear();
            }

            return employeeIdList;
        }

        /// <summary>
        /// Get a primary country by its employee ID
        /// </summary>
        /// <param name="employeeId">
        /// Employee ID to obtain a record for
        /// </param>
        /// <param name="accountId">
        /// Account ID to obtain connectionstring
        /// </param>
        /// <returns>
        /// Primary country string
        /// </returns>
        public static string GetEmployeePrimaryCountryById(int employeeId, int accountId)
        {
            string employeeCountry = string.Empty;

            using (var expdata = null ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                if (employeeId > 0)
                {
                    employeeCountry = Employee.GetPrimaryCountry(employeeId, expdata);
                }
            }

            return employeeCountry;
        }

        /// <summary>
        /// Searches for employees based on the supplied filter and search criteria
        /// </summary>
        /// <param name="filters">
        /// The <see cref="JSFieldFilter">JSFieldFilter's </see> to apply</param>
        /// <param name="criteria">The search criteria</param>
        /// <param name="user"> The current user.</param>
        /// <returns>
        /// A list of <see cref="TokenInputResult">TokenInputResult</see>
        /// </returns>
        public List<TokenInputResult> SearchEmployees(Dictionary<string, JSFieldFilter> filters, string criteria, ICurrentUser user)
        {    
            List<sAutoCompleteResult> employees = SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(user, 0, "618DB425-F430-4660-9525-EBAB444ED754", "142EA1B4-7E52-4085-BAAA-9C939F02EB77", "142EA1B4-7E52-4085-BAAA-9C939F02EB77,0F951C3E-29D1-49F0-AC13-4CFCABF21FDA", criteria, true, filters);

            List<TokenInputResult> tokenInputResults = employees.Take(10).Select(e =>
                new TokenInputResult
                {
                    id = e.value,
                    name = e.label,
                    searchDisplay = Regex.Replace(e.label, "[:,&=]", "")
                }).ToList();

            const string cantFindText = "Can't find who you're looking for?";
            tokenInputResults.Add(new TokenInputResult
            {
                id = "-1",
                name = cantFindText,
                searchDisplay = cantFindText
            });

            return tokenInputResults;
        }
    }

    /// <summary>
    /// Represents the required parts of the Employee.
    /// </summary>
    public class RequiredEmployeeInfo
    {
        /// <summary>
        /// The unique Id of the Employee.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The title of the Employee.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// The first name of the Employee.
        /// </summary>
        public string Forename { get; set; }
        
        /// <summary>
        /// The last name of the Employee. 
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// The username of the Employee.
        /// </summary>
        public string Username { get; set; }

            /// <summary>
            /// Is the employee archived?
            /// </summary>
            public bool Archived { get; set; }
    }
    /// <summary>
    /// Structure of employee level details
    /// </summary>
    public struct EmployeeLevelStructure
    {
        public int? LevelNumber;

        public decimal? LevelAmount;

    }


        /// <summary>
        /// A class to record the outcome of Employee Authentication
        /// </summary>
        public class AuthenicationOutcome
        {
            /// <summary>
            /// The unique Id of the Employee.
            /// </summary>
            public int employeeId { get; set; }

            /// <summary>
            /// The result of the authentication.
            /// </summary>
            public LoginResult LoginResult { get; set; }
        }
}
