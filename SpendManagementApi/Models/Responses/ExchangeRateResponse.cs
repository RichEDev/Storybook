namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The exchange rate response
    /// </summary>
    public class ExchangeRateResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Driving licence is added successfully.
        /// </summary>
        public bool IsExchangeRateAdded { get; set; }

        /// <summary>
        /// Gets or sets the current record of the Driving licence.
        /// </summary>
        public List<CurrencyExchangeRates> Records { get; set; }
    }
}