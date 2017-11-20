namespace UnitTest2012Ultimate.Library
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.JourneyDeductionRules;

    /// <summary>
    /// Unit tests for "Deduct first and/or last Home to Office Distance from Journey"
    /// </summary>
    [TestClass]
    public class JourneyDeductionHomeToOfficeForFirstAndLastHomeTest
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
        /// No deductions here
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereNoHomeAddressInJourney()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// One office to home deduction
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereOneHomeAddressInMiddleOfJourney()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, 0);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// Deduction from first step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInFirstJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - HomeToOfficeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// Deduction from last step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInLastJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = HomeLocationId }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance - OfficeToHomeDistance);
        }

        /// <summary>
        /// Deduction from first and last step, home address is used in middle steps but no deductions should occur on them
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInFirstAndLastJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 32.3m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 32.2m, 0, 1, 32.3m, false));
            const decimal Step3EnteredDistance = 32.3m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step3EnteredDistance, 32.2m, 0, 2, 32.3m, false));
            const decimal Step4EnteredDistance = 32.3m;
            steps.Add(3, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step4EnteredDistance, 32.2m, 0, 3, 32.3m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - HomeToOfficeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
            Assert.AreEqual(deductedSteps[3].nummiles, Step4EnteredDistance - OfficeToHomeDistance);
        }

        /// <summary>
        /// Deduction from first step only, deduction distance exceeds first step distance
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInFirstJourneyStepAndDeductionDistanceExceedsFirstStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 2.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, Step1EnteredDistance, false));
            const decimal Step2EnteredDistance = 23.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, Step2EnteredDistance, 0, 1, Step2EnteredDistance, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, Step3EnteredDistance, 0, 2, Step3EnteredDistance, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// Deduction from last step only, deduction distance exceeds last step distance
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInLastJourneyStepAndDeductionDistanceExceedsLastStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 29.9m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, Step1EnteredDistance, false));
            const decimal Step2EnteredDistance = 23.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, Step2EnteredDistance, 0, 1, Step2EnteredDistance, false));
            const decimal Step3EnteredDistance = 2.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = HomeLocationId }, Step3EnteredDistance, Step3EnteredDistance, 0, 2, Step3EnteredDistance, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, 0);
        }

        /// <summary>
        /// Deduction from first and last step, deduction distance exceeds both first and last step distances
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInFirstAndLastJourneyStepAndDeductionDistanceExceedsFirstAndLastStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 2.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 32.3m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 32.2m, 0, 1, 32.3m, false));
            const decimal Step3EnteredDistance = 32.3m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step3EnteredDistance, 32.2m, 0, 2, 32.3m, false));
            const decimal Step4EnteredDistance = 4.33m;
            steps.Add(3, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step4EnteredDistance, 32.2m, 0, 3, 32.3m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
            Assert.AreEqual(deductedSteps[3].nummiles, 0);
        }

        /// <summary>
        /// Deduction from first and last step, deduction distances exceed all step distances
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - First and last Home to Office Distance from Journey")]
        public void DeductWhereHomeAddressInFirstAndLastJourneyStepAndDeductionDistanceExceedsAllStepDistances()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 2.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 3.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 32.2m, 0, 1, 32.3m, false));
            const decimal Step3EnteredDistance = 2.1m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step3EnteredDistance, 32.2m, 0, 2, 32.3m, false));
            const decimal Step4EnteredDistance = 4.3m;
            steps.Add(3, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step4EnteredDistance, 32.2m, 0, 3, 32.3m, false));

            var rule = new HomeToOfficeForFirstAndLastHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 0);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
            Assert.AreEqual(deductedSteps[3].nummiles, 0);
        }

        #endregion Test Methods
    }
}
