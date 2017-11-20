namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Custom Entity View API class.
    /// </summary>
    public class CustomEntityView : ArchivableBaseExternalType
    {
        /// <summary>
        /// Gets or sets the unique Id of this View
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the related custom entity Id.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the plural name of the view's custom entity.
        /// </summary>
        public string EntityPluralName { get; set; }

        /// <summary>
        /// Gets or sets the URI to the corresponding icon.
        /// </summary>
        public string IconUri { get; set; }

        /// <summary>
        /// Gets or sets the default edit <see cref="CustomEntityForm">form</see>.
        /// </summary>
        public CustomEntityForm DefaultEditForm { get; set; }
    }
}