using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace tempMobileUnitTests
{
    internal class cExpenseCategoryObject
    {
        public static cCategory New(cCategory category)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCategories clsCats = new cCategories(currentUser.AccountID);

            int categoryId = clsCats.addCategory(category.category, category.description, currentUser.EmployeeID);

            clsCats = new cCategories(currentUser.AccountID);
            return clsCats.FindById(categoryId);
        }

        public static cCategory Template(int categoryId = 0, string category = default(string), string description = default(string))
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            category = (category == default(string) ? "Unit Test Category" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString() : category);
            description = (description == default(string) ? "Category for Unit Tests" : description);

            cCategory retCategory = new cCategory(categoryId, category, description, DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID);

            return retCategory;
        }

        public static void TearDown(int categoryId)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCategories clsCats = new cCategories(currentUser.AccountID);

            clsCats.deleteCategory(categoryId);
        }
    }
}
