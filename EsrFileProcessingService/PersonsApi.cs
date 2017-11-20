using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsrFileProcessingService
{
    using ApiLibrary.ApiObjects.ESR;

    using EsrFileProcessingService.ApiService;

    public class PersonsApi : IApi<EsrPersonRecord>
    {
        public List<EsrPersonRecord> Send(int accountId, string trustVpd, List<EsrPersonRecord> dataRecords)
        {
            ApiRpcClient svc = new ApiRpcClient();
            List<EsrPersonRecord> returnRecs = new List<EsrPersonRecord>();

            returnRecs.AddRange(svc.UpdateEsrPersonRecords(accountId, trustVpd, dataRecords.ToArray()));

            return returnRecs;
        }
    }
}
