namespace SpendManagementApi.Controllers.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="ProcessedReceipt">processed wallet receipts</see>.
    /// </summary>
    [RoutePrefix("OCR")]
    [Version(1)]
    public class OCRV1Controller : BaseApiController<ProcessedReceipt>
    {
        /// <summary>
        /// Get a processed receipt
        /// </summary>
        /// <param name="accountId">The id of the account you want to get the receipt from</param>
        /// <param name="walletReceiptId">The id of the receipt</param>
        /// <returns>The receipt</returns>
        [HttpGet, Route("{accountId:int}/{walletReceiptId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public ProcessedReceiptResponse Get(int accountId, int walletReceiptId)
        {
            var response = this.InitialiseResponse<ProcessedReceiptResponse>();
            response.Item = new OCRRepository().Get(accountId, walletReceiptId);
            return response;
        }

        /// <summary>
        /// Post back the data from the receipt being processed
        /// </summary>
        /// <param name="processedReceipt">The processed receipt and its data</param>
        /// <returns>0 is successful</returns>
        [HttpPost, Route("")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public NumericResponse PostProcessedReceiptData([FromBody] ProcessedReceiptRequest processedReceipt)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = new OCRRepository().Post(processedReceipt);
            return response;
        }
    }
}