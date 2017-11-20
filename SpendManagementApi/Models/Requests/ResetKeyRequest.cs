namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Encapsulates requests to the validate reset key method of 
    /// </summary>
    public class ResetKeyRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the reset key.
        /// </summary>
        [Required]
        public string ResetKey { get; set; }
    }
}