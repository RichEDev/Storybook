namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The corporate card transaction details response.
    /// </summary>
    public class CorporateCardTransactionDetailsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the transaction details.
        /// </summary>
        public Dictionary<string, string> TransactionDetails { get; set; }
    }
}