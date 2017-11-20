namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// A class holding the information returned to users who request a change of their password.
    /// </summary>
    public class ChangePasswordResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public int RequestResponse { get; set; }
    }
}
