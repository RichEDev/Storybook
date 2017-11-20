namespace ApiClientHelper.ExchangeRateProviders.OpenExchangeRate
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;

    using ApiClientHelper.ExchangeRateProviders.InterFaces;
    using ApiClientHelper.Models;
    using ApiClientHelper.Responses;

    using Newtonsoft.Json;

    using SpendManagementLibrary;

    using CurrencyType = Models.CurrencyType;
    using ExchangeRateProvider = Enums.ExchangeRateProvider;

    /// <summary>
    /// The open exchange rates.
    /// </summary>
    public class OpenExchangeRates : IExchangeRateProvider
    {
        /// <summary>
        /// The open exchange rate url.
        /// </summary>
        private const string OpenExchangeRateUrl = "https://openexchangerates.org/api/historical/";

        /// <summary>
        /// The app id for open exchange rate
        /// </summary>
        private static readonly string AppId = ConfigurationManager.AppSettings.Get("openExchangeRates");

        /// <summary>
        /// The _exchange rates list for all providers.
        /// </summary>
        private readonly List<OpenExchangeRatesResponse> _exchangeRates;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenExchangeRates"/> class.
        /// </summary>
        public OpenExchangeRates()
        {
            this._exchangeRates = new List<OpenExchangeRatesResponse>();
        }

        /// <summary>
        /// Is equal to the given exchange rate provider
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> true if it matches
        /// </returns>
        public bool Equals(ExchangeRateProvider provider)
        {
            return provider == ExchangeRateProvider.OpenExchangeRates;
        }

        /// <summary>
        /// Gets exchange rates for the base currency from open exchange rates for the date specified
        /// </summary>
        /// <param name="activecurrencies">
        /// The activecurrencies for the account
        /// </param>
        /// <param name="baseCurrency">
        /// The base currency that you want the exchange rates for.
        /// </param>
        /// <param name="date">
        /// The date that you want the exchange rates for.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CurrencyExchangeRates"/>.
        /// </returns>
        public CurrencyExchangeRates GetExchangeRates(Currencies activecurrencies, Currency baseCurrency, DateTime date)
        {
            OpenExchangeRatesResponse response;
            if (this._exchangeRates.Any(x => x.Base == baseCurrency.AlphaCode))
            {
                response = this._exchangeRates.FirstOrDefault(x => x.Base == baseCurrency.AlphaCode);
            }
            else
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                Console.WriteLine($"Getting exchange rates for - Source Currency: {baseCurrency.AlphaCode}, Date:{formattedDate}");
                var url = OpenExchangeRateUrl + formattedDate + ".json?app_id=" + AppId + "&base=" + baseCurrency.AlphaCode;
                response = DownloadSerializedJsonData<OpenExchangeRatesResponse>(url);
                if (response.Rates == null)
                {
                    Console.WriteLine($"Failed to retrieve exchange rates for {baseCurrency.AlphaCode}");
                    return null;
                }
                this._exchangeRates.Add(response);
            }

            var rate = this.CreateExchangeRateRequest(activecurrencies, baseCurrency, response, date);
            return rate;
        }

        /// <summary>
        /// Download serialized json data.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <typeparam name="T">
        /// Generic class
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private static T DownloadSerializedJsonData<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var jsonData = string.Empty;

                // attempt to download JSON data as a string
                try
                {
                    jsonData = w.DownloadString(url);
                }
                catch (WebException we)
                {
                    cEventlog.LogEntry(
                        "Web Exception returned from service  : " + we.Message);
                    Console.WriteLine($"Web Exception returned from service - {we.Message}");
                }
                catch (NotSupportedException nse)
                {
                    cEventlog.LogEntry(
                        "Not supported Exception returned from service : " + nse.Message);
                    Console.WriteLine($"Not supported Exception returned from service - {nse.Message}");
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Error when downloading serialized data from open exchange rates : " + ex.Message);
                    Console.WriteLine("Historical rates for the requested date are not available - please try a different date");
                }

                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }

        /// <summary>
        /// The create exchange rate request.
        /// </summary>
        /// <param name="activeCurrencies">
        /// The list of active currencies for the account
        /// </param>
        /// <param name="baseCurrency">
        /// The base currency that the exchange rate is populated for
        /// </param>
        /// <param name="response">
        /// The response from open exchange rates
        /// </param>
        /// <param name="date">
        /// The date for which the exchange rate is populated
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyExchangeRates"/>.
        /// </returns>
        private CurrencyExchangeRates CreateExchangeRateRequest(
            Currencies activeCurrencies,
            Currency baseCurrency,
            OpenExchangeRatesResponse response,
            DateTime date)
        {
            var currencyExchangeRate = new CurrencyExchangeRates();
            var exchangeRateTable = new List<CurrencyWithRate>();

            foreach (var activeCurrency in activeCurrencies.List)
            {
                exchangeRateTable.Add(
                    new CurrencyWithRate
                        {
                            CurrencyId = activeCurrency.CurrencyId,
                            ExchangeRate = Convert.ToDouble(response.Rates.FirstOrDefault(rate => rate.Key == activeCurrency.AlphaCode).Value)
                        });
            }

            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            var currencydateRange =
                new CurrencyDateRange
                    {
                        CurrencyDateRangeId = 0,
                        EndDate = date,
                        ApiExchangeRateTable = exchangeRateTable,
                        StartDate = date
                    };
            var listdaterange = new List<CurrencyDateRange> { currencydateRange };
            currencyExchangeRate.CurrencyId = baseCurrency.CurrencyId;
            currencyExchangeRate.CurrencyType = CurrencyType.Range;
            currencyExchangeRate.ExchangeRates = new ExchangeRates { DateRangeExchangeRates = listdaterange };

            return currencyExchangeRate;
        }


    }
}
