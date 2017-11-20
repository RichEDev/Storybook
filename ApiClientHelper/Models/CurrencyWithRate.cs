namespace ApiClientHelper.Models
{
    /// <summary>
    /// The currency with rate.
    /// </summary>
    public class CurrencyWithRate
    {
        /// <summary>
        /// Gets or sets the currency id.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate.
        /// </summary>
        public double ExchangeRate { get; set; }
    }
}