namespace Expenses_Reports.Formula
{
    using System;

    public class NullTypeSelector : ITypeSelector
    {
        private Type _type;

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
        public string Delimiter => "";

        public NullTypeSelector()
        {
            this._type = typeof(DBNull);
        }
    }
}