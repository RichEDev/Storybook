using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderProductTest and is intended
    ///to contain all cPurchaseOrderProductTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderProductTest
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
        ///A test for UnitOfMeasure
        ///</summary>
        [TestMethod()]
        public void UnitOfMeasureTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            cUnit actual;
            actual = target.UnitOfMeasure;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderProductID
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderProductIDTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PurchaseOrderProductID = expected;
            actual = target.PurchaseOrderProductID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderID
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderIDTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PurchaseOrderID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProductUnitPrice
        ///</summary>
        [TestMethod()]
        public void ProductUnitPriceTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.ProductUnitPrice;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProductQuantity
        ///</summary>
        [TestMethod()]
        public void ProductQuantityTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.ProductQuantity;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Product
        ///</summary>
        [TestMethod()]
        public void ProductTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            cProduct actual;
            actual = target.Product;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CostCentreBreakdown
        ///</summary>
        [TestMethod()]
        public void CostCentreBreakdownTest()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> actual;
            target.CostCentreBreakdown = expected;
            actual = target.CostCentreBreakdown;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderProduct Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderProductConstructorTest2()
        {
            cPurchaseOrderProduct target = new cPurchaseOrderProduct();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderProduct Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderProductConstructorTest1()
        {
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cProduct product = null; // TODO: Initialize to an appropriate value
            cUnit unitOfMeasure = null; // TODO: Initialize to an appropriate value
            Decimal productUnitPrice = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal productQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> costCentres = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(purchaseOrderID, product, unitOfMeasure, productUnitPrice, productQuantity, costCentres);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderProduct Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderProductConstructorTest()
        {
            int purchaseOrderProductID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cProduct product = null; // TODO: Initialize to an appropriate value
            cUnit unitOfMeasure = null; // TODO: Initialize to an appropriate value
            Decimal productUnitPrice = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal productQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> costCentres = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderProduct target = new cPurchaseOrderProduct(purchaseOrderProductID, purchaseOrderID, product, unitOfMeasure, productUnitPrice, productQuantity, costCentres);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
