namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request to copy system greenlight entity.
    /// </summary>
    public class CustomEntityToCopy
    {
        /// <summary>
        /// Custom entityid to copy.
        /// </summary>
        [Required]
        public int EntityId { get; set; }

        /// <summary>
        /// Accountid of the target database.
        /// </summary>
        [Required]
        public int TargetAccountId { get; set; }

    }
}