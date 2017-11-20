namespace SpendManagementApi.Models.Types.Employees
{
    /// <summary>
    /// The cost code breakdown with label class.
    /// </summary>
    public class CostCodeBreakdownWithLabelData
    {

        /// <summary>
        /// Gets or sets the department for the element to which the percentage is allocated 
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department description for the element to which the percentage is allocated 
        /// </summary>
        public string DepartmentDecription { get; set; }

        /// <summary>
        /// Gets or sets the cost code for the element to which the percentage is allocated 
        /// </summary>
        public int? CostCodeId { get; set; }

        /// <summary>
        /// Gets or sets the cost code description for the element to which the percentage is allocated 
        /// </summary>
        public string CostCodeDecription { get; set; }

        /// <summary>
        /// Gets or sets the project code for the element to which the percentage is allocated 
        /// </summary>
        public int? ProjectCodeId { get; set; }

        /// <summary>
        /// Gets or sets the project code description for the element to which the percentage is allocated 
        /// </summary>
        public string ProjectDecription { get; set; }

        /// <summary>
        /// Gets or sets the percentage of allocation
        /// </summary>
        public int Percentage { get; set; }
    }
}