namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// The custom entity list item.
    /// </summary>
    [Serializable]
    public class CustomEntityListItem
    {
        /// <summary>
        /// Gets or sets the list item value.
        /// </summary>
        public string elementText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the element value.
        /// </summary>
        public int elementValue { get; set; }

        /// <summary>
        /// Gets or sets the element order.
        /// </summary>
        public int elementOrder { get; set; }


        /// <summary>
        /// return current List Item as a list attribute element.
        /// </summary>
        /// <returns>
        /// The <see cref="cListAttributeElement"/>.
        /// </returns>
        public cListAttributeElement ToListAttributeElement()
        {
            return new cListAttributeElement(
                this.elementValue, this.elementText, this.elementOrder, this.Archived);
        }
    }
}
