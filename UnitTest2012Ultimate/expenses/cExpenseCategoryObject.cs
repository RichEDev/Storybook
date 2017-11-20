using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;

namespace UnitTest2012Ultimate
{
    internal class cExpenseCategoryObject
    {
        public static cCategory New(cCategory category)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCategories clsCats = new cCategories(currentUser.AccountID);

            //int categoryId = clsCats.addCategory(category.category, category.description, currentUser.EmployeeID); // can't use at present as uses HttpContext
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
            
            if (clsCats.getCategoryByName(category.category) != null)
            {
                return clsCats.getCategoryByName(category.category);
            }

            DateTime createdon = DateTime.Now.ToUniversalTime();

            expdata.addParameter("@categoryid", 0);
            expdata.addParameter("@category", category.category);
            if (category.description.Length > 3999)
            {
                expdata.addParameter("@description", category.description.Substring(0, 3999));
            }
            else
            {
                expdata.addParameter("@description", category.description);
            }
            expdata.addParameter("@date", createdon);

            expdata.addParameter("@userid", currentUser.EmployeeID);
            int categoryid = expdata.executeSaveCommand("saveCategory");

            Assert.IsTrue(categoryid > 0, "cExpenseCategoryObject.New failure : categoryId = " + categoryid.ToString());

            clsCats.refreshCache();
            clsCats = new cCategories(currentUser.AccountID);
            return clsCats.FindById(categoryid);
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
            
            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
            db.addParameter("@categoryid", categoryId);
            int retCode = db.executeSaveCommand("deleteCategory"); // call stored proc because class method uses HttpContext and fails
            switch(retCode)
            {
                case 1:
                    Assert.Fail("cExpenseCategoryObject.TearDown failure : Subcat still defined that references the Expense Category");
                    break;
                case -10:
                    Assert.Fail("cExpenseCategoryObject.TearDown failure : checkReferencedBy stored proc preventing TearDown");
                    break;
            }
            cCategories clsCats = new cCategories(currentUser.AccountID);
            clsCats.refreshCache();
        }
    }
}
