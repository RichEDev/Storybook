namespace SpendManagementLibrary.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A common interface for support tickets
    /// </summary>
    public interface ISupportTicket
    {
        /// <summary>
        /// Not necessarily the identifier, but a friendly number used to identify the support ticket
        /// </summary>
        string CaseNumber { get; set; }

        /// <summary>
        /// The subject line of the support request
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// The full description of the problem
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// A list of attached files
        /// </summary>
        List<ISupportTicketAttachment> Attachments { get; set; }

        /// <summary>
        /// A list of associated comments
        /// </summary>
        List<ISupportTicketComment> Comments { get; set; }

        /// <summary>
        /// The status of the support ticket
        /// </summary>
        string Status { get; set; }

        /// <summary>
        /// When the ticket was created
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// When the ticket was last modified
        /// </summary>
        DateTime ModifiedOn { get; set; }
    }
}
