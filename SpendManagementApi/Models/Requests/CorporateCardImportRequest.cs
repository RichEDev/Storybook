namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;
    /// <summary>
    /// Defines a request that describes corporate card import details (including file data).
    /// </summary>
    public class CorporateCardImportRequest : ApiResponse
    {
        /// <summary>
        /// Gets or sets the Account Id for the Corporate Card import
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the Corporate Card Id based on the filename
        /// </summary>
        public int CorporateCardId { get; set; }

        /// <summary>
        /// The original filename used for the query.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the Corporate Card File data. This should be a Base 64 string
        /// </summary>
        public string CorporateCardFileReceipt { get; set; }
    }
}