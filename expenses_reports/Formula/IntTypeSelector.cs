using System;

namespace Expenses_Reports.Formula
{
    internal class IntTypeSelector : ITypeSelector
    {
        private readonly Type _type;

        public IntTypeSelector()
        {
            this._type = typeof(int);
        }

        /// <summary>
        /// The type of the current field.
        /// </summary>
        public Type Type => this._type;

        /// <summary>
        /// The default value to use when an instance of this type is not found.
        /// </summary>
        public object DefaultValue => 0;

        /// <summary>
        /// The delimiter (if any) used when formatting this value.
        /// </summary>
        public string Delimiter => string.Empty;
    }
}