using Syncfusion.XlsIO.Implementation.Collections;

namespace UnitTest2012Ultimate.API
{
    using System.IO;
    using System.Linq;
    using SpendManagementApi.Controllers.Expedite;
    using SpendManagementApi.Controllers.Expedite.V1;
    using SpendManagementApi.Models.Responses.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;

    [TestClass]
    public class EnvelopePhysicalStateTests
    {
        #region Properties

        private ControllerFactory<EnvelopePhysicalState> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestEnvelopes _envelopes;
        private static EnvelopePhysicalStatesV1Controller _envelopePhysicalStatesController;

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
            var repository = RepositoryFactory.GetRepository<EnvelopePhysicalState>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<EnvelopePhysicalState>(repository);
            _envelopePhysicalStatesController = (EnvelopePhysicalStatesV1Controller)_controllerFactory.GetController();
        }

        #endregion Init Methods

        #region AddEnvelopePhysicalState

        [TestMethod]
        [TestCategory("AddEnvelopePhysicalState")]
        public void EnvelopePhysicalStateController_AddEnvelopePhysicalState_CorrectEnvelopePhysicalStateGetsAdded()
        {
            var count = TestEnvelopes.States.Count;
            var envelopePhysicalState = CreateEnvelopePhysicalState();
            _controllerFactory.Post<EnvelopePhysicalStateResponse>(envelopePhysicalState);
            Assert.AreEqual(count + 1, TestEnvelopes.States.Count);
        }

        #endregion AddEnvelopePhysicalState

        #region EditEnvelopePhysicalState

        [TestMethod]
        [TestCategory("EditEnvelopePhysicalState")]
        public void EnvelopesController_EditEnvelopePhysicalState_CorrectEnvelopeGetsEdited()
        {
            var targetType = TestEnvelopes.States.First();
            var envelopePhysicalState = CreateEnvelopePhysicalState();
            
            // make sure we use the ID of the target and change something.
            envelopePhysicalState.EnvelopePhysicalStateId = targetType.EnvelopePhysicalStateId;
            envelopePhysicalState.Details = TestEnvelopes.States.Last().Details;

            _controllerFactory.Put<EnvelopePhysicalStateResponse>(envelopePhysicalState);
            Assert.AreEqual(targetType.EnvelopePhysicalStateId, envelopePhysicalState.EnvelopePhysicalStateId);
        }

        [TestMethod]
        [TestCategory("EditEnvelopePhysicalState")]
        public void EnvelopesController_EditEnvelopePhysicalState_ShouldErrorWhenInvalidId()
        {
            var envelopePhysicalState = CreateEnvelopePhysicalState(); // Id is 0!
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopePhysicalStateResponse>(envelopePhysicalState));

            envelopePhysicalState.EnvelopePhysicalStateId = 9999; // id is not in Typees
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopePhysicalStateResponse>(envelopePhysicalState));
        }

        #endregion EditEnvelopePhysicalState

        #region DeleteEnvelopePhysicalState

        [TestMethod]
        [TestCategory("DeleteEnvelopePhysicalState")]
        public void EnvelopesController_Delete_CorrectEnvelopePhysicalStateGetsDeleted()
        {
            var count = TestEnvelopes.States.Count;
            var targetState = TestEnvelopes.States.Last();
            _envelopePhysicalStatesController.Delete(targetState.EnvelopePhysicalStateId);
            Assert.AreEqual(TestEnvelopes.States.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopePhysicalState")]
        public void EnvelopesController_Delete_FailsOnBadEnvelopePhysicalState()
        {
            ExceptionAssert.ThrowsException<ApiException>(() => _envelopePhysicalStatesController.Delete(999));
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopePhysicalState")]
        public void EnvelopesController_Delete_FailsDueToBeingInUse()
        {
            // mae sure the Type being deleted is in use.
            var state = TestEnvelopes.States.First();
            var envelope = TestEnvelopes.Envelopes.First();
            envelope.PhysicalState = new SFArrayList<SpendManagementLibrary.Expedite.EnvelopePhysicalState> {state};

            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopePhysicalStatesController.Delete(state.EnvelopePhysicalStateId));
        }

        #endregion DeleteEnvelopePhysicalState

        #region Utils

        private static EnvelopePhysicalState CreateEnvelopePhysicalState()
        {
            return new EnvelopePhysicalState
            {
                EnvelopePhysicalStateId = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                Details = "New Label"
            };
        }

        #endregion Utils
    }
}
