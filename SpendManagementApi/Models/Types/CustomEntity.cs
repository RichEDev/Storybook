namespace SpendManagementApi.Models.Types
{
    using System.Web.Http.Description;
    using System.Collections.Generic;

    /// <summary>
    /// An instance of a Custom Entity
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntity : BaseExternalType
    {
        /// <summary>
        /// Gets or sets a list of <see cref="CustomEntityFormField">custom entity form fields</see>
        /// </summary>
        public List<CustomEntityFormField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the identifier of this custom entity
        /// </summary>
        public int Id { get; set; }
    }
}