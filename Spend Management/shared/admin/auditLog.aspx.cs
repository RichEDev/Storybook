using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Collections;
using System.Web.Services;
using System.Configuration;
using System.Text;

namespace Spend_Management.shared.admin
{
    public partial class auditLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true, true);

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1132;
                        break;
                    default:
                        Master.helpid = 1086;
                        break;
                }

                this.Page.Title = "Audit Log";
                Master.title = "Audit Log"; 
                cElements clselements = new cElements();

                cmdsearch.Attributes.Add("onclick", "javascript:if(validateform('vgAuditLog') == false) { return false; }");
                cmpmaxenddate.ValueToCompare = DateTime.Today.ToShortDateString();
                cmpmaxenddate.ErrorMessage = "End date specified must be on or before " + cmpmaxenddate.ValueToCompare;

                if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AuditLog, true, false))
                {
                    litclear.Text = "<a href=\"javascript:clearAuditLog();\" class=\"submenuitem\">Clear audit log</a>";
                }
                ddlstElement.Items.Add(new ListItem("[None]", "0"));
                ddlstElement.Items.AddRange(clselements.CreateDropDown(user.AccountID, user.CurrentActiveModule).ToArray());
                txtstartdate.Text = DateTime.Today.ToShortDateString();
                txtenddate.Text = DateTime.Today.ToShortDateString();

                string[] gridData = createGrid();
                litgrid.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "auditLogGridVars", cGridNew.generateJS_init("auditLogGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);
            }
        }

        public string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAudit", "SELECT [logid],[datestamp],[employeeID],[username],[delegate],[elementFriendlyName],[action],recordtitle, [description],[oldvalue],[newvalue] FROM auditLogView");
            cFields clsfields = new cFields(user.AccountID);
            if (ddlstElement.SelectedValue != "0")
            {
                clsgrid.addFilter(clsfields.GetFieldByID(new Guid("b193bcf2-796e-4f4c-a926-db334aefb193")), ConditionType.Equals, new object[] { Convert.ToInt32(ddlstElement.SelectedValue) }, null, ConditionJoiner.And);
            }
            if (ddlstAction.SelectedValue != "0")
            {
                clsgrid.addFilter(clsfields.GetFieldByID(new Guid("f4f5e4f3-083c-4708-8d52-58824c102e0e")), ConditionType.Equals, new object[] { Convert.ToInt32(ddlstAction.SelectedValue) }, null, ConditionJoiner.And);
            }
            DateTime start,end;
            start = DateTime.Parse(txtstartdate.Text);
            end = DateTime.Parse(txtenddate.Text);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("fb7462ad-7930-472a-b84a-2ccd34426878")), ConditionType.Between, new object[] { start.ToString("yyyy/MM/dd") + " 00:00:00" }, new object[] { end.ToString("yyyy/MM/dd") + " 23:59:59" }, ConditionJoiner.And);
            if (txtusername.Text != "")
            {
                clsgrid.addFilter(clsfields.GetFieldByID(new Guid("72bfbefa-7c5e-4027-a3f3-fdffb4c10f87")), ConditionType.Like, new object[] { txtusername.Text + "%" }, null, ConditionJoiner.And);
            }
            clsgrid.KeyField = "logid";
            clsgrid.getColumnByName("logid").hidden = true;
            clsgrid.getColumnByName("employeeid").hidden = true;
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(1, "Add");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(2, "Update");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(3, "Delete");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(4, "Logged On");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(5, "Logged Off");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(6, "View");

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        protected void cmdsearch_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string[] gridData = createGrid();
            litgrid.Text = gridData[2];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "auditLogGridVars", cGridNew.generateJS_init("auditLogGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);
        }

        [WebMethod(EnableSession=true)]
        public static int generateReportRequest(int? element, int? action, DateTime start, DateTime end, string username)
        {
            Guid reportid = Guid.NewGuid();
            
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            if (System.Web.HttpContext.Current.Session["currentrequestnum"] == null)
            {
                System.Web.HttpContext.Current.Session["currentrequestnum"] = 0;
            }
            int requestnum;

            int currentrequestnum = (int)System.Web.HttpContext.Current.Session["currentrequestnum"];
            requestnum = currentrequestnum + 1;
            System.Web.HttpContext.Current.Session["currentrequestnum"] = requestnum;

            ArrayList columns = new ArrayList();
            columns.Add(new cStandardColumn(new Guid("FB7462AD-7930-472A-B84A-2CCD34426878"), reportid, ReportColumnType.Standard, ColumnSort.None, 1, clsfields.GetFieldByID(new Guid("FB7462AD-7930-472A-B84A-2CCD34426878")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("72BFBEFA-7C5E-4027-A3F3-FDFFB4C10F87"), reportid, ReportColumnType.Standard, ColumnSort.None, 2, clsfields.GetFieldByID(new Guid("72BFBEFA-7C5E-4027-A3F3-FDFFB4C10F87")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("E503CBBB-3A3D-450C-90B1-3463C1F61DC9"), reportid, ReportColumnType.Standard, ColumnSort.None, 3, clsfields.GetFieldByID(new Guid("E503CBBB-3A3D-450C-90B1-3463C1F61DC9")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("CC1E8A63-E683-4A52-8C07-217268BD61B6"), reportid, ReportColumnType.Standard, ColumnSort.None, 4, clsfields.GetFieldByID(new Guid("CC1E8A63-E683-4A52-8C07-217268BD61B6")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("F4F5E4F3-083C-4708-8D52-58824C102E0E"), reportid, ReportColumnType.Standard, ColumnSort.None, 5, clsfields.GetFieldByID(new Guid("F4F5E4F3-083C-4708-8D52-58824C102E0E")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("0E35EA55-D6EB-43C7-B286-E1522A174773"), reportid, ReportColumnType.Standard, ColumnSort.None, 6, clsfields.GetFieldByID(new Guid("0E35EA55-D6EB-43C7-B286-E1522A174773")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("678B4E7A-CA4E-4412-B4AE-95771B7F95DD"), reportid, ReportColumnType.Standard, ColumnSort.None, 7, clsfields.GetFieldByID(new Guid("678B4E7A-CA4E-4412-B4AE-95771B7F95DD")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("A389495A-DD51-4987-9C52-F89E83ADA31C"), reportid, ReportColumnType.Standard, ColumnSort.None, 8, clsfields.GetFieldByID(new Guid("A389495A-DD51-4987-9C52-F89E83ADA31C")), false, false, false, false, false, false, false));
            columns.Add(new cStandardColumn(new Guid("B1FC8E2C-499E-4AD4-9E5D-7008E87FDC78"), reportid, ReportColumnType.Standard, ColumnSort.None, 9, clsfields.GetFieldByID(new Guid("B1FC8E2C-499E-4AD4-9E5D-7008E87FDC78")), false, false, false, false, false, false, false));
            ArrayList criteria = new ArrayList();
            if (element != null)
            {
                criteria.Add(new cReportCriterion(Guid.NewGuid(), reportid, clsfields.GetFieldByID(new Guid("b193bcf2-796e-4f4c-a926-db334aefb193")), ConditionType.Equals, new object[] { element }, null, ConditionJoiner.And, 1, false, 0));

            }
            if (action != null)
            {
                criteria.Add(new cReportCriterion(Guid.NewGuid(), reportid, clsfields.GetFieldByID(new Guid("f4f5e4f3-083c-4708-8d52-58824c102e0e")), ConditionType.Equals, new object[] { action }, null, ConditionJoiner.And, 2, false, 0));
            }


            criteria.Add(new cReportCriterion(Guid.NewGuid(), reportid, clsfields.GetFieldByID(new Guid("fb7462ad-7930-472a-b84a-2ccd34426878")), ConditionType.Between, new object[] { start.ToString("yyyy/MM/dd") + " 00:00:00" }, new object[] { end.ToString("yyyy/MM/dd") + " 23:59:59" }, ConditionJoiner.And, 3, false, 0));

            if (username != "")
            {
                criteria.Add(new cReportCriterion(Guid.NewGuid(), reportid, clsfields.GetFieldByID(new Guid("72bfbefa-7c5e-4027-a3f3-fdffb4c10f87")), ConditionType.Like, new object[] { username + "%" }, null, ConditionJoiner.And, 4, false, 0));
            }
            var clsreport = new cReport(user.AccountID, user.CurrentSubAccountId, reportid, user.EmployeeID, "Audit Log", "", Guid.Empty, clstables.GetTableByID(new Guid("BDDC9561-40EC-4F35-A5B1-2E63F1CA0ACD")), false, false, columns, criteria, 0, Modules.None);
            var clsrequest = new cReportRequest(user.AccountID, user.CurrentSubAccountId, requestnum, clsreport, ExportType.Excel, new cExportOptions(user.EmployeeID, reportid, true, true, true, new SortedList<Guid, int>(), null, new Guid(), FinancialApplication.CustomReport, ",", false, true, false), false, user.EmployeeID, AccessRoleLevel.AllData);
            System.Web.HttpContext.Current.Session["rpt" + clsreport.reportid] = clsreport;
            System.Web.HttpContext.Current.Session["request" + requestnum] = clsrequest;
            return requestnum;
        }

        [WebMethod(EnableSession = true)]
        public static bool clearAuditLog()
        {
            cAuditLog clsaudit = new cAuditLog();
            bool result= clsaudit.clearAuditLog();
            if(result)
            {
                CurrentUser user = cMisc.GetCurrentUser();
               clsaudit.NotificationOnAuditLogCleared(user);
            }
            return result;
        }

        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
