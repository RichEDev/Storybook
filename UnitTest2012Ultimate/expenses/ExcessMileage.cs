using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using EsrGo2FromNhsWcfLibrary.Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Employees;
using Spend_Management;
using UnitTest2012Ultimate.DatabaseMock;
using Utilities.DistributedCaching;
using Address = SpendManagementLibrary.Addresses.Address;

namespace UnitTest2012Ultimate.expenses
{
    [TestClass]
    public class ExcessMileage
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        private List<string> _postcodeList = new List<string>();

            #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialise()
        {
            new GlobalVariables(GlobalVariables.ApplicationType.Service);
            cSubAccountObject.CreateSubAccount();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, EmployeeWorkAddresses.CacheArea, GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture));
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            cSubAccountObject.DeleteSubAccount();
            var cache = new Cache();
            cache.Delete(GlobalTestVariables.AccountId, EmployeeWorkAddresses.CacheArea, GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture));
        }


        #endregion

        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        public void ExcessMileageNoHomeAndOffice()
        {
            var currentUser = Moqs.CurrentUser();
            var car = new cCar();
            var homeLocationId = 0;
            var oldOffice = 0;
            var newSubaccs = new cAccountSubAccounts(GlobalTestVariables.AccountId);
            var currentOffice = 0;
            var result = Spend_Management.shared.code.Mileage.ExcessMileage.CalculateRelocationMileageDistance(car,
                currentUser, homeLocationId, oldOffice, newSubaccs.getSubAccountById(GlobalTestVariables.SubAccountId), currentOffice);
            Assert.IsTrue(result == 0);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        public void ExcessMileageNoHomeValidOffice()
        {
            
            var car = new cCar();
            
            var homeLocationId = 0;
            var oldOffice = GetAddressIdWithPostcode(0);
            var newSubaccs = new cAccountSubAccounts(GlobalTestVariables.AccountId);
            var currentOffice = GetAddressIdWithPostcode(1);
            var result = Spend_Management.shared.code.Mileage.ExcessMileage.CalculateRelocationMileageDistance(car,
                Moqs.CurrentUser(), homeLocationId, oldOffice, newSubaccs.getSubAccountById(GlobalTestVariables.SubAccountId), currentOffice);
            Assert.IsTrue(result == 0);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        public void ExcessMileageValidHomeValidOffice()
        {
            var currentUser = Moqs.CurrentUser();
            var car = new cCar();
            var homeLocationId = GetAddressIdWithPostcode(0);
            var oldOffice = GetAddressIdWithPostcode(1);
            var newSubaccs = new cAccountSubAccounts(GlobalTestVariables.AccountId);
            var currentOffice = GetAddressIdWithPostcode(2);
            var result = Spend_Management.shared.code.Mileage.ExcessMileage.CalculateRelocationMileageDistance(car,
                currentUser, homeLocationId, oldOffice, newSubaccs.getSubAccountById(GlobalTestVariables.SubAccountId), currentOffice);
            Assert.IsTrue(result != 0);
        }

        //[TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        //public void ExcessMileageGetCurrentOfficeWhenNotESR()
        //{
        //    var date = new DateTime(2014, 5, 1);
        //    var startdate = new DateTime(2014, 1, 1);
        //    var enddate = new DateTime(2014, 12, 31);
        //    var esrLocationIDs = new List<long>();
        //    var location = GetAddressIdWithPostcode(0);
        //    var employeeWorkAddresses = new List<object> {new cEmployeeWorkLocation(0, GlobalTestVariables.EmployeeId, location,
        //        startdate, enddate, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null, null)};

        //    var addressDb =
        //        Reader.MockReaderDataFromClassData(
        //            "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
        //            employeeWorkAddresses).AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", l => l.EmployeeWorkAddressId ).AddAlias<cEmployeeWorkLocation>("addressID", a => a.LocationID);
        //    var esrLocations = new List<object>
        //    {
        //        new AddressEsrAllocation(location, 1234),
        //        new AddressEsrAllocation(location, 4321)
        //    };
        //    var esrLocationDb =
        //        Reader.MockReaderDataFromClassData(
        //            "select addressId, esrlocationid  from AddressEsrAllocation WHERE EsrLocationID is not null",
        //            esrLocations);
        //    var database = Reader.NormalDatabase(new[] { addressDb, esrLocationDb });
        //    var workAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId, database.Object);
            
        //    var result = Spend_Management.shared.code.Mileage.ExcessMileage.GetCurrentOffice(date, workAddresses, null);
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.LocationID == location);
        //}


        //[TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        //public void ExcessMileageGetCurrentOfficeWhenUsingESR()
        //{
        //    var date = new DateTime(2014,5,1);
        //    var startdate = new DateTime(2014, 1, 1);
        //    var enddate = new DateTime(2014, 12, 31);
        //    var esrLocationIDs = new List<long> {1234, 4321};
        //    var location = GetAddressIdWithPostcode(0);
        //    var address = new cEmployeeWorkLocation(0, GlobalTestVariables.EmployeeId, location,
        //        startdate, enddate, true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var employeeWorkAddresses = new List<object> {address};

        //    var addressDb =
        //        Reader.MockReaderDataFromClassData(
        //            "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
        //            employeeWorkAddresses).AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", l => l.EmployeeWorkAddressId).AddAlias<cEmployeeWorkLocation>("addressID", a => a.LocationID);
        //    var esrLocations = new List<object>
        //    {
        //        new AddressEsrAllocation(location, 1234),
        //        new AddressEsrAllocation(location, 4321)
        //    };

        //    var esrLocationDb =
        //        Reader.MockReaderDataFromClassData(
        //            "select addressId, esrlocationid  from AddressEsrAllocation WHERE EsrLocationID is not null",
        //            esrLocations);

        //    var database = Reader.NormalDatabase(new[] { addressDb, esrLocationDb });
        //    var workAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId, database.Object);

        //    var result = Spend_Management.shared.code.Mileage.ExcessMileage.GetCurrentOffice(date, workAddresses, 4321);
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.LocationID == location);
        //}

        //[TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        //public void ExcessMileageGetCurrentOfficeWhenUsingESRAndHavingMultipleWorkAddressesWithTheSameLocation()
        //{
        //    var date = new DateTime(2014, 5, 1);

        //    var esrLocationIDs = new List<long> { 1234, 4321 };
        //    var location = GetAddressIdWithPostcode(0);
        //    var location2 = GetAddressIdWithPostcode(1);
        //    var address = new cEmployeeWorkLocation(0, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 1, 1), new DateTime(2014, 1, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address2 = new cEmployeeWorkLocation(1, GlobalTestVariables.EmployeeId, location2,
        //        new DateTime(2014, 2, 1), new DateTime(2014, 2, 28), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address3 = new cEmployeeWorkLocation(2, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 3, 1), new DateTime(2014, 3, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address4 = new cEmployeeWorkLocation(3, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 4, 1), new DateTime(2014, 12, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var employeeWorkAddresses = new List<object> { address, address2, address3, address4 };

        //    var addressDb =
        //        Reader.MockReaderDataFromClassData(
        //            "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
        //            employeeWorkAddresses).AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", l => l.EmployeeWorkAddressId).AddAlias<cEmployeeWorkLocation>("addressID", a => a.LocationID);
        //    var esrLocations = new List<object>
        //    {
        //        new AddressEsrAllocation(location, 4321),
        //        new AddressEsrAllocation(location2, 4321)
        //    };

        //    var esrLocationDb =
        //        Reader.MockReaderDataFromClassData(
        //            "select addressId, esrlocationid  from AddressEsrAllocation WHERE EsrLocationID is not null",
        //            esrLocations);

        //    var database = Reader.NormalDatabase(new[] { addressDb, esrLocationDb });
        //    var workAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId, database.Object);

        //    var result = Spend_Management.shared.code.Mileage.ExcessMileage.GetCurrentOffice(date, workAddresses, 4321);
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.LocationID == location);
        //}


        //[TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("ExcessMileage")]
        //public void ExcessMileageGetPreviousOfficeWhenUsingESRAndHavingMultipleWorkAddressesWithTheSameLocation()
        //{
        //    var date = new DateTime(2014, 5, 1);

        //    var esrLocationIDs = new List<long> { 1234, 4321 };
        //    var location = GetAddressIdWithPostcode(0);
        //    var location2 = GetAddressIdWithPostcode(1);
        //    var address = new cEmployeeWorkLocation(0, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 1, 1), new DateTime(2014, 1, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address2 = new cEmployeeWorkLocation(1, GlobalTestVariables.EmployeeId, location2,
        //        new DateTime(2014, 2, 1), new DateTime(2014, 6, 28), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address3 = new cEmployeeWorkLocation(2, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 3, 1), new DateTime(2014, 3, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var address4 = new cEmployeeWorkLocation(3, GlobalTestVariables.EmployeeId, location,
        //        new DateTime(2014, 4, 1), new DateTime(2014, 12, 31), true, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, null,
        //        esrLocationIDs);
        //    var employeeWorkAddresses = new List<object> { address, address2, address3, address4 };

        //    var addressDb =
        //        Reader.MockReaderDataFromClassData(
        //            "SELECT EmployeeWorkAddressId, EmployeeID, AddressID,  StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM EmployeeWorkAddresses WHERE (employeeID = @employeeid)",
        //            employeeWorkAddresses).AddAlias<cEmployeeWorkLocation>("EmployeeWorkAddressId", l => l.EmployeeWorkAddressId).AddAlias<cEmployeeWorkLocation>("addressID", a => a.LocationID);
        //    var esrLocations = new List<object>
        //    {
        //        new AddressEsrAllocation(location, 4321),
        //        new AddressEsrAllocation(location2, 1234)
        //    };

        //    var esrLocationDb =
        //        Reader.MockReaderDataFromClassData(
        //            "select addressId, esrlocationid  from AddressEsrAllocation WHERE EsrLocationID is not null",
        //            esrLocations);

        //    var database = Reader.NormalDatabase(new[] { addressDb, esrLocationDb });
        //    var workAddresses = new EmployeeWorkAddresses(GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId, database.Object);
        //    var currentOffice = Spend_Management.shared.code.Mileage.ExcessMileage.GetCurrentOffice(date, workAddresses, 4321);
        //    Assert.IsNotNull(currentOffice);
        //    var result = Spend_Management.shared.code.Mileage.ExcessMileage.GetOldOfficeId(workAddresses, currentOffice, 4321);
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result == location2);
        //}






        private int GetAddressIdWithPostcode(int index)
        {
            var currentUser = Moqs.CurrentUser();
            var addresses = new Addresses(currentUser.AccountID);
            var addressList = addresses.Search("e", 232);
            var count = 0;
            foreach (Address address in addressList.Where(address => !string.IsNullOrEmpty(address.Postcode)))
            {
                if (count >= index && !_postcodeList.Contains(address.Postcode))
                {
                    _postcodeList.Add(address.Postcode);
                    return address.Identifier;    
                }

                count ++;
            }

            return 0;
        }
    }

     public class AddressEsrAllocation
    {
         public AddressEsrAllocation(int addressId, long esrlocationid)
         {
             this.addressId = addressId;
             this.esrlocationid = esrlocationid;
         }
         public int addressId { get; set; }
         public long esrlocationid { get; set; }
    }
}
