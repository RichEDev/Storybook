namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// The InboundRecord column types.
    /// </summary>
    public enum ValueTypes
    {
        /// <summary>
        /// A static column type.
        /// </summary>
        Static,

        /// <summary>
        /// Group by this column.
        /// </summary>
        GroupBy,

        /// <summary>
        /// A Value column type, so can be summarised.
        /// </summary>
        Value,

        /// <summary>
        /// A NULL column.
        /// </summary>
        Null
    }
}