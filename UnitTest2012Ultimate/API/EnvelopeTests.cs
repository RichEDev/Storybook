using SpendManagementLibrary;

namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using SpendManagementApi.Controllers.Expedite;
    using SpendManagementApi.Controllers.Expedite.V1;
    using SpendManagementApi.Models.Responses.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;
    using Moq;
    using SpendManagementApi.Models.Common;
    using Spend_Management;

    [TestClass]
    public class EnvelopeTests
    {
        #region Properties

        private ControllerFactory<Envelope> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestEnvelopes _envelopes;
        private static EnvelopesV1Controller _envelopesController;

        #endregion Properties

        #region Init Methods

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
            _envelopes = new TestEnvelopes();
            _actionContext.SetEnvelopes(_envelopes);
            var repository = RepositoryFactory.GetRepository<Envelope>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<Envelope>(repository);
            _envelopesController = (EnvelopesV1Controller) _controllerFactory.GetController();
        }

        #endregion Init Methods

        #region AddEnvelope

        [TestMethod]
        [TestCategory("AddEnvelope")]
        public void EnvelopesController_AddEnvelope_CorrectEnvelopeGetsAdded()
        {
            var count = TestEnvelopes.Envelopes.Count;
            var envelope = CreateEnvelope();
            _controllerFactory.Post<EnvelopeResponse>(envelope);
            Assert.AreEqual(count + 1, TestEnvelopes.Envelopes.Count);
        }

        [TestMethod]
        [TestCategory("AddEnvelope")]
        public void EnvelopesController_AddEnvelope_ShouldErrorWhenInvalidId()
        {
            var envelope = CreateEnvelope();
            envelope.Id = 50;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Post<EnvelopeResponse>(envelope));
        }

        [TestMethod]
        [TestCategory("AddEnvelope")]
        public void EnvelopesController_AddEnvelope_ShouldErrorWhenInvalidType()
        {
            var envelope = CreateEnvelope();
            envelope.EnvelopeType = 9999;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Post<EnvelopeResponse>(envelope));
        }

        #endregion AddEnvelope

        #region AddEnvelopeBatch
        
        [TestMethod]
        [TestCategory("AddEnvelopeBatch")]
        public void EnvelopesController_AddEnvelopeBatch_CorrectEnvelopesAllGetAdded()
        {
            var count = TestEnvelopes.Envelopes.Count;
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope(),
                CreateEnvelope()
            };

            _envelopesController.PostBatch(list);
            Assert.AreEqual(count + 3, TestEnvelopes.Envelopes.Count);
        }

        [TestMethod]
        [TestCategory("AddEnvelopeBatch")]
        public void EnvelopesController_AddEnvelopeBatch_ShouldErrorWhenInvalidId()
        {
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope()
            };

            list[0].Id = 50;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.PostBatch(list));
        }

        [TestMethod]
        [TestCategory("AddEnvelopeBatch")]
        public void EnvelopesController_AddEnvelopeBatch_ShouldErrorWhenInvalidType()
        {
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope()
            };

            list[0].EnvelopeType = 999;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.PostBatch(list));
        }

        #endregion AddEnvelopeBatch

        #region EditEnvelope

        [TestMethod]
        [TestCategory("EditEnvelope")]
        public void EnvelopesController_EditEnvelope_CorrectEnvelopeGetsEdited()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            var envelope = CreateEnvelope();
            
            // make sure we use the ID of the target and change something.
            envelope.Id = targetEnv.EnvelopeId;
            envelope.Status = EnvelopeStatus.InStorage;
            
            _controllerFactory.Put<EnvelopeResponse>(envelope);
            Assert.AreEqual((int)targetEnv.Status, (int)envelope.Status);
        }

        [TestMethod]
        [TestCategory("EditEnvelope")]
        public void EnvelopesController_EditEnvelope_ShouldErrorWhenInvalidId()
        {
            var envelope = CreateEnvelope(); // Id is 0!
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeResponse>(envelope));

            envelope.Id = 9999; // id is not in envelopes
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeResponse>(envelope));
        }

        [TestMethod]
        [TestCategory("EditEnvelope")]
        public void EnvelopesController_EditEnvelope_ShouldErrorWhenInvalidStatus()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            var envelope = CreateEnvelope();
            envelope.Id = targetEnv.EnvelopeId;
            envelope.Status = (EnvelopeStatus)9999;
            ExceptionAssert.ThrowsException<Exception>(() => _controllerFactory.Post<EnvelopeResponse>(envelope));
        }


        [TestMethod]
        [TestCategory("EditEnvelope")]
        public void EnvelopesController_EditEnvelope_ShouldErrorWhenInvalidType()
        {
            var envelope = CreateEnvelope();
            envelope.EnvelopeType = 9999;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Post<EnvelopeResponse>(envelope));
        }

        #endregion EditEnvelope

        #region EditEnvelopeBatch

        [TestMethod]
        [TestCategory("EditEnvelopeBatch")]
        public void EnvelopesController_EditEnvelopeBatch_CorrectEnvelopesAllGetAdded()
        {
            var count = TestEnvelopes.Envelopes.Count;
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope(),
                CreateEnvelope()
            };

            _envelopesController.PostBatch(list);
            Assert.AreEqual(count + 3, TestEnvelopes.Envelopes.Count);
        }

        [TestMethod]
        [TestCategory("EditEnvelopeBatch")]
        public void EnvelopesController_EditEnvelopeBatch_ShouldErrorWhenInvalidId()
        {
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope()
            };

            list[0].Id = 50;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.PutBatch(list));
        }

        [TestMethod]
        [TestCategory("EditEnvelopeBatch")]
        public void EnvelopesController_EditEnvelopeBatch_ShouldErrorWhenInvalidStatus()
        {
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope()
            };

            list[0].Status = (EnvelopeStatus)999;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.PutBatch(list));
        }

        [TestMethod]
        [TestCategory("EditEnvelopeBatch")]
        public void EnvelopesController_EditEnvelopeBatch_ShouldErrorWhenInvalidType()
        {
            var list = new List<Envelope>
            {
                CreateEnvelope(),
                CreateEnvelope()
            };

            list[0].EnvelopeType = 999;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.PutBatch(list));
        }

        #endregion EditEnvelopeBatch

        #region PatchEnvelope

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueToAccount_CorrectEnvelopeGetsPatched()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            _envelopesController.IssueToAccount(targetEnv.EnvelopeId, GlobalTestVariables.AccountId);
            Assert.AreEqual(targetEnv.AccountId, GlobalTestVariables.AccountId);
            Assert.AreEqual((int)targetEnv.Status, (int)EnvelopeStatus.IssuedToAccount);
            Assert.IsNotNull(targetEnv.DateIssuedToClaimant);
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueToAccount_FailsOnBadEnvelope()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueToAccount(0, GlobalTestVariables.AccountId));
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueToAccount(99999, GlobalTestVariables.AccountId));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueToAccount_FailsOnBadAccount()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueToAccount(targetEnv.EnvelopeId, 0));
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueToAccount(targetEnv.EnvelopeId, 9999));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueBatchToAccount_SucceedsWithCorrectBatch()
        {
            Assert.AreEqual(TestEnvelopes.Envelopes[0].AccountId, null);
            Assert.AreEqual(TestEnvelopes.Envelopes[1].AccountId, null);

            var targetEnv = TestEnvelopes.Envelopes.First(); // the 3 envelopes in the list have the same batchcode
            var batchCode = targetEnv.EnvelopeNumber.Split(char.Parse("-"))[1];
            _envelopesController.IssueBatchToAccount(batchCode, GlobalTestVariables.AccountId);

            Assert.AreEqual(TestEnvelopes.Envelopes[0].AccountId, GlobalTestVariables.AccountId);
            Assert.AreEqual(TestEnvelopes.Envelopes[1].AccountId, GlobalTestVariables.AccountId);
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueBatchToAccount_FailsOnBadAccount()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            var batchCode = targetEnv.EnvelopeNumber.Split(char.Parse("-"))[1];
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueBatchToAccount(batchCode, 0));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_IssueBatchToAccount_FailsOnBadBatchCode()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.IssueBatchToAccount("ZZZ", GlobalTestVariables.AccountId));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_AttachToClaim_SucceedsWithCorrectClaim()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            targetEnv.AccountId = GlobalTestVariables.AccountId;

            var claim = cClaimObject.New(cClaimObject.Template(employeeid: _actionContext.EmployeeId));
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getClaimById(It.IsAny<int>())).Returns(claim);
            _actionContext.SetClaimsMock(claimsMock);

            _envelopesController.AttachToClaim(targetEnv.EnvelopeId, claim.claimid); 
            Assert.IsNotNull(targetEnv.ClaimId);
            Assert.IsNotNull(targetEnv.DateAssignedToClaim);
            Assert.AreEqual((int)targetEnv.Status, (int)EnvelopeStatus.AttachedToClaim);
            cClaimObject.TearDown(claim.claimid);

        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_AttachToClaim_FailsOnBadAccountId()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            targetEnv.AccountId = null; // no account Id should throw.
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.AttachToClaim(targetEnv.EnvelopeId, 1));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_AttachToClaim_FailsOnBadClaimId()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            targetEnv.AccountId = GlobalTestVariables.AccountId;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.AttachToClaim(targetEnv.EnvelopeId, 999));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_MarkeReceived_SucceedsWithCorrectId()
        {
            var claim = cClaimObject.New(cClaimObject.Template(employeeid: _actionContext.EmployeeId));
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getClaimById(It.IsAny<int>())).Returns(claim);
            claimsMock.Setup(c => c.UpdateClaimHistory(null, null, It.IsAny<int>(), null));
            _actionContext.SetClaimsMock(claimsMock);

            var emailsMock = new Mock<cEmailTemplates>(MockBehavior.Default);
            emailsMock.Setup(e => e.SendMessage(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int[]>(), null, false, null,null));
            _actionContext.SetEmailsMock(emailsMock);

            var employeesMock = new Mock<cEmployees>(MockBehavior.Default, _actionContext.AccountId);
            employeesMock.Setup(e => e.GetEmployeeById(It.IsAny<int>(), null)).Returns(cEmployeeObject.GetUTEmployeeTemplateObject());
            _actionContext.SetEmployeesMock(employeesMock);

            var signoffMock = new Mock<cGroups>(MockBehavior.Default, _actionContext.AccountId);
            signoffMock.Setup(s => s.GetGroupById(It.IsAny<int>())).Returns(new cGroup());
            _actionContext.SetGroupsMock(signoffMock);

            var targetEnv = TestEnvelopes.Envelopes.First();
            targetEnv.ClaimId = claim.claimid;
            _envelopesController.MarkReceived(targetEnv.EnvelopeId); 
            Assert.IsNotNull(targetEnv.DateReceived);
            Assert.AreEqual((int)targetEnv.Status, (int)EnvelopeStatus.ReceivedBySEL);
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_MarkReceived_FailsOnBadId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.MarkReceived(99999));
        }
        
        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_UpdateStatus_SucceedsWithCorrectStatus()
        {
            var targetEnv = TestEnvelopes.Envelopes.First();
            targetEnv.AccountId = GlobalTestVariables.AccountId;
            _envelopesController.UpdateStatus(targetEnv.EnvelopeId, EnvelopeStatus.ReadyForStorage);

            Assert.AreEqual((int)targetEnv.Status, (int)EnvelopeStatus.ReadyForStorage);
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_UpdateStatus_FailsOnBadEnvelopeId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.UpdateStatus(9999, EnvelopeStatus.AttachedToClaim));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_UpdateStatus_FailsOnBadStatusId()
        {
            ExceptionAssert.ThrowsException<HttpException>(() => _envelopesController.UpdateStatus(GlobalTestVariables.AccountId, (EnvelopeStatus)9999));
        }

        [TestMethod]
        [TestCategory("PatchEnvelope")]
        public void EnvelopesController_UpdatePhysicalState_FailsOnBadEnvelopeId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopesController.UpdatePhysicalState(9999, new List<int>{1}));
        }

        #endregion PatchEnvelope
        
        #region DeleteEnvelope

        [TestMethod]
        [TestCategory("DeleteEnvelope")]
        public void EnvelopesController_Delete_CorrectEnvelopeGetsDeleted()
        {
            var count = TestEnvelopes.Envelopes.Count;
            var targetEnv = TestEnvelopes.Envelopes.Last();
            _envelopesController.Delete(targetEnv.EnvelopeId);
            Assert.AreEqual(TestEnvelopes.Envelopes.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteEnvelope")]
        public void EnvelopesController_Delete_FailsOnBadEnvelope()
        {
            ExceptionAssert.ThrowsException<ApiException>(() => _envelopesController.Delete(999));
        }

        #endregion DeleteEnvelope

        #region Utils

        private static Envelope CreateEnvelope()
        {
            return new Envelope
            {
                Id = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                AccountId = null,
                ClaimId = null,
                EnvelopeNumber = "A-ABC-003",
                ClaimReferenceNumber = null,
                Status = EnvelopeStatus.Generated,
                EnvelopeType = 1,
                DateIssuedToClaimant = null,
                DateReceived = null,
                OverpaymentCharge = null,
                PhysicalState = new List<int>(),
                PhysicalStateProofUrl = null
            };
        }

        #endregion Utils
    }
}
