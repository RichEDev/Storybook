using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderDeliveriesAJAXTest and is intended
    ///to contain all cPurchaseOrderDeliveriesAJAXTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderDeliveriesAJAXTest
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
        ///A test for DeliveryReference
        ///</summary>
        [TestMethod()]
        public void DeliveryReferenceTest()
        {
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderDeliveryDetails> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderDeliveryDetails> actual;
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
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.DeliveryDate = expected;
            actual = target.DeliveryDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveriesAJAX Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveriesAJAXConstructorTest1()
        {
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            DateTime deliveryDate = new DateTime(); // TODO: Initialize to an appropriate value
            cCompany deliveryLocation = null; // TODO: Initialize to an appropriate value
            string deliveryReference = string.Empty; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderDeliveryDetails> deliveryDetails = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX(deliveryID, deliveryDate, deliveryLocation, deliveryReference, deliveryDetails);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveriesAJAX Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveriesAJAXConstructorTest()
        {
            cPurchaseOrderDeliveriesAJAX target = new cPurchaseOrderDeliveriesAJAX();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
