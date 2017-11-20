
namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request to Process calculation.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Account Id for the process
        /// </summary>
        [Required]
        public int AccountId { get; set; }
               
    }
}