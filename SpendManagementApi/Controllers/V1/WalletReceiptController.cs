namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="WalletReceipt"> wallet receipts</see>.
    /// </summary>
    [RoutePrefix("WalletReceipt")]
    [Version(1)]
    public class WalletReceiptV1Controller : BaseApiController<WalletReceipt>
    {
        /// <summary>Gets all of the available end points from the <see cref="Receipt">Receipt</see> part of the API.</summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<Link> Options()
        {
            return this.Links();
        }

        /// <summary>
        /// Saves a wallet receipt
        /// </summary>
        /// <param name="receiptData">The wallet receipt data</param>
        /// <returns>The saved wallet receipt</returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("UploadReceipt")]
        public WalletReceiptResponse Post([FromBody] WalletReceiptRequest receiptData)
        {
            var response = this.InitialiseResponse<WalletReceiptResponse>();
            response.Item = ((WalletReceiptRepository)this.Repository).UploadReceipt(receiptData);
            return response;
        }

        /// <summary>
        /// Delete a wallet receipt
        /// </summary>
        /// <param name="id">The id of the wallet receipt</param>
        /// <returns>A response code based on the success of the deletion</returns>
        [HttpDelete]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("{id:int}")]
        public NumericResponse Delete(int id)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((WalletReceiptRepository)this.Repository).DeleteReceipt(id);
            return response;
        }

        /// <summary>
        /// Get a wallet receipt
        /// </summary>
        /// <param name="id">The id of the wallet receipt</param>
        /// <returns>The wallet receipt</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public WalletReceiptResponse Get(int id)
        {
            var response = this.InitialiseResponse<WalletReceiptResponse>();
            response.Item = ((WalletReceiptRepository)this.Repository).Get(id);
            return response;
        }

        /// <summary>
        /// Get a list of all your wallet receipts
        /// </summary>
        /// <returns>A list of wallet receipts</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IList<WalletReceipt> Get()
        {
            return ((WalletReceiptRepository)this.Repository).GetAll();
        }
    }
}