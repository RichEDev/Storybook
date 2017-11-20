namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Representing receipt object details uploaded for an expense item.
    /// </summary>
    public class Receipt : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the receipt url.
        /// </summary>
        public string ReceiptUrl { get; set; }

        /// <summary>
        /// Gets or sets a Base64 string which represents the receipt data.
        /// </summary>
        public string ReceiptData { get; set; }

        /// <summary>
        /// Gets or sets the receipt thumbnaile url.
        /// </summary>
        public string ReceiptThumbnailUrl { get; set; }


        /// <summary>
        /// Gets or sets a Base64 string which represents the receipt thumbnail data.
        /// </summary>
        public string ReceiptThumbnailData { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the mime header.
        /// </summary>
        public string MimeHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is approver.
        /// </summary>
        public bool IsApprover { get; set; }
    }
}