using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.API
{
    using System.Collections.Generic;

    using Moq;

    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    using Spend_Management;

    using UnitTest2012Ultimate.API.Stubs;
    using UnitTest2012Ultimate.API.Utilities;

    using SignoffType = SpendManagementApi.Common.Enums.SignoffType;
    using StageInclusionType = SpendManagementApi.Common.Enums.StageInclusionType;

    [TestClass]
    public class VehiclesTests
    {
        private ControllerFactory<Vehicle> controllerFactory;

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
            var repository =
                RepositoryFactory.GetRepository<Vehicle>(new object[] { currentUser.Object, _actionContext });
            controllerFactory = new ControllerFactory<Vehicle>(repository);
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void VehiclesController_Get_ShouldReturnNotNullOwnerId()
        {
            int vehicleId = 0;
            try
            {
                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);

                var repository = RepositoryFactory.GetRepository<Vehicle>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<Vehicle>(repository);

                Vehicle vehicle = RequestStubCreator<Vehicle>.GetValidEmployeeCar();

                VehicleResponse postResponse = controllerFactory.Post<VehicleResponse>(vehicle, true);

                vehicleId = postResponse.Item.Id;

                VehicleResponse response = controllerFactory.Get<VehicleResponse>(vehicleId, true);
                vehicle = response.Item;
                Assert.AreNotEqual(vehicle.EmployeeId, null);
            }
            finally
            {
                controllerFactory.Delete<VehicleResponse>(vehicleId, true);
            }
        }
    }
}
