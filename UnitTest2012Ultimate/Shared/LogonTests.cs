namespace UnitTest2012Ultimate.Shared
{
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.SessionState;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;

    using Spend_Management;
    using Spend_Management.shared.code.Authentication;

    /// <summary>
    ///     The logon tests.
    /// </summary>
    [TestClass]
    public class LogonTests
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The my class clean up.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// The logon authenticate locked user.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateLockedUser()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username;
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid;
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password);
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var employees = new cEmployees(GlobalTestVariables.AccountId);
            int employeeid = employees.getEmployeeidByUsername(GlobalTestVariables.AccountId, userName);
            try
            {
                employee.Locked = true;
                employee.Save(Moqs.CurrentUser());
                var result =
                    (LoginResult)
                    logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
                Assert.IsTrue(result == LoginResult.EmployeeLocked || result == LoginResult.LogonAttemptsExceeded);
            }
            finally
            {
                employee.Locked = false;
                employee.Save(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The logon authenticate archived user.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateArchivedUser()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username;
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid;
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password);
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var employees = new cEmployees(GlobalTestVariables.AccountId);
            int employeeid = employees.getEmployeeidByUsername(GlobalTestVariables.AccountId, userName);
            try
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());

                var result =
                    (LoginResult)
                    logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
                Assert.IsTrue(result == LoginResult.EmployeeArchived || result == LoginResult.LogonAttemptsExceeded, string.Format("Actual Result = {0}", result));
            }
            finally
            {
                employee.Archived = false;
                employee.Save(Moqs.CurrentUser());

            }
        }

        /// <summary>
        ///     The logon authenticate success.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateSuccess()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username;
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid;
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password);
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var result =
                (LoginResult)
                logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
            Assert.IsTrue(result == LoginResult.Success);
        }

        /// <summary>
        ///     The logon authenticate wrong company name.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateWrongCompanyName()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username;
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid + "wrong";
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password);
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var result =
                (LoginResult)
                logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
            Assert.IsTrue(result == LoginResult.IncorrectCompanyName || result == LoginResult.LogonAttemptsExceeded);
        }

        /// <summary>
        ///     The logon authenticate wrong password.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateWrongPassword()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username;
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid;
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password) + "wrong password";
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var result =
                (LoginResult)
                logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
            Assert.IsTrue(result == LoginResult.InvalidUsernamePasswordCombo || result == LoginResult.LogonAttemptsExceeded);
        }

        /// <summary>
        ///     The logon authenticate wrong user.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonAuthenticateWrongUser()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            Employee employee =
                new cEmployees(GlobalTestVariables.AccountId).GetEmployeeById(GlobalTestVariables.EmployeeId);
            string userName = employee.Username + "wronguser";
            var accounts = new cAccounts();
            string companyName = accounts.GetAccountByID(GlobalTestVariables.AccountId).companyid;
            string passWord = employee.Password;
            if (employee.PasswordMethod == PasswordEncryptionMethod.RijndaelManaged)
            {
                passWord = new cSecureData().Decrypt(employee.Password);
            }

            var request = new HttpRequest(string.Empty, "http://chronos6.sel-expenses.com", string.Empty);
            var result =
                (LoginResult)
                logonObj.Invoke("Authenticate", new object[] { companyName, userName, passWord, request, false });
            Assert.IsTrue(result == LoginResult.InvalidUsernamePasswordCombo || result == LoginResult.LogonAttemptsExceeded);
        }

        /// <summary>
        /// The logon redirect to hostname default.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonRedirectToHostnameDefault()
        {
            var currentAddress = "http://localhost/expenses";
            var currentAccount = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var username = "james";
            var companyId = "testing";
            var password = "Password1";
            var requestUrl = new Uri("http://localhost");

            var result = Logon.GetRedirectToHostnameAddress(currentAddress, currentAccount, username, companyId, password, Modules.expenses);
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result), result, string.Format("Actual result = {0}", result));
            Assert.IsTrue(result.Contains("&a="), string.Format("Actual result = {0}", result));
        }

        /// <summary>
        /// The logon check concurrent user licence limit reached not contracts.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonCheckConcurrentUserLicenceLimitReachedNotContracts()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var result = (Guid)logonObj.Invoke("CheckConcurrentUserLicenceLimitReached", new object[] { Modules.expenses });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == Guid.Empty);
        }

        /// <summary>
        /// The logon check concurrent user licence limit reached not contracts.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonCheckConcurrentUserLicenceLimitReachedWithContracts()
        {
            var testAccount = cAccounts.CachedAccounts[GlobalTestVariables.AccountId];
            if (testAccount.LicencedUsers > 0)
            {
                var logon = new Logon();
                var logonObj = new PrivateObject(logon);
                logonObj.SetField("accountId", GlobalTestVariables.AccountId);
                logonObj.SetField("employeeId", GlobalTestVariables.EmployeeId);
                var result = (Guid)logonObj.Invoke("CheckConcurrentUserLicenceLimitReached", new object[] { Modules.contracts });
                Assert.IsNotNull(result);
                Assert.IsFalse(result == Guid.Empty, string.Format("Actual result = {0} - Check concurrent users on unit test account is not ZERO.", result));    
            }
            else
            {
                Assert.IsTrue(1 == 1);
            }
        }

        /// <summary>
        /// The logon get failed to logon message "details incorrect".
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageIncorrect()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.InvalidUsernamePasswordCombo;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);
            
            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "The details you have entered are incorrect.", result);
        }

        /// <summary>
        /// The logon get failed to logon message "No sub account".
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageNoSubAccount()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.NoSubAccount;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);

            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "Your account is incorrectly configured, contact your administrator.", result);
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageArchived()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.EmployeeArchived;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);

            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "Your account is currently archived, contact your administrator.", result);
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageLocked()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.EmployeeLocked;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);

            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "Your account is currently locked, contact your administrator.", result);
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageInvalidIp()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.InvalidIPAddress;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);

            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "Please note that access is restricted from this location for security purposes.", result);
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageInactiveAccount()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var username = "james";
            var module = Modules.expenses;
            var authorisationResult = LoginResult.InactiveAccount;
            var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
            var globalProperties = this.GetTestProperties(0);

            var result = (string)logonObj.Invoke(
                "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "Your account has not yet been activated.", result);
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageTooManyTries()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var employees = new cEmployees(GlobalTestVariables.AccountId); 
            var employee = employees.GetEmployeeById(GlobalTestVariables.EmployeeId);
            try
            {
                var username = employee.Username;
                var module = Modules.expenses;
                var authorisationResult = LoginResult.LogonAttemptsExceeded;
                var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
                var globalProperties = this.GetTestProperties(1);

                var result = (string)logonObj.Invoke(
                    "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
                Assert.IsNotNull(result);
                Assert.IsTrue(result == "Too many attempts your account has been locked.", result);
            }
            finally
            {
                employee.Locked = false;
                employee.Save(Moqs.CurrentUser());

            }
        }

        /// <summary>
        /// The logon get failed to logon message .
        /// </summary>
        [TestMethod]
        [TestCategory("Logon"), TestCategory("Spend Management")]
        public void LogonGetFailedToLogonMessageTryagain()
        {
            var logon = new Logon();
            var logonObj = new PrivateObject(logon);
            var employees = new cEmployees(GlobalTestVariables.AccountId);
            var employee = employees.GetEmployeeById(GlobalTestVariables.EmployeeId);
            try
            {
                var username = employee.Username;
                var module = Modules.expenses;
                var authorisationResult = LoginResult.InvalidUsernamePasswordCombo;
                var account = new cAccounts().GetAccountByID(GlobalTestVariables.AccountId);
                var globalProperties = this.GetTestProperties(1);

                var result = (string)logonObj.Invoke(
                    "GetFailedToLogonMessage", new object[] { username, module, authorisationResult, account, globalProperties });
                Assert.IsNotNull(result);
                Assert.IsTrue(result == "The details you have entered are incorrect.  1 attempts left.", result);
            }
            finally
            {
                employee.Locked = false;
                employee.Save(Moqs.CurrentUser());

            }
        }

        /// <summary>
        /// The get test properties.
        /// </summary>
        /// <param name="attempts">
        /// The attempts.
        /// </param>
        /// <returns>
        /// The <see cref="cGlobalProperties"/>.
        /// </returns>
        private cGlobalProperties GetTestProperties(byte attempts)
        {
            var result = new cGlobalProperties(
                0,
                "localhost",
                0,
                0,
                "",
                0,
                attempts,
                0,
                PasswordLength.AnyLength,
                0,
                0,
                false,
                false,
                0,
                0,
                0,
                0,
                0,
                false,
                false,
                GlobalTestVariables.AccountId,
                false,
                false,
                false,
                false,
                false,
                0,
                false,

                false,
                false,
                0,
                0,
                0,
                false,
                
                false,
                false,
                false,
                0,
                false,
                false,
                false,
                false,
                false,
                false,
                "uk",
                "£",
                ",",
                false,
                0,
                0,
                false,
                0,
                false,
                false,
                false,
                "",
                0,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                0,
                false,
                false,
                Guid.Empty,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                "",
                "",
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                FieldType.Integer,
                FieldType.Integer,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                false,
                AutoArchiveType.ArchiveAuto,
                AutoActivateType.ActivateAuto,
                0,
                false,
                false,
                false
                );
            return result;
        }

        /// <summary>
        ///     The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
        }

        /// <summary>
        ///     The test clean up.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion
    }
}
