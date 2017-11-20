using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cAccessRolesTest and is intended
    ///to contain all cAccessRolesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAccessRolesTest
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for cAccessRoles Constructor
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void cAccessRolesConstructorTest()
        {
            int accountID = cGlobalVariables.AccountID;
            
            string connectionString = cAccounts.getConnectionString(accountID);
            cAccessRoles target = new cAccessRoles(accountID, connectionString);
            Assert.AreEqual(accountID, target.AccountID);
            Assert.AreEqual(connectionString, target.ConnectionString);
        }
    }
}
