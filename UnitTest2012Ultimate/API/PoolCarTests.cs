namespace UnitTest2012Ultimate.API
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spend_Management;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class PoolCarTests : BaseTests<Vehicle, VehicleResponse, cPoolCars>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("PoolCarsController")]
        public void RoundTrip()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                AddEditDeleteFullCycle(
                    item,
                    added =>
                    {
                        Assert.IsNotNull(added);
                        Assert.AreNotEqual(added.Id, 0);
                    },
                    toMod =>
                    {
                        toMod.Registration += LabelDescriptionMod;
                        return toMod;
                    },
                    modified => Assert.AreEqual(modified.Registration, Label + LabelDescriptionMod),
                    toDelete =>
                    {
                        InitialIds.Remove(toDelete.Id);
                        return toDelete.Id;
                    },
                    Assert.IsNull);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("PoolCarsController")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.Vehicles_MakeOfTheCarMustBeProvided)]
        public void AddPoolCarWithNoMakeFails()
        {
            var item = GenerateBasicWorkingItems();
            item.Make = null;

            try
            {
                // attempt to add the other
                AddWithAssertions(item);

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("PoolCarsController")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.Vehicles_ModelOfTheCarMustBeProvided)]
        public void AddPoolCarWithNoModelFails()
        {
            var item = GenerateBasicWorkingItems();
            item.Model = null;

            try
            {
                // attempt to add the other
                AddWithAssertions(item);

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("PoolCarsController")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.Vehicles_RegistrationNumberOfTheCarMustBeProvided)]
        public void AddPoolCarWithNoRegFails()
        {
            var item = GenerateBasicWorkingItems();
            item.Registration = null;

            try
            {
                // attempt to add the other
                AddWithAssertions(item);

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }


        #endregion Test Methods

        #region Utilities

        /// <summary>
        /// Returns and allowance with correct properties for AccountId, CurrencyId, EmployeeId, Label, and Description.
        /// The AllowanceRates property is set to null. The instance variables dal and intiialAllowanceIds are also set...
        /// </summary>
        /// <returns>A new Allowance object populated as per summary.</returns>
        private Vehicle GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.Cars.Select(x => x.carid).ToList();

            return new Vehicle
            {
                Id = 0,
                AccountId = User.AccountID,
                EmployeeId = User.EmployeeID,
                Make = "Ford",
                Model = "Fiesta",
                Approved = true,
                FuelType = 1,
                IsExemptFromHomeToLocationMileage = true,
                UnitOfMeasure = SpendManagementLibrary.MileageUOM.KM,
                Registration = Label,
                EngineSize = 2000,
                IsActive = true,
                CarUsageStartDate = DateTime.UtcNow,
                CarUsageEndDate = DateTime.UtcNow.AddYears(1),
                MileageCategoryIds = new List<int>()
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.Cars.Select(x => x.carid).ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.DeleteCar(id);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            newList.ForEach(id =>
            {
                var item = dal.GetCarByID(id);
                if (item != null &&
                    (item.registration == Label || item.registration == Label + LabelDescriptionMod))
                {
                    dal.DeleteCar(id);
                }
            });
        }

        #endregion Utilities
    }
}
