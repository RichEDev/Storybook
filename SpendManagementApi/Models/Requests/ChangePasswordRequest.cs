namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Encapsulates the details required for a user to change their password.
    /// </summary>
    public class ChangePasswordRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the reset key.
        /// </summary>
        public string ResetKey { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
