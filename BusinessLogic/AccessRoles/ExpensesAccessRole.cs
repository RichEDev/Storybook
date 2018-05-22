namespace BusinessLogic.AccessRoles
{
    using System;

    /// <summary>
    /// <see cref="ExpensesAccessRole">ExpensesAccessRole</see> defines criteria for an accessRole
    /// </summary>
    [Serializable]
    public class ExpensesAccessRole : AccessRole,  IExpensesAccessRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpensesAccessRole"/> class. 
        /// </summary>
        /// <param name="accessRole">
        /// The accessRole 
        /// </param>
        /// <param name="claimMaximumAmount">
        /// The maximum claim amount for the ExpenseAccessRole 
        /// </param>
        /// <param name="claimMinimumAmount">
        /// The minimum claim amount for the ExpenseAccessRole
        /// </param>
        /// <param name="canEditCostCode">
        /// Whether the ExpenseAccessRole can edit a cost code
        /// </param>
        /// <param name="canEditDepartment">
        /// Whether the ExpenseAccessRole can edit a department
        /// </param>
        /// <param name="canEditProjectCode">
        /// Whether the ExpenseAccessRole can edit a project code
        /// </param>
        /// <param name="mustHaveBankAccount">
        /// Whether the ExpenseAccessRole must have a bank account
        /// </param>
        public ExpensesAccessRole(IAccessRole accessRole, decimal? claimMaximumAmount, decimal? claimMinimumAmount, bool canEditCostCode, bool canEditDepartment, bool canEditProjectCode, bool mustHaveBankAccount) : base(accessRole.Id, accessRole.Name, accessRole.Description, accessRole.ApplicationScopes, accessRole.ReportsAccess, accessRole.AccessScopes)
        {
            this.ClaimMaximumAmount = claimMaximumAmount;
            this.ClaimMinimumAmount = claimMinimumAmount;
            this.CanEditCostCode = canEditCostCode;
            this.CanEditDepartment = canEditDepartment;
            this.CanEditProjectCode = canEditProjectCode;
            this.MustHaveBankAccount = mustHaveBankAccount;
        }

        /// <summary>
        /// Gets or sets the maximum claim amount for the ExpenseAccessRole
        /// </summary>
        public decimal? ClaimMaximumAmount { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum claim amount for the ExpenseAccessRole
        /// </summary>
        public decimal? ClaimMinimumAmount { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ExpenseAccessRole can edit a cost code
        /// </summary>
        public bool CanEditCostCode { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether ExpenseAccessRole can edit a department
        /// </summary>
        public bool CanEditDepartment { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ExpenseAccessRole can edit a project code
        /// </summary>
        public bool CanEditProjectCode { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether ExpenseAccessRole must have a bank account
        /// </summary>
        public bool MustHaveBankAccount { get; protected set; }
    }
}
