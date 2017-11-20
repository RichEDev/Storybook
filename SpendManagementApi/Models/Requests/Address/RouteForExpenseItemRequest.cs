namespace SpendManagementApi.Models.Requests.Address
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// A request to get the route and mapping details for an expense item
    /// </summary>
    public class RouteForExpenseItemRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the expense Id.
        /// </summary>
        public int ExpenseId { get; set; }
    }
}