namespace BusinessLogic.Fields.Type.ValueList
{
    using BusinessLogic.Fields.Type.Base;

    public class IntegerFieldValueList : IntegerField, IFieldValueList
    {
        /// <summary>
        /// Get or set the List Item values for this <see cref="Field"/>
        /// </summary>
        public ListItemValues ListItemValues { get; set; }

        /// <summary>
        /// Create an instance of <see cref="IntegerFieldValueList"/>
        /// </summary>
        /// <param name="field">The <see cref="Field"/> to cast to <seealso cref="IFieldValueList"/></param>
        /// <param name="listItemValues">The <see cref="ListItemValues"/> to include in the object.</param>
        public IntegerFieldValueList(Field field, ListItemValues listItemValues)
            : base(field)
        {
            this.ListItemValues = listItemValues;
        }

    }
}
