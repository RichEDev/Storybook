namespace SpendManagementApi.Models.Requests
{
    /// <summary>
    /// Custom Entity request class.
    /// </summary>
    public class CustomEntityRequest
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
    }
}