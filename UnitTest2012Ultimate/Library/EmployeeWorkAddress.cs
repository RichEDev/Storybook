#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Interfaces;
using UnitTest2012Ultimate.DatabaseMock;
using UnitTest2012Ultimate.expenses;
using Utilities.DistributedCaching;

#endregion

namespace UnitTest2012Ultimate.Library
{
    [TestClass]
    public class EmployeeWorkAddress
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        /// <summary>
        ///     The my class initialize.
        /// </summary>
        /// <param name="testContext">
        ///     The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        ///     Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion

        [TestMethod]
        public void EmployeeWorkLocationGetBy_NoData()
        {
            this.EmployeeWorkLocationGetBy_Test(new List<object>(), null);
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_Single()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetBy_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 1, 1),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false));
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_Multi()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetBy_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 1, 1),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false));
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_SingleSingle()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetBy_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false));
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_SingleMulti()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetBy_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 79, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false));
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_MultiMulti()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetBy_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(10, GlobalTestVariables.EmployeeId, 71, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 79, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false));
        }


        [TestMethod]
        public void EmployeeWorkLocationGetBy_MultiNoPrimary()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetByPrimary_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(10, GlobalTestVariables.EmployeeId, 71, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 79, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false),
                null);
        }

        [TestMethod]
        public void EmployeeWorkLocationGetBy_MultiOnePrimary()
        {
            var dtCreatedOn = DateTime.Now;
            this.EmployeeWorkLocationGetByPrimary_Test(
                new List<object>
                {
                    new cEmployeeWorkLocation(10, GlobalTestVariables.EmployeeId, 71, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(11, GlobalTestVariables.EmployeeId, 76, new DateTime(2011, 1, 1),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false),
                    new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 79, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, true),
                    new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                        new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                        null, null, null, false)
                },
                new cEmployeeWorkLocation(13, GlobalTestVariables.EmployeeId, 82, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, false),
                new cEmployeeWorkLocation(12, GlobalTestVariables.EmployeeId, 79, new DateTime(2011, 4, 4),
                    new DateTime(2019, 12, 31), true, false, dtCreatedOn, GlobalTestVariables.EmployeeId,
                    null, null, null, true));
        }


        private void EmployeeWorkLocationGetBy_Test(List<object> employeeWorkAddressDataObjects,
            cEmployeeWorkLocation expectedResult)
        {
            var user = Moqs.CurrentUser();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, "employeeWorkAddresses",
                GlobalTestVariables.EmployeeId.ToString());

            var employeeWorkAddressData =
                Reader.MockReaderDataFromClassData(
                    "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, ESRAssignmentLocationId, rotational, primaryRotational FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
                    employeeWorkAddressDataObjects);
            employeeWorkAddressData.AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", a => a.EmployeeWorkAddressId);
            employeeWorkAddressData.AddAlias<cEmployeeWorkLocation>("AddressID", a => a.LocationID);

            Mock<IDBConnection> dbConnection =
                Reader.NormalDatabase(new[] { employeeWorkAddressData });
            var employeeWorkAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId,
                GlobalTestVariables.EmployeeId, dbConnection.Object);

            // Before start
            Assert.IsNull(employeeWorkAddresses.GetBy(DateTime.MinValue));

            // After start, unfiltered
            Assert.IsTrue(this.AreEqual(employeeWorkAddresses.GetBy(new DateTime(2014, 10, 1)), expectedResult));

            // After start, filtered
            Assert.IsTrue(this.AreEqual(employeeWorkAddresses.GetBy(user, new DateTime(2014, 10, 1), 42), expectedResult));

            // After end
            Assert.IsNull(employeeWorkAddresses.GetBy(DateTime.MaxValue));
        }

        private void EmployeeWorkLocationGetByPrimary_Test(List<object> employeeWorkAddressDataObjects,
            cEmployeeWorkLocation expectedResult, cEmployeeWorkLocation primaryResult)
        {
            var user = Moqs.CurrentUser();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, "employeeWorkAddresses",
                GlobalTestVariables.EmployeeId.ToString());

            var employeeWorkAddressData =
                Reader.MockReaderDataFromClassData(
                    "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, ESRAssignmentLocationId, rotational, primaryRotational FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
                    employeeWorkAddressDataObjects);
            employeeWorkAddressData.AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", a => a.EmployeeWorkAddressId);
            employeeWorkAddressData.AddAlias<cEmployeeWorkLocation>("AddressID", a => a.LocationID);

            Mock<IDBConnection> dbConnection =
                Reader.NormalDatabase(new[] { employeeWorkAddressData });
            var employeeWorkAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId,
                GlobalTestVariables.EmployeeId, dbConnection.Object);

            // Before start
            Assert.IsNull(employeeWorkAddresses.GetBy(DateTime.MinValue));

            // After start, unfiltered
            Assert.IsTrue(this.AreEqual(employeeWorkAddresses.GetBy(new DateTime(2014, 10, 1)), expectedResult));

            // After start, filtered
            Assert.IsTrue(this.AreEqual(employeeWorkAddresses.GetBy(user, new DateTime(2014, 10, 1), 42), expectedResult));

            // Just get the primary
            Assert.IsTrue(this.AreEqual(employeeWorkAddresses.GetBy(new DateTime(2014, 10, 1), true), primaryResult));

            // After end
            Assert.IsNull(employeeWorkAddresses.GetBy(DateTime.MaxValue));
        }

        protected bool AreEqual(cEmployeeWorkLocation original, cEmployeeWorkLocation compareTo)
        {
            if (ReferenceEquals(original, compareTo))
            {
                // Handles nulls too
                return true;
            }
            return original.EmployeeWorkAddressId == compareTo.EmployeeWorkAddressId
                && original.EmployeeID == compareTo.EmployeeID
                && original.LocationID == compareTo.LocationID
                && original.StartDate.Equals(compareTo.StartDate)
                && original.EndDate.Equals(compareTo.EndDate)
                && original.Active.Equals(compareTo.Active)
                && original.Temporary.Equals(compareTo.Temporary)
                && original.CreatedOn.Equals(compareTo.CreatedOn)
                && original.CreatedBy == compareTo.CreatedBy
                && original.ModifiedOn.Equals(compareTo.ModifiedOn)
                && original.ModifiedBy == compareTo.ModifiedBy
                && original.Rotational == compareTo.Rotational
                && original.PrimaryRotational == compareTo.PrimaryRotational;

        }

    }
}