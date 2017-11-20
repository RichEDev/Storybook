namespace UnitTest2012Ultimate.APIRequestTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Requests;

    /// <summary>
    /// Tests the <see cref="FindCurrencyExchangeRateRequest">FindCurrencyExchangeRateRequest </see> validation criteria
    /// </summary>
    [TestClass]
    public class FindCurrencyExchangeRateRequests
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Tests the request type <see cref="FindCurrencyExchangeRateRequest">FindCurrencyExchangeRateRequest </see> to ensure validation captures erroneous request data as expected
        /// </summary>
        [TestMethod]
        [TestCategory(testCategory: "FindExchangeRateRequest")]
        public void FindExchangeRate_RequestValidation()
        {
            foreach (var testCase in ValidationTestCases())
            {
                FindCurrencyExchangeRateRequest currencyExchangeRateRequest = GenerateFindCurrencyExchangeRateRequest();
                currencyExchangeRateRequest.GetType().GetProperty(testCase.PropertyName).SetValue(currencyExchangeRateRequest, testCase.InvalidValue);

                var validationResults = new List<ValidationResult>();
                bool outcome = Validator.TryValidateObject(currencyExchangeRateRequest, new ValidationContext(currencyExchangeRateRequest), validationResults, true);

                string assertFailMessage = string.Format("Validation should have failed for {0} with the value {1}.", testCase.PropertyName, testCase.InvalidValue);

                //Validate Fails
                Assert.AreEqual(outcome, false, assertFailMessage);
    
                foreach (var result in validationResults)
                {
                    //Test that validation results failed validation the on the correct property. 
                    Assert.IsNotNull(result.MemberNames.Contains(testCase.PropertyName), assertFailMessage);
                }
            }
        }

        /// <summary>
        /// Tests a correct request passes validation
        /// </summary>
        [TestMethod]
        [TestCategory(testCategory: "FindExchangeRateRequest")]
        public void FindExchangeRate_ValidRequest()
        {
            FindCurrencyExchangeRateRequest currencyExchangeRateRequest = GenerateFindCurrencyExchangeRateRequest();
            var validationResults = new List<ValidationResult>();
            bool outcome = Validator.TryValidateObject(currencyExchangeRateRequest, new ValidationContext(currencyExchangeRateRequest), validationResults, true);
            Assert.IsTrue(outcome, "Validation failed for the request.");
        }

        private static FindCurrencyExchangeRateRequest GenerateFindCurrencyExchangeRateRequest()
        {
            var currencies = new cCurrencies(GlobalTestVariables.AccountId, null);
            var fromCurrency = currencies.getCurrencyByAlphaCode("GBP");
            var toCurrency = currencies.getCurrencyByAlphaCode("EUR");

            var currencyExchangeRateRequest = new FindCurrencyExchangeRateRequest
            {
                FromCurrencyId = fromCurrency.currencyid,
                ToCurrencyId = toCurrency.currencyid,
                DateTimeOfRate = DateTime.Today
            };

            return currencyExchangeRateRequest;
        }

        private static IEnumerable<TestCase> ValidationTestCases()
        {
            const string propertyFromCurrencyId = "FromCurrencyId";
            const string propertyToCurrencyId = "ToCurrencyId";
            const string propertyDateTimeOfRate = "DateTimeOfRate";

            yield return new TestCase()
            {
                InvalidValue = -1,
                PropertyName = propertyFromCurrencyId
            };
            yield return new TestCase()
            {
                InvalidValue = -1,
                PropertyName = propertyToCurrencyId
            };
            yield return new TestCase()
            {
                InvalidValue = DateTime.Today.AddDays(1),
                PropertyName = propertyDateTimeOfRate
            };
            yield return new TestCase()
            {
                InvalidValue = SqlDateTime.MinValue.Value.AddDays(-1),
                PropertyName = propertyDateTimeOfRate
            };
        }
    }
}
