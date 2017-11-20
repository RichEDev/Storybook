using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.JourneyDeductionRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest2012Ultimate.Library
{
    using Spend_Management;

    [TestClass]
    public class JourneyDeductionJuniorDoctorRotation
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
        private const decimal HomeToOfficeDistance = 4m;

        /// <summary>
        /// The employee's office to home distance for this set of tests
        /// </summary>
        private const decimal OfficeToHomeDistance = 4m;

        private cSubcat subcat;

        private const int baseLocationId = 9876;

        #endregion Fields

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney1()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
              AddressName = "Home",
              Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, officeAddress, 6, 6, 0, 0, 6, false));
            steps.Add(2, new cJourneyStep(2, officeAddress, homeAddress, 6, 6, 0, 0, 6, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            foreach (cJourneyStep item in result.Values)
            {
                Assert.IsTrue(item.nummiles == 2);
            }
        }

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney2()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var baseWorkAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, baseAddress, 5, 5, 0, 0, 5, false));
            steps.Add(2, new cJourneyStep(2, baseAddress, homeAddress, 5, 5, 0, 0, 5, false));
            
            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            foreach (cJourneyStep item in result.Values)
            {
                Assert.IsTrue(item.nummiles == 1);
            }
        }

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney3()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var baseWorkAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, officeAddress, 6, 6, 0, 0, 6, false));
            steps.Add(2, new cJourneyStep(2, officeAddress, baseAddress, 5, 5, 0, 0, 5, false));
            steps.Add(3, new cJourneyStep(3, baseAddress, homeAddress, 6, 6, 0, 0, 6, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            Assert.IsTrue(result[1].nummiles == 6);
            Assert.IsTrue(result[2].nummiles == 5);
            Assert.IsTrue(result[3].nummiles == 6);
            Assert.IsTrue(result[2].OfficialJourney);
        }


        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney4()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var baseWorkAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, baseAddress, 4, 4, 0, 0, 4, false));
            steps.Add(2, new cJourneyStep(2, baseAddress, officeAddress, 5, 5, 0, 0, 5, false));
            steps.Add(3, new cJourneyStep(3, officeAddress, homeAddress, 5, 5, 0, 0, 5, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            Assert.IsTrue(result[1].nummiles == 4);
            Assert.IsTrue(result[2].nummiles == 5);
            Assert.IsTrue(result[3].nummiles == 5);
            Assert.IsTrue(result[2].OfficialJourney);
        }

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney5()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var otherAddress = new Address
            {
                AddressName = "UHND",
                Identifier = 998877
            };

            var baseWorkAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, baseAddress, 4, 4, 0, 0, 4, false));
            steps.Add(2, new cJourneyStep(2, baseAddress, officeAddress, 5, 5, 0, 0, 5, false));
            steps.Add(3, new cJourneyStep(3, officeAddress, otherAddress, 12, 12, 0, 0, 12, false));
            steps.Add(4, new cJourneyStep(4, officeAddress, homeAddress, 12, 12, 0, 0, 12, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            Assert.IsTrue(result[1].nummiles == 4);
            Assert.IsTrue(result[2].nummiles == 5);
            Assert.IsTrue(result[3].nummiles == 12);
            Assert.IsTrue(result[4].nummiles == 12);
            Assert.IsTrue(result[3].OfficialJourney);
        }

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourney6()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var otherAddress = new Address
            {
                AddressName = "UHND",
                Identifier = 998877
            };

            var yetAnotherAddress = new Address
            {
                AddressName = "SEL",
                Identifier = 998878
            };

            var baseWorkAddress = new cEmployeeWorkLocation(baseLocationId, GlobalTestVariables.EmployeeId, baseLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);

            steps.Add(1, new cJourneyStep(1, homeAddress, baseAddress, 4, 4, 0, 0, 4, false));
            steps.Add(2, new cJourneyStep(2, baseAddress, officeAddress, 5, 5, 0, 0, 5, false));
            steps.Add(3, new cJourneyStep(3, officeAddress, otherAddress, 12, 12, 0, 0, 12, false));
            steps.Add(4, new cJourneyStep(4, officeAddress, yetAnotherAddress, 20, 20, 0, 0, 20, false));
            steps.Add(5, new cJourneyStep(5, yetAnotherAddress, homeAddress, 60, 60, 0, 0, 60, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, HomeToOfficeDistance, OfficeToHomeDistance, HomeLocationId, OfficeLocationId, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            Assert.IsTrue(result[1].nummiles == 4);
            Assert.IsTrue(result[2].nummiles == 5);
            Assert.IsTrue(result[3].nummiles == 12);
            Assert.IsTrue(result[4].nummiles == 20);
            Assert.IsTrue(result[5].nummiles == 60);
            Assert.IsTrue(result[3].OfficialJourney);
        }

        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorJourneyNoWorkAddress()
        {
            var steps = new SortedList<int, cJourneyStep>();
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var yetAnotherAddress = new Address
            {
                AddressName = "SEL",
                Identifier = 998878
            };

            cEmployeeWorkLocation baseWorkAddress = null;

            steps.Add(1, new cJourneyStep(1, homeAddress, yetAnotherAddress, 4, 4, 0, 0, 4, false));
            steps.Add(2, new cJourneyStep(2, yetAnotherAddress, homeAddress, 60, 60, 0, 0, 60, false));

            var juniorDoctor = new JuniorDoctorRotation(steps, 0, 0, HomeLocationId, 0, this.subcat, baseWorkAddress);
            var result = juniorDoctor.Deduct();
            Assert.IsNotNull(result);
            Assert.IsTrue(result[1].nummiles == 4);
            Assert.IsTrue(result[1].OfficialJourney);
            Assert.IsTrue(result[2].nummiles == 60);
            Assert.IsTrue(result[2].OfficialJourney);
        }

        [TestMethod, TestCategory("Spend Management Library"),
         TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorPencePerMileToKmToMile()
        {
            var mileage = new cMileageCat { mileUom = MileageUOM.KM };
            decimal result = cMileagecats.ConvertPencePerMileToPencePerKilometer(mileage, 1);
            Assert.IsTrue(result == (decimal)1.609344);
            mileage = new cMileageCat { mileUom = MileageUOM.Mile };
            result = cMileagecats.ConvertPencePerMileToPencePerKilometer(mileage, 1);
            Assert.IsTrue(result == (decimal)1);
        }

        [TestMethod, TestCategory("Spend Management Library"),
         TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorAddPassengersPencePerMile()
        {
            var threshold = new cMileageThreshold(1, 1, 1, 1, RangeType.Any, 1, (decimal)0.5, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 1);
            var step = new cJourneyStep(1, null, null, 2, 2, 1, 1, 1, false);
            decimal result = cMileagecats.AddPassengersPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            this.subcat = new cSubcat();
            result = cMileagecats.AddPassengersPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            this.subcat.calculation = CalculationType.PencePerMile;
            result = cMileagecats.AddPassengersPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            this.subcat.passengersapp = true;
            result = cMileagecats.AddPassengersPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)2);
        }


        [TestMethod, TestCategory("Spend Management Library"),
         TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorAddHeavyEquipmentPencePerMile()
        {
            var threshold = new cMileageThreshold(1, 1, 1, 1, RangeType.Any, 1, (decimal)0.5, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 1);
            var step = new cJourneyStep(1, null, null, 2, 2, 1, 1, 1, false);
            decimal result = cMileagecats.AddHeavyEquipmentPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            this.subcat = new cSubcat();
            result = cMileagecats.AddHeavyEquipmentPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            this.subcat.allowHeavyBulkyMileage = true;
            result = cMileagecats.AddHeavyEquipmentPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)1);
            step.heavyBulkyEquipment = true;
            result = cMileagecats.AddHeavyEquipmentPencePerMile(this.subcat, threshold, step, 1);
            Assert.IsTrue(result == (decimal)2);
        }

        [TestMethod, TestCategory("Spend Management Library"),
         TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorHomeToOfficePencePerMileDeduction()
        {
            var publicTransportRate = new VehicleJourneyRateThresholdRate { RatePerUnit = (decimal?).33 };

            decimal result = cMileagecats.JuniorDoctorHomeToOfficePencePerMileDeduction(publicTransportRate, 1);
            Assert.IsTrue(result == (decimal).67);
            publicTransportRate = null;

            result = cMileagecats.JuniorDoctorHomeToOfficePencePerMileDeduction(publicTransportRate, 1);
            Assert.IsTrue(result == (decimal)1);
        }

        [TestMethod, TestCategory("Spend Management Library"),
         TestCategory("Journey Deduction Rules - Junior Doctor Rotation")]
        public void JuniorDoctorAddFullRateMileageToJourney()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var expenseitem = cExpenseItemObject.Template(1, 1, hometoofficedeductionmethod: HomeToLocationType.JuniorDoctorRotation);
            
            var rate = new VehicleJourneyRateThresholdRate();
            rate.MileageThresholdRateId = 1;
            var publicTransportRate = new VehicleJourneyRateThresholdRate();
            publicTransportRate.MileageThresholdRateId = 2;
            
            decimal grandTotal = 1;
            decimal vat = 0;
            var updateOldMiles = false;
            var vatCalc = new Mock<IVat>();
            vatCalc.SetupAllProperties();
            var subAccountProp = new cAccountProperties();
            var employeeWorkLocation = new cEmployeeWorkLocation(OfficeLocationId, GlobalTestVariables.EmployeeId, OfficeLocationId, null, null, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, null, true);
            var employeeHomeLocation = new cEmployeeHomeLocation(HomeLocationId, GlobalTestVariables.EmployeeId, HomeLocationId, null, null, DateTime.Now, GlobalTestVariables.EmployeeId, null, null);

            var distance = new  Mock<IJourneyDistances>();
            distance.SetupAllProperties();

            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);

            Assert.IsTrue(grandTotal == 1);
            rate.RatePerUnit = (decimal?).58;
            publicTransportRate.RatePerUnit = (decimal?).24;

            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);
            Assert.IsTrue(grandTotal == 1);
            expenseitem.journeysteps.Add(1, new cJourneyStep(1, null, null, 2, 2, 0, 1, 2, false));

            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);
            Assert.IsTrue(grandTotal == 1);

            
            var homeAddress = new Address
            {
                AddressName = "Home",
                Identifier = HomeLocationId
            };

            var officeAddress = new Address
            {
                AddressName = "Office",
                Identifier = OfficeLocationId
            };

            var baseAddress = new Address
            {
                AddressName = "Base",
                Identifier = baseLocationId
            };

            var otherAddress = new Address
            {
                AddressName = "UHND",
                Identifier = 998877
            };

            var otherAddress2 = new Address
            {
                AddressName = "SEL",
                Identifier = 998878
            };

            var otherAddress3 = new Address
            {
                AddressName = "SEL2",
                Identifier = 998879
            };

            var otherAddress4 = new Address
            {
                AddressName = "SEL4",
                Identifier = 998880
            };

            grandTotal = new decimal(32.334);
            distance.Setup(x => x.GetRecommendedOrCustomDistance(officeAddress.Identifier, otherAddress2.Identifier))
                .Returns(28.5M);
            expenseitem.journeysteps.Clear();
            expenseitem.journeysteps.Add(1, this.AddStep(1, new decimal(15.6), false, homeAddress, officeAddress));
            expenseitem.journeysteps.Add(2, this.AddStep(2, new decimal(14), true, officeAddress, otherAddress));
            expenseitem.journeysteps.Add(3, this.AddStep(3, new decimal(22.5), true, otherAddress, otherAddress2));
            expenseitem.journeysteps.Add(4, this.AddStep(4, new decimal(43), true, otherAddress2, homeAddress));

            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);
            Assert.IsTrue(grandTotal == new decimal(47.934), "Was expecting {0}, I got {1}", 47.934, grandTotal);


            grandTotal = new decimal(35.496);
            distance.Setup(x => x.GetRecommendedOrCustomDistance(officeAddress.Identifier, otherAddress4.Identifier))
                .Returns(12);

            expenseitem.journeysteps.Clear();
            expenseitem.journeysteps.Add(1, this.AddStep(1, new decimal(15.6), false, homeAddress, officeAddress));
            expenseitem.journeysteps.Add(2, this.AddStep(2, new decimal(4), true, officeAddress, otherAddress));
            expenseitem.journeysteps.Add(3, this.AddStep(3, new decimal(4), true, otherAddress, otherAddress2));
            expenseitem.journeysteps.Add(4, this.AddStep(4, new decimal(8), true, otherAddress2, officeAddress));
            expenseitem.journeysteps.Add(5, this.AddStep(5, new decimal(15.6), false, officeAddress, homeAddress));
            expenseitem.journeysteps.Add(6, this.AddStep(6, new decimal(15.6), false, homeAddress, officeAddress));
            expenseitem.journeysteps.Add(7, this.AddStep(7, new decimal(6), true, officeAddress, otherAddress3));
            expenseitem.journeysteps.Add(8, this.AddStep(8, new decimal(8), true, otherAddress3, otherAddress4));
            expenseitem.journeysteps.Add(9, this.AddStep(9, new decimal(12), true, otherAddress4, officeAddress));
            expenseitem.journeysteps.Add(10, this.AddStep(10, new decimal(15.6), false, officeAddress, homeAddress));
            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);
            Assert.IsTrue(grandTotal == new decimal(45.576), "Was expecting {0}, I got {1}", 45.576, grandTotal);



            grandTotal = new decimal(47.6);
            distance.Setup(x => x.GetRecommendedOrCustomDistance(officeAddress.Identifier, otherAddress3.Identifier))
                .Returns(30);

            expenseitem.journeysteps.Clear();
            expenseitem.journeysteps.Add(1, this.AddStep(1, new decimal(20), false, homeAddress, officeAddress));
            expenseitem.journeysteps.Add(2, this.AddStep(2, new decimal(40), true, officeAddress, otherAddress));
            expenseitem.journeysteps.Add(3, this.AddStep(3, new decimal(20), true, otherAddress, otherAddress3));
            expenseitem.journeysteps.Add(4, this.AddStep(4, new decimal(60), true, otherAddress3, homeAddress));
            
            mileageCats.AddFullRateMileageToJourney(
                GlobalTestVariables.EmployeeId,
                expenseitem,
                rate,
                publicTransportRate,
                ref grandTotal,
                ref vat,
                updateOldMiles,
                this.subcat,
                vatCalc.Object,
                subAccountProp,
                employeeWorkLocation,
                employeeHomeLocation,
                distance.Object);
            Assert.IsTrue(grandTotal == new decimal(69.2), "Was expecting {0}, I got {1}", 69.2, grandTotal);
        }

        private cJourneyStep AddStep(byte stepNo, decimal distance, bool official, Address from, Address to)
        {
            var result = new cJourneyStep(stepNo, from, to, distance, distance, 0, stepNo, distance, false)
                             {
                                 OfficialJourney = official
                             };
            return result;
        }
    }

}
