using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderDeliveriesTest and is intended
    ///to contain all cPurchaseOrderDeliveriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderDeliveriesTest
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
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PurchaseOrderID = expected;
            actual = target.PurchaseOrderID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedOn
        ///</summary>
        [TestMethod()]
        public void ModifiedOnTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.ModifiedOn = expected;
            actual = target.ModifiedOn;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedBy
        ///</summary>
        [TestMethod()]
        public void ModifiedByTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            Nullable<int> expected = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            target.ModifiedBy = expected;
            actual = target.ModifiedBy;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryReference
        ///</summary>
        [TestMethod()]
        public void DeliveryReferenceTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.DeliveryReference = expected;
            actual = target.DeliveryReference;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryLocation
        ///</summary>
        [TestMethod()]
        public void DeliveryLocationTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            cCompany expected = null; // TODO: Initialize to an appropriate value
            cCompany actual;
            target.DeliveryLocation = expected;
            actual = target.DeliveryLocation;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryID
        ///</summary>
        [TestMethod()]
        public void DeliveryIDTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DeliveryID = expected;
            actual = target.DeliveryID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryDetails
        ///</summary>
        [TestMethod()]
        public void DeliveryDetailsTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveryDetails> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveryDetails> actual;
            target.DeliveryDetails = expected;
            actual = target.DeliveryDetails;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryDate
        ///</summary>
        [TestMethod()]
        public void DeliveryDateTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.DeliveryDate = expected;
            actual = target.DeliveryDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.CreatedOn = expected;
            actual = target.CreatedOn;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.CreatedBy = expected;
            actual = target.CreatedBy;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveries Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveriesConstructorTest1()
        {
            int POID = 0; // TODO: Initialize to an appropriate value
            DateTime deliveryDate = new DateTime(); // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            cCompany deliveryLocation = null; // TODO: Initialize to an appropriate value
            string deliveryReference = string.Empty; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveryDetails> deliveryDetails = null; // TODO: Initialize to an appropriate value
            DateTime createdOn = new DateTime(); // TODO: Initialize to an appropriate value
            int createdBy = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedOn = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedBy = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries(POID, deliveryDate, deliveryID, deliveryLocation, deliveryReference, deliveryDetails, createdOn, createdBy, modifiedOn, modifiedBy);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveries Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveriesConstructorTest()
        {
            cPurchaseOrderDeliveries target = new cPurchaseOrderDeliveries();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
