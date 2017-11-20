namespace SpendManagementApi.Models.Responses
{
    using System.Web.Http.Description;

    /// <summary>
    /// A class holding the response information returned to mobile users upon login.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileLoginResponse : LoginResponse
    {
        /// <summary>
        /// Gets or sets the login response from the user credential validation.
        /// </summary>
        public int LoginResponse { get; set; }

        /// <summary>
        /// Gets or sets the attempts remaining before account lockup.
        /// </summary>
        public int AttemptsRemaining { get; set; }
    }
}