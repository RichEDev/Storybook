using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spend_Management;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    /// <summary>
    /// Tests the cEmployees.Authenticate method
    /// <see cref="cEmployees.Authenticate"/>
    ///
    /// Username not matched
    /// NULL/string.Empty password used
    /// Password not mached
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
    public class cEmployeesAuthenticate
    {
        private string username = string.Empty;
        private string password = string.Empty;
        private cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
        private cEmployee reqGlobalEmployee = null;
        private string defaultPlainTextPassword = "password";

        private cEmployee GetGlobalEmployee()
        {
            if (reqGlobalEmployee == null)
            {
                reqGlobalEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            }
            return reqGlobalEmployee;
        }

        private int[] CheckPasswordMethod(PwdMethod pwdMethod)
        {
            cEmployee reqEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            cEmployeeObject.UpdateEmployeePasswordDetails(PwdMethod.FWBasic, defaultPlainTextPassword, reqEmployee.employeeid);
            int[] results = new int[] { clsEmployees.Authenticate(reqEmployee.username, defaultPlainTextPassword), reqEmployee.employeeid };
            return results;
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_UnknownUsernameWithPassword()
        {
            string uniqueUsername = Guid.NewGuid().ToString() ;
            int returnValue = clsEmployees.Authenticate(uniqueUsername, defaultPlainTextPassword);
            Assert.AreEqual(0, returnValue);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_UnknownUsernameWithOutPassword()
        {
            string uniqueUsername = Guid.NewGuid().ToString();
            int returnValue = clsEmployees.Authenticate(uniqueUsername, string.Empty);
            Assert.AreEqual(0, returnValue);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_NULLOrEmptyPassword()
        {
            cEmployeeObject.UpdateEmployeePasswordDetails(PwdMethod.FWBasic, string.Empty, GetGlobalEmployee().employeeid);
            int returnValue = clsEmployees.Authenticate(GetGlobalEmployee().username, string.Empty);
            Assert.AreEqual(GetGlobalEmployee().employeeid, returnValue);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_PasswordNotMatched()
        {
            string randomPassword = Guid.NewGuid().ToString();
            int returnValue = clsEmployees.Authenticate(GetGlobalEmployee().username, randomPassword);
            Assert.AreEqual((0 - GetGlobalEmployee().employeeid), returnValue);
        }

        #region Tests for specific password encryption/hash methods

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_FWBasic_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.FWBasic);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_Hash_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.Hash);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_SHA_Hash_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.SHA_Hash);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_MD5_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.MD5);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_RijndaelManaged_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.RijndaelManaged);
            Assert.AreEqual(results[0], results[1]);
        }

        [TestCategory("Continuous Integration"), TestMethod]
        public void Authenticate_NoCrypt_Password()
        {
            int[] results = CheckPasswordMethod(PwdMethod.NoCrypt);
            Assert.AreEqual(results[0], results[1]);
        }

        #endregion Tests for specific password encryption/hash methods

        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (reqGlobalEmployee != null)
            {
                clsEmployees.deleteEmployee(reqGlobalEmployee.employeeid);
            }
        }

    }
}
