using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cProjectCodesTest and is intended
    ///to contain all cProjectCodesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cProjectCodesTest
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
        ///A test for deleteProjectCode
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void deleteProjectCodeTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cProjectCodes target = new cProjectCodes(accountid);
            cProjectCode expected = new cProjectCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int projectcodeid = target.saveProjectCode(expected);

            target.deleteProjectCode(projectcodeid, cGlobalVariables.EmployeeID);

            Assert.IsNull(target.getProjectCodeById(projectcodeid));
        }

        /// <summary>
        ///A test for getProjectCodeById
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void getProjectCodeByIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cProjectCodes target = new cProjectCodes(accountid);
            cProjectCode expected = new cProjectCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int projectcodeid = target.saveProjectCode(expected);

            cProjectCode actual = target.getProjectCodeById(projectcodeid);
            Assert.AreEqual(projectcodeid, actual.projectcodeid);
            Assert.AreEqual(expected.projectcode, actual.projectcode);
            Assert.AreEqual(expected.description, actual.description);
            Assert.AreEqual(expected.archived, actual.archived);
            Assert.AreEqual(expected.rechargeable, actual.rechargeable);
            Assert.AreEqual(expected.createdon, actual.createdon);
            Assert.AreEqual(expected.createdby, actual.createdby);
            Assert.AreEqual(expected.modifiedon, actual.modifiedon);
            Assert.AreEqual(expected.modifiedby, actual.modifiedby);

            actual = target.getProjectCodeById(99999);
            Assert.IsNull(actual);

            target.deleteProjectCode(projectcodeid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for getProjectCodeByString
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void getProjectCodeByStringTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cProjectCodes target = new cProjectCodes(accountid);
            cProjectCode expected = new cProjectCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int projectcodeid = target.saveProjectCode(expected);

            cProjectCode actual = target.getProjectCodeByString(expected.projectcode);
            Assert.AreEqual(projectcodeid, actual.projectcodeid);
            Assert.AreEqual(expected.projectcode, actual.projectcode);
            Assert.AreEqual(expected.description, actual.description);
            Assert.AreEqual(expected.archived, actual.archived);
            Assert.AreEqual(expected.rechargeable, actual.rechargeable);
            Assert.AreEqual(expected.createdon, actual.createdon);
            Assert.AreEqual(expected.createdby, actual.createdby);
            Assert.AreEqual(expected.modifiedon, actual.modifiedon);
            Assert.AreEqual(expected.modifiedby, actual.modifiedby);

            actual = target.getProjectCodeByString("A code what dis nay exist");
            Assert.IsNull(actual);

            target.deleteProjectCode(projectcodeid, cGlobalVariables.EmployeeID);
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
            cProjectCodes target = new cProjectCodes(accountid);
            cProjectCode expected = new cProjectCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int projectcodeid = target.saveProjectCode(expected);

            bool archive = false;
            target.changeStatus(projectcodeid, archive);

            archive = true;
            target.changeStatus(projectcodeid, archive);

            cProjectCode actual = target.getProjectCodeById(projectcodeid);
            Assert.AreEqual(archive, actual.archived);

            target.deleteProjectCode(projectcodeid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for saveProjectCode
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void saveProjectCodeTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cProjectCodes target = new cProjectCodes(accountid);
            cProjectCode expected = new cProjectCode(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int projectcodeid = target.saveProjectCode(expected);

            cProjectCode actual = target.getProjectCodeById(projectcodeid);
            Assert.AreEqual(projectcodeid, actual.projectcodeid);
            Assert.AreEqual(expected.projectcode, actual.projectcode);
            Assert.AreEqual(expected.description, actual.description);
            Assert.AreEqual(expected.archived, actual.archived);
            Assert.AreEqual(expected.rechargeable, actual.rechargeable);
            Assert.AreEqual(expected.createdon, actual.createdon);
            Assert.AreEqual(expected.createdby, actual.createdby);

            //add the same again
            int actualid = target.saveProjectCode(expected);
            Assert.AreEqual(-1, actualid);

            //update a record
            cProjectCode updatedProjectCode = new cProjectCode(projectcodeid, expected.projectcode, "Updated description", expected.archived, expected.rechargeable, expected.createdon, expected.createdby, DateTime.Now, cGlobalVariables.EmployeeID, new SortedList<int, object>());
            actualid = target.saveProjectCode(updatedProjectCode);

            actual = target.getProjectCodeById(actualid);
            Assert.AreEqual(actual.projectcodeid, projectcodeid);
            Assert.AreEqual(actual.projectcode, updatedProjectCode.projectcode);
            Assert.AreEqual(actual.description, updatedProjectCode.description);
            Assert.AreEqual(actual.archived, updatedProjectCode.archived);
            Assert.AreEqual(actual.rechargeable, updatedProjectCode.rechargeable);
            Assert.AreEqual(actual.createdon.ToShortDateString(), updatedProjectCode.createdon.ToShortDateString());
            Assert.AreEqual(actual.createdby, updatedProjectCode.createdby);
            Assert.AreEqual(actual.modifiedon.Value.ToShortDateString(), updatedProjectCode.modifiedon.Value.ToShortDateString());
            Assert.AreEqual(actual.modifiedby, updatedProjectCode.modifiedby);

            target.deleteProjectCode(projectcodeid, cGlobalVariables.EmployeeID);
        }
    }
}
