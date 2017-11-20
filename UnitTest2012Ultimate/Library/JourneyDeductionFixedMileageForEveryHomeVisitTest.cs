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
    public class JourneyDeductionFixedMileageForEveryHomeVisitTest
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
        private const decimal HomeToOfficeDistance = 9.3m;

        /// <summary>
        /// The employee's office to home distance for this set of tests
        /// </summary>
        private const decimal OfficeToHomeDistance = 9.3m;

        /// <summary>
        /// The subcat used in the tests
        /// </summary>
        private cSubcat _testSubcat;

        #endregion Fields
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this._testSubcat = new cSubcat();
            _testSubcat.HomeToOfficeFixedMiles = (float?) HomeToOfficeDistance ;
        }
        #region Test Methods

        /// <summary>
        /// No deductions here
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereNoHomeAddressInJourney()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// One Office to Home deduction
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereHomeAddressInMiddleOfJourneyAndDeductionDistanceExceedsFistDeductableStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, Step2EnteredDistance, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11110 }, Step3EnteredDistance, Step3EnteredDistance, 0, 2, 29.9m, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var expectedStep3Deduction = Step3EnteredDistance - (OfficeToHomeDistance - steps[1].nummiles) - HomeToOfficeDistance;

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, 0);
            Assert.AreEqual(deductedSteps[2].nummiles, expectedStep3Deduction);
        }

        /// <summary>
        /// One Home to Office deduction from first step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereHomeAddressInFirstJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = 11110 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - HomeToOfficeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance);
        }

        /// <summary>
        /// One Office to Home deduction from last step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereHomeAddressInLastJourneyStep()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11110 }, new Address { Identifier = 11111 }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 11112 }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 11112 }, new Address { Identifier = HomeLocationId }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, Step2EnteredDistance);
            Assert.AreEqual(deductedSteps[2].nummiles, Step3EnteredDistance - OfficeToHomeDistance);
        }

        /// <summary>
        /// One Office to Home deduction from last step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereHomeAddressInEveryJourneyStepAndHomeToOfficeDistanceExceedsOneStepDistance()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 32.3m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step1EnteredDistance, 32.2m, 0, 0, 32.3m, false));
            const decimal Step2EnteredDistance = 2.4m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = HomeLocationId }, Step2EnteredDistance, 2.2m, 0, 1, 2.4m, false));
            const decimal Step3EnteredDistance = 29.9m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step3EnteredDistance, 30.0m, 0, 2, 29.9m, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var expectedStep3Deduction = Step3EnteredDistance - (OfficeToHomeDistance - steps[1].nummiles) - HomeToOfficeDistance;

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, Step1EnteredDistance - OfficeToHomeDistance);
            Assert.AreEqual(deductedSteps[1].nummiles, 0);
            Assert.AreEqual(deductedSteps[2].nummiles, expectedStep3Deduction);
        }

        /// <summary>
        /// One Office to Home deduction from last step only
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Deduct Fixed mileage Every Time Home is Visited")]
        public void DeductWhereHomeAddressInEveryJourneyStepAndHomeToOfficeDistanceExceedsOneStepDistance2()
        {
            var steps = new SortedList<int, cJourneyStep>();
            const decimal Step1EnteredDistance = 149.75m;
            steps.Add(0, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 11111 }, Step1EnteredDistance, Step1EnteredDistance, 0, 0, Step1EnteredDistance, false));
            const decimal Step2EnteredDistance = 149.79m;
            steps.Add(1, new cJourneyStep(1, new Address { Identifier = 11111 }, new Address { Identifier = 22222 }, Step2EnteredDistance, Step2EnteredDistance, 0, 1, Step2EnteredDistance, false));
            const decimal Step3EnteredDistance = 5.63m;
            steps.Add(2, new cJourneyStep(1, new Address { Identifier = 22222 }, new Address { Identifier = HomeLocationId }, Step3EnteredDistance, Step3EnteredDistance, 0, 2, Step3EnteredDistance, false));
            const decimal Step4EnteredDistance = 5.6m;
            steps.Add(3, new cJourneyStep(1, new Address { Identifier = HomeLocationId }, new Address { Identifier = 22222 }, Step4EnteredDistance, Step4EnteredDistance, 0, 3, Step4EnteredDistance, false));
            const decimal Step5EnteredDistance = 5.63m;
            steps.Add(4, new cJourneyStep(1, new Address { Identifier = 22222 }, new Address { Identifier = HomeLocationId }, Step5EnteredDistance, Step5EnteredDistance, 0, 4, Step5EnteredDistance, false));

            var rule = new FixedDeductionIfStartOrFinishHome(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, _testSubcat);

            var deductedSteps = rule.Deduct();

            Assert.AreEqual(deductedSteps[0].nummiles, 140.45m);
            Assert.AreEqual(deductedSteps[1].nummiles, 138.61m);
            Assert.AreEqual(deductedSteps[2].nummiles, 0);
            Assert.AreEqual(deductedSteps[3].nummiles, 0);
            Assert.AreEqual(deductedSteps[4].nummiles, 0);
        }

        #endregion Test Methods
    }
}
