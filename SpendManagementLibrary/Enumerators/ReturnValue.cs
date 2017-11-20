namespace SpendManagementLibrary
{
    /// <summary>
    /// Standard return values
    /// </summary>
    public enum ReturnValues
    {
        Success = 0,
        AlreadyExists = -1,
        Error=-2,
        EmployeeHomeLocationAssigned = -3,
        EmployeeWorkLocationAssigned = -4,
        ExpenseItemLocationAssigned = -5,
        CustomFieldAssignedToWorkflowConditon = -6,
        CustomFieldAssignedToCustEntView = -7,
        ReferenceByCustomEntityOrUDF = -10,
        InvalidFile = -11,
        InvalidFileType = -12,
        ErrorWithAttachmentDataSave = -13
}

    /// <summary>
    /// Return values for an employee
    /// </summary>
    public enum EmployeeReturnValues
    {
        AlreadyExists = -1,
        HasClaimsInSubmission,
        IsCheckAndPayer
    }
}
