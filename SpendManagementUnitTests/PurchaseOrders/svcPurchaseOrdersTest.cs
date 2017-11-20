using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;
using System.Collections.Generic;
using SpendManagementLibrary;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for svcPurchaseOrdersTest and is intended
    ///to contain all svcPurchaseOrdersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class svcPurchaseOrdersTest
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
        ///A test for SetPurchaseOrderSubmissionStatus
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SetPurchaseOrderSubmissionStatusTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            bool submit = false; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SetPurchaseOrderSubmissionStatus(purchaseOrderID, submit);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SavePurchaseOrderDelivery
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SavePurchaseOrderDeliveryTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            int locationID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            DateTime deliveryDate = new DateTime(); // TODO: Initialize to an appropriate value
            string deliveryReference = string.Empty; // TODO: Initialize to an appropriate value
            object[] productID = null; // TODO: Initialize to an appropriate value
            object[] delivered = null; // TODO: Initialize to an appropriate value
            object[] returned = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SavePurchaseOrderDelivery(deliveryID, locationID, purchaseOrderID, deliveryDate, deliveryReference, productID, delivered, returned);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SavePurchaseOrder
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SavePurchaseOrderTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            string title = string.Empty; // TODO: Initialize to an appropriate value
            string supplier = string.Empty; // TODO: Initialize to an appropriate value
            int countryID = 0; // TODO: Initialize to an appropriate value
            int currencyID = 0; // TODO: Initialize to an appropriate value
            string comments = string.Empty; // TODO: Initialize to an appropriate value
            short orderType = 0; // TODO: Initialize to an appropriate value
            Nullable<short> orderReccurrence = new Nullable<short>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderStartDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderEndDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            object[] calendarPoints = null; // TODO: Initialize to an appropriate value
            object[] products = null; // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.SavePurchaseOrder(purchaseOrderID, title, supplier, countryID, currencyID, comments, orderType, orderReccurrence, orderStartDate, orderEndDate, calendarPoints, products);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }


        /// <summary>
        ///A test for GetPurchaseOrderForInvoiceByPurchaseOrderNumber
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderForInvoiceByPurchaseOrderNumberTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            string poNumber = string.Empty; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax actual;
            actual = target.GetPurchaseOrderForInvoiceByPurchaseOrderNumber(poNumber);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderDeliveryDetails
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderDeliveryDetailsTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderDeliveriesAJAX> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderDeliveriesAJAX> actual;
            actual = target.GetPurchaseOrderDeliveryDetails(purchaseOrderID, deliveryID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderApprovalDetails
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderApprovalDetailsTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrderAjax actual;
            actual = target.GetPurchaseOrderApprovalDetails(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetProductTotalWithTaxByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetProductTotalWithTaxByUnitCostTimesQuantityTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int tableRowIndex = 0; // TODO: Initialize to an appropriate value
            Decimal unitQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal unitPrice = new Decimal(); // TODO: Initialize to an appropriate value
            int salesTaxID = 0; // TODO: Initialize to an appropriate value
            bool aspElement = false; // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetProductTotalWithTaxByUnitCostTimesQuantity(tableRowIndex, unitQuantity, unitPrice, salesTaxID, aspElement);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetProductTotalByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetProductTotalByUnitCostTimesQuantityTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int tableRowIndex = 0; // TODO: Initialize to an appropriate value
            Decimal unitQuantity = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal unitPrice = new Decimal(); // TODO: Initialize to an appropriate value
            bool aspElement = false; // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetProductTotalByUnitCostTimesQuantity(tableRowIndex, unitQuantity, unitPrice, aspElement);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeletePurchaseOrderDelivery
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void DeletePurchaseOrderDeliveryTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.DeletePurchaseOrderDelivery(deliveryID, purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeletePurchaseOrder
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void DeletePurchaseOrderTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.DeletePurchaseOrder(purchaseOrderID);

            if (expected != actual)
            {
                Assert.Fail("this test has not passed because he likes apples");
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }


        ///// <summary>
        /////A test for ApprovePurchaseOrder
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void ApprovePurchaseOrderTest()
        //{
        //    svcPurchaseOrders target = new svcPurchaseOrders(); // TODO: Initialize to an appropriate value
        //    int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
        //    target.ApprovePurchaseOrder(purchaseOrderID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for svcPurchaseOrders Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void svcPurchaseOrdersConstructorTest()
        {
            svcPurchaseOrders target = new svcPurchaseOrders();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
