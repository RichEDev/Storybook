namespace SpendManagementLibrary.Interfaces
{
    using System;

    /// <summary>
    /// A common interface for support ticket attachments
    /// </summary>
    public interface ISupportTicketAttachment
    {
        /// <summary>
        /// The identifier of the attachment
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The attachment's filename
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        Int64 FileSize { get; set; }

        /// <summary>
        /// The contents of the file
        /// </summary>
        byte[] FileBytes { get; set; }

        /// <summary>
        /// When the attachment was created
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// The Identifier of the user who created the attachment
        /// </summary>
        int CreatedBy { get; set; }

    }
}
