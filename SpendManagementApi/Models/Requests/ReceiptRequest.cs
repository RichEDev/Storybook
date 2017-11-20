namespace SpendManagementApi.Models.Requests
{
    /// <summary>
    /// Class holds a receipt post.
    /// </summary>
    public class ReceiptRequest
    {
        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        public int ExpenseId { get; set; }

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