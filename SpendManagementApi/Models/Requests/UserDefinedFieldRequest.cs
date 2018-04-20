namespace SpendManagementApi.Models.Requests
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    /// <summary>
    /// The user defined field request.
    /// </summary>
    public class UserDefinedFieldRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the user defined field id.
        /// </summary>
        public int UserDefinedFieldId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mandatory.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink path.
        /// </summary>
        public string HyperlinkPath { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink text.
        /// </summary>
        public string HyperlinkText { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// Gets or sets the max length.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is item specific.
        /// </summary>
        public bool ItemSpecific { get; set; }

        /// <summary>
        /// Whether the employee is allowed to populate.
        /// </summary>
        public bool AllowEmployeeToPopulate { get; set; }

        /// <summary>
        /// Gets or sets the table id.
        /// </summary>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public AttributeFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the applies to table Id.
        /// </summary>
        public Guid AppliesToTableId { get; set; }

        /// <summary>
        /// Gets or sets the max rows.
        /// </summary>
        public int MaxRows { get; set; }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        public List<UdfListElement> ListItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow search.
        /// </summary>
        public bool AllowSearch { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is encrypted.
        /// </summary>
        public bool Encrypted { get; set; }
    }
}