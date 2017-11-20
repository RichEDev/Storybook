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
    ///This is a test class for cGlobalCountriesTest and is intended
    ///to contain all cGlobalCountriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cGlobalCountriesTest
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
        ///A test for getModifiedGlobalCountries
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getModifiedGlobalCountriesTest()
        {
            cGlobalCountries target = new cGlobalCountries(); // TODO: Initialize to an appropriate value
            DateTime date = new DateTime(); // TODO: Initialize to an appropriate value
            Dictionary<int, cGlobalCountry> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cGlobalCountry> actual;
            actual = target.getModifiedGlobalCountries(date);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getGlobalCountryIds
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void getGlobalCountryIdsTest()
        {
            cGlobalCountries target = new cGlobalCountries();
            cGlobalCountry country = target.getGlobalCountryByAlphaCode("GB");

            List<int> actual;
            actual = target.getGlobalCountryIds();
            Assert.IsTrue(actual.Contains(country.globalcountryid));
        }

        /// <summary>
        ///A test for getGlobalCountryById
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCountryByIdTest()
        {
            cGlobalCountries target = new cGlobalCountries();
            cGlobalCountry expected = target.getGlobalCountryByAlphaCode("GB");

            cGlobalCountry actual;
            actual = target.getGlobalCountryById(expected.globalcountryid);
            cGlobalCountryAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getGlobalCountryByAlphaCode
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGlobalCountryByAlphaCodeTest()
        {
            cGlobalCountries target = new cGlobalCountries();
            string code = "GB";
            cGlobalCountry expected = target.getCountryByName("United Kingdom");
            cGlobalCountry actual;
            actual = target.getGlobalCountryByAlphaCode(code);
            cGlobalCountryAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getCountryByName
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getCountryByNameTest()
        {
            cGlobalCountries target = new cGlobalCountries();
            string name = "United Kingdom";
            cGlobalCountry expected = target.getGlobalCountryByAlphaCode("GB");
            cGlobalCountry actual;
            actual = target.getCountryByName(name);
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
            cGlobalCountries target = new cGlobalCountries(); // TODO: Initialize to an appropriate value
            List<ListItem> expected = null; // TODO: Initialize to an appropriate value
            List<ListItem> actual;
            actual = target.CreateDropDown();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
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
            cGlobalCountries_Accessor target = new cGlobalCountries_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, cGlobalCountry> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, cGlobalCountry> actual;
            actual = target.CacheList();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cGlobalCountries Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void cGlobalCountriesConstructorTest()
        {
            cGlobalCountries target = new cGlobalCountries();
            List<int> test = target.getGlobalCountryIds();
            Assert.IsTrue(test.GetType() == typeof(List<int>));
        }
    }
}
