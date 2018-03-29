namespace SpendManagementLibrary.Enumerators
{
    using System.ComponentModel;

    /// <summary>
    /// The change cost code archive status.
    /// </summary>
    public enum ChangeCostCodeArchiveStatus
    {
        /// <summary>
        /// Action was successful.
        /// </summary>
        [Description("Success")]
        Success = 0,

        /// <summary>
        /// The Cost Code cannot be archived as it is assigned to one or more Signoff Stages.
        /// </summary>
        [Description("This Cost Code cannot be archived as it is assigned to one or more Signoff Stages.")]
        AssignedToSignOffStage = -1,

        /// <summary>
        /// The Cost Code cannot be archived as it is assigned to one or more Employees.
        /// </summary>
        [Description("This Cost Code cannot be archived as it is assigned to one or more Employees.")]
        AssignedToEmployee = -2,

    }
}
