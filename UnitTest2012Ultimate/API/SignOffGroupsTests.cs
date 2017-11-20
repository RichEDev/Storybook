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
    public class SignOffGroupsTests
    {
        private ControllerFactory<SignOffGroup> controllerFactory;
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
            var currentUser = Moqs.CurrentUser();
            var accounts = new Mock<cAccounts>();
            accounts.Setup(x => x.GetAccountByID(It.IsAny<int>())).Returns(currentUser.Account);
            _actionContext.SetAccountsMock(accounts);
            var repository = RepositoryFactory.GetRepository<SignOffGroup>(new object[] { currentUser, _actionContext });
            controllerFactory = new ControllerFactory<SignOffGroup>(repository);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenIdNotZero()
        {
            SignOffGroup group = new SignOffGroup();
            group.GroupId = 100;
            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameMessage)]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvalidGroupName()
        {
            SignOffGroup group = new SignOffGroup();
            group.GroupId = 0;
            group.GroupName = string.Empty;
            SignOffGroupResponse response = controllerFactory.Post<SignOffGroupResponse>(group);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidNameMessage);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Group with name specified already exists.")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenGroupWithNameExists()
        {
            SignOffGroup group = new SignOffGroup();
            group.GroupId = 0;
            group.GroupName = "TestGroup";
            group.Stages = null;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns(dbGroup.Object);
            _actionContext.SetGroupsMock(groups);
            
            SignOffGroupResponse response = controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "'When to Include' of the last stage must be set to 'Always'.")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenIncludeTypeOfLastStageNotAlways()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.GroupName = "TestGroup";
            group.Stages[0].StageInclusionType = SpendManagementApi.Common.Enums.StageInclusionType.ClaimTotalBelow;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "'Involvement' of the last stage must be set to 'User is to check claim'.")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvolvementOfLastStageIsNotUserIsToCheckClaim()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.GroupName = "TestGroup";
            group.Stages[0].StageInclusionType = SpendManagementApi.Common.Enums.StageInclusionType.Always;
            group.Stages[0].Notify = Notify.JustNotifyUserOfClaim;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "The last stage of a group cannot be skipped if the approver is on holiday.")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenHolidayProvisionIsSetToSkipWhenApproverIsOnHolidayForLastStage()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.GroupName = "TestGroup";
            group.Stages[0].StageInclusionType = SpendManagementApi.Common.Enums.StageInclusionType.Always;
            group.Stages[0].Notify = Notify.UserIsToCheckClaim;
            group.Stages[0].OnHolidayProvision = HolidayProvision.SkipStage;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Valid IncludeId must be provided. Refer to StageInclusionType for valid values. Sign off stages provided have not been saved")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvalidStageInclusionType()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.Stages[0].IncludeId = 11;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }
        
        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Valid HolidayTypeId must be provided. Refer to SignOffTyoe for valid values. Sign off stages provided have not been saved")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvalidHolidayTypeId()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.Stages[0].HolidayType = (SignoffType)11;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Valid OnHolidayId must be provided. Refer to HolidayProvision for valid values. Sign off stages provided have not been saved")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvalidOnHolidayId()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.Stages[0].OnHolidayProvision = (HolidayProvision)11;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Valid NotifyId must be provided. Refer to Notify for valid values. Sign off stages provided have not been saved")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenInvalidNotifyId()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.Stages[0].Notify = (Notify)11;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Group Description is a required field")]
        public void SignOffGroupsController_Post_ShouldReturnErrorWhenMissingGroupDescription()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 0;
            group.Description = string.Empty;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            groups.Setup(g => g.GetGroupById(10)).Returns<SignOffGroup>(null);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "This signoff group cannot currently be amended as there are one or more claims in the approval process relating to this signoff group.")]
        public void SignOffGroupsController_Put_ShouldReturnErrorWhenClaimCountGreaterThanZero()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
            group.GroupId = 10;

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> dbGroup = new Mock<cGroup>(MockBehavior.Strict);
            dbGroup.SetupAllProperties();

            groups.Setup(g => g.GetGroupById(10)).Returns(dbGroup.Object);
            groups.Setup(g => g.getGroupByName(It.IsAny<string>())).Returns<SignOffGroup>(null);
            groups.Setup(g => g.getCountOfClaimsInProcessByGroupID(It.IsAny<int>())).Returns(2);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Put<SignOffGroupResponse>(group);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "No record available for specified id")]
        public void SignOffGroupsController_Put_ShouldReturnErrorWhenInvalidGroupId()
        {
            SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            groups.Setup(g => g.GetGroupById(10)).Returns<cGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Put<SignOffGroupResponse>(group);
        }



        [TestMethod]
        [TestCategory("EndToEnd")]
        public void SignOffGroupsController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            SignOffGroupResponse response = null;
            try
            {
                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);

                var repository = RepositoryFactory.GetRepository<SignOffGroup>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<SignOffGroup>(repository);

                SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
                group.GroupId = 0;

                response = controllerFactory.Post<SignOffGroupResponse>(group);

                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(group, response.Item);
            }
            finally
            {
                controllerFactory.Delete<SignOffGroupResponse>(response.Item.GroupId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void SignOffGroupsController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            SignOffGroupResponse response = null;
            SignOffGroup original = null;
            try
            {
                var currentUser = Moqs.CurrentUser();

                var repository = RepositoryFactory.GetRepository<SignOffGroup>(new object[] { currentUser, null });
                controllerFactory = new ControllerFactory<SignOffGroup>(repository);

                SignOffGroup group = RequestStubCreator<SignOffGroup>.GetValidSignOffGroup();
                group.GroupId = 0;

                response = controllerFactory.Post<SignOffGroupResponse>(group);

                original = response.Item;
                original.GroupName = "Test Group Name 2";
                original.OneClickAuthorization = false;
                original.Stages[0].Amount = 100;
                original.Stages[0].ApproveHigherLevelsOnly = true;
                original.Stages[0].ClaimantMail = false;
                original.Stages[0].DisplayDeclaration = false;
                original.Stages[0].ExtraLevels = 10;
                original.Stages[0].HolidayId = 2;
                original.Stages[0].HolidayType = (SignoffType)2;
                original.Stages[0].IncludeId = 2;
                original.Stages[0].StageInclusionType = (StageInclusionType)1;
                original.Stages[0].Notify = (Notify)2;
                original.Stages[0].OnHolidayProvision = (HolidayProvision)1;
                original.Stages[0].Relid = 2;
                original.Stages[0].SendMail = true;
                original.Stages[0].SignOffId = 2;
                original.Stages[0].SignOffStage = 1;
                original.Stages[0].SignOffType = SignoffType.Employee;
                original.Stages[0].SingleSignOff = true;

                response = controllerFactory.Put<SignOffGroupResponse>(original);
                Assert.AreEqual(ApiStatusCode.Success, response.ResponseInformation.Status);
                Assert.AreEqual(original, response.Item);
            }
            finally
            {
                controllerFactory.Delete<SignOffGroupResponse>(original.GroupId);
            }
        }
    }
}
