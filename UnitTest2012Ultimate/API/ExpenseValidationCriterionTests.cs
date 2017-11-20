using System;
using SpendManagementApi.Controllers.Expedite.V1;

namespace UnitTest2012Ultimate.API
{
    using System.IO;
    using System.Linq;
    using SpendManagementApi.Controllers.Expedite;
    using SpendManagementApi.Controllers.Expedite.V1;
    using SpendManagementApi.Models.Responses.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;

    [TestClass]
    public class ExpenseValidationCriterionTests
    {
        #region Properties

        private ControllerFactory<ExpenseValidationCriterion> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestExpenseValidationManager _expenseValidation;
        private static ExpenseValidationCriteriaV1Controller _criterionsController;

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
            var repository = RepositoryFactory.GetRepository<ExpenseValidationCriterion>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<ExpenseValidationCriterion>(repository);
            _criterionsController = (ExpenseValidationCriteriaV1Controller)_controllerFactory.GetController();
        }

        #endregion Init Methods

        #region AddExpenseValidationCriterion

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_AddExpenseValidationCriterion_CorrectExpenseValidationCriterionGetsAdded()
        {
            var count = TestExpenseValidationManager.Criteria.Count;
            var criterion = CreateExpenseValidationCriterion();
            _controllerFactory.Post<ExpenseValidationCriterionResponse>(criterion);
            Assert.AreEqual(count + 1, TestExpenseValidationManager.Criteria.Count);
        }

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_AddExpenseValidationCriterion_PopulatedIdFails()
        {
            var criterion = CreateExpenseValidationCriterion();
            criterion.Id = 12;
            var message = ExceptionAssert.ThrowsException<ApiException>(() => _controllerFactory.Post<ExpenseValidationCriterionResponse>(criterion));
        }

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_AddExpenseValidationCriterion_BadFieldIdFails()
        {
            var criterion = CreateExpenseValidationCriterion();
            criterion.FieldId = new Guid();
            var message = ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Post<ExpenseValidationCriterionResponse>(criterion));
            Assert.AreEqual(message, TestExpenseValidationManager.InvalidFieldIdError.Message);
        }

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_AddExpenseValidationCriterion_BadAccountIdFails()
        {
            var criterion = CreateExpenseValidationCriterion();
            criterion.AccountId = 999;
            var message = ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Post<ExpenseValidationCriterionResponse>(criterion));
            Assert.AreEqual(message, TestExpenseValidationManager.InvalidAccountIdError.Message);
        }
        
        #endregion AddExpenseValidationCriterion

        #region EditExpenseValidationCriterion

        [TestMethod]
        [TestCategory("EditExpenseValidationCriterion")]
        public void CriteriaController_EditExpenseValidationCriterion_CorrectCriterionGetsEdited()
        {
            var targetCriterion = TestExpenseValidationManager.Criteria.First();
            var criterion = CreateExpenseValidationCriterion();
            
            // make sure we use the ID of the target and change something.
            criterion.Id = targetCriterion.Id;
            criterion.Requirements = TestExpenseValidationManager.Criteria.Last().Requirements;

            _controllerFactory.Put<ExpenseValidationCriterionResponse>(criterion);
            Assert.AreEqual(targetCriterion.Id, criterion.Id);
        }

        [TestMethod]
        [TestCategory("EditExpenseValidationCriterion")]
        public void CriteriaController_EditExpenseValidationCriterion_ShouldErrorWhenInvalidId()
        {
            var criterion = CreateExpenseValidationCriterion(); // Id is 0!
            ExceptionAssert.ThrowsException<ApiException>(() => _controllerFactory.Put<ExpenseValidationCriterionResponse>(criterion));

            criterion.Id = 9999; // id is not in Criteria
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<ExpenseValidationCriterionResponse>(criterion));
        }

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_EditExpenseValidationCriterion_BadFieldIdFails()
        {
            var criterion = CreateExpenseValidationCriterion();
            criterion.Id = 1;
            criterion.FieldId = new Guid();
            var message = ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<ExpenseValidationCriterionResponse>(criterion));
            Assert.AreEqual(message, TestExpenseValidationManager.InvalidFieldIdError.Message);
        }

        [TestMethod]
        [TestCategory("AddExpenseValidationCriterion")]
        public void ExpenseValidationCriterionController_EditExpenseValidationCriterion_BadAccountIdFails()
        {
            var criterion = CreateExpenseValidationCriterion();
            criterion.Id = 1;
            criterion.AccountId = 999;
            var message = ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<ExpenseValidationCriterionResponse>(criterion));
            Assert.AreEqual(message, TestExpenseValidationManager.InvalidAccountIdError.Message);
        }

        #endregion EditExpenseValidationCriterion

        #region DeleteExpenseValidationCriterion

        [TestMethod]
        [TestCategory("DeleteExpenseValidationCriterion")]
        public void CriteriaController_Delete_CorrectExpenseValidationCriterionGetsDeleted()
        {
            var count = TestExpenseValidationManager.Criteria.Count;
            var targetCriterion = TestExpenseValidationManager.Criteria.First();
            _criterionsController.Delete(targetCriterion.Id);
            Assert.AreEqual(TestExpenseValidationManager.Criteria.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteExpenseValidationCriterion")]
        public void CriteriaController_Delete_FailsOnBadExpenseValidationCriterion()
        {
            ExceptionAssert.ThrowsException<ApiException>(() => _criterionsController.Delete(999));
        }

        [TestMethod]
        [TestCategory("DeleteExpenseValidationCriterion")]
        public void CriteriaController_Delete_FailsDueToBeingInUse()
        {
            // make sure the criterion being deleted is in use.
            var type = TestExpenseValidationManager.Criteria.Last();
            var errorMessage = ExceptionAssert.ThrowsException<InvalidDataException>(() => _criterionsController.Delete(type.Id));
            Assert.AreEqual(errorMessage, TestExpenseValidationManager.DeleteFailedDependencies.Message);
        }

        #endregion DeleteExpenseValidationCriterion

        #region Utils

        private static ExpenseValidationCriterion CreateExpenseValidationCriterion()
        {
            return new ExpenseValidationCriterion
            {
                Id = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                AccountId = GlobalTestVariables.AccountId,
                FieldId = null,
                Requirements = "Requirements",
                Enabled = true,
                FraudulentIfFailsVAT = false
            };
        }

        #endregion Utils
    }
}
