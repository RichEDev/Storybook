namespace SpendManagementApi.Models.Requests.SelfRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Models.Types;

    /// <summary>
    /// 
    /// </summary>
    public class SelfRegistrationRequest : SelfRegistrationInitiatorRequest
    {
        /// <summary>
        /// The extension number of the registrant.
        /// </summary>
        [StringLength(50, ErrorMessage = @"Extension number should be 50 characters or less")]
        string ExtensionNumber { get; set; }

        /// <summary>
        /// The mobile number of the registrant.
        /// </summary>
        [StringLength(50, ErrorMessage = @"Mobile number should be 50 characters or less")]
        string MobileNumber { get; set; }

        /// <summary>
        /// The pager number for the registrant.
        /// </summary>
        [StringLength(50, ErrorMessage = @"Pager number should be 50 characters or less")]
        string PagerNumber { get; set; }

        /// <summary>
        /// The current address information for ther registrant.
        /// </summary>
        Address Address { get; set; }

        /// <summary>
        /// The date the registrant moved to his or her current address.
        /// </summary>
        DateTime DateMovedToAddress { get; set; }

        /// <summary>
        /// The home telelphone number of the registrant.
        /// </summary>
        [StringLength(50, ErrorMessage = @"Extension number should be 50 characters or less")]
        string HomeTelephone { get; set; }

        /// <summary>
        /// The Capture Plus ID to be used for address searches.
        /// </summary>
        [StringLength(40, ErrorMessage = @"Capture Plus ID should be 40 characters or less")]
        string CapturePlusId { get; set; }


    }
}