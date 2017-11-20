namespace UnitTest2012Ultimate.API
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementApi.Controllers;
    using SpendManagementApi.Controllers.V1;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests.SelfRegistration;
    using SpendManagementApi.Models.Responses.SelfRegistration;

    [TestClass]
    public class SelfRegistrationTests
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
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

        [TestMethod]
        public void TestDUmmyDataForSelfRegistrationApi()
        {
            var response = new SelfRegistrationResponse();

            SelfRegistrationDummyDataPopulator.PopulateStandardDummyResponse(response);
            TestResponse(response);
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("SelfRegistration")]
        public void CreateValidDummyDataResponseFromValidInput()
        {
            var controller = new SelfRegistrationV1Controller();
            var request = new SelfRegistrationInitiatorRequest
                          {
                              EmailAddress = "paul.griffiths@software-europe.com",
                              Firstname = "Paul",
                              Password = "YouBetterNotHaveSeenThis",
                              Surname = "Griffiths",
                              Title = "Mr",
                              Username = "paul.griffiths"
                          };

            var response = controller.InitiateSelfRegistration(request);
            TestResponse(response);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void TestResponse(SelfRegistrationResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsTrue(response.AccessRoles.Count > 0);
            Assert.IsTrue(response.CarEngineTypes.Count > 0);
            Assert.IsTrue(response.CostCodes.Count > 0);
            Assert.IsTrue(response.Countries.Count > 0);
            Assert.IsTrue(response.Currencies.Count > 0);
            Assert.IsTrue(response.Departments.Count > 0);
            Assert.IsTrue(response.LineManagers.Count > 0);
            Assert.IsTrue(response.MileageUoMs.Count > 0);
            Assert.IsTrue(response.PostCodeAnywhereLicenseKey == "PA53-DM28-UK87-AP39");
            Assert.IsTrue(response.ProjectCodes.Count > 0);
            Assert.IsTrue(response.SignoffGroups.Count > 0);
            Assert.IsTrue(response.UserDefinedFields.Count > 0);
            Assert.IsTrue(response.ResponseInformation.Status == ApiStatusCode.Success);
            Assert.IsNull(response.ResponseInformation.Errors);
        }
    }
}
