using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cCurrencyObject
    {
        public static cCurrency CreateCurrency()
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency clsGlobalCurrency = clsGlobalCurrencies.getGlobalCurrencyByAlphaCode("BOB"); // choose unlikely currency to prevent clashes with basecurrency etc.
            cGlobalVariables.GlobalCurrencyID = clsGlobalCurrency.globalcurrencyid;

            cCurrencies clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency tempCurrency = clsCurrencies.getCurrencyByGlobalCurrencyId(clsGlobalCurrency.globalcurrencyid);
            if (tempCurrency == null)
            {
                int tempCurrencyId = clsCurrencies.saveCurrency(new cCurrency(cGlobalVariables.AccountID, 0, clsGlobalCurrency.globalcurrencyid, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null));
                clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                tempCurrency = clsCurrencies.getCurrencyById(tempCurrencyId);
            }
            cGlobalVariables.CurrencyID = tempCurrency.currencyid;
            return tempCurrency;
        }

        public static cCurrency CreateCurrency(string currencyAlphaCode, bool updateGlobalVariables)
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency clsGlobalCurrency = clsGlobalCurrencies.getGlobalCurrencyByAlphaCode(currencyAlphaCode); // choose unlikely currency to prevent clashes with basecurrency etc.
            if (updateGlobalVariables)
            {
                cGlobalVariables.GlobalCurrencyID = clsGlobalCurrency.globalcurrencyid;
            }

            cCurrencies clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency tempCurrency = clsCurrencies.getCurrencyByGlobalCurrencyId(clsGlobalCurrency.globalcurrencyid);
            if (tempCurrency == null)
            {
                int tempCurrencyId = clsCurrencies.saveCurrency(new cCurrency(cGlobalVariables.AccountID, 0, clsGlobalCurrency.globalcurrencyid, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null));
                clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                tempCurrency = clsCurrencies.getCurrencyById(tempCurrencyId);
            }
            if (updateGlobalVariables)
            {
                cGlobalVariables.CurrencyID = tempCurrency.currencyid;
            }
            return tempCurrency;
        }

        public static cCurrency CreateBaseCurrency()
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency clsGlobalCurrency = clsGlobalCurrencies.getGlobalCurrencyByAlphaCode("GBP"); // choose unlikely currency to prevent clashes with basecurrency etc.
            cGlobalVariables.GlobalCurrencyID = clsGlobalCurrency.globalcurrencyid;

            cCurrencies clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCurrency tempCurrency = clsCurrencies.getCurrencyByGlobalCurrencyId(clsGlobalCurrency.globalcurrencyid);
            if (tempCurrency == null)
            {
                int tempCurrencyId = clsCurrencies.saveCurrency(new cCurrency(cGlobalVariables.AccountID, 0, clsGlobalCurrency.globalcurrencyid, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null));
                clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                tempCurrency = clsCurrencies.getCurrencyById(tempCurrencyId);
            }
            cGlobalVariables.CurrencyID = tempCurrency.currencyid;
            return tempCurrency;
        }

        public static cCurrency UpdateCurrencyPostiveAndNegativeFormats(int CurrencyID, byte positiveFormat, byte negativeFormat)
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cCurrency newCurrency = new cCurrency(cGlobalVariables.AccountID, CurrencyID, cGlobalVariables.CurrencyID, positiveFormat, negativeFormat, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
            cGlobalVariables.CurrencyID = target.saveCurrency(newCurrency);
            return newCurrency;
        }

        public static CurrencyType GetGlobalCurrencyType()
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties;

            CurrencyType currType = properties.currencyType;

            return currType;
        }

        public static SortedList<int, double> CreateExchangeRates(CurrencyType currType, int globalcurrencyid)
        {
            cCurrencies target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            SortedList<int, double> exchangerates = new SortedList<int, double>();
            cCurrency expected = target.getCurrencyByGlobalCurrencyId(globalcurrencyid);
            if (expected == null)
            {
                expected = new cCurrency(cGlobalVariables.AccountID, 0, globalcurrencyid, 1, 1, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null);
                target.saveCurrency(expected);
                target = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                expected = target.getCurrencyByGlobalCurrencyId(globalcurrencyid);
            }

            double val = 2.2;

            foreach (int id in target.currencyList.Keys)
            {
                if (id != expected.currencyid)
                {
                    exchangerates.Add(id, val);
                }
            }

            switch(currType)
            {
                case CurrencyType.Static:
                    
                    target.addExchangeRates(expected.currencyid, currType, exchangerates, DateTime.UtcNow, cGlobalVariables.EmployeeID);
                    break;
                case CurrencyType.Monthly:
                    cMonthlyCurrencies clsMonthly = new cMonthlyCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                    clsMonthly.saveCurrencyMonth(new cCurrencyMonth(cGlobalVariables.AccountID, expected.currencyid, 0, 1, DateTime.Now.Year, DateTime.UtcNow, 0, null, null, exchangerates));
                    break;
                case CurrencyType.Range:
                    cRangeCurrencies clsRange = new cRangeCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                    clsRange.saveCurrencyRange(new cCurrencyRange(cGlobalVariables.AccountID, expected.currencyid, 0, DateTime.UtcNow.AddMonths(-2), DateTime.UtcNow.AddMonths(2), DateTime.UtcNow, 0, null, null, exchangerates));
                    break;
                    
            }


            return exchangerates;
        }
    }
}
