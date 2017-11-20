
namespace SpendManagementApi.Models.Types.MyDetails
{
    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary;

    /// <summary>
    /// The Cost Centre Breakdown 
    /// </summary>
    public class CostCentreBreakdown : IApiFrontForDbObject<cCostCentreBreakdown, CostCentreBreakdown>
    {
        /// <summary>
        /// Gets the Employee Department
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets the Cost code
        /// </summary>
        public string Costcode { get; set; }

        /// <summary>
        /// Gets the Project code
        /// </summary>
        public string Projectcode { get; set; }

        /// <summary>
        /// Gets the Percent used
        /// </summary>
        public int Percentused { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType">The DAL type.</param>
        /// <param name="actionContext">The IActionContext</param>
        /// <returns>This, the API type.</returns>
        public CostCentreBreakdown From(cCostCentreBreakdown dbType, IActionContext actionContext)
        {
            this.Department = dbType.nDepartment;
            this.Costcode = dbType.nCostcode;
            this.Projectcode = dbType.nProjectcode;
            this.Percentused = dbType.nPercentused;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cCostCentreBreakdown To(IActionContext actionContext)
        {
           return new cCostCentreBreakdown(Department, Costcode, Projectcode, Percentused);
        }
    }
}