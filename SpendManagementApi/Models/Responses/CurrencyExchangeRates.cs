namespace SpendManagementApi.Models.Responses
{
    using Common;
    using Types;

    /// <summary>
    /// A response containing a particular set of <see cref="CurrencyExchangeRates">CurrencyExchangeRates</see>.
    /// </summary>
    public class CurrencyExchangeRatesResponse : ApiResponse<CurrencyExchangeRates>
    {
    }
  
    /// <summary>
    /// A response containing an exchange rate.
    /// </summary>
    public class CurrencyExchangeRateResponse : ApiResponse
    {
        /// <summary>
        /// The exchange rate
        /// </summary>
        public double Rate { get; set; }
    }
}