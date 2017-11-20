namespace SpendManagementLibrary.DVLA
{
    /// <summary>
    /// The current custom entity record.
    /// </summary>
    public class CurrentCustomEntityRecord
    {
        /// <summary>
        /// Gets or sets the record id.
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the view id.
        /// </summary>
        public int ViewId { get; set; }

        /// <summary>
        /// Gets or sets the ID for the form which will be used to view the licence.
        /// </summary>
        public int FormId { get; set; }
    }
}
