using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EsrFileProcessingService
{
    using ApiLibrary.ApiObjects.ESR;

    using EsrFileProcessingService.ApiService;

    public class PositionApi : IApi<EsrPositionRecord>
    {
        public List<EsrPositionRecord> Send(int accountId, string trustVpd, List<EsrPositionRecord> dataRecords)
        {
            ApiRpcClient svc = new ApiRpcClient();
            List<EsrPositionRecord> returnRecs = new List<EsrPositionRecord>();

            returnRecs.AddRange(svc.UpdateEsrPositions(accountId, trustVpd, dataRecords.ToArray()));

            return returnRecs;
        }
    }
}