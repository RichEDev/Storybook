namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Common;
    
    /// <summary>
    /// Encapsulates the extra details required to log in to the API.
    /// </summary>
    public class LoginRequest : ApiRequest
    {
        /// <summary>
        /// The username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The password to match the <see cref="Username"/>
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// The company for which the API user is using the API.
        /// </summary>
        [Required]
        public string Company { get; set; }
    }
}