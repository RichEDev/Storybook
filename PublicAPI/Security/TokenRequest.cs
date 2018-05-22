namespace PublicAPI.Security
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the basic details required for making a token request.
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Gets or sets the employeeId for the employee using the API.
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the accountId for the employee using the API.
        /// </summary>
        [Required]
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the secret key used for logon
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the sub-account id for the current request.
        /// </summary>
        public int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the delegateId if the employee using the API is logged in as a delegate.
        /// </summary>
        public int? DelegateId { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes this token should last for before expiring.
        /// </summary>
        public int TimeoutMinutes { get; set; }
    }
}