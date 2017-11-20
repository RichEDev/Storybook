using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibrary.DataObjects.Base
{
    public enum ApiRecordType
    {
        EsrLocationRecord = 1,
        EsrOrganisationRecord,
        EsrPositionRecord,
        EsrPersonRecord,
        EsrAddressRecord,
        EsrVehicleRecord,
        EsrAssignmentRecord,
        EsrAssignmentCostingRecord,
        EsrPhoneRecord
    }
}
