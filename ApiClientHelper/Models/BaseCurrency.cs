namespace ApiClientHelper.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// The Base Currency class. 
    /// </summary>
    public class BaseCurrency
    {

        /// <summary>
        ///  Gets or sets the sorted list of Exchange Rates.
        /// </summary>
        public List<CurrencyWithRate> ApiExchangeRateTable { get; set; }

    }
}