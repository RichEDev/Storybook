namespace SpendManagementApi.Models
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// A response that returns a string
    /// </summary>
    public class StringResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string Value { get; set; }
    }
}