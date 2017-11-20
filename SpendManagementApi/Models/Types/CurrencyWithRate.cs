namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// The class that contains currency id with its exchange rate
    /// </summary>
    public class CurrencyWithRate
    {
        /// <summary>
        /// Gets or sets the currency id
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate
        /// </summary>
        public double ExchangeRate { get; set; }
    }
}