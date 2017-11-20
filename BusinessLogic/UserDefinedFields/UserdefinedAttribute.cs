namespace BusinessLogic.UserDefinedFields
{
    using System;

    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// The userdefined attribute.
    /// </summary>
    public class UserdefinedAttribute : IFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserdefinedAttribute"/> class.
        /// </summary>
        /// <param name="userdefinedFieldId">
        /// The userdefined field id.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="precision">
        /// The precision.
        /// </param>
        /// <param name="hyperlinkText">
        /// The hyperlink text.
        /// </param>
        /// <param name="hyperlinkPath">
        /// The hyperlink path.
        /// </param>
        /// <param name="displayFieldId">
        /// The display field id.
        /// </param>
        /// <param name="maxRows">
        /// The max rows.
        /// </param>
        public UserdefinedAttribute(int userdefinedFieldId, string label, string description, string tooltip, int order, int format, object defaultValue, int precision, string hyperlinkText, string hyperlinkPath, Guid? displayFieldId, int maxRows)
        {
            this.UserDefinedFieldId = userdefinedFieldId;
            this.Label = label;
            this.UserDefinedDescription = description;
            this.Tooltip = tooltip;
            this.Order = order;
            this.Format = format;
            this.DefaultValue = defaultValue;
            this.Precision = precision;
            this.HyperlinkText = hyperlinkText;
            this.HyperlinkPath = hyperlinkPath;
            this.DisplayFieldId = displayFieldId;
            this.MaxRows = maxRows;
        }

        /// <summary>
        ///  Gets a value indicating whether or not this <see cref="UserdefinedAttribute"/> is a hyper link or not.
        /// </summary>
        public bool IsHyperLink => string.IsNullOrWhiteSpace(this.HyperlinkText) == false && string.IsNullOrWhiteSpace(this.HyperlinkPath) == false;

        /// <summary>
        /// Gets or sets the User defined Field Id.
        /// </summary>
        public int UserDefinedFieldId { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field description.
        /// </summary>
        public string UserDefinedDescription { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field Order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field format.
        /// </summary>
        public int Format { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field default value.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field precision (for numeric types).
        /// </summary>
        public int Precision { get; set; }// decimal only?

        /// <summary>
        /// Gets or sets the User defined Field Hyperlink Text.
        /// </summary>
        public string HyperlinkText { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field hyperlink path.
        /// </summary>
        public string HyperlinkPath { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field Display Field <see cref="Guid  "/> which points to an <see cref="IField"/>.
        /// </summary>
        public Guid? DisplayFieldId { get; set; }

        /// <summary>
        /// Gets or sets the User defined Field Max Rows.
        /// </summary>
        public int MaxRows { get; set; }
    }
}
