namespace PublicAPI.Security
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the basic details required for making a token request.
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// The employeeId for the employee using the API.
        /// </summary>
        [Required]
        public int EmployeeId;

        /// <summary>
        /// The accountId for the employee using the API.
        /// </summary>
        [Required]
        public int AccountId;

        /// <summary>
        /// The delegateId if the employee using the API is logged in as a delegate.
        /// </summary>
        public int? DelegateId;

        /// <summary>
        /// The number of minutes this token should last for before expiring.
        /// </summary>
        public int TimeoutMinutes;
    }
}