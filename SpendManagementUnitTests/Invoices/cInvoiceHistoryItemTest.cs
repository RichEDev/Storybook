using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInvoiceHistoryItemTest and is intended
    ///to contain all cInvoiceHistoryItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInvoiceHistoryItemTest
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
        ///A test for InvoiceID
        ///</summary>
        [TestMethod()]
        public void InvoiceIDTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.InvoiceID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.CreatedOn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedByString
        ///</summary>
        [TestMethod()]
        public void CreatedByStringTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.CreatedByString;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CreatedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Comment
        ///</summary>
        [TestMethod()]
        public void CommentTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Comment;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cInvoiceHistoryItem Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceHistoryItemConstructorTest()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cInvoiceHistoryItem target = new cInvoiceHistoryItem(invoiceid, comment, createdByString, createdon, createdby);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
