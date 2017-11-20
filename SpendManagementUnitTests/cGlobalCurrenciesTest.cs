using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cGlobalCurrenciesTest and is intended
    ///to contain all cGlobalCurrenciesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cGlobalCurrenciesTest
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


        ///// <summary>
        /////A test for InitialiseData
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //[DeploymentItem("Spend Management.dll")]
        //public void InitialiseDataTest()
        //{
        //    cGlobalCurrencies_Accessor target = new cGlobalCurrencies_Accessor(); // TODO: Initialize to an appropriate value
        //    target.InitialiseData();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for getModifiedGlobalCurrencies
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getModifiedGlobalCurrenciesTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies(); // TODO: Initialize to an appropriate value
            DateTime date = new DateTime(); // TODO: Initialize to an appropriate value
            Dictionary<int, cGlobalCurrency> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cGlobalCurrency> actual;
            actual = target.getModifiedGlobalCurrencies(date);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getGlobalCurrencyIds
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCurrencyIdsTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            cGlobalCurrency expected = target.getGlobalCurrencyByAlphaCode("GBP");
            List<int> actual;
            actual = target.getGlobalCurrencyIds();
            Assert.IsTrue(actual.Contains(expected.globalcurrencyid));
        }

        /// <summary>
        ///A test for getGlobalCurrencyByNumericCode
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCurrencyByNumericCodeTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            string code = "826";
            cGlobalCurrency expected = target.getGlobalCurrencyByAlphaCode("GBP");
            cGlobalCurrency actual;
            actual = target.getGlobalCurrencyByNumericCode(code);
            cGlobalCurrencyAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getGlobalCurrencyByLabel
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCurrencyByLabelTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            string label = "Pound Sterling";
            cGlobalCurrency expected = target.getGlobalCurrencyByAlphaCode("GBP");
            cGlobalCurrency actual;
            actual = target.getGlobalCurrencyByLabel(label);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getGlobalCurrencyById
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCurrencyByIdTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            int id = target.getGlobalCurrencyByLabel("Pound Sterling").globalcurrencyid;
            cGlobalCurrency expected = target.getGlobalCurrencyByAlphaCode("GBP");
            cGlobalCurrency actual;
            actual = target.getGlobalCurrencyById(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getGlobalCurrencyByAlphaCode
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCurrencyByAlphaCodeTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            string code = "GBP";
            cGlobalCurrency expected = target.getGlobalCurrencyByNumericCode("826");
            cGlobalCurrency actual;
            actual = target.getGlobalCurrencyByAlphaCode(code);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void CreateDropDownTest()
        {
            cGlobalCurrencies target = new cGlobalCurrencies();
            List<int> expected = target.getGlobalCurrencyIds();
            List<ListItem> actual;
            actual = target.CreateDropDown();
            foreach (ListItem li in actual)
            {
                Assert.IsTrue(expected.Contains(Convert.ToInt32(li.Value)));
            }
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void CacheListTest()
        {
            cGlobalCurrencies_Accessor target = new cGlobalCurrencies_Accessor();
            SortedList<int, cGlobalCurrency> expected = null;
            SortedList<int, cGlobalCurrency> actual;
            actual = target.CacheList();

            Assert.AreEqual(expected, actual);
        }
    }
}
