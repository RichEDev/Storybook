namespace SpendManagementApi.Models.Types
{
    using System;
    using SpendManagementLibrary;

    /// <summary>
    /// The attachment.
    /// </summary>
    public class Attachment : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the attachment's identifier
        /// </summary>
        public int AttachmentId { get; set; }

        /// <summary>
        /// Gets or sets the created on date.
        /// </summary>
        public new DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the file data bytes.
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the record id of the entity that relates to this attachment.
        /// </summary>
        public int OwnerRecordId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Convenience method for converting a <see cref="cAttachment"/> to this type.
        /// </summary>
        /// <param name="attachment">
        /// The source <see cref="cAttachment"/> attachment.
        /// </param>
        /// <returns>
        /// The new <see cref="Attachment"/>.
        /// </returns>
        public static Attachment From(cAttachment attachment)
        {
            if (attachment == null)
            {
                return null;
            }

            return new Attachment
            {
                AttachmentId = attachment.attachmentID,
                CreatedOn = attachment.CreatedOn,
                FileData = attachment.AttachmentData,
                Filename = attachment.FileName,
                OwnerRecordId = attachment.OwnerRecordID,
                Title = attachment.Title
            };
        }
    }
}