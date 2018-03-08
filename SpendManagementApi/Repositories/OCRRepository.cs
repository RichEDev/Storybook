namespace SpendManagementApi.Repositories
{
    using System.Web.Http;

    using SpendManagementLibrary.Receipts;

    using SpendManagementApi.Models.Requests;
    using ApiProcessedReceipt = SpendManagementApi.Models.Types.ProcessedReceipt;

    /// <summary>
    /// OCRRepository manages data access for ProcessedReceipt.
    /// </summary>
    internal class OCRRepository 
    {
        /// <summary>
        /// Save a <see cref="ApiProcessedReceipt"/>
        /// </summary>
        /// <param name="processedReceipt">The <see cref="ApiProcessedReceipt"/></param>
        /// <returns>Success of the posting of the data</returns>
        public int Post([FromBody] ProcessedReceiptRequest processedReceipt)
        {
            var processedReceipts = new ProcessedReceipts(processedReceipt.AccountId);
            var returnVal = processedReceipts.PostReceiptData(processedReceipt.To(processedReceipt));

            return returnVal;
        }

        /// <summary>
        /// Reset a <see cref="ApiProcessedReceipt" is in/>
        /// </summary>
        /// <param name="accountId">The id of the account the <see cref="ApiProcessedReceipt" is in/></param>
        /// <param name="walletReceiptId">The wallet receipt in the <see cref="ApiProcessedReceipt"/></param>
        /// <returns>Success of the posting of the data</returns>
        public int Post(int accountId, int walletReceiptId)
        {
            var processedReceipts = new ProcessedReceipts(accountId);
            processedReceipts.ResetReceipt(walletReceiptId);
            return 0;
        }

        /// <summary>
        /// Get a <see cref="ApiProcessedReceipt"/>
        /// </summary>
        /// <param name="accountId">The id of the account the <see cref="ApiProcessedReceipt"/> is in</param>
        /// <param name="walletReceiptId">The wallet receipt in the <see cref="ApiProcessedReceipt"/></param>
        /// <returns>A <see cref="ApiProcessedReceipt"/></returns>
        public ApiProcessedReceipt Get(int accountId, int walletReceiptId)
        {
            var processedReceipts = new ProcessedReceipts(accountId);
            var processedReceipt = processedReceipts.GetProcessedReceipt(walletReceiptId);

            return new ApiProcessedReceipt().From(processedReceipt);
        }
    }
}