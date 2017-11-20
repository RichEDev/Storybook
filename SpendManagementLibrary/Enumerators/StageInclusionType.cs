namespace SpendManagementLibrary.Enumerators
{
    using System;

    [Serializable()]
    public enum StageInclusionType
    {
        None,
        Always = 1,
        ClaimTotalExceeds = 2,
        ExpenseItemExceeds = 3,
        IncludesCostCode = 4,
        ClaimTotalBelow = 5,
        IncludesExpenseItem = 6,
        OlderThanDays = 7,
        IncludesDepartment = 8,
        ValidationFailedTwice = 9
    }
}