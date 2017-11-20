using SpendManagementLibrary.Enumerators;

namespace UnitTest2012Ultimate
{
using System;
using System.Collections.Generic;
    using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SpendManagementLibrary.Employees;

using Spend_Management;
using SpendManagementLibrary;

    /// <summary>
    /// Tests the cEmployees.Authenticate method<see cref="cEmployees.Authenticate"/>Username not matched
    /// NULL/string.Empty password used
    /// Password not matched
    /// PwdMethod.FWBasic password match
    /// PwdMethod.Hash password match
    /// PwdMethod.SHA_Hash password match
    /// PwdMethod.MD5 password match
    /// PwdMethod.RijndaelManaged password match
    /// PwdMethod.NoCrypt password match
    /// Unknown PwdMethod fails to logon
    /// Any PwdMethod other than RijndaelManaged should be updated to RijndaelManaged in the database
    /// Incorrect password matches call IncrementLogonRetryCount (negative employeeID returned
    /// Check reset of the logon attempts
    /// </summary>
    [TestClass]
    public class EmployeeTests
    {
        /// <summary>
        /// The employees class.
        /// </summary>
        private cEmployees clsEmployees;

        /// <summary>
        /// The req global employee.
        /// </summary>
        private Employee reqGlobalEmployee;

        /// <summary>
        /// The default plain text password.
        /// </summary>
        private string defaultPlainTextPassword;

        /// <summary>
        /// The employee id.
        /// </summary>
        private int employeeId;

        #region Additional Test Attributes

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
        /// The my class cleanup.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The test initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.clsEmployees = new cEmployees(GlobalTestVariables.AccountId);
            this.reqGlobalEmployee = null;
            this.defaultPlainTextPassword = "password";
            this.employeeId = GlobalTestVariables.EmployeeId;
        }

