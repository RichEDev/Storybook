namespace EsrGo2FromNhsWcfLibrary.Base
{
    using System;

    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The data class attribute to set the key field for a specific field on a data class.
    /// </summary>
    public sealed class DataClassAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether is key field.
        /// </summary>
        public bool IsKeyField { get; set; }

        /// <summary>
        /// Gets or sets the field id.
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// Gets or sets the column ref.
        /// </summary>
        public int ColumnRef { get; set; }

        /// <summary>
        /// Gets or sets the element type.
        /// </summary>
        public TemplateMapping.ImportElementType ElementType { get; set; }

        /// <summary>
        /// Gets or sets the table id.
        /// </summary>
        public string TableId { get; set; }
    }
}
