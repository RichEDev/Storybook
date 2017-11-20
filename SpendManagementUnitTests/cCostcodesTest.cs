using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using SpendManagementLibrary;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCostcodesTest and is intended
    ///to contain all cCostcodesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCostcodesTest
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
        ///A test for saveCostcode
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://localhost/fred/unitTest.aspx")]
        public void saveCostcodeTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCostcodes target = new cCostcodes(accountid);
            cCostCode costcode = new cCostCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int costcodeid = target.saveCostcode(costcode);

            //get the costcode and check values
            cCostCode check = target.GetCostCodeById(costcodeid);
            Assert.AreEqual(check.costcodeid, costcodeid);
            Assert.AreEqual(check.costcode, costcode.costcode);
            Assert.AreEqual(check.description, costcode.description);
            Assert.AreEqual(check.archived, costcode.archived);
            Assert.AreEqual(check.createdon.ToShortDateString(), costcode.createdon.ToShortDateString());
            Assert.AreEqual(check.createdby, costcode.createdby);
            
            //add the same again
            int actual = target.saveCostcode(costcode);
            Assert.AreEqual(-1, actual);

            //update a record
            cCostCode updatedCostCode = new cCostCode(costcodeid, costcode.costcode, "Updated description", false, costcode.createdon, costcode.createdby, DateTime.Now, cGlobalVariables.EmployeeID, new SortedList<int, object>());
            actual = target.saveCostcode(updatedCostCode);

            check = target.GetCostCodeById(actual);
            Assert.AreEqual(check.costcodeid, costcodeid);
            Assert.AreEqual(check.costcode, updatedCostCode.costcode);
            Assert.AreEqual(check.description, updatedCostCode.description);
            Assert.AreEqual(check.archived, updatedCostCode.archived);
            Assert.AreEqual(check.createdon.ToShortDateString(), updatedCostCode.createdon.ToShortDateString());
            Assert.AreEqual(check.createdby, updatedCostCode.createdby);

            target.deleteCostCode(costcodeid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for getCostCodeByString
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void getCostCodeByStringTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCostcodes target = new cCostcodes(accountid);
            cCostCode costcode = new cCostCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int costcodeid = target.saveCostcode(costcode);
            
            cCostCode actual = target.getCostCodeByString(costcode.costcode);
            Assert.AreEqual(costcodeid, actual.costcodeid);
            Assert.AreEqual(costcode.costcode, actual.costcode);
            Assert.AreEqual(costcode.description, actual.description);
            Assert.AreEqual(costcode.archived, actual.archived);
            Assert.AreEqual(costcode.createdon, actual.createdon);
            Assert.AreEqual(costcode.createdby, actual.createdby);
            Assert.AreEqual(costcode.modifiedon, actual.modifiedon);
            Assert.AreEqual(costcode.modifiedby, actual.modifiedby);

            actual = target.getCostCodeByString("a code what dis nay exist");
            Assert.IsNull(actual);

            target.deleteCostCode(costcodeid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for GetCostCodeById
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void GetCostCodeByIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCostcodes target = new cCostcodes(accountid);
            cCostCode costcode = new cCostCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int costcodeid = target.saveCostcode(costcode);
            
            cCostCode actual = target.GetCostCodeById(costcodeid);
            Assert.AreEqual(costcodeid, actual.costcodeid);
            Assert.AreEqual(costcode.costcode, actual.costcode);
            Assert.AreEqual(costcode.description, actual.description);
            Assert.AreEqual(costcode.archived, actual.archived);
            Assert.AreEqual(costcode.createdon, actual.createdon);
            Assert.AreEqual(costcode.createdby, actual.createdby);
            Assert.AreEqual(costcode.modifiedon, actual.modifiedon);
            Assert.AreEqual(costcode.modifiedby, actual.modifiedby);

            actual = target.GetCostCodeById(99999);
            Assert.IsNull(actual);

            target.deleteCostCode(costcodeid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for deleteCostCode
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void deleteCostCodeTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCostcodes target = new cCostcodes(accountid);
            cCostCode costcode = new cCostCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int costcodeid = target.saveCostcode(costcode);

            int expected = 0;
            int actual = target.deleteCostCode(costcodeid, cGlobalVariables.EmployeeID);
            Assert.AreEqual(expected, actual);

            Assert.IsNull(target.GetCostCodeById(costcodeid));
            
        }


        /// <summary>
        ///A test for changeStatus
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void changeStatusTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCostcodes target = new cCostcodes(accountid);
            cCostCode costcode = new cCostCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int costcodeid = target.saveCostcode(costcode);

            bool archive = false;
            byte expected = 0;
            int actual;
            actual = target.changeStatus(costcodeid, archive);
            Assert.AreEqual(expected, actual);

            archive = true;
            actual = target.changeStatus(costcodeid, archive);
            Assert.AreEqual(expected, actual);

            target.deleteCostCode(costcodeid, cGlobalVariables.EmployeeID);
        }
    }
}
