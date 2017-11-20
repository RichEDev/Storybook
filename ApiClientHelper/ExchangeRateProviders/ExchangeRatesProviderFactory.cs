
namespace ApiClientHelper.ExchangeRateProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ApiClientHelper.ExchangeRateProviders.Enums;
    using ApiClientHelper.ExchangeRateProviders.InterFaces;
    using ApiClientHelper.ExchangeRateProviders.OpenExchangeRate;
    using ApiClientHelper.Models;
    using ApiClientHelper.Responses;

    /// <summary>
    /// The exchange rates provider factory.
    /// </summary>
    public class ExchangeRatesProviderFactory
    {
        /// <summary>
        /// The _exchange rate providers.
        /// </summary>
        private readonly List<IExchangeRateProvider> _exchangeRateProviders;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRatesProviderFactory"/> class.
        /// </summary>
        public ExchangeRatesProviderFactory()
        {
            this._exchangeRateProviders = new List<IExchangeRateProvider> { new OpenExchangeRates() };
        }

        /// <summary>
        /// Populates exchange rates from a 3rd party provider
        /// </summary>
        /// <param name="baseCurrency">
        /// The base currency for which you want the exchange rates
        /// </param>
        /// <param name="date">
        /// The date for which you want the exchange rates
        /// </param>
        /// <param name="provider">
        /// The exchange rate provider for the account.
        /// </param>
        /// <param name="activeCurrencies">
        /// The active Currencies for the account
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyExchangeRates"/>.
        /// </returns>
        public CurrencyExchangeRates GetExchangeRates(Currency baseCurrency, DateTime date, ExchangeRateProvider provider, Currencies activeCurrencies)
        {
            return this._exchangeRateProviders.First(e => e.Equals(provider)).GetExchangeRates(activeCurrencies, baseCurrency, date);
  
        }


    }
}
