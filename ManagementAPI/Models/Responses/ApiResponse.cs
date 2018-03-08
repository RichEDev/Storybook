namespace ManagementAPI.Models.Responses
{
    using ManagementAPI.Interface;

    /// <summary>
    /// The api response.
    /// </summary>
    public class ApiResponse : IApiResponse 
    {
        /// <summary>
        /// Gets or sets the response information.
        /// </summary>
        public ApiResponseInformation ResponseInformation { get; set; }
    }

    /// <summary>
    /// The api response.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class ApiResponse<T> : ApiResponse, IResponse<T>
    {
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public T Item { get; set; }
    }
}