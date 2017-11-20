namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ReceiptResult = SpendManagementLibrary.Mobile.ReceiptResult;
    using UploadReceiptResult = SpendManagementLibrary.Mobile.UploadReceiptResult;

    /// <summary>
    /// The controller dealing with the saving and retrieval of receipts.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileReceiptsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Uploads a base64 encoded receipt from a mobile device.
        /// </summary>
        /// <param name="receiptObject">
        /// The receipt Object.
        /// </param>
        /// <returns>
        /// The ID of the expense item for the upload receipt.
        /// </returns>
        [HttpPost]
        [MobileAuth]
        [Route("mobile/receipts/save")]
        public UploadReceiptResult UploadReceipt([FromBody]ReceiptObject receiptObject)
        {
            UploadReceiptResult uploadResultMsg = new UploadReceiptResult { ReturnCode = this.ServiceResultMessage.ReturnCode, FunctionName = "UploadReceipt", MobileID = receiptObject.MobileExpenseID };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    byte[] bReceipt = Convert.FromBase64String(receiptObject.Receipt);

                    cMobileDevices clsdevices = new cMobileDevices(this.PairingKeySerialKey.PairingKey.AccountID);
                    clsdevices.saveMobileItemReceipt(receiptObject.MobileExpenseID, bReceipt);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.UploadReceipt():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return uploadResultMsg;
        }

        /// <summary>
        /// Gets the first receipt for the expense id parameter.
        /// </summary>
        /// <param name="expenseid">The expense to get the receipt for</param>
        /// <returns>A path to the receipt wrapped in a <see cref="ReceiptResult"/> object</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/receipts/{expenseid}")]
        public ReceiptResult GetReceiptById(int expenseid)
        {
            ReceiptResult result = new ReceiptResult
            {
                FunctionName = "GetReceiptByID",
                ReturnCode = this.ServiceResultMessage.ReturnCode
            };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    var accountId = this.PairingKeySerialKey.PairingKey.AccountID;
                    var receiptData = new SpendManagementLibrary.Expedite.Receipts(accountId, this.PairingKeySerialKey.PairingKey.EmployeeID);

                    // multiple receipts per expense item are now possible, until the mobile apps can be enhanced we'll just return the first receipt
                    AttachedReceipt receipt = receiptData.GetByClaimLine(expenseid)
                                                             .Select(r => AttachedReceipt.FromReceipt(r, accountId))
                                                             .FirstOrDefault() ?? new AttachedReceipt();

                    result.isApprover = cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);

                    if (receipt.filename != null)
                    {
                        Uri uri = this.Request.RequestUri;

                        cAccounts accounts = new cAccounts();
                        cAccount account = accounts.GetAccountByID(this.PairingKeySerialKey.PairingKey.AccountID);
                        Host host = HostManager.GetHost(account.HostnameIds.FirstOrDefault());

                        string fullPath = uri.Scheme + "://" + host.HostnameDescription +
                                          (receipt.filename.StartsWith("/") ? receipt.filename : "/" + receipt.filename);
                        result.Receipt = fullPath;
                        result.Message = "success";
                        result.mimeHeader = receipt.mimeType;
                    }
                    else
                    {
                        result.Receipt = string.Empty;
                        result.Message = "file not found";
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry(
                        "MobileAPI.GetReceiptByID():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey
                        + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }",
                        true,
                        System.Diagnostics.EventLogEntryType.Information,
                        cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return result;
        }
    }
}
