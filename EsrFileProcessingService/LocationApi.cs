using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EsrFileProcessingService
{
    using ApiLibrary.ApiObjects.ESR;

    using EsrFileProcessingService.ApiService;

    public class LocationApi : IApi<EsrLocationRecord>
    {
        public List<EsrLocationRecord> Send(int accountId, string trustVpd, List<EsrLocationRecord> dataRecords)
        {
            ApiRpcClient svc = new ApiRpcClient();
            List<EsrLocationRecord> returnRecs = new List<EsrLocationRecord>();

            returnRecs.AddRange(svc.UpdateEsrLocationRecords(accountId, trustVpd, dataRecords.ToArray()));

            return returnRecs;
        }
    }
}