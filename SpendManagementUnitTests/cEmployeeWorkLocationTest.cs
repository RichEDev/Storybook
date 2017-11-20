using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cEmployeeWorkLocationTest and is intended
    ///to contain all cEmployeeWorkLocationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cEmployeeWorkLocationTest
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
        ///A test for Temporary
        ///</summary>
        [TestMethod()]
        public void TemporaryTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Temporary;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StartDate
        ///</summary>
        [TestMethod()]
        public void StartDateTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.StartDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedOn
        ///</summary>
        [TestMethod()]
        public void ModifiedOnTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.ModifiedOn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedBy
        ///</summary>
        [TestMethod()]
        public void ModifiedByTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.ModifiedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LocationID
        ///</summary>
        [TestMethod()]
        public void LocationIDTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.LocationID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EndDate
        ///</summary>
        [TestMethod()]
        public void EndDateTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.EndDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EmployeeLocationID
        ///</summary>
        [TestMethod()]
        public void EmployeeLocationIDTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.EmployeeLocationID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EmployeeID
        ///</summary>
        [TestMethod()]
        public void EmployeeIDTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.EmployeeID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.CreatedOn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CreatedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Active
        ///</summary>
        [TestMethod()]
        public void ActiveTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Active;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cEmployeeWorkLocation Constructor
        ///</summary>
        [TestMethod()]
        public void cEmployeeWorkLocationConstructorTest()
        {
            int employeelocationid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int locationid = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            bool temporary = false; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeWorkLocation target = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, createdon, createdby, modifiedon, modifiedby);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
