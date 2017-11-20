using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderDeliveryDetailsTest and is intended
    ///to contain all cPurchaseOrderDeliveryDetailsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderDeliveryDetailsTest
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
        ///A test for ReturnedQuantity
        ///</summary>
        [TestMethod()]
        public void ReturnedQuantityTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.ReturnedQuantity;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProductID
        ///</summary>
        [TestMethod()]
        public void ProductIDTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ProductID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryQuantity
        ///</summary>
        [TestMethod()]
        public void DeliveryQuantityTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.DeliveryQuantity;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryID
        ///</summary>
        [TestMethod()]
        public void DeliveryIDTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.DeliveryID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryDetailsID
        ///</summary>
        [TestMethod()]
        public void DeliveryDetailsIDTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.DeliveryDetailsID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveryDetails Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveryDetailsConstructorTest1()
        {
            int deliveryDetailsID = 0; // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            int productID = 0; // TODO: Initialize to an appropriate value
            Decimal deliveryQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal returnedQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails(deliveryDetailsID, deliveryID, productID, deliveryQuantity, returnedQuantity);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderDeliveryDetails Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderDeliveryDetailsConstructorTest()
        {
            cPurchaseOrderDeliveryDetails target = new cPurchaseOrderDeliveryDetails();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