        /// <summary>
        /// The test cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (this.reqGlobalEmployee != null)
            {
                this.reqGlobalEmployee.Delete(Moqs.CurrentUser());
            }
        }

        #endregion

        #region Authenticate tests

        /// <summary>
        /// The authenticate unknown username with password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateUnknownUsernameWithPassword()
        {
            var uniqueUsername = Guid.NewGuid().ToString();

            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(uniqueUsername, this.defaultPlainTextPassword, AccessRequestType.Website);
            Assert.AreEqual(0, authOutcome.employeeId);
        }

        /// <summary>
        /// The authenticate unknown username with out password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateUnknownUsernameWithOutPassword()
        {
            var uniqueUsername = Guid.NewGuid().ToString();
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(uniqueUsername, string.Empty, AccessRequestType.Website);   
            Assert.AreEqual(0, authOutcome.employeeId);
        }

        /// <summary>
        /// The authenticate null or empty password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateNullOrEmptyPassword()
        {
            Employee employee = GetGlobalEmployee();
            try
            {
                cEmployeeObject.UpdateEmployeePasswordDetails(PasswordEncryptionMethod.FWBasic, string.Empty, employee.EmployeeID);       
                AuthenicationOutcome authOutcome = clsEmployees.Authenticate(employee.Username, string.Empty, AccessRequestType.Website);
                Assert.AreEqual(employee.EmployeeID, authOutcome.employeeId);
        }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate password not matched.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticatePasswordNotMatched()
        {
            Employee employee = GetGlobalEmployee();
            try
            {
            string randomPassword = Guid.NewGuid().ToString();
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(employee.Username, randomPassword, AccessRequestType.Website);
            Assert.AreEqual((0 - employee.EmployeeID), authOutcome.employeeId);
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        #endregion

        #region Tests for specific password encryption/hash methods

        /// <summary>
        /// The authenticate fw basic password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateFwBasicPassword()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.FWBasic);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate hash password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateHashPassword()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.Hash);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate Sha hash password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateShaHashPassword()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.ShaHash);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate md 5 password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateMd5Password()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.MD5);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate Rijndael managed password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void AuthenticateRijndaelManagedPassword()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.RijndaelManaged);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The authenticate no crypt password.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod, ExpectedException(typeof(Exception))]
        public void AuthenticateNoCryptPassword()
        {
            int employeeid = 0;

            try
            {
                int[] results = CheckPasswordMethod(PasswordEncryptionMethod.NoCrypt);
                employeeid = results[1];
            Assert.AreEqual(results[0], results[1]);
        }
            finally
            {
                var target = new cEmployees(GlobalTestVariables.AccountId);
                var employee = target.GetEmployeeById(employeeid);
                if (employee != null)
                {
                    employee.Archived = true;
                    employee.Save(Moqs.CurrentUser());
                    employee.Delete(Moqs.CurrentUser());
                }
            }
        }

        #endregion Tests for specific password encryption/hash methods

        #region General employee tests

        /// <summary>
        /// A test for deleteEmployee
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void CEmployeesTestDeleteEmployeeNoConstraints()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            int expected = 0;
            int actual;
            try
            {
                actual = employee.Delete(Moqs.CurrentUser());

            Assert.IsTrue(actual == 4); // should get return value that says employee not archived
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                actual = employee.Delete(Moqs.CurrentUser());
            }

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for saveEmployee
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void CEmployeesTestSaveEmployeeNewEmployee()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0);
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// A test for saveEmployee
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void CEmployeesTestSaveEmployeeExistingEmployee()
        {
            int accountid = GlobalTestVariables.AccountId;
            cEmployees target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            int actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));

                int editing = employee.Save(Moqs.CurrentUser());
                Assert.AreEqual(actual, editing, "No employee id returned from editing save - return code " + editing.ToString(CultureInfo.InvariantCulture));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// A test for saveEmployee
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod]
        public void CEmployeesTestSaveEmployeeDuplicateEmployee()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            var duplicateEmployee = cEmployeeObject.GetUTEmployeeTemplateObject();
            duplicateEmployee.Username = employee.Username;

            duplicateEmployee.EmployeeID = 0;
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));

                int duplicate = duplicateEmployee.Save(Moqs.CurrentUser());
                Assert.IsTrue(duplicate == -1, "SaveEmployee duplicate actual Return Value = " + duplicate.ToString(CultureInfo.InvariantCulture));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
                duplicateEmployee.Archived = true;
                duplicateEmployee.Save(Moqs.CurrentUser());
                duplicateEmployee.Delete(Moqs.CurrentUser());
            }
        }

        #endregion

        #region Login Tests

        /// <summary>
        /// Create a new employee, then set lock flag.  On login this should return "Employee Locked" status
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestLockExistingEmployeeReadBackAsAsLocked()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));

                // Lock the current Employee
                employee.Locked = true;
                employee.Save(Moqs.CurrentUser());
                var lockedEmployee = target.GetEmployeeById(actual);
                Assert.IsTrue(lockedEmployee.Locked, "Not showing user as locked");
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The c employees test lock existing employee read back as as not locked.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestLockExistingEmployeeReadBackAsAsNotLocked()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));

                var lockedEmployee = target.GetEmployeeById(actual);
                Assert.IsTrue(!lockedEmployee.Locked, "Showing user as locked");
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The c employees test forgotten details.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestForgottenDetails()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObjectUniqueEmailAddress();
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));
                
                var response = cEmployees.RequestForgottenDetails(employee.EmailAddress, Modules.expenses);
                Assert.IsTrue(
                    response == ForgottenDetailsResponse.EmployeeDetailsSent, 
                    string.Format("Response ={0}, not {1} as expected", response, ForgottenDetailsResponse.EmployeeDetailsSent));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The c employees test forgotten details duplicate emails.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestForgottenDetailsDuplicateEmails()
        {
            var accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            Employee duplicateEmployee = cEmployeeObject.GetUTEmployeeTemplateObject();
            var actual = 0;
            var duplicate = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                employee = cEmployeeObject.GetUTEmployeeTemplateObject();
                duplicate = duplicateEmployee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));
                
                ForgottenDetailsResponse response = cEmployees.RequestForgottenDetails(employee.EmailAddress, Modules.expenses);
                Assert.IsTrue(
                    response == ForgottenDetailsResponse.EmailNotUnique,
                    string.Format("Response ={0}, not {1} as expected", response, ForgottenDetailsResponse.EmailNotUnique));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
                duplicateEmployee.Archived = true;
                duplicateEmployee.Save(Moqs.CurrentUser());
                duplicateEmployee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The c employees test forgotten details employee locked.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestForgottenDetailsEmployeeLocked()
        {
            int accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObjectUniqueEmailAddress(true);
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));
                employee.Locked = true;
                employee.Save(Moqs.CurrentUser());
                ForgottenDetailsResponse response = cEmployees.RequestForgottenDetails(employee.EmailAddress, Modules.expenses);

                Assert.IsTrue(
                    response == ForgottenDetailsResponse.EmployeeLocked,
                    string.Format("Response ={0}, not {1} as expected", response, ForgottenDetailsResponse.EmployeeLocked));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        /// <summary>
        /// The c employees test forgotten details employee archived.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Employees"), TestCategory("Logon"), TestMethod]
        public void CEmployeesTestForgottenDetailsEmployeeArchived()
        {
            int accountid = GlobalTestVariables.AccountId;
            var target = new cEmployees(accountid);
            Employee employee = cEmployeeObject.GetUTEmployeeTemplateObjectUniqueEmailAddress(true);
            var actual = 0;

            try
            {
                actual = employee.Save(Moqs.CurrentUser());
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString(CultureInfo.InvariantCulture));
                ForgottenDetailsResponse response = cEmployees.RequestForgottenDetails(employee.EmailAddress, Modules.expenses);

                Assert.IsTrue(
                    response == ForgottenDetailsResponse.ArchivedEmployee,
                    string.Format("Response ={0}, not {1} as expected", response, ForgottenDetailsResponse.ArchivedEmployee));
            }
            finally
            {
                employee.Archived = true;
                employee.Save(Moqs.CurrentUser());
                employee.Delete(Moqs.CurrentUser());
            }
        }

        #endregion
            
        /// <summary>
        /// The get global employee.
        /// </summary>
        /// <returns>
        /// The <see cref="cEmployee"/>.
        /// </returns>
        private Employee GetGlobalEmployee()
        {
            return this.reqGlobalEmployee ?? (this.reqGlobalEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject()));
        }

        /// <summary>
        /// The check password method.
        /// </summary>
        /// <param name="pwdMethod">
        /// The pwd method.
        /// </param>
        /// <returns>
        /// An int array.
        /// </returns>
        private int[] CheckPasswordMethod(PasswordEncryptionMethod pwdMethod)
        {
            Employee reqEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            cEmployeeObject.UpdateEmployeePasswordDetails(pwdMethod, this.defaultPlainTextPassword, reqEmployee.EmployeeID);
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(reqEmployee.Username, this.defaultPlainTextPassword, AccessRequestType.Website);
   
            var results = new[] { authOutcome.employeeId, reqEmployee.EmployeeID };

            return results;
        }

        /// <summary>
        /// Takes an inactive employee, activates it and confirms that the cache is updated
        /// </summary>
        [TestMethod]
        public void ActivateCacheTest()
        {

            Employee employeeSaved = null;
            try
            {
                // define our employee
                var employeeTemplate = cEmployeeObject.GetUTEmployeeTemplateObject();
                employeeTemplate.Active = false;

                // save the employee
                employeeSaved = cEmployeeObject.CreateUTEmployeeObject(employeeTemplate);

                // activate the new employee
                this.clsEmployees.Activate(employeeSaved.EmployeeID);

                // get the employee
                var employeeFromCache = Employee.Get(employeeSaved.EmployeeID, GlobalTestVariables.AccountId);

                // check it's active
                Assert.IsTrue(employeeFromCache.Active);
            }
            finally
            {
                if (employeeSaved != null)
                {
                    // delete the employee after the test
                    employeeSaved.Delete(Moqs.CurrentUser());
                }
            }
        }
    }
}
