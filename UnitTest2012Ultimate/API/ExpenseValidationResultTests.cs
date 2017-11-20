namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementApi.Controllers.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Controllers.Expedite.V1;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Types.Expedite;
    using Spend_Management;
    using Utilities;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces;

    [TestClass]
    public class ExpenseValidationResultTests
    {
        #region Properties

        private ControllerFactory<ExpenseValidationResult> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestExpenseValidationManager _expenseValidation;
        private static ExpenseValidationResultsV1Controller _resultsController;

        #endregion Properties

        #region Init Methods

        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            _actionContext = new TestActionContext();
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// The my class cleanup.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _expenseValidation = new TestExpenseValidationManager(TestExpenseValidationManager.TestAccountId);
            _actionContext.SetExpenseValidation(_expenseValidation);
            var repository = RepositoryFactory.GetRepository<ExpenseValidationResult>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<ExpenseValidationResult>(repository);
            _resultsController = (ExpenseValidationResultsV1Controller)_controllerFactory.GetController();
        }

        #endregion Init Methods

        #region GetExpenseValidationResults

        [TestMethod]
        [TestCategory("GetExpenseValidationResult")]
        public void ExpenseValidationResultController_GetForExpenseItemExpenseValidationResult_BadExpenseIdFails()
        {
            var expense = cExpenseItemObject.Template(1, 1, TestExpenseValidationManager.ExpenseItemIdA);
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getExpenseItemById(It.IsAny<int>())).Returns(expense);
            _actionContext.SetClaimsMock(claimsMock);

            var message = ExceptionAssert.ThrowsException<InvalidDataException>(() => _resultsController.GetForExpenseItem(999));
            Assert.AreEqual(message, TestExpenseValidationManager.InvalidExpenseIdError.Message);
        }

        #endregion GetExpenseValidationResults

        #region DeleteExpenseValidationResult

        [TestMethod]
        [TestCategory("DeleteExpenseValidationResult")]
        public void ExpenseValidationResultController_Delete_CorrectExpenseValidationResultGetsDeleted()
        {
            var expenseA = cExpenseItemObject.Template(1, 1, TestExpenseValidationManager.ExpenseItemIdA, validationProgress: SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationProgress.InProgress);
            var expenseB = cExpenseItemObject.Template(1, 1, TestExpenseValidationManager.ExpenseItemIdB, validationProgress: SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationProgress.InProgress);
            var expenseList = new SortedList<int, cExpenseItem> { { TestExpenseValidationManager.ExpenseItemIdA, expenseA }, { TestExpenseValidationManager.ExpenseItemIdB, expenseB } };
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getClaimById(It.IsAny<int>())).Returns(cClaimObject.Template());
            claimsMock.Setup(c => c.getExpenseItemById(It.IsAny<int>())).Returns(expenseA);
            claimsMock.Setup(c => c.getExpenseItemsFromDB(It.IsAny<int?>(), It.IsAny<int?>())).Returns(expenseList);
            claimsMock.Setup(c => c.UpdateClaimHistory(It.IsAny<cClaim>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            claimsMock.Setup(c => c.ReturnExpenses(It.IsAny<cClaim>(), It.IsAny<List<int>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<IDBConnection>(), true)).Returns(true);
            _actionContext.SetClaimsMock(claimsMock);

            var count = TestExpenseValidationManager.Results.Count;
            var targetResult = TestExpenseValidationManager.Results.Last();
            targetResult.ExpenseItemId = TestExpenseValidationManager.ExpenseItemIdA;
            _resultsController.Delete(targetResult.Id);
            Assert.AreEqual(TestExpenseValidationManager.Results.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteExpenseValidationResult")]
        public void ExpenseValidationResultController_Delete_FailsOnBadExpenseValidationResult()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _resultsController.Delete(999));
        }

        #endregion DeleteExpenseValidationResult

        #region Utils

        private static ExpenseValidationResult CreateExpenseValidationResult()
        {
            return new ExpenseValidationResult
            {
                Id = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                AccountId = GlobalTestVariables.AccountId,
                Comments = "New item comments",
                VATStatus = ExpenseValidationStatus.Pass,
                CriterionId = TestExpenseValidationManager.Criteria.Last().Id,
                Timestamp = DateTime.UtcNow,
                ExpenseItemId = TestExpenseValidationManager.ExpenseItemIdA
            };
        }

        #endregion Utils
    }
}
