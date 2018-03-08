namespace ManagementAPI.Models.Responses
{
    using ManagementAPI.Interface;

    /// <summary>
    /// The Response interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IResponse<T> : IApiResponse
    {
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        T Item { get; set; }
    }
}