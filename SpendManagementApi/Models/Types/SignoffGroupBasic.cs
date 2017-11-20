namespace SpendManagementApi.Models.Types
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The basic description of a Signoff Group
    /// </summary>
    public class SignoffGroupBasic
    {
        /// <summary>
        /// The unique Id of the signoffgroup.
        /// </summary>
        [Required]
        public int GroupId { get; set; }

        /// <summary>
        /// The name of the signoffgroup.
        /// </summary>
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// The description of the signoffgroup.
        /// </summary>
        public string Description { get; set; }
    }
}