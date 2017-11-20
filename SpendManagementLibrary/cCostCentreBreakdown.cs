
namespace SpendManagementLibrary
{
    /// <summary>
    /// The Cost Centre Breakdown 
    /// </summary>
    public class cCostCentreBreakdown
    {
        /// <summary>
        /// Gets the Employee Department
        /// </summary>
        public string nDepartment { get; set; }

        /// <summary>
        /// Gets the Cost code
        /// </summary>
        public string nCostcode { get; set; }

        /// <summary>
        /// Gets the Project code
        /// </summary>
        public string nProjectcode { get; set; }

        /// <summary>
        /// Gets the Percent used
        /// </summary>
        public int nPercentused { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="cCostCentreBreakdown"/> class.
        /// </summary>
        /// <param name="department">
        /// The department.
        /// </param>
        /// <param name="costcode">
        /// The costcode.
        /// </param>
        /// <param name="projectcode">
        /// The projectcode.
        /// </param>
        /// <param name="percentused">
        /// The percentused.
        /// </param>
        public cCostCentreBreakdown(string department, string costcode, string projectcode, int percentused)
        {
            this.nDepartment = department;
            this.nCostcode = costcode;
            this.nProjectcode = projectcode;
            this.nPercentused = percentused;
        }
    }
}
