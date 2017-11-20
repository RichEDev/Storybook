namespace EsrFileProcessingService
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using EsrFileProcessingService.ApiService;

    /// <summary>
    /// The import history API.
    /// </summary>
    public class ImportHistoryApi : IApi<ImportHistory>
    {
        /// <summary>
        /// The send.
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
        public List<ImportHistory> Send(int accountId, string trustVpd, List<ImportHistory> dataRecords)
        {
            if (accountId > 0)
            {
                var svc = new ApiRpcClient();
                var returnRecs = new List<ImportHistory>();

                returnRecs.AddRange(svc.UpdateImportHistory(accountId, dataRecords.ToArray()));

                return returnRecs;
            }

            return dataRecords;
        }
    }
}