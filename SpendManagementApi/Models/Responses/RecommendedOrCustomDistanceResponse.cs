namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// A response that contains a distance for a mileage lookup
    /// </summary>
    public class RecommendedOrCustomDistanceResponse : ApiResponse
    {
        /// <summary>
        /// Get or sets the mileage distance
        /// </summary>
        public decimal? Distance { get; set; }
    }
}