using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System;
using System.Collections.Generic;
using System.Web.Security;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cDepartmentsTest and is intended
    ///to contain all cDepartmentsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cDepartmentsTest
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
        ///A test for changeStatus
        ///</summary>
        
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void changeStatusTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cDepartments target = new cDepartments(accountid);
            cDepartment department = new cDepartment(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int departmentid = target.saveDepartment(department);

            bool archive = false;
            target.changeStatus(departmentid, archive);
            archive = true;
            target.changeStatus(departmentid, archive);
            
            cDepartment actual = target.GetDepartmentById(departmentid);
            Assert.AreEqual(archive, actual.archived);
            
            target.deleteDepartment(departmentid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for GetDepartmentById
        ///</summary>
        
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void GetDepartmentByIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cDepartments target = new cDepartments(accountid);
            cDepartment department = new cDepartment(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int departmentid = target.saveDepartment(department);

            cDepartment actual;
            actual = target.GetDepartmentById(departmentid);

            Assert.AreEqual(departmentid, actual.departmentid);
            Assert.AreEqual(department.department, actual.department);
            Assert.AreEqual(department.description, actual.description);
            Assert.AreEqual(department.archived, actual.archived);
            Assert.AreEqual(department.createdon, actual.createdon);
            Assert.AreEqual(department.createdby, actual.createdby);
            Assert.AreEqual(department.modifiedon, actual.modifiedon);
            Assert.AreEqual(department.modifiedby, actual.modifiedby);

            actual = target.GetDepartmentById(99999);
            Assert.IsNull(actual);

            target.deleteDepartment(departmentid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for getDepartmentByString
        ///</summary>
        
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void getDepartmentByStringTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cDepartments target = new cDepartments(accountid);
            cDepartment department = new cDepartment(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int departmentid = target.saveDepartment(department);

            cDepartment actual;
            actual = target.getDepartmentByString(department.department);

            Assert.AreEqual(departmentid, actual.departmentid);
            Assert.AreEqual(department.department, actual.department);
            Assert.AreEqual(department.description, actual.description);
            Assert.AreEqual(department.archived, actual.archived);
            Assert.AreEqual(department.createdon, actual.createdon);
            Assert.AreEqual(department.createdby, actual.createdby);
            Assert.AreEqual(department.modifiedon, actual.modifiedon);
            Assert.AreEqual(department.modifiedby, actual.modifiedby);

            actual = target.getDepartmentByString("a code what dis nay exist");
            Assert.IsNull(actual);

            target.deleteDepartment(departmentid, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        ///A test for deleteDepartment
        ///</summary>
        
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void deleteDepartmentTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cDepartments target = new cDepartments(accountid);
            cDepartment department = new cDepartment(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int departmentid = target.saveDepartment(department);
            
            target.deleteDepartment(departmentid, cGlobalVariables.EmployeeID);

            Assert.IsNull(target.GetDepartmentById(departmentid));
        }

        /// <summary>
        ///A test for saveDepartment
        ///</summary>
        
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/unitTest.aspx")]
        public void saveDepartmentTest()
        {

            int accountid = cGlobalVariables.AccountID;
            int departmentid;
            cDepartments target = new cDepartments(accountid);
            cDepartment department = new cDepartment(0, "Unit Test " + DateTime.Now.ToString(), "A description", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, object>());
            int actual;
            actual = target.saveDepartment(department);
            departmentid = actual;

            //get the costcode and check values
            cDepartment check = target.GetDepartmentById(actual);
            Assert.AreEqual(check.department, department.department);
            Assert.AreEqual(check.archived, department.archived);
            Assert.AreEqual(check.departmentid, departmentid);
            Assert.AreEqual(check.createdby, department.createdby);
            Assert.AreEqual(check.createdon.ToShortDateString(), department.createdon.ToShortDateString());
            Assert.AreEqual(check.description, department.description);

            //add the same again
            actual = target.saveDepartment(department);
            Assert.AreEqual(-1, actual);

            //update a record
            cDepartment updatedDepartment = new cDepartment(departmentid, department.department, "Updated description", false, department.createdon, department.createdby, DateTime.Now, 517, new SortedList<int, object>());
            actual = target.saveDepartment(updatedDepartment);

            check = target.GetDepartmentById(actual);
            Assert.AreEqual(check.department, updatedDepartment.department);
            Assert.AreEqual(check.archived, updatedDepartment.archived);
            Assert.AreEqual(check.departmentid, updatedDepartment.departmentid);
            Assert.AreEqual(check.createdby, updatedDepartment.createdby);
            Assert.AreEqual(check.createdon.ToShortDateString(), updatedDepartment.createdon.ToShortDateString());
            Assert.AreEqual(check.description, updatedDepartment.description);

            target.deleteDepartment(department.departmentid, cGlobalVariables.EmployeeID);
        }
    }
}
