namespace UnitTest2012Ultimate.Library
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.JourneyDeductionRules;

    /// <summary>
    /// Unit tests for "Full Home to office deduction if start or end address is home" journey deduction rule
    /// </summary>
    [TestClass]
    public class JourneyDeductionHomeToOfficeForEveryHomeVisitTest
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
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Every Time Home is Visited")]
        public void DeductWhereNoHomeAddressInJourney()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForEveryHomeVisit(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// One Office to Home deduction 
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Every Time Home is Visited")]
        public void DeductWhereHomeAddressInMiddleOfJourneyAndDeductionDistanceExceedsFistDeductableStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForEveryHomeVisit(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, 0);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance - HomeToOfficeDistance);
        }

        /// <summary>
        /// One Home to Office deduction from first step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Every Time Home is Visited")]
        public void DeductWhereHomeAddressInFirstJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForEveryHomeVisit(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - HomeToOfficeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// One Office to Home deduction from last step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Every Time Home is Visited")]
        public void DeductWhereHomeAddressInLastJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = HomeLocationId }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForEveryHomeVisit(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance - OfficeToHomeDistance);
        }

        /// <summary>
        /// deduction from every step
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Every Time Home is Visited")]
        public void DeductWhereHomeAddressInEveryJourneyStepAndHomeToOfficeDistanceExceedsOneStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new HomeToOfficeForEveryHomeVisit(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, new cSubcat());

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - OfficeToHomeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, 0);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance - HomeToOfficeDistance);
        }
        #endregion Test Methods
    }
}
