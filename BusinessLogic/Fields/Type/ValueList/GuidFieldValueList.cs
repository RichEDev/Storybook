namespace BusinessLogic.Fields.Type.ValueList
{
    using BusinessLogic.Fields.Type.Base;

    public class GuidFieldValueList: GuidField, IFieldValueList
    {
        /// <summary>
        /// Get or set the List Item values for this <see cref="IField"/>
        /// </summary>
        public ListItemValues ListItemValues { get; set; }

        /// <summary>
        /// Create an instance of <see cref="GuidFieldValueList"/>
        /// </summary>
        /// <param name="field">The <see cref="Field"/> to cast to <seealso cref="IFieldValueList"/></param>
        /// <param name="listItemValues">The <see cref="ListItemValues"/> to include in the object.</param>
        public GuidFieldValueList(Field field, ListItemValues listItemValues)
            : base(field)
        {
            this.ListItemValues = listItemValues;
        }
    }
}
