namespace SpendManagementLibrary.Selectinator
{
    /// <summary>
    /// Determines the parent filter for n:1 relationships
    /// </summary>
    public class SelectinatorParentFilter
    {
        /// <summary>
        /// The Id of the parent control
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The value of the parent control
        /// </summary>
        public string Value { get; set; }
    }
}
