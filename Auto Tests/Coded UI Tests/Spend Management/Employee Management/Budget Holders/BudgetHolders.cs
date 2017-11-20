namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders
{

    using Auto_Tests.Coded_UI_Tests.Spend_Management.User_Management.Employees;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The budget holders.
    /// </summary>
    public class BudgetHolders
    {
         /// <summary>
        ///  The Sql Items
        /// </summary>
        public static string SqlItems = "Select * from budgetholders where budgetholderid = @budgetholderid";
         
        /// <summary>
        /// Initialises a new instance of the <see cref="BudgetHolders"/> class.
        /// </summary>
        /// <param name="budgetHolderId">
        /// The budget holder id.
        /// </param>
        /// <param name="budgetHolder">
        /// The budget holder.
        /// </param>
        /// <param name="descritption">
        /// The descritption.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        public BudgetHolders(int budgetHolderId, string budgetHolder, string descritption, int employeeId, Employees.Employees employee)
        {
            this.BudgetHolderId = budgetHolderId;
            this.BudgetHolder = budgetHolder;
            this.Description = descritption;
            this.EmployeeId = employeeId;
            this.Employee = employee;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="BudgetHolders"/> class.
        /// </summary>
        public BudgetHolders()
        {
        }

        /// <summary>
        /// Gets or sets the budget holder id.
        /// </summary>
        internal int BudgetHolderId { get; set; }

        /// <summary>
        /// Gets or sets the budget holder.
        /// </summary>
        internal string BudgetHolder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        internal int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        internal Employees.Employees Employee { get; set; }

        /// <summary>
        /// Gets the budget holder grid values.
        /// </summary>
        internal List<string> BudgetHolderGridValues 
        {
            get
            {
                return new List<string> 
                             {
                                this.BudgetHolder, 
                                this.Description, 
                                this.Employee.EmployeeFullName
                             };
            }
        }

        internal string ApproverName
        {
            get
            {
                return String.Concat(this.BudgetHolder,  " (Budget Holder)");
            }
        }
    }
}
