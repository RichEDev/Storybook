namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing an employees expense categories and sub categories
    /// </summary>
    public class EmployeeCategoriesAndSubCategoriesResponse : ApiResponse
    {
        /// <summary>
        /// A List of <see cref="ListItemData">ListItemData. </see> Containing the categoryId and Name
        /// </summary>
        public List<ListItemData> Categories { get; set; }

        /// <summary>
        /// A List of <see cref="ExpenseSubCategory">ExpenseSubCategory</see>/>
        /// </summary>
        public List<ExpenseSubCategory> SubCategories { get; set; }
    }
}