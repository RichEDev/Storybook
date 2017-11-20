namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Holds the detail about password complexity requirements.
    /// </summary>
    public class PasswordPolicy : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of required password options.
        /// </summary>
        public List<string> Requirements { get; set; } 
    }
}