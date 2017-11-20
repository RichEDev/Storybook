using SpendManagementLibrary.Interfaces.Expedite;

namespace UnitTest2012Ultimate.API.Utilities
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    
    public class MockExpenseItem : cExpenseItem
    {
        public MockExpenseItem(int id, int claimid, bool returned, ExpenseValidationProgress progress, List<ExpenseValidationResult> results)
            : base(id, ItemType.Cash, 10, 10, "", false, 10, 10, 10, 10, DateTime.Now, 0, 0, 262, returned, false, "", claimid, 1, 1, 1, null, 1, 1, 1, 1, 1, false, 1, false, DateTime.Now, DateTime.Now.Add(TimeSpan.FromDays(1)), 1, 10, 1, 0, 1, 0, 10, 1, false, 1, "", 1, 1, "", 1, 1, 1, 1, 1, false, false, 1, DateTime.UtcNow, GlobalTestVariables.EmployeeId, DateTime.UtcNow, GlobalTestVariables.EmployeeId, 1, MileageUOM.KM, 10, HomeToLocationType.DeductFirstAndLastHome, false, 0)
        {
            this.ValidationProgress = progress;
            this.ValidationResults = results;
        }

        public virtual new ExpenseValidationProgress ValidationProgress { get; set; }
        public virtual new List<ExpenseValidationResult> ValidationResults { get; set; }

        /// <summary>
        /// Updates the validation progress and results of this expense item, in case it is out of sync with the Validate property of the subcat.
        /// Also will populate the results if the account and subcat support it and the Progress is at the correct stage.
        /// </summary>
        /// <param name="account">The account under which to check the validation service.</param>
        /// <param name="subcatSaysValidate">The subcat which will be used for reference.</param>
        /// <param name="validationManager">The validation manager required to populate the results.</param>
        public ExpenseValidationProgress DetermineValidationProgressAndResults(cAccount account, bool subcatSaysValidate,
            IManageExpenseValidation validationManager)
        {
            ValidationProgress = ExpenseValidationProgress.InProgress;
            ValidationResults = new List<ExpenseValidationResult>
            {
                TestExpenseValidationManager.Results[0],
                TestExpenseValidationManager.Results[1]
            };
            return ValidationProgress;
        }
    }
}
