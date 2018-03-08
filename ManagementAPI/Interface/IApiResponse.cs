namespace ManagementAPI.Interface
{
    using ManagementAPI.Models.Responses;

    /// <summary>
    /// The ApiResponse interface.
    /// </summary>
    public interface IApiResponse
    {
        /// <summary>
        /// Gets or sets the response information.
        /// </summary>
        ApiResponseInformation ResponseInformation { get; set; }
    }
}