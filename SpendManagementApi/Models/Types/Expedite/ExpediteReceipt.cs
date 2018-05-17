namespace SpendManagementApi.Models.Types.Expedite
{
    /// <summary>
    /// Represents an uploaded file that can be linked to a Claim Line (savedexpense) for a specified account.
    /// </summary>
    public class ExpediteReceipt : Receipt
    {
        /// <summary>
        /// Gets or sets the Account Id of this receipt.
        /// </summary>
        public new int? AccountId { get; set; }
    }
}