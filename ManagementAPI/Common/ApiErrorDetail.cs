namespace ManagementAPI.Common
{
    /// <summary>
    /// The api error detail.
    /// </summary>
    public class ApiErrorDetail
    {
        /// <summary>
        /// Short code for the error 
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// A human-friendly message to help with solving the error.
        /// </summary>
        public string Message { get; set; }
    }
}