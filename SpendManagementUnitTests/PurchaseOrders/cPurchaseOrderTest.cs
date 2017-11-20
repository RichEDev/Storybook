using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderTest and is intended
    ///to contain all cPurchaseOrderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderTest
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Title = expected;
            actual = target.Title;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Supplier
        ///</summary>
        [TestMethod()]
        public void SupplierTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            cSupplier expected = null; // TODO: Initialize to an appropriate value
            cSupplier actual;
            target.Supplier = expected;
            actual = target.Supplier;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderTotal
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderTotalTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.PurchaseOrderTotal;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderState
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderStateTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            PurchaseOrderState expected = new PurchaseOrderState(); // TODO: Initialize to an appropriate value
            PurchaseOrderState actual;
            target.PurchaseOrderState = expected;
            actual = target.PurchaseOrderState;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderProducts
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderProductsTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderProduct> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderProduct> actual;
            target.PurchaseOrderProducts = expected;
            actual = target.PurchaseOrderProducts;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderNumber
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderNumberTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PurchaseOrderID = expected;
            actual = target.PurchaseOrderID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParentPurchaseOrderID
        ///</summary>
        [TestMethod()]
        public void ParentPurchaseOrderIDTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<int> expected = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            target.ParentPurchaseOrderID = expected;
            actual = target.ParentPurchaseOrderID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderType
        ///</summary>
        [TestMethod()]
        public void OrderTypeTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.OrderStartDate = expected;
            actual = target.OrderStartDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrderRecurrence
        ///</summary>
        [TestMethod()]
        public void OrderRecurrenceTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.OrderEndDate = expected;
            actual = target.OrderEndDate;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedOn
        ///</summary>
        [TestMethod()]
        public void ModifiedOnTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<int> expected = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            target.ModifiedBy = expected;
            actual = target.ModifiedBy;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HistoryItems
        ///</summary>
        [TestMethod()]
        public void HistoryItemsTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            List<cPurchaseOrderHistoryItem> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderHistoryItem> actual;
            target.HistoryItems = expected;
            actual = target.HistoryItems;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeliveryRecords
        ///</summary>
        [TestMethod()]
        public void DeliveryRecordsTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveries> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveries> actual;
            target.DeliveryRecords = expected;
            actual = target.DeliveryRecords;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DateOrdered
        ///</summary>
        [TestMethod()]
        public void DateOrderedTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.DateOrdered = expected;
            actual = target.DateOrdered;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DateApproved
        ///</summary>
        [TestMethod()]
        public void DateApprovedTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.DateApproved = expected;
            actual = target.DateApproved;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Currency
        ///</summary>
        [TestMethod()]
        public void CurrencyTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            cCurrency expected = null; // TODO: Initialize to an appropriate value
            cCurrency actual;
            target.Currency = expected;
            actual = target.Currency;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
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
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Comments = expected;
            actual = target.Comments;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ChildPurchaseOrders
        ///</summary>
        [TestMethod()]
        public void ChildPurchaseOrdersTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            target.ChildPurchaseOrders = expected;
            actual = target.ChildPurchaseOrders;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CalendarPoints
        ///</summary>
        [TestMethod()]
        public void CalendarPointsTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            target.CalendarPoints = expected;
            actual = target.CalendarPoints;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod()]
        public void CloneTest()
        {
            cPurchaseOrder target = new cPurchaseOrder(); // TODO: Initialize to an appropriate value
            cPurchaseOrder expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrder actual;
            actual = target.Clone();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cPurchaseOrder Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderConstructorTest1()
        {
            int poID = 0; // TODO: Initialize to an appropriate value
            cSupplier supplier = null; // TODO: Initialize to an appropriate value
            string title = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> dateApproved = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> dateOrdered = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            PurchaseOrderType orderType = new PurchaseOrderType(); // TODO: Initialize to an appropriate value
            Nullable<PurchaseOrderRecurrence> orderRecurrence = new Nullable<PurchaseOrderRecurrence>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderStartDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderEndDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            string comments = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdOn = new DateTime(); // TODO: Initialize to an appropriate value
            int createdBy = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedOn = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedBy = new Nullable<int>(); // TODO: Initialize to an appropriate value
            PurchaseOrderState purchaseOrderState = new PurchaseOrderState(); // TODO: Initialize to an appropriate value
            string purchaseOrderNumber = string.Empty; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveries> deliveryRecords = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderProduct> purchaseOrderProducts = null; // TODO: Initialize to an appropriate value
            cCountry country = null; // TODO: Initialize to an appropriate value
            cCurrency currency = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderHistoryItem> history = null; // TODO: Initialize to an appropriate value
            List<int> childPurchaseOrders = null; // TODO: Initialize to an appropriate value
            Nullable<int> parentPurchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            List<int> calendarPoints = null; // TODO: Initialize to an appropriate value
            cPurchaseOrder target = new cPurchaseOrder(poID, supplier, title, dateApproved, dateOrdered, orderType, orderRecurrence, orderStartDate, orderEndDate, comments, createdOn, createdBy, modifiedOn, modifiedBy, purchaseOrderState, purchaseOrderNumber, deliveryRecords, purchaseOrderProducts, country, currency, history, childPurchaseOrders, parentPurchaseOrderID, calendarPoints);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cPurchaseOrder Constructor
        ///</summary>
        [TestMethod()]
        public void cPurchaseOrderConstructorTest()
        {
            cPurchaseOrder target = new cPurchaseOrder();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
