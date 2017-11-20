using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using expenses;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;
public partial class reports_exportoptions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            litStyles.Text = cColours.customiseStyles(false);

            Guid reportid = new Guid(Request.QueryString["reportid"].ToString());
            ViewState["reportid"] = reportid;

            int requestnum = int.Parse(Request.QueryString["requestnum"]);

            cReportRequest request = (cReportRequest)Session["request" + requestnum];
            ViewState["requestnum"] = requestnum;

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            cExportOptions clsoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);
            chkshowheaderscsv.Checked = clsoptions.showheaderscsv;
            chkshowheadersexcel.Checked = clsoptions.showheadersexcel;
            chkshowheadersflat.Checked = clsoptions.showheadersflatfile;
            chkencloseinspeechmarks.Checked = clsoptions.EncloseInSpeechMarks;
            chkremovecarriagereturns.Checked = clsoptions.RemoveCarriageReturns;
            populateFlatFileGrid(requestnum, clsoptions.flatfile);

            cmbfooter.Items.AddRange(CreateFooterList(user.AccountID, user.CurrentSubAccountId, request.report.basetable.TableID, request.report.reportid));
            cmbfooter.Items.Insert(0, new ListItem("", Guid.Empty.ToString()));
            if (clsoptions.footerreport != null)
            {
                if (cmbfooter.Items.FindByValue(clsoptions.footerreport.reportid.ToString()) != null)
                {
                    cmbfooter.Items.FindByValue(clsoptions.footerreport.reportid.ToString()).Selected = true;
                }
            }

            if (clsoptions.Delimiter == "\t")
            {
                optdelimitertab.Checked = true;
            }
            else
            {
                optdelimiterother.Checked = true;
                txtdelimiter.Text = clsoptions.Delimiter;
            }


        }
    }

    private void populateFlatFileGrid(int requestnum, SortedList<Guid, int> flatfile)
    {
        cReportRequest request = (cReportRequest)Session["request" + requestnum];

        ArrayList columns = request.report.columns;
        cReportColumn column;
        cStandardColumn standard;
        cStaticColumn staticcol;
        cCalculatedColumn calculatedcol;
        DataTable tbl = new DataTable();
        object[] values;
        tbl.Columns.Add("reportcolumnid", typeof(System.Guid));
        tbl.Columns.Add("columnname", typeof(System.String));
        tbl.Columns.Add("fieldlength", typeof(System.Int32));

        bool removeClaimid = false;
        if ((request.report.basetable.TableID == new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0") || request.report.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8")) && request.report.getReportType() == ReportType.Item)
        {
            removeClaimid = true;
        }
        for (int i = 0; i < columns.Count; i++)
        {
            values = new object[3];
            column = (cReportColumn)columns[i];

            if (column.system == false)
            {
                if (flatfile.ContainsKey(column.reportcolumnid) == true)
                {
                    values[2] = (int)flatfile[column.reportcolumnid];
                }

                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        standard = (cStandardColumn)column;
                        values[0] = column.reportcolumnid;
                        values[1] = standard.field.Description;
                        if (!removeClaimid || (removeClaimid && standard.field.FieldID != new Guid("e3af2b67-a613-437e-aabf-6853c4553977")))
                        {
                            tbl.Rows.Add(values);
                        }
                        break;
                    case ReportColumnType.Static:
                        staticcol = (cStaticColumn)column;
                        values[0] = column.reportcolumnid;
                        values[1] = staticcol.literalname;
                        tbl.Rows.Add(values);
                        break;
                    case ReportColumnType.Calculated:
                        calculatedcol = (cCalculatedColumn)column;
                        values[0] = column.reportcolumnid;
                        values[1] = calculatedcol.columnname;
                        tbl.Rows.Add(values);
                        break;
                }
            }
            
        }

        gridflatfile.DataSource = tbl;
        gridflatfile.DataBind();
    }
    protected void gridflatfile_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
    {
        e.Layout.Bands[0].Columns.FromKey("columnname").Header.Caption = "Column";
        e.Layout.Bands[0].Columns.FromKey("reportcolumnid").Hidden = true;
        e.Layout.Bands[0].Columns.FromKey("fieldlength").Header.Caption = "Length";
        e.Layout.Bands[0].Columns.FromKey("fieldlength").AllowUpdate = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;
    }
    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        bool showheadersexcel = chkshowheadersexcel.Checked;
        bool showheaderscsv = chkshowheaderscsv.Checked;
        bool showheadersflatfile = chkshowheadersflat.Checked;
        bool removecarriagereturns = chkremovecarriagereturns.Checked;
        bool encloseinspeechmarks = chkencloseinspeechmarks.Checked;
        string delimiter;
        SortedList<Guid, int> flatfile = new SortedList<Guid,int>();
        Guid reportcolumnid;
        int columnlength;
        Guid footerreportid = Guid.Empty;

        if (optdelimitertab.Checked)
        {
            delimiter = "\\t";
        }
        else
        {
            delimiter = txtdelimiter.Text;
            if (delimiter == "")
            {
                delimiter = ",";
            }
        }
        for (int i = 0; i < gridflatfile.Rows.Count; i++)
        {
            reportcolumnid = (Guid)gridflatfile.Rows[i].Cells.FromKey("reportcolumnid").Value;
            if (gridflatfile.Rows[i].Cells.FromKey("fieldlength").Value == null)
            {
                columnlength = 0;
            }
            else
            {
                columnlength = (int)gridflatfile.Rows[i].Cells.FromKey("fieldlength").Value;
            }
            flatfile.Add(reportcolumnid, columnlength);
        }

        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

        cExportOptions oldoptions = clsreports.getExportOptions((int)ViewState["accountid"], (int)ViewState["employeeid"], (Guid)ViewState["reportid"]);

        footerreportid = new Guid(cmbfooter.SelectedValue);
        cReport footer = clsreports.getReportById((int)ViewState["accountid"], footerreportid);
        CurrentUser user = cMisc.GetCurrentUser();
        var clsoptions = new cExportOptions(user.EmployeeID, new Guid(ViewState["reportid"].ToString()), showheadersexcel, showheaderscsv, showheadersflatfile, flatfile, footer, oldoptions.drilldownreport, FinancialApplication.CustomReport, delimiter, removecarriagereturns,encloseinspeechmarks, false);
        clsreports.updateExportOptions((int)ViewState["accountid"], clsoptions);

        Response.Write("<script type=\"text/javascript\">\nwindow.close();\n</script>");
    }

    public ListItem[] CreateFooterList(int accountID, int subaccountID, Guid basetable, Guid reportid)
    {
        cReports clsreports = new cReports(accountID,subaccountID);
        
        List<ListItem> rpts = clsreports.getFooterReports(basetable, reportid);

        return rpts.ToArray();

        
    }

    private SortedList sortList(ArrayList lst)
    {
        SortedList sorted = new SortedList();

        cReport rpt;

        for (int i = 0; i < lst.Count; i++)
        {
            rpt = (cReport)lst[i];
            if (!sorted.Contains(rpt.reportname))
            {
                sorted.Add(rpt.reportname, rpt);
            }
        }
        return sorted;
    }
}
