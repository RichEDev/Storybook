using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcFinancialExports
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcFinancialExports : System.Web.Services.WebService
    {

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public int DeleteFinancialExport(int financialExportID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cFinancialExports clsexports = new cFinancialExports(currentUser.AccountID);
            clsexports.deleteFinancialExport(financialExportID);
            return financialExportID;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cFinancialExport GetFinancialExport(int financialExportID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            var clsExports = new cFinancialExports(currentUser.AccountID);

            cFinancialExport export = null;
            if (financialExportID > 0)
            {
                 export = clsExports.getExportById(financialExportID);
            }

            return export;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveFinancialExport(int financialExportID, string applicationTypeID, string trustID, string exportTypeID, string reportGUID, bool preventNegativePayment, bool expeditePaymentReport)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            var exports = new cFinancialExports(currentUser.AccountID);
            var financialApplication = (FinancialApplication)byte.Parse(applicationTypeID);
            cFinancialExport export;
            var nhsTrustId = 0;
            byte exporttype = 0;
                
            var reportId = new Guid(reportGUID);

            switch (financialApplication)
            {
                case FinancialApplication.CustomReport:
                    exporttype = byte.Parse(exportTypeID);
                    break;
            
                case FinancialApplication.ESR:
                    nhsTrustId = int.Parse(trustID);
                    break;
            }

            if (financialExportID > 0)
            {
                cFinancialExport oldexport = exports.getExportById(financialExportID);
                export = new cFinancialExport(financialExportID, currentUser.AccountID, financialApplication, reportId, false, oldexport.createdby, oldexport.createdon, oldexport.curexportnum, oldexport.lastexportdate, exporttype, nhsTrustId, preventNegativePayment, expeditePaymentReport);
                exports.updateFinancialExport(export, currentUser.EmployeeID);
            }
            else
            {
                export = new cFinancialExport(0, currentUser.AccountID, financialApplication, reportId, false, currentUser.EmployeeID, DateTime.Now, 1, new DateTime(1900, 01, 01), exporttype, nhsTrustId, preventNegativePayment, expeditePaymentReport);
                exports.addFinancialExport(export);
            }

            return financialExportID;
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public bool CheckESRTrustExists(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRTrusts trustDetails = new cESRTrusts(currentUser.AccountID);
            cESRTrust trust = trustDetails.GetESRTrustByID(trustID);
            if (trust == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Get the ESR assignment Information for the modal popup when running a financial export
        /// </summary>
        /// <param name="financialExportID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string CheckExpenseESRAssignments(int financialExportID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cFinancialExports clsExports = new cFinancialExports(currentUser.AccountID);
            cFinancialExport export = clsExports.getExportById(financialExportID);

            string assignmentInfo = "";

            if (export.application == FinancialApplication.ESR)
            {
                assignmentInfo = clsExports.CheckFinancialExportESRAssignments();
            }

            return assignmentInfo;
        }


        /// <summary>
        /// Get the financial export history grid
        /// </summary>
        /// <param name="financialExportID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession=true)]
        public string[] CreateExportHistoryGrid(int financialExportID, int appType)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            var eStatus = FinancialExportStatus.AwaitingESRInboundTransfer;

            string strSQL = "SELECT exporthistory.exporthistoryid, exporthistory.financialexportid, exporthistory.exportnum, exporthistory.dateexported, dbo.GetEmployeeUserNameFromId(exporthistory.employeeid), exporthistory.exportStatus, financial_exports.applicationtype FROM dbo.exporthistory";
            var gridExportHistory = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "exportHistoryGrid", strSQL);

            gridExportHistory.KeyField = "exporthistoryid";
            gridExportHistory.getColumnByName("exporthistoryid").hidden = true;
            gridExportHistory.getColumnByName("financialexportid").hidden = true;

            var clsFields = new cFields(currentUser.AccountID);

            var financialExportId = clsFields.GetFieldByID(new Guid("B6A641F5-0152-4962-9572-F6CF9178EE59"));

            gridExportHistory.addFilter(financialExportId, ConditionType.Equals, new object[] { financialExportID }, null, ConditionJoiner.None);

            gridExportHistory.enabledeleting = false;
            gridExportHistory.enableupdating = false;
            gridExportHistory.EmptyText = "This Financial Report has not been exported yet";

            gridExportHistory.addEventColumn("viewhistory", "/shared/images/icons/export1.png", "javascript:exportReport({financialexportid},{exporthistoryid});", "View Export from History", "View this Financial Export");
        
           ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)0, "No Status set");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)1, "Export Failed");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)2, "Export Succeeded");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)3, "Awaiting ESR Inbound Transfer");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)4, "Collected For Upload To ESR");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)5, "Upload To ESR Failed");
            ((cFieldColumn)gridExportHistory.getColumnByName("exportStatus")).addValueListItem((byte)6, "Upload To ESR Succeeded");
            if (appType == 2)
            {
                ((cFieldColumn)gridExportHistory.getColumnByName("applicationtype")).hidden = false;
                ((cFieldColumn)gridExportHistory.getColumnByName("applicationtype")).addValueListItem((byte)1, "");
                ((cFieldColumn)gridExportHistory.getColumnByName("applicationtype")).addValueListItem((byte)2, "<a href=\"javascript:RerunESRInbound(this," + currentUser.AccountID.ToString() + ",{exporthistoryid}," + ((int)eStatus).ToString() + ", {financialexportid});\">Rerun Export (ESR Inbound File)</a>");
            }
            else
            {
                gridExportHistory.getColumnByName("applicationtype").hidden = true;
            }
            return gridExportHistory.generateGrid();
        }
    }
}
