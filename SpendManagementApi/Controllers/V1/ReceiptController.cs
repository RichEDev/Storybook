

namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="Receipt">Receipt</see>.
    /// </summary>
    [RoutePrefix("Receipt")]
    [Version(1)]
    public class ReceiptV1Controller : BaseApiController<Receipt>
    {
        /// <summary>Gets all of the available end points from the <see cref="Receipt">Receipt</see> part of the API.</summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links();
        }

        /// <summary>
        /// Gets a list of receipts for the expense Id
        /// </summary>
        /// <param name="expenseId">
        /// The expense Id
        /// </param>
        /// <param name="generateThumbnails">Optional param to indicate if thumbnails are to be generated. False by default</param>
        /// <returns>The <see cref="ReceiptResponse">ReceiptResponse</see></returns>
        [HttpGet, Route("{expenseid:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ReceiptResponse Get([FromUri] int expenseId, bool generateThumbnails = false)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.List = ((ReceiptsRepository)this.Repository).GetReceiptsByExpenseId(expenseId, Request.RequestUri, generateThumbnails);
            return response;
        }

        /// <summary>
        /// Deletes a receipt by its Id.
        /// </summary>
        /// <param name="id"> The Id of the receipt to delete.</param>
        /// <returns>1 if successuly, 0 if exepction thrown</returns>
        [HttpDelete, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public NumericResponse Delete(int id)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ReceiptsRepository)this.Repository).Delete(id);
            return response;
        }

        /// <summary>
        ///  Uploads a base64 encoded receipt.
        /// </summary>
        /// <param name="receiptObject">
        /// Receipt object.
        /// </param>
        /// <returns>
        /// <see cref="NumericResponse">Id of the expense item for the upload receipt.</see>.
        /// </returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("UploadReceipt")]
        public NumericResponse UploadReceipt([FromBody] ReceiptRequest receiptObject)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ReceiptsRepository)this.Repository).UploadReceipt(receiptObject, Request.RequestUri);
            return response;
        }

        /// <summary>
        ///  Uploads a base64 encoded receipt and returns the receipt data for the uploaded receipt.
        /// </summary>
        /// <param name="receiptObject">
        /// <see cref="ReceiptRequest">ReceiptRequest</see> object.
        /// </param>
        /// <returns>
        /// <see cref="ReceiptResponse">ReceiptResponse</see> containing the uploaded receipt data.
        /// </returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("UploadReceiptWithFullReceiptReturn")]
        public ReceiptResponse UploadReceiptWithFullReceiptReturn([FromBody] ReceiptRequest receiptObject)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.List = ((ReceiptsRepository)this.Repository).UploadReceiptWithFullReceiptReturn(receiptObject, Request.RequestUri);
            return response;
        }

        /// <summary>
        /// Updates the claim history with the approver's reason for deleteing the receipt, then deletes the receipt.
        /// </summary>
        /// <param name="request">
        /// The <see cref="DeleteReceiptAsApproverRequest">DeleteReceiptAsApproverRequest</see>.
        /// </param>
        /// <returns>
        /// <see cref="NumericResponse">The outcome of the action. 1 if successuly, 0 if not.</see>.
        /// </returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("DeleteReceiptWithApproverJustification")]
        public NumericResponse DeleteReceiptWithApproverJustification([FromBody] DeleteReceiptAsApproverRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ReceiptsRepository)this.Repository).DeleteReceiptWithApproverJustification(request.ReceiptId, request.ExpenseId, request.DeleteReason);
            return response;
        }
    }
}