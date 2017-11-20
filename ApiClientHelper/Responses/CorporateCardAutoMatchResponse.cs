namespace ApiClientHelper.Responses
{
    /// <summary>
    /// A response containing Account Id and Corporate Card Id matching the given filename
    /// </summary>
    public class CorporateCardAutoMatchResponse 
    {
        /// <summary>
        /// Gets or sets the Account Id for the Corporate Card import
        /// minus values signify failures
        /// -1 Failed to validate
        /// -100 Duplicate customer ID
        /// The file 0 Failed validation
        /// </summary>
        public int Item { get; set; }

    }
}
