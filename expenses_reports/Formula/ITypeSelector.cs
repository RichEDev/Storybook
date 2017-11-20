using System;

namespace Expenses_Reports.Formula
{
    /// <summary>
    /// Manage the type of a field.
    /// </summary>
    internal interface ITypeSelector
    {
        /// <summary>
        /// The type of the current field.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// The default value to use when an instance of this type is not found.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// The delimiter (if any) used when formatting this value.
        /// </summary>
        string Delimiter { get; }
    }
}