using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

using SpendManagementLibrary.Employees;

using Spend_Management;

using SpendManagementLibrary;

using Spend_Management.shared.reports;

/// <summary>
/// Summary description for ReportFunctions
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class ReportFunctions : System.Web.Services.WebService
{

    public ReportFunctions()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public bool exportReport(int requestnum, byte exporttypeid, Guid reportid, int financialexportid, int exporthistoryid)
    {
        CurrentUser user = cMisc.GetCurrentUser();

        cEmployees clsemployees = new cEmployees(user.AccountID);
        int accountid = user.AccountID;

        bool claimants = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, false) == false;

        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

        cReportRequest clsrequest = (cReportRequest)this.Session["request" + requestnum];

        ExportType exporttype = (ExportType)exporttypeid;

        Session["exporttype"] = exporttype;

        cExportOptions clsoptions = clsrequest.report.exportoptions ?? clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);

        cReportRequest exportrequest = null;

        if (clsoptions.footerreport != null)
        {
            clsoptions.footerreport.isFooter = true;

            // replace criteria with our report
            clsoptions.footerreport.criteria.Clear();


            for (int i = 0; i < clsrequest.report.criteria.Count; i++)
            {
                clsoptions.footerreport.criteria.Add(clsrequest.report.criteria[i]);
            }
        }
        
        switch (clsoptions.application)
        {
            case FinancialApplication.CustomReport:
                switch (exporttype)
                {
                    case ExportType.Pivot:
                        exportrequest = new cReportRequest(accountid, clsrequest.SubAccountId, clsrequest.requestnum, clsrequest.report, ExportType.Pivot, clsoptions, claimants, user.EmployeeID, user.HighestAccessLevel);
                        break;
                    case ExportType.Excel:
                        exportrequest = new cReportRequest(accountid, clsrequest.SubAccountId, clsrequest.requestnum, clsrequest.report, ExportType.Excel, clsoptions, claimants, user.EmployeeID, user.HighestAccessLevel);
                        break;
                    case ExportType.CSV:
                        exportrequest = new cReportRequest(accountid, clsrequest.SubAccountId, clsrequest.requestnum, clsrequest.report, ExportType.CSV, clsoptions, claimants, user.EmployeeID, user.HighestAccessLevel);
                        break;
                    case ExportType.FlatFile:
                        exportrequest = new cReportRequest(accountid, clsrequest.SubAccountId, clsrequest.requestnum, clsrequest.report, ExportType.FlatFile, clsoptions, claimants, user.EmployeeID, user.HighestAccessLevel);
                        break;
                }
                break;
            case FinancialApplication.ESR:
                exportrequest = new cReportRequest(accountid, clsrequest.SubAccountId, clsrequest.requestnum, clsrequest.report, ExportType.CSV, clsoptions, false, user.EmployeeID, user.HighestAccessLevel);
                break;
        }

        Session["exportrequestnum"] = requestnum;

        if (financialexportid > 0)
        {
            exportrequest.reportRunFrom = ReportRunFrom.PrimaryServer;
        }

        if (exportrequest.AccessLevel == AccessRoleLevel.SelectedRoles && cReport.canFilterByRole(exportrequest.report.basetable.TableID))
        {
            // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
            cAccessRoles roles = new cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
            List<int> reportRoles = new List<int>();
            List<int> lstAccessRoles = user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

            foreach (int emp_arId in lstAccessRoles)
            {
                cAccessRole empRole = roles.GetAccessRoleByID(emp_arId);
                foreach (int arId in empRole.AccessRoleLinks)
                {
                    if (!reportRoles.Contains(arId))
                    {
                        reportRoles.Add(arId);
                    }
                }
            }
            exportrequest.AccessLevelRoles = reportRoles.ToArray();
        }

        Session["request" + requestnum] = exportrequest;

        return clsreports.createReport(exportrequest);
    }

    [WebMethod(EnableSession=true)]
    [ScriptMethod]
    public object[] getReportProgress(int requestnum)
    {
        cReportRequest clsrequest = (cReportRequest)Session["request" + requestnum];
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

        object[] data = clsreports.getReportProgress(clsrequest);

        return data;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public void ClearReportFromSession(int requestnum)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        if (Session[requestnum.ToString() + "_" + currentUser.EmployeeID.ToString() + "_" + currentUser.AccountID.ToString()] != null)
        {
            Session.Remove(requestnum.ToString() + "_" + currentUser.EmployeeID.ToString() + "_" + currentUser.AccountID.ToString());
        }
    }

    [WebMethod(EnableSession=true)]
    [ScriptMethod]
    public void cancelReportRequest(int requestnum)
    {
        cReportRequest clsrequest = (cReportRequest)Session["request" + requestnum];
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        clsreports.cancelRequest(clsrequest);
    }

    public object GetReportError(int requestnum)
    {
        cReportRequest clsrequest = (cReportRequest)Session["request" + requestnum];
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        Exception ex = clsreports.GetReportError(clsrequest);
        return ex;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public object getReportData(int requestnum)
    {
        cReportRequest clsrequest = (cReportRequest)Session["request" + requestnum];
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        return clsreports.getReportData(clsrequest);

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public ViewClaimDetails ViewClaim(int requestnum, int claimid, int expenseid)
    {
        var user = cMisc.GetCurrentUser();
        var clsclaims = new cClaims(user.AccountID);
        var result = new ViewClaimDetails();
        cClaim reqclaim = clsclaims.getClaimById(claimid);
        Employee claimemp = Employee.Get(reqclaim.employeeid, user.AccountID);

        string empname = claimemp.Title + " " + claimemp.Forename + " " + claimemp.Surname;

        result.EmployeeName = empname;
        result.ClaimNumber = reqclaim.claimno.ToString();
        if (reqclaim.paid == true)
        {
            result.DatePaid = reqclaim.datepaid.ToShortDateString();
        }
        result.Description = reqclaim.description;

        var ds = (DataSet)Session[string.Format("{0}_{1}_{2}", requestnum, user.EmployeeID, user.AccountID)];
        var columns = new List<cNewGridColumn>();
        var request = (cReportRequest)this.Session["request" + requestnum];
        if (ds.Tables.Count > 0)
        {
            var idx = 0;
            var claimIdColumn = string.Empty;
            var gridHasRealTitles = ds.Tables[0].ExtendedProperties.Contains("Titled");
            foreach (cReportColumn column in request.report.columns)
            {
                var fieldDescription = string.Empty;
                if (column is cStandardColumn)
                {
                    var standardColumn = (cStandardColumn) column;
                    columns.Add(new cFieldColumn(standardColumn.field));
                    fieldDescription = standardColumn.field.FieldName;
                    if (!gridHasRealTitles && ds.Tables[0].Columns.Contains(fieldDescription.ToLower()))
                    {
                        fieldDescription = standardColumn.field.Description;
                    }

                    if (((cStandardColumn) column).field.FieldID == new Guid(ReportKeyFields.ClaimsClaimId))
                    {
                        claimIdColumn = fieldDescription;
                    }

                    var current = (cFieldColumn) columns[idx];
                    current.hidden = column.hidden;
                }
                else
                {
                    if (column is cStaticColumn)
                    {
                        columns.Add(new cStaticGridColumn(((cStaticColumn) column).literalname, Guid.NewGuid()));
                        columns[idx].hidden = true;
                        fieldDescription = ((cStaticColumn) column).literalname;
                    }
                    else
                    {
                        columns.Add(new cStaticGridColumn(((cCalculatedColumn) column).columnname, Guid.NewGuid()));
                        fieldDescription = ((cCalculatedColumn) column).columnname;
                        columns[idx].hidden = true;
                    }
                }
                if (!gridHasRealTitles)
                {
                    ds.Tables[0].Columns[idx].ColumnName = fieldDescription;
                }
                
                idx++;
            }

            ds.Tables[0].DefaultView.RowFilter = $"[{claimIdColumn}] = {claimid}";
            if (!gridHasRealTitles)
            {
                ds.Tables[0].ExtendedProperties.Add("Titled", true);
            }
            
            var table = ds.Tables[0].DefaultView.ToTable();
            
            ds = new DataSet();
            ds.Tables.Add(table);
        }
        var gridInfo = new SerializableDictionary<string, object> { { "expenseid", expenseid } };
        var grid = new cGridNew(user.AccountID, user.EmployeeID, "ViewClaim", request.report.basetable, columns, ds)
                       {
                           InitialiseRowGridInfo = gridInfo
                       };

        grid.InitialiseRow += this.InitaliseGridRow;
        grid.ServiceClassForInitialiseRowEvent = "ReportFunctions";
        grid.ServiceClassMethodForInitialiseRowEvent = "InitaliseGridRow";
        result.Grid = grid.generateGrid();

        return result;
    }

    private void InitaliseGridRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
    {
        var targetExpenseId = (int)gridInfo["expenseid"];
        var expenseIdCell = row.getCellByID("expenseid");
        if (expenseIdCell != null)
        {
            var expenseId = int.Parse(expenseIdCell.Value.ToString());
            if (targetExpenseId == expenseId)
            {
                row.Highlight = true;
            }
        }
    }
}



