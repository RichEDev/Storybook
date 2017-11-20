using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.Flags
{
    using System.Web.UI.MobileControls;

    using Moq;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Interfaces;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Mileage;

    using Spend_Management.expenses.code;
    using System.Data.SqlClient;

    [TestClass]
    public class Flags
    {
        [TestMethod]
        [TestCategory("Flags"), TestCategory("Spend Management")]
        public void Flags_Weekend_Validation()
        {
            WeekendFlag flag = new WeekendFlag(0, FlagType.ItemOnAWeekend, FlagAction.BlockItem, string.Empty,new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty,true,45,true,true, FlagColour.Red, true,string.Empty, FlagInclusionType.All,FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25));
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 26));
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 24));
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_InvalidDate_InitialDate()
        {
            InvalidDateFlag flag = new InvalidDateFlag(0, FlagType.InvalidDate, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, InvalidDateFlagType.SetDate, new DateTime(2014, 01, 01), null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25));
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2013, 10, 25));
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_InvalidDate_SetMonths()
        {
            InvalidDateFlag flag = new InvalidDateFlag(0, FlagType.InvalidDate, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, InvalidDateFlagType.LastXMonths, null, 3, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            DateTime itemDate = DateTime.Today;
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: itemDate);
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
            itemDate = itemDate.AddMonths(-4);
            expenseItem = cExpenseItemObject.Template(4567, 12134, date: itemDate);
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_NonReimbursable_Validate()
        {
            NonReimbursableFlag flag = new NonReimbursableFlag(0, FlagType.ItemNotReimbursable, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25));
            flag.Reimbursable = true;
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
            flag.Reimbursable = false;
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_Reimbursable_Validate()
        {
            ReimbursableFlag flag = new ReimbursableFlag(0, FlagType.ItemReimbursable, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25));
            flag.Reimbursable = false;
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
            flag.Reimbursable = true;
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_AllowanceAvailable_Validate()
        {
            AllowanceAvailableFlag flag = new AllowanceAvailableFlag(0, FlagType.ItemNotReimbursable, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000);
            var connection = new Mock<IDBConnection>();
            connection.SetupAllProperties();

            connection.Setup(x => x.ExecuteScalar<decimal>("select sum([float]) - isnull((select sum(amount) from float_allocations inner join floats as innerFloats on innerFloats.floatid = float_allocations.floatid where employeeid = @employeeid and currencyid = @currencyid),0) from floats where approved = 1 and employeeid = @employeeid and currencyid = @currencyid", CommandType.Text)).Returns((decimal)45);
            connection.SetupGet(x => x.sqlexecute).Returns(new SqlCommand());
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties(), connection.Object);
            Assert.IsNotNull(item[0]);

            connection.Setup(x => x.ExecuteScalar<decimal>("select sum([float]) - isnull((select sum(amount) from float_allocations inner join floats as innerFloats on innerFloats.floatid = float_allocations.floatid where employeeid = @employeeid and currencyid = @currencyid),0) from floats where approved = 1 and employeeid = @employeeid and currencyid = @currencyid", CommandType.Text)).Returns((decimal)0);
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties(), connection.Object);
            Assert.IsNull(item);

            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, itemtype: ItemType.CreditCard);
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties(), connection.Object);
            Assert.IsNull(item);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_ReceiptNotAttached_Validate()
        {
            ReceiptNotAttachedFlag flag = new ReceiptNotAttachedFlag(0, FlagType.ItemNotReimbursable, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, receiptattached:false);
            cAccountProperties accountProperties = new cAccountProperties();
            accountProperties.AttachReceipts = true;
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, accountProperties);
            Assert.IsNotNull(item[0]);

            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, itemtype: ItemType.CreditCard, receiptattached: true);
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, accountProperties);
            Assert.IsNull(item[0]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_NumberOfPassengersLimit_Validate()
        {
            NumberOfPassengersFlag flag = new NumberOfPassengersFlag(0, FlagType.ItemNotReimbursable, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, string.Empty, true, 0, false, false, FlagColour.Amber, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All, 1);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, receiptattached: false);
            expenseItem.journeysteps.Add(1, new cJourneyStep(0, new Address(), new Address(), 2, 2, 0, 1, 0,false));
            expenseItem.journeysteps[1].passengers = new Passenger[1];
            expenseItem.journeysteps[1].passengers[0] = new Passenger();
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            //number of passengers OK
            Assert.IsNull(item[0]);
            expenseItem.journeysteps[1].passengers = new Passenger[2];
            expenseItem.journeysteps[1].passengers[0] = new Passenger();
            expenseItem.journeysteps[1].passengers[1] = new Passenger();
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            //number of passengers > limit
            Assert.IsNotNull(item[0]);

            //more than one step failing
            expenseItem.journeysteps.Add(2, new cJourneyStep(0, new Address(), new Address(), 2, 2, 0, 1, 0, false));
            expenseItem.journeysteps[2].passengers = new Passenger[2];
            expenseItem.journeysteps[2].passengers[0] = new Passenger();
            expenseItem.journeysteps[2].passengers[1] = new Passenger();
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            MileageFlaggedItem mileageItem = (MileageFlaggedItem)item[0];
            Assert.IsNotNull(mileageItem.FlaggedJourneySteps[0]);
            Assert.IsNotNull(mileageItem.FlaggedJourneySteps[1]);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_Check_Tolerances()
        {
            ToleranceFlag toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, null, null, true, 0, true, true, null, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            FlagColour returnColour = toleranceFlag.CheckTolerance(10, 20);

            //check no tolerance
            Assert.IsTrue(returnColour == FlagColour.Red);

            //no flag tolerance
            toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, null, null, true, 0, true, true, 5, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            returnColour = toleranceFlag.CheckTolerance((decimal)10.5, 10);
            Assert.IsTrue(returnColour == FlagColour.None);

            //no flag tolerance over, without amber tolerance should cause red
            toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, null, null, true, 0, true, true, 5, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            returnColour = toleranceFlag.CheckTolerance(11, 10);
            Assert.IsTrue(returnColour == FlagColour.Red);

            //no flag tolerance over, amber tolerance should cause red
            toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 10, null, true, 0, true, true, 5, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            returnColour = toleranceFlag.CheckTolerance(11, 10);
            Assert.IsTrue(returnColour == FlagColour.Amber);

            returnColour = toleranceFlag.CheckTolerance(12, 10);
            Assert.IsTrue(returnColour == FlagColour.Red);

            //amber without no flag tolerance
            toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 10, null, true, 0, true, true, null, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            returnColour = toleranceFlag.CheckTolerance(11, 10);
            Assert.IsTrue(returnColour == FlagColour.Amber);

            //amber without no flag tolerance, above should cause red
            toleranceFlag = new ToleranceFlag(0, FlagType.LimitWithReceipt, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 10, null, true, 0, true, true, null, FlagColour.None, false, string.Empty, string.Empty, string.Empty, FlagInclusionType.All, FlagInclusionType.All, true, true, true, true);
            returnColour = toleranceFlag.CheckTolerance(12, 10);
            Assert.IsTrue(returnColour == FlagColour.Red);
        }

        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void Flags_TipLimit_Validate()
        {
            TipFlag flag = new TipFlag(0, FlagType.TipLimitExceeded, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, null, true, 0, 10, true, true, null, null, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            cExpenseItem expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, receiptattached: false, total: 20, tip: 2);
            
            //tip over
            List<FlaggedItem> item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNotNull(item[0]);

            //tip not over
            expenseItem = cExpenseItemObject.Template(4567, 12134, date: new DateTime(2014, 10, 25), currencyid: 1000, receiptattached: false, total: 22, tip: 2);
            item = flag.Validate(expenseItem, GlobalTestVariables.EmployeeId, new cAccountProperties());
            Assert.IsNull(item[0]);
        }

        /// <summary>
        /// The flags frequency get date range.
        /// </summary>
        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void FlagsFrequencyGetDateRange()
        {
            FrequencyFlag flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 1, FlagPeriodType.Days, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);

            DateTime startDate, endDate;
            DateTime initialDate = new DateTime(2015, 03, 19);

            // 1 per day
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 03 && startDate.Day == 19 && endDate.Year == 2015 && endDate.Month == 03 && endDate.Day == 19);
            
            // 1 every 2 days
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 2, FlagPeriodType.Days, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 03 && startDate.Day == 18 && endDate.Year == 2015 && endDate.Month == 03 && endDate.Day == 20);

            // 1 every calendar week
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 1, FlagPeriodType.CalendarWeeks, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 03 && startDate.Day == 15 && endDate.Year == 2015 && endDate.Month == 03 && endDate.Day == 21);

            // 1 every 3 calendar week
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 3, FlagPeriodType.CalendarWeeks, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 03 && startDate.Day == 01 && endDate.Year == 2015 && endDate.Month == 04 && endDate.Day == 04);

            // 1 every 1 calendar month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 1, FlagPeriodType.CalendarMonths, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 03 && startDate.Day == 01 && endDate.Year == 2015 && endDate.Month == 03 && endDate.Day == 31);

            // 1 every 2 calendar month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 2, FlagPeriodType.CalendarMonths, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 02 && startDate.Day == 01 && endDate.Year == 2015 && endDate.Month == 04 && endDate.Day == 30);

            // 1 every 3 calendar month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 3, FlagPeriodType.CalendarMonths, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 01 && startDate.Day == 01 && endDate.Year == 2015 && endDate.Month == 05 && endDate.Day == 31);

            // 1 every 1 calendar year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 1, FlagPeriodType.CalendarYears, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 01 && startDate.Day == 01 && endDate.Year == 2015 && endDate.Month == 12 && endDate.Day == 31);

            // 1 every 1 calendar year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 2, FlagPeriodType.CalendarYears, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2014 && startDate.Month == 01 && startDate.Day == 01 && endDate.Year == 2016 && endDate.Month == 12 && endDate.Day == 31);

            // 1 every 1 calendar year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 3, FlagPeriodType.CalendarYears, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2013 && startDate.Month == 01 && startDate.Day == 01 && endDate.Year == 2017 && endDate.Month == 12 && endDate.Day == 31);

            // 1 every 1 financial year
            flag = new FrequencyFlag(
                0, 
                FlagType.FrequencyOfItemCount, 
                FlagAction.BlockItem, 
                string.Empty, 
                new List<int>(),
                new List<AssociatedExpenseItem>(), 
                DateTime.Now, 
                null, 
                null, 
                null, 
                1, 
                FlagFrequencyType.Every, 
                1, 
                FlagPeriodType.FinancialYears, 
                null, 
                string.Empty, 
                true, 
                0, 
                true, 
                true, 
                2015, 
                new DateTime(2015, 04, 06), 
                new DateTime(2015, 04, 05), 
                FlagColour.None, 
                false, 
                string.Empty, 
                FlagInclusionType.All, 
                FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2014 && startDate.Month == 04 && startDate.Day == 06 && endDate.Year == 2015 && endDate.Month == 04 && endDate.Day == 05);

            // 1 every 2 financial year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 2, FlagPeriodType.FinancialYears, null, string.Empty, true, 0, true, true, 2015, new DateTime(2015, 04, 06), new DateTime(2015, 04, 05), FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2013 && startDate.Month == 04 && startDate.Day == 06 && endDate.Year == 2016 && endDate.Month == 04 && endDate.Day == 05);

            // 1 every 3 financial year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 3, FlagPeriodType.FinancialYears, null, string.Empty, true, 0, true, true, 2015, new DateTime(2015, 04, 06), new DateTime(2015, 04, 05), FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2012 && startDate.Month == 04 && startDate.Day == 06 && endDate.Year == 2017 && endDate.Month == 04 && endDate.Day == 05);

            // 1 every 1 financial year, different financial year
            initialDate = new DateTime(2015, 05, 05);
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.Every, 1, FlagPeriodType.FinancialYears, null, string.Empty, true, 0, true, true, 2015, new DateTime(2015, 04, 06), new DateTime(2015, 04, 05), FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            flag.GetDateRange(initialDate, out startDate, out endDate);
            Assert.IsTrue(startDate.Year == 2015 && startDate.Month == 04 && startDate.Day == 06 && endDate.Year == 2016 && endDate.Month == 04 && endDate.Day == 05);
        }

        /// <summary>
        /// The flags frequency get initial flag date.
        /// </summary>
        [TestMethod]
        [TestCategory("Flags")]
        [TestCategory("Spend Management")]
        public void FlagsFrequencyGetInitialFlagDate()
        {
            FrequencyFlag flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 1, FlagPeriodType.Days, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            DateTime initialDate = new DateTime(2015, 03, 19);

            // 1 in the last 1 day
            DateTime returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 03 && returnDate.Day == 18);

            // 1 in the last 2 day
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 2, FlagPeriodType.Days, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 03 && returnDate.Day == 17);

            // 1 in the last 3 day
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 3, FlagPeriodType.Days, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 03 && returnDate.Day == 16);

            // 1 in the last 1 month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 1, FlagPeriodType.Months, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 02 && returnDate.Day == 19);

            // 1 in the last 2 month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 2, FlagPeriodType.Months, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 01 && returnDate.Day == 19);

            // 1 in the last 3 month
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 3, FlagPeriodType.Months, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2014 && returnDate.Month == 12 && returnDate.Day == 19);

            // 1 in the last 1 week
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 1, FlagPeriodType.Weeks, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 03 && returnDate.Day == 12);

            // 1 in the last 2 week
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 2, FlagPeriodType.Weeks, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 03 && returnDate.Day == 5);

            // 1 in the last 3 week
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 3, FlagPeriodType.Weeks, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2015 && returnDate.Month == 02 && returnDate.Day == 26);

            // 1 in the last 1 year
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 1, FlagPeriodType.Years, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2014 && returnDate.Month == 03 && returnDate.Day == 19);

            // 1 in the last 2 years
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 2, FlagPeriodType.Years, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2013 && returnDate.Month == 03 && returnDate.Day == 19);

            // 1 in the last 3 years
            flag = new FrequencyFlag(0, FlagType.FrequencyOfItemCount, FlagAction.BlockItem, string.Empty, new List<int>(), new List<AssociatedExpenseItem>(), DateTime.Now, null, null, null, 1, FlagFrequencyType.InTheLast, 3, FlagPeriodType.Years, null, string.Empty, true, 0, true, true, FlagColour.None, false, string.Empty, FlagInclusionType.All, FlagInclusionType.All);
            returnDate = flag.GetInitialFlagDate(initialDate);
            Assert.IsTrue(returnDate.Year == 2012 && returnDate.Month == 03 && returnDate.Day == 19);
        }
    }
}
