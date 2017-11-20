namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    /// <summary>
    /// Custom Entity Record API Class
    /// </summary>
    public class CustomEntityRecord : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the identifier of the record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the data for the record.
        /// The key is an object because cCustomEntities.getEntityRecord() returns a list that uses integer keys,
        /// whereas cGridNew (used for getting multiple records) returns a list that uses string keys.
        /// </summary>
        public SortedList<object, object> Data { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="Attachment">attachments</see> associated with this custom entity record
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="Attachment">torch history attachments</see> associated with this custom entity record
        /// </summary>
        public List<Attachment> TorchAttachments { get; set; }
    }
}