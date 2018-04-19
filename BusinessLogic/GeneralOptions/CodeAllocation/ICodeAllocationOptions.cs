namespace BusinessLogic.GeneralOptions.CodeAllocation
{
    /// <summary>
    /// Defines a <see cref="ICodeAllocationOptions"/> and it's members
    /// </summary>
    public interface ICodeAllocationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use cost codes.
        /// </summary>
        bool UseCostCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the cost code description.
        /// </summary>
        bool UseCostCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use department codes.
        /// </summary>
        bool UseDepartmentCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the department description.
        /// </summary>
        bool UseDepartmentDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use project codes.
        /// </summary>
        bool UseProjectCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the project code description.
        /// </summary>
        bool UseProjectCodeDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cost codes are switched on.
        /// </summary>
        bool CostCodesOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departments are switched on.
        /// </summary>
        bool DepartmentsOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project codes are swtiched on.
        /// </summary>
        bool ProjectCodesOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cost code appears in general details.
        /// </summary>
        bool UseCostCodeOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the department appears in general details.
        /// </summary>
        bool UseDeptOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the project code appears in general details.
        /// </summary>
        bool UseProjectCodeOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets the cost code owner base key.
        /// </summary>
        string CostCodeOwnerBaseKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto assign allocation.
        /// </summary>
        bool AutoAssignAllocation { get; set; }
    }
}
