namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;
    /// <summary>
    /// A response containing Account Id and Corporate Card Id matching the given filename
    /// </summary>
    public class CorporateCardAutoMatchResponse : ApiResponse
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
    }
}