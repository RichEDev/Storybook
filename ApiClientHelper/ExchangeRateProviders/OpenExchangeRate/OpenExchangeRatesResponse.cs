namespace ApiClientHelper.ExchangeRateProviders.OpenExchangeRate
{
    using System.Collections.Generic;

    /// <summary>
    /// The open exchange rates response.
    /// </summary>
    public class OpenExchangeRatesResponse
    {
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public int TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the base.
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Gets or sets the rates.
        /// </summary>
        public Dictionary<string, decimal> Rates { get; set; }

    }
}
