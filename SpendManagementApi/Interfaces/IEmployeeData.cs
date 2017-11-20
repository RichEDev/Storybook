namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// The EmployeeData interface.
    /// </summary>
    public  interface IEmployeeData
    {
        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        int ExpenseId { get; set; }
    }
}
