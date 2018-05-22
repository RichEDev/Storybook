namespace BusinessLogic.AccessRoles
{
    /// <summary>
    /// Interface defining common fields of an ExpenseAccessRole
    /// </summary>
    public interface IExpensesAccessRole : IAccessRole
    {
        /// <summary>
        /// Gets a value indicating whether the ExpenseAccessRole can edit Cost Codes.
        /// </summary>
        bool CanEditCostCode { get; }

        /// <summary>
        /// Gets a value indicating whether the ExpenseAccessRole can edit Departments.
        /// </summary>
        bool CanEditDepartment { get; }

        /// <summary>
        /// Gets a value indicating whether the ExpenseAccessRole can edit Project Codes.
        /// </summary>
        bool CanEditProjectCode { get; }

        /// <summary>
        /// Gets a value indicating the maximum claim amount for the ExpenseAccessRole.
        /// </summary>
        decimal? ClaimMaximumAmount { get; }

        /// <summary>
        /// Gets a value indicating the minimum claim amount for the ExpenseAccessRole.
        /// </summary>
        decimal? ClaimMinimumAmount { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IExpensesAccessRole"/> must have a Bank Account.
        /// </summary>
        bool MustHaveBankAccount { get; }
    }
}