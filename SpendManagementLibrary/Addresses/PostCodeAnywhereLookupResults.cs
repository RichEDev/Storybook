namespace SpendManagementLibrary.Addresses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// The post code anywhere lookup results.
    /// </summary>
    [JsonObject(Title = "RootObject")]
    public class PostCodeAnywhereLookupResults
    {
        /// <summary>
        /// Gets or sets the list of PostCode Anywhere Lookup Results.
        /// </summary>
        [JsonProperty(PropertyName = "Items")]
        public List<PostcodeAnywhereLookupResult> LookupResults { get; set; }
    }
}
