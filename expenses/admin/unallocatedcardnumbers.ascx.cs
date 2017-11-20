using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using expenses.Old_App_Code.admin;

using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    public partial class unallocatedcardnumbers : System.Web.UI.UserControl
    {

        #region properties
        public int statementid
        {
            get
            {
                if (ViewState["statementid"] == null)
                {
                    return 0;
                }
                return (int)ViewState["statementid"];
            }
            set { ViewState["statementid"] = value; }
        }
        public string provider
        {
            get
            {
                if (ViewState["provider"] == null)
                {
                    return "";
                }
                return (string)ViewState["provider"];
            }
            set { ViewState["provider"] = value; }
        }
        public int accountid
        {
            get
            {
                if (ViewState["accountid"] == null)
                {
                    return 0;
                }
                return (int)ViewState["accountid"];
            }
            set
            {
                ViewState["accountid"] = value;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gridunallocatednumbers.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridunallocatednumbers_InitializeDataSource);
        }

        void gridunallocatednumbers_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            if (ViewState["statementid"] != null)
            {
                cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
                cCardTemplate template = clstemplates.getTemplate(provider);
                if (template != null)
                {
                    cCardStatements clsstatements = new cCardStatements((int)ViewState["accountid"]);
                    gridunallocatednumbers.DataSource = clsstatements.getUnallocatedCardGrid(template, (int)ViewState["statementid"]);
                }
                else
                {
                    litMessage.Text = "<font style=\"color: red;\">ERROR - There was a problem obtaining the card template path. Please contact your administrator</font>";
                }
            }
        }

        protected void gridunallocatednumbers_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.FromKey("match") == null)
            {
                e.Layout.Bands[0].Columns.Add("match", "Match Number");
                
            }
            cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
            cCardTemplate template = clstemplates.getTemplate(provider);
            cCardTemplateField field;

            if (e.Layout.Bands[0].Columns.FromKey("CardNumber") != null)
            {
                e.Layout.Bands[0].Columns.FromKey("CardNumber").Header.Caption = "Card Number";
            }

            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn column in gridunallocatednumbers.Columns)
            {
                field = template.RecordTypes[CardRecordType.CardTransaction].getFieldByName(column.Key);
                if (field != null && !string.IsNullOrEmpty(field.label))
                {
                    column.Header.Caption = field.label;
                }
            }
        }

        protected void gridunallocatednumbers_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("match").Value = "<a href=\"javascript:match('" + e.Row.Cells.FromKey("CardNumber").Value + "');\">Match Number</a>";
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            string surname = txtsurname.Text;
            string username = txtusername.Text;
            cEmployees clsemployees = new cEmployees(accountid);

            gridemployees.DataSource = clsemployees.generateUnallocatedCardGrid(surname, accountid, 0, 0, 0, 0, 0, username, 0);
            gridemployees.DataBind();
            MultiView1.ActiveViewIndex = 1;
        }

        protected void gridemployees_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("employeeid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("archived").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("username").Header.Caption = "Username";
            e.Layout.Bands[0].Columns.FromKey("groupname").Header.Caption = "Group";
            e.Layout.Bands[0].Columns.FromKey("department").Header.Caption = "Department";
            e.Layout.Bands[0].Columns.FromKey("costcode").Header.Caption = "Cost Code";

            if (e.Layout.Bands[0].Columns.FromKey("select") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("select", "Select", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("select").Width = Unit.Pixel(15);
            }
        }

        protected void gridemployees_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("select").Value = "<input type=\"radio\" name=\"select\" value=\"" + e.Row.Cells.FromKey("employeeid").Value + "\" />";
        }

        protected void cmdallocate_Click(object sender, ImageClickEventArgs e)
        {
            int employeeid = Convert.ToInt32(Request.Form["select"]);
            if (employeeid == 0)
            {
                lblmsg.Text = "Please select an employee to assign this card to";
                lblmsg.Visible = true;
                return;
            }
            string cardnumber = txtcardnumber.Text;

            txtcardnumber.Text = "";

            cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
            cCardTemplate template = clstemplates.getTemplate(provider);
            if (template != null)
            {
                cCardStatements clsstatements = new cCardStatements(accountid);
                clsstatements.allocateCard(statementid, employeeid, cardnumber);
                gridunallocatednumbers.DataSource = clsstatements.getUnallocatedCardGrid(template, statementid);
                gridunallocatednumbers.DataBind();
                lblmsg.Visible = false;
                MultiView1.ActiveViewIndex = 0;
                modunallocated.Hide();
            }
            else
            {
                lblmsg.Text = "Unable to load the card template for the card provider";
                lblmsg.Visible = true;
            }
        }

        public void refresh()
        {
            cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
            cCardTemplate template = clstemplates.getTemplate(provider);

            cCardStatements clsstatements = new cCardStatements(accountid);
            
            gridunallocatednumbers.DataSource = clsstatements.getUnallocatedCardGrid(template, statementid);
            gridunallocatednumbers.DataBind();
        }

        public bool visible
        {
            set { gridemployees.Visible = value; }
            get { return gridemployees.Visible; }
        }

        protected void cmdallocatecancel_Click(object sender, ImageClickEventArgs e)
        {
            lblmsg.Visible = false;
            MultiView1.ActiveViewIndex = 0;
            modunallocated.Hide();
        }
    }
}
