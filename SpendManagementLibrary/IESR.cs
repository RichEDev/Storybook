using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary.ESRTransferServiceClasses;

namespace SpendManagementLibrary
{
    public interface IESR
    {
        List<cESRElement> getESRElementsBySubcatID(int AccountID, int NHSTrustID, int SubcatID);
        cESRTrust GetESRTrustByVPD(string trustVPD);
        cESRTrust GetESRTrustByID(int AccountID, int NHSTrustID);
        [Obsolete("Taken out as the ESR File name is now attcahed to the export options on a cReportRequest", true)]
        string getESRInboundFilename(int accountid, int NHSTrustID);
        int checkExistenceOfESRAutomatedTemplate(int AccountID, int NHSTrustID);
        void processESROutboundImport(int AccountID, int TemplateID, byte[] data);
        SortedList<int, cGlobalESRElement> getListOfGlobalElements(int AccountID, int NHSTrustID);
        List<cAutomatedTransfer> GetReadyTransfers();
        List<TrustUpdateInfo> GetUpdatedTrusts(DateTime since);
        byte[] GetExportData(cReportRequest request);
        cReportRequest startInboundExport(int accountID, int financialReportID, int exportHistoryID);
        object[] getExportProgress(cReportRequest request);
        OutboundStatus UploadOutboundFiles(int accountId, cESRFileInfo outboundFileInfo, int expTrustId);
        int UpdateTransferStatus(int accountID, int exportHistoryID, FinancialExportStatus status);
        DateTime GetUpdateServerUTCDateTime();
    }
}
