namespace BusinessLogic.GeneralOptions.CodeAllocation
{
    /// <summary>
    /// The code allocation options.
    /// </summary>
    public class CodeAllocationOptions : ICodeAllocationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use cost codes.
        /// </summary>
        public bool UseCostCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the cost code description.
        /// </summary>
        public bool UseCostCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use department codes.
        /// </summary>
        public bool UseDepartmentCodes { get; set; }
  
        /// <summary>
        /// Gets or sets a value indicating whether to display the department description.
        /// </summary>
        public bool UseDepartmentDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use project codes.
        /// </summary>
        public bool UseProjectCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the project code description.
        /// </summary>
        public bool UseProjectCodeDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cost codes are switched on.
        /// </summary>
        public bool CostCodesOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departments are switched on.
        /// </summary>
        public bool DepartmentsOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project codes are swtiched on.
        /// </summary>
        public bool ProjectCodesOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cost code appears in general details.
        /// </summary>
        public bool UseCostCodeOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the department appears in general details.
        /// </summary>
        public bool UseDeptOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the project code appears in general details.
        /// </summary>
        public bool UseProjectCodeOnGenDetails { get; set; }

        /// <summary>
        /// Gets or sets the cost code owner base key.
        /// </summary>
        public string CostCodeOwnerBaseKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto assign allocation.
        /// </summary>
        public bool AutoAssignAllocation { get; set; }
    }
}
