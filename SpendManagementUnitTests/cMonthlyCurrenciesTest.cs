using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using SpendManagementLibrary;
using System;
using Spend_Management;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cMonthlyCurrenciesTest and is intended
    ///to contain all cMonthlyCurrenciesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cMonthlyCurrenciesTest
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
        ///A test for getMonthGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void getMonthGridTest()
        {
            int nAccountid = 0; // TODO: Initialize to an appropriate value
            cMonthlyCurrencies target = new cMonthlyCurrencies(nAccountid, cGlobalVariables.SubAccountID); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getMonthGrid();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveCurrencyMonth
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void saveCurrencyMonthTest()
        {
            cMonthlyCurrencies target = null;
            int currencyID = 0;

            try
            {
                cMonthlyCurrency currency = CreateCurrency();
                currencyID = currency.currencyid;
                target = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                
                cCurrencyMonth newMonth = new cCurrencyMonth(cGlobalVariables.AccountID, currencyID, 0, 1, 2009, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, CreateExchangeRates(currencyID, CurrencyType.Monthly));
                int CurrencyMonthID = target.saveCurrencyMonth(newMonth);

                cMonthlyCurrency monthCurrency = target.getCurrencyById(currencyID);
                cCurrencyMonth actual = monthCurrency.exchangerates.Values[0];

                Assert.AreEqual(CurrencyMonthID, actual.currencymonthid);
                Assert.AreEqual(newMonth.month, actual.month);
                Assert.AreEqual(newMonth.year, actual.year);
                Assert.AreEqual(newMonth.createdon, actual.createdon);
                Assert.AreEqual(newMonth.createdby, actual.createdby);
                Assert.AreEqual(newMonth.exchangerates.Count, actual.exchangerates.Count);

                cCurrencyMonth updatedMonth = new cCurrencyMonth(cGlobalVariables.AccountID, currencyID, CurrencyMonthID, 2, 2008, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, newMonth.exchangerates);

                int updatedCurrencyMonthID = target.saveCurrencyMonth(updatedMonth);
                
                monthCurrency = target.getCurrencyById(currencyID);
                actual = monthCurrency.exchangerates.Values[0];

                //Check the updated values
                Assert.AreEqual(CurrencyMonthID, actual.currencymonthid);
                Assert.AreEqual(updatedMonth.month, actual.month);
                Assert.AreEqual(updatedMonth.year, actual.year);
                Assert.AreEqual(updatedMonth.createdon, actual.createdon);
                Assert.AreEqual(updatedMonth.createdby, actual.createdby);
                Assert.AreEqual(updatedMonth.exchangerates.Count, actual.exchangerates.Count);

                //Check the added and updated values are different
                Assert.AreNotEqual(updatedMonth.month, newMonth.month);
                Assert.AreNotEqual(updatedMonth.year, newMonth.year);
            }
            finally
            {
                if (target != null)
                {
                    target.deleteCurrency(currencyID);
                }
            }
        }
       

        /// <summary>
        ///A test for deleteCurrencyMonth
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Visual Studio Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void deleteCurrencyMonthTest()
        {
            cMonthlyCurrencies target = null;
            int CurrencyID = 0;

            try
            {
                cCurrencyMonth currMonth = CreateMonthlyCurrency();
                target = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                CurrencyID = currMonth.currencyid; // TODO: Initialize to an appropriate value
                int CurrencyMonthID = currMonth.currencymonthid; // TODO: Initialize to an appropriate value
                target.deleteCurrencyMonth(CurrencyID, CurrencyMonthID);

                cMonthlyCurrency month = target.getCurrencyById(CurrencyID);

                int expected = 0;

                if (month.exchangerates.Count > 0)
                {
                    if (month.exchangerates.ContainsKey(CurrencyMonthID))
                    {
                        expected = month.exchangerates.Keys[0];
                    }
                }

                Assert.AreNotEqual(expected, CurrencyMonthID);
            }
            finally
            {
                if (target != null)
                {
                    target.deleteCurrency(CurrencyID);
                }
            }
        }

        private cMonthlyCurrency CreateCurrency()
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobalCurrencies.getGlobalCurrencyByLabel("Afghani");
            cMonthlyCurrencies target = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            //New currency
            cMonthlyCurrency newCurrency = new cMonthlyCurrency(cGlobalVariables.AccountID, 0, globCurr.globalcurrencyid, 1, 1, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, new SortedList<int,cCurrencyMonth>());
            cGlobalVariables.CurrencyID = target.saveCurrency(newCurrency);
            newCurrency = target.getCurrencyById(cGlobalVariables.CurrencyID);
            return newCurrency;
        }

        private cCurrencyMonth CreateMonthlyCurrency()
        {
            cMonthlyCurrency currency = CreateCurrency();
            int currencyID = currency.currencyid;
            cMonthlyCurrencies target = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cCurrencyMonth newMonth = new cCurrencyMonth(cGlobalVariables.AccountID, currencyID, 0, 1, 2009, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, CreateExchangeRates(currencyID, CurrencyType.Monthly));

            int CurrencyMonthID = target.saveCurrencyMonth(newMonth);
            currency = target.getCurrencyById(currencyID);
            newMonth = currency.exchangerates.Values[0];
            return newMonth;
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
