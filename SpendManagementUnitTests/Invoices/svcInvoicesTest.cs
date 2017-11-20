using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for svcInvoicesTest and is intended
    ///to contain all svcInvoicesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class svcInvoicesTest
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
            svcInvoices target = new svcInvoices(); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            string purchaseOrderNumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<int> contractID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string supplierName = string.Empty; // TODO: Initialize to an appropriate value
            string invoiceNumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> receivedDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> dueDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> invoiceStatus = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string comment = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> paidDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> coverPeriodEnd = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            string paymentReference = string.Empty; // TODO: Initialize to an appropriate value
            object[] lineItems = null; // TODO: Initialize to an appropriate value
            Nullable<int> invoiceState = new Nullable<int>(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SaveInvoice(invoiceID, purchaseOrderNumber, contractID, supplierName, invoiceNumber, receivedDate, dueDate, invoiceStatus, comment, paidDate, coverPeriodEnd, paymentReference, lineItems, invoiceState);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for RejectInvoice
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void RejectInvoiceTest()
        //{
        //    svcInvoices target = new svcInvoices(); // TODO: Initialize to an appropriate value
        //    int invoiceID = 0; // TODO: Initialize to an appropriate value
        //    string reasonForRejection = string.Empty; // TODO: Initialize to an appropriate value
        //    target.RejectInvoice(invoiceID, reasonForRejection);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for HelloWorld
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void HelloWorldTest()
        {
            svcInvoices target = new svcInvoices(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.HelloWorld();
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
            svcInvoices target = new svcInvoices(); // TODO: Initialize to an appropriate value
            int invoiceID = 0; // TODO: Initialize to an appropriate value
            cInvoice expected = null; // TODO: Initialize to an appropriate value
            cInvoice actual;
            actual = target.GetInvoiceApprovalDetails(invoiceID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for ApproveInvoice
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void ApproveInvoiceTest()
        //{
        //    svcInvoices target = new svcInvoices(); // TODO: Initialize to an appropriate value
        //    int invoiceID = 0; // TODO: Initialize to an appropriate value
        //    target.ApproveInvoice(invoiceID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for svcInvoices Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void svcInvoicesConstructorTest()
        {
            svcInvoices target = new svcInvoices();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
