namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// Reponse with list of Item
    /// </summary>
    public class GetExpenseItemsResponse : GetApiResponse<ExpenseItem>
    {
        /// <summary>
        /// Creates a new GetExpenseItemResponse.
        /// </summary>
        public GetExpenseItemsResponse()
        {
            List = new List<ExpenseItem>();
        }
    }

    /// <summary>
    /// Returns the added/ updated expense item
    /// </summary>
    public class ExpenseItemResponse : ApiResponse<ExpenseItem>
    {
    }
}