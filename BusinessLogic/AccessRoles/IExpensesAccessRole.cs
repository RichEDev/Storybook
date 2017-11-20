namespace BusinessLogic.AccessRoles
{
    /// <summary>
    /// Interface defining common fields of an ExpenseAccessRole
    /// </summary>
    public interface IExpensesAccessRole : IAccessRole
    {
        /// <summary>
        /// Whether the ExpenseAccessRole can edit Cost Codes
        /// </summary>
        bool CanEditCostCode { get; }

        /// <summary>
        /// Whether the ExpenseAccessRole can edit Departments
        /// </summary>
        bool CanEditDepartment { get; }

        /// <summary>
        /// Whether the ExpenseAccessRole can edit Project Codes
        /// </summary>
        bool CanEditProjectCode { get; }

        /// <summary>
        /// The maximum claim amount for the ExpenseAccessRole
        /// </summary>
        decimal? ClaimMaximumAmount { get; }

        /// <summary>
        /// The minimum claim amount for the ExpenseAccessRole
        /// </summary>
        decimal? ClaimMinimumAmount { get; }

        /// <summary>
        /// Whether the ExpenseAccessRole must has a Bank Account
        /// </summary>
        bool MustHaveBankAccount { get; }
    }
}