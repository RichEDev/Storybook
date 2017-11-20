namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Utilities;
    using SpendManagementLibrary;
    using Spend_Management;
    /// <summary>
    /// Manages data access for the ExpenseCategory
    /// </summary>
    internal class ExpenseCategoryRepository : BaseRepository<ExpenseCategory>
    {
        private cCategories _data;
        
        /// <summary>
        /// Creates a new ExpenseCategoryRespository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        public ExpenseCategoryRepository(ICurrentUser user)
            : base(user, x => x.Id, x => x.Label)
        {
            _data = ActionContext.Categories;
        }

        /// <summary>
        /// Gets all Expense Categories.
        /// </summary>
        /// <returns></returns>
        public override IList<ExpenseCategory> GetAll()
        {
            return _data.CachedList().Select(x => new ExpenseCategory().From(x, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets an Expense Category by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>An Expense Category.</returns>
        public override ExpenseCategory Get(int id)
        {
            _data = ActionContext.Categories;
            var item = _data.FindById(id);
            if (item == null)
            {
                return null;
            }
            return new ExpenseCategory().From(item, ActionContext);
        }

        /// <summary>
        /// Adds an ExpenseCategory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added item.</returns>
        public override ExpenseCategory Add(ExpenseCategory item)
        {
            item = base.Add(item);
            return Save(item, true);
        }

        /// <summary>
        /// Updates an ExpenseCategory.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>An updated item.</returns>
        public override ExpenseCategory Update(ExpenseCategory item)
        {
            item = base.Update(item);
            return Save(item, false);
        }

        /// <summary>
        /// Deletes an ExpenseCategory.
        /// </summary>
        /// <param name="id">the Id of the expense category to delete.</param>
        /// <returns>The deleted ExpenseCategory.</returns>
        public override ExpenseCategory Delete(int id)
        {
            base.Delete(id);

            var result = _data.deleteCategory(id, User);

            switch (result)
            {
                case 0:
                    return Get(result);
                case 1:
                    throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorCategoryInvalidLinkedToSubcat);
                case -10:
                    throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorDeleteUnsuccessfulMessage + ": ExpenseItem.");
                default: 
                    throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorGeneralError);
            }
        }

        /// <summary>
        /// Saves an Expense Category - called from either add or update.
        /// </summary>
        /// <param name="item">The item to save</param>
        /// <param name="addNotUpdate">Whether to add or update</param>
        /// <returns></returns>
        private ExpenseCategory Save(ExpenseCategory item, bool addNotUpdate)
        {
            int id;
            if (addNotUpdate)
            {
                id = _data.addCategory(item.Label, item.Description, User.EmployeeID);
            }
            else
            {
                id = _data.updateCategory(item.Id, item.Label, item.Description, User.EmployeeID);
                if (id == 0) id = item.Id;
            }
            
            if (id < 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            if (id == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorInvalidNameAlreadyExists);
            }

            return Get(id);
        }

        /// <summary>
        /// Gets a List of Expense Categories for an employee
        /// </summary>
        /// <param name="employeeId">Employee id</param>
        /// <returns>A list of <see cref="ListItemData">ListItemData</see></returns>
        public List<ListItemData> GetEmployeeExpenseCategories(int employeeId = 0)
        {
            var categories = ActionContext.Categories;
            var subCategories = ActionContext.SubCategories;

            List<SubcatItemRoleBasic> employeeItemRoles = subCategories.GetSubCatsByEmployeeItemRoles(employeeId > 0 ? employeeId : User.EmployeeID);
            SortedList<string, int> expenseCategories = categories.GetExpenseCategoriesFromItemRoles(employeeItemRoles);

            return  expenseCategories.Select(expenseCategory => new ListItemData(expenseCategory.Value, expenseCategory.Key)).ToList();
        }

        /// <summary>
        /// Gets a List of Expense Categories and Sub categories for an employee
        /// </summary>
        /// <returns>A list of <see cref="EmployeeCategoriesAndSubCategoriesResponse">EmployeeCategoriesAndSubCategoriesResponse</see></returns>
        public EmployeeCategoriesAndSubCategoriesResponse GetEmployeeCategoriesAndSubCategories(EmployeeCategoriesAndSubCategoriesResponse response)
        {
            response.Categories = this.GetEmployeeExpenseCategories();

            List<SubcatItemRoleBasic> basicSubCats = ActionContext.SubCategories.GetSubCatsByEmployeeItemRoles(User.EmployeeID);
            var expenseSubCategoryRepository = new ExpenseSubCategoryRepository(this.User, this.ActionContext);

            List<ExpenseSubCategory> subcats = new List<ExpenseSubCategory>();

            foreach (SubcatItemRoleBasic basicSubCat in basicSubCats.DistinctBy(c => c.SubcatId))
            {
                var subcat = expenseSubCategoryRepository.Get(basicSubCat.SubcatId);
                if (basicSubCat.StartDate != null)
                {
                    subcat.StartDate = basicSubCat.StartDate;
                }

                if (basicSubCat.EndDate != null)
                {
                    subcat.EndDate = basicSubCat.EndDate;
                }
                subcats.Add(subcat);
            }

            response.SubCategories = subcats;

            return response;
        }

        /// <summary>
        /// Gets a List of Expense Categories and Sub categories for an employee
        /// </summary>
        /// <param name="response">EmployeeCategoriesAndSubCategoriesResponse</param>
        /// <param name="expenseId">Expense id</param>
        /// <param name="employeeId">Employee id</param>
        /// <returns>A list of <see cref="EmployeeCategoriesAndSubCategoriesResponse">EmployeeCategoriesAndSubCategoriesResponse</see></returns>
        public EmployeeCategoriesAndSubCategoriesResponse GetCategoriesAndSubCategoriesForEmployee(EmployeeCategoriesAndSubCategoriesResponse response,int expenseId, int employeeId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            response.Categories = this.GetEmployeeExpenseCategories(employeeId);
            var basicSubCats = this.ActionContext.SubCategories.GetSubCatsByEmployeeItemRoles(employeeId);
            var expenseSubCategoryRepository = new ExpenseSubCategoryRepository(this.User, this.ActionContext);

            var subcats = new List<ExpenseSubCategory>();

            foreach (var basicSubCat in basicSubCats.DistinctBy(c => c.SubcatId))
            {
                var subcat = expenseSubCategoryRepository.GetSubCat(basicSubCat.SubcatId, employeeId);
                if (basicSubCat.StartDate != null)
                {
                    subcat.StartDate = basicSubCat.StartDate;
                }

                if (basicSubCat.EndDate != null)
                {
                    subcat.EndDate = basicSubCat.EndDate;
                }
                subcats.Add(subcat);
            }

            response.SubCategories = subcats;

            return response;
        }

    }
}