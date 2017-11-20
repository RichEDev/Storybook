using ApiClientHelper.ExchangeRateProviders.Enums;
using ApiClientHelper.Models;
using ApiClientHelper.Responses;
using System;

namespace ApiClientHelper.ExchangeRateProviders.InterFaces
{
    /// <summary>
    /// Exchange Rate provider interface
    /// </summary>
    public interface IExchangeRateProvider
    {
        /// <summary>
        /// Is equal to the given exchange rate provider
        /// </summary>
        /// <param name="provider">
        /// The exchange rate provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> true if it matches
        /// </returns>
        bool Equals(ExchangeRateProvider provider);

        /// <summary>
        /// Gets exchange rates 
        /// </summary>
        /// <param name="baseCurrency">
        /// The base currency for which the exchange rate is to be populated
        /// </param>
        /// <param name="date">
        /// The date for which the exchange rate is to be populated
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyExchangeRates"/>.
        /// </returns>
        CurrencyExchangeRates GetExchangeRates(Currencies activecurrencies, Currency baseCurrency, DateTime date);
    }
}
