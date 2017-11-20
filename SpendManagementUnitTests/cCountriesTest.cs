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
    ///This is a test class for cCountriesTest and is intended
    ///to contain all cCountriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCountriesTest
    {
        static int nGlobalCountryId = 0;
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            if (nGlobalCountryId == 0)
            {
                int accountid = cGlobalVariables.AccountID;
                cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
                cGlobalCountries clsGlobalCountries = new cGlobalCountries();
                cGlobalCountry clsGlobalCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("WF");
                cCountry testExists = target.getCountryByGlobalCountryId(clsGlobalCountry.globalcountryid);
                if (testExists != null)
                {
                    clsGlobalCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("PN");
                    testExists = target.getCountryByGlobalCountryId(clsGlobalCountry.globalcountryid);
                    if (testExists != null)
                    {
                        clsGlobalCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("RE");
                        testExists = target.getCountryByGlobalCountryId(clsGlobalCountry.globalcountryid);
                        if (testExists != null)
                        {
                            clsGlobalCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("TV");
                            testExists = target.getCountryByGlobalCountryId(clsGlobalCountry.globalcountryid);
                        }
                    }
                }

                if (testExists != null)
                {
                    Assert.Fail("Could not create a Global Country Id to use with the tests");
                }
                else
                {
                    nGlobalCountryId = clsGlobalCountry.globalcountryid;
                }
            }
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (nGlobalCountryId != 0)
            {
                int accountid = cGlobalVariables.AccountID;
                cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
                cCountry testExists = target.getCountryByGlobalCountryId(nGlobalCountryId);
                if (testExists != null)
                {
                    target.deleteCountry(testExists.countryid);
                }
            }
        }
        
        #endregion


        ///// <summary>
        /////A test for saveVatRate
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void saveVatRateTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cCountries target = new cCountries(accountid); // TODO: Initialize to an appropriate value
        //    sForeignVatRate vatRate = new sForeignVatRate(); // TODO: Initialize to an appropriate value
        //    int countrysubcatid = 0; // TODO: Initialize to an appropriate value
        //    int employeeid = 0; // TODO: Initialize to an appropriate value
        //    target.saveVatRate(vatRate, countrysubcatid, employeeid);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for saveCountry
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void saveCountryTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int actual;
            actual = target.saveCountry(country);
            Assert.IsTrue(actual > 0);

            int retVal = target.deleteCountry(actual);
        }

        /// <summary>
        ///A test for getModifiedVatRates
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getModifiedVatRatesTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            DateTime date = DateTime.UtcNow.AddSeconds(-2);

            Dictionary<int, sForeignVatRate[]> actual;
            actual = target.getModifiedVatRates(date);
            Assert.IsTrue(actual.Count == 0);

            cSubcats sc = new cSubcats(accountid);
            List<int> lstSubcats = sc.getSubcatIds();
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);

            int newCountry = target.saveCountry(country);
            sForeignVatRate vatRate = new sForeignVatRate();
            vatRate.countryid = newCountry;
            vatRate.subcatid = lstSubcats[0];
            vatRate.vat = 15;
            vatRate.vatpercent = 100;
            target.saveVatRate(vatRate, 0, cGlobalVariables.EmployeeID);

            actual = target.getModifiedVatRates(date);
            Assert.IsTrue(actual.Count > 0);

            int retVal = target.deleteCountry(newCountry);
        }

        /// <summary>
        ///A test for getModifiedCountries
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getModifiedCountriesTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            DateTime date = DateTime.Now.AddSeconds(-2);
            Dictionary<int, cCountry> actual;
            actual = target.getModifiedCountries(date);
            Assert.IsTrue(actual.Count == 0);

            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int newCountry = target.saveCountry(country);

            actual = target.getModifiedCountries(date);
            Assert.IsTrue(actual.Count == 1);

            int retVal = target.deleteCountry(newCountry);
        }

        /// <summary>
        ///A test for getGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getGridTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            string expected = "select countries.countryid, countries.archived, global_countries.country FROM countries";
            string actual;
            actual = target.getGrid();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getForeignVATRates
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //[DeploymentItem("Spend Management.dll")]
        //public void getForeignVATRatesTest()
        //{
        //    PrivateObject param0 = new PrivateObject(cGlobalVariables.AccountID);
        //    cCountries_Accessor target = new cCountries_Accessor(param0);
        //    Dictionary<int, sForeignVatRate> actual;
        //    actual = target.getForeignVATRates();
        //    Assert.IsTrue(actual.Count > 0);
        //}

        /// <summary>
        ///A test for getForeignVATGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getForeignVATGridTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            string expected = "SELECT countrysubcats.countrysubcatid, subcats.subcat, countrysubcats.subcatid, countrysubcats.vat, countrysubcats.vatpercent FROM countrysubcats";
            string actual;
            actual = target.getForeignVATGrid();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getCountryIds
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getCountryIdsTest()
        {
            int accountid = cGlobalVariables.AccountID; // TODO: Initialize to an appropriate value
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID); // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.getCountryIds();
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for getCountryById
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getCountryByIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);

            int expected;
            expected = target.saveCountry(country);

            cCountry actual;
            actual = target.getCountryById(expected);
            Assert.AreEqual(expected, actual.countryid);

            int retVal = target.deleteCountry(actual.countryid);
        }

        /// <summary>
        ///A test for getCountryByGlobalCountryId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getCountryByGlobalCountryIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int id = nGlobalCountryId;
            int newCountry = target.saveCountry(country);

            int expected = newCountry;
            cCountry actual = target.getCountryByGlobalCountryId(id);
            Assert.AreEqual(expected, actual.countryid);

            int retVal = target.deleteCountry(newCountry);
        }

        /// <summary>
        ///A test for getCountryByCode
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getCountryByCodeTest()
        {
            cGlobalCountries clsGCountries = new cGlobalCountries();
            string code = clsGCountries.getGlobalCountryById(nGlobalCountryId).countrycode;

            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int expected = target.saveCountry(country);
            cCountry actual;
            actual = target.getCountryByCode(code);
            Assert.AreEqual(expected, actual.countryid);
        }

        ///// <summary>
        /////A test for deleteVatRate
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void deleteVatRateTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cCountries target = new cCountries(accountid); // TODO: Initialize to an appropriate value
        //    int countrysubcatid = 0; // TODO: Initialize to an appropriate value
        //    target.deleteVatRate(countrysubcatid);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for deleteCountry
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void deleteCountryTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);

            int countryid = target.saveCountry(country);
            int expected = 0;
            int actual;
            actual = target.deleteCountry(countryid);
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
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int countryid = target.saveCountry(country);

            List<ListItem> actual;
            actual = target.CreateDropDown();
            Assert.IsTrue(actual.Count > 0);
            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == countryid.ToString())
                {
                    cnt++;
                }
            }
            Assert.IsTrue(cnt == 1);
            target.deleteCountry(countryid);
        }

        /// <summary>
        ///A test for changeStatus
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void changeStatusTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int countryid = target.saveCountry(country);
            

            bool archive = false;
            int expected = 0;
            int actual;
            actual = target.changeStatus(countryid, archive);
            Assert.AreEqual(expected, actual);

            archive = true;
            actual = target.changeStatus(countryid, archive);
            Assert.AreEqual(expected, actual);

            cCountry newCountry = target.getCountryById(countryid);
            Assert.AreEqual(archive, newCountry.archived);

            target.deleteCountry(countryid);
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
        public void CacheListTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cCountries target = new cCountries(accountid, cGlobalVariables.SubAccountID);
            cCountry country = new cCountry(0, nGlobalCountryId, false, new Dictionary<int, sForeignVatRate>(), DateTime.Now, cGlobalVariables.EmployeeID);
            int countryid = target.saveCountry(country);

            SortedList<int, cCountry> actual;
            actual = target.CacheList();
            Assert.IsTrue(actual.Count > 0, "Sorted list empty");

            Assert.IsTrue(actual.ContainsKey(countryid),"No entry for added country");

            target.deleteCountry(countryid);
        }
    }
}
