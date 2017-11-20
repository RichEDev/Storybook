namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Costcode breakdown settings from general options
    /// </summary>
    public class CostcodeBreakdownSettings
    {
        /// <summary>
        /// Is department assigned to item
        /// </summary>
        public bool UseDepartmentCodes { get; set; }

        /// <summary>
        /// Is department visible for claimant
        /// </summary>
        public bool DepartmentsOn { get; set; }

        /// <summary>
        /// Is description visible for department
        /// </summary>
        public bool UseDepartmentCodeDescription { get; set; }

        /// <summary>
        /// Whether show department in general detail section or not
        /// </summary>
        public bool UseDeptOnGenDetails { get; set; }

        /// <summary>
        /// Is costcode assigned to item
        /// </summary>
        public bool UseCostCodes { get; set; }

        /// <summary>
        /// Is costcode visible for claimant
        /// </summary>
        public bool CostCodesOn { get; set; }

        /// <summary>
        /// Is description visible for costcode
        /// </summary>
        public bool UseCostCodeDescription { get; set; }

        /// <summary>
        /// Whether show costcode in general detail section or not
        /// </summary>
        public bool UseCostCodeOnGenDetails { get; set; }

        /// <summary>
        /// Is projectcode assigned to item
        /// </summary>
        public bool UseProjectCodes { get; set; }

        /// <summary>
        /// Is projectcode visible for claimant
        /// </summary>
        public bool ProjectCodesOn { get; set; }

        /// <summary>
        /// Is description visible for projectcode
        /// </summary>
        public bool UseProjectCodeDescription { get; set; }

        /// <summary>
        /// Whether show projectcode in general detail section or not
        /// </summary>
        public bool UseProjectCodeOnGenDetails { get; set; }

        /// <summary>
        /// Whether to use default costcode allocation or not
        /// </summary>
        public bool UseDefaultAllocation { get; set; }
    }
}