using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ExpensesLibrary;
using System.Web.Services;
using expenses.Old_App_Code;

namespace expenses.admin
{
    public partial class adminExpensesConnectUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "expensesConnect Users";
            Master.title = Title;
            int noOfUsedLicenses = 0;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
                ViewState["accountid"] = user.accountid;
                ViewState["employeeid"] = user.employeeid;
            }

            cAccounts clsAccs = new cAccounts();

            cAccount account = clsAccs.getAccountById((int)ViewState["accountid"]);
            int noOfLicenses = account.expensesConnectLicenses;

            cEmployees clsEmps = new cEmployees((int)ViewState["accountid"]);

            noOfUsedLicenses = clsEmps.getExpensesConnectUserCount();

            if (IsPostBack == false)
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "noOfUsedLicenses", "var noOfUsedLicenses = " + noOfUsedLicenses + ";", true);
            }
            
            lblNoOfLicenses.Text = noOfUsedLicenses.ToString();
            lblNoOfLicensesDesc1.Text = "You currently have ";
            lblNoOfLicensesDesc2.Text = "out of " + noOfLicenses.ToString() + " licenses allocated.";

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "noOfLicenses", "var noOfLicenses = " + noOfLicenses + ";", true);
            }

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridExpensesConnectUsers.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridExpensesConnectUsers_InitializeDataSource);
        }

        void gridExpensesConnectUsers_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            gridExpensesConnectUsers.DataSource = getExpensesConnectUsersDataSet();
        }

        protected void gridExpensesConnectUsers_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            
            e.Layout.Bands[0].Columns.FromKey("employeeid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("employeename").Header.Caption = "Employee Name";
            e.Layout.Bands[0].Columns.FromKey("employeeusername").Header.Caption = "Username";

            if (e.Layout.Bands[0].Columns.FromKey("delete") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
            }
        }

        protected void gridExpensesConnectUsers_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteUser(" + e.Row.Cells.FromKey("employeeid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
        }

        public DataSet getExpensesConnectUsersDataSet()
        {
            object[] values;
            DataSet ds = new DataSet();
            DataTable table = new DataTable();

            table.Columns.Add(new DataColumn("employeeid", typeof(System.Int32)));
            table.Columns.Add(new DataColumn("employeename", typeof(System.String)));
            table.Columns.Add(new DataColumn("employeeusername", typeof(System.String)));

            cEmployees clsEmps = new cEmployees((int)ViewState["accountid"]);
            List<cEmployee> lstEmps = clsEmps.getExpensesConnectUsers();

            foreach (cEmployee emp in lstEmps)
            {
                //deleteButton = "<img src=\"../icons/delete2.gif\" height=\"16\" width=\"16\" alt=\"Delete\" title=\"Delete\" onclick=\"deleteEmailTemplate(this," + email.emailtemplateid + ");\" />";
                values = new object[3];
                values[0] = emp.employeeid;
                values[1] = emp.surname + ", " + emp.firstname;
                values[2] = emp.username;

                table.Rows.Add(values);
            }

            ds.Tables.Add(table);
            return ds;
        }

        protected void cmdadduser_Click(object sender, ImageClickEventArgs e)
        {
            cEmployees clsEmps = new cEmployees((int)ViewState["accountid"]);
            string[] splitval = txtsurname.Text.Split('[');
            string username = splitval[1].Replace("]", "");

            int employeeid = clsEmps.getEmployeeidByUsername((int)ViewState["accountid"], username);
            clsEmps.saveExpensesConnectUser(employeeid, true);
            txtsurname.Text = "";
            gridExpensesConnectUsers.DataSource = getExpensesConnectUsersDataSet();
            gridExpensesConnectUsers.DataBind();

            int noOfUsedLicenses = clsEmps.getExpensesConnectUserCount();
            lblNoOfLicenses.Text = noOfUsedLicenses.ToString();

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "noOfUsedLicenses", "var noOfUsedLicenses = " + noOfUsedLicenses + ";", true);
        
        }

        [WebMethod]
        public static void deleteEmployeeLicense(int accountid, int employeeid)
        {
            cEmployees clsEmps = new cEmployees(accountid);
            clsEmps.saveExpensesConnectUser(employeeid, false);
        }

        [WebMethod]
        public static bool checkLicenseExists(int accountid, string userdetails)
        {
            cEmployees clsEmps = new cEmployees(accountid);
            List<cEmployee> lstEmps = clsEmps.getExpensesConnectUsers();

            string[] splitval = userdetails.Split('[');
            string username = splitval[1].Replace("]", "");
            int employeeid = clsEmps.getEmployeeidByUsername(accountid, username);
            bool userHasLicense = false;

            foreach (cEmployee emp in lstEmps)
            {
                if (emp.employeeid == employeeid)
                {
                    userHasLicense = true;
                    break;
                }
            }
            return userHasLicense;
        }

    }
}
