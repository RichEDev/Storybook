namespace SpendManagementLibrary.Addresses
{
    using Newtonsoft.Json;

    /// <summary>
    /// The post code anywhere lookup result.
    /// </summary>
    [JsonObject(Title = "Item")]
    public class PostcodeAnywhereLookupResult
    {
        /// <summary>
        /// Gets or sets the id of the result.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the match result i.e. the search criteria matched (postcode, part postcode, town)
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// Gets or sets the suggested address.
        /// </summary>
        public string Suggestion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the address is retrivable retrievable.
        /// </summary>
        public bool IsRetrievable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto select.
        /// </summary>
        public bool AutoSelect { get; set; }
    }
}
