using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="BudgetHolder">BudgetHolder</see>s.
    /// </summary>
    public class GetBudgetHoldersResponse : GetApiResponse<BudgetHolder>
    {
        /// <summary>
        /// Creates a new GetBudgetHoldersResponse.
        /// </summary>
        public GetBudgetHoldersResponse()
        {
            List = new List<BudgetHolder>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="BudgetHolder">BudgetHolder</see>.
    /// </summary>
    public class BudgetHolderResponse : ApiResponse<BudgetHolder>
    {

    }
}
