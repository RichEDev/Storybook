namespace ApiClientHelper.Models
{
    using System.Collections.Generic;

    using ApiClientHelper.ExchangeRateProviders.Enums;

    /// <summary>
    /// The currency exchange rates list.
    /// </summary>
    public class CurrencyExchangeRatesList
    {
        /// <summary>
        /// The list of currency exchange rates
        /// </summary>
        public List<CurrencyExchangeRates> List = new List<CurrencyExchangeRates>();
    }
}