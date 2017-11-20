//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Stub_viewclaim_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'viewclaim.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Infragistics.WebUI.UltraWebGrid;
using expenses.Old_App_Code;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for viewclaim.
	/// </summary>
    public partial class viewclaim : Page
    {
        public int requestnum;
        public bool showStatus = true;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);

                int claimid = int.Parse(this.Request.QueryString["claimid"]);

                int expenseid = 0;
                if (Request.QueryString["expenseid"] != null)
                {
                    expenseid = int.Parse(Request.QueryString["expenseid"]);
                }
                ViewState["expenseid"] = expenseid;

                requestnum = int.Parse(Request.QueryString["requestnum"]);
                cReportRequest request = (cReportRequest)Session["request" + requestnum];
                int currentrequestnum = (int)Session["currentrequestnum"];
                requestnum = currentrequestnum + 1;

                cReportRequest viewrequest = new cReportRequest(request.accountid, user.CurrentSubAccountId, requestnum, (cReport)request.report.Clone(), request.exporttype, request.report.exportoptions, request.claimantreport, request.employeeid, request.AccessLevel);
                
                ViewState["requestnum"] = requestnum;
                
                ViewState["reportid"] = viewrequest.report.reportid;

                Session["request" + requestnum] = viewrequest;

                cClaims clsclaims = new cClaims(user.AccountID);

                cClaim reqclaim = clsclaims.getClaimById(claimid);

                Employee claimemp = clsemployees.GetEmployeeById(reqclaim.employeeid);


                string empname = claimemp.Title + " " + claimemp.Forename + " " + claimemp.Surname;
                lblemployee.Text = empname;
                lblclaimno.Text = reqclaim.claimno.ToString();
                if (reqclaim.paid == true)
                {
                    lbldatepaid.Text = reqclaim.datepaid.ToShortDateString();
                }
                lbldescription.Text = reqclaim.description;

                cFields clsfields = new cFields(user.AccountID);
                IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");



                //clear the current criteria;
                viewrequest.report.criteria.Clear();
                //add claimid
                cReportCriterion criteria = new cReportCriterion(new Guid(), request.report.reportid, clsfields.GetFieldByID(new Guid("e3af2b67-a613-437e-aabf-6853c4553977")), ConditionType.Equals, new object[] {claimid}, new object[0], ConditionJoiner.None, 1, false, 0);

                viewrequest.report.criteria.Add(criteria);
                request.report.criteria.Add(criteria);

                clsreports.createReport(viewrequest);

                cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);

                System.Text.StringBuilder output = new System.Text.StringBuilder();
                output.Append("<style type\"text/css\">\n");
                if (clscolours.sectionHeadingUnderlineColour != clscolours.defaultSectionHeadingUnderlineColour)
                {
                    output.Append(".infobar\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".datatbl th\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".inputpaneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".paneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".homepaneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.rowTxtColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.fieldTxtColour != clscolours.defaultFieldTxt)
                {
                    output.Append(".labeltd\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.fieldTxtColour + ";\n");
                    output.Append("}\n");
                }
                output.Append("</style>");
                litstyles.Text = output.ToString();
            }
            else
            {
            }
        }



        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion






        protected void gridclaim_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            cReportRequest clsrequest = (cReportRequest)Session["request" + Request.QueryString["requestnum"]];
            cReport rpt = clsrequest.report;

            for (int i = 0; i < rpt.columns.Count; i++)
            {
                cReportColumn column = (cReportColumn)rpt.columns[i];

                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        cStandardColumn standard = (cStandardColumn)column;
                        if (standard.field.FieldID == new Guid("a528de93-3037-46f6-974c-a76bd0c8642a"))
                        {
                            ViewState["expenseidcol"] = standard.order;
                        }
                        if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin || standard.funcsum)
                        {
                            if (standard.funcavg)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_AVG").Hidden = column.hidden;
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_AVG").Header.Caption = "AVG of " + standard.field.Description;

                            }
                            if (standard.funccount)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_COUNT").Hidden = column.hidden;
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_COUNT").Header.Caption = "COUNT of " + standard.field.Description;
                            }
                            if (standard.funcmax)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MAX").Hidden = column.hidden;
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MAX").Header.Caption = "MAX of " + standard.field.Description;
                            }
                            if (standard.funcmin)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MIN").Hidden = column.hidden;
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MIN").Header.Caption = "MIN of " + standard.field.Description;
                            }
                            if (standard.funcsum)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_SUM").Hidden = column.hidden;
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_SUM").Header.Caption = "SUM of " + standard.field.Description;
                            }
                        }
                        else
                        {
                            e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Header.Caption = standard.field.Description;
                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                            switch (standard.field.FieldType)
                            {
                                case "D":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "dd/MM/yyyy";
                                    break;
                                case "DT":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "dd/MM/yyyy HH:mm:ss";
                                    break;
                                case "T":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "HH:mm:ss";
                                    break;
                                case "X":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Type = ColumnType.CheckBox;
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Center;
                                    break;
                                case "N":
                                    if (standard.field.CanTotal)
                                    {
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    break;
                                case "C":
                                    if (standard.field.CanTotal)
                                    {
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "#,###,##0.00";
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    break;
                                case "M":
                                case "FD":
                                    if (standard.field.CanTotal)
                                    {
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "#,###,##0.00";
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    break;
                            }
                        }
                        break;
                    case ReportColumnType.Static:
                        cStaticColumn staticcol = (cStaticColumn)column;
                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                        e.Layout.Bands[0].Columns.FromKey(staticcol.columnid.ToString()).Header.Caption = staticcol.literalname;
                        e.Layout.Bands[0].Columns.FromKey(staticcol.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    case ReportColumnType.Calculated:
                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                        cCalculatedColumn calculatedcol = (cCalculatedColumn)column;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Header.Caption = calculatedcol.columnname;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Formula = convertFormula(calculatedcol.formula);
                        break;
                }
            }
        }

        public string convertFormula(string formula)
        {
            int start = 0;
            while (start < formula.Length)
            {
                int startIndex = formula.IndexOf('[', start);
                if (startIndex == -1)
                {
                    break;
                }
                int endIndex = formula.IndexOf(']', startIndex);
                string column = formula.Substring(startIndex + 1, endIndex - startIndex - 1);
                //replace with index
                for (int i = 0; i < gridclaim.Columns.Count; i++)
                {
                    if (gridclaim.Columns[i].Header.Caption == column)
                    {
                        int pos = i + 1;
                        formula = formula.Replace(column, pos.ToString());
                        break;
                    }
                }
                endIndex = formula.IndexOf(']', startIndex);
                start = endIndex;
            }
            return formula;
        }
        protected void gridclaim_InitializeRow(object sender, RowEventArgs e)
        {
            int expenseid = (int)ViewState["expenseid"];

            if (expenseid != 0)
            {
                if ((int)e.Row.Cells.FromKey(ViewState["expenseidcol"].ToString()).Value == expenseid)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Style.ForeColor = Color.Red;
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }

        }

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridclaim.InitializeDataSource += new InitializeDataSourceEventHandler(gridclaim_InitializeDataSource);
        }

        void gridclaim_InitializeDataSource(object sender, UltraGridEventArgs e)
        {

            ReportFunctions functions = new ReportFunctions();
            DataSet ds;
            requestnum = (int)ViewState["requestnum"];
            CurrentUser user = cMisc.GetCurrentUser();

            if (ViewState[requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()] != null)
            {
                ds = (DataSet)ViewState[requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()];

                gridclaim.DataSource = ds;
                gridclaim.Visible = true;
                reportStatusInformation.Visible = false;
                cReportRequest claimrequest = (cReportRequest)Session["request" + requestnum];

                IReports clsIReports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                clsIReports.cancelRequest(claimrequest);
            }
            else if (functions.getReportProgress(requestnum) != null && functions.getReportProgress(requestnum)[0].ToString() == "Complete")
            {
                showStatus = false;
                if (ViewState[requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()] == null)
                {
                    ds = (DataSet)functions.getReportData(requestnum);
                    ViewState[requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()] = ds;

                    cReportRequest claimrequest = (cReportRequest)Session["request" + requestnum];

                    IReports clsIReports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                    clsIReports.cancelRequest(claimrequest);
                }
                else
                {
                    ds = (DataSet)ViewState[requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()];
                }

                gridclaim.DataSource = ds;
                gridclaim.Visible = true;
                reportStatusInformation.Visible = false;
                
            }
        }
    }
}

