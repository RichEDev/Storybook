namespace SpendManagementLibrary
{
    /// <summary>
    /// The customer type for expenses.
    /// </summary>
    public enum CustomerType 
    {
        /// <summary>
        /// Standard, non-NHS customer.
        /// </summary>
        Standard=1,

        /// <summary>
        /// NHS trust customer.
        /// </summary>
        NHS=2,
    }
}