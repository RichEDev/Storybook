using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInvoicesTest and is intended
    ///to contain all cInvoicesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInvoicesTest
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
        ///A test for SaveLineItem
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SaveLineItemTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            cInvoiceLineItem lineItem = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SaveLineItem(lineItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SaveInvoice
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SaveInvoiceTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            Nullable<int> purchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> contractID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> supplierID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string invoiceNumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> receivedDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> dueDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> invoiceStatus = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> paidDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> coverPeriodEnd = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            string paymentReference = string.Empty; // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            Nullable<int> invoiceState = new Nullable<int>(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SaveInvoice(invoiceID, purchaseOrderID, contractID, supplierID, invoiceNumber, receivedDate, dueDate, invoiceStatus, comment, paidDate, coverPeriodEnd, paymentReference, employeeID, invoiceState);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for saveHistoryItem
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void saveHistoryItemTest1()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
        //    int invoiceId = 0; // TODO: Initialize to an appropriate value
        //    string comment = string.Empty; // TODO: Initialize to an appropriate value
        //    string createdbystring = string.Empty; // TODO: Initialize to an appropriate value
        //    DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
        //    int createdby = 0; // TODO: Initialize to an appropriate value
        //    target.saveHistoryItem(invoiceId, comment, createdbystring, createdon, createdby);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for saveHistoryItem
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void saveHistoryItemTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
        //    cInvoiceHistoryItem item = null; // TODO: Initialize to an appropriate value
        //    target.saveHistoryItem(item);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for GetLineItemsByInvoiceID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetLineItemsByInvoiceIDTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItem> actual;
            actual = target.GetLineItemsByInvoiceID(invoiceID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInvoiceLineItemsByInvoiceLineItemId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetInvoiceLineItemsByInvoiceLineItemIdTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            int invoiceLineItemId = 0; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItemCostCentre> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceLineItemCostCentre> actual;
            actual = target.GetInvoiceLineItemsByInvoiceLineItemId(invoiceLineItemId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getInvoiceHistory
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void getInvoiceHistoryTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            cInvoices_Accessor target = new cInvoices_Accessor(param0); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> expected = null; // TODO: Initialize to an appropriate value
            List<cInvoiceHistoryItem> actual;
            actual = target.getInvoiceHistory(invoiceID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInvoiceFromDB
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void GetInvoiceFromDBTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            cInvoices_Accessor target = new cInvoices_Accessor(param0); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            cInvoice expected = null; // TODO: Initialize to an appropriate value
            cInvoice actual;
            actual = target.GetInvoiceFromDB(invoiceID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInvoiceByID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetInvoiceByIDTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            cInvoice expected = null; // TODO: Initialize to an appropriate value
            cInvoice actual;
            actual = target.GetInvoiceByID(invoiceID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInvoiceApprovalDetails
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetInvoiceApprovalDetailsTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            cInvoice expected = null; // TODO: Initialize to an appropriate value
            cInvoice actual;
            actual = target.GetInvoiceApprovalDetails(invoiceID, employeeID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for DeleteLineItems
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void DeleteLineItemsTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
        //    int invoiceID = 0; // TODO: Initialize to an appropriate value
        //    int employeeID = 0; // TODO: Initialize to an appropriate value
        //    target.DeleteLineItems(invoiceID, employeeID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DeleteInvoice
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void DeleteInvoiceTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
        //    int invoiceID = 0; // TODO: Initialize to an appropriate value
        //    target.DeleteInvoice(invoiceID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for ChangeInvoiceState
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void ChangeInvoiceStateTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cInvoices target = new cInvoices(accountID); // TODO: Initialize to an appropriate value
        //    int invoiceID = 0; // TODO: Initialize to an appropriate value
        //    InvoiceState approvalState = new InvoiceState(); // TODO: Initialize to an appropriate value
        //    int employeeID = 0; // TODO: Initialize to an appropriate value
        //    target.ChangeInvoiceState(invoiceID, approvalState, employeeID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}
    }
}
