namespace SpendManagementApi.Models.Types.ClaimableItem
{
    using Spend_Management;
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The claimable item conversion.
    /// </summary>
    public static class ClaimableItemConversion
    {
        /// <summary>
        /// The cast SubcatItemRoleBasic object to ClaimableItem object.
        /// </summary>
        /// <param name="itemRole">
        /// The sub category item role basic.
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="ClaimableItem">ClaimableItem</see>.
        /// </returns>
        internal static TResult Cast<TResult>(
            this SpendManagementLibrary.SubcatItemRoleBasic itemRole,
            ICurrentUser user,
            IActionContext actionContext) where TResult : ClaimableItem, new()
        {
            if (itemRole == null)
            {
                return null;
            }

            var employee = user.Employee;
            var categories = actionContext.Categories;
            var category = categories.FindById(itemRole.CategoryId);

            return new TResult()
                {
                    SubCatId = itemRole.SubcatId,
                    Description = itemRole.Description,
                    ExpenseItemName = itemRole.Subcat,
                    ExpenseCategoryName = category.category,
                    MaximumLimit = itemRole.Maximum,
                    MaximumLimitWithoutReceipt = itemRole.ReceiptMaximum,
                    IsAppear = employee.GetSubCategories().Contains(itemRole.SubcatId),
                    CurrencySymbol = itemRole.CurrencySymbol
                };
        }
    }
}