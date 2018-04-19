namespace BusinessLogic.GeneralOptions.Framework.Attachments
{
    /// <summary>
    /// Defines a <see cref="AttachmentOptions"/> and it's members
    /// </summary>
    public class AttachmentOptions : IAttachmentOptions
    {
        /// <summary>
        /// Gets or sets the link attachement default
        /// </summary>
        public byte LinkAttachmentDefault { get; set; }

        /// <summary>
        /// Gets or sets the max upload size
        /// </summary>
        public int MaxUploadSize { get; set; }

        /// <summary>
        /// Gets or sets the enable attachment hyperlink
        /// </summary>
        public bool EnableAttachmentHyperlink { get; set; }

        /// <summary>
        /// Gets or sets enable attachment upload
        /// </summary>
        public bool EnableAttachmentUpload { get; set; }

        /// <summary>
        /// Gets or sets the document repository
        /// </summary>
        public string DocumentRepository { get; set; }

        /// <summary>
        /// Gets or sets the open save attachments
        /// </summary>
        public byte OpenSaveAttachments { get; set; }
    }
}
