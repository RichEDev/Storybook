using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInvoiceLineItemTest and is intended
    ///to contain all cInvoiceLineItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInvoiceLineItemTest
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
        ///A test for UnitPrice
        ///</summary>
        [TestMethod()]
        public void UnitPriceTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            Decimal expected = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal actual;
            target.UnitPrice = expected;
            actual = target.UnitPrice;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UnitOfMeasure
        ///</summary>
        [TestMethod()]
        public void UnitOfMeasureTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            cUnit expected = null; // TODO: Initialize to an appropriate value
            cUnit actual;
            target.UnitOfMeasure = expected;
            actual = target.UnitOfMeasure;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SalesTax
        ///</summary>
        [TestMethod()]
        public void SalesTaxTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            cSalesTax expected = null; // TODO: Initialize to an appropriate value
            cSalesTax actual;
            target.SalesTax = expected;
            actual = target.SalesTax;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Quantity
        ///</summary>
        [TestMethod()]
        public void QuantityTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            Decimal expected = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal actual;
            target.Quantity = expected;
            actual = target.Quantity;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Product
        ///</summary>
        [TestMethod()]
        public void ProductTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            cProduct expected = null; // TODO: Initialize to an appropriate value
            cProduct actual;
            target.Product = expected;
            actual = target.Product;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceLineItemID
        ///</summary>
        [TestMethod()]
        public void InvoiceLineItemIDTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.InvoiceLineItemID = expected;
            actual = target.InvoiceLineItemID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceID
        ///</summary>
        [TestMethod()]
        public void InvoiceIDTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.InvoiceID = expected;
            actual = target.InvoiceID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CostCentreBreakdown
        ///</summary>
        [TestMethod()]
        public void CostCentreBreakdownTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem(); // TODO: Initialize to an appropriate value
            List<cInvoiceLineItemCostCentre> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItemCostCentre> actual;
            target.CostCentreBreakdown = expected;
            actual = target.CostCentreBreakdown;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cInvoiceLineItem Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceLineItemConstructorTest1()
        {
            int lineItemID = 0; // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            cProduct product = null; // TODO: Initialize to an appropriate value
            cUnit uom = null; // TODO: Initialize to an appropriate value
            cSalesTax salesTax = null; // TODO: Initialize to an appropriate value
            Decimal unitPrice = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal quantity = new Decimal(); // TODO: Initialize to an appropriate value
            List<cInvoiceLineItemCostCentre> costCentres = null; // TODO: Initialize to an appropriate value
            cInvoiceLineItem target = new cInvoiceLineItem(lineItemID, invoiceID, product, uom, salesTax, unitPrice, quantity, costCentres);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cInvoiceLineItem Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceLineItemConstructorTest()
        {
            cInvoiceLineItem target = new cInvoiceLineItem();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
