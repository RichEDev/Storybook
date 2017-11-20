namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Response for system greenlight action.
    /// </summary>
    public class SystemGreenLightResponse : ApiResponse
    {
        /// <summary>
        /// Message in response for system greenlight copy request.
        /// </summary>
        public string ResponseMessage { get; set; }

    }
}