using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cMonthlyCurrency : cCurrency
    {
        private SortedList<int, cCurrencyMonth> lstExchangeRates;

        public cMonthlyCurrency(int accountid, int currencyid, int globalcurrencyid, byte positiveFormat, byte negativeFormat, bool archived, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cCurrencyMonth> exchangerates)
            : base(accountid, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby)
        {
            lstExchangeRates = exchangerates;
        }

        /// <summary>
        /// Retrieve a particular currency month entity by its ID
        /// </summary>
        /// <param name="currencymonthid">ID of the currency month entity to retrieve</param>
        /// <returns>The currency month entity record</returns>
        public cCurrencyMonth getCurrentMonthById(int currencymonthid)
        {
            return (cCurrencyMonth)lstExchangeRates[currencymonthid];
        }

        /// <summary>
        /// Get an exchange rate for a particular year and month to a specified target currency
        /// </summary>
        /// <param name="toCurrencyId">Currency Id converting to</param>
        /// <param name="dateTime">The date/time at which to retrieve the exchange rate.</param>
        /// <returns>Exchange rate in decimal. Returns zero if not found.</returns>
        public override double getExchangeRate(int toCurrencyId, DateTime dateTime)
        {
            foreach (cCurrencyMonth reqmonth in  lstExchangeRates.Values)
            {
                if (reqmonth.month == dateTime.Month && reqmonth.year == dateTime.Year)
                {
                    return reqmonth.getExchangeRate(toCurrencyId);
                }
            }

            return 0;
        }

        
      
        /// <summary>
        /// Add an exchange rate for a particular month
        /// </summary>
        /// <param name="month">Currency month element to add</param>
        public void addCurrencyMonth(cCurrencyMonth month)
        {
            lstExchangeRates.Add(month.currencymonthid, month);
        }

        /// <summary>
        /// Update a currency month entry
        /// </summary>
        /// <param name="month">Currency month entity to update</param>
        public void updateCurrencyMonth(cCurrencyMonth month)
        {
            lstExchangeRates[month.currencymonthid] = month;
        }

        /// <summary>
        /// Delete an exchange rate entry for a given period
        /// </summary>
        /// <param name="currencymonthid">Currency Month Id to delete</param>
        public void deleteCurrencyMonth(int currencymonthid)
        {
            lstExchangeRates.Remove(currencymonthid);
        }

        #region properties
        /// <summary>
        /// Gets the number of exchange rates
        /// </summary>
        public int exchangeratecount
        {
            get { return lstExchangeRates.Count; }
        }
        /// <summary>
        /// Gets the current exchange rate months collection
        /// </summary>
        public SortedList<int, cCurrencyMonth> exchangerates
        {
            get { return lstExchangeRates; }
        }
        #endregion

        /// <summary>
        /// Converts a supplied amount to a another currency using exchangerate for a specific date
        /// </summary>
        /// <param name="amountToConvert">Amount to be converted</param>
        /// <param name="convertToCurrencyId">Target currency ID</param>
        /// <param name="exchangeRateDate">Date of required exchange rate</param>
        /// <returns>Converted amount in decimal</returns>
        public decimal convertCurrencyValue(decimal amountToConvert, int convertToCurrencyId, DateTime exchangeRateDate)
        {
            decimal exchangeRate = (decimal) getExchangeRate(convertToCurrencyId, exchangeRateDate);

            if (exchangeRate > 0)
            {
                return amountToConvert * (1 / exchangeRate);
            }
            else
            {
                return amountToConvert;
            }
        }
    }

    [Serializable()]
    public class cCurrencyMonth
    {
        private int nAccountid;
        private int nCurrencyid;
        private int nCurrencymonthid;
        private int nMonth;
        private int nYear;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;


        private SortedList<int, double> lstExchangerates = new SortedList<int, double>();
        public cCurrencyMonth(int accountid, int currencyid, int currencymonthid, int month, int year, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, double> exchangerates)
        {
            nAccountid = accountid;
            nCurrencyid = currencyid;
            nCurrencymonthid = currencymonthid;
            nMonth = month;
            nYear = year;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            lstExchangerates = exchangerates;
        }

        

        public double getExchangeRate(int currencyid)
        {
            if (exchangerates.ContainsKey(currencyid))
            {
                return (double)exchangerates[currencyid];
            }
            return 0;
        }

        
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public int currencyid
        {
            get { return nCurrencyid; }
        }
        public int currencymonthid
        {
            get { return nCurrencymonthid; }
        }
        public int month
        {
            get { return nMonth; }
        }
        public int year
        {
            get { return nYear; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        public SortedList<int, double> exchangerates
        {
            get { return lstExchangerates; }
            set { lstExchangerates = value; }
        }
        #endregion
    }
}
