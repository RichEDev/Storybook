namespace SpendManagementApi.Models.Requests
{
    using System;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The import transaction request.
    /// </summary>
    public class ImportTransactionRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the card provider name.
        /// </summary>
        public string CardProviderName { get; set; }

        /// <summary>
        /// Gets or sets the statement date.
        /// </summary>
        public DateTime StatementDate { get; set; }

        /// <summary>
        /// Gets or sets the transactionData to import.
        /// </summary>
        public string TransactionData { get; set; }
    }
}