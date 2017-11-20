namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// A class holding the information returned to users who request a 'forgotten details' email.
    /// </summary>
    public class ForgottenDetailsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the response
        /// </summary>
        public int RequestResponse { get; set; }
    }
}