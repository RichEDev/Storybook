using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cEmployeeCorporateCardTest and is intended
    ///to contain all cEmployeeCorporateCardTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cEmployeeCorporateCardTest
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
        ///A test for ModifiedOn
        ///</summary>
        [TestMethod()]
        public void ModifiedOnTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
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
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.ModifiedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for employeeid
        ///</summary>
        [TestMethod()]
        public void employeeidTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.employeeid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.CreatedOn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.CreatedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for corporatecardid
        ///</summary>
        [TestMethod()]
        public void corporatecardidTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.corporatecardid = expected;
            actual = target.corporatecardid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cardprovider
        ///</summary>
        [TestMethod()]
        public void cardproviderTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            cCardProvider actual;
            actual = target.cardprovider;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cardnumber
        ///</summary>
        [TestMethod()]
        public void cardnumberTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.cardnumber;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for active
        ///</summary>
        [TestMethod()]
        public void activeTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.active;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for updateCorporateCardId
        ///</summary>
        [TestMethod()]
        public void updateCorporateCardIdTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            target.updateCorporateCardId(id);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for cEmployeeCorporateCard Constructor
        ///</summary>
        [TestMethod()]
        public void cEmployeeCorporateCardConstructorTest()
        {
            int corporatecardid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            cCardProvider cardprovider = null; // TODO: Initialize to an appropriate value
            string cardnumber = string.Empty; // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> createdby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cEmployeeCorporateCard target = new cEmployeeCorporateCard(corporatecardid, employeeid, cardprovider, cardnumber, active, createdon, createdby, modifiedon, modifiedby);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
