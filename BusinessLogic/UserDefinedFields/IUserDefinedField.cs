

namespace BusinessLogic.UserDefinedFields
{
    using System;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Interfaces;
    public interface IUserDefinedField:IField, IArchivable
    {
        /// <summary>
        /// Gets and sets the User defined Field Id.
        /// </summary>
        int UserDefinedFieldId { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field label.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field description.
        /// </summary>
        string UserDefinedDescription { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field tooltip.
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field Order.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field format.
        /// </summary>
        int Format { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field default value.
        /// </summary>
        object DefaultValue { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field precision (for numeric types).
        /// </summary>
        int Precision { get; set; }// decimal only?

        /// <summary>
        /// Gets and sets the User defined Field Hyperlink Text.
        /// </summary>
        string HypertextLink { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field hyperlink path.
        /// </summary>
        string HypertextPath { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field Display Field <see cref="Guid  "/> which points to an <see cref="IField"/>.
        /// </summary>
        Guid DisplayFieldId { get; set; }

        /// <summary>
        /// Gets and sets the User defined Field Max Rows.
        /// </summary>
        int MaxRows { get; set; }
    }
}
