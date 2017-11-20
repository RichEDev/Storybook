using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInvoiceTest and is intended
    ///to contain all cInvoiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInvoiceTest
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
        ///A test for TotalInvoiceAmount
        ///</summary>
        [TestMethod()]
        public void TotalInvoiceAmountTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.TotalInvoiceAmount;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Supplier
        ///</summary>
        [TestMethod()]
        public void SupplierTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            cSupplier actual;
            actual = target.Supplier;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PurchaseOrderID
        ///</summary>
        [TestMethod()]
        public void PurchaseOrderIDTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.PurchaseOrderID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for POStartDate
        ///</summary>
        [TestMethod()]
        public void POStartDateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.POStartDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PONumber
        ///</summary>
        [TestMethod()]
        public void PONumberTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PONumber;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for POMaxValue
        ///</summary>
        [TestMethod()]
        public void POMaxValueTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<Decimal> actual;
            actual = target.POMaxValue;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for POExpiryDate
        ///</summary>
        [TestMethod()]
        public void POExpiryDateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.POExpiryDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PaymentReference
        ///</summary>
        [TestMethod()]
        public void PaymentReferenceTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PaymentReference;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedOn
        ///</summary>
        [TestMethod()]
        public void ModifiedOnTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
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
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.ModifiedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LineItems
        ///</summary>
        [TestMethod()]
        public void LineItemsTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> actual;
            target.LineItems = expected;
            actual = target.LineItems;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceStatus
        ///</summary>
        [TestMethod()]
        public void InvoiceStatusTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            cInvoiceStatus actual;
            actual = target.InvoiceStatus;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceState
        ///</summary>
        [TestMethod()]
        public void InvoiceStateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<InvoiceState> actual;
            actual = target.InvoiceState;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceReceivedDate
        ///</summary>
        [TestMethod()]
        public void InvoiceReceivedDateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.InvoiceReceivedDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoicePaidDate
        ///</summary>
        [TestMethod()]
        public void InvoicePaidDateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.InvoicePaidDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceNumber
        ///</summary>
        [TestMethod()]
        public void InvoiceNumberTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.InvoiceNumber;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceId
        ///</summary>
        [TestMethod()]
        public void InvoiceIdTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.InvoiceId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceDueDate
        ///</summary>
        [TestMethod()]
        public void InvoiceDueDateTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.InvoiceDueDate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HistoryItems
        ///</summary>
        [TestMethod()]
        public void HistoryItemsTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> actual;
            target.HistoryItems = expected;
            actual = target.HistoryItems;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedOn
        ///</summary>
        [TestMethod()]
        public void CreatedOnTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.CreatedOn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreatedBy
        ///</summary>
        [TestMethod()]
        public void CreatedByTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CreatedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CoverPeriodEnd
        ///</summary>
        [TestMethod()]
        public void CoverPeriodEndTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.CoverPeriodEnd;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ContractId
        ///</summary>
        [TestMethod()]
        public void ContractIdTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.ContractId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Comment
        ///</summary>
        [TestMethod()]
        public void CommentTest()
        {
            cInvoice target = new cInvoice(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Comment;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cInvoice Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceConstructorTest2()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            Nullable<int> purchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> contractid = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cSupplier supplier = null; // TODO: Initialize to an appropriate value
            string invoicenumber = string.Empty; // TODO: Initialize to an appropriate value
            string po_number = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> po_startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> po_expirydate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<Decimal> po_maxvalue = new Nullable<Decimal>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoicereceiveddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoiceduedate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<Decimal> totalinvoiceamount = new Nullable<Decimal>(); // TODO: Initialize to an appropriate value
            cInvoiceStatus invoiceStatus = null; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoicepaiddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> coverperiodend = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            string payment_ref = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> items = null; // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> historyItems = null; // TODO: Initialize to an appropriate value
            cInvoice target = new cInvoice(invoiceid, purchaseOrderID, contractid, supplier, invoicenumber, po_number, po_startdate, po_expirydate, po_maxvalue, invoicereceiveddate, invoiceduedate, totalinvoiceamount, invoiceStatus, comment, invoicepaiddate, coverperiodend, payment_ref, createdon, createdby, modifiedon, modifiedby, items, historyItems);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cInvoice Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceConstructorTest1()
        {
            int invoiceid = 0; // TODO: Initialize to an appropriate value
            Nullable<int> purchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> contractid = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cSupplier supplier = null; // TODO: Initialize to an appropriate value
            string invoicenumber = string.Empty; // TODO: Initialize to an appropriate value
            string po_number = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> po_startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> po_expirydate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<Decimal> po_maxvalue = new Nullable<Decimal>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoicereceiveddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoiceduedate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<Decimal> totalinvoiceamount = new Nullable<Decimal>(); // TODO: Initialize to an appropriate value
            cInvoiceStatus invoiceStatus = null; // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> invoicepaiddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> coverperiodend = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            string payment_ref = string.Empty; // TODO: Initialize to an appropriate value
            DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> items = null; // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> historyItems = null; // TODO: Initialize to an appropriate value
            InvoiceState invoicestate = new InvoiceState(); // TODO: Initialize to an appropriate value
            cInvoice target = new cInvoice(invoiceid, purchaseOrderID, contractid, supplier, invoicenumber, po_number, po_startdate, po_expirydate, po_maxvalue, invoicereceiveddate, invoiceduedate, totalinvoiceamount, invoiceStatus, comment, invoicepaiddate, coverperiodend, payment_ref, createdon, createdby, modifiedon, modifiedby, items, historyItems, invoicestate);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cInvoice Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceConstructorTest()
        {
            cInvoice target = new cInvoice();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
