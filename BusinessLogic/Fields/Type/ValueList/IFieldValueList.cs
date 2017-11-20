namespace BusinessLogic.Fields.Type.ValueList
{
    using BusinessLogic.Fields.Type.Base;
    public interface IFieldValueList
    {
        /// <summary>
        /// Get or set the List Item values for this <see cref="IField"/>
        /// </summary>
        ListItemValues ListItemValues { get; set; }
    }
}