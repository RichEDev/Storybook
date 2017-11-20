using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spend_Management;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using System.Collections.Generic;

namespace UnitTest2012Ultimate.Mileage
{
    [TestClass]
    public class MileageCatsTests
    {
        [TestMethod()]
        public void GetPencePerMileForThresholdTest()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var subcat = new cSubcat(1, 1, "uisubcat", "uisubcat", true, false, false, false, false, false, 0, string.Empty, false, false, 1, false, false, CalculationType.PencePerMile, true, true, false, string.Empty, false, 1, true, false, false, false, false, false, false, false, false, false, false, string.Empty, false, false, 1, 1, false, false, null, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, string.Empty, false, false, null, null, null, null, false, null, true, HomeToLocationType.DeductFirstAndLastHome, 1, false, 1, false, false, 0, 1, DateTime.Now, DateTime.Now.AddDays(2), false, null);
            decimal fuelcost = 0;
            decimal pencepermile = 0;
            var mileage = new cMileageCat();
            var threshold = new cMileageThreshold(1, 1, 200M, null, RangeType.LessThan, .1M, .2M, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, .2M);
            var startlocation = new Address();
            var endlocation = new Address();
            var step = new cJourneyStep(1, startlocation, endlocation, 10, 10, 0, 0, 10, false);
            var rate = new VehicleJourneyRateThresholdRate();
            rate.MileageThresholdId = 1;
            rate.RatePerUnit = .2M;
            rate.AmountForVat = 1;

            pencepermile = mileageCats.GetPencePerDistanceForThreshold(subcat, mileage, threshold, step, rate, null);
            Assert.IsTrue(pencepermile == .2M);
            step.numpassengers = 1;
            pencepermile = mileageCats.GetPencePerDistanceForThreshold(subcat, mileage, threshold, step, rate, null);
            Assert.IsTrue(pencepermile == .3M);

        }
        [TestMethod()]
        public void SetStartingThresholdTest()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var thresholds = new List<cMileageThreshold> {
                new cMileageThreshold(2, 2,  null, null, RangeType.LessThan, 0, 0, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 0),
                new cMileageThreshold(3, 4,  50, 100, RangeType.Between, 0, 0, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 0),
                new cMileageThreshold(1, 1,  100, null, RangeType.GreaterThanOrEqualTo, 0, 0, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 0)
                };
            var threshold = new cMileageThreshold(1, 1, null, null, RangeType.Any, 0, 0, DateTime.Now, GlobalTestVariables.EmployeeId, null, null, 0);
            var daterange = new cMileageDaterange(1, 1, null, null, thresholds, DateRangeType.Any, DateTime.Now, GlobalTestVariables.EmployeeId, null, null);
            var result = mileageCats.SetStartingThreshold(ThresholdType.Annual, daterange, threshold);
            Assert.IsTrue(result == 3);
        }
        [TestMethod()]
        public void CompareMileageToCurrentThresholdTest()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var below = 0M;
            var total = 10M;
            var grandtotal = 0M;
            var oldmiles = 0M;
            var pencepermile = 0.5M;
            var result = mileageCats.CompareMileageToCurrentThreshold(100, 200, out below, out total, ref grandtotal, ref oldmiles, ref pencepermile);
            Assert.IsTrue(result);
            Assert.IsTrue(total == 50M);
            oldmiles = 100M;
            result = mileageCats.CompareMileageToCurrentThreshold(300, 200, out below, out total, ref grandtotal, ref oldmiles, ref pencepermile);
            Assert.IsFalse(result);
            Assert.IsTrue(below == 100);
            Assert.IsTrue(total == 50M);
        }

        [TestMethod(), ExpectedException(typeof(System.OverflowException))]
        public void ConvertMilesToKMMaxValueTest()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var result = mileageCats.convertMilesToKM(decimal.MaxValue);
        }

        [TestMethod()]
        public void convertMilesToKMTest()
        {
            var mileageCats = new cMileagecats(GlobalTestVariables.AccountId);
            var result = mileageCats.convertMilesToKM(0);
            Assert.IsTrue(result == 0);
            result = mileageCats.convertMilesToKM(1);
            Assert.IsTrue(result == 1.61M, result.ToString());
        }

        [TestMethod(), ExpectedException(typeof(System.OverflowException))]
        public void ConvertMilesToKilometresMaxValueTest()
        {
            var result = cMileagecats.ConvertMilesToKilometres(decimal.MaxValue);
        }

        [TestMethod()]
        public void ConvertMilesToKilometresTest()
        {
            var result = cMileagecats.ConvertMilesToKilometres(0);
            Assert.IsTrue(result == 0);
            result = cMileagecats.ConvertMilesToKilometres(1);
            Assert.IsTrue(result == 1.609344M, result.ToString());
        }

        [TestMethod()]
        public void ConvertKilometresToMilesTest()
        {
            var result = cMileagecats.ConvertKilometresToMiles(0);
            Assert.IsTrue(result == 0);
            result = cMileagecats.ConvertKilometresToMiles(decimal.MaxValue);
            Assert.IsTrue(result == 49230097800261682768596366181M);
            result = cMileagecats.ConvertKilometresToMiles(-1);
            Assert.IsTrue(result == -0.6213711922373339696174341844M);
        }


    }
}
