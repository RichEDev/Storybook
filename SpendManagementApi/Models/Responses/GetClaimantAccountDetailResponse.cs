namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Claiman account detail
    /// </summary>
    public class GetClaimantAccountDetailResponse : ApiResponse
    {
        /// <summary>
        /// Whether the employee has credit cards assigned
        /// </summary>
        public bool HasCreditCard { get; set; }

        /// <summary>
        /// Whether the employee has purchase cards assigned
        /// </summary>
        public bool HasPurchaseCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user is an NHS customer
        /// </summary>
        public bool IsNHSCustomer { get; set; }
    }
}