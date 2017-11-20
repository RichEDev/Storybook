using SpendManagementLibrary.Enumerators;
using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Employees;

    [TestClass()]
    public class cEmployeesAuthenticate
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            username = string.Empty;
            password = string.Empty;
            defaultPlainTextPassword = "password";
            clsEmployees = new cEmployees(GlobalTestVariables.AccountId);
            reqGlobalEmployee = null;
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (this.reqGlobalEmployee != null)
            {
                reqGlobalEmployee.Archived = true;
                reqGlobalEmployee.Save(Moqs.CurrentUser());
                reqGlobalEmployee.Delete(Moqs.CurrentUser());
            }
        }

        #endregion

        private string username;
        private string password;
        private string defaultPlainTextPassword;
        private cEmployees clsEmployees;
        public Employee reqGlobalEmployee;


        public Employee GetGlobalEmployee()
        {
            if (reqGlobalEmployee == null)
            {
                reqGlobalEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            }

            return reqGlobalEmployee;
        }

        private int[] CheckPasswordMethod(PasswordEncryptionMethod pwdMethod)
        {
            GetGlobalEmployee();
            cEmployeeObject.UpdateEmployeePasswordDetails(PasswordEncryptionMethod.FWBasic, defaultPlainTextPassword, reqGlobalEmployee.EmployeeID);
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(username, password, AccessRequestType.Api);
            int[] results = new int[] { authOutcome.employeeId, reqGlobalEmployee.EmployeeID };
            return results;
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_UnknownUsernameWithPassword()
        {
            string uniqueUsername = Guid.NewGuid().ToString();
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(uniqueUsername, defaultPlainTextPassword, AccessRequestType.Api);
            Assert.AreEqual(0, authOutcome.employeeId);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_UnknownUsernameWithOutPassword()
        {
            string uniqueUsername = Guid.NewGuid().ToString();    
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(uniqueUsername, string.Empty, AccessRequestType.Api);  
            Assert.AreEqual(0, authOutcome.employeeId);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_NULLOrEmptyPassword()
        {
            cEmployeeObject.UpdateEmployeePasswordDetails(PasswordEncryptionMethod.FWBasic, string.Empty, GetGlobalEmployee().EmployeeID);
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(GetGlobalEmployee().Username, string.Empty, AccessRequestType.Api);
            Assert.AreEqual(GetGlobalEmployee().EmployeeID, authOutcome.employeeId);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_PasswordNotMatched()
        {
            string randomPassword = Guid.NewGuid().ToString();
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(GetGlobalEmployee().Username, randomPassword, AccessRequestType.Api);    
            Assert.AreEqual((0 - GetGlobalEmployee().EmployeeID), authOutcome.employeeId);
        }

        #region Tests for specific password encryption/hash methods

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_FWBasic_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.FWBasic);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_Hash_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.Hash);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_SHA_Hash_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.ShaHash);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_MD5_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.MD5);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_RijndaelManaged_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.RijndaelManaged);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Spend Management"), TestCategory("Employees"), TestMethod()]
        public void Authenticate_NoCrypt_Password()
        {
            int[] results = CheckPasswordMethod(PasswordEncryptionMethod.NoCrypt);
            Assert.AreEqual(results[0], results[1]);
        }

        #endregion Tests for specific password encryption/hash methods
    }
}