namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// The flag frequency type.
    /// </summary>
    public enum FlagFrequencyType
    {
        /// <summary>
        /// Looks back from today over the period specified.
        /// </summary>
        InTheLast = 1,

        /// <summary>
        /// Checks dates in the range given such as this calendar year.
        /// </summary>
        Every
    }
}
