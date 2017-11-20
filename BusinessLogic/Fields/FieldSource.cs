namespace BusinessLogic.Fields
{
    /// <summary>
    /// the source of a field..
    /// </summary>
    public enum FieldSource
    {
        /// <summary>
        /// A metabase field.
        /// </summary>
        Field = 0,

        /// <summary>
        /// a custom field (Including user defined fields)
        /// </summary>
        Custom = 1,

        /// <summary>
        /// A custom entity field..
        /// </summary>
        CustomEntity = 2
    }
}