namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// The flag action.
    /// </summary>
    public enum FlagAction
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Flag the expense item.
        /// </summary>
        FlagItem = 1,

        /// <summary>
        /// Block the expense item from being added.
        /// </summary>
        BlockItem
    }
}
