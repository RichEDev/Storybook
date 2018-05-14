using System;
using System.Web.UI;

using BusinessLogic;

using SEL.FeatureFlags;

using Spend_Management;

using SpendManagementLibrary;

public partial class reports_enteratruntime : Page
{
    [Dependency]
    public IFeatureFlagManager FeatureFlagManager { get; set; }

    public bool NewStyle { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser user = cMisc.GetCurrentUser();
        ViewState["accountid"] = user.AccountID;
        ViewState["employeeid"] = user.EmployeeID;
        this.NewStyle = this.FeatureFlagManager.IsEnabled("Syncfusion report viewer");
        int financialExportID = 0;
        if (Request.QueryString["financialexportid"] != null)
        {
            int.TryParse(Request.QueryString["financialexportid"], out financialExportID);
        }

        ViewState["financialExportID"] = financialExportID;

        int exportHistoryID = 0;
        if (Request.QueryString["exporthistoryid"] != null)
        {
            int.TryParse(Request.QueryString["exporthistoryid"], out exportHistoryID);
        }

        ViewState["exportHistoryID"] = exportHistoryID;


        Title = @"Filter Details";
        Master.title = Title;
        Master.PageSubTitle = "Enter Filter Details";
        int requestnum = int.Parse(Request.QueryString["requestnum"]);

        if (Request.QueryString["exporttype"] != null)
        {
            ExportType exporttype = (ExportType)byte.Parse(Request.QueryString["exporttype"]);
            ViewState["exporttype"] = exporttype;
        }
        cReportRequest request = (cReportRequest)Session["request" + requestnum];
        ViewState["requestnum"] = requestnum;

        criteria.accountid = user.AccountID;
        criteria.subAccountId = user.CurrentSubAccountId;
        criteria.report = request.report;

        ClientScript.RegisterClientScriptBlock(this.GetType(), "runtime", "self.focus();", true);
    }

    

    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        cReportRequest request = (cReportRequest)Session["request" + ViewState["requestnum"]];
        cReport rpt = request.report;
        criteria.getRuntimeValues(ref rpt);

        if (ViewState["exporttype"] != null)
        {
            string financialExportExportQueryString = string.Empty;

            if (ViewState["financialExportID"] != null)
            {
                int financialExportID;
                if (int.TryParse(ViewState["financialExportID"].ToString(), out financialExportID))
                {
                    financialExportExportQueryString = "&financialexportid=" + this.ViewState["financialExportID"];
                }
            }

            string exportHistoryQueryString = string.Empty;

            if (ViewState["exportHistoryID"] != null)
            {
                int financialExportID;
                if (int.TryParse(ViewState["exportHistoryID"].ToString(), out financialExportID))
                {
                    exportHistoryQueryString = "&exporthistoryid=" + this.ViewState["exportHistoryID"];
                }
            }

            Response.Write("<script type=\"text/javascript\">\n");
            Response.Write("window.open('exportreport.aspx?exporttype=" + Convert.ToByte(ViewState["exporttype"]) + "&requestnum=" + ViewState["requestnum"] + financialExportExportQueryString + exportHistoryQueryString + "','export','width=300,height=150,status=no,menubar=no');\n");
            Response.Write("window.close();\n");
            Response.Write("</script>");
        }
        else
        {
            if (NewStyle)
            {
                Response.Redirect("view.aspx?reportid=" + request.report.reportid + "&requestnum=" + request.requestnum, true);
            }
            else
            {
                Response.Redirect("reportviewer.aspx?reportid=" + request.report.reportid + "&requestnum=" + request.requestnum, true);    
            }
        }
    }
}
