using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using SpendManagementLibrary;

namespace SpendManagementUnitTests
{    
    /// <summary>
    ///This is a test class for cConcurrentUsersTest and is intended
    ///to contain all cConcurrentUsersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cConcurrentUsersTest
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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cConcurrentUserObject.CreateConcurrentUser();
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cConcurrentUserObject.RemoveConcurrentUser();
        }
        
        #endregion


        /// <summary>
        ///A test for cConcurrentUsers Constructor
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_cConcurrentUsersConstructorTest()
        {
            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            cConcurrentUsers target = new cConcurrentUsers(accountID, employeeID);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CUActivityHit
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_CUActivityHitTest()
        {
            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string sql = "select lastActivity from dbo.accessManagement where manageID = @manageID";
            db.sqlexecute.Parameters.AddWithValue("@manageID", cGlobalVariables.ConcurrentUserManageID);
            
            DateTime? initalDate = null;
            DateTime? activityHitDate = null;

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while(reader.Read())
                {
                    initalDate = reader.GetDateTime(0);
                }
                reader.Close();
            }

            Assert.IsNotNull(initalDate);

            cConcurrentUsers.CUActivityHit(accountID, employeeID, DateTime.Now.AddMinutes(-2));

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    activityHitDate = reader.GetDateTime(0);
                }
                reader.Close();
            }

            Assert.IsNotNull(activityHitDate);
            Assert.AreNotEqual(initalDate, activityHitDate);
        }

        /// <summary>
        ///A test for Exists
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_ExistsTest_True()
        {
            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            cConcurrentUsers target = new cConcurrentUsers(accountID, employeeID);
            
            bool expected = true;
            bool actual;
            actual = target.Exists(employeeID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Exists
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_ExistsTest_False()
        {
            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            cConcurrentUsers target = new cConcurrentUsers(accountID, employeeID);

            bool expected = false;
            bool actual;
            actual = target.Exists(-1);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for LogoffUser
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_LogoffUserTest()
        {
            Guid accessManageID = cGlobalVariables.ConcurrentUserManageID;
            int accountID = cGlobalVariables.AccountID;
            cConcurrentUsers.LogoffUser(accessManageID, accountID);
            cConcurrentUsers cusers = new cConcurrentUsers(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            bool userExists = cusers.Exists(cGlobalVariables.EmployeeID);
            Assert.IsFalse(userExists);
        }

        /// <summary>
        ///A test for LogonUser
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_LogonUserTest()
        {
            // remove logon prior to testing
            cConcurrentUserObject.RemoveConcurrentUser();

            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            cConcurrentUsers target = new cConcurrentUsers(accountID, employeeID);
            Assert.IsFalse(target.Exists(cGlobalVariables.EmployeeID));

            Guid not_expected = Guid.Empty;
            Guid actual;
            actual = target.LogonUser();
            Assert.AreNotEqual(not_expected, actual);
            Assert.IsTrue(target.Exists(cGlobalVariables.EmployeeID));
        }

        /// <summary>
        ///A test for getManageID
        ///</summary>
        [TestMethod()]
        public void cConcurrentUsersTest_getManageIDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            int employeeID = cGlobalVariables.EmployeeID;
            cConcurrentUsers target = new cConcurrentUsers(accountID, employeeID);
            Guid expected = cGlobalVariables.ConcurrentUserManageID;
            Guid actual;
            actual = target.getManageID(cGlobalVariables.EmployeeID);
            Assert.AreEqual(expected, actual);
        }
    }
}
