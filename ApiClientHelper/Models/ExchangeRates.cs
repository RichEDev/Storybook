namespace ApiClientHelper.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a set of exchange rates for a given currency.
    /// </summary>
    public class ExchangeRates
    {
        /// <summary>
        /// Gets or sets the Currency Date Range - Only required when currency type = Range
        /// </summary>
        public List<CurrencyDateRange> DateRangeExchangeRates { get; set; }
    }
}