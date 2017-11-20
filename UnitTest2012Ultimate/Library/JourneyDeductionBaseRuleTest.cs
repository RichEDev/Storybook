namespace UnitTest2012Ultimate.Library
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.JourneyDeductionRules;

    /// <summary>
    /// Base JourneyDeductionRule tests
    /// </summary>
    [TestClass]
    public class JourneyDeductionBaseRuleTest
    {
        #region Fields

        /// <summary>
        /// The employee's "home" location (company) ID for this set of tests
        /// </summary>
        private const int HomeLocationId = 12345;

        /// <summary>
        /// The employee's "office" location (company) ID for this set of tests
        /// </summary>
        private const int OfficeLocationId = 12346;

        /// <summary>
        /// The employee's home to office distance for this set of tests
        /// </summary>
        private const decimal HomeToOfficeDistance = 10.2m;

        /// <summary>
        /// The employee's office to home distance for this set of tests
        /// </summary>
        private const decimal OfficeToHomeDistance = 10.9m;

        #endregion Fields

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Test Methods

        /// <summary>
        /// Tests the base class method DeductFromStep
        /// The step distance exceeds the deduction so the deduction remainder should be 0
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules Base")]
        public void DeductFromStepWhereStepDistanceExceedsDeductionDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var step = new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = OfficeLocationId }, 40.3m, 39.9m, 0, 0, 40.3m, false);
            steps.Add(0, step);

            var rule = new FullHomeToOfficeIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());
            var remainder = rule.DeductFromStep(step, HomeToOfficeDistance);

            Assert.AreEqual(remainder, 0);
        }

        /// <summary>
        /// Tests the base class method DeductFromStep
        /// The step distance equals the deduction so the deduction remainder should be 0
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules Base")]
        public void DeductFromStepWhereStepDistanceEqualsDeductionDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var step = new cJourneyStep(1, new Address() { Identifier = HomeLocationId }, new Address() { Identifier = OfficeLocationId }, 10.2m, 11m, 0, 0, 10.2m, false);
            steps.Add(0, step);

            var rule = new FullHomeToOfficeIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());
            var remainder = rule.DeductFromStep(step, HomeToOfficeDistance);

            Assert.AreEqual(remainder, 0);
        }

        /// <summary>
        /// Tests the base class method DeductFromStep
        /// The step distance equals the deduction so the deduction remainder should be 0
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules Base")]
        public void DeductFromStepWhereDeductionDistanceExceedsStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var step = new cJourneyStep(1, new Address() { Identifier = HomeLocationId }, new Address() { Identifier = OfficeLocationId }, 2.3m, 2.2m, 0, 0, 2.3m, false);
            steps.Add(0, step);

            // steps.Add(1, new cJourneyStep(1, new cCompany { companyid = officeLocationId }, new cCompany { companyid = homeLocationId }, 40.3m, 39.9m, 0, 1, false, string.Empty, 40.3m, false));
            var rule = new FullHomeToOfficeIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());
            var remainder = rule.DeductFromStep(step, HomeToOfficeDistance);

            Assert.AreEqual(remainder, HomeToOfficeDistance - 2.3m);
        }

        #endregion Test Methods
    }
}