namespace SpendManagementApi.Models.Common
{
    /// <summary>
    /// ApiErrorDetail represents a particular error encountered whilst processing a request.
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