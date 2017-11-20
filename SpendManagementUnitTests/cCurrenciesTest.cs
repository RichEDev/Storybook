using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System;
using System.Collections;
using Spend_Management;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCurrenciesTest and is intended
    ///to contain all cCurrenciesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCurrenciesTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            if (cGlobalVariables.CurrencyID == 0)
            {
                cCurrencyObject.CreateCurrency();
            }
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (cGlobalVariables.CurrencyID != 0)
            {
                cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                cCurrency testExists = target.getCurrencyByGlobalCurrencyId(cGlobalVariables.GlobalCurrencyID);
                if (testExists != null)
                {
                    target.deleteCurrency(cGlobalVariables.CurrencyID);
                }
            }

            cGlobalVariables.CurrencyID = 0;
        }
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
        
        #endregion

        /// <summary>
        ///A test for sortList
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_sortListTest()
        {
            cCurrencies_Accessor target = new cCurrencies_Accessor(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            SortedList actual;

            actual = target.sortList();
            if (actual != null && actual.GetType() != typeof(SortedList))
            {
                Assert.Fail("Should return a sorted list");
            }
        }

        /// <summary>
        ///A test for saveCurrency
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_saveCurrencyTest()
        {
            cCurrencies target = null;

            try
            {
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                cCurrency check = target.getCurrencyById(cGlobalVariables.CurrencyID);
                cCurrency newCurrency = new cCurrency(cGlobalVariables.AccountID, 0, cGlobalVariables.GlobalCurrencyID, 1, 1, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);

                //Add new currency
                int existingCurrID = target.saveCurrency(newCurrency);

                //Check that existing currencies are getting checked when adding
                Assert.AreEqual(-1, existingCurrID);

                //Check the newly added and returned currency are the same
                Assert.AreEqual(check.currencyid, cGlobalVariables.CurrencyID);
                Assert.AreEqual(check.globalcurrencyid, newCurrency.globalcurrencyid);
                Assert.AreEqual(check.positiveFormat, newCurrency.positiveFormat);
                Assert.AreEqual(check.negativeFormat, newCurrency.negativeFormat);
                Assert.AreEqual(check.createdon.ToShortDateString(), newCurrency.createdon.ToShortDateString());
                Assert.AreEqual(check.createdby, newCurrency.createdby);

                //Update the currency
                cCurrency updatedCurrency = new cCurrency(cGlobalVariables.AccountID, cGlobalVariables.CurrencyID, cGlobalVariables.GlobalCurrencyID, 2, 2, false, check.createdon, check.createdby, DateTime.UtcNow, cGlobalVariables.EmployeeID);

                //Get the currency id of the updated currency. This should be the same as the newly added one 
                int updatedCurrencyID = target.saveCurrency(updatedCurrency);

                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                check = target.getCurrencyById(updatedCurrencyID);

                //Check the saved and returned values are the same
                Assert.AreEqual(check.currencyid, updatedCurrencyID);
                Assert.AreEqual(check.globalcurrencyid, updatedCurrency.globalcurrencyid);
                Assert.AreNotEqual(check.positiveFormat, updatedCurrency.positiveFormat);
                Assert.AreNotEqual(check.negativeFormat, updatedCurrency.negativeFormat);
                Assert.AreEqual(check.createdon.ToShortDateString(), updatedCurrency.createdon.ToShortDateString());
                Assert.AreEqual(check.createdby, updatedCurrency.createdby);
                Assert.AreEqual(cGlobalVariables.EmployeeID, updatedCurrency.modifiedby);
                Assert.AreNotEqual(check.modifiedon, updatedCurrency.modifiedon);
            }
            finally
            {
                target.deleteCurrency(cGlobalVariables.CurrencyID);
            }
        }
       
        ///// <summary>
        /////A test for getModifiedCurrencies
        /////</summary>
        //[TestMethod()]
        //public void cCurrenciesTest_getModifiedCurrenciesTest()
        //{
        //    DateTime date = DateTime.Now.AddMinutes(-1);
        //    cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

        //    System.Diagnostics.Debugger.Break();
        //    cCurrency expected = target.getCurrencyById(cGlobalVariables.CurrencyID);
        //    SortedList actual;
        //    actual = target.getModifiedCurrencies(date);
        //    Assert.IsTrue(actual.ContainsKey(cGlobalVariables.CurrencyID));
        //}


        /// <summary>
        ///A test for getExchangeRate
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getExchangeRateTest()
        {
            cCurrencies target = null;
            CurrencyType original = CurrencyType.Static;

            try
            {
                //Static Currencies
                SortedList<int, double> exchangeRates = cCurrencyObject.CreateExchangeRates(CurrencyType.Static, cGlobalVariables.GlobalCurrencyID);
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                original = target.currencytype;
                target.changeCurrencyType(CurrencyType.Static, cGlobalVariables.EmployeeID);

                DateTime date = DateTime.UtcNow;

                double actual;
                double expected = 2.2;

                foreach (KeyValuePair<int, double> kp in exchangeRates)
                {
                    actual = target.getExchangeRate(cGlobalVariables.CurrencyID, kp.Key, date);
                    Assert.AreEqual(expected, actual);
                }
            }
            finally
            {
                if (target != null)
                {
                    target.deleteCurrency(cGlobalVariables.CurrencyID);
                    target.changeCurrencyType(original, cGlobalVariables.EmployeeID);
                }
            }
        }

        /// <summary>
        ///A test for getCurrencyIds
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getCurrencyIdsTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            List<int> actual;

            actual = target.getCurrencyIds();
            Assert.IsTrue(actual.Contains(cGlobalVariables.CurrencyID));
        }

        /// <summary>
        ///A test for getCurrencyByNumericCode
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getCurrencyByNumericCodeTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency expected = null;

            expected = target.getCurrencyById(cGlobalVariables.CurrencyID);
            cGlobalCurrencies clsGlobCurrs = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobCurrs.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID);
            string code = globCurr.numericcode;

            cCurrency actual = target.getCurrencyByNumericCode(code);
            cCurrencyAssert.AreEqual(expected, actual);
        }
        
        /// <summary>
        ///A test for GetCurrencyByName
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_GetCurrencyByNameTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency expected = null;

            expected = target.getCurrencyById(cGlobalVariables.CurrencyID);

            cGlobalCurrencies clsGlobCurrs = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobCurrs.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID);
            string name = globCurr.label;

            cCurrency actual = target.GetCurrencyByName(name);
            cCurrencyAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getCurrencyById
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getCurrencyByIdWithValidID()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency actual = target.getCurrencyById(cGlobalVariables.CurrencyID);
            Assert.AreEqual(actual.globalcurrencyid, cGlobalVariables.GlobalCurrencyID);
            Assert.AreEqual(actual.currencyid, cGlobalVariables.CurrencyID);        
        }

        /// <summary>
        ///A test for getCurrencyByGlobalCurrencyId
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getCurrencyByGlobalCurrencyIdTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency expected = null;

            expected = target.getCurrencyById(cGlobalVariables.CurrencyID);

            cCurrency actual = target.getCurrencyByGlobalCurrencyId(cGlobalVariables.GlobalCurrencyID);
            cCurrencyAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getCurrencyByAlphaCode
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getCurrencyByAlphaCodeTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency expected = null;

            expected = target.getCurrencyById(cGlobalVariables.CurrencyID);
            cGlobalCurrencies clsGlobCurrs = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobCurrs.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID);
            string code = globCurr.alphacode;

            cCurrency actual = target.getCurrencyByAlphaCode(code);
            cCurrencyAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getArray
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_getArrayTest()
        {
            cGlobalCurrencies gcurrs = new cGlobalCurrencies();
            cGlobalCurrency gcurr = gcurrs.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID);
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);

            string expected = gcurr.label;
            string[] actual;
            actual = target.getArray();
            Assert.IsTrue(actual.Length > 0);

            int cnt = 0;
            foreach (string str in actual)
            {
                if (str == expected)
                {
                    cnt++;
                }
            }
            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for FormatCurrency
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_FormatCurrencyTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = null;

            decimal value = (decimal)10.00;
            cGlobalCurrencies clsGlobCurrs = new cGlobalCurrencies();
            cGlobalCurrency globCurr = clsGlobCurrs.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID);

            bool FormatForEdit = true;

            object expected = "10.00";
            object actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            value = (decimal)-10.00;
            expected = "-10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);


            FormatForEdit = false;
            value = (decimal)10.00;

            //Positive Format 1
            expected = globCurr.symbol + "10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Positive Format 2
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 2, 1);
            expected = "10.00" + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Positive Format 3
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 3, 1);
            expected = globCurr.symbol + " 10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Positive Format 4
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 4, 1);
            expected = "10.00 " + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            ////Positive Format 5 default
            //currency = UpdateCurrencyPostiveAndNegativeFormats(CurrencyID, 5, 1);
            //expected = "????";
            //actual = target.FormatCurrency(value, currency, FormatForEdit);

            //Assert.AreEqual(expected, actual);

            //Change negative for negative formats
            value = (decimal)-10.00;

            //Negative Format 1
            expected = "-" + globCurr.symbol + "10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 2
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 2);
            expected = "(" + globCurr.symbol + "10.00)";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 3
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 3);
            expected = globCurr.symbol + "-10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 4
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 4);
            expected = globCurr.symbol + "10.00-";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 5
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 5);
            expected = "(10.00" + globCurr.symbol + ")";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 6
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 6);
            expected = "-10.00" + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 7
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 7);
            expected = "10.00-" + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 8
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 8);
            expected = "10.00" + globCurr.symbol + "-";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 9
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 9);
            expected = "(" + globCurr.symbol + " 10.00)";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 10
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 10);
            expected = "-" + globCurr.symbol + " 10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 11
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 11);
            expected = globCurr.symbol + " -10.00";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 12
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 12);
            expected = globCurr.symbol + " 10.00-";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 13
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 13);
            expected = "(10.00 " + globCurr.symbol + ")";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 14
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 14);
            expected = "-10.00 " + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 15
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 15);
            expected = "10.00- " + globCurr.symbol;
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            //Negative Format 16
            currency = cCurrencyObject.UpdateCurrencyPostiveAndNegativeFormats(cGlobalVariables.CurrencyID, 1, 16);
            expected = "10.00 " + globCurr.symbol + "-";
            actual = target.FormatCurrency(value, currency, FormatForEdit);

            Assert.AreEqual(expected, actual);

            ////Negative Format 17 Default
            //currency = UpdateCurrencyPostiveAndNegativeFormats(CurrencyID, 1, 17);
            //expected = "????";
            //actual = target.FormatCurrency(value, currency, FormatForEdit);

            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for deleteCurrency
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_deleteCurrencyTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            int currencyID = cGlobalVariables.CurrencyID;

            cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountProperties globProps = subaccs.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties;
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);
            cMisc clsMisc = new cMisc(cGlobalVariables.AccountID);
            int actual = 0;
            int currentGlobCurrID = cGlobalVariables.GlobalCurrencyID;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                cEmployee emp = clsEmps.GetEmployeeById(cGlobalVariables.EmployeeID);
                int currentEmpCurrencyID = emp.primarycurrency;

                cGlobalPropertiesObject.UpdateGlobalCurrency(cGlobalVariables.SubAccountID, currencyID);

                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                actual = target.deleteCurrency(currencyID);

                //Check to see if the currency is the global currency for the system
                Assert.AreEqual(1, actual);

                //Set the global currency back to its previous value
                cGlobalPropertiesObject.UpdateGlobalCurrency(cGlobalVariables.SubAccountID, currentGlobCurrID);

                cEmployeeObject.UpdateEmployeeBaseCurrency(currencyID);

                cClaimObject.CreateCurrentClaim();

                actual = target.deleteCurrency(currencyID);

                //Check to see if the currency is associated to any claims
                Assert.AreEqual(2, actual);

                //Cant use as user defined fields does not work when adding expense
                //cExpenseObject.CreateExpenseItem();

                //actual = target.deleteCurrency(currencyID);

                ////Check to see if the currency is associated to any expenses
                //Assert.AreEqual(2, actual);

                cClaimObject.deleteClaim();

                //Set the employee primary currency back to its previous value
                cEmployeeObject.UpdateEmployeeBaseCurrency(currentEmpCurrencyID);
                actual = target.deleteCurrency(currencyID);

                //Check to see if the currency deletes as it should not be associated to anything in expenses now.
                Assert.AreEqual(0, actual);
            }
            finally
            {
                cClaimObject.deleteClaim();
                target.deleteCurrency(cGlobalVariables.CurrencyID);
            }
        }

        /// <summary>
        ///A test for CreateVList
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateVListTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);

            cGlobalCurrencies clsCurrencies = new cGlobalCurrencies();
            string currencyName = clsCurrencies.getGlobalCurrencyById(cGlobalVariables.GlobalCurrencyID).label;

            ValueList actual;
            actual = target.CreateVList();
            if (actual.GetType() != typeof(Infragistics.WebUI.UltraWebGrid.ValueList))
            {
                Assert.Fail("Should be a ValueList");
            }
            else
            {
                int cnt = 0;
                foreach (ValueListItem vli in actual.ValueListItems)
                {
                    cnt = (vli.DisplayText == currencyName) ? cnt + 1 : cnt;
                }
                Assert.IsTrue(cnt == 1);
            }
        }

        /// <summary>
        ///A test for CreatePositiveFormatDropDown
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreatePositiveFormatDropDownTest()
        {
            SortedList<int, string> lstPositiveFormats = new SortedList<int, string>();
            lstPositiveFormats.Add(1, "X1.1");
            lstPositiveFormats.Add(2, "1.1X");
            lstPositiveFormats.Add(3, "X 1.1");
            lstPositiveFormats.Add(4, "1.1 X");

            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            List<ListItem> actual;
            actual = target.CreatePositiveFormatDropDown();
            Assert.IsTrue(actual.Count == 4);
            foreach (ListItem li in actual)
            {
                Assert.IsTrue(li.Text == lstPositiveFormats[Convert.ToInt32(li.Value)]);
            }
        }

        /// <summary>
        ///A test for CreateNegativeFormatDropDown
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateNegativeFormatDropDownTest()
        {
            SortedList<int, string> lstNegativeFormats = new SortedList<int, string>();
            lstNegativeFormats.Add(1, "-X1.1");
            lstNegativeFormats.Add(2, "(X1.1)");
            lstNegativeFormats.Add(3, "X-1.1");
            lstNegativeFormats.Add(4, "X1.1-");
            lstNegativeFormats.Add(5, "(1.1X)");
            lstNegativeFormats.Add(6, "-1.1X");
            lstNegativeFormats.Add(7, "1.1-X");
            lstNegativeFormats.Add(8, "1.1X-");
            lstNegativeFormats.Add(9, "(X 1.1)");
            lstNegativeFormats.Add(10, "-X 1.1");
            lstNegativeFormats.Add(11, "X -1.1");
            lstNegativeFormats.Add(12, "X 1.1-");
            lstNegativeFormats.Add(13, "(1.1 X)");
            lstNegativeFormats.Add(14, "-1.1 X");
            lstNegativeFormats.Add(15, "1.1- X");
            lstNegativeFormats.Add(16, "1.1 X-");

            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            List<ListItem> actual;
            actual = target.CreateNegativeFormatDropDown();
            Assert.IsTrue(actual.Count == 16);
            foreach (ListItem li in actual)
            {
                Assert.IsTrue(li.Text == lstNegativeFormats[Convert.ToInt32(li.Value)]);
            }
        }

        /// <summary>
        ///A test for CreateExchangeTable
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateExchangeTableTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);
            int id = 0;
            CurrencyType currType = CurrencyType.Static;
            string expected = string.Empty;
            string actual;

            actual = target.CreateExchangeTable(cGlobalVariables.CurrencyID, id, currType);
            if (actual.GetType() == typeof(string))
            {
                Assert.IsTrue(actual.Contains("<table class=datatbl>"));
                Assert.IsTrue(actual.Contains("<td"));
                Assert.IsTrue(actual.EndsWith("</table>"));
            }
            else
            {
                Assert.Fail("HTML not correct");
            }
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateDropDownTest1()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);

            List<ListItem> actual;
            actual = target.CreateDropDown();
            Assert.IsTrue(actual.Count > 0);
            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == cGlobalVariables.CurrencyID.ToString())
                {
                    cnt++;
                }
            }
            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateDropDownTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);


            ListItem[] actual;
            actual = target.CreateDropDown(cGlobalVariables.CurrencyID);
            Assert.IsTrue(actual.Length > 0);
            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == cGlobalVariables.CurrencyID.ToString() && li.Selected == true)
                {
                    cnt++;
                }
            }
            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for CreateColumnList
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_CreateColumnListTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);

            try
            {
                cColumnList actual;
                actual = target.CreateColumnList();
                Assert.IsTrue(actual.exists(cGlobalVariables.CurrencyID));
            }
            finally
            {
                target.deleteCurrency(cGlobalVariables.CurrencyID);
            }
        }

        /// <summary>
        ///A test for checkCurrencyRangeExists
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_checkCurrencyRangeExistsTest()
        {
            cRangeCurrencies target = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            CurrencyType original = target.currencytype;
            SortedList<int, double> exchangeRates = new SortedList<int, double>();
            SortedList<int, cCurrencyRange> rangeExchangeRates = new SortedList<int, cCurrencyRange>();
            cCurrencyRange currRange;
            cRangeCurrency rangeCurrency;
            DateTime baseDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;
            int currencyID = 0;
            int currRangeID = 0;

            try
            {
                // create list of faux exchange rates for all current currencies
                foreach (int id in target.currencyList.Keys)
                {
                    exchangeRates.Add(id, 1);
                }

                // create new currency of type range
                target.changeCurrencyType(CurrencyType.Range, cGlobalVariables.EmployeeID);
                rangeCurrency = new cRangeCurrency(cGlobalVariables.AccountID, 0, cGlobalVariables.CurrencyID, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int,cCurrencyRange>());
                currencyID = target.saveCurrency(rangeCurrency);

                // create new range of currencies
                startDate = baseDate.AddDays(-2);
                endDate = baseDate;
                currRange = new cCurrencyRange(cGlobalVariables.AccountID, currencyID, 0, startDate, endDate, DateTime.Now, cGlobalVariables.EmployeeID, null, null, exchangeRates);

                // should be false as the range hasn't been added yet
                bool expected = false;
                bool actual = target.checkCurrencyRangeExists(currencyID, 0, startDate, endDate);
                Assert.AreEqual(expected, actual, "Range not added yet, should be false");
                
                // add the range
                currRangeID = target.saveCurrencyRange(currRange);

                // should be true as entering 0 which asks if a range exists that covers this time period
                expected = true;
                actual = target.checkCurrencyRangeExists(currencyID, 0, startDate, endDate);
                Assert.AreEqual(expected, actual, "Checking range we just added");

                // should be false as a range shouldn't exist with a different id to the one we pass in
                expected = false;
                actual = target.checkCurrencyRangeExists(currencyID, currRangeID, startDate, endDate);
                Assert.AreEqual(expected, actual, "Entering rangeID to check for dupes");

                // change the dates to fall outside the range, should return false
                expected = false;
                startDate = baseDate.AddDays(5);
                endDate = baseDate.AddDays(6);
                actual = target.checkCurrencyRangeExists(currencyID, 0, startDate, endDate);
                Assert.AreEqual(expected, actual, "Range in the future");

                // change the dates to overlap the range, should return true
                expected = true;
                startDate = baseDate.AddDays(-3);
                endDate = baseDate.AddDays(-1.99);
                actual = target.checkCurrencyRangeExists(currencyID, 0, startDate, endDate);
                Assert.AreEqual(expected, actual, "Slight range overlap");
            }
            finally
            {
                if (currRangeID > 0)
                {
                    target.deleteCurrencyRange(currencyID, currRangeID);
                    target.deleteCurrency(currencyID);
                }
                else if (currencyID > 0)
                {
                    target.deleteCurrency(currencyID);
                }
                target.changeCurrencyType(original, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for checkCurrencyMonthExists
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_checkCurrencyMonthExistsTest()
        {
            cMonthlyCurrencies target = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            CurrencyType original = target.currencytype;
            SortedList<int, double> exchangeRates = new SortedList<int, double>();
            SortedList<int, cCurrencyMonth> monthlyExchangeRates = new SortedList<int, cCurrencyMonth>();
            cCurrencyMonth currMonth;
            cMonthlyCurrency monthlyCurrency;
            DateTime baseDate = DateTime.Now;
            int month = 0;
            int year = 0;
            int currencyID = 0;
            int currMonthID = 0;

            try
            {
                // create list of faux exchange rates for all currenct currencies
                foreach (int id in target.currencyList.Keys)
                {
                    exchangeRates.Add(id, 1);
                }

                // create new currency of type range
                target.changeCurrencyType(CurrencyType.Monthly, cGlobalVariables.EmployeeID);
                monthlyCurrency = new cMonthlyCurrency(cGlobalVariables.AccountID, 0, cGlobalVariables.CurrencyID, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, new SortedList<int, cCurrencyMonth>());
                currencyID = target.saveCurrency(monthlyCurrency);

                // create new range of currencies
                month = 1;
                year = 2009;
                currMonth = new cCurrencyMonth(cGlobalVariables.AccountID, currencyID, 0, month, year, DateTime.Now, cGlobalVariables.EmployeeID, null, null, exchangeRates);

                // should be false as the range hasn't been added yet
                bool expected = false;
                bool actual = target.checkCurrencyMonthExists(currencyID, 0, month, year);
                Assert.AreEqual(expected, actual, "Month not added yet, should be false");

                // add the range
                currMonthID = target.saveCurrencyMonth(currMonth);

                // should be true as entering 0 which asks if a range exists that covers this time period
                expected = true;
                actual = target.checkCurrencyMonthExists(currencyID, 0, month, year);
                Assert.AreEqual(expected, actual, "Checking month we just added");

                // should be false as a range shouldn't exist with a different id to the one we pass in
                expected = false;
                actual = target.checkCurrencyMonthExists(currencyID, currMonthID, month, year);
                Assert.AreEqual(expected, actual, "Entering monthID to check for dupes");

                // change the dates to fall outside the range, should return false
                expected = false;
                month = 2;
                year = 2009;
                actual = target.checkCurrencyMonthExists(currencyID, 0, month, year);
                Assert.AreEqual(expected, actual, "Month different");

                expected = false;
                month = 1;
                year = 2010;
                actual = target.checkCurrencyMonthExists(currencyID, 0, month, year);
                Assert.AreEqual(expected, actual, "Year different");
            }
            finally
            {
                if (currMonthID > 0)
                {
                    target.deleteCurrencyMonth(currencyID, currMonthID);
                    target.deleteCurrency(currencyID);
                }
                else if (currencyID > 0)
                {
                    target.deleteCurrency(currencyID);
                }
                target.changeCurrencyType(original, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for changeStatus
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_changeStatusTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency currency = target.getCurrencyById(cGlobalVariables.CurrencyID);
            int expected = 0;
            int actual;

            actual = target.changeStatus(cGlobalVariables.CurrencyID, true);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for changeCurrencyType
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_changeCurrencyTypeTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            CurrencyType original = target.currencytype;

            try
            {
                target.changeCurrencyType(CurrencyType.Static, cGlobalVariables.EmployeeID);
                CurrencyType actual = cCurrencyObject.GetGlobalCurrencyType();

                Assert.AreEqual(CurrencyType.Static, actual);

                target.changeCurrencyType(CurrencyType.Monthly, cGlobalVariables.EmployeeID);
                actual = cCurrencyObject.GetGlobalCurrencyType();

                Assert.AreEqual(CurrencyType.Monthly, actual);

                target.changeCurrencyType(CurrencyType.Range, cGlobalVariables.EmployeeID);
                actual = cCurrencyObject.GetGlobalCurrencyType();

                Assert.AreEqual(CurrencyType.Range, actual);
            }
            finally
            {
                target.changeCurrencyType(original, cGlobalVariables.EmployeeID);
                //target.changeCurrencyType(CurrencyType.Static, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for addExchangeRates getExchangeRates and deleteExchangeRates as this is contained within this method and not used anywhere else
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_addDeleteGetExchangeRatesTest()
        {
            cCurrencies target = null;
            cCurrency expected = null;

            try
            {
                expected = cCurrencyObject.CreateCurrency();
                int CurrencyID = expected.currencyid;
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                
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

                //Static currencies
                target.addExchangeRates(CurrencyID, CurrencyType.Static, exchangerates, DateTime.UtcNow, cGlobalVariables.EmployeeID);

                SortedList<int, SortedList<int, double>> addedExchangeRates = target.getExchangeRates(CurrencyType.Static);

                int i = 0;

                foreach (KeyValuePair<int, SortedList<int, double>> kvp in addedExchangeRates)
                {
                    if (kvp.Key == CurrencyID)
                    {
                        foreach (KeyValuePair<int, double> kp in kvp.Value)
                        {
                            Assert.AreEqual(kp.Key, exchangerates.Keys[i]);
                            Assert.AreEqual(kp.Value, exchangerates.Values[i]);
                            i++;
                        }
                    }
                }

                //Range currencies
                target.addExchangeRates(CurrencyID, CurrencyType.Range, exchangerates, DateTime.UtcNow, cGlobalVariables.EmployeeID);

                addedExchangeRates = target.getExchangeRates(CurrencyType.Range);

                i = 0;
                foreach (KeyValuePair<int, SortedList<int, double>> kvp in addedExchangeRates)
                {
                    if (kvp.Key == CurrencyID)
                    {
                        foreach (KeyValuePair<int, double> kp in kvp.Value)
                        {
                            Assert.AreEqual(kp.Key, exchangerates.Keys[i]);
                            Assert.AreEqual(kp.Value, exchangerates.Values[i]);
                            i++;
                        }
                    }
                }

                //Monthly currencies
                target.addExchangeRates(CurrencyID, CurrencyType.Monthly, exchangerates, DateTime.UtcNow, cGlobalVariables.EmployeeID);

                addedExchangeRates = target.getExchangeRates(CurrencyType.Monthly);

                i = 0;
                foreach (KeyValuePair<int, SortedList<int, double>> kvp in addedExchangeRates)
                {
                    if (kvp.Key == CurrencyID)
                    {
                        foreach (KeyValuePair<int, double> kp in kvp.Value)
                        {
                            Assert.AreEqual(kp.Key, exchangerates.Keys[i]);
                            Assert.AreEqual(kp.Value, exchangerates.Values[i]);
                            i++;
                        }
                    }
                }

            }
            finally
            {
                if (target != null && expected != null)
                {
                    target.deleteCurrency(expected.currencyid);
                }
            }
        }

        /// <summary>
        ///A test for convertToBase
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_convertToBaseTest()
        {
            cCurrency fromCurrency = null;
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            CurrencyType original = target.currencytype;

            try
            {
                cGlobalCurrencies gcurrencies = new cGlobalCurrencies();
                cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
                cAccountProperties properties = subaccs.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties;
                
                cCurrency baseCurrency = target.getCurrencyByAlphaCode("GBP");

                if (baseCurrency == null)
                {
                    baseCurrency = cCurrencyObject.CreateBaseCurrency();
                }

                fromCurrency = target.getCurrencyByAlphaCode("USD");
                cGlobalPropertiesObject.UpdateGlobalCurrency(cGlobalVariables.SubAccountID, baseCurrency.currencyid);
                int convertFromCurrencyID = 0;
                
                target.changeCurrencyType(CurrencyType.Static, cGlobalVariables.EmployeeID);
                int globalFromCurrencyID = gcurrencies.getGlobalCurrencyByAlphaCode("USD").globalcurrencyid;

                if (fromCurrency == null)
                {
                    fromCurrency = new cCurrency(cGlobalVariables.AccountID, 0, globalFromCurrencyID, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null);
                    convertFromCurrencyID = target.saveCurrency(fromCurrency);
                    cCurrencyObject.CreateExchangeRates(CurrencyType.Static, globalFromCurrencyID);

                    target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                    fromCurrency = target.getCurrencyById(convertFromCurrencyID);
                }
                else
                {
                    if (((cStaticCurrency)fromCurrency).exchangerates.Count == 0)
                    {
                        cCurrencyObject.CreateExchangeRates(CurrencyType.Static, globalFromCurrencyID);
                    }
                }
                
                cStaticCurrency scurrency = (cStaticCurrency)fromCurrency;
                double toRate = scurrency.exchangerates[baseCurrency.currencyid];
                decimal amountToConvert = 100;
                decimal expected = amountToConvert * (1 / (decimal)toRate);
                decimal actual;
                actual = target.convertToBase(fromCurrency.currencyid, amountToConvert, null);
                Assert.AreEqual(expected, actual);

                // now for monthly
                target.changeCurrencyType(CurrencyType.Monthly, cGlobalVariables.EmployeeID);

                cCurrencyObject.CreateExchangeRates(CurrencyType.Monthly, globalFromCurrencyID);
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                fromCurrency = target.getCurrencyById(fromCurrency.currencyid);
                cMonthlyCurrency mcurrency = (cMonthlyCurrency)fromCurrency;

                cCurrencyMonth cmonth = (cCurrencyMonth)mcurrency.exchangerates.Values[0];
                toRate = cmonth.exchangerates[baseCurrency.currencyid];
                expected = amountToConvert * (1 / (decimal)toRate);
                actual = target.convertToBase(fromCurrency.currencyid, amountToConvert, DateTime.Now);
                Assert.AreEqual(expected, actual);

                // now for date range
                target.changeCurrencyType(CurrencyType.Monthly, cGlobalVariables.EmployeeID);

                cCurrencyObject.CreateExchangeRates(CurrencyType.Monthly, globalFromCurrencyID);
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                fromCurrency = target.getCurrencyById(fromCurrency.currencyid);
                cRangeCurrency drcurrency = (cRangeCurrency)fromCurrency;
                cCurrencyRange drmonth = (cCurrencyRange)drcurrency.exchangerates.Values[0];
                toRate = drmonth.exchangerates[baseCurrency.currencyid];
                expected = amountToConvert * (1 / (decimal)toRate);
                actual = target.convertToBase(fromCurrency.currencyid, amountToConvert, DateTime.Now);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                // tidy up
                target.changeCurrencyType(original, cGlobalVariables.EmployeeID);

                if (fromCurrency != null)
                {
                    target.deleteCurrency(fromCurrency.currencyid);
                }
            }
        }

        /// <summary>
        ///A test for convertAmount
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_convertAmountTest()
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            int convertFromCurrencyID = cGlobalVariables.CurrencyID;
            decimal amountToConvert = (decimal)3.14;
            int convertToCurrencyID = 0;

            try
            {
                cCurrencyObject.CreateExchangeRates(CurrencyType.Static, cGlobalVariables.GlobalCurrencyID); // all exchange rates will be 2.2
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                cCurrency curCurrency = target.getCurrencyById(cGlobalVariables.CurrencyID);
                decimal expected = (amountToConvert * (decimal)2.2);
                decimal actual;
                actual = target.convertAmount(convertFromCurrencyID, amountToConvert, convertToCurrencyID, DateTime.Now);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                target.deleteCurrency(cGlobalVariables.CurrencyID);
            }
        }

        /// <summary>
        ///A test for currencyExists
        ///</summary>
        [TestMethod()]
        public void cCurrenciesTest_currencyExistsTest()
        {
            cCurrencies_Accessor target = new cCurrencies_Accessor(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            
            bool expected = true;
            bool actual;
            actual = target.currencyExists(cGlobalVariables.GlobalCurrencyID);
            Assert.AreEqual(expected, actual);
        }
    }
}
