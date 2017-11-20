namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types.Employees;

    /// <summary>
    /// A response containing a list of <see cref="CostCentreBreakdown">CostCentreBreakdown</see>s.
    /// </summary>
    public class GetExpenseItemCostCodeBreakdownResponse : GetApiResponse<CostCentreBreakdown>
    {
        /// <summary>
        /// Creates a new ist of <see cref="CostCentreBreakdown">CostCentreBreakdown</see>s.
        /// </summary>
        public GetExpenseItemCostCodeBreakdownResponse()
        {
            List = new List<CostCentreBreakdown>();
        }
    }
}