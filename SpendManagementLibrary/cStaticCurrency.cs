using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cStaticCurrency : cCurrency
    {
        private SortedList<int, double> lstExchangerates = new SortedList<int, double>();

        public cStaticCurrency(int accountid, int currencyid, int globalcurrencyid, byte positiveFormat, byte negativeFormat, bool archived, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, double> exchangerates) 
            : base(accountid, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby)
        {
            lstExchangerates = exchangerates;
        }

        #region properties
        /// <summary>
        /// Gets or Sets the exchange rates collection
        /// </summary>
        public SortedList<int, double> exchangerates
        {
            get { return lstExchangerates; }
            set { lstExchangerates = value; }
        }
        #endregion

        /// <summary>
        /// Get the current static exchange rate
        /// </summary>
        /// <param name="toCurrencyId">Target Currency ID to obtain exchange rate for</param>
        /// <param name="dateTime">The date/time at which to rectrieve the exchange rate</param>
        /// <returns>Exchange rate (double). Returns zero if not found.</returns>
        public override double getExchangeRate(int toCurrencyId, DateTime dateTime)
        {
            return getExchangeRate(toCurrencyId);
        }

        private double getExchangeRate(int toCurrencyId)
        {
            double exchangerate = 0;
            if (lstExchangerates.ContainsKey(toCurrencyId))
            {
                exchangerate = lstExchangerates[toCurrencyId];
            }
            return exchangerate;

        }

        /// <summary>
        /// Converts a supplied amount to a another currency
        /// </summary>
        /// <param name="amountToConvert">Amount to be converted</param>
        /// <param name="convertToCurrencyId">Target currency ID</param>
        /// <returns>Converted amount in decimal</returns>
        public decimal convertCurrencyValue(decimal amountToConvert, int convertToCurrencyId)
        {
            decimal exchangeRate = (decimal)getExchangeRate(convertToCurrencyId);

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
}
