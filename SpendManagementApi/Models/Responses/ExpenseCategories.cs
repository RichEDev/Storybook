using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;
using Spend_Management;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="ExpenseCategory">ExpenseCategories.</see>
    /// </summary>
    public class GetExpenseCategoriesResponse : GetApiResponse<ExpenseCategory>
    {
        /// <summary>
        /// Creates a new GetExpenseCategoriesResponse.
        /// </summary>
        public GetExpenseCategoriesResponse()
        {
            List = new List<ExpenseCategory>();
        }
    }

    /// <summary>
    /// A response containing one <see cref="ExpenseCategory">ExpenseCategory</see> object.
    /// </summary>
    public class ExpenseCategoryResponse : ApiResponse<ExpenseCategory>
    {
    }
}