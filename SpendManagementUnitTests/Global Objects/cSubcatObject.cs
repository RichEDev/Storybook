using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cSubcatObject
    {
        /// <summary>
        /// Create a global mileage subcat
        /// </summary>
        /// <returns></returns>
        public static cSubcat CreateDummySubcat()
        {
            //Need the category associated
            cCategory category = CreateExpenseCategory();

            cSubcats clsSubcats = new cSubcats(cGlobalVariables.AccountID);
            
            int tempSubcatID = clsSubcats.saveSubcat(new cSubcat(0, category.categoryid, "Unit Test dummy Item", "Unit Test dummy Item", false, false, false, false, false, false, 0, "UnitTest01", false, false, 0, false, false, CalculationType.NormalItem, false, false, "Used for Unit Tests", false, 0, true, false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "Unit Test Normal", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false, HomeToLocationType.None, null, false, null, false));
            cGlobalVariables.SubcatID = tempSubcatID;
            clsSubcats = new cSubcats(cGlobalVariables.AccountID);
            cSubcat subcat = clsSubcats.getSubcatById(tempSubcatID);
            return subcat;
        }


        /// <summary>
        /// Create the global object for the expense category
        /// </summary>
        /// <returns></returns>
        public static cCategory CreateExpenseCategory()
        {
            cCategories clsCats = new cCategories(cGlobalVariables.AccountID);
            int tempCategoryID = clsCats.addCategory("Unit Test Category" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Category for Unit Tests", cGlobalVariables.EmployeeID);
            cGlobalVariables.CategoryID = tempCategoryID;
            clsCats = new cCategories(cGlobalVariables.AccountID);
            cCategory category = clsCats.FindById(tempCategoryID);
            return category;
        }

        /// <summary>
        /// Delete the static subcat object from the database
        /// </summary>
        public static void DeleteSubcat()
        {
            cSubcats clsSubcats = new cSubcats(cGlobalVariables.AccountID);
            clsSubcats.deleteSubcat(cGlobalVariables.SubcatID);
            DeleteExpenseCategory();
        }

        /// <summary>
        /// Delete the static expense category object from the database
        /// </summary>
        public static void DeleteExpenseCategory()
        {
            cCategories clsCats = new cCategories(cGlobalVariables.AccountID);
            clsCats.deleteCategory(cGlobalVariables.CategoryID);
        }



        /// <summary>
        /// Create mileage item category
        /// </summary>
        /// <returns>Subcat for mileage items</returns>
        public static cSubcat CreateMileageSubcat()
        {
            cSubcats clsSubcats = new cSubcats(cGlobalVariables.AccountID);
            cCategory oCategory = CreateExpenseCategory();
            int nSubcatID;

            try
            {
                nSubcatID = clsSubcats.saveSubcat(new cSubcat(0, oCategory.categoryid, "Unit Test Mileage Subcat: " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Unit Test dummy Item", true, false, false, false, false, false, 0, "UnitTest01", false, false, 0, false, false, CalculationType.NormalItem, false, false, "Used for Unit Tests", false, 0, true, false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "Unit Test Normal", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false, HomeToLocationType.None, null, false, null, false));
            }
            catch (Exception e)
            {
                return null;
            }

            cSubcat oSubcat = clsSubcats.getSubcatById(nSubcatID);

            return oSubcat;
        }
    }
}
