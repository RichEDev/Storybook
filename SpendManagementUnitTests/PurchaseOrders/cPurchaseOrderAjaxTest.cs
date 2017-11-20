using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderAjaxTest and is intended
    ///to contain all cPurchaseOrderAjaxTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderAjaxTest
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
        ///A test for Title
        ///</summary>
        [TestMethod()]
        public void TitleTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Title = expected;
            actual = target.Title;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SupplierName
        ///</summary>
        [TestMethod()]
        public void SupplierNameTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.SupplierName = expected;
            actual = target.SupplierName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderNumber
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderNumberTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.PurchaseOrderNumber = expected;
            actual = target.PurchaseOrderNumber;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderID
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderIDTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PurchaseOrderID = expected;
            actual = target.PurchaseOrderID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Products
        ///</summary>
        [TestMethod()]
        public void ProductsTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProduct> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProduct> actual;
            target.Products = expected;
            actual = target.Products;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderTypeString
        ///</summary>
        [TestMethod()]
        public void OrderTypeStringTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.OrderTypeString = expected;
            actual = target.OrderTypeString;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderType
        ///</summary>
        [TestMethod()]
        public void OrderTypeTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            PurchaseOrderType expected = new PurchaseOrderType(); // TODO: Initialize to an appropriate value
            PurchaseOrderType actual;
            target.OrderType = expected;
            actual = target.OrderType;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderStartDate
        ///</summary>
        [TestMethod()]
        public void OrderStartDateTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.OrderStartDate = expected;
            actual = target.OrderStartDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderRecurrenceString
        ///</summary>
        [TestMethod()]
        public void OrderRecurrenceStringTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.OrderRecurrenceString;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderRecurrence
        ///</summary>
        [TestMethod()]
        public void OrderRecurrenceTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            Nullable<PurchaseOrderRecurrence> expected = new Nullable<PurchaseOrderRecurrence>(); // TODO: Initialize to an appropriate value
            Nullable<PurchaseOrderRecurrence> actual;
            target.OrderRecurrence = expected;
            actual = target.OrderRecurrence;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderEndDate
        ///</summary>
        [TestMethod()]
        public void OrderEndDateTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.OrderEndDate = expected;
            actual = target.OrderEndDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HistoryItems
        ///</summary>
        [TestMethod()]
        public void HistoryItemsTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderHistoryItem> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderHistoryItem> actual;
            target.HistoryItems = expected;
            actual = target.HistoryItems;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GlobalCountry
        ///</summary>
        [TestMethod()]
        public void GlobalCountryTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            cGlobalCountry expected = null; // TODO: Initialize to an appropriate value
            cGlobalCountry actual;
            target.GlobalCountry = expected;
            actual = target.GlobalCountry;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EmployeeName
        ///</summary>
        [TestMethod()]
        public void EmployeeNameTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.EmployeeName = expected;
            actual = target.EmployeeName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CurrencyName
        ///</summary>
        [TestMethod()]
        public void CurrencyNameTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.CurrencyName = expected;
            actual = target.CurrencyName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Currency
        ///</summary>
        [TestMethod()]
        public void CurrencyTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            cCurrency expected = null; // TODO: Initialize to an appropriate value
            cCurrency actual;
            target.Currency = expected;
            actual = target.Currency;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.CreatedBy = expected;
            actual = target.CreatedBy;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Country
        ///</summary>
        [TestMethod()]
        public void CountryTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            cCountry expected = null; // TODO: Initialize to an appropriate value
            cCountry actual;
            target.Country = expected;
            actual = target.Country;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Comments
        ///</summary>
        [TestMethod()]
        public void CommentsTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Comments = expected;
            actual = target.Comments;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CalendarPoints
        ///</summary>
        [TestMethod()]
        public void CalendarPointsTest()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(); // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            target.CalendarPoints = expected;
            actual = target.CalendarPoints;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrderAjax Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderAjaxConstructorTest2()
        {
            cPurchaseOrderAjax target = new cPurchaseOrderAjax();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderAjax Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderAjaxConstructorTest1()
        {
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            string purchaseOrderNumber = string.Empty; // TODO: Initialize to an appropriate value
            string supplierName = string.Empty; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderProduct> products = null; // TODO: Initialize to an appropriate value
            string title = string.Empty; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(purchaseOrderID, purchaseOrderNumber, supplierName, products, title);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrderAjax Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderAjaxConstructorTest()
        {
            cPurchaseOrder po = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax target = new cPurchaseOrderAjax(po);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
