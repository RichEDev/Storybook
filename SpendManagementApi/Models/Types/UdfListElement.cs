namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// A class to hold the list element details for a list type user defined field
    /// </summary>
    public class UdfListElement
    {
        /// <summary>
        /// Gets or sets the element value.
        /// </summary>
        public int ElementValue { get; set; }

        /// <summary>
        /// Gets or sets the element text.
        /// </summary>
        public string ElementText { get; set; }

        /// <summary>
        /// Gets or sets the element order.
        /// </summary>
        public int ElementOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether element is archived.
        /// </summary>
        public bool Archived { get; set; }
    }
}
