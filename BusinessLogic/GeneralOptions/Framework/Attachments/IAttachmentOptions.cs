namespace BusinessLogic.GeneralOptions.Framework.Attachments
{
    /// <summary>
    /// Defines a <see cref="IAttachmentOptions"/> and it's members
    /// </summary>
    public interface IAttachmentOptions
    {
        /// <summary>
        /// Gets or sets the link attachement default
        /// </summary>
        byte LinkAttachmentDefault { get; set; }

        /// <summary>
        /// Gets or sets the max upload size
        /// </summary>
        int MaxUploadSize { get; set; }

        /// <summary>
        /// Gets or sets the enable attachment hyperlink
        /// </summary>
        bool EnableAttachmentHyperlink { get; set; }

        /// <summary>
        /// Gets or sets enable attachment upload
        /// </summary>
        bool EnableAttachmentUpload { get; set; }

        /// <summary>
        /// Gets or sets the document repository
        /// </summary>
        string DocumentRepository { get; set; }

        /// <summary>
        /// Gets or sets the open save attachments
        /// </summary>
        byte OpenSaveAttachments { get; set; }
    }
}
