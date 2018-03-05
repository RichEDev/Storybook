namespace SpendManagementApi.Models.Requests
{
    /// <summary>
    /// Defines a <see cref="WalletReceiptRequest"/> and all its members
    /// </summary>
    public class WalletReceiptRequest
    {
        /// <summary>
        /// Gets or sets the receipt image data. This should be a Base 64 string
        /// </summary>
        public string Receipt { get; set; }

        /// <summary>
        /// The file type of the receipt.
        /// </summary>
        public string FileType { get; set; }
    }
}