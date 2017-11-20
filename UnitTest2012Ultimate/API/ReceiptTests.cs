using Moq;
using SpendManagementLibrary;
using SpendManagementLibrary.Expedite;
using Spend_Management;

namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementApi.Controllers.Expedite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Controllers.Expedite.V1;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Types.Expedite;
    using Utilities;

    [TestClass]
    public class ReceiptTests
    {
        #region Properties

        private ControllerFactory<Receipt> _controllerFactory;
        private static TestActionContext _actionContext;

        private static TestReceipts _receipts;
        private static ReceiptsV1Controller _receiptsController;
        
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
            _receipts = new TestReceipts(GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            _actionContext.SetReceipts(_receipts);
            var repository = RepositoryFactory.GetRepository<Receipt>(new object[] {Moqs.CurrentUser(), _actionContext});
            _controllerFactory = new ControllerFactory<Receipt>(repository);
            _receiptsController = (ReceiptsV1Controller) _controllerFactory.GetController();

            var claim = cClaimObject.Template(1, employeeid: _actionContext.EmployeeId);
            var item = cExpenseItemObject.Template(1, 1, 1);
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose, _actionContext.AccountId);
            claimsMock.SetupAllProperties();
            claimsMock.Setup(c => c.getClaimById(It.IsAny<int>())).Returns(claim);
            claimsMock.Setup(c => c.getExpenseItemById(It.IsAny<int>())).Returns(item);
            _actionContext.SetClaimsMock(claimsMock);

            var employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            employee.EmployeeID = GlobalTestVariables.EmployeeId;
            var employeesMock = new Mock<cEmployees>(MockBehavior.Loose, _actionContext.AccountId);
            employeesMock.SetupAllProperties();
            employeesMock.Setup(e => e.GetEmployeeById(It.IsAny<int>(), null)).Returns(employee);
            _actionContext.SetEmployeesMock(employeesMock);
        }

        #endregion Init Methods

        #region AddReceipt

        [TestMethod]
        [TestCategory("AddReceipt")]
        public void ReceiptsController_AddReceipt_CorrectReceiptGetsAdded()
        {
            var receipt = CreateReceipt();
            var count = TestReceipts.Receipts.Count;

            _receiptsController.Post(receipt);
            Assert.AreEqual(count + 1, TestReceipts.Receipts.Count);

            receipt.Id = 0;
            count = TestReceipts.Orphans.Count;
            _receiptsController.PostOrphan(receipt);
            Assert.AreEqual(count + 1, TestReceipts.Orphans.Count);
        }

        [TestMethod]
        [TestCategory("AddReceipt")]
        public void ReceiptsController_AddReceipt_ShouldErrorWhenInvalidId()
        {
            var receipt = CreateReceipt();
            receipt.Id = 50;
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.Post(receipt));
            ExceptionAssert.ThrowsException<Exception>(() => _receiptsController.PostOrphan(receipt));
        }

        [TestMethod]
        [TestCategory("AddReceipt")]
        public void ReceiptsController_AddReceipt_ShouldErrorWithNoData()
        {
            var receipt = CreateReceipt();
            receipt.Data = null;
            ExceptionAssert.ThrowsException<Exception>(() => _receiptsController.Post(receipt));
            ExceptionAssert.ThrowsException<Exception>(() => _receiptsController.PostOrphan(receipt));
        }

        #endregion AddReceipt

        #region PatchReceipt

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimLine_Success()
        {
            // TestReceipts.Receipts[0] is assigned to the GlobalTestVars EmployeeId initially.
            _receiptsController.AttachToClaimLine(1, 1);
            var receipt = TestReceipts.Receipts.First();
            Assert.IsNotNull(receipt.OwnershipInfo);
            Assert.IsNull(receipt.OwnershipInfo.EmployeeId);
            Assert.IsNotNull(receipt.OwnershipInfo.ClaimLines);
            Assert.AreEqual(receipt.OwnershipInfo.ClaimLines.Count, 1);
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimLine_FailsInvalidId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaimLine(999, 1));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimLine_FailsInvalidClaimLineId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaimLine(1, 999));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToMultipleClaimLine_Success()
        {
            ReceiptsController_LinkToClaimLine_Success();
            _receiptsController.AttachToClaimLine(1, 2);
            Assert.AreEqual(TestReceipts.Receipts.First().OwnershipInfo.ClaimLines.Count, 2);
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_UnlinkFromClaimLine_Success()
        {
            TestReceipts.Receipts.First().OwnershipInfo = new ReceiptOwnershipInfo
            {
                ClaimId = null,
                EmployeeId = null,
                ClaimLines = new List<int> { 1, 2 }
            };

            _receiptsController.DetatchFromClaimLine(1, 2);
            Assert.AreEqual(TestReceipts.Receipts.First().OwnershipInfo.ClaimLines.Count, 1);
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_UnlinkFromClaimLine_FailureBadClaimLine()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.DetatchFromClaimLine(1, 9999));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaim_Success()
        {
            var claim = cClaimObject.New(cClaimObject.Template(employeeid: _actionContext.EmployeeId));
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getClaimById(It.IsAny<int>())).Returns(claim);
            _actionContext.SetClaimsMock(claimsMock);

            _receiptsController.AttachToClaim(1, 1);
            var receipt = TestReceipts.Receipts.First();
            Assert.IsNotNull(receipt.OwnershipInfo);
            Assert.IsNull(receipt.OwnershipInfo.EmployeeId);
            Assert.IsNull(receipt.OwnershipInfo.ClaimLines);
            Assert.AreEqual(receipt.OwnershipInfo.ClaimId, 1);
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaim_FailsInvalidId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaim(999, 1));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaim_FailsInvalidClaimId()
        {
            var claimsMock = new Mock<cClaims>(MockBehavior.Loose);
            claimsMock.Setup(c => c.getClaimById(999)).Returns((cClaim) null);
            _actionContext.SetClaimsMock(claimsMock);

            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaim(1, 999));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimant_Success()
        {
            var receipt = TestReceipts.Receipts[1]; // this one has a claimId set.
            _receiptsController.AttachToClaimant(receipt.ReceiptId, GlobalTestVariables.EmployeeId);
            Assert.IsNotNull(receipt.OwnershipInfo);
            Assert.IsNull(receipt.OwnershipInfo.ClaimId);
            Assert.IsNull(receipt.OwnershipInfo.ClaimLines);
            Assert.AreEqual(receipt.OwnershipInfo.EmployeeId, GlobalTestVariables.EmployeeId);
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimant_FailsInvalidId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaimant(999, GlobalTestVariables.EmployeeId));
        }

        [TestMethod]
        [TestCategory("LinkTo")]
        public void ReceiptsController_LinkToClaimant_FailsInvalidClaimantId()
        {
            ExceptionAssert.ThrowsException<InvalidDataException>(() => _receiptsController.AttachToClaimant(1, 99999));
        }

        #endregion PatchReceipt

        #region DeleteReceipt

        [TestMethod]
        [TestCategory("DeleteReceipt")]
        public void ReceiptsController_Delete_CorrectReceiptGetsDeleted()
        {
            var count = TestReceipts.Receipts.Count;
            var targetEnv = TestReceipts.Receipts.Last();
            _receiptsController.Delete(targetEnv.ReceiptId);
            Assert.AreEqual(TestReceipts.Receipts.Count, count - 1);
        }

        [TestMethod]
        [TestCategory("DeleteReceipt")]
        public void ReceiptsController_Delete_FailsOnBadReceipt()
        {
            ExceptionAssert.ThrowsException<Exception>(() => _receiptsController.Delete(999));
        }

        #endregion DeleteReceipt

        #region Utils

        private static Receipt CreateReceipt()
        {
            const string data = "This is a text Receipt";
            var bytes = new byte[data.Length * sizeof(char)];
            Buffer.BlockCopy(data.ToCharArray(), 0, bytes, 0, bytes.Length);
            
            return new Receipt
            {
                Id = 0,
                CreationMethod = ReceiptCreationMethod.UploadedByExpedite,
                Extension = ".txt",
                Data = Convert.ToBase64String(bytes),
                EmployeeId = null,
                ClaimId = null,
                ClaimLines = null,
                EnvelopeId = 1,
                ExpediteUserName = "Henson"
            };
        }

        #endregion Utils
    }
}
