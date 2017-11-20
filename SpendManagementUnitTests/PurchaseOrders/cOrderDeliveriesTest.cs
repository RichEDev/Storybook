using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cOrderDeliveriesTest and is intended
    ///to contain all cOrderDeliveriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cOrderDeliveriesTest
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
            int accountID = 0; // TODO: Initialize to an appropriate value
            cOrderDeliveries target = new cOrderDeliveries(accountID); // TODO: Initialize to an appropriate value
            int deliveryID = 0; // TODO: Initialize to an appropriate value
            int locationID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            DateTime deliveryDate = new DateTime(); // TODO: Initialize to an appropriate value
            string deliveryReference = string.Empty; // TODO: Initialize to an appropriate value
            List<int> productID = null; // TODO: Initialize to an appropriate value
            List<Decimal> delivered = null; // TODO: Initialize to an appropriate value
            List<Decimal> returned = null; // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SavePurchaseOrderDelivery(deliveryID, locationID, purchaseOrderID, deliveryDate, deliveryReference, productID, delivered, returned, employeeID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDeliveryRecordsByPurchaseOrderID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetDeliveryRecordsByPurchaseOrderIDTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cOrderDeliveries target = new cOrderDeliveries(accountID); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, Dictionary<int, cPurchaseOrderDeliveryDetails>> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, Dictionary<int, cPurchaseOrderDeliveryDetails>> actual;
            actual = target.GetDeliveryRecordsByPurchaseOrderID(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDeliveriesByPurchaseOrderID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetDeliveriesByPurchaseOrderIDTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cOrderDeliveries target = new cOrderDeliveries(accountID); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveries> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cPurchaseOrderDeliveries> actual;
            actual = target.GetDeliveriesByPurchaseOrderID(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for DeletePurchaseOrderDelivery
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void DeletePurchaseOrderDeliveryTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cOrderDeliveries target = new cOrderDeliveries(accountID); // TODO: Initialize to an appropriate value
        //    int deliveryID = 0; // TODO: Initialize to an appropriate value
        //    int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
        //    int employeeID = 0; // TODO: Initialize to an appropriate value
        //    target.DeletePurchaseOrderDelivery(deliveryID, purchaseOrderID, employeeID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}
    }
}
