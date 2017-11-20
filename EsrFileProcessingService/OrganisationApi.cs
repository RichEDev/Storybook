using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EsrFileProcessingService
{
    using ApiLibrary.ApiObjects.ESR;

    using EsrFileProcessingService.ApiService;

    public class OrganisationApi : IApi<EsrOrganisationRecord>
    {
        public List<EsrOrganisationRecord> Send(int accountId, string trustVpd, List<EsrOrganisationRecord> dataRecords)
        {
            ApiRpcClient svc = new ApiRpcClient();
            List<EsrOrganisationRecord> returnRecs = new List<EsrOrganisationRecord>();

            returnRecs.AddRange(svc.UpdateEsrOrganisations(accountId, trustVpd, dataRecords.ToArray()));

            return returnRecs;
        }
    }
}