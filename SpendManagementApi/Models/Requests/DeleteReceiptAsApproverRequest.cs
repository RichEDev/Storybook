namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;
    
    /// <summary>
    /// A request to hold the details of the delete receipt as an approver details.
    /// </summary>
    public class DeleteReceiptAsApproverRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        public int ExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the receipt Id of the receipt.
        /// </summary>
        public int ReceiptId { get; set; }

        /// <summary>
        /// Gets or sets the approver's reason for the receipt deletion. 
        /// </summary>
        public string DeleteReason { get; set; }
    }
}