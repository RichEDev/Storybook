namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The Base Currency superclass has 2 subclasses, CurrencyMonth and CurrencyDateRange. 
    /// It contains the common property, ExchangeRateTable.
    /// </summary>
    public class BaseCurrency
    {
        /// <summary>
        /// The sorted list of Exchange Rates.
        /// </summary>
        [Required]
        public SortedList<int, double> ExchangeRateTable { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate table received from the api. Api could not cope with a sorted list hence needed the class CurrencyWithRate.
        /// </summary>
        public List<CurrencyWithRate> ApiExchangeRateTable { get; set; }


        /// <summary>
        /// Converts apiExchangeRateTable to sorted list of exchange rate table.
        /// </summary>
        /// <param name="apiExchangeRateTable">The exchange rate table received from the api</param>
        /// <returns>The exchange rate table</returns>
        public SortedList<int, double> Cast(List<CurrencyWithRate> apiExchangeRateTable)
        {
            SortedList<int, double> exchangeRateTable = new SortedList<int, double>();
            foreach (CurrencyWithRate currencyWithRate in apiExchangeRateTable)
            {
                exchangeRateTable.Add(currencyWithRate.CurrencyId, currencyWithRate.ExchangeRate);
            }
            return exchangeRateTable;
        }
    }
}