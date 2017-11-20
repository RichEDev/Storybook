namespace ApiClientHelper.Models
{
    using ApiClientHelper.ExchangeRateProviders.Enums;
    using ApiClientHelper.Responses;

    /// <summary>
    /// Currency exchange rates are collections of conversion values that affect <see cref="Currency">Currencies</see>.
    /// </summary>
    public class CurrencyExchangeRates
    {
        /// <summary>
        /// Gets or sets the unique Id of the exchange rate.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        ///  Gets or sets the type of the currency this exchange rate applies to.
        /// </summary>
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        ///  Gets or sets the list of actual exchange rates for their types.
        /// </summary>
        public ExchangeRates ExchangeRates { get; set; }

    }
}