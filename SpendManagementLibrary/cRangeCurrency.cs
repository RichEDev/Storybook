using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cRangeCurrency : cCurrency
    {
        private SortedList<int, cCurrencyRange> lstExchangerates;

        public cRangeCurrency(int accountid, int currencyid, int globalcurrencyid, byte positiveFormat, byte negativeFormat, bool archived, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cCurrencyRange> exchangerates)
            : base(accountid, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby)
        {
            lstExchangerates = exchangerates;
        }

        /// <summary>
        /// Gets an exchange rate for a specified currency for a particular date
        /// </summary>
        /// <param name="date">Date to obtain an exchange rate for</param>
        /// <param name="toCurrencyId">Currency ID to convert to</param>
        /// <returns></returns>
        public override double getExchangeRate(int toCurrencyId, DateTime date)
        {
            foreach (cCurrencyRange reqrange in lstExchangerates.Values)
            {
                if (reqrange.startdate <= date && reqrange.enddate >= date)
                {
                    return reqrange.getExchangeRate(toCurrencyId);
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets a currency exchange rate range entry by its ID
        /// </summary>
        /// <param name="currencyrangeid">ID of the exchange rate range to obtain</param>
        /// <returns>Currency Range Entity</returns>
        public cCurrencyRange getCurrencyRangeById(int currencyrangeid)
        {
            return (cCurrencyRange)lstExchangerates[currencyrangeid];
        }

        #region Properties
        /// <summary>
        /// Gets the current number of exchange rate ranges in the collection
        /// </summary>
        public int exchangeratecount
        {
            get { return lstExchangerates.Count; }
        }

        /// <summary>
        /// Gets the currency exchange rate range collection
        /// </summary>
        public SortedList<int, cCurrencyRange> exchangerates
        {
            get { return lstExchangerates; }
        }
        #endregion

        /// <summary>
        /// Add a new currency exchange range
        /// </summary>
        /// <param name="range">Currency Range Entry to add</param>
        public void addCurrencyRange(cCurrencyRange range)
        {
            if (!this.lstExchangerates.ContainsKey(range.currencyrangeid))
            {
                this.lstExchangerates.Add(range.currencyrangeid, range);
            }
            
        }

        /// <summary>
        /// Update a particular currency range
        /// </summary>
        /// <param name="range">Currency Range Entry to update</param>
        public void updateCurrencyRange(cCurrencyRange range)
        {
            lstExchangerates[range.currencyrangeid] = range;
        }

        /// <summary>
        /// Delete a particular exchange rate range
        /// </summary>
        /// <param name="currencyrangeid">Currency Range Entry to delete</param>
        public void deleteCurrencyRange(int currencyrangeid)
        {
            lstExchangerates.Remove(currencyrangeid);
        }

        /// <summary>
        /// Converts a supplied amount to a another currency using exchangerate for a specific date
        /// </summary>
        /// <param name="amountToConvert">Amount to be converted</param>
        /// <param name="convertToCurrencyId">Target currency ID</param>
        /// <param name="exchangeRateDate">Date of required exchange rate</param>
        /// <returns>Converted amount in decimal</returns>
        public decimal convertCurrencyValue(decimal amountToConvert, int convertToCurrencyId, DateTime exchangeRateDate)
        {
            decimal exchangeRate = (decimal)getExchangeRate(convertToCurrencyId, exchangeRateDate);

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
    public class cCurrencyRange
    {
        private int nAccountid;
        private int nCurrencyid;
        private int nCurrencyrangeid;
        private DateTime dtStartdate;
        private DateTime dtEnddate;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;

        private SortedList<int, double> lstExchangerates;
        public cCurrencyRange(int accountid, int currencyid, int currencyrangeid, DateTime startdate, DateTime enddate, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, double> exchangerates)
        {
            nAccountid = accountid;
            nCurrencyid = currencyid;
            nCurrencyrangeid = currencyrangeid;
            dtStartdate = startdate;
            dtEnddate = enddate;
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
        public int currencyrangeid
        {
            get { return nCurrencyrangeid; }
        }
        public DateTime startdate
        {
            get { return dtStartdate; }
        }
        public DateTime enddate
        {
            get { return dtEnddate; }
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
