using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;

using SpendManagementLibrary;

using Spend_Management;

public partial class reports_exportoptions : System.Web.UI.Page
{    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            this.litStyles.Text = cColours.customiseStyles(false);

            Guid reportid = new Guid(this.Request.QueryString["reportid"].ToString());
            this.ViewState["reportid"] = reportid;

            int requestnum = int.Parse(this.Request.QueryString["requestnum"]);

            cReportRequest request = (cReportRequest)this.Session["request" + requestnum];
            this.ViewState["requestnum"] = requestnum;

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            cExportOptions clsoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);
            this.chkshowheaderscsv.Checked = clsoptions.showheaderscsv;
            this.chkshowheadersexcel.Checked = clsoptions.showheadersexcel;
            this.chkshowheadersflat.Checked = clsoptions.showheadersflatfile;
            this.chkencloseinspeechmarks.Checked = clsoptions.EncloseInSpeechMarks;
            this.chkremovecarriagereturns.Checked = clsoptions.RemoveCarriageReturns;
            this.PopulateFlatFileGrid(requestnum, clsoptions.flatfile);

            this.cmbfooter.Items.AddRange(this.CreateFooterList(user.AccountID, user.CurrentSubAccountId, request.report.basetable.TableID, request.report.reportid));
            this.cmbfooter.Items.Insert(0, new ListItem(string.Empty, Guid.Empty.ToString()));
            if (clsoptions.footerreport != null)
            {
                if (this.cmbfooter.Items.FindByValue(clsoptions.footerreport.reportid.ToString()) != null)
                {
                    this.cmbfooter.Items.FindByValue(clsoptions.footerreport.reportid.ToString()).Selected = true;
                }
            }

            if (clsoptions.Delimiter == "\t")
            {
                this.optdelimitertab.Checked = true;
            }
            else
            {
                this.optdelimiterother.Checked = true;
                this.txtdelimiter.Text = clsoptions.Delimiter;
            }            
        }
    }

    /// <summary>
    /// Creates the data for flat file options field length grid
    /// </summary>
    /// <param name="requestnum">The request number of the report</param>
    /// <param name="flatfile">The list of flat file fields and their lengths</param>
    private void PopulateFlatFileGrid(int requestnum, SortedList<Guid, int> flatfile)
    {
        cReportRequest request = (cReportRequest)Session["request" + requestnum];

        ArrayList columns = request.report.columns;
        cReportColumn column;
        cStandardColumn standard;
        cStaticColumn staticcol;
        cCalculatedColumn calculatedcol;
        DataTable tbl = new DataTable();
        object[] values;
        tbl.Columns.Add("reportcolumnid", typeof(Guid));
        tbl.Columns.Add("columnname", typeof(String));
        tbl.Columns.Add("fieldlength", typeof(int));

        bool removeClaimid = (request.report.basetable.TableID == new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0") || request.report.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8")) && request.report.getReportType() == ReportType.Item;
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
                        if (!removeClaimid || standard.field.FieldID != new Guid("e3af2b67-a613-437e-aabf-6853c4553977"))
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

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        Dictionary<string, object> childRow;
        foreach (DataRow row in tbl.Rows)
        {
            childRow = tbl.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col]);
            parentRow.Add(childRow);
        }

        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "script", "<script>initialiseGrid('" + serializer.Serialize(parentRow) + "');</script>");
    }

    /// <summary>
    /// Saves the export options
    /// </summary>
    /// <param name="optdelimitertab">
    /// Checks if Delimiter is enabled or not
    /// </param>
    /// <param name="txtdelimiter">
    /// The value of Delimiter, if enabled
    /// </param>
    /// <param name="cmbfooterValue">
    /// The footer report value
    /// </param>
    /// <param name="reportId">
    /// The report id
    /// </param>
    /// <param name="showheadersexcel">
    /// Checks if Excel Options header is enabled or not
    /// </param>
    /// <param name="showheaderscsv">
    /// Checks if CSV Options header is enabled or not
    /// </param>
    /// <param name="showheadersflatfile">
    /// Checks if Flat File Options header is enabled or not
    /// </param>
    /// <param name="removecarriagereturns">
    /// Checks if Carriage Returns should be removed ot not
    /// </param>
    /// <param name="encloseinspeechmarks">
    /// Checks if Speech Marks should be enabled
    /// </param>
    /// <param name="fieldLengths">
    /// The JSON of field length values
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> true when export options have successfulyy saved.
    /// </returns>
    [WebMethod(EnableSession = true)]
    public static bool SaveExportOptions(bool optdelimitertab, string txtdelimiter, string cmbfooterValue, string reportId, bool showheadersexcel, bool showheaderscsv, bool showheadersflatfile, bool removecarriagereturns, bool encloseinspeechmarks, string fieldLengths)
    {
        string delimiter;
        SortedList<Guid, int> flatfile = new SortedList<Guid,int>();

        if (optdelimitertab)
        {
            delimiter = "\\t";
        }
        else
        {
            delimiter = txtdelimiter;
            if (delimiter == string.Empty)
            {
                delimiter = ",";
            }
        }


        SortedList<string, string> fieldLengthSortedList = new JavaScriptSerializer().Deserialize<SortedList<string, string>>(fieldLengths);
        int columnlength;
        Guid reportcolumnid;
        foreach (var fieldLength in fieldLengthSortedList)
        {
            if (Guid.TryParse(fieldLength.Key, out reportcolumnid))
            {
                columnlength = string.IsNullOrEmpty(fieldLength.Value) ? 0 : Convert.ToInt32(fieldLength.Value);
                flatfile.Add(reportcolumnid, columnlength);
            }
        }

        CurrentUser user = cMisc.GetCurrentUser();
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

        cExportOptions oldoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, new Guid(reportId));

        var footerreportid = new Guid(cmbfooterValue);
        cReport footer = clsreports.getReportById(user.AccountID, footerreportid);

        var clsoptions = new cExportOptions(user.EmployeeID, new Guid(reportId), showheadersexcel, showheaderscsv, showheadersflatfile, flatfile, footer, oldoptions.drilldownreport, FinancialApplication.CustomReport, delimiter, removecarriagereturns, encloseinspeechmarks, false);
        clsreports.updateExportOptions(user.AccountID, clsoptions);
        return true;
    }

    /// <summary>
    /// Creates the dropdown for footer list of reports
    /// </summary>
    /// <param name="accountID">The account id of the logged in user</param>
    /// <param name="subaccountID">The sub account id of the logged in user</param>
    /// <param name="basetable">The ID of base table used to create the report</param>
    /// <param name="reportid">The report ID</param>
    /// <returns>Returns the list of footer reports</returns>
    public ListItem[] CreateFooterList(int accountID, int subaccountID, Guid basetable, Guid reportid)
    {
        cReports clsreports = new cReports(accountID,subaccountID);
        
        List<ListItem> rpts = clsreports.getFooterReports(basetable, reportid);

        return rpts.ToArray();
    }
}
