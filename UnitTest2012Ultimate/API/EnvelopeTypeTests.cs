using SpendManagementApi.Models.Common;

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
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;

    [TestClass]
    public class EnvelopeTypeTests
    {
        #region Properties

        private ControllerFactory<EnvelopeType> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestEnvelopes _envelopes;
        private static EnvelopeTypesV1Controller _envelopeTypesController;

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
            var repository = RepositoryFactory.GetRepository<EnvelopeType>(new object[] { Moqs.CurrentUser(), _actionContext });
            _controllerFactory = new ControllerFactory<EnvelopeType>(repository);
            _envelopeTypesController = (EnvelopeTypesV1Controller)_controllerFactory.GetController();
        }

        #endregion Init Methods

        #region AddEnvelopeType

        [TestMethod]
        [TestCategory("AddEnvelopeType")]
        public void EnvelopeTypeController_AddEnvelopeType_CorrectEnvelopeTypeGetsAdded()
        {
            var count = TestEnvelopes.Types.Count;
            var envelopeType = CreateEnvelopeType();
            _controllerFactory.Post<EnvelopeTypeResponse>(envelopeType);
            Assert.AreEqual(count + 1, TestEnvelopes.Types.Count);
        }

        #endregion AddEnvelopeType

        #region EditEnvelopeType

        [TestMethod]
        [TestCategory("EditEnvelopeType")]
        public void EnvelopesController_EditEnvelopeType_CorrectEnvelopeGetsEdited()
        {
            var targetType = TestEnvelopes.Types.First();
            var envelopeType = CreateEnvelopeType();
            
            // make sure we use the ID of the target and change something.
            envelopeType.Id = targetType.EnvelopeTypeId;
            envelopeType.Label = TestEnvelopes.Types.Last().Label;

            _controllerFactory.Put<EnvelopeTypeResponse>(envelopeType);
            Assert.AreEqual(targetType.EnvelopeTypeId, envelopeType.Id);
        }

        [TestMethod]
        [TestCategory("EditEnvelopeType")]
        public void EnvelopesController_EditEnvelopeType_ShouldErrorWhenInvalidId()
        {
            var envelopeType = CreateEnvelopeType(); // Id is 0!
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeTypeResponse>(envelopeType));

            envelopeType.Id = 9999; // id is not in Typees
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _controllerFactory.Put<EnvelopeTypeResponse>(envelopeType));
        }

        #endregion EditEnvelopeType

        #region DeleteEnvelopeType

        [TestMethod]
        [TestCategory("DeleteEnvelopeType")]
        public void EnvelopesController_Delete_CorrectEnvelopeTypeGetsDeleted()
        {
            var count = TestEnvelopes.Types.Count;
            var targetType = TestEnvelopes.Types.Last();
            _envelopeTypesController.Delete(targetType.EnvelopeTypeId);
            Assert.AreEqual(TestEnvelopes.Types.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopeType")]
        public void EnvelopesController_Delete_FailsOnBadEnvelopeType()
        {
            ExceptionAssert.ThrowsException<ApiException>(() => _envelopeTypesController.Delete(999));
        }

        [TestMethod]
        [TestCategory("DeleteEnvelopeType")]
        public void EnvelopesController_Delete_FailsDueToBeingInUse()
        {
            // mae sure the Type being deleted is in use.
            var type = TestEnvelopes.Types.First();
            var envelope = TestEnvelopes.Envelopes.First();
            envelope.Type = type;

            ExceptionAssert.ThrowsException<InvalidDataException>(() => _envelopeTypesController.Delete(type.EnvelopeTypeId));
        }

        #endregion DeleteEnvelopeType

        #region Utils

        private static EnvelopeType CreateEnvelopeType()
        {
            return new EnvelopeType
            {
                Id = 0,
                EmployeeId = GlobalTestVariables.EmployeeId,
                Label = "New Label"
            };
        }

        #endregion Utils
    }
}
