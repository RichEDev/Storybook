using SpendManagementApi.Models.Common;

namespace UnitTest2012Ultimate.API
{
    using System.IO;
    using System.Linq;
    using SpendManagementApi.Controllers.Expedite;
    using SpendManagementApi.Models.Responses.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;

    [TestClass]
    public class EnvelopeStatusTests
    {
        #region Properties

        private ControllerFactory<EnvelopeStatus> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestEnvelopes _envelopes;
        private static EnvelopeStatusesController _envelopeStatusesController;

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
            var repository = RepositoryFactory.GetRepository<EnvelopeStatus>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<EnvelopeStatus>(repository);
            _envelopeStatusesController = (EnvelopeStatusesController)_controllerFactory.GetController();
        }

        #endregion Init Methods

        #region AddEnvelopeStatus

        [TestMethod]
        [TestCategory("AddEnvelopeStatus")]
        public void EnvelopeStatusController_AddEnvelopeStatus_CorrectEnvelopeStatusGetsAdded()
        {
            var count = TestEnvelopes.Statuses.Count;
            var envelopeStatus = CreateEnvelopeStatus();
            _controllerFactory.Post<EnvelopeStatusResponse>(envelopeStatus);
            Assert.AreEqual(count + 1, TestEnvelopes.Statuses.Count);
        }

        #endregion AddEnvelopeStatus

        #region EditEnvelopeStatus

        [TestMethod]
        [TestCategory("EditEnvelopeStatus")]
        public void EnvelopesController_EditEnvelopeStatus_CorrectEnvelopeGetsEdited()
        {
            var targetStatus = TestEnvelopes.Statuses.First();
            var envelopeStatus = CreateEnvelopeStatus();
            
            // make sure we use the ID of the target and change something.
            envelopeStatus.Id = targetStatus.EnvelopeStatusId;
            envelopeStatus.Label = TestEnvelopes.Statuses.Last().Label;

            _controllerFactory.Put<EnvelopeStatusResponse>(envelopeStatus);
            Assert.AreEqual(targetStatus.EnvelopeStatusId, envelopeStatus.Id);
        }

        [TestMethod]
        [TestCategory("EditEnvelopeStatus")]
        public void EnvelopesController_EditEnvelopeStatus_ShouldErrorWhenInvalidId()
        {
            var envelopeStatus = CreateEnvelopeStatus(); // Id is 0!
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeStatusResponse>(envelopeStatus));

            envelopeStatus.Id = 9999; // id is not in statuses
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeStatusResponse>(envelopeStatus));
        }

        #endregion EditEnvelopeStatus

        #region DeleteEnvelopeStatus

        [TestMethod]
        [TestCategory("DeleteEnvelopeStatus")]
        public void EnvelopesController_Delete_CorrectEnvelopeStatusGetsDeleted()
        {
            var count = TestEnvelopes.Statuses.Count;
            var targetStatus = TestEnvelopes.Statuses.Last();
            _envelopeStatusesController.Delete(targetStatus.EnvelopeStatusId);
            Assert.AreEqual(TestEnvelopes.Statuses.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopeStatus")]
        public void EnvelopesController_Delete_FailsOnBadEnvelopeStatus()
        {
            ExceptionAssert.ThrowsException<ApiException>(() => _envelopeStatusesController.Delete(999));
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopeStatus")]
        public void EnvelopesController_Delete_FailsDueToBeingInUse()
        {
            // mae sure the status being deleted is in use.
            var status = TestEnvelopes.Statuses.First();
            var envelope = TestEnvelopes.Envelopes.First();
            envelope.Status = status;

            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopeStatusesController.Delete(status.EnvelopeStatusId));
        }

        #endregion DeleteEnvelopeStatus

        #region Utils

        private static EnvelopeStatus CreateEnvelopeStatus()
        {
            return new EnvelopeStatus
            {
                Id = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                Label = "New Label"
            };
        }

        #endregion Utils
    }
}
