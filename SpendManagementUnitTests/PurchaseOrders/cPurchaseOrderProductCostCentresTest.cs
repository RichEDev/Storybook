﻿using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrderProductCostCentresTest and is intended
    ///to contain all cPurchaseOrderProductCostCentresTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrderProductCostCentresTest
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
        ///A test for GetPurchaseOrderProductCostCentresByPurchaseOrderProductId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderProductCostCentresByPurchaseOrderProductIdTest()
        {
            int accountID = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrderProductCostCentres target = new cPurchaseOrderProductCostCentres(accountID); // TODO: Initialize to an appropriate value
            int purchaseOrderProductId = 0; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> expected = null; // TODO: Initialize to an appropriate value
            List<cPurchaseOrderProductCostCentre> actual;
            actual = target.GetPurchaseOrderProductCostCentresByPurchaseOrderProductId(purchaseOrderProductId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
