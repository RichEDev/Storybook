namespace SpendManagementApi.Models
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The find passenger request.
    /// </summary>
    public class FindPassengerRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the search criteria.
        /// </summary>
        public string Criteria { get; set; }
    }
}