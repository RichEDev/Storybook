namespace UnitTest2012Ultimate.Library
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.JourneyDeductionRules;

    /// <summary>
    /// Unit tests for "Deduct Home to Office Distance from journey once" journey deduction rule
    /// </summary>
    [TestClass]
    public class JourneyDeductionHomeToOfficeOnceTest
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

        #region Test Methods

        /// <summary>
        /// Deduction on the first (only) step, step distance exceeds deduction distance and contains no home or office locations
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Home To Office Once")]
        public void DeductWhereStepDistanceExceedsDeductionDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));

            var rule = new HomeToOfficeOnce(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - HomeToOfficeDistance);
        }

        /// <summary>
        /// Deduction on the first (only) step, step is home to office 
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Home To Office Once")]
        public void DeductWhereStepIsHomeToOffice()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = HomeToOfficeDistance;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = OfficeLocationId }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, Step1EnteredDistance, false));

            var rule = new HomeToOfficeOnce(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
        }

        /// <summary>
        /// Deduction on the first (only) step, step is office to home 
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Home To Office Once")]
        public void DeductWhereStepIsOfficeToHome()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = HomeToOfficeDistance;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = OfficeLocationId }, new Address { Identifier = HomeLocationId }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, Step1EnteredDistance, false));

            var rule = new HomeToOfficeOnce(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
        }

        /// <summary>
        /// Deduction on the first (only) step, step distance is less than deduction distance and contains no home or office locations
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Home To Office Once")]
        public void DeductWhereDeductionDistanceExceedsStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 2.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 2.3m, 0, 0, 2.3m, false));

            var rule = new HomeToOfficeOnce(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
        }

        /// <summary>
        /// Deduction from first step only, additional steps should remain the same
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Home To Office Once")]
        public void DeductWhereDeductionDistanceExceedsStepDistanceMultipleSteps()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 2.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 2.3m, 0, 0, 2.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeOnce(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        #endregion Test Methods
    }
}
