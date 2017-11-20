namespace UnitTest2012Ultimate.expenses
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementLibrary.Enumerators;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using Spend_Management.expenses.code.Claims;
    using Utilities.DistributedCaching;

    [TestClass]
    public class ExpenseItemTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }


        #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialise()
        {
            new GlobalVariables(GlobalVariables.ApplicationType.Service);
            cSubAccountObject.CreateSubAccount();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, EmployeeWorkAddresses.CacheArea, GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture));
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            cSubAccountObject.DeleteSubAccount();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, EmployeeWorkAddresses.CacheArea, GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture));
        }


        #endregion
        [TestMethod]
        public void ClaimSubmission_ExpenseItemsOutsideTolerance_OnTolerance()
        {
            var expenseItems = new List<cExpenseItem>
                                   {
                                       CreateExpenseItem(1, 1, 1, true, true, 10, null),
                                       CreateExpenseItem(1, 1, 2, true, false, -10, 1),
                                       CreateExpenseItem(1, 1, 2, false, false, (decimal)10.1, 1)
                                   };

            var result = ClaimSubmission.ExpenseItemsOutsideChangeTolerance(expenseItems, 1);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 0);


        }

        [TestMethod]
        public void ClaimSubmission_ExpenseItemsOutsideTolerance_OverTolerance()
        {
            var expenseItems = new List<cExpenseItem>
                                   {
                                       CreateExpenseItem(1, 1, 1, true, true, 10, null),
                                       CreateExpenseItem(1, 1, 2, true, false, -10, 1),
                                       CreateExpenseItem(1, 1, 2, false, false, (decimal)10.2, 1)
                                   };

            var result = ClaimSubmission.ExpenseItemsOutsideChangeTolerance(expenseItems, 1);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 1);


        }


        [TestMethod]
        public void ClaimSubmission_ExpenseItemsOutsideTolerance_ZeroChange()
        {
            var expenseItems = new List<cExpenseItem>
                                   {
                                       CreateExpenseItem(1, 1, 1, true, true, 10, null),
                                       CreateExpenseItem(1, 1, 2, true, false, -10, 1),
                                       CreateExpenseItem(1, 1, 2, false, false, 10, 1)
                                   };

            var result = ClaimSubmission.ExpenseItemsOutsideChangeTolerance(expenseItems, 1);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 0);


        }

        [TestMethod]
        public void ClaimSubmission_ExpenseItemsOutsideTolerance_MultipleItems()
        {
            var expenseItems = new List<cExpenseItem>
                                   {
                                       CreateExpenseItem(1, 1, 1, true, true, 10, null),
                                       CreateExpenseItem(1, 1, 2, true, false, -10, 1),
                                       CreateExpenseItem(1, 1, 3, false, false, (decimal)10.1, 1),
                                       CreateExpenseItem(1, 1, 4, false, false, 20, null),
                                       CreateExpenseItem(1, 1, 5, true, true, 30, null),
                                       CreateExpenseItem(1, 1, 6, true, false, -30, 5),
                                       CreateExpenseItem(1, 1, 7, false, false, (decimal)33.34, 5)
                                   };

            var result = ClaimSubmission.ExpenseItemsOutsideChangeTolerance(expenseItems, 1);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeAccessExpenseItem()
        {
            cClaim claim = null;
            try
            {
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.EmployeeId, submitted: false, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.Pass);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeDoesNotOwnCurrenctClaim()
        {
            cClaim claim = null;
            Employee employee = null;
            try
            {
                employee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                claim = cClaimObject.New(cClaimObject.Template(employeeid: employee.EmployeeID, submitted: false, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.EmployeeDoesNotOwnClaim);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }

                if (employee != null)
                {                   
                    employee.Delete(Moqs.CurrentUser());
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeAccessClaimThatIsSubmitted()
        {
            cClaim claim = null;
            try
            {
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.EmployeeId, submitted: true, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.ClaimHasBeenSubmitted);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeAccessClaimThatIsSubmittedButReturned()
        {
            cClaim claim = null;
            try
            {
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.EmployeeId, submitted: true, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid, returned: true);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.Pass);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeAccessClaimThatIsPrevious()
        {
            cClaim claim = null;
            try
            {
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.EmployeeId, submitted: true, paid: true));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.ClaimHasBeenApproved);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationEmployeeAccessExpenseItemThatIsEdited()
        {

            cClaim claim = null;
            try
            {
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.EmployeeId, submitted: false, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid, edited: true);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ICurrentUser currentUser = Moqs.CurrentUser();
                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.ExpenseItemHasBeenEdited);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationClaimCheckerCanAccessExpenseItem()
        {

            cClaim claim = null;
            try
            {
                ICurrentUser currentUser = Moqs.CurrentUser();
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.AlternativeEmployeeId, submitted: true, paid: false, checkerid: currentUser.EmployeeID));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.Pass);

            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod]
        public void ExpenseItems_ValidationItemCheckerCanAccessExpenseItem()
        {
            cClaim claim = null;
            try
            {            
                ICurrentUser currentUser = Moqs.CurrentUser();
                claim = cClaimObject.New(cClaimObject.Template(employeeid: GlobalTestVariables.AlternativeEmployeeId, submitted: true, paid: false));

                cExpenseItem expenseItem = cExpenseItemObject.Template(1, claim.claimid, itemCheckerId: currentUser.EmployeeID);
                var expenseItems = new cExpenseItems(GlobalTestVariables.AccountId);

                ExpenseItemPermissionResult result = expenseItems.ExpenseItemPermissionCheck(claim.ClaimStage, claim.employeeid, claim.checkerid, expenseItem.returned, expenseItem.itemCheckerId, expenseItem.Edited, false, currentUser);

                Assert.AreEqual(result, ExpenseItemPermissionResult.Pass);

            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        private static cExpenseItem CreateExpenseItem(int subCatId, int claimId, int expenseId, bool edited, bool paid, decimal amountPayable, int? originalExpenseId)
        {
            var expenseItem = cExpenseItemObject.Template(subCatId, claimId, expenseId);
            expenseItem.Edited = edited;
            expenseItem.Paid = paid;
            expenseItem.amountpayable = amountPayable;
            expenseItem.OriginalExpenseId = originalExpenseId;
            return expenseItem;
        }
    }
}
