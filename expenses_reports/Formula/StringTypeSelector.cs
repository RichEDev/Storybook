using System;

namespace Expenses_Reports.Formula
{
    internal class StringTypeSelector : ITypeSelector
    {
        private readonly Type _type;

        public StringTypeSelector()
        {
            this._type = typeof(string);
        }

        /// <summary>
        /// The type of the current field.
        /// </summary>
        public Type Type => this._type;

        /// <summary>
        /// The default value to use when an instance of this type is not found.
        /// </summary>
        public object DefaultValue => string.Empty;

        /// <summary>
        /// The delimiter (if any) used when formatting this value.
        /// </summary>
        public string Delimiter => "\"";
    }
}