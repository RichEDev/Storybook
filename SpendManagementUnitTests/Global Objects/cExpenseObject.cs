using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cExpenseObject
    {
        /// <summary>
        /// Create a global static object for the expense item
        /// </summary>
        /// <returns></returns>
        public static cExpenseItem CreateExpenseItem()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = clsEmps.GetEmployeeById(cGlobalVariables.EmployeeID);
            cExpenseItems clsExpenseItems = new cExpenseItems(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            cSubcats subcats = new cSubcats(cGlobalVariables.AccountID);
            cSubcat subcat = subcats.getSubcatById(cGlobalVariables.SubcatID);

            cExpenseItem item = new cExpenseItem(cGlobalVariables.AccountID, 0, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, cGlobalVariables.SubcatID, DateTime.Now, 0, 0, 0, false, false, "", cGlobalVariables.ClaimID, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, new SortedList<int, cJourneyStep>(), MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), 0, subcat.HomeToLocationType);
            int expenseid = clsExpenseItems.addItem(item, cGlobalVariables.EmployeeID);
            cGlobalVariables.ExpenseID = expenseid;
            item = new cExpenseItem(cGlobalVariables.AccountID, expenseid, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, cGlobalVariables.SubcatID, DateTime.Now, 0, 0, 0, false, false, "", cGlobalVariables.ClaimID, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, new SortedList<int, cJourneyStep>(), MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), 0, subcat.HomeToLocationType);
            return item;
        }

        /// <summary>
        /// Create a global static object for the expense item with an ESR assignment associated
        /// </summary>
        /// <returns></returns>
        public static cExpenseItem CreateExpenseItemWithESRAssignment()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = clsEmps.GetEmployeeById(cGlobalVariables.EmployeeID);
            cExpenseItems clsExpenseItems = new cExpenseItems(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            cSubcats subcats = new cSubcats(cGlobalVariables.AccountID);
            cSubcat subcat = subcats.getSubcatById(cGlobalVariables.SubcatID);

            cExpenseItem item = new cExpenseItem(cGlobalVariables.AccountID, 0, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, cGlobalVariables.SubcatID, DateTime.Now, 0, 0, 0, false, false, "", cGlobalVariables.ClaimID, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, new SortedList<int, cJourneyStep>(), MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), cGlobalVariables.ESRAssignmentID, subcat.HomeToLocationType);
            int expenseid = clsExpenseItems.addItem(item, cGlobalVariables.EmployeeID);
            cGlobalVariables.ExpenseID = expenseid;
            item = new cExpenseItem(cGlobalVariables.AccountID, expenseid, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, cGlobalVariables.SubcatID, DateTime.Now, 0, 0, 0, false, false, "", cGlobalVariables.ClaimID, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, new SortedList<int, cJourneyStep>(), MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), cGlobalVariables.ESRAssignmentID, subcat.HomeToLocationType);
            return item;
        }

        /// <summary>
        /// Create a global static mileage object for the expense item
        /// </summary>
        /// <returns></returns>
        public static cExpenseItem CreateMileageExpenseItem()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = clsEmps.GetEmployeeById(cGlobalVariables.EmployeeID);
            cExpenseItems clsExpenseItems = new cExpenseItems(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            cSubcat subcat = cSubcatObject.CreateMileageSubcat();
            SortedList<int, cJourneyStep> steps = new SortedList<int, cJourneyStep>();
            cClaim claim = cClaimObject.CreateCurrentClaim();
            cGlobalVariables.ClaimID = claim.claimid;

            cExpenseItem item = new cExpenseItem(cGlobalVariables.AccountID, 0, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, subcat.subcatid, DateTime.Now, 0, 0, 0, false, false, "", claim.claimid, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, new SortedList<int, cJourneyStep>(), MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), 0, subcat.HomeToLocationType);
            int expenseid = clsExpenseItems.addItem(item, cGlobalVariables.EmployeeID);
            cGlobalVariables.ExpenseID = expenseid;
            steps.Add(0, CreateJourneyStepOject(expenseid));
            item = new cExpenseItem(cGlobalVariables.AccountID, expenseid, ItemType.Cash, 0, 0, "", true, (decimal)2.98, (decimal)0.52, (decimal)3.5, subcat.subcatid, DateTime.Now, 0, 0, 0, false, false, "", claim.claimid, 0, 0, cGlobalVariables.CurrencyID, "", 0, emp.primarycountry, 0, (decimal)3.5, 1, false, 0, false, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), 0, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", emp.primarycurrency, emp.primarycurrency, 1, (decimal)3.5, new SortedList<int, object>(), 0, false, false, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "", 0, steps, MileageUOM.Mile, new Dictionary<FlagType, cFlaggedItem>(), new List<cDepCostItem>(), 0, subcat.HomeToLocationType);
            return item;
        }

        /// <summary>
        /// Delete expense item from the database
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteExpenseItem(int ID)
        {
            cClaims clsClaims = new cClaims(cGlobalVariables.AccountID);
            cClaim claim = clsClaims.getClaimById(cGlobalVariables.ClaimID);
            cExpenseItems clsExpItems = new cExpenseItems(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            cExpenseItem expItem = claim.getExpenseItemById(ID);
            clsClaims.deleteExpense(claim, expItem, false);
        }


        /// <summary>
        /// Create a journey step object
        /// </summary>
        /// <returns></returns>
        public static cJourneyStep CreateJourneyStepOject(int expenseID)
        {
            cJourneyStep step = new cJourneyStep(expenseID, null, null, 10, 0, 0, 1, false, "", 10, false);
            return step;
        }
    }
}
