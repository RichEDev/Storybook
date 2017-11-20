namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The global identifier request.
    /// </summary>
    public class GlobalIdentifierRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the global identified.
        /// </summary>
        public string GlobalIdentifier { get; set; }
    }
}