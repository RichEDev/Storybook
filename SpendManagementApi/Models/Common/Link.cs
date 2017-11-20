namespace SpendManagementApi.Models.Common
{
    /// <summary>
    /// Represents a link to another related resource.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The relationship that between this link and the current link.
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// The absoulute URL to the resource.
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// The title of this link.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Whether the URI is templated to accept parameters.
        /// </summary>
        public bool IsTemplated { get; set; }
    }
}