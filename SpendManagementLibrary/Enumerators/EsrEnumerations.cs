namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The include ESR details in add/edit expenses.
    /// </summary>
    public enum IncludeEsrDetails
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The pay point.
        /// </summary>
        PayPoint = 1,

        /// <summary>
        /// The job role.
        /// </summary>
        JobRole = 2,

        /// <summary>
        /// The position name and assignment category
        /// </summary>
        PositionNameAndCategory = 3
    }
}
