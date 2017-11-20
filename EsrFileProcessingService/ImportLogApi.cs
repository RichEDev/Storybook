namespace EsrFileProcessingService
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Spend_Management;
    using EsrFileProcessingService.ApiService;

    /// <summary>
    /// The import log API.
    /// </summary>
    public class ImportLogApi : IApi<ImportLog>
    {
        /// <summary>
        /// Create / Update Import Log Item.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="dataRecords">
        /// The data records.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportLog> Send(int accountId, string trustVpd, List<ImportLog> dataRecords)
        {
            if (accountId > 0)
            {
                var svc = new ApiRpcClient();
                var returnRecs = new List<ImportLog>();

                returnRecs.AddRange(svc.UpdateImportLog(accountId, dataRecords.ToArray()));

                return returnRecs;
            }

            return dataRecords;
        }
    }
}