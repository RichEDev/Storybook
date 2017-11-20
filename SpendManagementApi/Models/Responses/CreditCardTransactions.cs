namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The credit card transaction response
    /// </summary>
    public class CreditCardTransactions : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of <see cref="CreditCardTransaction">CreditCardTransaction</see>
        /// </summary>
        public List<CreditCardTransaction> List { get; set; }
    }
}