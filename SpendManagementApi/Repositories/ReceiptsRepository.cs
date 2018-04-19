namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Web.Http;

    using BusinessLogic.FilePath;

    using Spend_Management;

	using SpendManagementApi.Common;
	using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions;

    using Claim = SpendManagementApi.Models.Types.Claim;
    using Receipts = SpendManagementLibrary.Expedite.Receipts;

    /// <summary>
    /// Manages data access for <see cref="Receipt">Receipt</see>.
    /// </summary>
    internal class ReceiptsRepository : BaseRepository<Receipt>, ISupportsActionContext
    {
        private readonly IActionContext _actionContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="ReceiptsRepository"/> class.
        /// </summary>
        /// <param name="user">
        /// Current user.
        /// </param>
        /// <param name="actionContext">
        /// Action context.
        /// </param>
        public ReceiptsRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.ReceiptUrl)
        {
            this._actionContext = actionContext;
        }

        /// <summary>
        /// Gets a list of receipts for the expense Id
        /// </summary>
        /// <param name="id">
        /// The expense Id
        /// </param>
        /// <param name="requestUri">
        /// Current request Uri.
        /// </param>
        /// <param name="generateThumbnails">
        /// Whether the thumbnails should be generated
        /// </param>
        /// <returns>
        /// A list of <see cref="Receipt">Receipt</see> data
        /// </returns>
        public List<Receipt> GetReceiptsByExpenseId(int id, Uri requestUri, bool generateThumbnails)
        {
            CheckClaimOwnership(id);
            var result = new List<Receipt>();
            var accountId = this.User.AccountID;
            SpendManagementLibrary.Expedite.Receipts receiptData = new SpendManagementLibrary.Expedite.Receipts(accountId, this.User.EmployeeID);

            Host host = this.GetHost(new cAccounts());
            var urlPath = string.Format("{0}://{1}", requestUri.Scheme, host.HostnameDescription);

            var expenseItem = ActionContext.Claims.getExpenseItemById(id);
            var claim = this.ActionContext.Claims.getClaimById(expenseItem.claimid);
            var subCategory = this.ActionContext.SubCategories.GetSubcatById(expenseItem.subcatid);

            IList<SpendManagementLibrary.Expedite.Receipt> receipts = receiptData.GetByClaimLine(expenseItem, this.User, subCategory, claim);
            var globalFolderPaths = new GlobalFolderPaths();

            foreach (SpendManagementLibrary.Expedite.Receipt receipt in receipts)
            {
                var expenseReceipt = this.BuildReceiptData(generateThumbnails, receipt, accountId, urlPath, receiptData, new Receipt(), globalFolderPaths);

                result.Add(expenseReceipt);
            }

            return result;
        }

        /// <summary>
        /// Uploads a base64 encoded receipt.
        /// </summary>
        /// <param name="receiptObject">
        /// The <see cref="ReceiptRequest">Receipt</see> request.
        /// </param>
        /// <param name="requestUri"></param>
        /// <returns>
        /// <see cref="int">Id of the expense item for receipt to upload.</see>.
        /// </returns>
        public int UploadReceipt([FromBody] ReceiptRequest receiptObject, Uri requestUri)
        {
            List<Receipt> receipts = this.UploadReceiptWithFullReceiptReturn(receiptObject, requestUri);
            Receipt firstOrDefault = receipts.FirstOrDefault();
            return firstOrDefault?.Id ?? 0;
        }

        /// <summary>
        /// Uploads a base64 encoded receipt, and returns the uploaded receipt data.
        /// </summary>
        /// <param name="receiptObject">
        /// The <see cref="ReceiptRequest">Receipt</see> request.
        /// </param>
        /// <param name="requestUri"></param>
        /// <returns>
        /// A list of <see cref="Receipt">containing the uploaded receipt data</see>.
        /// </returns>
        public List<Receipt> UploadReceiptWithFullReceiptReturn([FromBody]ReceiptRequest receiptObject, Uri requestUri) 
        {
            if (receiptObject == null)
            {
                throw new ApiException(ApiResources.ApiErrorReceiptUploadInvalid, ApiResources.ApiErrorReceiptUploadMessage);
            }

            try
            {
                CheckClaimOwnership(receiptObject.ExpenseId);
                var receiptData = Convert.FromBase64String(receiptObject.Receipt);
                SpendManagementLibrary.Expedite.Receipts receipts = new SpendManagementLibrary.Expedite.Receipts(User.AccountID, this.User.EmployeeID);

                var receipt = new SpendManagementLibrary.Expedite.Receipt
                {
                    Extension = receiptObject.FileType,
                    CreatedBy = User.EmployeeID
                };

                SpendManagementLibrary.Expedite.Receipt addReceiptOutcome = receipts.AddReceipt(receipt, receiptData);
                receipts.LinkToClaimLine(addReceiptOutcome.ReceiptId, receiptObject.ExpenseId);

                List<Receipt> receiptDetails = this.GetReceiptById(receipt.ReceiptId, requestUri, new List<Receipt>(), new cAccounts());
                return receiptDetails;

            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.ApiErrorReceiptUploadInvalid, ApiResources.ApiErrorReceiptUploadMessage);
            }          
        }

       public override IList<Receipt> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Receipt Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a receipt by the receiptId
        /// </summary>
        /// <param name="receiptId"> The Id of the receipt to delete.</param>
        /// <returns>1 if successuly, 0 if exepction thrown</returns>
        public int Delete (int receiptId)
        {
            SpendManagementLibrary.Expedite.Receipts receiptData = new SpendManagementLibrary.Expedite.Receipts(User.AccountID, this.User.EmployeeID);

            try
            {
                receiptData.DeleteReceipt(receiptId, true);
            }
            catch (Exception)
            {
                return 0;
            }

            return 1;
        }


        /// <summary>
        /// Updates the claim history with the approver's reason for deleteing the receipt, then deletes the receipt
        /// </summary>
        /// <param name="receiptId">
        /// The Id of the receipt to delete.
        /// </param>
        /// <param name="expenseId">
        /// The expense Id the receipt belongs to.
        /// </param>
        /// <param name="reason">
        /// The approver's reason for the delete.
        /// </param>
        /// <returns>
        /// The outcome of the action. 1 if successuly, 0 if not
        /// </returns>
        public int DeleteReceiptWithApproverJustification(int receiptId, int expenseId, string reason)
        {
            cExpenseItem item = new ExpenseItemRepository(this.User, this.ActionContext).GetExpenseItem(expenseId);      
            Claim claim = new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);

            cClaim receiptClaim = claim.To(ActionContext);

            new ClaimRepository(User, ActionContext).UpdateClaimHistory(receiptClaim, $"Receipt {receiptId} deleted: {reason}", this.User.EmployeeID);

            return this.Delete(receiptId);
        }
     
        /// <summary>
        /// Checks claim ownership before proceeding
        /// </summary>
        /// <param name="id">
        /// The expenseId
        /// </param>   
        private void CheckClaimOwnership(int id)
        {
            var expenseItem = new ExpenseItemRepository(this.User, this.ActionContext).Get(id);
            new ClaimRepository(this.User, this.ActionContext).Get(expenseItem.ClaimId);
        }

       

        /// <summary>
        /// Gets a receipt by its Id
        /// </summary>
        /// <param name="receiptId">The Id of the receipt</param>
        /// <param name="requestUri">The Uri of the orginating request</param>
        /// <param name="receipts">A list of <see cref="Receipt">Receipt</see></param>
        /// <param name="accounts">An instance of <see cref="cAccounts">cAccounts</see></param>
        /// <returns></returns>
        private List<Receipt> GetReceiptById(int receiptId, Uri requestUri, List<Receipt> receipts, cAccounts accounts)
        {
            SpendManagementLibrary.Expedite.Receipts receiptData = new SpendManagementLibrary.Expedite.Receipts(User.AccountID, this.User.EmployeeID);
            SpendManagementLibrary.Expedite.Receipt receipt = receiptData.GetById(receiptId);

            Host host = this.GetHost(accounts);
            var urlPath = string.Format("{0}://{1}", requestUri.Scheme, host.HostnameDescription);
            var globalFolderPaths = new GlobalFolderPaths();

            Receipt expenseReceipt = this.BuildReceiptData(true, receipt, this.User.AccountID, urlPath, receiptData, new Receipt(), globalFolderPaths);
            receipts.Add(expenseReceipt);
            return receipts;
        }

        /// <summary>
        /// Determines the host
        /// </summary>
        /// <param name="accounts">An instance of <see cref="cAccounts">cAccounts</see></param>
        /// <returns>The <see cref="Host">Host</see></returns>
        private Host GetHost(cAccounts accounts)
        {
            var account = accounts.GetAccountByID(this.User.AccountID);
            Host host = HostManager.GetHost(account.HostnameIds.FirstOrDefault());
            return host;
        }

        /// <summary>
        /// Builds up the receipt data.
        /// </summary>
        /// <param name="generateThumbnails">Whether thumbnails need to be generated</param>
        /// <param name="receipt">The <see cref="SpendManagementLibrary.Expedite.Receipt">Receipt</see></param>
        /// <param name="accountId">The accountId</param>
        /// <param name="urlPath">The url path of the request</param>
        /// <param name="receiptData">The <see cref="Receipts">ReceiptData</see></param>
        /// <param name="expenseReceipt">The <see cref="Receipt">Receipt</see>API Type</param>
        /// <returns>The <see cref="Receipt">Receipt</see>API Type</returns>
        private Receipt BuildReceiptData(bool generateThumbnails, SpendManagementLibrary.Expedite.Receipt receipt, int accountId, string urlPath, Receipts receiptData, Receipt expenseReceipt, GlobalFolderPaths globalFolderPaths)
        {
            AttachedReceipt attachedReceipt = AttachedReceipt.FromReceipt(receipt, accountId);
            expenseReceipt.IsApprover = cAccessRoles.CanCheckAndPay(this.User.AccountID, this.User.EmployeeID);

            if (attachedReceipt.filename != null)
            {                      
                expenseReceipt.Id = receipt.ReceiptId;
                expenseReceipt.Extension = receipt.Extension;
             
                // Return generic receipt image path for security reasons.
                // This is to prevent the ability to change the file path to view other claimant's receipts
                expenseReceipt.ReceiptUrl = urlPath + "/static/icons/custom/128/file_unknown.png";          
                string mappedTempPath = globalFolderPaths.GetSingleFolderPath(accountId, FilePathType.Receipt) + accountId.ToString();
                var fullPath = $"{mappedTempPath}\\{receipt.ReceiptId}.{receipt.Extension}";

				if (attachedReceipt.extension == "pdf")
				{
					expenseReceipt.ReceiptData = Helper.ConvertFileToBase64(fullPath);					
				}
				else
				{
					Image receiptImage = receiptData.GetImageFromPath(fullPath);
					expenseReceipt.ReceiptData = receiptData.ConvertImageToBase64(receiptImage, receiptImage.RawFormat);
				}

				expenseReceipt.MimeHeader = attachedReceipt.mimeType;

                if (generateThumbnails)
                {
                    // Return generic receipt thumbnail image path for security reasons.
                    // This is to prevent the ability to change the file path to view other claimant's receipts
                    expenseReceipt.ReceiptThumbnailUrl = urlPath + "/static/icons/custom/file_unknown_thumbnail.png";

                    string receiptThumbnaildata = receiptData.GenerateThumbnailImageData(attachedReceipt.receiptid.ToString(), attachedReceipt.extension);
                    expenseReceipt.ReceiptThumbnailData = receiptThumbnaildata;
                }
            }
            else
            {
                expenseReceipt.ReceiptUrl = string.Empty;
            }
            return expenseReceipt;
        }
    }
}