
namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    using Spend_Management;

    using Stubs;
    using Utilities;

    using RangeType = SpendManagementApi.Common.Enums.RangeType;
    using ThresholdType = SpendManagementApi.Common.Enums.ThresholdType;
    using UnitOfMeasure = SpendManagementLibrary.MileageUOM;

    [TestClass]
    public class MileageCategoriesTests
    {
        private ControllerFactory<MileageCategory> controllerFactory;
        private static TestActionContext _actionContext;

        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            _actionContext = new TestActionContext();
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// The my class cleanup.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var currentUser = new Mock<ICurrentUser>();
            int accountId = GlobalTestVariables.AccountId;
            currentUser.SetupGet(u => u.AccountID).Returns(accountId);
            currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
            var repository =
                RepositoryFactory.GetRepository<MileageCategory>(new object[] { currentUser.Object, _actionContext });
            controllerFactory = new ControllerFactory<MileageCategory>(repository);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void MileageCategoriesController_Post_ShouldReturnErrorWhenIdNotZero()
        {
            MileageCategory mileageCategory = new MileageCategory();
            mileageCategory.MileageCategoryId = 100;
            controllerFactory.Post<MileageCategoryResponse>(mileageCategory);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameMessage)]
        public void MileageCategoriesController_Post_ShouldReturnErrorWhenInvalidMileageCategoryName()
        {
            MileageCategory mileageCategory = new MileageCategory();
            mileageCategory.MileageCategoryId = 0;
            mileageCategory.Label = string.Empty;
            controllerFactory.Post<MileageCategoryResponse>(mileageCategory);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MileageCategories_InvalidCarSizeMessage)]
        public void MileageCategoriesController_Post_ShouldReturnErrorWhenMileageCategoryWithCarSizeExists()
        {
            MileageCategory category = RequestStubCreator<MileageCategory>.GetValidMileageCategory();
            category.MileageCategoryId = 0;
            category.Label = "Test Car Size";

            Mock<cMileagecats> mileagecats = new Mock<cMileagecats>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cMileageCat> dbMileagecat = new Mock<cMileageCat>(MockBehavior.Strict);
            mileagecats.Setup(mc => mc.getMileageCatByName(It.IsAny<string>())).Returns(dbMileagecat.Object);
            _actionContext.SetMileageCategoriesMock(mileagecats);

            controllerFactory.Post<MileageCategoryResponse>(category);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MileageCategories_InvalidDateRangeMessage)]
        public void MileageCategoriesController_Put_ShouldReturnErrorWhenMileageDateRangeInvalid()
        {
            MileageCategoryResponse response = null;
            MileageCategory original = null;
            try
            {
                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);

                var repository =
                    RepositoryFactory.GetRepository<MileageCategory>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<MileageCategory>(repository);

                MileageCategory category = RequestStubCreator<MileageCategory>.GetValidMileageCategory();
                category.MileageCategoryId = 0;

                response = controllerFactory.Post<MileageCategoryResponse>(category);

                original = response.Item;
                original.CalculateNewJourneyTotal = false;
                original.Label = "CarSizeModified";
                original.Comment = "Comment 2";
                original.FinancialYearId = 2;
                original.NhsMileageCode = "Mileage Code 2";
                original.ThresholdType = ThresholdType.Journey;
                original.StartEngineSize = 10000;
                original.EndEngineSize = 20000;
                original.UnitOfMeasure = (int)SpendManagementLibrary.MileageUOM.Mile;

                original.DateRanges.Add(
                    new DateRange
                        {
                            //AccountId = GlobalTestVariables.AccountId,
                            DateRangeType = SpendManagementApi.Common.Enums.DateRangeType.Any
                            //EmployeeId = GlobalTestVariables.EmployeeId
                        });

                controllerFactory.Put<MileageCategoryResponse>(original);
            }
            finally
            {
                controllerFactory.Delete<MileageCategoryResponse>(original.MileageCategoryId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void MileageCategoriesController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            MileageCategoryResponse response = null;
            try
            {
                var repository = RepositoryFactory.GetRepository<MileageCategory>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<MileageCategory>(repository);

                MileageCategory category = RequestStubCreator<MileageCategory>.GetValidMileageCategory();
                category.MileageCategoryId = 0;

                response = controllerFactory.Post<MileageCategoryResponse>(category);

                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(category, response.Item);
            }
            finally 
            {
                controllerFactory.Delete<MileageCategoryResponse>(response.Item.MileageCategoryId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void MileageCategoriesController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            MileageCategoryResponse response = null;
            try
            {
                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);

                var repository = RepositoryFactory.GetRepository<MileageCategory>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<MileageCategory>(repository);

                MileageCategory category = RequestStubCreator<MileageCategory>.GetValidMileageCategory();
                category.MileageCategoryId = 0;

                response = controllerFactory.Post<MileageCategoryResponse>(category);

                MileageCategory original = response.Item;
                original.CalculateNewJourneyTotal = false;
                original.Label = "Car Size 2";
                original.Comment = "Comment 2";
                original.FinancialYearId = 2;
                original.NhsMileageCode = "Mileage Code 2";
                original.ThresholdType = ThresholdType.Journey;
                original.StartEngineSize = 10000;
                original.EndEngineSize = 20000;
                original.UnitOfMeasure = UnitOfMeasure.KM;
                original.DateRanges[0].DateRangeType = SpendManagementApi.Common.Enums.DateRangeType.Before;
                original.DateRanges[0].DateValue1 = new DateTime(2012, 1, 1);
                original.DateRanges[0].DateValue2 = null;
                original.DateRanges[0].Thresholds = new List<Threshold> { new Threshold() };
                original.DateRanges[0].Thresholds[0].RangeType = RangeType.LessThan;
                original.DateRanges[0].Thresholds[0].RangeValue1 = 10000;
                original.DateRanges[0].Thresholds[0].RangeValue2 = 20000;
                original.DateRanges[0].Thresholds[0].HeavyBulkyEquipmentRate = 50;
                original.DateRanges[0].Thresholds[0].Passenger1Rate = 50;
                original.DateRanges[0].Thresholds[0].PassengerXRate = 50;
                original.DateRanges[0].Thresholds[0].PencePerMileDiesel = 50;
                original.DateRanges[0].Thresholds[0].PencePerMileDieselEuroV = 50;
                original.DateRanges[0].Thresholds[0].PencePerMileElectric = 50;
                original.DateRanges[0].Thresholds[0].PencePerMileHybrid = 50;
                original.DateRanges[0].Thresholds[0].PencePerMileLpg = 50;
                original.DateRanges[0].Thresholds[0].PencePerMilePetrol = 50;
                original.DateRanges[0].Thresholds[0].VatAmountDiesel = 50;
                original.DateRanges[0].Thresholds[0].VatAmountDieselEuroV = 50;
                original.DateRanges[0].Thresholds[0].VatAmountElectric = 50;
                original.DateRanges[0].Thresholds[0].VatAmountHybrid = 50;
                original.DateRanges[0].Thresholds[0].VatAmountLpg = 50;
                original.DateRanges[0].Thresholds[0].VatAmountPetrol = 50;

                response = controllerFactory.Put<MileageCategoryResponse>(original);
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(original, response.Item);
            }
            finally
            {
                controllerFactory.Delete<MileageCategoryResponse>(response.Item.MileageCategoryId);
            }
        }
    }
}
