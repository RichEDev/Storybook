using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using Spend_Management;
using System;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cRangeCurrenciesTest and is intended
    ///to contain all cRangeCurrenciesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cRangeCurrenciesTest
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
        ///A test for deleteCurrencyRange
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void deleteCurrencyRangeTest()
        {
            cRangeCurrencies target = null;
            int CurrencyID = 0;

            try
            {
                cCurrencyRange currRange = CreateRangeCurrency();
                target = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                CurrencyID = currRange.currencyid; // TODO: Initialize to an appropriate value
                int CurrencyRangeID = currRange.currencyrangeid; // TODO: Initialize to an appropriate value
                target.deleteCurrencyRange(CurrencyID, CurrencyRangeID);

                cRangeCurrency range = target.getCurrencyById(CurrencyID);

                int expected = 0;

                if (range.exchangerates.Count > 0)
                {
                    if (range.exchangerates.ContainsKey(CurrencyRangeID))
                    {
                        expected = range.exchangerates.Keys[0];
                    }
                }

                Assert.AreNotEqual(expected, CurrencyRangeID);
            }
            finally
            {
                if (target != null)
                {
                    target.deleteCurrency(CurrencyID);
                }
            }
        }

        /// <summary>
        ///A test for saveCurrencyRange
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void saveCurrencyRangeTest()
        {
            cRangeCurrencies target = null;
            int CurrencyID = 0;

            try
            {
                cRangeCurrency currency = CreateCurrency();
                int currencyID = currency.currencyid;
                target = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

                cCurrencyRange newRange = new cCurrencyRange(cGlobalVariables.AccountID, currencyID, 0, new DateTime(2009, 06, 01), new DateTime(2009, 06, 15), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, CreateExchangeRates(currencyID, CurrencyType.Monthly));
                int CurrencyRangeID = target.saveCurrencyRange(newRange);

                cRangeCurrency rangeCurrency = target.getCurrencyById(currencyID);
                cCurrencyRange actual = rangeCurrency.exchangerates.Values[0];

                Assert.AreEqual(CurrencyRangeID, actual.currencyrangeid);
                Assert.AreEqual(newRange.startdate, actual.startdate);
                Assert.AreEqual(newRange.enddate, actual.enddate);
                Assert.AreEqual(newRange.createdon, actual.createdon);
                Assert.AreEqual(newRange.createdby, actual.createdby);
                Assert.AreEqual(newRange.exchangerates.Count, actual.exchangerates.Count);

                cCurrencyRange updatedRange = new cCurrencyRange(cGlobalVariables.AccountID, currencyID, CurrencyRangeID, new DateTime(2009, 06, 03), new DateTime(2009, 06, 16), DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, newRange.exchangerates);

                int updatedCurrencyRangeID = target.saveCurrencyRange(updatedRange);

                rangeCurrency = target.getCurrencyById(currencyID);
                actual = rangeCurrency.exchangerates.Values[0];

                //Check the updated values
                Assert.AreEqual(CurrencyRangeID, actual.currencyrangeid);
                Assert.AreEqual(updatedRange.startdate, actual.startdate);
                Assert.AreEqual(updatedRange.enddate, actual.enddate);
                Assert.AreEqual(updatedRange.createdon, actual.createdon);
                Assert.AreEqual(updatedRange.createdby, actual.createdby);
                Assert.AreEqual(updatedRange.exchangerates.Count, actual.exchangerates.Count);

                //Check the added and updated values are different
                Assert.AreNotEqual(updatedRange.startdate, newRange.startdate);
                Assert.AreNotEqual(updatedRange.enddate, newRange.enddate);

            }
            finally
            {
                if (target != null)
                {
                    target.deleteCurrency(CurrencyID);
                }
            }
        }

        private cRangeCurrency CreateCurrency()
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobalCurrencies.getGlobalCurrencyByLabel("Afghani");
            cRangeCurrencies target = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            //New currency
            cRangeCurrency newCurrency = new cRangeCurrency(cGlobalVariables.AccountID, 0, globCurr.globalcurrencyid, 1, 1, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, new SortedList<int,cCurrencyRange>());
            cGlobalVariables.CurrencyID = target.saveCurrency(newCurrency);
            newCurrency = target.getCurrencyById(cGlobalVariables.CurrencyID);
            return newCurrency;
        }

        private cCurrencyRange CreateRangeCurrency()
        {
            cRangeCurrency currency = CreateCurrency();
            int currencyID = currency.currencyid;
            cRangeCurrencies target = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cCurrencyRange newRange = new cCurrencyRange(cGlobalVariables.AccountID, currencyID, 0, new DateTime(2009, 06, 01), new DateTime(2009, 06, 15), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, CreateExchangeRates(currencyID, CurrencyType.Range));

            int CurrencyRangeID = target.saveCurrencyRange(newRange);
            currency = target.getCurrencyById(currencyID);
            newRange = currency.exchangerates.Values[0];
            return newRange;
        }

        private SortedList<int, double> CreateExchangeRates(int CurrencyID, CurrencyType currType)
        {
            cGlobalVariables.CurrencyID = CurrencyID;
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            SortedList<int, double> exchangerates = new SortedList<int, double>();

            int val = 1;

            foreach (int id in target.currencyList.Keys)
            {
                if (id != CurrencyID)
                {
                    exchangerates.Add(id, val);
                    val++;
                }
            }

            //target.addExchangeRates(CurrencyID, currType, exchangerates, DateTime.UtcNow, cGlobalVariables.EmployeeID);

            return exchangerates;
        }
    }
}
