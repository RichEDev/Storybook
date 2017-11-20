namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    /// <summary>
    /// The currency exchange rates list.
    /// </summary>
    public class CurrencyExchangeRatesList
    {
        /// <summary>
        /// Gets or sets the list of currency exchange rates
        /// </summary>
        public List<CurrencyExchangeRates> List = new List<CurrencyExchangeRates>();
    }
}