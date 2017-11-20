using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderHistoryItemTest and is intended
    ///to contain all cPurchaseOrderHistoryItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderHistoryItemTest
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
        ///A test for PurchaseOrderID
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderIDTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PurchaseOrderID = expected;
            actual = target.PurchaseOrderID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.CreatedOn = expected;
            actual = target.CreatedOn;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedByString
        ///</summary>
        [TestMethod()]
        public void CreatedByStringTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.CreatedByString = expected;
            actual = target.CreatedByString;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.CreatedBy = expected;
            actual = target.CreatedBy;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Comment
        ///</summary>
        [TestMethod()]
        public void CommentTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Comment = expected;
            actual = target.Comment;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderHistoryItem Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderHistoryItemConstructorTest1()
        {
            int purchaseorderid = 0; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            string createdByString = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem(purchaseorderid, comment, createdByString, createdon, createdby);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderHistoryItem Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderHistoryItemConstructorTest()
        {
            cPurchaseOrderHistoryItem target = new cPurchaseOrderHistoryItem();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
