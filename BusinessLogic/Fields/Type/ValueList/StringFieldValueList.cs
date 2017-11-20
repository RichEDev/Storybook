namespace BusinessLogic.Fields.Type.ValueList
{
    using BusinessLogic.Fields.Type.Base;

    public class StringFieldValueList : StringField, IFieldValueList
    {
        /// <summary>
        /// Get or set the List Item values for this <see cref="Field"/>
        /// </summary>
        public ListItemValues ListItemValues { get; set; }

        /// <summary>
        /// Create an instance of <see cref="StringFieldValueList"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to cast to <seealso cref="IFieldValueList"/></param>
        /// <param name="listItemValues">The <see cref="ListItemValues"/> to include in the object.</param>
        public StringFieldValueList(Field field, ListItemValues listItemValues)
            : base(field)
        {
            this.ListItemValues = listItemValues;
        }
    }
}
