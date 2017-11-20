namespace UnitTest2012Ultimate.APIRequestTests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Types;
    using API.Utilities;

    /// <summary>
    /// This is a test class for validating <see cref="ExpenseCategory"> ExpenseCategory </see> requests
    /// </summary>
    [TestClass]
    public class ExpenseCategoryRequests
    {
        private static TestActionContext _actionContext;
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            _actionContext = new TestActionContext();
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Tests the request type <see cref="ExpenseCategory">ExpenseCategory </see> to ensure validation captures erroneous request data as expected
        /// </summary>
        [TestMethod]
        [TestCategory("ExpenseCategoryRequests")]
        public void ExpenseCategory_RequestValidation()
        {
            foreach (var testCase in ValidationTestCases())
            {
                var expenseCategory = GenerateExpenseCategory();
                expenseCategory.GetType().GetProperty(testCase.PropertyName).SetValue(expenseCategory, testCase.InvalidValue);

                var validationResults = new List<ValidationResult>();
                bool outcome = Validator.TryValidateObject(expenseCategory, new ValidationContext(expenseCategory), validationResults, true);

                string assertFailMessage = string.Format("Validation should have failed for {0} with the value {1}", testCase.PropertyName, testCase.InvalidValue);

                //Validate Fails
                Assert.AreEqual(outcome, false, assertFailMessage);
                //One failure in resultset
                Assert.AreEqual(validationResults.Count, 1, assertFailMessage);
                //validation failed on expected property/value
                Assert.IsNotNull(validationResults.FirstOrDefault(t => t.MemberNames.Contains(testCase.PropertyName)), assertFailMessage);
            }
        }

        private static ExpenseCategory GenerateExpenseCategory(string label = "Expense Category Label", string description = "Expense Category Description")
        {
            var expenseCat = cExpenseCategoryObject.Template(0, label, description);
            return new ExpenseCategory().From(expenseCat, _actionContext);
        }


        private static IEnumerable<TestCase> ValidationTestCases()
        {
            yield return new TestCase()
            {
                InvalidValue = -1,
                PropertyName = "Id"          
            };
            yield return new TestCase()
            {
                InvalidValue = string.Empty,
                PropertyName = "Label"
            };
            yield return new TestCase()
            {
                InvalidValue = new string('*', 51),
                PropertyName = "Label"
            };
            yield return new TestCase()
            {
                InvalidValue = string.Empty,
                PropertyName = "Description"
            };
            yield return new TestCase()
            {
                InvalidValue = new string('*', 40001),
                PropertyName = "Description"
            };
        }
    }
}

